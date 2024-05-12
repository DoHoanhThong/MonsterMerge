
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserCollections
{
    public List<CollectionsDATA> monsters = new List<CollectionsDATA>();
}
[System.Serializable]
public class CollectionsDATA
{
    public Sprite monster_im;
    public float monster_width, monster_height; //cd, cr
    public int monster_star;
    public string monster_name;
    public string monster_rarity;
    public int monster_id;
    public string monster_ability;
    public string monster_location;
    public string monster_lifespan;
}

