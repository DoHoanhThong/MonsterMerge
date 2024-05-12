using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
using System.IO;
using Castle.Core.Internal;
using System.Security.Cryptography;

public class CollectionsManager : MonoBehaviour
{
    
    [SerializeField] GameObject _confirmDelete;
    [SerializeField] Canvas _canvas;
    [SerializeField] GameObject _BackHome;
    [SerializeField] GameObject _NotifyPanel, _fadeScreen;
    [SerializeField] Star_CollectionController _starCollection;
    [SerializeField] CollectionsDATA _tmpDATA;
    [SerializeField] Image _imageOfMons, _templateImage;
    [SerializeField] Dictionary<GameObject, CollectionsDATA> _collectionslist = new Dictionary<GameObject, CollectionsDATA>();
    [SerializeField] Text _monsterName;
    [SerializeField] GameObject _oldObject;
    public GameObject button;
    [SerializeField] AudioClip _clickSound;
    [SerializeField] GameObject _viewDetailButton;
    [SerializeField] GameObject _templatePrefab;
    [SerializeField] GameObject _contentInScroll;
    [SerializeField] GameObject _scroll;
    [SerializeField] Vector3 _beginScale;
    [SerializeField] GameObject _scrollBar, _handle;
    [SerializeField] GameObject _suggest;
    [SerializeField] GameObject _createMons;
    Vector3 _beginPosScroll;
    private void Start()
    {
        _beginPosScroll= _contentInScroll.transform.GetComponent<RectTransform>().anchoredPosition;
        _confirmDelete.SetActive(false);
        _createMons.SetActive(false);
        _suggest.SetActive(false);
        _scroll.SetActive(true);
        
        _BackHome.SetActive(true);
        _fadeScreen.SetActive(false);
        _NotifyPanel.SetActive(false);
        _beginScale = _imageOfMons.transform.localScale;
        _imageOfMons.sprite = _templateImage.sprite;
        _monsterName.text = "";
        _templatePrefab.SetActive(false);
        _viewDetailButton.SetActive(false);
        ShowCollection();
    }
    void ShowCollection()
    {
        _scroll.transform.GetComponent<ScrollRect>().enabled = false;
        _handle.GetComponent<Image>().enabled = false;
        _scrollBar.GetComponent<Image>().enabled = false;
        if (!File.Exists(Application.persistentDataPath + "/CollectionsDATA.json"))
        {
            _suggest.SetActive(true);
            _createMons.SetActive(true);
            return;
        }     
        string path = Application.persistentDataPath + "/CollectionsDATA.json"; // ???ng d?n t?i t?p JSON c?a b?n
        string jsonContent = File.ReadAllText(path);
        UserCollections userCollections = JsonUtility.FromJson<UserCollections>(jsonContent);
        if (userCollections.monsters.Count == 0 || userCollections.monsters.IsNullOrEmpty())
        {
            _suggest.SetActive(true);
            return;
        }
        StartCoroutine(Show(userCollections));
    }
    IEnumerator Show(UserCollections userCollections)
    {
        foreach (CollectionsDATA t in userCollections.monsters)
        {
            GameObject a = ObjectPooling.instance.GetObject(_templatePrefab);
            a.transform.GetComponent<Image>().sprite = Resources.Load<Sprite>("Collection/" + t.monster_star);
            a.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/" + t.monster_id);
            a.transform.SetParent(_contentInScroll.transform);
            a.SetActive(true);
            a.transform.localScale = Vector3.one * 0.6f;
            a.transform.DOScale(Vector3.one, 0.4f);
            a.transform.GetChild(0).localScale = new Vector3(0.8f, 0.8f, 1);
            if (!_collectionslist.ContainsKey(a.transform.GetChild(0).gameObject))
            {
                _collectionslist.Add(a.transform.GetChild(0).gameObject, t);
            }
            _contentInScroll.transform.GetComponent<RectTransform>().anchoredPosition = _beginPosScroll;
            yield return new WaitForSeconds(0.04f);
        }
        yield return new WaitForSeconds(0.1f);
        if (userCollections.monsters.Count > 9)
        {
            _scroll.transform.GetComponent<ScrollRect>().enabled = true;
            _handle.GetComponent<Image>().enabled = true;
            _scrollBar.GetComponent<Image>().enabled = true;
        }
       
    }
    public void VIEWDETAIL_ON()
    {
        SoundEffect.instance.PlaySound(_clickSound);
        _viewDetailButton.SetActive(true);

        Transform parTrans = button.transform.parent;
        parTrans.DOScale(CONSTANT.ScaleFirstZoomPar, 0.08f);
        parTrans.DOScale(CONSTANT.ScaleSecondZoomPar, 0.08f).SetDelay(0.08f);
        parTrans.DOScale(CONSTANT.ScaleLastScalePar, 0.1f).SetDelay(0.16f);
        _imageOfMons.sprite = button.transform.GetComponent<Image>().sprite;
        
        _imageOfMons.transform.localScale =  Vector3.one*0.65f;
        _imageOfMons.transform.DOScale(Vector2.one*0.95f, 0.1f);
        if (_oldObject == button)
        {
            return;
        }
        foreach (GameObject t in _collectionslist.Keys)
        {
            if (button == t)
            {
                _tmpDATA = _collectionslist[t];
                _monsterName.text = _collectionslist[t].monster_name;
            }
        }
        _starCollection.DemoStar(_tmpDATA);
        if (_oldObject != null)
        {
            _oldObject.transform.parent.DOScale(CONSTANT.ScaleBeginPar, 0.1f);
        }
        _oldObject = button; //Save old
    }
    public void CLICKVIEWDETAIL()
    {
        _oldObject = null;
        _starCollection.DisAbleStarDemo(_tmpDATA);
        foreach(GameObject t in _collectionslist.Keys)
        {
            t.transform.parent.gameObject.SetActive(false);
        }
        _scroll.SetActive(false);
        _collectionslist.Clear();
        _BackHome.SetActive(false);
        button.transform.parent.localScale = CONSTANT.ScaleBeginPar;
        _viewDetailButton.SetActive(false);
        _starCollection.CLICKVIEWDETAIL(_tmpDATA);
    }

