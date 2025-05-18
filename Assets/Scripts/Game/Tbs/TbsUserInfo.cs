using YxFramwork.Common.Model;

namespace Assets.Scripts.Game.Tbs
{
    public class TbsUserInfo : YxBaseGameUserInfo
    {
        public int BetGold;

        public override void Parse(Sfs2X.Entities.Data.ISFSObject userData)
        {
            base.Parse(userData);
            BetGold = userData.GetInt("betgold");
        }
    }
}
