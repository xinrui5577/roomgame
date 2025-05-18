using Sfs2X.Entities.Data;
using YxFramwork.Common.Model;

namespace Assets.Scripts.Game.jlgame.Modle
{
    public class JlGameUserInfo : YxBaseGameUserInfo
    {
        public int[] Cards;
        public int FoldNum;
        public int CardLen;
        public int FoldScore;
        public int[] FoldCards;
        public int[] ActiveCards;
        public bool IsCurSpeaker;
        public bool IsTrusteeship;

        public override void Parse(ISFSObject userData)
        {
            base.Parse(userData);
            Cards = userData.ContainsKey("cards") ? userData.GetIntArray("cards") : null;
            FoldNum = userData.ContainsKey("foldNum") ? userData.GetInt("foldNum") : -1;
            CardLen = userData.ContainsKey("cardLen") ? userData.GetInt("cardLen") : -1;
            FoldCards = userData.ContainsKey("foldCards") ? userData.GetIntArray("foldCards") : null;
            ActiveCards = userData.ContainsKey("activeCards") ? userData.GetIntArray("activeCards") : null;
            IsTrusteeship = userData.ContainsKey("trusteeship") && userData.GetBool("trusteeship");//
            FoldScore = userData.ContainsKey("foldScore") ? userData.GetInt("foldScore") : -1;
        }

    }
}
