using System.Collections.Generic;
using YxFramwork.Common;
using YxFramwork.Controller;

namespace Assets.Scripts.Game.lhc
{
    public class LhcGameServer : YxPhpGameServer
    {
        public void ss()
        {
           // SendGameRequest();游戏正常交互发送
            //SendFrameRequest();除了rqst之外的交互 比如退出
        }
        /// <summary>
        /// 发送下注信息
        /// </summary>
        /// <param name="gold"></param>
        public void SendBetReq(int gold,int p)
        {
            var dic=new Dictionary<string,object>();
            dic["lottery_num"] = App.GetGameData<LhcGameData>().LotteryNum;
            dic["loseAnte"] = App.GetGameData<LhcGameData>().LoseAntes;
            dic["type"] = 1;
            dic["p"] = p;
            dic["gold"] = gold;
            dic["seat"] = App.GameData.SelfSeat;
            dic["coin"] = App.GetGameData<LhcGameData>().CurrentBanker.Coin;
            SendGameRequest(dic);
        }

        public void SendApplyBank(long gold)
        {
            var dic = new Dictionary<string, object>();
            dic["loseAnte"] = App.GetGameData<LhcGameData>().LoseAntes;
            dic["lottery_num"] = App.GetGameData<LhcGameData>().LotteryNum;
            dic["coin"] = gold;
            dic["type"] = 2;
            SendGameRequest(dic);
        }

        public void SendRstHistory()
        {
            var dic = new Dictionary<string, object>();
            dic["loseAnte"] = App.GetGameData<LhcGameData>().LoseAntes;
            dic["lottery_num"] = App.GetGameData<LhcGameData>().LotteryNum;
            dic["type"] = 3;
            SendGameRequest(dic);
        }

        public void RequestResultState()
        {
            var dic = new Dictionary<string, object>();
            dic["lottery_num"] = App.GetGameData<LhcGameData>().LotteryNum;
            dic["type"] = 4;
            SendGameRequest(dic);
        }

        public void RequestResult()
        {
            var dic = new Dictionary<string, object>();
            dic["lottery_num"] = App.GetGameData<LhcGameData>().LotteryNum;
            dic["loseAnte"] = App.GetGameData<LhcGameData>().LoseAntes;
            dic["type"] = 5;
            SendGameRequest(dic);
        }
    }
}
