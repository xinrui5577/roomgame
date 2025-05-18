using Sfs2X.Entities.Data;
using YxFramwork.Common.Model;

namespace Assets.Scripts.Game.biji.Modle
{
    public class BjUserInfo : YxBaseGameUserInfo
    {
        public int[] Cards;
        public bool HasPutCard;
        public bool HasTouXiang;

        public override void Parse(ISFSObject userData)
        {
            base.Parse(userData);
            Cards = userData.ContainsKey("cards") ? userData.GetIntArray("cards") : null;
            HasPutCard = userData.ContainsKey("isput");
            HasTouXiang = userData.ContainsKey("touxiang");
        }
    }
}
