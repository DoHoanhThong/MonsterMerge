using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SettingInGame : MonoBehaviour
{
    [SerializeField] GameObject _settingPanel, _fade;
    [SerializeField] AudioClip _clickSound;
    [SerializeField] Toggle _music, _sfx;
    [SerializeField] GameObject _thankS;
    int dem ;
    private void Start()
    {
        if (PlayerPrefs.GetInt(CONSTANT.Music) == 0)
        {
            
            _music.isOn = false;
        }
        else
        {
            
            _music.isOn = true;
        }
        if (PlayerPrefs.GetInt(CONSTANT.SFX) == 0)
        {
            
            _sfx.isOn = false;
        }
        else
        {
            
            _sfx.isOn = true;
        }
        dem = 0;
        _settingPanel.SetActive(false);
    }
    public void ActiveSetting()
    {
         SoundEffect.instance.PlaySound(_clickSound);
        _settingPanel.SetActive(true);
        _settingPanel.transform.DORotate(new Vector3(0, 0, 0.087f * Mathf.Rad2Deg), 0.2f);
        _settingPanel.transform.DORotate(new Vector3(0, 0, 0), 0.2f).SetDelay(0.2f).OnComplete(() =>
        {
            dem += 1;
            GameManager.instance.canplaySound = true;
        });
        _fade.SetActive(true);
        
    }
    public void DeActiveSetting()
    {
        SoundEffect.instance.PlaySound(_clickSound);
        _settingPanel.SetActive(false);
        _fade.SetActive(false);
    }
    public void MusicOnvalueChanged()
    {
        //SoundEffect.instance.PlaySound(_clickSound);
        if (_music.isOn)
        {
            if (!BGMusic.instance._soundsource.isPlaying)
            {
                BGMusic.instance._soundsource.Play();
            }

            PlayerPrefs.SetInt(CONSTANT.Music, 1);
            return;
        }
        BGMusic.instance._soundsource.Stop();
        PlayerPrefs.SetInt(CONSTANT.Music, 0);
    }
    public void SFXsOnvalueChanged()
    {
        //SoundEffect.instance.PlaySound(_clickSound);
        if (_sfx.isOn)
        {
            SoundEffect.instance._soundsource.volume = 1;
            PlayerPrefs.SetInt("SFXVolume", 1);
            PlayerPrefs.SetInt(CONSTANT.SFX, 1);
            return;
        }
        SoundEffect.instance._soundsource.volume = 0;
        PlayerPrefs.SetInt(CONSTANT.SFX, 0);

    }
    public void DeactiveThankYou()
    {
        _thankS.transform.DOScale(Vector2.one * 0.6f, 0.1f).OnComplete(() =>
        {
            SoundEffect.instance.PlaySound(_clickSound);
            _thankS.SetActive(false);
            _fade.SetActive(false);
        });
    }
}

