using UnityEngine;

namespace Assets.Scripts.Game.FishGame.script
{
    public class Moudle_main : MonoBehaviour
    {
        //0为开始界面,1:游戏主界面
        public static int iState;
        public delegate void Event_Generic();

        public static Event_Generic EvtBackStart;           //返回主界面
        public static Event_Generic EvtGameStart;           //游戏开始
        public static Event_Generic EvtSceneSelect;         //选择场景
        public static Event_Generic EvtHelp;                //帮助
        public static Event_Generic EvtRank;                //排行榜
        public static Event_Generic EvtWikipedia;           //百科
        public static Event_Generic EvtJiaocheng;           //新手教程
        public static Event_Generic EvtSetting;             //设置
        public static Event_Generic EvtAchievement;         //成就
        public static Event_Generic EvtRecharge;            //充值
        public static Event_Generic EvtShop;                //商店
        public static Event_Generic EvtRechState;           //充值方式
        public static Event_Generic EvtLevelUP;             //升级
        public static Event_Generic EvtEveryDayReward;      //每日奖励 
    

        public static Event_Generic EvtChangeName;          //改名

        public static Moudle_main Singlton
        {
            get
            {
                if (mSingleton == null)
                    mSingleton = GameObject.FindObjectOfType(typeof(Moudle_main)) as Moudle_main;
                return mSingleton;
            }
        }

        private static Moudle_main mSingleton;
        public  GameObject Prefab_Black;
        public  GameObject go_Black;

        void Awake()
        {
            Clear();
        }

        // Use this for initialization
        void Start ()
        {
            iState = 0;
            go_Black = Instantiate(Prefab_Black) as GameObject;
            go_Black.SetActive(false); 
        }
	
        // Update is called once per frame
        void Update ()
        {
            //wiipay.
        }

        private void Clear()
        {
            EvtBackStart = null; //返回主界面
            EvtGameStart = null; //游戏开始
            EvtSceneSelect = null; //选择场景
            EvtHelp = null; //帮助
            EvtRank = null; //排行榜
            EvtWikipedia = null; //百科
            EvtJiaocheng = null; //新手教程
            EvtSetting = null; //设置
            EvtAchievement = null; //成就
            EvtRecharge = null; //充值
            EvtShop = null; //商店
            EvtRechState = null; //充值方式
            EvtLevelUP = null; //升级
            EvtEveryDayReward = null; //每日奖励 
        }
    }
}