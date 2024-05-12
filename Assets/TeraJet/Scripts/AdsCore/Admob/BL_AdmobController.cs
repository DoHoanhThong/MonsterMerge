using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Threading;

#if BL_ADMOB
public class BL_AdmobController : AdController
{
    AdmobConfigData _configData;

    private AppOpenAd _openAppAd;
    private BannerView _bannerView;
    private BannerView _bannerMrec;
    private InterstitialAd _interstitialAd;
    private RewardedAd _rewardedAd;

    int _retryAttemptInter, _retryAttemptReward, _retryAttemptMrec;

    bool _isShowingOAA;

    CancellationTokenSource _disposeCancelSource;

    public BL_AdmobController(AdmobConfigData configData, CancellationTokenSource cancellation)
    {
        _configData = configData;
        _disposeCancelSource = cancellation;
    }

    public override void Init()
    {
        MobileAds.SetiOSAppPauseOnBackground(true);

        List<String> deviceIds = new List<String>() { AdRequest.TestDeviceSimulator };

        RequestConfiguration requestConfiguration =
           new RequestConfiguration.Builder()
           .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.Unspecified)
           .SetTestDeviceIds(deviceIds).build();
        MobileAds.SetRequestConfiguration(requestConfiguration);

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(HandleInitCompleteAction);
    }

    private void HandleInitCompleteAction(InitializationStatus initstatus)
    {
        Debug.Log("Initialization complete.");

        // Callbacks from GoogleMobileAds are not guaranteed to be called on
        // the main thread.
        // In this example we use MobileAdsEventExecutor to schedule these calls on
        // the next Update() loop.
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            //  LoadOpenAppAd();
            RequestMrec();
            Debug.Log("Google Mobile Ads Init Complete");
        });
    }

    private AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder().Build();
    }

    #region BANNER ADS

    public void RequestBannerAd()
    {

        // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
        string adUnitId = "3221671f11f59a43";
#elif UNITY_ANDROID
        string adUnitId = "3221671f11f59a43";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Clean up banner before reusing
        if (_bannerView != null)
        {
            _bannerView.Destroy();
        }

        // Create a 320x50 banner at top of the screen
        _bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

        // Add Event Handlers
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner ad loaded.");
            //OnAdLoadedEvent.Invoke();
        };
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.Log("Banner ad failed to load with error: " + error.GetMessage());
            //OnAdFailedToLoadEvent.Invoke();
        };
        _bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner ad recorded an impression.");
        };
        _bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner ad recorded a click.");
        };
        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner ad opening.");
            //OnAdOpeningEvent.Invoke();
        };
        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner ad closed.");
            //OnAdClosedEvent.Invoke();
        };
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            string msg = string.Format("{0} (currency: {1}, value: {2}",
                                        "Banner ad received a paid event.",
                                        adValue.CurrencyCode,
                                        adValue.Value);
            Debug.Log(msg);
        };

        // Load a banner ad
        _bannerView.LoadAd(CreateAdRequest());
    }

    public void DestroyBannerAd()
    {
        if (_bannerView != null)
        {
            _bannerView.Destroy();
        }
    }

    #endregion

    #region INTERSTITIAL ADS

    public void RequestAndLoadInterstitialAd()
    {
        Debug.Log("Requesting Interstitial ad.");

        // Clean up interstitial before using it
        if (_interstitialAd != null)
        {
            _interstitialAd.OnAdFullScreenContentOpened -= OnInterFullScreenContentOpened;
            _interstitialAd.OnAdFullScreenContentClosed -= OnInterFullScreenContentClose;
            _interstitialAd.OnAdImpressionRecorded -= OnInterImpressionRecorded;
            _interstitialAd.OnAdFullScreenContentFailed -= OnInterFullScreenContentFailed;
            _interstitialAd.OnAdPaid -= OnInterPaid;
            _interstitialAd.Destroy();
        }

        _interstitialAd = null;

        // Load an interstitial ad
        InterstitialAd.Load("", CreateAdRequest(),
            (InterstitialAd ad, LoadAdError loadError) =>
            {
                if (loadError != null)
                {
                    Debug.Log("Admob Interstitial ad failed to load with error: " +
                        loadError.GetMessage());

                    _retryAttemptInter++;
                    double retryDelay = Math.Pow(2, Math.Min(6, _retryAttemptInter));
                    ReloadInterAfterFail(retryDelay).Forget();

                    return;
                }
                else if (ad == null)
                {
                    Debug.Log("Admob Interstitial ad failed to load.");

                    _retryAttemptInter++;
                    double retryDelay = Math.Pow(2, Math.Min(6, _retryAttemptInter));
                    ReloadInterAfterFail(retryDelay).Forget();

                    return;
                }

                Debug.Log("Admob Interstitial ad loaded.");
                _retryAttemptInter = 0;

                _interstitialAd = ad;

                _interstitialAd.OnAdFullScreenContentOpened += OnInterFullScreenContentOpened;
                _interstitialAd.OnAdFullScreenContentClosed += OnInterFullScreenContentClose;
                _interstitialAd.OnAdImpressionRecorded += OnInterImpressionRecorded;
                _interstitialAd.OnAdFullScreenContentFailed += OnInterFullScreenContentFailed;
                _interstitialAd.OnAdPaid += OnInterPaid;
            });
    }

    async UniTaskVoid ReloadInterAfterFail(double delaySecs)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delaySecs), true, cancellationToken: _disposeCancelSource.Token);
        RequestAndLoadInterstitialAd();
    }

    void OnInterFullScreenContentOpened()
    {
        Debug.Log("Admob Interstitial ad opening.");
    }
    void OnInterFullScreenContentClose()
    {
        Debug.Log("Admob Interstitial ad closed.");
    }

    void OnInterImpressionRecorded()
    {
        Debug.Log("Admob Interstitial ad recorded an impression.");
    }

    void OnInterFullScreenContentFailed(AdError error)
    {
        Debug.Log("Admob Interstitial ad failed to show with error: " +
                                error.GetMessage());
    }

    void OnInterPaid(AdValue adValue)
    {
        string msg = string.Format("{0} (currency: {1}, value: {2}",
                                               "Admob Interstitial ad received a paid event.",
                                               adValue.CurrencyCode,
                                               adValue.Value);
        Debug.Log(msg);
    }

    #endregion

    #region REWARD ADS

    public void RequestAndLoadRewardAd()
    {
        Debug.Log("Requesting Reward ad.");

#if UNITY_EDITOR
        string adUnitId = "8e7ad47908cae831";
#elif UNITY_ANDROID
        string adUnitId
        = "8e7ad47908cae831";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Clean up interstitial before using it
        if (_rewardedAd != null)
        {
            _rewardedAd.OnAdFullScreenContentOpened -= OnRewardFullScreenContentOpened;
            _rewardedAd.OnAdFullScreenContentClosed -= OnRewardFullScreenContentClose;
            _rewardedAd.OnAdImpressionRecorded -= OnRewardImpressionRecorded;
            _rewardedAd.OnAdFullScreenContentFailed -= OnRewardFullScreenContentFailed;
            _rewardedAd.OnAdPaid -= OnRewardPaid;
            _rewardedAd.Destroy();
        }

        _rewardedAd = null;

        // Load an interstitial ad
        RewardedAd.Load(adUnitId, CreateAdRequest(),
            (RewardedAd ad, LoadAdError loadError) =>
            {
                if (loadError != null)
                {
                    Debug.Log("Admob Interstitial ad failed to load with error: " +
                        loadError.GetMessage());

                    _retryAttemptReward++;
                    double retryDelay = Math.Pow(2, Math.Min(6, _retryAttemptReward));
                    ReloadRewardAfterFail(retryDelay).Forget();

                    return;
                }
                else if (ad == null)
                {
                    Debug.Log("Admob Interstitial ad failed to load.");

                    _retryAttemptReward++;
                    double retryDelay = Math.Pow(2, Math.Min(6, _retryAttemptReward));
                    ReloadRewardAfterFail(retryDelay).Forget();

                    return;
                }

                Debug.Log("Admob Interstitial ad loaded.");
                _retryAttemptReward = 0;

                _rewardedAd = ad;

                _rewardedAd.OnAdFullScreenContentOpened += OnRewardFullScreenContentOpened;
                _rewardedAd.OnAdFullScreenContentClosed += OnRewardFullScreenContentClose;
                _rewardedAd.OnAdImpressionRecorded += OnRewardImpressionRecorded;
                _rewardedAd.OnAdFullScreenContentFailed += OnRewardFullScreenContentFailed;
                _rewardedAd.OnAdPaid += OnRewardPaid;
            });
    }

    async UniTaskVoid ReloadRewardAfterFail(double delaySecs)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delaySecs), true, cancellationToken: _disposeCancelSource.Token);
        RequestAndLoadRewardAd();
    }

    void OnRewardFullScreenContentOpened()
    {
        Debug.Log("Admob Reward ad opening.");
    }
    void OnRewardFullScreenContentClose()
    {
        Debug.Log("Admob Reward ad closed.");
    }

    void OnRewardImpressionRecorded()
    {
        Debug.Log("Admob Reward ad recorded an impression.");
    }

    void OnRewardFullScreenContentFailed(AdError error)
    {
        Debug.Log("Admob Reward ad failed to show with error: " +
                                error.GetMessage());
    }

    void OnRewardPaid(AdValue adValue)
    {
        string msg = string.Format("{0} (currency: {1}, value: {2}",
                                               "Admob Reward ad received a paid event.",
                                               adValue.CurrencyCode,
                                               adValue.Value);
        Debug.Log(msg);
    }

    #endregion

    #region Banner Mrec
    public void RequestMrec()
    {
        AdRequest request = new AdRequest.Builder().Build();
        _bannerMrec = new BannerView(_configData.mrecID, AdSize.MediumRectangle, AdPosition.Center);
        _bannerMrec.OnBannerAdLoaded += OnMrecLoaded;
        _bannerMrec.OnBannerAdLoadFailed += OnMrecLoadedFailed;
        _bannerMrec.LoadAd(request);
    }

    void OnMrecLoaded()
    {
        Debug.Log("Mrec complete");
        _retryAttemptMrec = 0;
        HideMrec();
    }
    void OnMrecLoadedFailed(LoadAdError args)
    {
        _retryAttemptMrec++;
        double retryDelay = Math.Pow(2, Math.Min(6, _retryAttemptMrec));
        ReloadMrecAfterFail(retryDelay).Forget();

        Debug.Log("Mrec fail " + " " + args.GetCode() + " " + args.GetMessage());
    }

    async UniTaskVoid ReloadMrecAfterFail(double delaySecs)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delaySecs), true, cancellationToken: _disposeCancelSource.Token);
        RequestMrec();
    }

    public void ShowMrec()
    {
        if (_bannerMrec == null) return;
        _bannerMrec.Show();
    }
    public void HideMrec()
    {
        if (_bannerMrec == null) return;
        _bannerMrec.Hide();
    }

    #endregion

    #region open app ad
    public void LoadOpenAppAd()
    {
        AdRequest request = new AdRequest.Builder().Build();
        Debug.Log("Load AOA Ads");
        // Load an app open ad for portrait orientation
        AppOpenAd.Load(_configData.openAppAdID, ScreenOrientation.Portrait, request, ((appOpenAd, error) =>
        {
            if (error != null)
            {
                // Handle the error.
                Debug.LogFormat("Failed to load the ad. (reason: {0})", error.GetMessage());
                return;
            }

            Debug.Log("AppOpenAd loaded. Please background the app and return.");

            // App open ad is loaded.
            _openAppAd = appOpenAd;
        }));

    }

    async UniTaskVoid ReloadOpenAppAdAfterFail(double delaySecs)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delaySecs), true, cancellationToken: _disposeCancelSource.Token);
        LoadOpenAppAd();
    }

    public void ShowOAA()
    {
        if (IsOAAvailable)
        {
            this._openAppAd.OnAdFullScreenContentClosed += HandleAdDidDismissFullScreenContent;
            this._openAppAd.OnAdFullScreenContentFailed += HandleAdFailedToPresentFullScreenContent;
            this._openAppAd.OnAdFullScreenContentOpened += HandleAdDidPresentFullScreenContent;
            this._openAppAd.OnAdImpressionRecorded += HandleAdDidRecordImpression;
            this._openAppAd.OnAdPaid += HandlePaidEvent;
            _openAppAd.Show();
        }       
    }

    private void HandleAdDidDismissFullScreenContent()
    {
        Debug.Log("Closed app open ad");

        //StartCoroutine(ActionHelper.StartAction(() => { _isShowingOAA = false; }, 3f));

        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        this._openAppAd.OnAdFullScreenContentClosed += HandleAdDidDismissFullScreenContent;
        this._openAppAd.OnAdFullScreenContentFailed += HandleAdFailedToPresentFullScreenContent;
        this._openAppAd.OnAdFullScreenContentOpened += HandleAdDidPresentFullScreenContent;
        this._openAppAd.OnAdImpressionRecorded += HandleAdDidRecordImpression;
        this._openAppAd.OnAdPaid += HandlePaidEvent;
        this._openAppAd = null;
        LoadOpenAppAd();
    }
    private void HandleAdFailedToPresentFullScreenContent(AdError args)
    {
        Debug.LogFormat("Failed to present the ad (reason: {0})", args.GetMessage());
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        this._openAppAd.OnAdFullScreenContentClosed += HandleAdDidDismissFullScreenContent;
        this._openAppAd.OnAdFullScreenContentFailed += HandleAdFailedToPresentFullScreenContent;
        this._openAppAd.OnAdFullScreenContentOpened += HandleAdDidPresentFullScreenContent;
        this._openAppAd.OnAdImpressionRecorded += HandleAdDidRecordImpression;
        this._openAppAd.OnAdPaid += HandlePaidEvent;
        this._openAppAd = null;
        LoadOpenAppAd();
    }

    private void HandleAdDidPresentFullScreenContent()
    {
        Debug.Log("Displayed app open ad");

#if BL_APPSFLYER
        AppsflyerManager.OnAppOpenAdsDisplayed();
#endif

        //isShowingAd = true;
    }

    private void HandleAdDidRecordImpression()
    {
        Debug.Log("Recorded ad impression");
    }

    private void HandlePaidEvent(AdValue args)
    {
        Debug.LogFormat("Received paid event. (currency: {0}, value: {1}",
                args.CurrencyCode, args.Value);
#if BL_FIREBASE
        FirebaseManager.OnAOAPaidEvent(args);
#endif
    }

    #endregion

    #region Native Ads
    public void RequestNativeAd(System.EventHandler<NativeAdEventArgs> loadHandler, System.EventHandler<AdFailedToLoadEventArgs> loadFail)
    {
        AdLoader adLoader = new AdLoader.Builder(_configData.nativeAds).ForNativeAd().Build();
        adLoader.OnNativeAdLoaded += loadHandler;
        adLoader.OnAdFailedToLoad += loadFail;
        adLoader.LoadAd(CreateAdRequest());

        Debug.Log("Reqest Native Ads");
    }
    #endregion

    public bool IsOAAvailable
    {
        get
        {
            return _openAppAd != null;
        }
    }

    public override bool IsInterAdsReady()
    {
        return _interstitialAd != null;
    }

    public override bool IsRewardAdsReady()
    {
        return _rewardedAd != null;
    }

    public override void ShowInterstitial(string placement = "")
    {
        if (IsInterAdsReady())
        {
            _interstitialAd.Show();
        }
    }

    public override void ShowRewardedAd(UnityAction successCallback, UnityAction failCallback, string adPlacement = "")
    {
        if (IsRewardAdsReady())
        {
            _rewardedAd.Show((reward) =>
            {
                successCallback?.Invoke();
            });
        }
        else
        {
            failCallback?.Invoke();
        }
    }

    public override void ShowBanner()
    {

    }

    public override void HideBanner()
    {

    }
}
#endif
