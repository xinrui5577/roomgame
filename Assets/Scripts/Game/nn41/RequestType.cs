using UnityEngine;

namespace Assets.Scripts.Game.nn41
{
    public class RequestType : MonoBehaviour
    {
        public const int Bet = 1;  //下注
        public const int ApplyBanker = 2;
        public const int ApplyQuit = 3;
        public const int BankerList = 4;
        public const int BeginBet = 5;
        public const int EndBet = 6;
        public const int GiveCards = 7;
        public const int Result = 8;
        public const int RobBank = 20;//抢庄
        public const int BeginGame = 16;//开始游戏
    }

    public class InteractParameter
    {
        /// <summary>
        /// gameInfo的最外层
        /// </summary>
        public const string Cargs2 = "cargs2";
        /// <summary>
        /// 总结算
        /// </summary>
        public const string GameOver = "over";
        /// <summary>
        /// 声音
        /// </summary>
        public const string Sound = "sound";
        /// <summary>
        /// 解散房间的时间
        /// </summary>
        public const string HupTime = "-tptout";
        /// <summary>
        /// 上庄的限制（显示的时候需要*10000）
        /// </summary>
        public const string Banklimit="-banklimit";
        /// <summary>
        /// 历史纪录
        /// </summary>
        public const string Record = "record";
        /// <summary>
        /// 抢庄的倍率
        /// </summary>
        public const string Rate = "rate";
        /// <summary>
        /// 金币数
        /// </summary>
        public const string Gold = "gold";
        /// <summary>
        /// 发送聊天的Key
        /// </summary>
        public const string Talk = "talk";
        /// <summary>
        /// 发送的文字信息
        /// </summary>
        public const string Text = "text";
        /// <summary>
        /// 聊天的发起人的名字
        /// </summary>
        public const string FromU = "fromu";
        /// <summary>
        /// 发送的表情
        /// </summary>
        public const string Exp = "exp";
        /// <summary>
        /// 玩家的座位号
        /// </summary>
        public const string Seat = "seat";
        /// <summary>
        /// 其他玩家的座位号
        /// </summary>
        public const string OtherSeat = "otherSeat";
        /// <summary>
        /// 发送服务器的交互类型
        /// </summary>
        public const string Type = "type";
        /// <summary>
        /// 动画的索引
        /// </summary>
        public const string AniIndex = "ani";
        /// <summary>
        /// 解散房间时发送
        /// </summary>
        public const string Hup = "hup";
        /// <summary>
        /// 命令
        /// </summary>
        public const string Cmd = "cmd";
        /// <summary>
        /// 游戏未开始房主直接解散
        /// </summary>
        public const string Dissolve = "dissolve";
        /// <summary>
        /// 解散游戏内
        /// </summary>
        public const string Dismiss = "dismiss";
        /// <summary>
        /// 玩家的名字
        /// </summary>
        public const string UserName = "username";
        /// <summary>
        /// 解散界面的时间
        /// </summary>
        public const string CdTime = "cdTime";
        /// <summary>
        /// 系统时间
        /// </summary>
        public const string Svt = "svt";
        /// <summary>
        /// 解散开始时间
        /// </summary>
        public const string HupStart = "hupstart";
        /// <summary>
        /// 玩家的名字
        /// </summary>
        public const string Nick = "nick";
        /// <summary>
        /// 牌组
        /// </summary>
        public const string Cards = "cards";
        /// <summary>
        /// 是否是庄
        /// </summary>
        public const string IsZ = "isZ";
        public const string IsBanker = "isBank";
        /// <summary>
        /// 庄的数据
        /// </summary>
        public const string ZhuangData = "zhuangData";
        /// <summary>
        /// 牛的数据
        /// </summary>
        public const string NiuData = "niuData";
        /// <summary>
        /// 是否赢
        /// </summary>
        public const string IsWin = "win";
        /// <summary>
        /// 是牛几
        /// </summary>
        public const string Niu = "niu";
        /// <summary>
        /// 抢庄倍率
        /// </summary>
        public const string RobRate = "robRate";
        /// <summary>
        /// 庄位
        /// </summary>
        public const string BanktypeStr = "banktypeStr";
        /// <summary>
        /// 创建房间的游戏规则
        /// </summary>
        public const string Rule = "rule";
        /// <summary>
        /// 发送此消息 可以再次请求gameInfo
        /// </summary>
        public const string Crjin = "crjin";
        /// <summary>
        /// 是否搓牌
        /// </summary>
        public const string IsCuoPai = "isCuoPai";
        /// <summary>
        /// 重连回来的状态值
        /// </summary>
        public const string State = "state";
        /// <summary>
        /// 庄家的座位号
        /// </summary>
        public const string Bkp = "bkp";
        /// <summary>
        /// 房间号
        /// </summary>
        public const string Rid = "rid";
        /// <summary>
        /// 当前的局数
        /// </summary>
        public const string Round = "round";
        /// <summary>
        /// 最大局数
        /// </summary>
        public const string MaxRound = "maxRound";
        /// <summary>
        /// 定庄时候的显示框的数组
        /// </summary>
        public const string Banks = "banks";
        /// <summary>
        /// antes的数组参数
        /// </summary>
        public const string Antes = "antes";
        //需要的时间
        public const string Cd = "cd";
        /// <summary>
        /// nn41发最后一张牌的时候的数组
        /// </summary>
        public const string NewCards = "newCards";
        /// <summary>
        /// 当前的金币总数
        /// </summary>
        public const string Ttgold = "ttgold";
        /// <summary>
        /// 是否隐藏游戏规则按钮的字段
        /// </summary>
        public const string Hrule = "-hrule";
        /// <summary>
        /// nn41模式 最后一张牌是否直接是翻完状态
        /// </summary>
        public const string EndCardFlop = "-ecflop";
    }
    public class GameSounds
    {
        /// <summary>
        /// 背景音乐的声音
        /// </summary>
        public const string BgMusic = "mjbg";
        /// <summary>
        /// 切换房间的声音
        /// </summary>
        public const string ChangeRoom = "exit";
        /// <summary>
        /// 闹钟的声音
        /// </summary>
        public const string Clock = "Clock";
        /// <summary>
        /// 金币飞行的声音
        /// </summary>
        public const string Coin = "coin";
        /// <summary>
        /// 发牌的声音
        /// </summary>
        public const string Card = "Card";
        /// <summary>
        /// 玩家下注的声音
        /// </summary>
        public const string PlayerBet = "player_add";
        /// <summary>
        /// 庄的声音
        /// </summary>
        public const string GetBanker = "m_get_banker";
        /// <summary>
        /// 准备的声音
        /// </summary>
        public const string Ready = "m_ready";
        /// <summary>
        /// 选庄的时候闪动的声音
        /// </summary>
        public const string SetBanker = "buttonpressed";
        /// <summary>
        /// 花的音效
        /// </summary>
        public const string Flower = "hua";
        /// <summary>
        /// 扔西红柿的音效
        /// </summary>
        public const string Tomato = "tomato";
        /// <summary>
        /// 爆炸的音效
        /// </summary>
        public const string Bomb = "bomb";
        /// <summary>
        /// 水桶倒水的音效
        /// </summary>
        public const string Bucket = "bucket";
        /// <summary>
        /// 鸡的音效
        /// </summary>
        public const string Chicken = "chicken";
        /// <summary>
        /// 酒杯的音效
        /// </summary>
        public const string Cup = "cup";
        /// <summary>
        /// 牌局开始音效
        /// </summary>
        public const string GameStart = "gamestart";
    }
    public enum GameInteract
    {
        /// <summary>
        /// 打赏
        /// </summary>
        GiveReward = 9,
        /// <summary>
        /// 抢夺庄家
        /// </summary>
        RobBanker = 10,
        /// <summary>
        /// 开始下注
        /// </summary>
        StartBet = 11,
        /// <summary>
        /// 发牌
        /// </summary>
        SendCard = 12,
        /// <summary>
        /// 亮出手牌
        /// </summary>
        ShowHandCard = 13,
        /// <summary>
        /// 牛牛41发最后一张牌
        /// </summary>
        SendLastCard = 14,
        /// <summary>
        /// 开放模式控制开始按钮
        /// </summary>
        ContrlStartBtn = 16,
        /// <summary>
        /// 定抢庄注
        /// </summary>
        SureRobDouble = 20,
        /// <summary>
        /// 下注阶段
        /// </summary>
        BetState = 21,
        /// <summary>
        /// 结算阶段
        /// </summary>
        ResultState = 30,
    }
    public class NiuNum
    {
        public const string NiuZero = "无牛";
        public const string NiuOne = "牛一";
        public const string NiuTwo = "牛二";
        public const string NiuThree = "牛三";
        public const string NiuFour = "牛四";
        public const string NiuFive = "牛五";
        public const string NiuSix = "牛六";
        public const string NiuSeven = "牛七";
        public const string NiuEight = "牛八";
        public const string NiuNine = "牛九";
        public const string NiuNiu = "牛牛";
        public const string FourHuaNiu = "四花牛";
        public const string FiveHuaNiu = "五花牛";
        public const string BombNiu = "炸弹牛";
        public const string FiveXiaoNiu = "小五牛";
    }
    public enum GameNnStates
    {
        /// <summary>
        /// 创建自己的房间阶段
        /// </summary>
        Init,
        /// <summary>
        /// 等待阶段
        /// </summary>
        Waiting,
        /// <summary>
        /// 抢庄阶段
        /// </summary>
        RobBank,
        /// <summary>
        /// 下注阶段
        /// </summary>
        Ante,
        /// <summary>
        /// 开牌阶段
        /// </summary>
        ViewCard,
        /// <summary>
        /// 游戏结束阶段
        /// </summary>
        GameOver
    }
     public enum GameNn41States
     {
         /// <summary>
         /// 创建自己的房间阶段
         /// </summary>
         Init=0,
         /// <summary>
         /// 准备阶段
         /// </summary>
         Ready=1,
         /// <summary>
         /// 发四张牌阶段
         /// </summary>
         SendFourCard=2,
         /// <summary>
         /// 确定庄家阶段
         /// </summary>
         SureBanker=3,
         /// <summary>
         /// 下注中阶段
         /// </summary>
         Beting=4,
         /// <summary>
         /// 看牌阶段
         /// </summary>
         LookCard=5,
         /// <summary>
         /// 计算结果阶段
         /// </summary>
         Result=8,
     }
}
