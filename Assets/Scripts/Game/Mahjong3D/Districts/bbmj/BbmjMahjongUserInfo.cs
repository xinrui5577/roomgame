using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class BbmjMahjongUserInfo : MahjongUserInfo
    {
        public override void Parse(ISFSObject userData)
        {
            base.Parse(userData);
            IsAuto = userData.ContainsKey("cards");
        }
    }
}
