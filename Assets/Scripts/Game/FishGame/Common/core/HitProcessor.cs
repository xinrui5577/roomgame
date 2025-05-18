using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Fishs;
using Sfs2X.Entities.Data;
using UnityEngine;
using System.Collections.Generic;
using YxFramwork.Common;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    public class HitProcessor:MonoBehaviour  
    { 
        protected readonly Dictionary<int, CollideData> _readyDiefishDict = new Dictionary<int, CollideData>(); 
        //获得附加鱼死亡时附加的奖励倍数(只用于计算死亡几率,被参与奖励派发.如:一条100倍鱼,奖励倍数附加20,那么死亡几率计算的倍数为120,而奖励则为100倍)
        //public delegate int Func_GetFishOddAddtiveForDieRatio(Player killer, Bullet b, Fish f);
        //public static Func_GetFishOddAddtiveForDieRatio FuncGetFishAddtiveForDieRatio;
        /// <summary>
        /// 击中参数
        /// </summary>
        /// <param name="isMiss">ture:碰撞了也但是无效</param>
        /// <param name="p"></param>
        /// <param name="b"></param>
        /// <param name="f"></param>
        public delegate void Event_HitMiss(bool isMiss, Player p, Bullet b, Fish f);
        public static Event_HitMiss Evt_Hit;//击中,(当oddbonus为0时触发)

        public delegate void Event_HitResult(bool isKillFish, Player p, Bullet b, Fish f);
#if FUNCTION_KILLNUM //连续打鱼统计
    public static Event_HitResult Evt_HitResult;//
#endif
        public delegate void Event_HitExtension(Bullet b, Fish f);
        public static Event_HitExtension Evt_HitExtension;//额外的碰撞类型处理 

        public enum Operator
        {
            LastModule,
            Add
        }
        public class OperatorOddFix
        {
            public OperatorOddFix(Operator o, int v)
            {
                Op = o;
                Val = v;
            }
            public Operator Op = Operator.Add;
            public int Val = 0;
        }
        public delegate OperatorOddFix Func_GetOdd(Player killer, Bullet b, Fish f,Fish fCauser);

        //private static Dictionary<Func_GetFishOddAddtiveForDieRatio, Func_GetFishOddAddtiveForDieRatio> mFuncFishDieRatioAddtiveDict;//附加的死亡几率字典
        private static Dictionary<Func_GetOdd, Func_GetOdd> _mFuncOddDieRatioDict;//
        private static Dictionary<Func_GetOdd, Func_GetOdd> _mFuncOddBonusDict;//

        public static void AddFunc_Odd(Func_GetOdd funcOddDieRatio,Func_GetOdd funcOddBonus)
        {
            if (_mFuncOddDieRatioDict == null)
                _mFuncOddDieRatioDict = new Dictionary<Func_GetOdd, Func_GetOdd>();
            if (_mFuncOddBonusDict == null)
                _mFuncOddBonusDict = new Dictionary<Func_GetOdd, Func_GetOdd>();

            if(funcOddDieRatio != null)
                _mFuncOddDieRatioDict.Add(funcOddDieRatio, funcOddDieRatio);

            if (funcOddBonus != null)
                _mFuncOddBonusDict.Add(funcOddBonus, funcOddBonus);
        }

        public static void RemoveFunc_Odd(Func_GetOdd funcOddDieRatio, Func_GetOdd funcOddBonus)
        {
            if(funcOddDieRatio != null)
                _mFuncOddDieRatioDict.Remove(funcOddDieRatio);

            if (funcOddBonus != null)
                _mFuncOddBonusDict.Remove(funcOddBonus);
        }

        public virtual void SetHitPlayer(Player p)
        {
            Player = p;
        }

        virtual public void SetPlayerData()
        {

        }

        virtual public void BuyCoin(int value)
        {
        }

        virtual public void Retrieve()
        {
        }

        public virtual void OnServerData(ISFSObject data)
        {
        }
 
        /// <summary>
        /// 用于显示的赔率
        /// </summary>
        /// <param name="b">子弹</param>
        /// <param name="f">鱼</param>
        /// <param name="fCauser"></param>
        /// <returns></returns>
        public static int GetFishOddBonus(Bullet b, Fish f, Fish fCauser)
        {
            var outVal = f.Odds;
            var lastModuleLst = new List<OperatorOddFix>(); 
            foreach (var func in _mFuncOddBonusDict.Values)
            {
                var opOddFix = func(b.Owner, b, f, fCauser);
                if (opOddFix == null) continue;
                if (opOddFix.Op == Operator.Add) outVal += opOddFix.Val; 
                else lastModuleLst.Add(opOddFix); 
            } 
            foreach (var opOddFix in lastModuleLst)
            {
                outVal *= opOddFix.Val;
            } 
            return outVal;
        }

        public virtual void FireEffect(int useScore, bool isLock = false)
        {
            Player.GunInst.OnFire(useScore, isLock);
        }
         
        public virtual bool OneBulletKillFish(int bulletScore, FishOddsData fishFirst)
        {
            //鱼死亡    
            if (fishFirst.Odds <= 1)
            {
                Debug.LogWarning("第一条鱼的odd小于等于一的话就怪了,暂时不可能存在这样的鱼!!,(全屏或者类型炸弹同事出两个可能会出现这种情况)");
                return false;
            }
            return IsKill(fishFirst.Odds);
        }

        /// <summary>
        ///  一子弹获得分值
        /// </summary>
        /// <returns>获得分值</returns>
        public virtual List<FishOddsData> OneBulletKillFish(int bulletScore, FishOddsData fishFirst, List<FishOddsData> otherFish)
        {
            var fishDieList = new List<FishOddsData>();
            //鱼死亡    
            var oddsTotal = fishFirst.Odds;
            if (fishFirst.Odds <= 1)
            {
                YxDebug.LogWarning("第一条鱼的odd小于等于一的话就怪了,暂时不可能存在这样的鱼!!,(全屏或者类型炸弹同事出两个可能会出现这种情况)");
                return fishDieList;
            }
        
            //Debug.Log("oddsTotal = " + oddsTotal);
            if (IsKill(fishFirst.Odds))//第一条鱼命中
            {
                fishDieList.Add(fishFirst);
                //Debug.Log("firstDieRatio = " + firstDieRatio+"    odds ="+fishFirst.Odds);
                //求其他鱼死是否死亡 ,逐条计算几率
                foreach (FishOddsData f in otherFish)
                {
                    if (f.Odds <= 1)
                    {
                        Debug.LogWarning("otherFish的odd小于等于一的话就怪了,暂时不可能存在这样的鱼!!");
                        //return fishDieList;
                    }

                    oddsTotal += f.Odds;
                }
                foreach (FishOddsData f in otherFish)
                {
                    //Debug.Log("otherDieRatio = " + dieRatio + "    odds =" + f.Odds);
                    if (IsKill(oddsTotal))
                    {
                        fishDieList.Add(f);
                    }
                }
            }
            return fishDieList;
        }

        /// <summary>
        /// 弹出一个指定的数据,之后从数据集合中删除
        /// </summary>
        /// <param name="bulletId"></param>
        /// <returns></returns>
        public CollideData PopCollideData(int bulletId)
        {
            if (_readyDiefishDict.ContainsKey(bulletId))
            {
                var data = _readyDiefishDict[bulletId];
                _readyDiefishDict.Remove(bulletId); 
                return data;
            }
            return null;
        }

        virtual protected bool IsKill(int oddsTotal)
        {
            return Random.Range(0, 1100) < 1000 / oddsTotal;
        }

        protected Player Player;

        virtual public void ProcessHit(Bullet bullet, Fish fish)
        {
            if (!fish.Attackable) return;
            var totalOdd = fish.GetFishOddBonus(bullet); 
            var list = fish.BeHit(bullet, totalOdd);
            if (Evt_Hit != null) Evt_Hit(totalOdd == 0, bullet.Owner, bullet, fish);
            if (list != null && list.Count > 0)
            {
//                YxDebug.LogError("ProcessHit【{0}】", null, bullet.Score * fish.OddBonus * bullet.FishOddsMulti);
//                YxDebug.LogWrite("ProcessHit【{0}】", null, bullet.Score * fish.OddBonus * bullet.FishOddsMulti); 
                var bulletScore = bullet.Score;
                var rate = fish.OddBonus * bullet.FishOddsMulti;
                fish.DieSkillEffect(Player, bulletScore, rate, bullet.FishOddsMulti, bullet.IsLockingFish, 0);
                GameMain.Singleton.GameServer.SendUserWinAwardMsg(Player.Username, fish.FishName, rate * bulletScore, fish.OddBonus);
            } 
        } 

        /// <summary>
        /// 打鱼事件
        /// </summary>
        /// <param name="killer">击杀者</param>
        /// <param name="fish">被打中鱼</param>
        /// <param name="bulletScore">子弹分数</param>
        /// <param name="rate">总倍数</param>
        /// <param name="isLockFishBullet">是否为锁鱼子弹</param>
        public static void HitFishEvent(Player killer, Fish fish, int bulletScore, int rate, bool isLockFishBullet)
        {
            var scoreTotalGetted = rate * bulletScore;
            killer.GainScore(scoreTotalGetted);
            var gdata = App.GetGameData<FishGameData>();
            if (gdata.EvtPlayerGainScoreFromFish != null)
                gdata.EvtPlayerGainScoreFromFish(killer, scoreTotalGetted, fish, bulletScore);
            //触发杀死锁定鱼事件
            if (isLockFishBullet && gdata.EvtKillLockingFish != null)
                gdata.EvtKillLockingFish(killer);
        }  
    }

    public class CollideData
    {  
        /// <summary>
        /// 将要死的鱼
        /// </summary>
        public Fish DyingFish;
        /// <summary>
        /// 是否锁定子弹
        /// </summary>
        public bool IsLockFishBullet;
        /// <summary>
        /// 粒子炮倍数
        /// </summary>
        public int BulletOddsMulti;
        /// <summary>
        /// 子弹分数
        /// </summary>
        public int BulletScore;
        /// <summary>
        /// 杀死的个数
        /// </summary>
        public int KillCount;
        /// <summary>
        /// 鱼的唯一识别
        /// </summary>
        public uint FishId;
        /// <summary>
        /// 小鱼type
        /// </summary>
        public int FishType;
        /// <summary>
        /// 小鱼本身倍率
        /// </summary>
        public int FishOdds;
        /// <summary>
        /// 获得鱼的倍率
        /// </summary>
        public int TotalOdds;
    }
}
