using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.ttz
{
    public class BrttzGameServer : YxGameServer
    {
        public void UserBet(string table, int gold)
        {
            var sfsObject = new SFSObject();
            sfsObject.PutUtfString(Parameter.P, table);
            sfsObject.PutInt(Parameter.Gold, gold);
            sfsObject.PutInt(Parameter.Type, RequestType.Bet);//根据后台下注的值
            SendGameRequest(sfsObject);
        }

        public void ApplyQuit()
        {
            var sfsObject = new SFSObject();
            sfsObject.PutInt(Parameter.Type, RequestType.ApplyQuit);
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sfsObject));
        }

        public void ApplyBanker()
        {
            var sfsObject = new SFSObject();
            sfsObject.PutInt(Parameter.Type, RequestType.ApplyBanker);
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sfsObject));
        }

        public void BetAsLastGame(int[] wBet)
        {
            var sfsObject = new SFSObject();

            sfsObject.PutIntArray("golds", wBet);
            sfsObject.PutInt(Parameter.Type, RequestType.Bet);
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sfsObject));
            App.GetGameManager<BrttzGameManager>().CanQuitGame = false;
        }
    }
}