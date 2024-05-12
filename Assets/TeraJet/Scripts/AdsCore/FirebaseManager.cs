using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Analytics;
using System;
using Firebase.RemoteConfig;
using System.Threading.Tasks;
using Firebase.Extensions;
using TeraJet;

public class FirebaseManager : Singleton<FirebaseManager>
{
#if BL_FIREBASE
    [SerializeField] int m_revenueDayInterval = 1;

    [SerializeField] float _interCappingDefault = 15f;

    public static bool isInitialized;

    bool _isBeingInitialize;

    public Dictionary<string, object> remoteDiction = new Dictionary<string, object>();

    void Start()
    {

    }

    public float GetInterCappingTime()
    {
        return (float)remoteDiction["inter_ads_capping"];
    }

    public void Initialize()
    {
        if (_isBeingInitialize) return;

        _isBeingInitialize = true;

        remoteDiction.Add("inter_ads_capping", _interCappingDefault);
        remoteDiction.Add("afk_inter", "on");
        remoteDiction.Add("afk_inter_timer", 12f);
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                InitializeFirebase();
                FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(remoteDiction)
                .ContinueWithOnMainThread(task =>
                {
                    FetchDataAsync();
                });
                // Set a flag here to indicate whether Firebase is ready to use by your app.
                Debug.Log("Firebase Initialized");
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });

    }
    public Task FetchDataAsync()
    {
        Debug.Log("Fetching data...");
        System.Threading.Tasks.Task fetchTask =
        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(
            TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }
    private void FetchComplete(Task fetchTask)
    {
        if (fetchTask.IsCanceled)
        {
            Debug.Log("Fetch canceled.");
        }
        else if (fetchTask.IsFaulted)
        {
            Debug.Log("Fetch encountered an error.");
        }
        else if (fetchTask.IsCompleted)
        {
            Debug.Log("Fetch completed successfully!");
        }

        var info = FirebaseRemoteConfig.DefaultInstance.Info;

        switch (info.LastFetchStatus)
        {
            case LastFetchStatus.Success:
                FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
                .ContinueWithOnMainThread(task =>
                {
                    string afk_inter = FirebaseRemoteConfig.DefaultInstance.GetValue("afk_inter").StringValue;
                    float afk_inter_timer = (float)FirebaseRemoteConfig.DefaultInstance.GetValue("afk_inter_timer").DoubleValue;
                    float interAdsCapping = (float)FirebaseRemoteConfig.DefaultInstance.GetValue("inter_ads_capping").DoubleValue;

                    if (!string.IsNullOrEmpty(afk_inter))
                    {
                        remoteDiction["afk_inter"] = afk_inter;
                    }
                    remoteDiction["afk_inter_timer"] = afk_inter_timer;

                    remoteDiction["inter_ads_capping"] = interAdsCapping;

                    Debug.Log("Success: inter capping" + remoteDiction["inter_ads_capping"]);
                    Debug.Log("Success afk_inter: " + remoteDiction["afk_inter"]);
                    Debug.Log("Success afk_inter_timer: " + remoteDiction["afk_inter_timer"]);

                    isRemoteDataReady = true;
                });
                break;
            case LastFetchStatus.Failure:
                switch (info.LastFetchFailureReason)
                {
                    case FetchFailureReason.Error: Debug.Log("Error"); break;
                    case FetchFailureReason.Throttled:
                        Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
                        break;
                }
                break;
            case LastFetchStatus.Pending:
                Debug.Log("Latest Fetch call still pending.");
                break;
        }
    }
    protected void InitializeFirebase()
    {
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        Firebase.Messaging.FirebaseMessaging.TokenRegistrationOnInitEnabled = true;
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        // Set a flag here to indicate whether Firebase is ready to use by your app.
        isInitialized = true;
    }
    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
    }
    public void OnDestroy()
    {
        if (isInitialized)
        {

            Firebase.Messaging.FirebaseMessaging.MessageReceived -= OnMessageReceived;
            Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
        }
    }
    //Firebase Normal Event

    public void SendNormalEvent(string eventKey, params Parameter[] parameters)
    {
        if (!isInitialized) return;
        FirebaseAnalytics.LogEvent(eventKey, parameters);
    }
    public void OnLevelStartEvent(string currentMoneyVal)
    {
        SendNormalEvent("level_start", new Parameter("current_gold", currentMoneyVal));
    }
    public void OnAdsRewardShow(string placement)
    {
        SendNormalEvent("ads_reward_show", new Parameter("placement", placement));
    }
    public void OnAdsRewardFail(string placement, string errorMsg)
    {
        errorMsg = LimitLength(errorMsg, 39);
        SendNormalEvent("ads_reward_fail", new Parameter("placement", placement), new Parameter("errormsg", errorMsg));
    }
    public void OnAdsRewardLoadedFail(string errorMsg)
    {
        errorMsg = LimitLength(errorMsg, 39);
        if (IsInternetAvailable())
        {
            SendNormalEvent("ads_reward_loaded_fail", new Parameter("errormsg", errorMsg));
        }
        else
        {
            SendNormalEvent("ads_reward_loaded_fail_no_internet", new Parameter("errormsg", errorMsg));
        }
    }
    public void OnAdsInterFail(string errorMsg)
    {
        errorMsg = LimitLength(errorMsg, 39);
        SendNormalEvent("ad_inter_fail", new Parameter("errormsg", errorMsg));
    }
    public void OnAdsInterLoadedFail(string errorMsg)
    {
        errorMsg = LimitLength(errorMsg, 39);
        if (IsInternetAvailable())
        {
            SendNormalEvent("ads_inter_loaded_fail", new Parameter("errormsg", errorMsg));
        }
        else
        {
            SendNormalEvent("ads_inter_loaded_fail_no_internet", new Parameter("errormsg", errorMsg));
        }
    }
    public void OnAdsInterShow()
    {
        SendNormalEvent("ad_inter_show", new Parameter("placement", ""));
    }

    public string LimitLength(string source, int maxLength)
    {
        if (source.Length <= maxLength)
        {
            return source;
        }

        return source.Substring(0, maxLength);
    }

    //Firebase Ad impression
