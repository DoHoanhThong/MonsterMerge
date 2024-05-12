using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TextController : MonoBehaviour
{
    [SerializeField] Text _structureText;
    public Text _Rarity;
    [SerializeField] Text _topText;
    [SerializeField] Text _viewDetail;

    private void Awake()
    {
        _Rarity.gameObject.SetActive(true);
        _structureText.transform.parent.gameObject.SetActive(false);
        _structureText.transform.gameObject.SetActive(true);
        _structureText.text = "";
        _Rarity.enabled = false;
        if(_viewDetail == null)
            return;
        _viewDetail.gameObject.SetActive(false);

    }
    public void PICKAMONSTERORDAD()
    {
        if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            _topText.text = "PICK A MONSTER";
            return;
        }
        _topText.text = "PICK A DAD";
    }
   
    public void CLICKMONSTER(BaseDATA a)
    {
        _topText.text = a.monster_name;
    }
    public void PICKAMOMORMONSTER(GameObject m2)
    {
        
        _topText.transform.GetComponent<CanvasGroup>().DOFade(0, 0.35f).OnComplete(() =>
        {
            if (SceneManager.GetActiveScene().buildIndex == 4)
            {
                _topText.text= "PICK A MONSTER";
            }
            else _topText.text = "PICK A MOM";
        });
        _topText.transform.GetComponent<CanvasGroup>().DOFade(1, 0.35f).SetDelay(0.35f).OnComplete(() =>
        {
            m2.transform.GetComponent<Image>().enabled = true;
        });
    }

    public void GENERATETEXT(GameObject m1, GameObject m2, GameObject m3)
    {
        _topText.transform.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() =>
        {
            _topText.text = "GENERATING RESULT";
            string a = m1.name + " + " + m2.name;
            _structureText.text = (m3 == null) ? a : a + " + " + m3.name;
            _structureText.transform.parent.gameObject.SetActive(true);
        });
        _topText.transform.GetComponent<CanvasGroup>().DOFade(1, 0.3f).SetDelay(0.5f);
    }
    public void RESULTTEXT(ResultDATA t)
    {
  
        _Rarity.text = t.monster_rarity;
    }
    public void MONSTERRESULTNAME(ResultDATA t)
    {
        _topText.text = t.monster_name;
    }
    public void Reset()
    {
        _structureText.transform.parent.gameObject.SetActive(false);
        _structureText.text = "";
        _Rarity.enabled = false;
    }
    
}
