using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class QjmjActionReconnect : ActionReconnect
    {
        public override void ReconnectAction(ISFSObject data)
        {
            base.ReconnectAction(data);
            MahjongUserInfo userInfo;
            var groups = Game.MahjongGroups;
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                userInfo = DataCenter.Players[i];
                var lzGang = userInfo.LaiziGangCards;
                if (lzGang == null || lzGang.Length < 1) continue;
                for (int j = 0; j < lzGang.Length; j++)
                {
                    groups.PopMahFromCurrWall();
                    groups.MahjongOther[i].GetInMahjong(lzGang[j]).Laizi = DataCenter.IsLaizi(lzGang[j]);
                }
                //麻将记录
                GameCenter.Shortcuts.MahjongQuery.AddRecordMahjongs(lzGang);
            }
        }
    }
}
