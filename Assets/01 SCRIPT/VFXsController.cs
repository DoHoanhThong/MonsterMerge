using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Reflection;

public class VFXsController : MonoBehaviour
{
    BaseDATA _tmp;
    [SerializeField] Image _scrollBar;
    [SerializeField] GameObject _handle;
    [SerializeField] GameObject _completeVFX;
    [SerializeField] GameObject _vfxtmp1, _vfxtmp2;
    [SerializeField] GameObject _fusionVFX;
    [SerializeField] GameObject _VFXcollect;
    [SerializeField] UIManagerInGame _uiMan;
    [SerializeField] ScrollRect _scrollrect;
    [SerializeField] AudioClip _collectMonster;
    public void FadeIn(int index, int dem)
    { 
        //if (_scrollBar != null && _handle!=null)
        //{
        //    _scrollBar.enabled = false;
        //    _handle.SetActive(false);
        //}
        //if (index >= _uiMan._listImage.Count)
        //{
        //    _uiMan.button.transform.parent.gameObject.SetActive(false);
        //    if (dem == 0 || (SceneManager.GetActiveScene().buildIndex == 4 && dem==1))
        //    {
        //        FadeOut(0);
        //    }
        //    return;
        //}
        //_uiMan._listImage[index].transform.DOScale(Vector3.one *0.6f, 0.04f).OnComplete(() =>
        //{
        //    _uiMan._listImage[index].transform.GetChild(1).gameObject.SetActive(false);
        //    _uiMan._listImage[index].transform.GetChild(0).gameObject.SetActive(false);
        //    _uiMan._listImage[index].GetComponent<Image>().enabled = false;
        //    FadeIn(index + 1,dem );
        //});
        StartCoroutine(FadeInn(dem));
    }
    IEnumerator FadeInn(int dem)
    {
        
        if (_scrollBar != null && _handle != null)
        {
            _scrollBar.enabled = false;
            _handle.SetActive(false);
        }
        int i = 0;
        for (i = 0; i < _uiMan._listImage.Count; i++)
        {
            int currentIndex = i;
            _uiMan._listImage[currentIndex].transform.DOScale(Vector3.one * 0.6f, 0.05f).OnComplete(() =>
            {
                _uiMan._listImage[currentIndex].transform.GetChild(1).gameObject.SetActive(false);
                _uiMan._listImage[currentIndex].transform.GetChild(0).gameObject.SetActive(false);
                _uiMan._listImage[currentIndex].GetComponent<Image>().enabled = false;
               
            });
            yield return new WaitForSeconds(0.05f);
        }
        while (i < _uiMan._listImage.Count)
        {
            continue;
        }
        _uiMan.button.transform.parent.gameObject.SetActive(false);
        if (dem == 0 || (SceneManager.GetActiveScene().buildIndex == 4 && dem == 1))
        {
            
            StartCoroutine(FadeOutt());
        }

    }
    IEnumerator FadeOutt()
    {
        Vector3 a = _uiMan._contentInScroll.anchoredPosition;
        a.y = _uiMan._beginPosY;
        
        int i = 0;
        
        for (i = 0; i < _uiMan._listImage.Count; i++)
        {
            Transform t = _uiMan._listImage[i].transform.GetChild(0);
            _tmp = t.GetComponent<ClickButtonInGame>()._objectData;
            if (PlayerPrefs.HasKey("adsMons"+_tmp.monster_id) && PlayerPrefs.GetString("adsMons" + _tmp.monster_id)=="true")
            {
                _uiMan._listImage[i].transform.GetChild(1).gameObject.SetActive(true);
            }
            _uiMan._listImage[i].transform.GetChild(0).GetComponent<Button>().interactable = true;
            _uiMan._listImage[i].transform.GetChild(0).gameObject.SetActive(true);
            _uiMan._listImage[i].GetComponent<Image>().enabled = true;
            _uiMan._listImage[i].transform.DOScale(Vector3.one, 0.05f);
            _uiMan._contentInScroll.anchoredPosition = a;
            yield return new WaitForSeconds(0.05f);
        }
        while (i < _uiMan._listImage.Count)
        {
            continue;
        }
        if (_scrollrect != null)
        {
            _scrollrect.enabled = true;
        }
        ActiveScroll();
    }
    //public void FadeOut(int index)
    //{

