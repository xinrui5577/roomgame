using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class NamjMahjongUserInfo : MahjongUserInfo
    {
        public int Guoval = -1;

        public override void Reset()
        {
            base.Reset();
            Guoval = -1;
        }

        public override void Parse(ISFSObject userData)
        {
            base.Parse(userData);

            if (userData.ContainsKey("qiduitings"))
            {
                int[] array = userData.GetIntArray("qiduitings");
                SetTinglist(array);
            }
            if (userData.ContainsKey("guoval"))
            {
                Guoval = new VarInt(userData.GetInt("guoval"));
            }
        }
    }
}
