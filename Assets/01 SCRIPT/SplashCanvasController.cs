using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class SplashCanvasController : MonoBehaviour
{
    #region Singleton

    [SerializeField] bool isDontDestroyOnLoad;
    public static SplashCanvasController Instance;
    public void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        if (isDontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    #endregion
    [SerializeField] Text _percent;
    [SerializeField] AudioClip _clickSound;
    [SerializeField] GameObject _internetNotify;
    [SerializeField] Canvas _canvas;
    [SerializeField] List<GameObject> _listObject = new List<GameObject>();
    public bool isCanvasLoadDone;
    [SerializeField] float loadingFakeSpeed = 3;
    [SerializeField] Image imgLoadingFill;
    [SerializeField] RectTransform _iconRect;

    float currentProgress = 0;
    float maxProgress = 0;

    float fillWidth;
    private void Start()
    {
        _percent.text = "0%";
        StartCoroutine(Zoom(0));
        StartCoroutine(Zoom(1));
        StartCoroutine(Zoom(2));
    }

    public IEnumerator StartLoading()
    {
        currentProgress = 0;
        fillWidth = imgLoadingFill.rectTransform.sizeDelta.x;
        float y = _iconRect != null ? _iconRect.anchoredPosition.y : 0;
        
        imgLoadingFill.fillAmount = currentProgress;
        _percent.text = Mathf.FloorToInt(imgLoadingFill.fillAmount * 100).ToString() + "%";
        isCanvasLoadDone = false;
        while (currentProgress < 1)
        {
            if (currentProgress <= maxProgress)
            {
                currentProgress += Time.deltaTime * loadingFakeSpeed;
                currentProgress = Mathf.Clamp(currentProgress, 0, maxProgress);
                imgLoadingFill.fillAmount = currentProgress;
                _percent.text = Mathf.FloorToInt(imgLoadingFill.fillAmount * 100).ToString() + "%";
                if (_iconRect != null)
                {
                    _iconRect.anchoredPosition = new Vector2(fillWidth * currentProgress, y);
                }
            }
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(GameManager.instance.CheckInternet1(currentProgress, _internetNotify));
            
        isCanvasLoadDone = true;

    }
    public void SetMaxProgress(float value)
    {
        maxProgress = value;
    }
    public void SetProgress(float value)
    {
        imgLoadingFill.fillAmount = value;
    }

    IEnumerator Zoom(int index)
    {
        GameObject g = _listObject[index];
        yield return new WaitForSeconds(0.2f + (index+1) * 0.2f);
        while (currentProgress < 1)
        {
            g.transform.DOScale(new Vector2(1.045f + index * 0.02f, 1.045f + index * 0.02f), 0.9f + 0.12f*index).OnComplete(() =>
            {
                g.transform.DOScale(new Vector2(1 - index * 0.1f, 1 - index * 0.1f), 0.9f + 0.12f*index);
            });
            yield return new WaitForSeconds(1.3f);
        }
    }

    public void DeactiveInternetNotify()
    {
        SoundEffect.instance.PlaySound(_clickSound);
        //Debug.LogError("turn off internetPanel");
        _internetNotify.transform.GetChild(0).gameObject.SetActive(false);
    }
}
