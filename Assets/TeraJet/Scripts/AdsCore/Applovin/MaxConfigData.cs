using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu()]
public class MaxConfigData : ScriptableObject
{
    [Header("KEY")]
    public string sdkKey;

#if UNITY_ANDROID
    [Header("Android ID")]
    public string interAdUnitId = "8dce353e6036956c";
    public string rewardAdUnitId = "82fe8b6971c7969e ";
    public string bannerAdUnitId = "191194c61ec0593b ";
    public string appOpenAdUnitId = "ca-app-pub-3940256099942544/1033173712";
    public string mrecAdUnitId = "dc4f78de882ffb29 ";
#elif UNITY_IOS
    [Header("IOS ID")]
    public string interAdUnitId = "";
    public string rewardAdUnitId = "";
    public string bannerAdUnitId = "";
    public string appOpenAdUnitId = "";
    public string mrecAdUnitId = "";
#else
    [Header("Other Platform ID")]
    public string interAdUnitId = "";
    public string rewardAdUnitId = "";
    public string bannerAdUnitId = "";
    public string appOpenAdUnitId = "";
    public string mrecAdUnitId = "";
#endif

    private void OnEnable()
    {
        
    }

}
