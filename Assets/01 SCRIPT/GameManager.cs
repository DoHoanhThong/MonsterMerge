using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.IO;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;

public class GameManager : Singleton<GameManager>
{
    public bool isSortListColection;
    [SerializeField] string _linkRateStar;
    public bool CanloadScene;
    public int countMerge;
    public Sprite[] _listImageStar= new Sprite[10];
    public ResultDATA[] _templateresult;
    public bool CanShowCappingADs;
    public int CountResult;
    public bool canplaySound;
    public int countMerge2, countMerge3;
    public bool haveReadPrivacy;
    public int TotalStarRate;
    public int SceneBefore;
    [SerializeField] int[] adsMons = new int[3];
    private void Start()
    {
        
        foreach(int i in adsMons)
        {
            if (!PlayerPrefs.HasKey("adsMons" + i))
            {
                PlayerPrefs.SetString("adsMons" + i, "true");
                
            }
        }
        //OnTotalStarRateReached += LoadLinkRate;
        TotalStarRate = 0;
        if (!PlayerPrefs.HasKey("ReadPrivacy"))
        {
            PlayerPrefs.SetInt("ReadPrivacy", 0);
            
        }
        canplaySound = false;
        CountResult = 0;
        CanloadScene = false;
        CanShowCappingADs = false;
        Application.targetFrameRate = 60;
    }
   
    public void LoadLinkRate()
    {
        Application.OpenURL(_linkRateStar);
    }
    public void RateUnder5Star(int star)
    {
        
        FirebaseManager.instance.SendNormalEvent(string.Format("rate_{0}_star", star));
    }
    public void Rate5Star() 
    {
        
        FirebaseManager.instance.SendNormalEvent("rate_5_star");
    }
    public void CountRsMerge2()
    {
        
        FirebaseManager.instance.SendNormalEvent(string.Format("level_merge_2_count_{0}",countMerge2 ));
    }
    public void CountRsMerge3()
    {
        
        FirebaseManager.instance.SendNormalEvent(string.Format("level_merge_3_count_{0}", countMerge3));
    }

    public void NameOfMonster(GameObject m1, GameObject m2, GameObject m3)
    {
        
        
        FirebaseManager.instance.SendNormalEvent(string.Format("monster_{0}_selected", m1.name));
        FirebaseManager.instance.SendNormalEvent(string.Format("monster_{0}_selected", m2.name));
        
        if (m3 != null)
        {
           
            FirebaseManager.instance.SendNormalEvent(string.Format("monster_{0}_selected", m3.name));
        }
    }
    public void Merge3ButtonDone()
    {
        
        FirebaseManager.instance.SendNormalEvent("result_click_merge3");
    }
    //public void OnClickSaveButton()
    //{
    //    Debug.LogError("EVENT : CLICK SAVE");
    //    FirebaseManager.instance.SendNormalEvent("menu_click_saved");
    //}
    public IEnumerator CheckInternet1(float currentProgress, GameObject _internetNotify)
    {
        while (currentProgress < 1)
        {
            yield return new WaitForSeconds(1f);
        }
        while (FirebaseManager.instance.IsInternetAvailable() == false)
        {
            CanloadScene = false;
            if (!_internetNotify.activeSelf)
            {
                _internetNotify.SetActive(true);
            }

            
            if (!_internetNotify.transform.GetChild(0).gameObject.activeSelf)
            {
                
                _internetNotify.transform.GetChild(0).gameObject.SetActive(true);
            }
            yield return new WaitForSeconds(1);
            
        }
        CanloadScene = true;
        _internetNotify.SetActive(false);
        yield return new WaitForSeconds(1);
    }
    public IEnumerator CheckInternet2(GameObject _internetNotify)
    {
        while (FirebaseManager.instance.IsInternetAvailable() == false)
        {
            if (!_internetNotify.activeSelf)
            {
                _internetNotify.SetActive(true);
            }

            
            if (!_internetNotify.transform.GetChild(0).gameObject.activeSelf)
            {
                
                _internetNotify.transform.GetChild(0).gameObject.SetActive(true);
            }
            yield return new WaitForSeconds(1);

        }

        _internetNotify.SetActive(false);
        yield return new WaitForSeconds(1);
    }
    public void InterAds()
    {
        if (CountResult == 3)
        {
            BL_AdManager.Instance.ShowInterstitial();
            return;
        }
    }
    public void TriggerAds()
    {
        if (CountResult >= 3)
        {
            BL_AdManager.Instance.ShowInterstitial();
        }
    }
    
}

    

