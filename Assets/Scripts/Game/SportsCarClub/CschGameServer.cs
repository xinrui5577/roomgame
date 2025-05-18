using System.Collections;
using YxFramwork.Controller;
using Sfs2X.Entities.Data;
using com.yxixia.utile.YxDebug;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.SportsCarClub
{
    public class CschGameServer : YxGameServer
    {

        private static CschGameServer _instance; 
        public static CschGameServer GetInstance()
        {
            return _instance ?? (_instance = new CschGameServer());
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            _instance = this;
        }
          
        public void SendRequest(RequestType rt,IDictionary data)
        {
            if (!HasGetGameInfo)
                return;

            YxDebug.Log("SendRequest == " + rt.ToString());

            var sfsObject = new SFSObject();

            switch (rt)
            {
                case RequestType.Bet:
                    if (data.Contains("golds"))
                    {
                        sfsObject.PutIntArray("golds", (int[])data["golds"]);
                        sfsObject.PutInt("type", (int)RequestType.Bet);
                        YxDebug.Log("发送重复上轮");
                    }
                    else
                    {
                        sfsObject.PutInt("p", (int)data["p"]);
                        sfsObject.PutInt("gold", (int)data["gold"]);
                        sfsObject.PutInt("type", (int)RequestType.Bet);
                    }
                    break;
                case RequestType.Reward:
                    break;
                case RequestType.ApplyBanker:
                    sfsObject.PutInt("type", (int)RequestType.ApplyBanker);
                    break;
                case RequestType.ApplyQuit:
                    sfsObject.PutInt("type", (int)RequestType.ApplyQuit);
                    break;
                case RequestType.BeginBet:
                    break;
                case RequestType.EndBet:
                    break;
                case RequestType.GiveCards:
                    break;
                case RequestType.Result:
                    break;
                case RequestType.BankerList:
                    break;
                default:
                    YxDebug.Log("不存在的服务器交互!");
                    return;
            }
            SendFrameRequest(GameKey + RequestCmd.Request,sfsObject);
        }
    }

    /// <summary>
    /// 服务器交互
    /// </summary>
    public enum RequestType
    {
        Bet = 1, //下注
        Reward = 2, //打赏妹子钱(100一次)
        ApplyBanker = 3, //上庄
        ApplyQuit = 4, //下庄
        BeginBet = 5, //开始下注
        EndBet = 6, //下注结束
        GiveCards = 7, //发牌信息
        Result = 8, //结果
        BankerList = 9, //庄家列表
    }
}