namespace Assets.Scripts.Game.jh.network
{
    public class JhTypeKey 
    {

        public const int TypeReady = 1;
        /*发牌*/
        public const int TypeFaPai = 10;
        /*跟注*/
        public const int TypeGenZhu = 11;
        /*弃牌*/
        public const int TypeQiPai = 12;
        /*看牌*/
        public const int TypeKanPai = 13;
        /*比牌*/
        public const int TypeBiPai = 14;
        /*结束*/
        public const int TypeOver = 15;
        public const int TypeDefault = 16;
        /// <summary>
        /// 
        /// </summary>
        public const int TypeCurInfo = 18;
        public const int TypeStart = 19;

        public const int TypeGZYZ = 17;
        /*显示用到的加注*/
        public const int TypeJiaZhu = -11;

        /*游戏 准备*/
        public const int TypeGameReady = 20;

        public const int TypeShowCard = 30;
    }

    public class JhRequestConstKey
    {
        public const string KeyUserId = "userid";

        public const string KeyBetGold = "betGold";

        public const string KeyPlayings = "playings";//需要发牌的人;


        public const string KeyLeaved = "leaved";

        public const string KeyBanker = "banker";///////

        public const string KeyPhoto = "avatar";///////头像


        public const string KeySex = "sex";///////

        public const string KeyAward = "award";//////

        public const string KeySignature = "signature";///////

        public const string KeyCall = "call";///////
        /**金币，或者钱。[Key]*/
        public const string KeyCash = "cash";
        /**总场数*/
        public const string KeyTotal = "total";

        public const string KeyWin = "win";

        public const string KeyQi = "qi";
        public const string KeyTax = "tax";//税收

        public const string KeyLost = "lost";

        public const string KeyWinner = "winner";//比牌获胜

        public const string KeyLoster = "loster";//比牌失败
        /**赢得的量*/

        public const string KeyToSeat = "toseat";/////////
        public const string KeySeat = "seat";/////////

        public const string KeyState = "state";//*/

        public const string KeyExp = "exp";
        public const string KeyText = "text";


        public const string KeyAnte = "ante";


        public const string KeyNowDanzhu = "minGold";

        public const string KeyLunshu = "round";

        public const string KeyCurrentP = "currentP";

        public const string KeyPsTime = "psTime";

        public const string KeyIsFail = "isFail";

        public const string KeyIsGiveUp = "qipai";

        public const string KeyIsLook = "kanpai";

        public const string KeyRejion = "rejoin";

        public const string KeyCompare = "compare";

        public const string KeyLook = "look";

        public const string KeyPlaying = "playing";

        public const string KeyNetWork = "network";
    }
}