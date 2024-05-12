using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SetDataForObject : MonoBehaviour
{
    [SerializeField] ScrollRect _scrollRect;
    [SerializeField] Image _scrollBar;
    [SerializeField] GameObject _handle;
    [SerializeField] Canvas _canvas;
    public BaseDATA[] _dataOfObject;
    [SerializeField] InStanceObject _view;
    [SerializeField] List<InStanceObject> _monsterView = new List<InStanceObject>();
    [SerializeField] UIManagerInGame _ui;
    private void Start()
    {
        if (_scrollRect != null)
        {
            _scrollRect.enabled = false;
        }
       if (_handle != null && _scrollBar != null)
        {
            _handle.SetActive(false);
            _scrollBar.enabled = false;
        }
        StartCoroutine(Create());
        
    }
    IEnumerator Create()
    {
        if (_scrollBar != null)
        {
            _scrollBar.transform.GetComponent<Scrollbar>().interactable = false;
        }
        
        int i = 0;
        foreach(var data in _dataOfObject)
        {
            InStanceObject tmp = ObjectPooling.instance.GetObject(_view.gameObject).transform.GetComponent<InStanceObject>();
            tmp.Initialize(data);
            tmp.gameObject.SetActive(true);
            _monsterView.Add(_view);
            tmp.transform.SetParent(_ui._listImage[i].transform.GetChild(0));
            i++;
            yield return new WaitForSeconds(0.1f);
        }
        while (i < _dataOfObject.Length)
        {
            continue;
        }
        foreach(var t in _ui._listImage)
        {
            t.transform.GetChild(0).GetComponent<Button>().interactable = true;
        }
        if (_scrollBar!=null && _handle != null)
        {
            
            _handle.SetActive(true);
            _scrollBar.enabled = true;

            _scrollBar.transform.GetComponent<CanvasGroup>().alpha = 0;
            _scrollBar.transform.GetComponent<CanvasGroup>().DOFade(1, 0.2f).OnComplete(() =>
            {
                _scrollBar.transform.GetComponent<Scrollbar>().interactable = true;
            });
            if (_scrollRect != null)
            {
                _scrollRect.enabled = true;
            }
        }
        
    }
    
}