#if BL_ADMOB
    public static void OnAOAPaidEvent(GoogleMobileAds.Api.AdValue adValue)
    {
        if (!isInitialized) return;
        double revenue = adValue.Value;
        var impressionParameters = new[] {
            new Firebase.Analytics.Parameter("ad_platform", "AdMob"),
            new Firebase.Analytics.Parameter("ad_source", "AdMob"),
            new Firebase.Analytics.Parameter("ad_unit_name", ""),
            new Firebase.Analytics.Parameter("ad_format", "AOA"),
            new Firebase.Analytics.Parameter("value", revenue),
            new Firebase.Analytics.Parameter("country_code",""),
            new Firebase.Analytics.Parameter("currency", adValue.CurrencyCode), // All AppLovin revenue is sent in USD
         };
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
    }

    public static void SendImpressionSuccessEvent(GoogleMobileAds.Api.AdValue impressionData)
    {
        if (!isInitialized) return;
        Debug.Log("unity-script:  ImpressionSuccessEvent impressionData = " + impressionData);
        if (impressionData != null)
        {
            double revenue = impressionData.Value / 1000000;

            Firebase.Analytics.Parameter[] AdParameters = {
                new Firebase.Analytics.Parameter("ad_platform", "ironSource"),
                new Firebase.Analytics.Parameter("ad_source", "AdMob"),
                new Firebase.Analytics.Parameter("ad_unit_name", "NativeAd"),
                new Firebase.Analytics.Parameter("ad_format", "NativeAd"),
                new Firebase.Analytics.Parameter("currency","USD"),
                new Firebase.Analytics.Parameter("value", revenue)
            };

            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", AdParameters);
        }
    }

#endif
#if BL_MAX_SDK
    public static void OnAdRevenuePaidEvent(MaxSdkBase.AdInfo impressionData)
    {
        if (!isInitialized) return;
        SendNormalAdsImpression(impressionData);
    }
    static void SendNormalAdsImpression(MaxSdkBase.AdInfo impressionData)
    {
        if (!isInitialized) return;
        double revenue = impressionData.Revenue;
        var impressionParameters = new[] {
            new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
            new Firebase.Analytics.Parameter("ad_source", impressionData.NetworkName),
            new Firebase.Analytics.Parameter("ad_unit_name", impressionData.AdUnitIdentifier),
            new Firebase.Analytics.Parameter("ad_format", impressionData.AdFormat),
            new Firebase.Analytics.Parameter("value", revenue),
            new Firebase.Analytics.Parameter("country_code",MaxSdk.GetSdkConfiguration().CountryCode),
            new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
         };
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
    }
#endif

#if BL_IRONSOURCE
    public static void SendImpressionSuccessEvent(IronSourceImpressionData impressionData)
    {
        if (!isInitialized) return;
        Debug.Log("unity-script:  ImpressionSuccessEvent impressionData = " + impressionData);
        if (impressionData != null)
        {
            double revenue = 0;

            if (impressionData.revenue != null)
            {
                revenue = (double)impressionData.revenue;
            }

            Firebase.Analytics.Parameter[] AdParameters = {
                new Firebase.Analytics.Parameter("ad_platform", "ironSource"),
                new Firebase.Analytics.Parameter("ad_source", impressionData.adNetwork),
                new Firebase.Analytics.Parameter("ad_unit_name", impressionData.adUnit),
                new Firebase.Analytics.Parameter("ad_format", impressionData.instanceName),
                new Firebase.Analytics.Parameter("currency","USD"),
                new Firebase.Analytics.Parameter("value", revenue)
            };

            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", AdParameters);
        }
    }
#endif



    double minRevToSent = 1.0f;
    private bool isRemoteDataReady;

    public bool CheckValidDay(string lastCheckTime, int dayInterval)
    {
        try
        {
            DateTime currentDate = DateTime.Now;
            DateTime lastCheckDate = new DateTime();

            bool isParsed = DateTime.TryParse(lastCheckTime, out lastCheckDate);

            currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 0, 0, 0);
            lastCheckDate = new DateTime(lastCheckDate.Year, lastCheckDate.Month, lastCheckDate.Day, 0, 0, 0);

            if (!isParsed)
            {
                return true;
            }

            double totalDays = (lastCheckDate - currentDate).TotalDays;

            if (totalDays >= dayInterval)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return true;
        }
    }
    string GetLTVD1LastCheckTime()
    {
        return PlayerPrefs.GetString("LVTD1_LastCheckTIme", DateTime.Now.ToString());
    }
    void SetLTVD1LastCheckTime(string time)
    {
        PlayerPrefs.SetString("LVTD1_LastCheckTIme", time);
    }
    double GetLTVD1Rev()
    {
        return (double)PlayerPrefs.GetFloat("LVTD1_Rev", 0);
    }
    void SetLTVD1Rev(double value)
    {
        PlayerPrefs.SetFloat("LVTD1_Rev", (float)value);
    }

    public bool IsInternetAvailable()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
#endif
}
