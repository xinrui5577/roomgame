using Sfs2X.Entities.Data;
using YxFramwork.Controller;

namespace Assets.Scripts.Game.hg
{
    public class HgGameServer : YxGameServer
    {
        public EventObject EventObj;

        public void OnRecive(EventData data)
        {
            switch (data.Name)
            {
                case "Bet":
                    var betData = (ISFSObject) data.Data;
                    UserBet(betData.GetUtfString("p"), betData.GetInt("gold"));
                    break;
                case "ContinuteBet":
                    ContinuteBet((int[]) data.Data);
                    break;
                case "ApplyBank":
                    ApplyBank();
                    break;
                case "GiveUpBank":
                    GiveUpBank();
                    break;
            }
        }

        private void UserBet(string pos,int gold)
        {
            ISFSObject betData = new SFSObject();
            betData.PutUtfString("p",pos);
            betData.PutInt("gold", gold);
            betData.PutInt("type",107);
            SendGameRequest(betData);
        }

        private void ContinuteBet(int[] golds)
        {
            ISFSObject betData = new SFSObject();
            betData.PutIntArray("golds", golds);
            betData.PutInt("type", 107);
            SendGameRequest(betData);
        }

        private void ApplyBank()
        {
            ISFSObject betData = new SFSObject();
            betData.PutInt("type", 101);
            SendGameRequest(betData);
        }

        private void GiveUpBank()
        {
            ISFSObject betData = new SFSObject();
            betData.PutInt("type", 102);
            SendGameRequest(betData);
        }
    }
}
