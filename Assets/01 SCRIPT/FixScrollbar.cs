using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FixScrollbar : MonoBehaviour
{
    public Scrollbar _scrollbar;
    private float previousValue = -1f;
    void Awake()
    {
        _scrollbar = this.GetComponent<Scrollbar>();
    }
    private void FixedUpdate()
    {
        _scrollbar.size = 0.23f;

    }
    private void Update()
    {
        _scrollbar.size = 0.23f;
    }
    private void LateUpdate()
    {
        _scrollbar.size = 0.23f;
    }
    public void OnvalueChangeScroll()
    {
        _scrollbar.size = 0.23f;
    }
}
