using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="BaseMonster", menuName ="BaseDATA")]
public class BaseDATA : ScriptableObject
{
    public Sprite monster_im;
    public float witdhofIM , heightofIM ; //cd, cr
    public string monster_name;
    public int monster_id;
    public AudioClip monster_sound;
    public bool isAds ;
}
