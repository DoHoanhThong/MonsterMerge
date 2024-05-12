using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InStanceObject : MonoBehaviour
{
    public List<GameObject> _listADsButton= new List<GameObject>();
    public BaseDATA _data;
    public float beginPosY;
    [SerializeField] GameObject _baseObject;
    [SerializeField] GameObject _contentInScroll;
    [SerializeField] UIManagerInGame _ui;
    private void Awake()
    {
        
        _baseObject.SetActive(false);
         beginPosY = _contentInScroll.transform.GetComponent<RectTransform>().anchoredPosition.y;
    }
    public void Initialize(BaseDATA data)
    {
        Vector3 a = _contentInScroll.transform.GetComponent<RectTransform>().anchoredPosition;
        a.y = beginPosY;
        _contentInScroll.transform.GetComponent<RectTransform>().anchoredPosition = a;

        GameObject instanceObject = ObjectPooling.instance.GetObject(_baseObject);

        instanceObject.transform.SetParent(_contentInScroll.transform);
        instanceObject.transform.localScale = Vector3.one * 0.5f;
        instanceObject.transform.DOScale(Vector3.one, 0.4f);
        //instanceObject.transform.DOScale(Vector3.one, 0.07f).SetDelay(0.05f);
        instanceObject.SetActive(true);
        _ui._listImage.Add(instanceObject);
        instanceObject.transform.GetChild(0).GetComponent<Image>().sprite = data.monster_im;
        instanceObject.transform.GetChild(0).GetComponent<Button>().interactable = false;
        instanceObject.transform.GetChild(0).localScale = new Vector3(data.witdhofIM * 8 / 9000, data.heightofIM *8/ 9000 , 1);
        _data = data;
        if (SceneManager.GetActiveScene().buildIndex != 2 && SceneManager.GetActiveScene().buildIndex != 4) 
            return;
        if(PlayerPrefs.HasKey("adsMons" + data.monster_id) && PlayerPrefs.GetString("adsMons" + data.monster_id)=="true")
        {
            instanceObject.transform.GetChild(1).gameObject.SetActive(true);
            return;
        }
        instanceObject.transform.GetChild(1).gameObject.SetActive(false);
    }
    
}
