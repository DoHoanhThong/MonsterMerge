using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    [SerializeField] Text _textBegin;
    private void Awake()
    {
        _textBegin = this.GetComponent<Text>();
    }
    private void Update()
    {
        
    }
}
