using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class AdController
{
    protected bool _isShowingNormalAd = false;
    public bool IsShowingNormalAd => _isShowingNormalAd;

    protected bool _isInit;
    public bool IsInit => _isInit;

    protected UnityAction _rewardedSuccessCallback;

    protected UnityAction _rewardFailedCallback;

    protected string _currentAdRewardPlacement = "";

    public Action<bool> onRewardAdLoadedStateChange;
    public Action onMediationInitComplete;

    public abstract void Init();
    public abstract void ShowInterstitial(string placement = "");
    public abstract void ShowRewardedAd(UnityAction successCallback, UnityAction failCallback, string adPlacement = "");
    public abstract bool IsRewardAdsReady();
    public abstract bool IsInterAdsReady();

    public abstract void ShowBanner();
    public abstract void HideBanner();
}
