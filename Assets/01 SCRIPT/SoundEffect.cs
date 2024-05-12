using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    private static SoundEffect _instance;
    public static SoundEffect instance => _instance;
    public AudioSource _soundsource;
    private void Awake()
    {
        if(_instance == null)
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
    }
    public void PlaySound(AudioClip sound)
    {
        _soundsource.PlayOneShot(sound);//phat am thanh 1 lan
    }
}
