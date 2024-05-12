using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using TeraJet;
using Cysharp.Threading.Tasks;

#if BL_MAX_SDK

public class BL_MaxController :AdController
{
    private MaxConfigData _configData;

    int _retryAttemptInter, _retryAttemptReward;

    private bool _isFinishedWatchingAds = false;

    CancellationTokenSource _disposeCancelationSource;

    private bool _isDebug;

    public BL_MaxController(MaxConfigData configData, CancellationTokenSource disposeSource, bool isDebug)
    {
        _configData = configData;
        _disposeCancelationSource = disposeSource;
        _isDebug = isDebug;
    }

    public override void Init()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {

            InitializeInterstitialAds();
            InitializeRewardedAds();
            InitializeBannerAds();
            InitializeAppOpenAds();
            InitializeMRecAds();

            _isInit = true;


            // Show Mediation Debugger
            if (_isDebug)
            {
                MaxSdk.ShowMediationDebugger();
            }
            // AppLovin SDK is initialized, start loading ads
        };

        MaxSdk.SetSdkKey(_configData.sdkKey);
        MaxSdk.InitializeSdk();
    }
    void InitializeAppOpenAds()
    {
        MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAppOpenDismissedEvent;

        MaxSdk.LoadAppOpenAd(_configData.appOpenAdUnitId);
    }

    public void OnAppOpenDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        MaxSdk.LoadAppOpenAd(_configData.appOpenAdUnitId);
        StartAction(() => _isShowingNormalAd = false, 3f).Forget();
    }

    public void ShowOAAIfReady()
    {
        if (MaxSdk.IsAppOpenAdReady(_configData.appOpenAdUnitId))
        {
            MaxSdk.ShowAppOpenAd(_configData.appOpenAdUnitId);
        }
        else
        {
            MaxSdk.LoadAppOpenAd(_configData.appOpenAdUnitId);
        }
    }

    public bool IsOOAReady()
    {
        return MaxSdk.IsAppOpenAdReady(_configData.appOpenAdUnitId);
    }

    #region Interstitial Ads
    public void InitializeInterstitialAds()
    {
        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialRevenuePaidEvent;

        // Load the first interstitial
        LoadInterstitial();
    }

    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(_configData.interAdUnitId);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

        // Reset retry attempt
        _retryAttemptInter = 0;
        Debug.Log("INTER ADS IS ALREADY: " + MaxSdk.IsInterstitialReady(adUnitId) + " " + adInfo.WaterfallInfo);

#if BL_APPSFLYER
        AppsflyerManager.OnInterAdCalled();
#endif
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)
        Debug.Log("LogEventAdsInterFail : " + errorInfo.AdLoadFailureInfo + " " + errorInfo.WaterfallInfo.ToString());
        _retryAttemptInter++;
        double retryDelay = Math.Pow(2, Math.Min(6, _retryAttemptInter));

#if BL_FIREBASE
        FirebaseManager.instance?.OnAdsInterLoadedFail(GetInternetStatus() + " " + errorInfo.AdLoadFailureInfo);
#endif

        ReloadInterAfterFail(retryDelay).Forget();
    }

    async UniTaskVoid ReloadInterAfterFail(double delaySecs)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delaySecs), true, cancellationToken: _disposeCancelationSource.Token);
        LoadInterstitial();
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
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
    private void OnInterstitialRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad revenue paid. Use this callback to track user revenue.
        Debug.Log("Interstitial revenue paid");

        // Ad revenue
        double revenue = adInfo.Revenue;

        // Miscellaneous data
        string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!

#if BL_FIREBASE
        FirebaseManager.OnAdRevenuePaidEvent(adInfo);
#endif
    }
    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        StartAction(() => _isShowingNormalAd = false, 3f).Forget();
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        Debug.Log("LogEventAdsInterFail : " + adInfo.NetworkName + errorInfo.MediatedNetworkErrorMessage + " " + errorInfo.MediatedNetworkErrorCode + " " + errorInfo.WaterfallInfo.ToString());
        Debug.Log("LogEventAdsInterFail WaterFall : " + errorInfo.WaterfallInfo.Name + " " + errorInfo.WaterfallInfo.NetworkResponses);
        //Time.timeScale = 1;
        LoadInterstitial();

#if BL_FIREBASE
        FirebaseManager.instance?.OnAdsInterFail(GetInternetStatus() + " " + adInfo.NetworkName + " " + errorInfo.MediatedNetworkErrorMessage);
#endif
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
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

        if (MaxSdk.IsInterstitialReady(_configData.interAdUnitId))
        {
            MaxSdk.ShowInterstitial(_configData.interAdUnitId);
            Debug.Log("---------SHOW INTER ADS ------------");
        }
    }
#endregion

    #region Rewarded ads


    public void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        // Load the first rewarded ad
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(_configData.rewardAdUnitId);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {     
        _retryAttemptReward = 0;

#if BL_APPSFLYER
        AppsflyerManager.OnRewardAdCalled();
#endif
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).
        string placementID = _currentAdRewardPlacement;
#if BL_FIREBASE
        FirebaseManager.instance?.OnAdsRewardLoadedFail(GetInternetStatus() + " " + errorInfo.AdLoadFailureInfo);
#endif
        Debug.Log("LogEventAdsRewardFail : " + placementID + "---" + errorInfo.AdLoadFailureInfo + " " + errorInfo.WaterfallInfo.ToString());
        _retryAttemptReward++;
        double retryDelay = Math.Pow(2, Math.Min(6, _retryAttemptReward));

        ReloadRewardAfterFail(retryDelay).Forget();
        onRewardAdLoadedStateChange?.Invoke(false);
    }

    async UniTaskVoid ReloadRewardAfterFail(double delay)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay), true,cancellationToken: _disposeCancelationSource.Token);
        LoadRewardedAd();
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
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

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        //Time.timeScale = 1;
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        StartAction(() => _isShowingNormalAd = false, 3f).Forget();
        string placementID = _currentAdRewardPlacement;
