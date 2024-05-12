using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasFitScreen : MonoBehaviour
{
    GameObject _canvas;
    // Start is called before the first frame update
    private void Awake()
    {
        _canvas = this.gameObject;
    }
    void Start()
    {
        float aspect = (float)Screen.width / Screen.height;

        if (aspect > 0.5625f)
        {
            _canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;

        }
        else if (aspect == 0.5625f)
        {
            _canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.5f;

        }
        else
        {
            _canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0;

        }
    }

   
}
