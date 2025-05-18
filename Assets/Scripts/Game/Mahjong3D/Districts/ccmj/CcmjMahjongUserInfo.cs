using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class CcmjMahjongUserInfo : MahjongUserInfo
    {
        public ISFSObject OtherUserData;

        public override void Parse(ISFSObject userData)
        {
            base.Parse(userData);
            OtherUserData = userData;
        }
    }
}
