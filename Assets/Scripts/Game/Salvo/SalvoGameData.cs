using Sfs2X.Entities.Data;
using YxFramwork.Common.Utils;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.Salvo
{
    public class SalvoGameData : YxGameData
    {
        public int Bet=0;
        public int BaseBet = 100;

        protected override void InitGameData(ISFSObject gameInfo)
        {
            base.InitGameData(gameInfo);
            BaseBet = gameInfo.GetInt(RequestKey.KeyAnte);
        }
    }
}
