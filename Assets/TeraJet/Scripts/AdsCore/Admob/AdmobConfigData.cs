using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AdmobConfigData : ScriptableObject
{

#if UNITY_ANDROID
    [Header("Android")]
    public string mrecID;
    public string openAppAdID;
    public string nativeAds;
#elif UNITY_IOS
    [Header("IOS")]
    public string mrecID;
    public string openAppAdID;
    public string nativeAds;
#else
    [Header("Unexpected Platform")]
    public string mrecID;
    public string openAppAdID;
    public string nativeAds;
#endif
}
