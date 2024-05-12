using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class AnimationOfImage : MonoBehaviour
{
    [SerializeField] Transform _posOfM3;
    [SerializeField]Vector3 _beginPosM3;
    [SerializeField] AudioClip _fusionSound;
    public Tween A, B, C,D,E,F;
    [SerializeField]bool _isDelay;
    public List<Tween> _listTween = new List<Tween>();
    public bool canDoTween;
    private void Awake()
    {
        canDoTween = false;
        _isDelay = false;
        if (_posOfM3 == null)
            return;
        _beginPosM3 = _posOfM3.position;
    }
    public void SpawnImage( Transform parentOfButton, Transform tmpGameObject, BaseDATA tmpdata)
    {
        _isDelay = false;
        foreach (var t in _listTween)
        {
            t.Kill();
            
        }
       
           
        canDoTween = false;
        
        D =parentOfButton.DOScale(CONSTANT.ScaleFirstZoomPar, 0.08f); _listTween.Add(D);
        E =parentOfButton.DOScale(CONSTANT.ScaleSecondZoomPar, 0.08f).SetDelay(0.08f); _listTween.Add(E);
        F =parentOfButton.DOScale(CONSTANT.ScaleLastScalePar, 0.1f).SetDelay(0.16f); _listTween.Add(F);
        if(SceneManager.GetActiveScene().buildIndex == 2)
        {
            B = tmpGameObject.DOScale(new Vector2(3.1f, 3.1f), 0.1f);
            C = tmpGameObject.DOScale(new Vector2(2.8f, 2.8f), 0.1f).SetDelay(0.1f).OnComplete(() =>
            {
                canDoTween = true;
                _isDelay = true;
                Breathing(tmpGameObject.gameObject, tmpdata);
            });
        }
        else
        {
            B = tmpGameObject.DOScale(new Vector2(2.8f, 2.8f), 0.1f);
            C = tmpGameObject.DOScale(new Vector2(2.6f, 2.6f), 0.1f).SetDelay(0.1f).OnComplete(() =>
            {
                canDoTween = true;
                _isDelay = true;
                Breathing(tmpGameObject.gameObject, tmpdata);
            });
        }
        _listTween.Add(B); _listTween.Add(C);
    }
    public void FirstClickNextButton(GameObject m1)
    {
        if (SceneManager.GetActiveScene().buildIndex != 4)
        {
            m1.GetComponent<RectTransform>().DOAnchorPosX(-250, 1f);
            return;
        }
        m1.GetComponent<RectTransform>().DOAnchorPosX(-300, 1f);
    }
    public void SecondClickNextButton(GameObject m1, GameObject m2)
    {
        m1.transform.GetComponent<RectTransform>().DOAnchorPosX(-50, 0.5f); //MOVE and ZOOM m1
        m2.transform.GetComponent<RectTransform>().DOAnchorPosX(50, 0.5f); // MOVE and ZOOM m2
        m1.transform.DOScale(new Vector2(m1.transform.localScale.x / 3, m1.transform.localScale.y / 3), 0.5f);
        m2.transform.DOScale(new Vector2(m2.transform.localScale.x / 3, m2.transform.localScale.y / 3), 0.5f).OnComplete(() =>
        {
            SoundEffect.instance.PlaySound(_fusionSound);
        });
    }
    public void ThirdClickNextButton(GameObject m1, GameObject m2, GameObject m3)
    {
        m1.transform.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-50,-400), 0.6f); //MOVE and ZOOM m1
        m2.transform.GetComponent<RectTransform>().DOAnchorPos(new Vector2(50, -400), 0.6f);
        m3.transform.GetComponent<RectTransform>().DOAnchorPosY(-529, 0.5f);// MOVE and ZOOM m2
        m1.transform.DOScale(new Vector2(m1.transform.localScale.x / 3, m1.transform.localScale.y / 3), 0.5f);
        m2.transform.DOScale(new Vector2(m2.transform.localScale.x / 3, m2.transform.localScale.y / 3), 0.5f);
        m3.transform.DOScale(new Vector2(m3.transform.localScale.x / 3, m3.transform.localScale.y / 3), 0.5f).OnComplete(() =>
        {
            SoundEffect.instance.PlaySound(_fusionSound);
        });
    }
    public void Resett(GameObject m1, GameObject m2,GameObject m3, Vector2 beginPosM2, Image _template, float beginposYofM1)
    {
        if (m3 != null)
        {
            m3.transform.GetComponent<Image>().sprite = _template.sprite;
            m3.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,-630);
            m3.transform.localScale = new Vector3(2.6f, 2.6f);
            m2.transform.localScale = new Vector3(2.6f, 2.6f);
            m1.transform.localScale = new Vector3(2.6f, 2.6f);
        }
        else
        {
            m2.transform.localScale = new Vector3(2.8f, 2.8f, 1);
            m1.transform.localScale = new Vector3(2.8f, 2.8f, 1);
        }
        m1.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, beginposYofM1);
        m2.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(300, beginposYofM1);
       

        m1.transform.GetComponent<Image>().enabled = true;
        m1.transform.GetComponent<Image>().sprite = _template.sprite;
        m2.transform.GetComponent<Image>().sprite = _template.sprite;
        
    }
    public void Breathing(GameObject g, BaseDATA tmpdata)
    {
        if (canDoTween)
        {
            float time = (_isDelay) ? 0.2f : 0;
            if(SceneManager.GetActiveScene().buildIndex == 2)
            {
                A = g.transform.DOScale(new Vector3(2.9f, 2.9f), 0.8f).SetDelay(time).OnComplete(() => {
                    g.transform.DOScale(new Vector2(2.8f, 2.8f), 0.8f).OnComplete(() => {
                        Breathing(g, tmpdata);
                    });
                });

            }
            else
            {
                A = g.transform.DOScale(new Vector3(2.7f, 2.7f), 0.8f).SetDelay(time).OnComplete(() => {
                    g.transform.DOScale(new Vector2(2.6f, 2.6f), 0.8f).OnComplete(() => {
                        Breathing(g, tmpdata);
                    });
                });

            }
            _listTween.Add(A);
            _isDelay = false;
        }
        
    }
    
    
}
