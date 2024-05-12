using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using static Unity.Burst.Intrinsics.X86.Avx;
using System.Linq;

public class JSONWriteRead : MonoBehaviour
{
    public void SaveToJson(ResultDATA resultMons)
    {
        string path= Application.persistentDataPath + "/CollectionsDATA.json";
        UserCollections collectionList = new UserCollections();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            collectionList = JsonUtility.FromJson<UserCollections>(json);
        }
    
        bool isMonsterInList = false;
        if(collectionList !=null && collectionList.monsters.Count!=0)
        {
            
            foreach (CollectionsDATA monster in collectionList.monsters)
            {
                if (monster.monster_id == resultMons.monster_id)
                {
                    isMonsterInList = true;
                    break;
                }
            }
        }
        
        if (!isMonsterInList)
        {
            CollectionsDATA data = new CollectionsDATA();
            data.monster_im = Resources.Load<Sprite>("Images/" + data.monster_id);
            data.monster_name = resultMons.monster_name;
            data.monster_width = resultMons.witdhofIM;
            data.monster_rarity = resultMons.monster_rarity;
            data.monster_height = resultMons.heightofIM;
            data.monster_star = resultMons.monster_star;
            data.monster_id = resultMons.monster_id;
            data.monster_lifespan = resultMons.monster_lifespan;
            data.monster_location = resultMons.monster_location;
            data.monster_ability = resultMons.monster_ability;
            collectionList.monsters.Add(data);
            try
            {
                string jsonToSave = JsonUtility.ToJson(collectionList, true);
                File.WriteAllText(path, jsonToSave);
                //Debug.Log("Complete");
            }
            catch (Exception e)
            {
                Debug.LogError("Loi: " + e.Message);
            }
            InsertionSort(collectionList, path);
        }
        //Debug.LogError(Application.persistentDataPath + "/CollectionsDATA.json");
    }
    public void InsertionSort(UserCollections collectionList, string path)
    {
     
        int n = collectionList.monsters.Count;
        for (int i = 1; i < n; ++i)
        {
            CollectionsDATA key = collectionList.monsters[i];
            int j = i - 1;
            while (j >= 0 && collectionList.monsters[j].monster_star < key.monster_star)
            {
                collectionList.monsters[j + 1] = collectionList.monsters[j];
                j = j - 1;
            }
            collectionList.monsters[j + 1] = key;
        }
        string sortedJson = JsonUtility.ToJson(collectionList, true);
        File.WriteAllText(path, sortedJson);

    }

}