#if BL_FIREBASE
        FirebaseManager.instance?.OnAdsRewardFail(GetInternetStatus() + " " + adInfo.NetworkName + " " + placementID, GetInternetStatus() + " " + errorInfo.MediatedNetworkErrorMessage + " " + errorInfo.MediatedNetworkErrorCode);
#endif
        Debug.Log("LogEventAdsRewardFail : " + placementID + "---" + errorInfo.AdLoadFailureInfo + " " + errorInfo.WaterfallInfo.ToString());
        Debug.Log("unity-script: I got RewardedVideoAdShowFailedEvent, code :  " + errorInfo.Code +
          ", description : " + errorInfo.AdLoadFailureInfo);
        LoadRewardedAd();
        onRewardAdLoadedStateChange?.Invoke(IsRewardAdsReady());

    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {

    }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
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
        LoadRewardedAd();
        onRewardAdLoadedStateChange?.Invoke(IsRewardAdsReady());
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // The rewarded ad displayed and the user should receive the reward.
        _isFinishedWatchingAds = true;
#if BL_APPSFLYER
        AppsflyerManager.OnRewardAdCompleted();
#endif
        Debug.Log("unity-script: I got RewardedVideoAdRewardedEvent");
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Ad revenue paid. Use this callback to track user revenue.
        // Rewarded ad revenue paid. Use this callback to track user revenue.
        Debug.Log("Rewarded ad revenue paid");

        // Ad revenue
#if BL_FIREBASE
        FirebaseManager.OnAdRevenuePaidEvent(adInfo);
#endif
    }
    public override void ShowRewardedAd(UnityAction successCallback, UnityAction failCallback, string adPlacement = "")
    {
#if BL_APPSFLYER
        AppsflyerManager.OnRewardAdEligible();
#endif
        _currentAdRewardPlacement = adPlacement;
        if (MaxSdk.IsRewardedAdReady(_configData.rewardAdUnitId))
        {

            MaxSdk.ShowRewardedAd(_configData.rewardAdUnitId);
            _isFinishedWatchingAds = false;

            _rewardedSuccessCallback = successCallback;
            _rewardFailedCallback = failCallback;

#if BL_APPSFLYER
            //AppsflyerManager.FireCustomEvent("RewardAds" + adPlacement);
#endif
        }
        else
        {
            Debug.Log("Show Reward Ads Failed : ads no ready");
            onRewardAdLoadedStateChange?.Invoke(IsRewardAdsReady());
        }
    }
    public override bool IsRewardAdsReady()
    {
        return MaxSdk.IsRewardedAdReady(_configData.rewardAdUnitId);
    }
    public override bool IsInterAdsReady()
    {
        return MaxSdk.IsInterstitialReady(_configData.interAdUnitId);
    }
#endregion


    #region Banner Ads

    public void InitializeBannerAds()
    {
        // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
        // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
        MaxSdk.CreateBanner(_configData.bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
        //MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.TopCenter);
        // Set background or background color for banners to be fully functional
        MaxSdk.SetBannerBackgroundColor(_configData.bannerAdUnitId, new Color(1,1,1,0));

        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;


#if !UNITY_EDITOR
        //ShowBanner();
#endif

    }
    public override void ShowBanner()
    {
        MaxSdk.ShowBanner(_configData.bannerAdUnitId);
    }
    public override void HideBanner()
    {
        MaxSdk.HideBanner(_configData.bannerAdUnitId);
    }
    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Bannerer IS loaded");
    }

    private void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        Debug.Log("Bannerer IS loaded Faild");
    }

    private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
#if BL_FIREBASE
        FirebaseManager.OnAdRevenuePaidEvent(adInfo);
#endif
    }

    private void OnBannerAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
    #endregion

    #region Mrec
    public void InitializeMRecAds()
    {
        // MRECs are sized to 300x250 on phones and tablets
        MaxSdk.CreateMRec(_configData.mrecAdUnitId, MaxSdkBase.AdViewPosition.BottomCenter);
        MaxSdkCallbacks.MRec.OnAdLoadedEvent += OnMRecAdLoadedEvent;
        MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += OnMRecAdLoadFailedEvent;
        MaxSdkCallbacks.MRec.OnAdClickedEvent += OnMRecAdClickedEvent;
        MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnMRecAdRevenuePaidEvent;
        MaxSdkCallbacks.MRec.OnAdExpandedEvent += OnMRecAdExpandedEvent;
        MaxSdkCallbacks.MRec.OnAdCollapsedEvent += OnMRecAdCollapsedEvent;
    }

    public void OnMRecAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    public void OnMRecAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo error) { }

    public void OnMRecAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    public void OnMRecAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
#if BL_FIREBASE
        FirebaseManager.OnAdRevenuePaidEvent(adInfo);
#endif
    }

    public void OnMRecAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    public void OnMRecAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    public void ShowMrec()
    {
        MaxSdk.ShowMRec(_configData.mrecAdUnitId);
    }
    public void HideMrec()
    {
        MaxSdk.HideMRec(_configData.mrecAdUnitId);
    }
    public void SetMrecPosition(float dpiX, float dpiY)
    {
        MaxSdk.UpdateMRecPosition(_configData.mrecAdUnitId, dpiX, dpiY);
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
