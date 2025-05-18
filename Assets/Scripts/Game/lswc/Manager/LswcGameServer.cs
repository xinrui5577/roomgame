using Assets.Scripts.Game.lswc.Data;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.lswc.Manager
{
    public class LswcGameServer : YxGameServer
    {      
        public void SendBetRequest()
        {
            var gdata = App.GetGameData<LswcGameData>();
            if (gdata.GlobalELswcGameStatu == ELswcGameState.BetState)
            {
                gdata.GlobalELswcGameStatu = ELswcGameState.ResultState;
                gdata.ISGetResult = false;
                if (gdata.TotalBets == 0)
                {
                    YxDebug.Log("未下注，不发送");
                    return;
                }
                ISFSObject data = new SFSObject();
                data.PutInt(RequestKey.KeyType,(int)LSRequestMessageType.BET);
                data.PutIntArray(LSConstant.KeyAntes, gdata.Bets);
                SendGameRequest(data);
                YxDebug.Log("发送下注请求,下注总金额是：" + gdata.TotalBets);
            }
        }
    }
}
