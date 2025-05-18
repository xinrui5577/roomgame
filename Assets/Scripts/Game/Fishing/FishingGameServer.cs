using System;
using System.Collections.Generic;
using Assets.Scripts.Game.Fishing.Constants;
using Assets.Scripts.Game.Fishing.entitys;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Game.Fishing
{
    /// <summary>
    /// 捕鱼交互
    /// </summary>
    public class FishingGameServer : YxGameServer
    {

        private int _seq = 7;
        private string _secureKey;


        public override void Init(Dictionary<string, Action<ISFSObject>> callBackDic)
        {
            var ks = new[] { 'A', 'B', '&', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K' };
            _secureKey = App.UserId + "gmima11xx33cao" + ks[2] + ks[2] + "khoiakkf_aujs";
        }


        private void PostData(ISFSObject data, int coin)
        {
            var sKey = _secureKey + _seq + "_" + coin;
            var key = GlobalUtils.MD5String(sKey); //adq
            //YxDebug.Log("发送金币同步【PostData】:" + coin + "   签名：" + sKey +"  "+ _seq);
            data.PutUtfString("sign", key);
            data.PutInt("cynccoin", coin);
            SendGameRequest(data);
            _seq++;
        }

        /// <summary>
        /// 发送子弹请求
        /// </summary>
        public void PostFirePower(int score, bool isLock = false)
        {
            YxDebug.LogWarning(">>>> 请求子弹");
            var data = SFSObject.NewInstance();
            data.PutInt(RequestKey.KeyType, (int)FishRequestType.FirePower);
            data.PutInt(FishRequestKey.Blt, score);
            data.PutBool(FishRequestKey.LockB, isLock);
            SendGameRequest(data);
        }

        /// <summary>
        /// 打中鱼的有效性
        /// </summary>
        public void PostHitFishValidity(int fishId, int bulletId, int rate)
        {
            var data = SFSObject.NewInstance();
            data.PutInt(RequestKey.KeyType, (int)FishRequestType.HitFish);
            data.PutInt(FishRequestKey.Fid, fishId);
            data.PutInt(FishRequestKey.Bid, bulletId);
            data.PutInt(FishRequestKey.Rate, rate); 
            SendGameRequest(data);
        }

        /// <summary>
        /// 上分
        /// </summary>
        /// <param name="coin">上分数</param>
        public void SendBuyCoin(int coin = 0)
        {
            var gdata = App.GameData; 
            var userinfo = gdata.GetPlayerInfo<FishingUserInfo>();
            var totalCoin = userinfo.CoinA;
            if (coin < 1)
            {
                coin = (int)totalCoin;
            }
            if (coin > totalCoin)
            {
                YxMessageBox.Show("金币不足" + YxUtiles.ReduceNumber(coin) + "，不足以兑换！！");
                return;
            }
            var buy = new SFSObject();
            buy.PutInt(RequestKey.KeyType, (int)FishRequestType.BuyCoin);
            buy.PutInt("coin", coin);
            PostData(buy, 0);
        }

        /// <summary>
        /// 下分
        /// </summary>
        /// <param name="coin">下分数</param>
        public void SendSellCoind(int coin)
        {
        }

        /// <summary>
        /// 机器人退出
        /// </summary>
        /// <param name="nid"></param>
        public void SendRobotQuit(int nid)
        {
            var parm = new SFSObject();
            parm.PutInt(RequestKey.KeyType, (int)FishRequestType.RobotOut);
            parm.PutInt("nid", nid);
            SendGameRequest(parm);
        }

        /// <summary>
        /// 发送获奖消息
        /// </summary>
        /// <param name="userName">玩家的名字</param>
        /// <param name="fishName">鱼的名字</param>
        /// <param name="coin">获得的奖励</param>
        /// <param name="fishRate">鱼的倍数</param>
        public void SendUserWinAwardMsg(string userName, string fishName, int coin, int fishRate)
        {
        }


        /// <summary>
        /// 退出游戏时发送
        /// </summary>
        public void SendQuitGame()
        {
     
        }
    }

    public enum FishRequestType
    {
        HitFish = 0,
        BuyCoin = 1,
        Sell = 2,
        Quit = 4,
        RobotOut = 6,
        Message = 7,
        FirePower = 8
    }
}
