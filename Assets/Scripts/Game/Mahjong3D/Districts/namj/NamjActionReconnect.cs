using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class NamjActionReconnect : ActionReconnect
    {
        public override void ReconnectAction(ISFSObject data)
        {
            base.ReconnectAction(data);
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                var namjUser = DataCenter.Players[i].ConverType<NamjMahjongUserInfo>();
                var guoval = namjUser.Guoval;
                if (namjUser.Guoval > 0)
                {
                    int[] array = new int[2] { guoval, guoval };
                    GameCenter.Scene.MahjongGroups.MahjongHandWall[i].SetHandCardState(HandcardStateTyps.TingAndShowCard, array);
                }
            }
        }
    }
}
