using System.Collections.Generic;
using Assets.Scripts.Game.FishGame.Common.Servers;
using Assets.Scripts.Game.FishGame.Fishs;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.View;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    class HitProcessorPlayer : HitProcessor
    {

        private int _start;
        private int _current;
        private int _totalBullet;
        private int _target;
        private int _clear;

       

        //手中金币
        private int _coin;
     
        override public void SetPlayerData()
        {  
            //OnServerData(Player.GameData);
            _current = Player.GameData.GetInt("winCoin");
        }

        public override void SetHitPlayer(Player p)
        {
            base.SetHitPlayer(p);
            var data = p.GameData;
            _totalBullet = data != null && data.ContainsKey("bulletNum") ? data.GetInt("bulletNum") : 0;
        }

        override public void OnServerData(ISFSObject data)
        { 
            var coin = data.GetInt("coin"); 
            var bId = data.GetInt("bid");
            var rate = data.GetInt("rate");
            //var fishId = data.GetInt("fid");　
            var blt = data.GetInt("blt"); 
            var collideData = PopCollideData(bId);
            var dyingFish = collideData.DyingFish;
            var selfSeat = App.GameData.SelfSeat % 6;
            var curCoin = GameMain.Singleton.BSSetting.Dat_PlayersScore[selfSeat].Val;
            var rewardCoin = coin - curCoin;
            var scoreTotalGetted = rate * blt;
            var fishId = collideData.FishId;
            var fishType = collideData.FishType;
            var fishOdds = collideData.FishOdds;
            if (dyingFish.TypeIndex != fishType)
            {
                YxDebug.LogWrite("子弹id：{0} | 小鱼倍数：{1}【{6}】 | 子弹倍数：{2} | 小鱼type：{3}【{7}】 | 获得：{4} | fishId:{5}【{8}】",
                                 null,
                                 bId, rate, blt, fishType, scoreTotalGetted, fishId,
                                 dyingFish.Odds, dyingFish.TypeIndex, dyingFish.ID);
            }
            if (rewardCoin != scoreTotalGetted)
            {
                YxDebug.LogError("应获取：{0}【实际：{1}】（鱼:{2}|{3}）：<color=#ffff00>{2,15}</color>", "HitProcessorPlayer", null, rewardCoin,
                                 scoreTotalGetted, fishType, fishOdds);
            }
//            YxDebug.LogWrite("获得{0}【{1}】：{2,15}", null, coin, curCoin, rewardCoin);
            if (dyingFish != null && dyingFish.ID == fishId)
                dyingFish.DieSkillEffect(Player, blt, rate, collideData.BulletOddsMulti, collideData.IsLockFishBullet, scoreTotalGetted);
            else//鱼已经为空,直接加钱
            {
                Player.GainScore(scoreTotalGetted); 
            }
            GameMain.Singleton.GameServer.SendUserWinAwardMsg(Player.Username, dyingFish.FishName, scoreTotalGetted,rate);
            curCoin = GameMain.Singleton.BSSetting.Dat_PlayersScore[selfSeat].Val;
            if (coin != curCoin)
            {
                YxDebug.LogError("玩家[{0}]金币不一致,已同步:旧({1})  新({2})", "HitProcessorPlayer", null, Player.Idx, curCoin, coin);
//                YxDebug.LogWrite("玩家[{0}]金币不一致,已同步:旧({1})  新({2})", null, Player.Idx, curCoin, coin);
                GameMain.Singleton.BSSetting.Dat_PlayersScore[selfSeat].Val = coin;
            } 
        }

        public override void FireEffect(int useScore, bool isLock = false)
        {
            var fishGameServer = FishGameServer.Instance;
            if (fishGameServer != null)
                fishGameServer.PostFirePower(useScore, isLock);
        }

        override protected bool IsKill(int oddsTotal)
        { 
            if (_target > _start)
            {
                return Random.Range(0, 1000) < 1500/oddsTotal;
            }
            return Random.Range(0, 1050) < 1300 / oddsTotal;
        }
         
        override public void BuyCoin(int value)
        {
            _coin += value;
        }

        override public void Retrieve()
        {
            _coin = 0;
        }

        /// <summary>
        /// 是否超出额度
        /// </summary>
        /// <param name="winScore"></param>
        /// <returns></returns>
        private bool OutMaxScore(int winScore)
        {
            int max;
            if (_target < _start)
            {
                max = _start - (_totalBullet - _clear) / 4;
            }
            else
            {
                max = _start + (_totalBullet - _clear) / 2;
            }
            if (winScore > max)
            {
                YxDebug.Log("return：true】    winScore：" + winScore + " max：" + max + " _start：" + _start + " _target：" + _target);
                return true;
            }
            YxDebug.Log("return：false】    winScore：" + winScore + "winScore：" + winScore + " max：" + max + " _start：" + _start + " _target：" + _target);
            return false;
        }

        public override void ProcessHit(Bullet bullet, Fish fish)
        { 
            var totalOdd = fish.GetFishOddBonus(bullet);
            if (Evt_Hit != null) Evt_Hit(totalOdd == 0, bullet.Owner, bullet, fish);
            var fishId = fish.GetInstanceID();
            CollideData data;
            var bulletId = bullet.Id;
            if (_readyDiefishDict.ContainsKey(bulletId))
            { 
                data = _readyDiefishDict[bulletId];
            }
            else
            {
                data = new CollideData();
                _readyDiefishDict.Add(bulletId, data);
            }
            data.DyingFish = fish;
            data.FishType = fish.TypeIndex;
            data.FishOdds = fish.Odds;
            data.TotalOdds = totalOdd;
            data.BulletScore = bullet.Score;
            data.IsLockFishBullet = bullet.IsLockingFish;
            data.BulletOddsMulti = bullet.FishOddsMulti;
            data.FishId = fish.ID;
//            data.KillCount = 
            var selfSeat = App.GameData.SelfSeat % 6;
            var curCoin = GameMain.Singleton.BSSetting.Dat_PlayersScore[selfSeat].Val;
            //YxDebug.LogWarning("碰撞当前:{0} | type:{1} | OddsOddBonus:{2} | bId:{3} | totalOdd:{4} | fishId:{5}", "HitProcessorPlayer",null, curCoin, fish.TypeIndex, fish.Odds,bullet.Id, totalOdd, data.FishId);  
            var fishGameServer = FishGameServer.Instance;
            if (fishGameServer != null)
                fishGameServer.PostHitFishValidity(fishId, bulletId, totalOdd);
        }

        public override bool OneBulletKillFish(int bulletScore, FishOddsData fishFirst)
        {
            _totalBullet += bulletScore;
            //鱼死亡    
            if (fishFirst.Odds <= 1)
            {
                YxDebug.LogError("第一条鱼的odd小于等于一的话就怪了,暂时不可能存在这样的鱼!!,(全屏或者类型炸弹同事出两个可能会出现这种情况)");
                return false;
            }
            _current -= bulletScore;
            _coin -= bulletScore;
            int win = fishFirst.Odds*bulletScore;
            int winScore = _current + win;
        
            if (OutMaxScore(winScore))
            {
                return false;
            }
            bool kill = IsKill( fishFirst.Odds);
            if (kill)
            {
                _current += win; 
                _coin += win;
            }
            return kill;
        }

        public override List<FishOddsData> OneBulletKillFish(int bulletScore, FishOddsData fishFirst, List<FishOddsData> otherFish)
        {
            _totalBullet += bulletScore;
            YxDebug.Log("========手中金币：" + _coin + "  子弹：" + bulletScore);
            _coin -= bulletScore;
            YxDebug.Log("          手中金币："+_coin);
            var fishDieList = new List<FishOddsData>();
            //鱼死亡    
            int oddsTotal = fishFirst.Odds;
            if (fishFirst.Odds <= 1)
            {
                YxDebug.LogError("第一条鱼的odd小于等于一的话就怪了,暂时不可能存在这样的鱼!!,(全屏或者类型炸弹同事出两个可能会出现这种情况)");
                return fishDieList;
            }
            _current -= bulletScore;
            int win = fishFirst.Odds * bulletScore;
            int winScore = _current + win;

            if (OutMaxScore(winScore))
            {
                return fishDieList;
            }
       
            //YxDebug.Log("oddsTotal = " + oddsTotal);
            if (IsKill(fishFirst.Odds))//第一条鱼命中
            {
                fishDieList.Add(fishFirst);
                //YxDebug.Log("firstDieRatio = " + firstDieRatio+"    odds ="+fishFirst.Odds);
                //求其他鱼死是否死亡 ,逐条计算几率
                foreach (FishOddsData f in otherFish)
                {
                    if (f.Odds <= 1)
                    {
                        YxDebug.LogError("otherFish的odd小于等于一的话就怪了,暂时不可能存在这样的鱼!!");
                    }
                    else
                    {
                        oddsTotal += f.Odds;
                    }
                }
                foreach (FishOddsData f in otherFish)
                {
                    int preWin = f.Odds*bulletScore;
                    if (OutMaxScore(preWin + winScore))
                    {
                        continue;
                    }
                    //YxDebug.Log("otherDieRatio = " + dieRatio + "    odds =" + f.Odds);
                    if (IsKill( oddsTotal))
                    {
                        fishDieList.Add(f);
                        winScore += preWin;
                        win += preWin;
                    }
                }
                _current += win; 
                _coin += win;
            }
            return fishDieList;
        }
    }
}
