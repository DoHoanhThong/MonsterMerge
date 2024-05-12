using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMusic : MonoBehaviour
{
    private static BGMusic _instance;
    public static BGMusic instance => _instance;
    public AudioSource _soundsource;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _soundsource = this.GetComponent<AudioSource>();
            DontDestroyOnLoad(this.gameObject);
            return;
        }
        if (_instance.gameObject.GetInstanceID() != this.gameObject.GetInstanceID())
        {
            Destroy(this.gameObject);
        }
        if (!PlayerPrefs.HasKey("BGMVolume"))
        {
            this.transform.GetComponent<AudioSource>().volume = 0.4f;
            this.transform.GetComponent<AudioSource>().enabled = true;
            PlayerPrefs.SetFloat("BGMVolume", 0.4f);
            PlayerPrefs.SetInt(CONSTANT.Music, 1);
        }
        
        if (PlayerPrefs.GetInt(CONSTANT.Music) == 0)
        {
            this.transform.GetComponent<AudioSource>().Stop();
        }
    }
}
