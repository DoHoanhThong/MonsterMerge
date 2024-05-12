using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResultMonster", menuName = "ResultDATA")]
public class ResultDATA : ScriptableObject
{
    //public Sprite monster_im;
    public float witdhofIM, heightofIM; //cd, cr
    public int monster_star;
    public string monster_name;
    public string monster_rarity;
    public int monster_id;
    public string monster_ability;
    public string monster_location;
    public string monster_lifespan;
}
