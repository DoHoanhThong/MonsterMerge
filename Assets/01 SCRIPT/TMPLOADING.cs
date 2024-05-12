using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TMPLOADING : MonoBehaviour
{
    [SerializeField] List<GameObject> _listImage= new List<GameObject>();
    [SerializeField] Image _image;
    [SerializeField] float time;

    // Update is called once per frame
    void Update()
    {
        _image.fillAmount += time/10*Time.deltaTime;
        if (_image.fillAmount >= 1)
        {
            SceneManager.LoadSceneAsync(1);
        }
    }
    void Zoom()
    {
        foreach(GameObject t in _listImage)
        {
            t.transform.DOScale(new Vector3(1.2f, 1.2f) ,0.1f);
        }
    }
}
