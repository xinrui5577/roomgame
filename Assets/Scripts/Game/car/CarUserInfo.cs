using Sfs2X.Entities.Data;
using YxFramwork.Common.Model;

namespace Assets.Scripts.Game.car
{
    public class CarUserInfo : YxBaseGameUserInfo
    {
        public int TwentyBet;
        public int TwentyWin;

        public override void Parse(ISFSObject userData)
        {
            base.Parse(userData);
            TwentyBet = userData.ContainsKey("twentyBet") ? userData.GetInt("twentyBet") : -1;
            TwentyWin = userData.ContainsKey("twentyWin") ? userData.GetInt("twentyWin") : -1;
        }
    }
}
