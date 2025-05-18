using UnityEngine;

namespace Assets.Scripts.Game.ttz
{
    public class RequestType
    {
        /// <summary>
        /// 1.上庄请求
        /// </summary>
        public const int ApplyBanker = 101;
        /// <summary>
        /// 2.下庄请求
        /// </summary>
        public const int ApplyQuit = 102;
        /// <summary>
        /// 3.发送庄家列表
        /// </summary>
        public const int BankerList = 103;
        /// <summary>
        /// 4.上庄带钱
        /// </summary>
        public const int BankerGold = 104;
        /// <summary>
        /// 5.开始下注
        /// </summary>
        public const int StartBet = 105;
        /// <summary>
        /// 6.停止下注
        /// </summary>
        public const int StopBet = 106;
        /// <summary>
        /// 7.下注
        /// </summary>
        public const int Bet = 107;
        /// <summary>
        /// 8.发牌
        /// </summary>
        public const int SendCard = 108;
        /// <summary>
        /// 9.结算
        /// </summary>
        public const int Result = 109;
        /// <summary>
        /// 10.流式下注
        /// </summary>
        public const int GroupBet = 110;
        /// <summary>
        /// 11.发明牌
        /// </summary>
        public const int SendMingCards = 30;
    }

    public class Parameter
    {
        public const string Cards = "cards";
        public const string Cd = "cd";
        public const string Seat = "seat";
        public const string Gold = "gold";
        public const string BankRound = "bankRound";
        public const string BankerGold = "bankerGold";
        public const string Bankers = "bankers";
        public const string Banker = "banker";
        public const string Win = "win";
        public const string Bwin = "bwin";
        public const string P = "p";
        public const string Coin = "coin";
        public const string Type = "type";
        public const string Pg = "pg";
        public const string Total = "total";
        public const string Bpg = "bpg";
        public const string Dices = "dices";
        public const string XiPai = "xipai";
        public const string Value = "value";
        public const string RCards = "rCards";
        public const string Record = "record";
        public const string Bround = "bround";
        public const string UserName = "username";
        public const string Status = "status";
        public const string RollResult = "rollResult";
        public const string Bet = "bet";
        public const string SameCardNum = "sameCardNum";
    }

}
