using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;

namespace Assets.Scripts.Game.BaiTuan
{
    public class BtwGameServer : YxGameServer
    {
        public void UserBet(int table, int gold)
        {
            var sfsObject = new SFSObject();
            sfsObject.PutInt("p", table);
            sfsObject.PutInt("gold", gold);
            sfsObject.PutInt("type", (int)BtwSkin02RequestType.XiaZhu);
            SendGameRequest(sfsObject);
        }

        public void ApplyBanker()
        {
            var sfsObject = new SFSObject();
            sfsObject.PutInt("type", (int)BtwSkin02RequestType.ShangZhuang);
            SendGameRequest(sfsObject);
        }

        public void ApplyQuit()
        {
            var sfsObject = new SFSObject();
            sfsObject.PutInt("type", (int)BtwSkin02RequestType.XiaZhuang);
            SendGameRequest(sfsObject);
        }

        public void BetAsLastGame(int[] wBet)
        {
            var sfsObject = new SFSObject();

            sfsObject.PutIntArray("golds", wBet);
            sfsObject.PutInt("type", (int)BtwSkin02RequestType.XiaZhu);

            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sfsObject));
        }
        public void UserBetOne(int table, int gold)
        {
            var sfsObject = new SFSObject();
            sfsObject.PutInt("p", table);
            sfsObject.PutInt("gold", gold);
            sfsObject.PutInt("type", (int)BtwRequestType.Bet);
            SendGameRequest(sfsObject);
        }

        public void ApplyBankerOne()
        {
            var sfsObject = new SFSObject();
            sfsObject.PutInt("type", (int)BtwRequestType.ApplyBanker);
            SendGameRequest(sfsObject);
        }

        public void ApplyQuitOne()
        {
            var sfsObject = new SFSObject();
            sfsObject.PutInt("type", (int)BtwRequestType.ApplyQuit);
            SendGameRequest(sfsObject);
        }
        
    }

}