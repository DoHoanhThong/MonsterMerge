using DG.Tweening;
using NSubstitute.Core;
using System.Collections;
using System.Collections.Generic;
using TeraJet;
using Unity.VisualScripting;
using UnityEngine;
public class StarRate : MonoBehaviour
{
    
    [SerializeField] GameObject _fadePanel;
    [SerializeField] AudioClip _clickSound;
    [SerializeField] GameObject _thanks;
    private void Start()
    {
        _thanks.SetActive(false);
        this.transform.GetChild(0).gameObject.SetActive(false);
    }
    public void OnClick()
    {
        SoundEffect.instance.PlaySound(_clickSound);
        if (this.transform.GetChild(0).gameObject.activeSelf)
            return;
        StartCoroutine(wait());

    }
    IEnumerator wait()
    {
        GameManager.instance.TotalStarRate = 0;   
        int index = this.transform.GetSiblingIndex();
        Transform parent = this.transform.parent;
        for (int i = 0; i <= index; i++)
        {
            GameObject t = parent.GetChild(i).gameObject;
            t.transform.GetChild(0).gameObject.SetActive(true);
            GameManager.instance.TotalStarRate += 1;
        }
        yield return new WaitForSeconds(0.7f);
        if (GameManager.instance.TotalStarRate <= 4)
        {
            GameManager.instance.RateUnder5Star(GameManager.instance.TotalStarRate);
            yield return new WaitForSeconds(0.1f);
            parent.parent.DOScale(Vector3.one * 0.6f, 0.1f).OnComplete(() =>
            {
                parent.parent.gameObject.SetActive(false);
                _thanks.SetActive(true);
                _thanks.transform.localScale = Vector2.one * 0.6f;
                _thanks.transform.DOScale(Vector2.one, 0.1f);
            });
        }
        else
        {
            parent.parent.gameObject.SetActive(false);
            IAPManager.instance.Call();
            GameManager.instance.Rate5Star();
            _fadePanel.SetActive(false);
        } 
    }
}