    //    Vector3 a = _uiMan._contentInScroll.anchoredPosition;
    //    a.y = _uiMan._beginPosY;
    //    _uiMan._contentInScroll.anchoredPosition = a;

    //    if (index >= _uiMan._listImage.Count)
    //    {
    //        if (_scrollrect != null)
    //        {
    //            _scrollrect.enabled = true;
    //        }
    //        ActiveScroll();
    //        return;
    //    }
    //    Transform t = _uiMan._listImage[index].transform.GetChild(0);
    //    if (t.GetComponent<ClickButtonInGame>()._objectData.isAds)
    //    {
    //        _uiMan._listImage[index].transform.GetChild(1).gameObject.SetActive(true);
    //    }
    //    _uiMan._listImage[index].transform.GetChild(0).GetComponent<Button>().interactable = true;
    //    _uiMan._listImage[index].transform.GetChild(0).gameObject.SetActive(true);
    //    _uiMan._listImage[index].GetComponent<Image>().enabled = true;
    //    _uiMan._listImage[index].transform.DOScale(Vector3.one, 0.04f).OnComplete(() =>
    //    {
    //        FadeOut(index + 1);
    //    });
    //}
    
    public void VFXfusion_ON(GameObject m1)
    {
        _fusionVFX.SetActive(true);
        if(SceneManager.GetActiveScene().buildIndex == 2)
        {
            _fusionVFX.transform.position = new Vector3(m1.transform.position.x, m1.transform.position.y -0.65f, 1);
        }
        else
        {
            _fusionVFX.transform.position = new Vector3(m1.transform.position.x, m1.transform.position.y - 0.8f, 1);
        }
        _fusionVFX.transform.localScale = Vector3.one *700;
    }
    public void VFXfusion_OFF()
    {
        _fusionVFX.SetActive(false);
    }
    public void VFXComplete_ON(GameObject m1)
    {
        _completeVFX.SetActive(true);
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            _completeVFX.transform.position = new Vector3(m1.transform.position.x, m1.transform.position.y -1, 1);
        }
        else
        {
            _completeVFX.transform.position = new Vector3(m1.transform.position.x, m1.transform.position.y - 1f, 1);
        }
        _completeVFX.transform.localScale = Vector3.one *200;
    }
    public void VFXComplete_OFF()
    {
        _completeVFX.SetActive(false);
    }
    public void VFXcollect(GameObject result, GameObject collection, GameObject desti, GameObject fadePanel, GameObject collectionCopy)
    {
        collectionCopy.SetActive(true);
        fadePanel.SetActive(true);
        _VFXcollect.transform.position= new Vector2(result.transform.position.x, result.transform.position.y-1);
        _VFXcollect.SetActive(true);
        _VFXcollect.transform.localScale = Vector3.one * 90;
        _VFXcollect.transform.DOJump(desti.transform.position, 0.2f,1,0.5f,false);
        _VFXcollect.transform.DOScale(Vector3.one * 170, 0.5f);
        _VFXcollect.transform.DOMove(collection.transform.position, 0.6f).SetDelay(0.4f);
        _VFXcollect.transform.DOScale(Vector3.one * 80, 0.6f).SetDelay(0.4f).OnComplete(() =>
            {
                collectionCopy.transform.DOScale(Vector2.one*1.4f, 0.2f);
                collectionCopy.transform.DOScale(Vector2.one, 0.2f).SetDelay(0.2f).OnComplete(() =>
                {
                    fadePanel.SetActive(false);
                    collectionCopy.SetActive(false);
                });
                SoundEffect.instance.PlaySound(_collectMonster);
                
                
                _VFXcollect.SetActive(false);
            });
       
    }
    public void ActiveScroll()
    {
        if(_handle!=null && _scrollBar != null)
        {
            _handle.SetActive(true);
            _scrollBar.enabled = true;
            _scrollBar.transform.GetComponent<CanvasGroup>().alpha = 0;
            _scrollBar.transform.GetComponent<CanvasGroup>().DOFade(1, 0.2f);
        }
        
    }
}
