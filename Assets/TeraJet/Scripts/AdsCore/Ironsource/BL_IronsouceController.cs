using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using TeraJet;
using Cysharp.Threading.Tasks;

#if BL_IRONSOURCE
public class BL_IronsouceController : AdController
{
    IronsourceConfigData _configData;

    int _retryAttemptInter, _retryAttemptReward;

    CancellationTokenSource _disposeCancelationSource;
    private bool _isFinishedWatchingAds;
    private bool _isBannerLoaded;

    public BL_IronsouceController(IronsourceConfigData configData, CancellationTokenSource cancellation)
    {
        _configData = configData;
        _disposeCancelationSource = cancellation;
    }

    public override void Init()
    {
        //Dynamic config example
        IronSourceConfig.Instance.setClientSideCallbacks(true);

        string id = IronSource.Agent.getAdvertiserId();
        Debug.Log("unity-script: IronSource.Agent.getAdvertiserId : " + id);

        Debug.Log("unity-script: IronSource.Agent.validateIntegration");
        IronSource.Agent.validateIntegration();

        Debug.Log("unity-script: unity version" + IronSource.unityVersion());

        #region register event
        InitializeInterstitialAds();
        InitializeRewardedAds();
        InitializeBannerAds();
        IronSourceEvents.onImpressionDataReadyEvent += ImpressionDataReadyEvent;
        IronSourceEvents.onSdkInitializationCompletedEvent += OnSDKInitComplete;
        #endregion

        // SDK init
        Debug.Log("unity-script: IronSource.Agent.init");
        IronSource.Agent.init(_configData.sdkKeyAndroid);



    }

    private void ImpressionDataReadyEvent(IronSourceImpressionData impressionData)
    {
#if BL_FIREBASE
        FirebaseManager.SendImpressionSuccessEvent(impressionData);
#endif
    }

    private void OnSDKInitComplete()
    {
        onMediationInitComplete?.Invoke();
        LoadInterstitial();
        //IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
    }




    #region Interstitial Ads
    public void InitializeInterstitialAds()
    {
        // Attach callback
        IronSourceInterstitialEvents.onAdReadyEvent += InterstitialAdReadyEvent;
        IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
        IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialOnAdShowSucceededEvent;
        IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialOnAdShowFailedEvent;
        IronSourceInterstitialEvents.onAdClickedEvent += InterstitialOnAdClickedEvent;
        IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialOnAdOpenedEvent;
        IronSourceInterstitialEvents.onAdClosedEvent += InterstitialOnAdClosedEvent;
    }

    private void LoadInterstitial()
    {
        IronSource.Agent.loadInterstitial();
    }

