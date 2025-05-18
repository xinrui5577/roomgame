using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.jlgame.network
{
    public class JlGameTypeKey
    {
        /*发牌*/
        public const int TypeAllocate = 10;
        /*谁出牌*/
        public const int TypeSpeaker = 11;
        /*出牌*/
        public const int TypeOutCard = 12;
        /*扣牌*/
        public const int TypeFoldCard = 13;
        /*结算*/
        public const int TypeResult = 14;
        /*托管交互*/
        public const int TypeTrusteeship = 15;
    }

    public class JlGameRequestConstKey
    {
        public const string KeyCard = "card";

        public const string KeyCards = "cards";

        public const string KeyCardsNum = "cardsNum";

        public const string KeyCdTime = "cd";

        public const string KeyActiveCards = "activeCards";

        public const string KeyFoldNum = "foldNum";

        public const string KeyHandNum = "handNum";

        public const string KeyFoldScore = "foldScore";

        public const string KeyResult = "result";

        public const string KeyKillDragon = "killDragon"; 

        public const string KeyIsDragon = "isDragon";

        public const string KeyTrusteeshipa = "trusteeship";

    }

    public class ResultData
    {
        public int Win;
        public long TtGold;
        public int FoldScore;
        public string Name;
        public int[] FoldCards;
        public int Seat;
        public short Sex;
        public string Head;
        public bool IsRoomOwner;
        public bool IsYouSelf;
        public bool IsNoFold;

        public ResultData(ISFSObject data)
        {
            Win = data.GetInt("win");
            TtGold = data.GetLong("ttgold");
            FoldScore = data.GetInt("foldScore");
            Name = data.GetUtfString("name");
            FoldCards = data.GetIntArray("foldCards");
            Seat = data.GetInt("seat");
            Sex = data.GetShort("sex");
            Head = data.GetUtfString("avatar");
            IsNoFold = data.GetBool("isNoFold");
        }
    }

    public class HupData
    {
        public int UserId;
        public string UserName;
        public int Type;
        public bool IsSelf;
        public int CdTime;
        public HupData(ISFSObject data)
        {
            UserId = data.GetInt("userId");
            UserName = data.GetUtfString("username");
            Type = data.GetInt("type");
            CdTime = data.ContainsKey("cdTime") ? data.GetInt("cdTime") : 300;
        }
    }

    public class TtResultUserData
    {
        public int UserId;
        public string UserName;
        public int UserGold;
        public string UserHead;
        public short Sex;
        public bool IsWinner;
        public bool IsRoomOwner;

        public TtResultUserData(ISFSObject data)
        {
            UserId = data.GetInt("id");
            UserName = data.GetUtfString("nick");
            UserHead = data.GetUtfString("avatar");
            UserGold = data.GetInt("gold");
            Sex = data.GetShort("sex");
           
        }
    }
}