    public void Back()
    {
        _scroll.SetActive(true);
        _imageOfMons.sprite = _templateImage.sprite;
        _monsterName.text = "";
        _imageOfMons.transform.localScale = _beginScale;
        foreach (GameObject t in _collectionslist.Keys)
        {
            t.transform.parent.gameObject.SetActive(true);
        }
        ShowCollection();
        SoundEffect.instance.PlaySound(_clickSound);
        _BackHome.SetActive(true);
        _contentInScroll.SetActive(true);
        _starCollection.back();
    }
    public void AskDelete()
    {
        SoundEffect.instance.PlaySound(_clickSound);
        _confirmDelete.SetActive(true);
        _confirmDelete.transform.localScale = Vector3.one * 0.6f;
        _confirmDelete.transform.DOScale(Vector3.one, 0.15f);
        _fadeScreen.SetActive(true);
    }
    public void AcceptDelete()
    {
        _fadeScreen.SetActive(false);
        _confirmDelete.transform.DOScale(Vector3.one * 0.6f, 0.1f).OnComplete(() =>
        {
            _fadeScreen.SetActive(true);
            _confirmDelete.SetActive(false);
            _NotifyPanel.SetActive(true);
            _NotifyPanel.transform.localScale = Vector3.one * 0.6f;
            _NotifyPanel.transform.DOScale(Vector3.one, 0.15f);
        });
        SoundEffect.instance.PlaySound(_clickSound);
        string path = Application.persistentDataPath + "/CollectionsDATA.json"; // ???ng d?n t?i t?p JSON c?a b?n
        string jsonContent = File.ReadAllText(path);
        UserCollections userCollections = JsonUtility.FromJson<UserCollections>(jsonContent);

        for (int i = userCollections.monsters.Count - 1; i >= 0; i--)
        {
            CollectionsDATA t = userCollections.monsters[i];
            if (_tmpDATA.monster_id == t.monster_id)
            {
                userCollections.monsters.RemoveAt(i);
                break;
            }
        }
        string updatedJsonContent = JsonUtility.ToJson(userCollections);
        File.WriteAllText(path, updatedJsonContent);
    
        
        //_fadeScreen.SetActive(true);
    }
    public void Cancel()
    {
        SoundEffect.instance.PlaySound(_clickSound);
        _confirmDelete.transform.DOScale(Vector3.one * 0.6f, 0.1f).OnComplete(() => {
            _confirmDelete.SetActive(false);
            _fadeScreen.SetActive(false);
        });
        
    }
    public void OK()
    {
        _NotifyPanel.transform.DOScale(Vector3.one * 0.6f, 0.15f).OnComplete(() =>
        {
            _fadeScreen.SetActive(false);
            _NotifyPanel.SetActive(false);
        });
        
        
        Back();
    }
    public enum Color
    {
        
    }
}
