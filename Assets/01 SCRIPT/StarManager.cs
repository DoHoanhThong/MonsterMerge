using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StarManager : MonoBehaviour
{
    [SerializeField] GameObject _collectionCopy;
    [SerializeField] GameObject _result, _desti, _collect;
    [SerializeField] VFXsController _vfxcontroller;
    [SerializeField] GameObject _ratePanel;
    Coroutine a;
    [SerializeField] GameObject _setting;
    [SerializeField] GameObject _merge3Panel;
    public GameObject _bgStarBar, _starBar;
    [SerializeField] GameObject _backMenubutton, _createMoreButton, _collectionButt, _fadePanel;
    [SerializeField] TextController _textControll;
    [SerializeField] List<GameObject> _listStarInstance = new List<GameObject>();
    [SerializeField] GameObject _starTemplate;
    [SerializeField] AudioClip[] _liststarAudio = new AudioClip[10];
    [SerializeField] AudioClip _RaritySound, _clickSound;
    [SerializeField] GameObject _starVFXs;
    public GameObject[] _listStarVFXsBase = new GameObject[10];
    public List<GameObject> _listVFXsInstance = new List<GameObject>();
    [SerializeField] ScrollRect _scroll;
    private void Start()
    {
        _collectionCopy.SetActive(false);
        if (_merge3Panel !=null )
        {
            _merge3Panel.SetActive(false);
            
        }
        
        _bgStarBar.transform.GetComponent<Image>().enabled = false;
        _bgStarBar.SetActive(true);
        _starBar.SetActive(false);
        _backMenubutton.SetActive(false);
        _createMoreButton.SetActive(false);
        _collectionButt.SetActive(false);
        if (_ratePanel == null)
        {
            return;
        }
        _ratePanel.SetActive(false);
    }
    public void InstanceStar(int star, float time, GameObject parent, GameObject BGImage, int begin)
    {
        //_setting.SetActive(false);
        if (begin >= star)
        {
            StartCoroutine(Wait1(star, BGImage));
             return;
        }
        StartCoroutine(Wait2( star,  time,  parent,  BGImage,  begin));
    }
    IEnumerator Wait1(int star, GameObject BGImage)
    {
        yield return new WaitForSeconds(1.3f);
        BGImage.transform.DOScale(new Vector2((star ==10) ? BGImage.transform.localScale.x : BGImage.transform.localScale.x + 1, 2.4f), 0.2f);
        BGImage.transform.GetComponent<Image>().pixelsPerUnitMultiplier = BGImage.transform.localScale.x ;
        BGImage.transform.DOMoveY(BGImage.transform.position.y, 0.2f);
        foreach (var t in _listStarInstance)
        {
            t.transform.GetComponent<Image>().sprite = GameManager.instance._listImageStar[star - 1];
        }
        ActiveStarVFX(star);
        SoundEffect.instance.PlaySound(_RaritySound);
        _textControll._Rarity.enabled = true;
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(ActiveButton());
    }
    IEnumerator Wait2(int star, float time, GameObject parent, GameObject BGImage, int begin)
    {
        
        if (begin <= 2 && begin >=0)
        {
            yield return new WaitForSeconds(0.2f);
        }
        else if (begin >= 3 && begin <= 4)
        {
            time += 0.2f;
            yield return new WaitForSeconds(time);
            
        }
        else if (begin >= 5 && begin <= 7)
        {
            
            yield return new WaitForSeconds(1f);
            
        }
        else if(begin >=8 && begin <= 9)
        {
            yield return new WaitForSeconds(1.3f);
        }

        GameObject a = ObjectPooling.instance.GetObject(_starTemplate);
        
        a.gameObject.SetActive(true);
        Vector3 z = a.transform.position;
        z.z = 12;
        a.transform.position = z;
        a.transform.SetParent(parent.transform);
        a.transform.localScale = Vector2.one * 1.5f;
        a.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = 
            _listStarVFXsBase[star - 1].GetComponentInChildren<ParticleSystem>().startColor;
        SoundEffect.instance.PlaySound(_liststarAudio[begin]);
        _listStarInstance.Add(a);
        BGImage.transform.GetComponent<Image>().enabled = true;
        BGImage.transform.localScale = new Vector2(BGImage.transform.localScale.x + 1f, BGImage.transform.localScale.y);
        BGImage.transform.GetComponent<Image>().pixelsPerUnitMultiplier = BGImage.transform.localScale.x * 2;

        a.transform.DORotate(new Vector3(0, 0, -6.283185307f * 5 * (Mathf.Rad2Deg)), 0.4f, RotateMode.FastBeyond360);
        a.transform.DOScale(Vector2.one, 0.4f).OnComplete(() =>
        {
            a.transform.GetComponent<Image>().sprite = GameManager.instance._listImageStar[0];
            InstanceStar(star, time, parent, BGImage, begin + 1);
        });
    }
    public void CREATEMOREBTN()
    {
        
        _bgStarBar.transform.GetComponent<Image>().enabled = false;
       // _bgStarBar.transform.position = _starBar.transform.position;
        _bgStarBar.transform.localScale = CONSTANT.beginBGStarBar;
        _createMoreButton.SetActive(false);
        _backMenubutton.SetActive(false);
        _collectionButt.SetActive(false);
        
        _starBar.SetActive(false);
        _starBar.transform.position = new Vector2(0, _starBar.transform.position.y);
        if (_scroll != null)
        {
            _scroll.enabled = true;
        }
        
        ShowBannerADSagain();
    }
    public void ResetListInstance()
    {
        foreach (var t in _listStarInstance)
        {
            t.gameObject.SetActive(false);
            t.transform.GetComponent<Image>().sprite = _starTemplate.transform.GetComponent<Image>().sprite;
            t.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void RESULT()
    {
        _starBar.SetActive(true);
    }
    public void ActiveStarVFX(int star)
    {
        foreach (GameObject t in _listStarInstance)
        {
            Transform h = t.transform.GetChild(0);
            h.gameObject.SetActive(true);
        }
    }
    public void ActiveMerge3Panel()
    {
        SoundEffect.instance.PlaySound(_clickSound);
        GameManager.instance.InterAds();
        _merge3Panel.SetActive(true);
        _merge3Panel.transform.localScale = Vector3.one * 0.5f;
        _merge3Panel.transform.DOScale(Vector2.one, 0.15f);
        _fadePanel.SetActive(true);
    }
    public void DeActiveMerge3Panel()
    {
        SoundEffect.instance.PlaySound(_clickSound);
        
        _merge3Panel.transform.DOScale(new Vector2(0.3f, 0.3f), 0.08f).OnComplete(() =>
        {
            _merge3Panel.SetActive(false);
        });
        
        _fadePanel.SetActive(false);
    }
    void ShowBannerADSagain()
    {
        BL_AdManager.Instance.ShowBanner();
        //Debug.LogError("show bannerads again complete!");
    }
    IEnumerator ActiveButton()
    {
        _setting.SetActive(true);
        _setting.GetComponent<Button>().enabled = false;
        _createMoreButton.SetActive(true);
        _createMoreButton.transform.GetComponent<Button>().enabled = false;
        _backMenubutton.SetActive(true);
        _backMenubutton.transform.GetComponent<Button>().enabled = false;
        _backMenubutton.transform.GetChild(0).GetComponent<Button>().enabled = false;
        _backMenubutton.transform.GetChild(1).GetComponent<Button>().enabled = false;
        _vfxcontroller.VFXcollect(_result, _collect, _desti, _fadePanel, _collectionCopy);
        _collectionButt.SetActive(true);
        _collectionButt.transform.GetComponent<Button>().enabled = false;
        _collectionButt.transform.GetChild(0).GetComponent<Button>().enabled = false;
        _collectionButt.transform.GetChild(1).GetComponent<Button>().enabled = false;
        yield return new WaitForSeconds(1.65f);
        
        if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            yield return new WaitForSeconds(0.5f);
            _setting.GetComponent<Button>().enabled = true;
            _collectionButt.transform.GetComponent<Button>().enabled = true;
            _collectionButt.transform.GetChild(0).GetComponent<Button>().enabled = true;
            _collectionButt.transform.GetChild(1).GetComponent<Button>().enabled = true;

            _backMenubutton.transform.GetComponent<Button>().enabled = true;
            _backMenubutton.transform.GetChild(0).GetComponent<Button>().enabled = true;
            _backMenubutton.transform.GetChild(1).GetComponent<Button>().enabled = true;
            _createMoreButton.transform.GetComponent<Button>().enabled = true;
        }
        else
        {
            yield return new WaitForSeconds(1.2f);
            if (GameManager.instance.CountResult == 1 && _merge3Panel != null)
            {
                _merge3Panel.SetActive(true);
                _merge3Panel.transform.localScale = Vector3.one * 0.5f;
                _merge3Panel.transform.DOScale(Vector2.one, 0.15f);
                _fadePanel.SetActive(true);
            }
            yield return new WaitForSeconds(0.3f);
            _setting.GetComponent<Button>().enabled = true;
            _collectionButt.transform.GetComponent<Button>().enabled = true;
            _collectionButt.transform.GetChild(0).GetComponent<Button>().enabled = true;
            _collectionButt.transform.GetChild(1).GetComponent<Button>().enabled = true;

            _backMenubutton.transform.GetComponent<Button>().enabled = true;
            _backMenubutton.transform.GetChild(0).GetComponent<Button>().enabled = true;
            _backMenubutton.transform.GetChild(1).GetComponent<Button>().enabled = true;
            _createMoreButton.transform.GetComponent<Button>().enabled = true;
        }
        if (GameManager.instance.CountResult == 2)
        {
            //Debug.LogError("okrate");
            _fadePanel.SetActive(true);
            _ratePanel.SetActive(true);
            _ratePanel.transform.localScale = Vector3.one * 0.5f;
            _ratePanel.transform.DOScale(Vector3.one, 0.12f);
            //TeraJet.GameUtils.ShowDialog(TeraJet.PopupController.PopupType.RATE);
        }
    }
    public void deactiveRATEPanel()
    {
        SoundEffect.instance.PlaySound(_clickSound);
        _fadePanel.SetActive(false);
        _ratePanel.SetActive(false);
    }
}
