using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;

namespace Assets.Scripts.Game.bjl3d
{
    public class Bjl3DGameServer : YxGameServer
    {
        public void UserBet(int table, int gold)
        {
            var sfsObject = new SFSObject();
            sfsObject.PutInt("p", table);
            sfsObject.PutInt("gold", gold);
            sfsObject.PutInt("type", 1);
            SendGameRequest(sfsObject);
        }
        public void ApplyBanker()
        {
            var sfsObject = new SFSObject();
            sfsObject.PutInt("type", RequestType.ApplyBanker);
            SendGameRequest(sfsObject);
        }

        public void ApplyQuit()
        {
            var sfsObject = new SFSObject();
            sfsObject.PutInt("type", RequestType.ApplyQuit);
            SendGameRequest(sfsObject);
        } 
    }
}
