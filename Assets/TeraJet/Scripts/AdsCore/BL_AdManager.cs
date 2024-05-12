using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine.Events;
using Sirenix.OdinInspector;
public enum Mediation
{
    MaxSDK = 0,
    Ironsource = 1,
}

public enum InterCappingType
{
    Custom,
    Firebase,
}

public class BL_AdManager : MonoBehaviour
{
    [SerializeField, ShowIf("_mediation", Mediation.MaxSDK)] MaxConfigData _maxConfig;
    [SerializeField, ShowIf("_mediation", Mediation.Ironsource)] IronsourceConfigData _ironsourceConfig;
    [SerializeField] AdmobConfigData _admobConfig;

#if BL_FIREBASE
    [SerializeField] FirebaseManager _firebaseManager;
#endif

    [SerializeField] Mediation _mediation = Mediation.MaxSDK;
    [SerializeField] InterCappingType _interCappingType;

    [SerializeField,ShowIf("_interCappingType", InterCappingType.Custom)] float _interCappingTime = 30;

    AdController _mediationController;

    CancellationTokenSource _disposeCancelSource;

#if BL_ADMOB
    BL_AdmobController _admobController;
    public BL_AdmobController AdmobController =>_admobController;
#endif

    float _lastShowNormalAd = 0;
    float _lastShowInterAd = 0;
    float _timer = 5;

    bool _isInit;

    bool _isShowBanner = false;

    public bool IsInit => _isInit;

    public static BL_AdManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        Init();
    }

    public void Init()
    {
        _lastShowNormalAd = 0;
#if BL_FIREBASE
        _firebaseManager.Initialize();
#endif

        _disposeCancelSource = new CancellationTokenSource();

        if(_mediation == Mediation.MaxSDK)
        {
#if BL_MAX_SDK
            _mediationController = new BL_MaxController(_maxConfig, _disposeCancelSource, false);
#endif
        }
        else if(_mediation == Mediation.Ironsource)
        {
#if BL_IRONSOURCE
            _mediationController = new BL_IronsouceController(_ironsourceConfig, _disposeCancelSource);
#endif
        }

        _mediationController.onMediationInitComplete += OnMediationInitComplete;
        _mediationController.Init();

        _admobController = new BL_AdmobController(_admobConfig, _disposeCancelSource);
        _admobController.Init();

        
    }

    void OnMediationInitComplete()
    {
        _isInit = true;
    }

    private void Update()
    {
        if (_isInit)
        {
            
        }
        _timer += Time.deltaTime;
    }

    public void ShowInterstitial(string placement = "")
    {
        if(AdsNotCollide() && InterCappingValid())
        {
            if (_mediationController.IsInterAdsReady())
            {
                _lastShowNormalAd = _timer;
                _lastShowInterAd = _timer;
                _mediationController.ShowInterstitial(placement);
            }
            else
            {

            }
        }
    }

    public void ShowRewardedAd(UnityAction successCallback, UnityAction failCallback, string adPlacement = "")
    {
        if (AdsNotCollide())
        {
            if (_mediationController.IsRewardAdsReady())
            {
                _lastShowNormalAd = _timer;
                _mediationController.ShowRewardedAd(successCallback, failCallback, adPlacement);
            }
            else
            {

#if BL_ADMOB
            
#endif
            }
        }
    }

    public void ShowOAA()
    {
        if (AdsNotCollide())
        {
            if (_admobController.IsOAAvailable)
            {
                _lastShowNormalAd = _timer;
                _admobController.ShowOAA();
            }
        }
    }

    public bool IsOAAReady()
    {
        return _admobController.IsOAAvailable;
    }

    public void ShowMrec()
    {
#if BL_ADMOB
        //_admobController.ShowMrec();
        BL_MaxController max = (BL_MaxController)_mediationController;
        max.ShowMrec();
#endif
    }
    public void HideMrec()
    {
#if BL_ADMOB
        BL_MaxController max = (BL_MaxController)_mediationController;
        max.HideMrec();
#endif
    }

    public void ShowBanner()
    {
        if (!_isShowBanner)
        {
            _isShowBanner = true;
            _mediationController.ShowBanner();
        }
    }

    public void HideBanner()
    {
        _isShowBanner = false;
        _mediationController.HideBanner();
    }

    bool AdsNotCollide()
    {
        return (_timer - _lastShowNormalAd) >= 4 && !_mediationController.IsShowingNormalAd;
    }

    bool InterCappingValid()
    {
        float cappingTime = 10;
        if(_interCappingType == InterCappingType.Custom)
        {
            cappingTime = _interCappingTime;
        }
        else if(_interCappingType == InterCappingType.Firebase)
        {
#if BL_FIREBASE
            if (FirebaseManager.isInitialized)
            {
                cappingTime = FirebaseManager.instance.GetInterCappingTime();
            }
            else
            {
                cappingTime = _interCappingTime;
            }
#endif
        }

        return (_timer - _lastShowInterAd) >= cappingTime;
    }
}
