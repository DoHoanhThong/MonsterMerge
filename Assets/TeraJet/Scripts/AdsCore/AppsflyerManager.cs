using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeraJet;

#if BL_APPSFLYER
using AppsFlyerSDK;

public class AppsflyerManager
{
    public static void OnInterAdEligible()
    {
        AppsFlyer.sendEvent("af_inters_ad_eligible", null);
    }
    public static void OnInterAdCalled()
    {
        AppsFlyer.sendEvent("af_inters_api_called", null);
    }
    public static void OnInterAdDisplayed()
    {
        AppsFlyer.sendEvent("af_inters_displayed", null);
    }

    //Reward ads event

    public static void OnRewardAdEligible()
    {
        AppsFlyer.sendEvent("af_rewarded_ad_eligible", null);
    }
    public static void OnRewardAdCalled()
    {
        AppsFlyer.sendEvent("af_rewarded_api_called", null);
    }
    public static void OnRewardAdDisplayed()
    {
        AppsFlyer.sendEvent("af_rewarded_displayed", null);
    }
    public static void OnRewardAdCompleted()
    {
        AppsFlyer.sendEvent("af_rewarded_ad_completed", null);
    }

    public static void FireCustomEvent(string e)
    {
        AppsFlyer.sendEvent(e, null);
    }

    public static void OnAppOpenAdsDisplayed()
    {
        AppsFlyer.sendEvent("af_aoa_ad_displayed", null);
    }
}
#endif
