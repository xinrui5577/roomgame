using System.Diagnostics;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;

namespace Assets.Scripts.Game.brnn
{
    public class BrnnGameServer : YxGameServer
    {
        public void ApplyQuit()
        {
            var sfsObject = new SFSObject();
            sfsObject.PutInt("type", RequestType.ApplyQuit);
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sfsObject));
        }

        public void ApplyBanker()
        {
            var sfsObject = new SFSObject();
            sfsObject.PutInt("type", 2);
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sfsObject));
        }

        public void UserBet(int table, int gold)
        {
            
            var sfsObject = new SFSObject();
            sfsObject.PutInt("p", table);
            sfsObject.PutInt("gold", gold);
            sfsObject.PutInt("type", RequestType.Bet);
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sfsObject));
        }

        public void BetAsLastGame(int[] wBet)
        {
            var sfsObject = new SFSObject();

            sfsObject.PutIntArray("golds", wBet);
            sfsObject.PutInt("type", RequestType.Bet);

            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sfsObject));
        }
    }
}
