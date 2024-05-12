using System;

namespace TeraJet
{
    [Serializable]
    public class GameAdData
    {
        public string ad_type;
        public string ad_game_id;
        public string ad_fullscreen_id;
        public string ad_reward_id;
        public int version_code;

        public override string ToString()
        {
            return UnityEngine.JsonUtility.ToJson(this, true);
        }
    }
}

