using System;
using System.Collections.Generic;
using Assets.Scripts.Game.FishGame.ChessCommon;
using Assets.Scripts.Game.FishGame.Common.Utils;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.View;
using com.yxixia.utile.YxDebug;
using YxFramwork.Tool;
using GlobalUtils = Assets.Scripts.Game.FishGame.Utils.GlobalUtils;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Game.FishGame.Common.Servers
{
    public class FishGameServer : YxGameServer
    {
        private int _seq = 7;
        private string _secureKey;
        /// <summary>
        /// 加载状态 
        /// </summary>
        private int _loaderState;

        public static FishGameServer Instance { get; private set; }

        protected override void OnAwake()
        {
            Instance = this;
        }

        public override void Init(Dictionary<string, Action<ISFSObject>> callBackDic)
        {
            var ks = new[] {'A', 'B', '&', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K'};
            _secureKey = App.UserId + "gmima11xx33cao" + ks[2] + ks[2] + "khoiakkf_aujs";
        }

        public void PostGameData(int totalbullet, int bullet, int odds, int coin)
        {
            const int keyLen = 7;
            var x = bullet + "," + odds + "," + coin + "," + totalbullet;
            YxDebug.Log(x);
            var cs = new char[keyLen + x.Length];
            int[] encs =
                {
                    64, 65, 84, 85, 86, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104,
                    105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122
                };
            for (var i = 0; i < keyLen; i++)
            {
                cs[i] = (char) encs[Random.Range(0, encs.Length)];
            }
            for (var i = keyLen; i < cs.Length; i++)
            {
                cs[i] = (char) (cs[i%keyLen] ^ x[i - keyLen]);
            }

            var data = SFSObject.NewInstance();
            data.PutInt(RequestKey.KeyType, 0);
            x = new string(cs);
            data.PutUtfString("x", x);
            PostData(data, coin);
        }

        /// <summary>
        /// 发送子弹请求
        /// </summary>
        public void PostFirePower(int score, bool isLock = false)
        {
            YxDebug.LogWarning(">>>> 请求子弹");
            var data = SFSObject.NewInstance();
            data.PutInt(RequestKey.KeyType, (int) FishRequestType.FirePower);
            data.PutInt(FishRequestKey.Blt, score);
            data.PutBool(FishRequestKey.LockB, isLock);
            SendGameRequest(data);
        }

        /// <summary>
        /// 打中鱼的有效性
        /// </summary>
        public void PostHitFishValidity(int fishId, int bullteId, int rate)
        {
            var data = SFSObject.NewInstance();
            data.PutInt(RequestKey.KeyType, (int) FishRequestType.HitFish);
            data.PutInt(FishRequestKey.Fid, fishId);
            data.PutInt(FishRequestKey.Bid, bullteId);
            data.PutInt(FishRequestKey.Rate, rate);
            SendGameRequest(data);
        }

        /// <summary>
        /// 退出游戏时发送
        /// </summary>
        public void SendQuitGame()
        {
            ISFSObject data = new SFSObject();
            data.PutInt(RequestKey.KeyType, (int) FishRequestType.Quit);
            var coin = GameMain.Singleton.BSSetting.Dat_PlayersScore[0].Val;
            YxDebug.Log("告诉服务器本地退出游戏：最终的金币（" + coin + ")");
            PostData(data, coin);
        }

        private void PostData(ISFSObject data, int coin)
        {
            var sKey = _secureKey + _seq + "_" + coin;
            var key = GlobalUtils.Md5String(sKey); //adq
            //YxDebug.Log("发送金币同步【PostData】:" + coin + "   签名：" + sKey +"  "+ _seq);
            data.PutUtfString("sign", key);
            data.PutInt("cynccoin", coin);
            SendGameRequest(data);
            _seq++;
        }

        public void SendBuyCoin(int baseCoin)
        {
            var totalCoin = App.GetGameData<FishGameData>().TotalCoin;
            if (baseCoin > totalCoin)
            {
                YxMessageBox.Show("金币不足" + YxUtiles.ReduceNumber(baseCoin)+ "，不足以兑换！！");
                App.GetGameData<FishGameData>().CanBuyCoin = true;
                return;
            }
            var buy = new SFSObject();
            buy.PutInt(RequestKey.KeyType, (int) FishRequestType.BuyCoin);
            buy.PutInt("coin", baseCoin);
            PostData(buy, 0);
        }

        public void SendSellCoind(int coin)
        { 
            if (coin<=0)
            {
                App.GetGameData<FishGameData>().CanRetrieveCoin = true;
                return;
            }
            var sell = new SFSObject();
            sell.PutInt(RequestKey.KeyType, (int) FishRequestType.Sell);
            sell.PutInt("coin", coin);
            PostData(sell, 0);
        }

        public void SendRobotQuit(int nid)
        {
            var parm = new SFSObject();
            parm.PutInt(RequestKey.KeyType, (int) FishRequestType.RobotOut);
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
        public void SendUserWinAwardMsg(string userName, string fishName, int coin,int fishRate)
        {
            var gdata = App.GetGameData<FishGameData>();
            if (gdata == null || !gdata.RadiateMsg) return;
            if (string.IsNullOrEmpty(fishName)) return;
            if (App.GetGameData<FishGameData>().Msgrate > fishRate) return;
//            var msg = string.Format("恭喜玩家【{0}】捕捉到【{1}】,获得大奖 {2}", userName, fishName, coin);
            var parm = new SFSObject();
            parm.PutInt(RequestKey.KeyType, (int) FishRequestType.Message);
            parm.PutUtfString("UserName", userName);
            parm.PutUtfString("FishName", fishName);
            parm.PutInt("Coin", coin);
            parm.PutInt("FishRate", fishRate);
//            parm.PutUtfString(FishRequestKey.Msg, msg);
            SendGameRequest(parm);
        } 
    }



    enum FishRequestType
    {
        HitFish = 0,
        BuyCoin =1,
        Sell = 2,
        Quit = 4,
        RobotOut=6,
        Message = 7,
        FirePower = 8
    }
}
