/*===================================================
 *文件名称:     ConstantData.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-06-27
 *描述:        	常量数据
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.nbjl
{
    public class ConstantData 
    {
        #region  Request Key
        /// <summary>
        /// Key上庄列表
        /// </summary>
        public const string KeyBankers = "bankers";

        /// <summary>
        /// Key庄家座位号
        /// </summary>
        public const string KeyBankerSeat= "banker";

        /// <summary>
        /// Key 历史记录
        /// </summary>
        public const string KeyRecord= "record";

        /// <summary>
        /// Key 上庄下限
        /// </summary>
        public const string KeyBankLimit = "bankLimit";

        /// <summary>
        /// Key 下注上限（单门）
        /// </summary>
        public const string KeyBetMax= "maxante";

        /// <summary>
        /// Key 下注区域
        /// </summary>
        public const string KeyBetPosition = "p";

        /// <summary>
        /// Key 各个注点允许下注的数量
        /// </summary>
        public const string KeyAllows = "allow";

        /// <summary>
        /// Key 金额
        /// </summary>
        public const string KeyGold = "gold";
        /// <summary>
        /// Key 金额们
        /// </summary>
        public const string KeyGolds = "golds";

        /// <summary>
        /// key 类型
        /// </summary>
        public const string KeyType = "type";

        /// <summary>
        /// 玩家昵称
        /// </summary>
        public const string KeyUserName = "username";

        /// <summary>
        /// Key玩家列表记录局数
        /// </summary>
        public const string KeyAccumulateCount = "playerRecordNum";

        /// <summary>
        /// Key累计下注数量
        /// </summary>
        public const string KeyAccumulateBet = "twentyBet";

        /// <summary>
        /// Key累计胜利次数
        /// </summary>
        public const string KeyAccumulateWin = "twentyWin";

        /// <summary>
        /// Key金币排行
        /// </summary>
        public const string KeyGoldRank = "goldRank";

        /// <summary>
        /// Key神算子座位号
        /// </summary>
        public const string KeySsz = "ssz";
        
        /// <summary>
        /// Key下注区域值
        /// </summary>
        public const string KeyRateValue = "rateValue";

        /// <summary>
        /// Key冷却时间
        /// </summary>
        public const string KeyCd = "cd";

        /// <summary>
        /// Key值(翻牌最终结果)
        /// </summary>
        public const string KeyValue = "value";

        /// <summary>
        /// Key 牌值
        /// </summary>
        public const string KeyCards = "cards";

        /// <summary>
        /// Key 玩家信息
        /// </summary>
        public const string KeyUser = "user";

        /// <summary>
        /// Key 玩家列表
        /// </summary>
        public const string KeyUsers = "users";

        /// <summary>
        /// Key 在线玩家列表
        /// </summary>
        public const string KeyPlayerList = "playerlist";

        /// <summary>
        /// Key 庄家（牌）
        /// </summary>
        public const string KeyZhuang = "zhuang";

        /// <summary>
        /// Key 闲家（牌）
        /// </summary>
        public const string KeyXian = "xian";

        /// <summary>
        /// Key 胜利次数
        /// </summary>
        public const string KeyWinCount = "win";

        /// <summary>
        /// Key 结算下注结果
        /// </summary>
        public const string KeyBetPage = "bpg";

        /// <summary>
        /// Key庄家赢得金币
        /// </summary>
        public const string KeyBankerWinGold = "bwin";

        /// <summary>
        /// Key 总金币
        /// </summary>
        public const string KeyTotalGold = "ttgold";

        /// <summary>
        ///Key 玩家ID
        /// </summary>
        public const string KeyUserId = "userid";
       
        /// <summary>
        /// Key 玩家座位号
        /// </summary>
        public const string KeySeat = "seat";

        /// <summary>
        /// Key 游戏状态
        /// </summary>
        public const string KeyStatus = "status";

        /// <summary>
        /// Key 下注类型庄
        /// </summary>
        public const string KeyBetBanker = "z";

        /// <summary>
        /// Key 下注类型庄
        /// </summary>
        public const string KeyBetLeisure = "x";

        /// <summary>
        /// Key 下注类型和
        /// </summary>
        public const string KeyBetEqual= "h";

        /// <summary>
        /// Key 下注类型庄对
        /// </summary>
        public const string KeyBetBankerDouble = "zd";

        /// <summary>
        /// Key 下注类型闲对
        /// </summary>
        public const string KeyBetLeisureDouble = "xd";

        /// <summary>
        /// Key 下注类型庄天王
        /// </summary>
        public const string KeyBetBankerKing = "zt";

        /// <summary>
        /// Key 下注类型闲天王
        /// </summary>
        public const string KeyBetLeisureKing = "xt";

        /// <summary>
        /// 对子
        /// </summary>
        public const string KeyDoubleCard = "dz";

        /// <summary>
        /// 天王
        /// </summary>
        public const string KeyKing = "tw";

        /// <summary>
        /// 下注结果
        /// </summary>
        public const string KeyRollResult = "rollResult";

        /// <summary>
        /// 流式下注标记
        /// </summary>
        public const string KeyCoin = "coin";

        #endregion

        #region Game Constant Data
        /// <summary>
        /// Key下注音效
        /// </summary>
        public const string KeySoundBet = "Bet";

        /// <summary>
        /// Key流式下注音效
        /// </summary>
        public const string KeySoundFlushBet = "groupbet";

        /// <summary>
        /// Key和
        /// </summary>
        public const string KeySoundEqule = "he";

        /// <summary>
        /// Key开始下注
        /// </summary>
        public const string KeySoundBeginBet = "beginbet";

        /// <summary>
        /// Key停止下注
        /// </summary>
        public const string KeySoundEndBet = "stopbet";

        /// <summary>
        /// Key 庄赢
        /// </summary>
        public const string KeySoungBankerWin = "zhuangbet";

        /// <summary>
        /// Key 闲赢
        /// </summary>
        public const string KeySoungLeisureWin = "xianbet";

        /// <summary>
        /// Key庄家
        /// </summary>
        public const string KeySoundBanker = "banker";

        /// <summary>
        /// Key闲家
        /// </summary>
        public const string KeySoundLeisure = "leisure";
        
        /// <summary>
        ///  Key追加闲方
        /// </summary>
        public const string KeySoundLeisureAdd = "leisureadd";

        /// <summary>
        ///  Key追加庄方
        /// </summary>
        public const string KeySoundBankerAdd = "bankeradd";

        /// <summary>
        /// Key 牌数量，配合cards使用
        /// </summary>
        public const string KeyCardNum = "cardNum";

        /// <summary>
        /// Key各门下注情况
        /// </summary>
        public const string KeyBetGolds = "betGolds";

        /// <summary>
        /// Key 下注总值
        /// </summary>
        public const string KeyBetGold = "betGold";

        /// <summary>
        /// 各门下注总值
        /// </summary>
        public const string KeyTotalBet = "bet";

        /// <summary>
        /// 整数默认值（座位号什么的）
        /// </summary>
        public const int KeyDefaultInt = -1;

        /// <summary>
        /// 快速模式
        /// </summary>
        public const int KeyQuickModel=2;

        /// <summary>
        /// 常规交互模式
        /// </summary>
        public const int KeyNormalModel = 1;

        #endregion
    }
}
