using Sfs2X.Entities.Data;
using YxFramwork.Common.Model;

namespace Assets.Scripts.Game.Ttzkf
{
    public class TtzUserInfo : YxBaseGameUserInfo
    {
        public int LostCoin;
        public bool HasCard;
        public int BetGold;

        public override void Parse(ISFSObject userData)
        {
            base.Parse(userData);
            LostCoin = userData.GetInt("lost");
            HasCard = userData.ContainsKey("cards");
            BetGold = userData.ContainsKey("betGold") ? userData.GetInt("betGold") : 0;
        }
    }
}