    private void InterstitialAdReadyEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("INTER ADS IS ALREADY: " + IronSource.Agent.isInterstitialReady());
        // Reset retry attempt
        _retryAttemptInter = 0;

#if BL_APPSFLYER
        AppsflyerManager.OnInterAdCalled();
#endif
    }

    private void InterstitialAdLoadFailedEvent(IronSourceError error)
    {
        // Interstitial ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)
        Debug.Log("LogEventAdsInterFail : " + GetInternetStatus() + " " + error.getErrorCode() + " " + error.getDescription());
        _retryAttemptInter++;
        double retryDelay = Math.Pow(2, Math.Min(3, _retryAttemptInter));

#if BL_FIREBASE
        FirebaseManager.instance?.OnAdsInterLoadedFail(GetInternetStatus() + " " + error.getErrorCode() + " " + error.getDescription());
#endif

        ReloadInterAfterFail(retryDelay).Forget();
    }

    async UniTaskVoid ReloadInterAfterFail(double delaySecs)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delaySecs), true, cancellationToken: _disposeCancelationSource.Token);
        LoadInterstitial();
    }

    private void InterstitialOnAdOpenedEvent(IronSourceAdInfo adInfo)
    {
        _isShowingNormalAd = true;
#if BL_APPSFLYER
        AppsflyerManager.OnInterAdDisplayed();
#endif
#if BL_FIREBASE
        FirebaseManager.instance?.OnAdsInterShow();
#endif
        Debug.Log("unity-script: I got InterstitialAdOpenedEvent");
        //Time.timeScale = 0;
    }

    void InterstitialOnAdShowSucceededEvent(IronSourceAdInfo adInfo)
    {
    }

    private void InterstitialOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo)
    {
        _retryAttemptInter++;
        double retryDelay = Math.Pow(2, Math.Min(3, _retryAttemptInter));

        ReloadInterAfterFail(retryDelay).Forget();
        StartAction(() => _isShowingNormalAd = false, 3f).Forget();

#if BL_FIREBASE
        FirebaseManager.instance?.OnAdsInterFail(GetInternetStatus() + " " + error.getErrorCode() + " " + error.getDescription());
#endif

        Debug.Log("LogEventAdsInterFail : " + " " + error.getErrorCode() + " " + error.getDescription());
    }

    private void InterstitialOnAdClickedEvent(IronSourceAdInfo adInfo) { }

    private void InterstitialOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        StartAction(() => _isShowingNormalAd = false, 3f).Forget();
        // Interstitial ad is hidden. Pre-load the next ad.
        LoadInterstitial();
    }
    public override void ShowInterstitial(string placement = "")
    {
#if BL_APPSFLYER
        AppsflyerManager.OnInterAdEligible();
#endif

        if (IronSource.Agent.isInterstitialReady() && !_isShowingNormalAd)
        {
            IronSource.Agent.showInterstitial();
            Debug.Log("---------SHOW INTER ADS ------------");
        }
    }
    #endregion

    #region Rewarded ads


    public void InitializeRewardedAds()
    {
        // Attach callback
        IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoAvailable;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoUnavailable;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoAdClickedEvent;
        IronSourceRewardedVideoEvents.onAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;

    }


    private void OnRewardedAdLoadFailedEvent(IronSourceError errorInfo)
    {
        // Rewarded ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).
        string placementID = _currentAdRewardPlacement;
#if BL_FIREBASE
        FirebaseManager.instance?.OnAdsRewardLoadedFail(GetInternetStatus() + " " + errorInfo.getDescription());
#endif
        //Debug.Log("LogEventAdsRewardFail : " + placementID + "---" + errorInfo.getDescription() + " " + errorInfo.getErrorCode());
        //_retryAttemptReward++;
        //double retryDelay = Math.Pow(2, Math.Min(6, _retryAttemptReward));

        //ReloadRewardAfterFail(retryDelay).Forget();
        //onRewardAdLoadedStateChange?.Invoke(false);
    }

    void RewardedVideoAvailable(IronSourceAdInfo adInfo)
    {
        _retryAttemptReward = 0;

#if BL_APPSFLYER
        AppsflyerManager.OnRewardAdCalled();
#endif
        onRewardAdLoadedStateChange?.Invoke(true);
    }

    void RewardedVideoUnavailable()
    {
        onRewardAdLoadedStateChange?.Invoke(true);
    }

    //async UniTaskVoid ReloadRewardAfterFail(double delay)
    //{
    //    await UniTask.Delay(TimeSpan.FromSeconds(delay), true, cancellationToken: _disposeCancelationSource.Token);
    //    LoadRewardedAd();
    //}

    private void RewardedVideoAdOpenedEvent(IronSourceAdInfo adInfo)
    {
#if BL_APPSFLYER
        AppsflyerManager.OnRewardAdDisplayed();
#endif
#if BL_FIREBASE
        FirebaseManager.instance?.OnAdsRewardShow(_currentAdRewardPlacement);
#endif
        _isShowingNormalAd = true;
        //Time.timeScale = 0;
    }

    private void RewardedVideoAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo)
    {
        //Time.timeScale = 1;
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        StartAction(() => _isShowingNormalAd = false, 3f).Forget();
        string placementID = _currentAdRewardPlacement;
#if BL_FIREBASE
        FirebaseManager.instance?.OnAdsRewardFail(placementID, GetInternetStatus() + " " + error.getErrorCode() + " " + error.getDescription());
#endif
        Debug.Log("LogEventAdsRewardFail : " + placementID + "---" + GetInternetStatus() + " " + error.getErrorCode() + " " + error.getDescription());
        //LoadRewardedAd();
        onRewardAdLoadedStateChange?.Invoke(IsRewardAdsReady());

    }

    private void RewardedVideoAdClickedEvent(IronSourcePlacement ssp, IronSourceAdInfo adInfo)
    {

    }

    private void RewardedVideoAdClosedEvent(IronSourceAdInfo adInfo)
    {
        //Time.timeScale = 1;
        // Rewarded ad is hidden. Pre-load the next ad
        StartAction(() => _isShowingNormalAd = false, 3f).Forget();
        if (_isFinishedWatchingAds)
        {
            _rewardedSuccessCallback?.Invoke();
        }
        else
        {
            _rewardFailedCallback?.Invoke();
        }
        //LoadRewardedAd();
        onRewardAdLoadedStateChange?.Invoke(IsRewardAdsReady());
    }

    private void RewardedVideoAdRewardedEvent(IronSourcePlacement ssp, IronSourceAdInfo adInfo)
    {
        // The rewarded ad displayed and the user should receive the reward.
        _isFinishedWatchingAds = true;
#if BL_APPSFLYER
        AppsflyerManager.OnRewardAdCompleted();
#endif
        Debug.Log("unity-script: I got RewardedVideoAdRewardedEvent");
    }

    public override void ShowRewardedAd(UnityAction successCallback, UnityAction failCallback, string adPlacement = "")
    {
#if BL_APPSFLYER
        AppsflyerManager.OnRewardAdEligible();
#endif
        _currentAdRewardPlacement = adPlacement;
        if (IsRewardAdsReady())
        {
            _isFinishedWatchingAds = false;
            IronSource.Agent.showRewardedVideo();

            _rewardedSuccessCallback = successCallback;
            _rewardFailedCallback = failCallback;
        }
        else
        {
            Debug.Log("Show Reward Ads Failed : ads no ready");
            onRewardAdLoadedStateChange?.Invoke(IsRewardAdsReady());
        }
    }
    public override bool IsRewardAdsReady()
    {
        return IronSource.Agent.isRewardedVideoAvailable();
    }
    public override bool IsInterAdsReady()
    {
        return IronSource.Agent.isInterstitialReady();
    }
    #endregion


    #region Banner Ads

    public void InitializeBannerAds()
    {
        IronSourceBannerEvents.onAdLoadedEvent += BannerAdLoadedEvent;
        IronSourceBannerEvents.onAdLoadFailedEvent += BannerAdLoadFailedEvent;
        IronSourceBannerEvents.onAdClickedEvent += BannerAdClickedEvent;
        IronSourceBannerEvents.onAdScreenPresentedEvent += BannerAdScreenPresentedEvent;
        IronSourceBannerEvents.onAdScreenDismissedEvent += BannerAdScreenDismissedEvent;
        IronSourceBannerEvents.onAdLeftApplicationEvent += BannerAdLeftApplicationEvent;


#if !UNITY_EDITOR
        //ShowBanner();
#endif

    }
    public override void ShowBanner()
    {
        if (!_isBannerLoaded)
        {
            IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
        }
        else
        {
            IronSource.Agent.displayBanner();
        }
        
    }
    public override void HideBanner()
    {
        IronSource.Agent.hideBanner();
    }
    void BannerAdLoadedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerAdLoadedEvent");
        _isBannerLoaded = true;
    }
    void BannerAdLoadFailedEvent(IronSourceError error)
    {
        Debug.Log("unity-script: I got BannerAdLoadFailedEvent, code: " + error.getCode() + ", description : " + error.getDescription());
    }

    void BannerAdClickedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerAdClickedEvent");
    }

    void BannerAdScreenPresentedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerAdScreenPresentedEvent");
    }

    void BannerAdScreenDismissedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerAdScreenDismissedEvent");
    }

    void BannerAdLeftApplicationEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerAdLeftApplicationEvent");
    }
    #endregion

    string GetInternetStatus()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return "NoInternet";
        }
        else
        {
            return "ReachableInternet";
        }
    }

    async UniTaskVoid StartAction(Action action, double delay)
    {
        await UniTask.SwitchToMainThread(_disposeCancelationSource.Token);
        await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: _disposeCancelationSource.Token);

        action?.Invoke();
    }
}
#endif
