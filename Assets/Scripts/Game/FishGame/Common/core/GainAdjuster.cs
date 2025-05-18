using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Common.external.NemoFileIO;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;
using System.Collections;
using YxFramwork.Common;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    /// <summary>
    /// 抽放水调节
    /// </summary>
    /// <remarks>抽放水索引(0-20) 
    /// (0)     "保持现状",
    /// (1-10)  "放水一千","放水二千","放水三千","放水四千","放水五千","放水八千","放水一万","放水两万","放水五万","放水十万" 
    /// (11-20) "抽水一千","抽水二千","抽水三千","抽水四千","抽水五千","抽水八千","抽水一万","抽水两万","抽水五万","抽水十万" 
    /// </remarks>
    public class GainAdjuster : MonoBehaviour {
        public PersistentData<int, int> GainAdjust_GainScoreRT;//机台实时获得分
        public PersistentData<int, int> GainAdjust_LossScoreRT;//机台实时亏损分
        public int GiftCoinSmall = 15000;
        public int GiftCoinMedium = 20000;
        public int GiftCoinLarge = 25000;


        private BackStageSetting mBss;

        private int mScoreGain;
        private int mScoreLoss;
        private bool mIsLossOrProfit = true;//true:放水状态:false:放水状态
        private int mCurLossProfitScore = 0;//当前抽放水数目,单位:分

        private static float GainRatio_Loss = -0.01F;//放水时的抽水率
        private static float GainRatio_Profit = 0.01F;//抽水时的抽水率
        private static int[] LossProfitData = //抽放水索引表:单位:币
            {
                0
                ,1000,2000,3000,4000,5000,8000,10000,20000,50000,100000//放水值1-10
                ,1000,2000,3000,4000,5000,8000,10000,20000,50000,100000//抽水值11-20
                ,15000,20000,25000//两次打码放水,根据小中大场地放水设置21-23
                ,10//临时值24
            };

        void Awake()
        {
            mBss = GameMain.Singleton.BSSetting;
        }
        // Use this for initialization
        void Start () {
            LossProfitData[21] = GiftCoinSmall;
            LossProfitData[22] = GiftCoinMedium;
            LossProfitData[23] = GiftCoinLarge;


            var gdata = App.GetGameData<FishGameData>();
            gdata.EvtBgClearAllDataBefore += Handle_BackGroundClearAllDataBefore;//在后台清所有数据0的时候.重置用于计算抽放水的盈亏币数
            //GameMain.EvtBGChangeArenaType += Handle_BackGroundChangeArenaType;
            if (mBss.Dat_GainAdjustIdx.Val == 0)//没有抽放水设置
            {
                return;
            }else if (mBss.Dat_GainAdjustIdx.Val >= 1 && mBss.Dat_GainAdjustIdx.Val <= 10)//放水
            {
                mIsLossOrProfit = true;
            }

            else if (mBss.Dat_GainAdjustIdx.Val >= 11 && mBss.Dat_GainAdjustIdx.Val <= 20)//抽水
            {
                mIsLossOrProfit = false;
            }
            else if (mBss.Dat_GainAdjustIdx.Val >= 21 && mBss.Dat_GainAdjustIdx.Val <= 24)//两次打码放水(21-23) + 放水临时值(24)
            {
                mIsLossOrProfit = true;
            }
            else
            {
                YxDebug.LogError("Dat_GainAdjustIdx 在有效值范围以外.");
                return;
            }


            gdata.EvtPlayerGunFired += Handle_PlayerFire;
            gdata.EvtPlayerGainScoreFromFish += Handle_PlayerGainScoreFromFish;
        

            if (GainAdjust_GainScoreRT == null) GainAdjust_GainScoreRT = new PersistentData<int, int>("GainAdjust_GainScoreRT");
            if (GainAdjust_LossScoreRT == null) GainAdjust_LossScoreRT = new PersistentData<int, int>("GainAdjust_LossScoreRT"); 
            mScoreGain = GainAdjust_GainScoreRT.Val;
            mScoreLoss = GainAdjust_LossScoreRT.Val;

            mCurLossProfitScore = LossProfitData[mBss.Dat_GainAdjustIdx.Val] * mBss.InsertCoinScoreRatio.Val;

            //YxDebug.Log("mCurLossProfitScore = " + mCurLossProfitScore);
            StartCoroutine("_Coro_SaveGainData");
            StartCoroutine("_Coro_LossOrProfitprocess");
        }

        /// <summary>
        /// 设置放水值
        /// </summary>
        /// <remarks>小于等于0的值忽略</remarks>
        /// <param name="small"></param>
        /// <param name="medium"></param>
        /// <param name="large"></param>
        public void SetGiftCoin(int small, int medium, int large)
        {
        
            if (small >= 0 )
                LossProfitData[21] = GiftCoinSmall = small;
            if( medium >= 0)
                LossProfitData[22] = GiftCoinMedium = medium;
            if(large >= 0 )
                LossProfitData[23] = GiftCoinLarge = large;

            mCurLossProfitScore = LossProfitData[mBss.Dat_GainAdjustIdx.Val] * mBss.InsertCoinScoreRatio.Val;
        }
        /// <summary>
        /// 外部调用自定义放水值
        /// </summary>
        /// <param name="coin">币值</param>
        public void FreeGainCustom(int coin)
        {
            if(mBss == null)
                mBss = GameMain.Singleton.BSSetting;

            if (mBss.Dat_GainAdjustIdx.Val != 0)
            {
                YxDebug.LogWarning("在抽放水状态下调用自定义抽放水,FreeGainCustom()调用无效");
                return;
            }

            mBss.Dat_GainAdjustIdx.Val = 24;
            LossProfitData[24] = coin;

            var gdata = App.GetGameData<FishGameData>();
            gdata.EvtPlayerGunFired += Handle_PlayerFire;
            gdata.EvtPlayerGainScoreFromFish += Handle_PlayerGainScoreFromFish;


            if (GainAdjust_GainScoreRT == null) GainAdjust_GainScoreRT = new PersistentData<int, int>("GainAdjust_GainScoreRT");
            if (GainAdjust_LossScoreRT == null) GainAdjust_LossScoreRT = new PersistentData<int, int>("GainAdjust_LossScoreRT");
            mScoreGain = GainAdjust_GainScoreRT.Val = 0;
            mScoreLoss = GainAdjust_LossScoreRT.Val = 0;

            mCurLossProfitScore = LossProfitData[mBss.Dat_GainAdjustIdx.Val] * mBss.InsertCoinScoreRatio.Val;

            StopCoroutine("_Coro_SaveGainData");
            StartCoroutine("_Coro_SaveGainData");

            StopCoroutine("_Coro_LossOrProfitprocess");
            StartCoroutine("_Coro_LossOrProfitprocess");

        }
        void Handle_PlayerFire(Player owner, Gun gun, int useScore, bool isLock, int bulletId)
        {
            //YxDebug.Log((mIsLossOrProfit ? "放水中" : "抽水中") + "  GainAdjustIdx =" + mBss.Dat_GainAdjustIdx.Val +"    gainOdd = "+GameOdds.GainRatio);
            mScoreGain += useScore;
        }

        void Handle_PlayerGainScoreFromFish(Player p, int scoreGetted, Fish firstFish, int bulletScore)
        {
            mScoreLoss += scoreGetted;
        }

        IEnumerator _Coro_LossOrProfitprocess()
        {
            while (true)
            {
                yield return new WaitForSeconds(1F);
                GameOdds.GainRatio = mIsLossOrProfit ? GainRatio_Loss : GainRatio_Profit;

                if (mIsLossOrProfit)// 放水
                {
                    if ((mScoreLoss - mScoreGain) > mCurLossProfitScore)//(机台亏损 - 机台盈利) 达到放水额
                    {
                        StopCoroutine("_Coro_SaveGainData");
                        ReleaseAllHook();
                        mBss.Dat_GainAdjustIdx.Val = 0;//还原抽放水标记
                        GameOdds.GainRatio = GameOdds.GainRatios[(int)(mBss.GameDifficult_.Val)];//还原赔率
                        GainAdjust_GainScoreRT.Val = 0;
                        GainAdjust_LossScoreRT.Val = 0;
                        yield break;
                        //YxDebug.Log("放水完毕 = " + (mScoreLoss - mScoreGain));
                        //Destroy(gameObject);
                    }
                }
                else// 抽水
                {
                    if ((mScoreGain - mScoreLoss) > mCurLossProfitScore)//(机台盈利 - 机台亏损) 达到抽水额
                    {
                        StopCoroutine("_Coro_SaveGainData");
                        ReleaseAllHook();
                        mBss.Dat_GainAdjustIdx.Val = 0;
                        GameOdds.GainRatio = GameOdds.GainRatios[(int)(mBss.GameDifficult_.Val)];//还原赔率
                        GainAdjust_GainScoreRT.Val = 0;
                        GainAdjust_LossScoreRT.Val = 0;
                        yield break;
                        //Destroy(gameObject);
                    }
                }
            }

        }
  

        /// <summary>
        /// 响应后台清空所有数据(两次打码),需使得记录数据清0
        /// </summary>
        void Handle_BackGroundClearAllDataBefore()
        {
            if (GainAdjust_GainScoreRT == null) GainAdjust_GainScoreRT = new PersistentData<int, int>("GainAdjust_GainScoreRT");
            if (GainAdjust_LossScoreRT == null) GainAdjust_LossScoreRT = new PersistentData<int, int>("GainAdjust_LossScoreRT"); 

            //_Coro_SaveGainData可能一直在作用
            StopCoroutine("_Coro_SaveGainData");
            StopCoroutine("_Coro_LossOrProfitprocess");
            mScoreGain = 0;
            mScoreLoss = 0;
            GainAdjust_GainScoreRT.SetImmdiately(0);
            GainAdjust_LossScoreRT.SetImmdiately(0);

            mBss.Dat_GainAdjustIdx.SetImmdiately(GameOdds.CoinPresents[(int)mBss.ArenaType_.Val]);
            //YxDebug.Log("set gain loss score zero");
        }

        void ReleaseAllHook()
        {
            var gdata = App.GetGameData<FishGameData>();
            gdata.EvtPlayerGunFired -= Handle_PlayerFire;
            gdata.EvtPlayerGainScoreFromFish -= Handle_PlayerGainScoreFromFish;
        }
        IEnumerator _Coro_SaveGainData()
        {
            while (true)
            {
                yield return new WaitForSeconds(5F);
                GainAdjust_GainScoreRT.Val = mScoreGain;
                GainAdjust_LossScoreRT.Val = mScoreLoss;
                //Debug.Log("_Coro_SaveGainData gainRatio = " + GameOdds.GainRatio + "   GainAdjust_GainScoreRT = " + GainAdjust_GainScoreRT.Val + "  GainAdjust_LossScoreRT = " + GainAdjust_LossScoreRT.Val);
            } 
        }

        //void OnApplicationQuit()
        //{
        //    mBss = null;
        //}
    }
}
