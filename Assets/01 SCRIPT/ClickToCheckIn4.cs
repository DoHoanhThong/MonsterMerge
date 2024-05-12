using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToCheckIn4 : MonoBehaviour
{
    [SerializeField] CollectionsManager _collectManager;
    [SerializeField] AudioClip _clickSound;
    [SerializeField] GameObject _ADSButton;
    [SerializeField] float _timeClick, _countTime;
    [SerializeField] ClickButtonInGame _clicBut;
    private void Start()
    {
        _countTime = _timeClick;
    }
    
    private void Update()
    {
        _countTime -= Time.deltaTime;
    }
    public void CLICKTOVIEW()
    {
        if (_countTime >= 0)
            return;
        _countTime = _timeClick;
        _collectManager.button = this.gameObject;
        _collectManager.VIEWDETAIL_ON();
    }
    public void clickADSButton()
    {
        SoundEffect.instance.PlaySound(_clickSound);
        BL_AdManager.Instance.ShowRewardedAd(Succes, Fail);
        
        
    }
    void Succes()
    {
        int i = _clicBut._objectData.monster_id;
        PlayerPrefs.SetString("adsMons" + i, "false");
        this.gameObject.SetActive(false);
    }
    void Fail()
    {

    }
}
