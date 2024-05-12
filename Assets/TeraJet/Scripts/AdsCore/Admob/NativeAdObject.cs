using Cysharp.Threading.Tasks;
using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System;

#if BL_ADMOB
public class NativeAdObject : MonoBehaviour
{
    public RawImage adIcon;
    public RawImage adChoices;
    public Text adHeadline;
    public Text callToAction;
    public Text adAdvertiser;

    bool _adIsLoaded;

    NativeAd _ad;

    CancellationTokenSource quitToken;

    private void OnEnable()
    {
        if (quitToken != null)
        {
            quitToken.Cancel();
            quitToken.Dispose();
        }

        quitToken = new CancellationTokenSource();
        BL_AdManager.Instance.AdmobController.RequestNativeAd(OnNativeLoaded, OnNativeLoadedFail);
    }

    private void OnDisable()
    {
        //if (quitToken != null)
        //{
        //    quitToken.Cancel();
        //    quitToken.Dispose();
        //}   
    }

    private void OnDestroy()
    {
        if (quitToken != null)
        {
            quitToken.Cancel();
            quitToken.Dispose();
        }
    }

    public void Init()
    {
        if (_adIsLoaded)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (_adIsLoaded && _ad != null)
        {
            _adIsLoaded = false;
            GoogleMobileAds.Api.NativeAd nativeAd = _ad;

            adIcon.texture = nativeAd.GetIconTexture();
            adChoices.texture = nativeAd.GetAdChoicesLogoTexture();
            adHeadline.text = nativeAd.GetHeadlineText();
            callToAction.text = nativeAd.GetCallToActionText();
            adAdvertiser.text = nativeAd.GetAdvertiserText();

            bool suc = nativeAd.RegisterIconImageGameObject(adIcon.gameObject);
            nativeAd.RegisterAdChoicesLogoGameObject(adChoices.gameObject);
            nativeAd.RegisterHeadlineTextGameObject(adHeadline.gameObject);
            nativeAd.RegisterCallToActionGameObject(callToAction.gameObject);
            nativeAd.RegisterAdvertiserTextGameObject(adAdvertiser.gameObject);

            if (!suc)
            {
                Debug.Log("Native Ads: register fail");
            }
        }
    }

    void OnNativeLoaded(object sender, NativeAdEventArgs nativeAd)
    {

        try
        {
            _adIsLoaded = true;
            gameObject.SetActive(true);
            //if (!GameManager.Instance._isShowAds) gameObject.SetActive(false);

            if (_ad != null)
            {
                _ad.OnPaidEvent -= RecordImpression;
            }

            _ad = nativeAd.nativeAd;

            _ad.OnPaidEvent += RecordImpression;
        }
        catch (NullReferenceException e)
        {
            Debug.Log(e.Message);
        }

        Debug.Log("Native loaded Success: " + nativeAd.nativeAd.GetResponseInfo()?.GetLoadedAdapterResponseInfo()?.AdSourceInstanceName);
    }
    void OnNativeLoadedFail(object sender, AdFailedToLoadEventArgs error)
    {
        _adIsLoaded = false;

        TurnOffObject(quitToken.Token).Forget();

        Debug.Log("Native loaded fail: " + error.LoadAdError.GetMessage());

        ReloadAd().Forget();
    }

    async UniTaskVoid TurnOffObject(CancellationToken cancellationTokenQuit)
    {
        await UniTask.SwitchToMainThread(cancellationToken: cancellationTokenQuit);
        if (_ad == null)
        {
            gameObject.SetActive(false);
        }

    }

    async UniTaskVoid ReloadAd()
    {
        await UniTask.Delay(3000, cancellationToken: quitToken.Token);

        try
        {
            BL_AdManager.Instance.AdmobController.RequestNativeAd(OnNativeLoaded, OnNativeLoadedFail);
        }
        catch (System.Exception e)
        {
            Debug.Log("Reload native error: " + e.Message);
        }
    }

    void RecordImpression(object sender, AdValueEventArgs ad)
    {
#if BL_FIREBASE
    FirebaseManager.SendImpressionSuccessEvent(ad.AdValue);
#endif
    }
}
#endif
