using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using DG.Tweening;

public class ClickButtonInGame : MonoBehaviour
{
    public BaseDATA _objectData;
    [SerializeField] UIManagerInGame _uiMan;
    [SerializeField] AudioClip _clickSound;
    [SerializeField] float _timeClick, _countTime;
    private void Start()
    {
        _countTime = _timeClick;
        _objectData = this.transform.GetChild(0).GetComponent<InStanceObject>()._data;
    }
    private void Update()
    {
        _countTime-= Time.deltaTime;
    }
    public void Invoke()
    {
        if (_countTime >= 0)
            return;
        _countTime = _timeClick;
        _uiMan.button = this.gameObject;
        _uiMan.SpawnImage();
    }
    
}
