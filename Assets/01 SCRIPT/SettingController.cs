using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class SettingController : MonoBehaviour
{
    [SerializeField] GameObject _merge3Panel;
    [SerializeField] Canvas _canvas;
    [SerializeField] List<GameObject> _listObject = new List<GameObject>();
    [SerializeField] Toggle _music, _sfx;
    [SerializeField] AudioClip _clickSound;
    [SerializeField] GameObject _settingPanel;
    [SerializeField] GameObject _FadePanel;
    [SerializeField] int dem;
    [SerializeField] GameObject _Privacy;
    // Start is called before the first frame update
    void Awake()
    {
        if (PlayerPrefs.GetInt("ReadPrivacy") == 0)
        {
            _Privacy.SetActive(true);
            _Privacy.transform.localScale = Vector3.one * 0.5f;
            _Privacy.transform.DOScale(Vector3.one, 0.12f);
            _FadePanel.SetActive(true);
        }
        else
        {
            _Privacy.SetActive(false);
            _FadePanel.SetActive(false);
        }
        dem = 0;
        _merge3Panel.SetActive(false);
        if (PlayerPrefs.GetInt(CONSTANT.Music) == 0)
        {
            //_musicHandle.color = Color.white;
            _music.isOn = false;
        }
        else
        {
            //_musicHandle.color = Color.green;
            _music.isOn = true;
        }
        if (PlayerPrefs.GetInt(CONSTANT.SFX) == 0)
        {
            //_sfxHandle.color = Color.white;
           _sfx.isOn = false;
        }
        else
        {
            //_sfxHandle.color = Color.green;
            _sfx.isOn = true;
        }
        _settingPanel.SetActive(false);
        
        StartCoroutine(Zoom(0));
       StartCoroutine(Zoom(1));
    }

    public void EnableSettingButton()
    {
         SoundEffect.instance.PlaySound(_clickSound);
         //Debug.LogError("sound setting");
        _FadePanel.SetActive(true);
        _settingPanel.transform.DORotate(new Vector3(0, 0, 0.087f*Mathf.Rad2Deg), 0.2f);
        _settingPanel.transform.DORotate(new Vector3(0, 0, 0), 0.2f).SetDelay(0.2f).OnComplete(() =>
        {
            dem += 1;
            GameManager.instance.canplaySound = true;
        });
        _settingPanel.SetActive(true);
        
    }
    public void DisableSettingButton()
    {
        _settingPanel.SetActive(false);
        _FadePanel.SetActive(false);
        SoundEffect.instance.PlaySound(_clickSound);
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
            
            PlayerPrefs.SetInt(CONSTANT.Music,1);
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
            SoundEffect.instance._soundsource.volume=1;
            PlayerPrefs.SetInt("SFXVolume", 1);
            PlayerPrefs.SetInt(CONSTANT.SFX, 1);
            return;
        }
        SoundEffect.instance._soundsource.volume=0;
        PlayerPrefs.SetInt(CONSTANT.SFX, 0);

    }
    public void LanguageButton()
    {
        SoundEffect.instance.PlaySound(_clickSound);
    }
    IEnumerator Zoom(int index)
    {
            GameObject g = _listObject[index];

            yield return new WaitForSeconds(  0.15f +(index+1) * 0.2f);
            while (SceneManager.GetActiveScene().buildIndex == 1)
            {
            g.transform.DOScale(new Vector2((0.95f + index * 0.03f) , 0.95f + index * 0.03f), 1.8f);
            g.transform.DOScale(new Vector2((0.85f - index * 0.1f) , 0.85f - index * 0.1f), 1.8f).SetDelay(1.25f);
            yield return new WaitForSeconds(1.5f);
            }
    }
    public void ActiveMerge3Panel()
    {
        SoundEffect.instance.PlaySound(_clickSound);
        _merge3Panel.SetActive(true);
        _merge3Panel.transform.localScale = Vector3.one * 0.5f;
        _merge3Panel.transform.DOScale(Vector2.one, 0.15f);
        _FadePanel.SetActive(true);
    }

    public void DeactiveMerge3Panel()
    {
        SoundEffect.instance.PlaySound(_clickSound);
        
            _merge3Panel.SetActive(false);
        
        _FadePanel.SetActive(false);
    }
    public void Accept()
    {
        PlayerPrefs.SetInt("ReadPrivacy",1);
        SoundEffect.instance.PlaySound(_clickSound);
        _Privacy.SetActive(false);
        _FadePanel.SetActive(false);
    }
}
