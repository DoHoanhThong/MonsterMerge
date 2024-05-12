using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

public class Star_CollectionController : MonoBehaviour
{
    [SerializeField] List<GameObject> _listStarDemo= new List<GameObject>();
    [SerializeField] GameObject _bgstarBarDemo;
    [SerializeField] GameObject _description;
    [SerializeField] Text _ability, _location, _lifeSpan;
    [SerializeField] GameObject _deleteButton;
    [SerializeField]
    GameObject[] _listStarVFXsBaseCollect= new GameObject[10];
    [SerializeField] Vector2 _oldPosOFbgStarBar;
    [SerializeField] AudioClip[] _liststarAudio = new AudioClip[10];
    [SerializeField] List<GameObject> _listStarInstance = new List<GameObject>();
    [SerializeField] GameObject _backButton, _starBar, _bgStarBar, _contentInScroll;
    [SerializeField] GameObject _starTemplate;
    [SerializeField]
    Sprite[] _listStarImage = new Sprite[10];
    [SerializeField] AudioClip _clickSound, _RaritySound;
    [SerializeField] Text _Rarity;
    private void Start()
    {
        _backButton.SetActive(false);
        _description.SetActive(false);
        _bgStarBar.SetActive(true);
        _bgStarBar.transform.GetComponent<Image>().enabled = false;
        _deleteButton.SetActive(false);
        _Rarity.text = "";
    
    }
    public void CLICKVIEWDETAIL(CollectionsDATA tmpDATA)
    {
        _oldPosOFbgStarBar = _bgStarBar.transform.position;
        _starBar.SetActive(true);
        _bgStarBar.SetActive(true);
        SoundEffect.instance.PlaySound(_clickSound);
        _contentInScroll.SetActive(false);
        StartCoroutine(Wait(tmpDATA));
    }
    IEnumerator Wait(CollectionsDATA tmpDATA)
    {
        _ability.text = " <color=white>Special Ability:</color>" + "\n" + tmpDATA.monster_ability;
        _location.text = " <color=white>Location:</color> " + "\n" + tmpDATA.monster_location;
        _lifeSpan.text = " <color=white>Lifespan:</color> " + "\n" + tmpDATA.monster_lifespan;
        for (int i = 0; i <= tmpDATA.monster_star - 1; i++)
        {
            GameObject a = ObjectPooling.instance.GetObject(_starTemplate);
            a.gameObject.SetActive(true);
            a.transform.SetParent(_starBar.transform);
            a.transform.localScale = Vector3.one * 1.5f;
            a.transform.GetChild(0).GetComponent<ParticleSystem>().startColor =
            _listStarVFXsBaseCollect[tmpDATA.monster_star - 1].GetComponentInChildren<ParticleSystem>().startColor;
            SoundEffect.instance.PlaySound(_liststarAudio[i]);
            _bgStarBar.transform.localScale = new Vector2(_bgStarBar.transform.localScale.x + 1.1f,
            _bgStarBar.transform.localScale.y);
            _bgStarBar.transform.GetComponent<Image>().pixelsPerUnitMultiplier = _bgStarBar.transform.localScale.x * 2;
            _bgStarBar.transform.GetComponent<Image>().enabled = true;
            _listStarInstance.Add(a);
            a.transform.DORotate(new Vector3(0, 0, -6.283185307f * 5 * (Mathf.Rad2Deg)), 0.15f, RotateMode.FastBeyond360);
            a.transform.DOScale(Vector2.one, 0.15f).OnComplete(() =>
            {
                a.transform.GetComponent<Image>().sprite = GameManager.instance._listImageStar[0];
            });
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(0.2f);
        _bgStarBar.transform.DOScale(new Vector2((tmpDATA.monster_star ==10) ? _bgStarBar.transform.localScale.x :
        _bgStarBar.transform.localScale.x + 0.7f, 2.4f), 0.2f);
        _bgStarBar.transform.GetComponent<Image>().pixelsPerUnitMultiplier = _bgStarBar.transform.localScale.x  ;
        _bgStarBar.transform.DOMoveY(_bgStarBar.transform.position.y , 0.2f);
        foreach (var t in _listStarInstance)
        {
            t.transform.GetComponent<Image>().sprite = GameManager.instance._listImageStar[tmpDATA.monster_star - 1];
        }
        ActiveStarVFX();
        
        SoundEffect.instance.PlaySound(_RaritySound);
        _Rarity.text = tmpDATA.monster_rarity;
        _Rarity.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        _description.SetActive(true);
        _description.transform.GetChild(1).GetComponent<Scrollbar>().value = 1;
        _backButton.SetActive(true);
        _deleteButton.SetActive(true);
    }
    public void ClearListSaveStar()
    {
        
        foreach (var t in _listStarInstance)
        {
            t.SetActive(false);
            t.transform.GetComponent<Image>().sprite= _starTemplate.transform.GetComponent<Image>().sprite;
            t.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    public void ActiveStarVFX()
    {
        foreach (GameObject t in _listStarInstance)
        {
            Transform h = t.transform.GetChild(0);
            h.gameObject.SetActive(true);
        }
    }
    public void back()
    {
        _description.SetActive(false);
        _deleteButton.SetActive(false);
        //_bgStarBar.transform.position = _oldPosOFbgStarBar;
        _bgStarBar.transform.localScale = new Vector2(0.3f, 1.2f);
        _bgStarBar.transform.GetComponent<Image>().enabled = false;
        _backButton.SetActive(false);
        ClearListSaveStar();
        _Rarity.gameObject.SetActive(false);
        _starBar.SetActive(false);
        
    }
    public void DemoStar(CollectionsDATA tmpDATA)
    {
        //Debug.LogError("success");
        _bgstarBarDemo.transform.GetComponent<Image>().enabled = true;
        _bgstarBarDemo.transform.localScale = new Vector2(1 + 0.85f * tmpDATA.monster_star, _bgstarBarDemo.transform.localScale.y);
        _bgstarBarDemo.transform.GetComponent<Image>().pixelsPerUnitMultiplier = _bgstarBarDemo.transform.localScale.x * 2;
        for (int i=0; i<tmpDATA.monster_star;i++)
        {
            _listStarDemo[i].SetActive(true);
            _listStarDemo[i].transform.GetComponent<Image>().sprite = GameManager.instance._listImageStar[tmpDATA.monster_star-1];
        }
        if(tmpDATA.monster_star < 10)
        {
            for(int i = tmpDATA.monster_star; i < 10; i++)
            {
                _listStarDemo[i].SetActive(false);
            }
        }
    }
    public void DisAbleStarDemo(CollectionsDATA tmpDATA)
    {
        _bgstarBarDemo.transform.GetComponent<Image>().enabled = false;
        for (int i = 0; i < tmpDATA.monster_star; i++)
        {
            _listStarDemo[i].SetActive(false);
            
        }
    }
}
