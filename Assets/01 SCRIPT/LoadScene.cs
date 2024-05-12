using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [SerializeField] GameObject _internetNotify;
    [SerializeField] AudioClip _clickSound;
    public void Load(int map)
    {
        GameManager.instance.SceneBefore = SceneManager.GetActiveScene().buildIndex;
        SoundEffect.instance.PlaySound(_clickSound);
        StartCoroutine(GameManager.instance.CheckInternet2(_internetNotify));
        if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            BL_AdManager.Instance.HideMrec();
        }
        while (FirebaseManager.instance.IsInternetAvailable() == false)
        {
            return;
        }
        SceneManager.LoadScene(map);
    }
    public void LoadWithAds2(int map)
    {
        GameManager.instance.SceneBefore = SceneManager.GetActiveScene().buildIndex;
        GameManager.instance.canplaySound = false;
        SoundEffect.instance.PlaySound(_clickSound);
        //StartCoroutine(GameManager.instance.CheckInternet2(_internetNotify));
        //while (FirebaseManager.instance.IsInternetAvailable() == false)
        //{
        //        continue;
        //}
        BL_AdManager.Instance.HideMrec();
        BL_AdManager.Instance.ShowBanner();
        SceneManager.LoadScene(map);
        GameManager.instance.InterAds();
        GameManager.instance.TriggerAds();
    }
    public void LoadWithAds(int map)
    {
        GameManager.instance.SceneBefore = SceneManager.GetActiveScene().buildIndex;
        GameManager.instance.canplaySound = false;
        SoundEffect.instance.PlaySound(_clickSound);

        StartCoroutine(LoadSceneWithAds(map));
    }

    private IEnumerator LoadSceneWithAds(int map)
    {
        if(FirebaseManager.instance.IsInternetAvailable() == false)
        {
            yield return StartCoroutine(GameManager.instance.CheckInternet2(_internetNotify));
        }
        else
        {
            BL_AdManager.Instance.HideMrec();
            BL_AdManager.Instance.ShowBanner();
            
            SceneManager.LoadScene(map);
            GameManager.instance.InterAds();
            GameManager.instance.TriggerAds();
        }
    }

    public void Back()
    {
        GameManager.instance.canplaySound = false;
        SoundEffect.instance.PlaySound(_clickSound);
        StartCoroutine(GameManager.instance.CheckInternet2(_internetNotify));
        
        StartCoroutine(LoadSceneWithAds(GameManager.instance.SceneBefore));
    }
    public void LoadNormal(int map)
    {
        GameManager.instance.SceneBefore = SceneManager.GetActiveScene().buildIndex;
        GameManager.instance.canplaySound = false;
        SoundEffect.instance.PlaySound(_clickSound);
        SceneManager.LoadScene(map);
    }
    public void HomeBUtINGAme(int map)
    {
        GameManager.instance.SceneBefore = SceneManager.GetActiveScene().buildIndex;
        GameManager.instance.canplaySound = false;
        SoundEffect.instance.PlaySound(_clickSound);
        StartCoroutine(LoadSceneWithAds(map));
        //GameManager.instance.TriggerAds();
        //BL_AdManager.Instance.HideMrec();
        //SceneManager.LoadScene(map);
        //BL_AdManager.Instance.ShowBanner();

    }
    public void Merge3Scene(int map)
    {
        
        SoundEffect.instance.PlaySound(_clickSound);
        StartCoroutine(GameManager.instance.CheckInternet2(_internetNotify));

        StartCoroutine(Wait(map));
    }
    IEnumerator Wait(int map)
    {
        while (FirebaseManager.instance.IsInternetAvailable() == false)
        {
            yield return new WaitForSeconds(1);
        }
        BL_AdManager.Instance.ShowRewardedAd(() => Success(map), Fail);
    }
    void Success(int map)
    {
        GameManager.instance.Merge3ButtonDone();
        GameManager.instance.SceneBefore = SceneManager.GetActiveScene().buildIndex;
        GameManager.instance.canplaySound = false;
        BL_AdManager.Instance.HideMrec();
        SceneManager.LoadScene(map);
    }
    void Fail()
    {

    }
    public void DeactiveInternetNotify()
    {
        SoundEffect.instance.PlaySound(_clickSound);
        //Debug.LogError("turn off internetPanel");
        _internetNotify.transform.GetChild(0).gameObject.SetActive(false);
    }
    public void ViewPrivacy(string a)
    {
        SoundEffect.instance.PlaySound(_clickSound);
        Application.OpenURL(a);
    }
}