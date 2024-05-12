using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashController : MonoBehaviour
{
    //float adsWaitCounter = 0;
    void Start()
    {
        //IAPManager.instance.Call();
        StartCoroutine(InitializeScene());
        
        if (!PlayerPrefs.HasKey("SFXVolume"))
        {
            SoundEffect.instance._soundsource.volume = 1f;
            PlayerPrefs.SetFloat("SFXVolume", 1f);
            PlayerPrefs.SetInt(CONSTANT.SFX, 1);
        }
        if (!PlayerPrefs.HasKey(CONSTANT.Music))
        {
            BGMusic.instance._soundsource.Play();
            PlayerPrefs.SetInt(CONSTANT.Music, 1);
            BGMusic.instance._soundsource.volume = 0.5f;
        }
        if (PlayerPrefs.GetInt(CONSTANT.SFX) == 0)
        {
            SoundEffect.instance._soundsource.volume = 0;
            PlayerPrefs.SetFloat("SFXVolume", 0);
        }
        else
        {
            SoundEffect.instance._soundsource.volume = 1f;
            PlayerPrefs.SetFloat("SFXVolume", 1f);
        }

        if (PlayerPrefs.GetInt(CONSTANT.Music) == 0)
        {
            BGMusic.instance._soundsource.Stop();
        }
        else
        {
            BGMusic.instance._soundsource.Play();
        }
    }
    IEnumerator InitializeScene()
    {

        StartCoroutine(SplashCanvasController.Instance.StartLoading());

        yield return new WaitForSeconds(0.5f);

        FirebaseManager.instance.Initialize();

        //load player data
        //GameManager.Instance.userData = GameTool.LoadUserData();
        //GameManager.OnUserDataLoaded?.Invoke();

        SplashCanvasController.Instance.SetMaxProgress(0.2f);

        //ShopController.Instance.Initialize();

        AsyncOperation operation = SceneManager.LoadSceneAsync(1);

        operation.allowSceneActivation = false;

        //load firebase
        FirebaseManager.instance.Initialize();

        SplashCanvasController.Instance.SetMaxProgress(0.4f);

        //while (AdController.instance.IsInitialized == false && adsWaitCounter < 4)
        //    while (adsWaitCounter < 4)
        //    {
        //        adsWaitCounter += Time.deltaTime;
        //        yield return new WaitForEndOfFrame();
        //    }
        yield return new WaitForSeconds(0.5f);
        SplashCanvasController.Instance.SetMaxProgress(0.7f);
        BL_AdManager.Instance.ShowBanner();
        yield return new WaitForSeconds(0.05f);
       // Debug.LogError("show bannerads in splash complete!");
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        float waiter = 0;
        //while (waiter <= 3f && !GameManager.Instance.IsOAAReady())
        while (waiter <= 3f)
        {
            waiter += 0.5f;
            yield return wait;
        }
        //if (GameManager.Instance.userData.isFirstTimeOpen)
        //{
        //    PrivacyPopup.OnFirstTimeOpenApp?.Invoke();
        //}


        yield return wait;
       

        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                SplashCanvasController.Instance.SetMaxProgress(1f);
                yield return new WaitUntil(() => SplashCanvasController.Instance.isCanvasLoadDone == true);
                while (!FirebaseManager.instance.IsInternetAvailable())
                {
                    yield return new WaitForSeconds(1);
                }
                operation.allowSceneActivation = true;

            }
            yield return null;
        }

        //GameManager.Instance.OnLevelLoaded?.Invoke();
    }
    private void Update()
    {
        
    }
}
