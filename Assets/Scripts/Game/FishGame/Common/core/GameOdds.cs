using UnityEngine;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    public class GameOdds  {
    
        public static float GainRatio 
        {
            get { return mGainRatio; }
            set { mGainRatio = value > 0 ? value * DifficultFactor : value; }//抽水率大于0时难度因子才奇效
        }
   


        public static float DifficultFactor = 1F;//难度系数(在抽水率大于0时有效)
        public static float[] GainRatios = new float[] { 0.0005F, 0.001F, 0.002F, 0.02F, 0.04F };//不同难度下得赔率
        public static float[] GainRatioConditFactors = new float[] { 1F, 0.5F, 0.2F };//= new float[] { 1F, 0.5F, 0.01F };//不同场地类型(ArenaType)下的条件收益的因子.(即:大于mReducegainOddLine时 :GainRatio *= GainRatiosReduceFactor[ArenaType];)
        public static int[] CoinPresents = new int[] { 21, 22, 23 };//两次打码后根据场地类型不同,赠送的币数.索引与场地类型(Arenatype)相关,值是抽水索引Dat_GainAdjustIdx
        public static float GainRatioConditFactor = 1F;//当前条件抽水因子

        //public static float RatioGet_Lizi = 0.0033F;//离子炮获得几率
        private static float mGainRatio = 0.0005F;//盈利率(-1F~1F)
        private static int mReduceGainOddLine = 100;//大于该倍数的鱼,按照场地类型来削减mGainRatio.(以提高高倍数鱼死亡几率,使得分数波动幅度增大)

        private static FishOddsData _fd = new FishOddsData(0,0);
        /// <summary>
        /// 随机值是否命中()
        /// </summary>
        /// <param name="r">0F~1F</param>
        /// <returns></returns>
        public static bool IsHitInOne(float r)
        {
            //return true;
#if UNITY_EDITOR
            if(GameMain.Singleton.OneKillDie)
            {
                return true;
            }
#endif
            return Random.Range(0F, 1F) < r;
            //return Random.Range(0, 10000000) < (int)( r * 10000000);
        }

        public static bool IsHitInOne2(float r)
        {
            return Random.Range(0F, 1F) < r;
            //return Random.Range(0, 10000000) < (int)( r * 10000000);
        }
        /// <summary>
        ///  一子弹获得分值
        /// </summary>
        /// <returns>获得分值</returns>
        public static List<FishOddsData> OneBulletKillFish(int bulletScore, FishOddsData fishFirst, List<FishOddsData> otherFish)
        {
            List<FishOddsData> fishDieList = new List<FishOddsData>();
            //鱼死亡    
            int oddsTotal = fishFirst.Odds;
            if (fishFirst.Odds <= 1)
            {
                YxDebug.LogWarning("第一条鱼的odd小于等于一的话就怪了,暂时不可能存在这样的鱼!!,(全屏或者类型炸弹同事出两个可能会出现这种情况)");
                return fishDieList ;
            }
            foreach (FishOddsData f in otherFish)
            {
                if (f.Odds <= 1)
                {
                    YxDebug.LogWarning("otherFish的odd小于等于一的话就怪了,暂时不可能存在这样的鱼!!");
                    //return fishDieList;
                }

                oddsTotal += f.Odds;
            }
            //YxDebug.Log("oddsTotal = " + oddsTotal);

            float gainRatio = GainRatio;
            if (gainRatio > 0F && oddsTotal >= mReduceGainOddLine)
            {
                gainRatio *= GainRatioConditFactor;
            }

            //第一条死亡几率 
            float firstDieRatio = (1F - gainRatio) * (fishFirst.Odds + oddsTotal) / (2F * oddsTotal * fishFirst.Odds);
            //YxDebug.Log("2F * oddsTotal * fishFirst.Odds = " + firstDieRatio);
            //YxDebug.Log("firstDieRatio = " + firstDieRatio + "    odds =" + fishFirst.Odds);
            if (IsHitInOne(firstDieRatio))//第一条鱼命中
            {
                fishDieList.Add(fishFirst);
                //Debug.Log("firstDieRatio = " + firstDieRatio+"    odds ="+fishFirst.Odds);
                //求其他鱼死是否死亡 ,逐条计算几率
                foreach (FishOddsData f in otherFish)
                {
                    float dieRatio = (1F - gainRatio - firstDieRatio * fishFirst.Odds) / (firstDieRatio * (oddsTotal - fishFirst.Odds));
                    //Debug.Log("otherDieRatio = " + dieRatio + "    odds =" + f.Odds);
                    if (IsHitInOne(dieRatio))
                        fishDieList.Add(f);
                }
            }

            return fishDieList;
        }

        public static FishOddsData OneBulletKillFish(int bulletScore, FishOddsData fishFirst)
        {
        
            //鱼死亡    
            int oddsTotal = fishFirst.Odds;
            if (fishFirst.Odds <= 1)
            {
                Debug.LogWarning("第一条鱼的odd小于等于一的话就怪了,暂时不可能存在这样的鱼!!,(全屏或者类型炸弹同事出两个可能会出现这种情况)");
                return _fd;
            }
       

            float gainRatio = GainRatio;
            if (gainRatio > 0F && oddsTotal >= mReduceGainOddLine)
            {
                gainRatio *= GainRatioConditFactor;
            }

            //第一条死亡几率 
            float firstDieRatio = (1F - gainRatio) * (fishFirst.Odds + oddsTotal) / (2F * oddsTotal * fishFirst.Odds);
            //Debug.Log("2F * oddsTotal * fishFirst.Odds = " + firstDieRatio);
            //Debug.Log("firstDieRatio = " + firstDieRatio + "    odds =" + fishFirst.Odds);
            if (IsHitInOne(firstDieRatio))//第一条鱼命中
            {
                return fishFirst;
            }
            return _fd;

        }
    }
}
