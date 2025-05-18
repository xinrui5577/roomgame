using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.Public.Adpater
{
    public enum AdpaterType
    {
        Normal, //1280*720,16:9
        ipad,   //4:3
    }

    public class GameAdpaterManager : MonoBehaviour
    {
        public static GameAdpaterManager Singleton;
        public AdpaterType Adpater = AdpaterType.Normal;

        public AdpaterConfigAssets AssetsConfigs;

        void Awake()
        {
            Singleton = this;
            Adpater = SetAdpaterType();
        }

        public AdpaterType SetAdpaterType()
        {
            float big = Screen.width;
            float small = Screen.height;
            float rate = big > small ? big / small : small / big;

            if (rate < 1.4f)
            {
                return AdpaterType.ipad;
            }
            else
            {
                return AdpaterType.Normal;
            }
        }

        public AdpaterConfig GetConfig
        {
            get
            {
                if (AssetsConfigs != null)
                {
                    return System.Array.Find<AdpaterConfig>(AssetsConfigs.Configs, (a) => { return a.Type == Adpater; });
                }
                else
                {
                    //一个初始数据
                    return new AdpaterConfig();
                }                   
            }           
        }
    }
}