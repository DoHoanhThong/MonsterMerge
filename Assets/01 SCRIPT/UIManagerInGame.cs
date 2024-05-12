using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManagerInGame : MonoBehaviour
{
    [SerializeField] GameObject _scroll;
    [SerializeField] JSONWriteRead JSON;
    [SerializeField] GameObject _back, _setting;
    float _beginposYofM1;
    [SerializeField] InStanceObject _instanceObj;
    private static UIManagerInGame instance;
    [SerializeField] GameObject _internetNotify;
    public static UIManagerInGame _instance => instance;
    [SerializeField] JSONWriteRead _saveToJSON;
    [SerializeField] AnimationOfImage _anim;
    [SerializeField] TextController _textCTL; 
    [SerializeField] StarManager _starManager;
    [SerializeField] float _timeShowStar;
    [SerializeField] Image _template;
    [SerializeField] GameObject _fadePanel;
    public ResultDATA _resDT;
    [SerializeField] VFXsController _vfxControll;
    [SerializeField] int _min1, _min2, _id3;
    public GameObject button; // button of Mons
    [SerializeField] GameObject _nextbutton;
    [SerializeField] GameObject _oldObject;  
    [SerializeField] GameObject _m1, _m2, _m3; // monster 1 &2
    [SerializeField]GameObject tmp;
    public List<GameObject> _listImage = new List<GameObject>(); // list Fade
    public int dem = 0;
    [SerializeField] AudioClip _ClickSound, _fusionCompleted, _RaritySound;
    public RectTransform _contentInScroll;
    public float _beginPosY;
    [SerializeField] Vector2 _beginPosM2;
    [SerializeField] Image _resultIM;
    [SerializeField]AudioSource _audio;
    void Start()
    {
        _back.SetActive(true);
        _internetNotify.SetActive(false);
        _beginposYofM1 = _m1.transform.GetComponent<RectTransform>().anchoredPosition.y;
        _audio.volume = PlayerPrefs.GetInt("SFXVolume");
        _m1.transform.GetComponent<Image>().enabled = true;
        _m2.transform.GetComponent<Image>().enabled = false;
        if (_m3 != null)
        {
            _id3 = 0;
            _m3.transform.GetComponent<Image>().enabled = false;
        }
        _fadePanel.SetActive(false);
        _beginPosM2 = _m2.transform.position;
        _resultIM.gameObject.SetActive(false);
        dem = 0;
        _nextbutton.SetActive(false);
        _beginPosY = _contentInScroll.anchoredPosition.y;
        _textCTL.PICKAMONSTERORDAD();
        
    }
    public void SpawnImage() // instant Mons
    {
        if (dem == 2)
        {
            tmp = _m3;
        }
        else
        {
            tmp = (dem == 0) ? _m1 : _m2;
        }

        BaseDATA tmpdata = button.transform.GetComponent<ClickButtonInGame>()._objectData;
        if (tmpdata.monster_sound != null)
        {
            _audio.clip = tmpdata.monster_sound;
            _audio.Play();
        }
        SoundEffect.instance.PlaySound(_ClickSound);
        Vector3 a = new Vector2(tmpdata.witdhofIM / 1000, tmpdata.heightofIM / 1000);
        tmp.name = tmpdata.monster_name;
        _textCTL.CLICKMONSTER(tmpdata);
        Transform parTrans = button.transform.parent;
        _anim.SpawnImage(parTrans, tmp.transform, tmpdata);
        _nextbutton.SetActive(true);
        if (_oldObject == button)
        {
            return;
        }

        if (_oldObject != null)
        {
            _oldObject.transform.parent.DOScale(CONSTANT.ScaleBeginPar, 0.1f);
        }
        tmp.GetComponent<Image>().enabled = true;
        tmp.GetComponent<Image>().sprite = button.GetComponent<Image>().sprite;
        _oldObject = button; //Save old
    }
    public void NextButton2()
    {
        _anim.canDoTween = false;
        foreach (var t in _anim._listTween)
        {
            t.Kill();
        }

        if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            tmp.transform.DOScale(new Vector2(2.6f, 2.6f), 0.2f);
        }
        else
        {
            tmp.transform.DOScale(new Vector2(2.8f, 2.8f), 0.2f);
        }
        StartCoroutine(GameManager.instance.CheckInternet2(_internetNotify));

        SoundEffect.instance.PlaySound(_ClickSound);
        
        if (dem == 0)
        {
            if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                _scroll.transform.GetComponent<ScrollRect>().enabled = false;
            }
            _min1 =_oldObject.transform.GetComponent<ClickButtonInGame>()._objectData.monster_id;
            _textCTL.PICKAMOMORMONSTER(_m2);
            _anim.FirstClickNextButton(_m1);
            
            BaseActionofNext();
            return;
        }
        else if (dem == 1)
        {
            if (SceneManager.GetActiveScene().buildIndex != 4)
            {
                _scroll.SetActive(false);
                _anim.SecondClickNextButton(_m1, _m2);
                //StopCoroutine(a);
                _textCTL.GENERATETEXT(_m1, _m2,_m3);
                StartCoroutine(WaitGenerating());
                _back.SetActive(false);
                _setting.SetActive(false);
                GameManager.instance.NameOfMonster(_m1, _m2, _m3);
                GameManager.instance.countMerge2 += 1;
            }
            else
            {
                _textCTL.PICKAMOMORMONSTER(_m3);
            }
           
            _min2 = _oldObject.transform.GetComponent<ClickButtonInGame>()._objectData.monster_id;
            BaseActionofNext();
            return;
        }
        GameManager.instance.countMerge3 += 1;
        GameManager.instance.NameOfMonster(_m1, _m2, _m3);
        _setting.SetActive(false);
        _back.SetActive(false);
        _nextbutton.SetActive(false);
        _id3 = _oldObject.transform.GetComponent<ClickButtonInGame>()._objectData.monster_id;
        _anim.ThirdClickNextButton(_m1, _m2, _m3);
        //StopCoroutine(a);
        _scroll.gameObject.SetActive(false);
        _textCTL.GENERATETEXT(_m1, _m2, _m3);
        StartCoroutine(WaitGenerating());
        
    }

    IEnumerator WaitGenerating()
    {
        BL_AdManager.Instance.ShowMrec();
        BL_AdManager.Instance.HideBanner();
        GameManager.instance.CountResult += 1;
        yield return new WaitForSeconds(0.4f);

        _m1.transform.GetComponent<Image>().enabled = false;
        _m2.transform.GetComponent<Image>().enabled = false;
        if (_m3 != null)
        {
            _m3.transform.GetComponent<Image>().enabled = false;
        }
        _vfxControll.VFXfusion_ON(_resultIM.transform.gameObject);
        yield return new WaitForSeconds(3.55f);

        _vfxControll.VFXfusion_OFF();
        _vfxControll.VFXComplete_ON(_resultIM.transform.gameObject);
        yield return new WaitForSeconds(0.2f);
        Result();
        _resultIM.gameObject.SetActive(true);
        _resultIM.enabled = true;
        SoundEffect.instance.PlaySound(_fusionCompleted);
        yield return new WaitForSeconds(0.6f);
        _vfxControll.VFXComplete_OFF();
    }

    public void CreateMoreButton()
    {
        if (SceneManager.GetActiveScene().buildIndex != 4)
        {
            _scroll.transform.GetComponent<ScrollRect>().enabled = false;
        }
        foreach(var t in _listImage)
        {
            t.SetActive(false);
        }
        _back.SetActive(true);
        BL_AdManager.Instance.HideMrec();
        GameManager.instance.TriggerAds();
        BL_AdManager.Instance.ShowBanner();
        
        SoundEffect.instance.PlaySound(_ClickSound); 
        dem = 0;
        
        _starManager.CREATEMOREBTN();
        _textCTL.Reset();
        _resultIM.transform.DOMoveX(300, 0.15f).OnComplete(() =>
        {
            _resultIM.gameObject.SetActive(false);
            _resultIM.transform.DOMoveX(0, 0.05f).OnComplete(() =>
            {
                Reset();
            });
            _contentInScroll.DOAnchorPosY(_beginPosY, 0.01f);
        
            _textCTL.PICKAMONSTERORDAD();
        });
        
    }
    void Reset()
    {
        _starManager.ResetListInstance();
        _scroll.SetActive(true);
        _anim.Resett(_m1, _m2, _m3, _beginPosM2, _template,_beginposYofM1);
            StartCoroutine(ShowAgain());
            return;
    }
    IEnumerator ShowAgain()
    {
        int i;
        for ( i = 0; i < _listImage.Count; i++)
        {
            Transform t = _listImage[i].transform.GetChild(0);
            BaseDATA a = t.GetComponent<ClickButtonInGame>()._objectData;
            if (PlayerPrefs.HasKey("adsMons" + a.monster_id) && PlayerPrefs.GetString("adsMons" + a.monster_id)=="true")
            {
                _listImage[i].transform.GetChild(1).gameObject.SetActive(true);
            }
            _listImage[i].transform.GetChild(0).gameObject.SetActive(true);
            _listImage[i].transform.GetChild(0).GetComponent<Button>().interactable = true;
            _listImage[i].SetActive(true);
            _listImage[i].transform.GetComponent<Image>().enabled = true;
            _listImage[i].transform.localScale = Vector3.one * 0.6f;
            _listImage[i].transform.DOScale(Vector3.one, 0.4f);
            yield return new WaitForSeconds(0.1f);
        }
        if (SceneManager.GetActiveScene().buildIndex != 4)
        {
            while (i < _listImage.Count - 1)
            {
                continue;
            }
            _vfxControll.ActiveScroll();
            _scroll.transform.GetComponent<ScrollRect>().enabled = true;
        }
        
    }
    void Result()
    {
        int[] g = { _min1, _min2, _id3 };
        Array.Sort(g);
        int tmp;
        if(SceneManager.GetActiveScene().buildIndex == 2)
        {
            tmp= g[1] * 100 + g[2];
            GameManager.instance.CountRsMerge2();
        }
        else
        {
             tmp = g[0] * 100 + g[1] * 10 + g[2] ;
            GameManager.instance.CountRsMerge3();
        }
        
        foreach (var t in GameManager.instance._templateresult)
        {
            if (tmp == t.monster_id)
            {
                _resDT = t;
                Image a = _resultIM.GetComponent<Image>();
                a.transform.localScale = new Vector2(2.8f, 2.8f);
                a.transform.DOScale(new Vector2(2.9f , 2.9f), 0.4f).OnComplete(() =>
                {
                    _starManager.RESULT();
                    _starManager.InstanceStar(t.monster_star, _timeShowStar,
                    _starManager._starBar, _starManager._bgStarBar, 0);
                    _textCTL.RESULTTEXT( t);
                    
                });
                a.sprite = Resources.Load<Sprite>("Images/" + tmp);
                _textCTL.MONSTERRESULTNAME(t);
                JSON.SaveToJson(t);
                break;
            } 
        }
        
    }
    void BaseActionofNext()
    {
        
        foreach (GameObject t in _listImage)
        {
            t.transform.GetChild(0).GetComponent<Button>().interactable = false;
        }
        _vfxControll.FadeIn(0, dem);
        _nextbutton.SetActive(false);
        //button.transform.parent.localScale = CONSTANT.ScaleBeginPar;
        dem += 1;
        _oldObject = null;//reset
    }
}
