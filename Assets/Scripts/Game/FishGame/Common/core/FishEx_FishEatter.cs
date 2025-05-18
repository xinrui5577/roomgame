using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    public class FishEx_FishEatter : MonoBehaviour
    {
        public delegate void Evt_BeforeEatFish(Fish eatter,Fish beEattedFish);
        public Evt_BeforeEatFish EvtBeforeEatFish;

        public Fish[] FishCanEat;
        public tk2dTextMesh Text_Odds;

        public Fish _Fish
        {
            get
            {
                if(mFish == null)
                    mFish = transform.parent.GetComponent<Fish>();

                return mFish;
            }
        }
        //private bool mEattable = true;//当前状态,用于避免OnTriggerEnter在死亡之后仍然触发
        private Fish mFish;
        private readonly static int OddsAccumulMin = 40;//累计分数初始值
        private readonly static int OddsAccumulMax = 300;//累计分数最大值

        private static Dictionary<int, Fish> mFishCanEatDict;
        private static int mOddsAccumul = OddsAccumulMin;//之前累计的分数
    
        void Awake()
        {
            if (mFishCanEatDict == null)
            {
                mFishCanEatDict = new Dictionary<int, Fish>();
                foreach (Fish f in FishCanEat)
                {
                    mFishCanEatDict.Add(f.TypeIndex, null);
                }
            }

            mFish = transform.parent.GetComponent<Fish>();
            mFish.Odds = mOddsAccumul;
            mFish.EvtFishKilled += Handle_FishKilled;
        }
        void Start()
        {
            Text_Odds.text = mOddsAccumul.ToString();
            Text_Odds.Commit();
        }

        void Handle_FishKilled(Player killer, int bulletScore, int fishOddBonus, int bulletOddsMulti, Fish fish, int reward)
        {
            mOddsAccumul = OddsAccumulMin; 
        }

        void OnTriggerEnter(Collider other)
        {
 
            Fish fishCollide = other.GetComponent<Fish>();
            //不包含鱼组件,排除
            if (fishCollide == null)
                return;

            //目标鱼已死亡,排除
            if (!fishCollide.Attackable)
                return;

            //不在可吃鱼列表中,排除
            if (!mFishCanEatDict.ContainsKey(fishCollide.TypeIndex))
                return;

            //累计分大于最大限制,排除
            if (mOddsAccumul >= OddsAccumulMax)
                return;

            //本体已死亡
            if (mFish == null || !mFish.Attackable)
                return;

            if (EvtBeforeEatFish != null)
                EvtBeforeEatFish(mFish,fishCollide);

            //加分
            mOddsAccumul += fishCollide.Odds;

            mFish.Odds = mOddsAccumul;

            //清除目标鱼
            fishCollide.Clear();

            //更新分数牌 
            Text_Odds.text = mOddsAccumul.ToString();
            Text_Odds.Commit();


            //YxDebug.Log("Fish =" + fishCollide.name + "   Odds = " + fishCollide.Odds + " totalOdds = " + mOddsAccumul);
        }
    }
}
