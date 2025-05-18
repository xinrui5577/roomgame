using System.Linq;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using Sfs2X.Entities.Data;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine
{
    //游戏状态 与app中的不同 这个是控制游戏一局流程的 App.GameData.GState 是控制整个开房流程的
    public enum EnGameStatus
    {
        GameFree,           //游戏空闲状态 重置游戏所有控件
        GameReady,          //接收到gameinfo 进入准备状态 等待所有人 准备
        GameSendCard,       //发牌阶段 发牌动画 以及塞子的点数
        GamePlay,           //打麻将阶段
        GameEnd,            //一局麻将结束 播放结果 后 返回游戏空闲状态
        None,
    }
    //解散房间类型
    public enum EnDismissFeedBack
    {
        None = 0,
        ApplyFor = 2,
        Apply = 3,
        Refuse = -1,
    }
    //聊天类型
    public enum EnChatType
    {
        exp,
        sorttalk,
        ani,
        text,
    }
    //游戏循环类型 
    public enum EnGameLoopType
    {
        round,      //用局算
        circle,     //用圈算
    }

    //房间类型
    public enum EnRoomType
    {
        FanKa,
        YuLe,
    }

    /// <summary>
    /// 牌值枚举
    /// </summary>
    public enum EnumMahjongValue
    {
        None = 0,
        Wan_1 = 17,
        Wan_2 = 18,
        Wan_3,
        Wan_4,
        Wan_5,
        Wan_6,
        Wan_7,
        Wan_8,
        Wan_9,
        Tiao_1 = 33,
        Tiao_2,
        Tiao_3,
        Tiao_4,
        Tiao_5,
        Tiao_6,
        Tiao_7,
        Tiao_8,
        Tiao_9,
        Bing_1 = 49,
        Bing_2,
        Bing_3,
        Bing_4,
        Bing_5,
        Bing_6,
        Bing_7,
        Bing_8,
        Bing_9,
        Dong = 65,
        Nan = 68,
        Xi = 71,
        Bei = 74,
        Zhong = 81,
        Fa = 84,
        Bai = 87,
        ChunF = 96,
        XiaF = 97,
        QiuF = 98,
        DongF,
        MeiF,
        LanF,
        ZuF,
        JuF,
        AnBao = 10086,
    }
    public class UserData
    {
        public string name;
        public int id;
        public int level;
        public string HeadImage = "";
        public short Sex;
        public int Seat = -1;
        public long Glod;
        public long TotalGlod;
        public long FloatGold;
        public string ip;
        public string Country;

        public bool IsReady = false;
        public bool IsOutLine = false;
        public bool IsNull = true;
        public int WinTimes { get; set; }
        public int LostTimes { get; set; }
        public int TotalTimes { get; set; }
        public int Taopao { get; set; }
        public int Jiama { get; set; }

        public string WinRate
        {
            get
            {
                int total = TotalTimes;
                int m = 0;
                if (total > 0)
                {
                    m = WinTimes * 10000 / total;
                }
                return (m / 100) + "%";
            }
        }

        public string SignText { get; set; }

        public virtual void ParseData(ISFSObject userData)
        {
            name = userData.GetUtfString(RequestKey.KeyName);
            Seat = userData.GetInt(RequestKey.KeySeat);
            IsReady = userData.GetBool(RequestKey.KeyState);
            TotalGlod = userData.GetLong(RequestKey.KeyTotalGold);
            if (userData.ContainsKey(RequestKey.KeyGold))
            {
                FloatGold = userData.GetLong(RequestKey.KeyGold);
            }
            Glod = TotalGlod + FloatGold;

            id = userData.GetInt(RequestKey.KeyId);
            Country = userData.GetUtfString(RequestKeyOther.Country);
            Sex = (short)(userData.GetShort(RequestKeyOther.Sex) % 2);
            WinTimes = userData.GetInt(RequestKeyOther.WinTime);
            LostTimes = userData.GetInt(RequestKeyOther.LostTime);
            TotalTimes = userData.GetInt(RequestKeyOther.TotalTime);
            level = userData.GetInt(RequestKeyOther.Level);
            SignText = userData.ContainsKey(RequestKeyOther.Signature) ? userData.GetUtfString(RequestKeyOther.Signature) : "";
            string avatar = userData.ContainsKey(RequestKeyOther.Avatar) ? userData.GetUtfString(RequestKeyOther.Avatar) : "";
            if (!string.IsNullOrEmpty(avatar))
            {
                HeadImage = avatar;
            }

            ip = userData.GetUtfString(RequestKeyOther.Ip);

            if (userData.ContainsKey(RequestKeyOther.KeyNetWork))
                IsOutLine = !userData.GetBool(RequestKeyOther.KeyNetWork);

            IsNull = false;

            if (userData.ContainsKey("jiaMa")) Jiama = userData.GetInt("jiaMa");
        }
    }

    public class RoomInfo
    {
        public int Maxrate { get { return value[0]; } }     //最大番
        public int Ante { get { return value[1]; } }        //底分
        public int Jue { get { return value[2]; } }         //绝1 杠2
        public int Baosanjia { get { return value[3]; } }   //包三家1
        public int Laizi { get { return value[4]; } }       //赖子1
        public int Qionghu { get { return value[5]; } }     //穷胡
        public int XFGang { get { return value[6]; } }      //旋风杠
        public int Qingyise { get { return value[7]; } }    //清一色
        public int Tuidaohu { get { return value[8]; } }    //推到胡
        public int HutType { get { return value[9]; } }
        public int Type { get { return value[10]; } }
        public int FeiDan { get { return value[13]; } }     //小鸡飞弹
        public int SanFeng { get { return value[14]; } }    //三风蛋
        public int ZanDan { get { return value[15]; } }    //攒蛋
        public int YaoJiuDan { get { return value[16]; } }    //幺九蛋
        public int AnBao { get { return value[17]; } }    //暗宝
        public int NiuBi { get { return value[18]; } }    //牛逼胡
        public int Pph { get { return value[19]; } }  //碰碰胡
        public int DeakType { get { return value[20]; } } //桌子分类     
        public int LuanFeng { get { return value[21]; } } //乱风

        public bool IsExsitLaizi { get { return Laizi == 1; } }
        public bool IsExsitXFGang { get { return XFGang == 1; } }

        public int TouPiaoTimeOut { get { return value[11] == 0 ? HandUpDefineTime : value[11]; } }
        public int JianMainPao { get { return value[12]; } }

        public int OutCardTime { get { return value[22]; } }//出牌时间
        public int HuanTime { get { return value[23]; } }//血战换三张时间
        public int DuanTime { get { return value[24]; } }//血战断门时间
        public int AutoReady { get { return value[25]; } }//是否自动准备

        public bool Huanshen = false;
        public int JySwitchCardCount { get { return value[27]; } }

        public int RoomID;
        [Tooltip("真实唯一ID")]
        public long RoomIdOnly;
        [Tooltip("是否存在圈参数")]
        public bool IsQuanExist;
        [Tooltip("圈数")]
        public int QuanNum;
        private int _currRound;

        public string Cargs = "";
        //开房用的消耗品类型 coin_a 金币 cash_a 元宝 item 房卡
        public string ConsumeType = string.Empty;
        //消耗品数量
        public int ConsumeNum = 0;

        public int CurrRound
        {
            set { _currRound = value; }
            get { return _currRound > MaxRound ? MaxRound : _currRound; }
        }

        public int MaxRound;

        public List<int> SysCards;

        //TODO: 花牌不 *4
        public int SysMahjongCnt
        {
            get
            {
                //return SysCards == null ? 0 : SysCards.Count * 4;
                int value = 0;
                if (SysCards != null)
                {
                    for (int i = 0; i < SysCards.Count; i++)
                    {
                        if (SysCards[i] < 0x60)
                            value += 4;
                        else
                            value++;
                    }
                    return value;
                }
                else
                    return 0;

            }
        }

        public EnGameLoopType GameLoopType = EnGameLoopType.round;

        private int[] value;
        private const int HandUpDefineTime = 300;

        private string _rule = "局数:4圈;封顶:3蕃;玩法:换三张 幺九将对 门清中张 对对胡 天胡地胡 一条龙 自摸加底 夹五星";

        private EnRoomType _roomType;

        private string keys =
            "-maxrate,-ante,-jue,-bsj,-lz,-qionghu,-xuanfeng,-qingyise,-kaimen,-hutype,-type,-tptout,-jmpao,-feidan,-dancnt,-zhandan,-yaojiudan,-anbao,-niubi,-pph,-desktype,-lf,-cdtime,-huantime,-duantime,-autoready,-huanshen,-changeCnt";

        public EnRoomType RoomType
        {
            set
            {
                _roomType = value;
                UtilData.RoomType = value;
            }
            get { return _roomType; }
        }

        public string TeaID; //茶馆ID

        public void ParseData(ISFSObject data)
        {
            if (data.ContainsKey("teaId"))
            {
                TeaID = data.GetUtfString("teaId");
            }
            if (data.ContainsKey(RequestKeyOther.KeyRoomID))
                RoomID = data.GetInt(RequestKeyOther.KeyRoomID);
            if (data.ContainsKey(RequestKeyOther.KeyRoomOnlyId))
            {
                RoomIdOnly = data.GetLong(RequestKeyOther.KeyRoomID);
            }
            else
            {
                RoomIdOnly = RoomID;
            }
            if (data.ContainsKey("maxRound"))
                MaxRound = data.GetInt("maxRound");

            if (data.ContainsKey(RequestKeyOther.KeyQuan))
            {
                CurrRound = data.GetInt(RequestKeyOther.KeyQuan) + 1;
                IsQuanExist = true;
                QuanNum = data.GetInt(RequestKeyOther.KeyQuan) + 1;
            }
            else if (data.ContainsKey(RequestKeyOther.KeyRound))
            {
                if (data.ContainsKey(RequestKeyOther.KeySeq))
                {
                    CurrRound = data.GetInt(RequestKeyOther.KeyRound);
                }
                else
                {
                    CurrRound = data.GetInt(RequestKeyOther.KeyRound) + 1;
                }
            }

            string strCarge = data.GetUtfString(RequestKeyOther.KeyCargs);
            Cargs = strCarge;
            string[] strkey = keys.Split(',');
            value = new int[strkey.Length];
            for (int i = 0; i < strkey.Length; i++)
            {
                string str = strkey[i];
                int findStartIndex = strCarge.IndexOf(str);
                if (findStartIndex < 0)
                    continue;

                int startIndex = findStartIndex + str.Length + 1;
                int endIndex = strCarge.IndexOf(",", startIndex);
                string strVal = strCarge.Substring(startIndex, endIndex - startIndex);
                value[i] = int.Parse(strVal);
            }

            if (data.ContainsKey("pcards"))
            {
                string cardsStr = data.GetUtfString("pcards");
                char[] cards = cardsStr.ToArray();
                SysCards = new List<int>();
                for (int i = 0; i < cards.Length / 2; i++)
                {
                    string card = new string(new[] { cards[i * 2], cards[i * 2 + 1] });
                    int cardValue = Convert.ToInt32(card, 16);
                    SysCards.Add(cardValue);
                }

                SysCards.Sort((a, b) =>
                {
                    if (a > b) return 1;
                    if (a < b) return -1;
                    return 0;
                });
            }

            if (data.ContainsKey("quan"))
                GameLoopType = EnGameLoopType.circle;
            else
                GameLoopType = EnGameLoopType.round;

            if (data.ContainsKey("rule"))
            {
                _rule = data.GetUtfString("rule");
            }

            if (data.ContainsKey("gtype"))
            {
                var type = data.GetInt("gtype");
                if (type >= 0)
                {
                    RoomType = EnRoomType.YuLe;
                }
                else
                {
                    RoomType = EnRoomType.FanKa;
                }
            }

            //金币赔率
            if (data.ContainsKey("showGoldRate"))
            {
                UtilData.ShowGoldRate = data.GetInt("showGoldRate");
            }

            //设置血战麻将出牌时间， 换三张时间， 断门时间
            if (OutCardTime != 0) { GameConfig.OutCardTime = OutCardTime; }
            if (HuanTime != 0) { GameConfig.HuanTime = HuanTime; }
            if (DuanTime != 0) { GameConfig.DuanTime = DuanTime; }

            if (AutoReady == 1)
            {
                UtilData.IsAutoPrepare = true;
            }

            if (data.ContainsKey("cargs2"))
            {
                ISFSObject sfs = data.GetSFSObject("cargs2");
                Huanshen = sfs.ContainsKey("-huanshen");
            }

            if (data.ContainsKey("consumeNum"))
            {
                ConsumeNum = data.GetInt("consumeNum");

            }

            if (data.ContainsKey("consumeType  "))
            {
                ConsumeType = data.GetUtfString("consumeType");
            }
        }

        public string GetRoomRuleString()
        {
            return _rule;
        }

        public Dictionary<string, string> GetRoomRule()
        {
            try
            {
                return TryGetRoomRuleForUi();
            }
            catch (Exception e)
            {
                YxDebug.LogError("可能传入数据为空或格式错误:" + e);
                return new Dictionary<string, string>();
            }
        }
        //消息格式: 局数:10局;玩法:xxx; 
        public Dictionary<string, string> TryGetRoomRuleForUi()
        {
            var dic = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(_rule))
            {
                return dic;
            }
            string[] rules = Regex.Split(_rule, ";");
            for (int i = 0; i < rules.Length; i++)
            {
                string[] kv = Regex.Split(rules[i], ":");
                if (kv.Length > 1)
                {
                    dic[kv[0]] = kv[1];
                }
            }
            return dic;
        }

        public string GetGameLoopString()
        {
            string ret = "";
            if (GameLoopType == EnGameLoopType.circle)
                ret += ""; //ret += "圈数:";
            else if (GameLoopType == EnGameLoopType.round)
                ret += ""; // ret += "局数:";

            ret += "" + CurrRound + "/" + MaxRound;

            return ret;
        }

        public string GetLoopString()
        {
            string ret = "";
            if (GameLoopType == EnGameLoopType.circle)
                ret += "圈";
            else if (GameLoopType == EnGameLoopType.round)
                ret += "局";
            return ret;
        }

        public bool IsEndRound { get { return _currRound > MaxRound; } }
    }

    public abstract class CpgData
    {
        //类型
        public virtual EnGroupType Type
        {
            get { return (EnGroupType)_type; }
        }

        public int Card = UtilDef.NullMj;                                //牌    吃碰杠
        public virtual int MahjongCount
        {
            get { return 0; }
        }

        /// <summary>
        /// 来源座位号
        /// </summary>
        public int FromSeat
        {
            get { return _fromSeat; }
            set { _fromSeat = value; }
        }

        /// <summary>
        /// 吃碰刚玩家的位置
        /// </summary>
        public int Chair;

        protected int _type;
        protected List<int> CardDatas = new List<int>();   //牌组
        protected List<int> AllCardDatas = new List<int>();
        protected int _fromSeat;
        protected int _seat;
        protected bool _dan;

        //cpg中有赖子牌，标记Icon
        public int Laizi;
        //cpg中有laizi1
        public int Laizi1;

        public int Fanpai;

        public bool Dan
        {
            get { return _dan; }
        }

        public int Seat
        {
            set { _seat = value; }
            get { return _seat; }
        }

        public virtual int AcrossIndex
        {
            get { return UtilFunc.GetChairId(_fromSeat, _seat); }
        }

        public virtual List<int> GetHardCards()
        {
            return CardDatas;
        }

        public virtual int GetOutPutCard()
        {
            return Card;
        }

        public virtual List<int> AllCards()
        {
            return AllCardDatas;
        }

        public abstract void SetAllCardDatas();

        public virtual void SetCardDatas()
        {
        }

        public virtual void SetXfdAllCards(List<int> xfdCards)
        {
        }

        public virtual void SetCard()
        {
            if (Card == UtilDef.DefInt) Card = CardDatas[0];
        }


        public virtual void ParseData(ISFSObject data)
        {
            if (data.ContainsKey(RequestKeyOther.KeyTType))
                _type = data.GetInt(RequestKeyOther.KeyTType);
            else if (data.ContainsKey(RequestKey.KeyType))
                _type = data.GetInt(RequestKey.KeyType);

            if (data.ContainsKey(RequestKey.KeyOpCard))
                Card = data.GetInt(RequestKey.KeyOpCard);
            else if (data.ContainsKey(RequestKey.KeyCard))
                Card = data.GetInt(RequestKey.KeyCard);

            if (data.ContainsKey("from"))
                _fromSeat = data.GetInt("from");

            if (data.ContainsKey("dan"))
            {
                _dan = true;
            }

            if (data.ContainsKey(RequestKey.KeySeat))
            {
                _seat = data.GetInt(RequestKey.KeySeat);
            }

            if (data.ContainsKey(RequestKey.KeyCards))
            {
                int[] values = data.GetIntArray(RequestKey.KeyCards);
                foreach (int value in values)
                {
                    CardDatas.Add(value);
                }
                UtilFunc.OutPutList(CardDatas, "吃碰杠 " + _type);
            }

            SetCard();
            SetCardDatas();
            SetAllCardDatas();
        }
    }

    public class CpgChi : CpgData
    {
        public override int MahjongCount
        {
            get { return 3; }
        }

        public override void SetAllCardDatas()
        {
            AllCardDatas = new List<int>(CardDatas);
            AllCardDatas.Add(Card);
            AllCardDatas.Sort((a, b) =>
            {
                if (a > b) return 1;
                if (a < b) return -1;
                return 0;
            });
        }
    }

    public class CpgPeng : CpgData
    {
        public override int MahjongCount
        {
            get { return 3; }
        }
        public override void SetAllCardDatas()
        {
            AllCardDatas = new List<int>(CardDatas);
            AllCardDatas.Add(Card);
        }

        public override void SetCardDatas()
        {
            while (MahjongCount - 1 != CardDatas.Count)
            {
                CardDatas.Add(Card);
            }
        }
    }

    public class CpgSelfGang : CpgData
    {
        public override int MahjongCount
        {
            get { return 4; }
        }

        public override void SetAllCardDatas()
        {
            AllCardDatas = new List<int>(CardDatas);
            while (MahjongCount != CardDatas.Count)
            {
                CardDatas.Add(Card);
            }
        }

        public override void SetCardDatas()
        {
            while (MahjongCount != CardDatas.Count)
            {
                CardDatas.Add(Card);
            }
        }

        public override int GetOutPutCard()
        {
            //自己杠是自己手牌中的 不是别人打出来的
            return UtilDef.NullMj;
        }

    }

    public class CpgSelfGangBao : CpgSelfGang
    {
        public override int MahjongCount
        {
            get { return 4; }
        }

        public override void SetAllCardDatas()
        {
            AllCardDatas = new List<int>(CardDatas);
            while (MahjongCount != CardDatas.Count)
            {
                CardDatas.Add(Card);
            }
        }

        public override void SetCardDatas()
        {
            while (MahjongCount != CardDatas.Count)
            {
                CardDatas.Add(Card);
            }
        }

        public override int GetOutPutCard()
        {
            //自己杠是自己手牌中的 不是别人打出来的
            return UtilDef.NullMj;
        }

    }

    public class CpgZhuaGang : CpgData
    {
        public override int MahjongCount
        {
            get { return 4; }
        }

        public bool Ok;                                 //抓杠独有的  抢杠胡的时候处理

        public override void SetAllCardDatas()
        {
            AllCardDatas.Add(Card);
            AllCardDatas.Add(Card);
            AllCardDatas.Add(Card);
            AllCardDatas.Add(Card);
        }

        public override void SetCardDatas()
        {
            CardDatas.Clear();
            CardDatas.Add(Card);
        }

        public override int GetOutPutCard()
        {
            //抓杠是自己手牌中的 不是别人打出来的
            return UtilDef.NullMj;
        }

        public override void ParseData(ISFSObject data)
        {
            base.ParseData(data);
            if (data.ContainsKey("ok"))
            {
                Ok = data.GetBool("ok");
            }
        }
    }

    public class CpgXfdGang : CpgZhuaGang
    {

        public override int MahjongCount
        {
            get { return CardDatas.Count; }
        }
        public override void SetAllCardDatas()
        {
            for (int i = 0; i < CardDatas.Count; i++)
            {
                AllCardDatas.Add(CardDatas[i]);
            }
        }

        public override void SetXfdAllCards(List<int> xfdCards)
        {
            foreach (int xfdCard in xfdCards)
            {
                AllCardDatas.Add(xfdCard);
            }
        }

        public override void SetCardDatas()
        {

        }

        public override void SetCard()
        {

        }

    }


    public class CpgOtherGang : CpgData
    {
        public override int MahjongCount
        {
            get { return 4; }
        }

        public override void SetAllCardDatas()
        {
            AllCardDatas = new List<int>(CardDatas);
            AllCardDatas.Add(Card);
        }

        public override void SetCardDatas()
        {
            while (MahjongCount - 1 != CardDatas.Count)
            {
                CardDatas.Add(Card);
            }
        }
    }

    public class CpgXFGang : CpgData
    {
        public override int MahjongCount
        {
            get { return CardDatas.Count; }
        }

        public override EnGroupType Type
        {
            get { return EnGroupType.XFGang; }
        }

        public override void SetAllCardDatas()
        {
            AllCardDatas = new List<int>(CardDatas);
        }

        public override void SetCard()
        {
            //旋风杠无Card
        }

        public override int GetOutPutCard()
        {
            //旋风杠 不是杠别人的
            return UtilDef.NullMj;
        }
    }

    public class CpgAnJuegang : CpgData
    {
        public override int MahjongCount
        {
            get { return 3; }
        }
        public override void SetAllCardDatas()
        {
            AllCardDatas = new List<int>(CardDatas);
            while (MahjongCount != CardDatas.Count)
            {
                CardDatas.Add(Card);
            }
        }
        public override void SetCardDatas()
        {
            while (MahjongCount != CardDatas.Count)
            {
                CardDatas.Add(Card);
            }
        }
        public override int GetOutPutCard()
        {
            //自己杠是自己手牌中的 不是别人打出来的
            return UtilDef.NullMj;
        }
    }

    public class GameResult
    {
        public List<int> HuSeat = new List<int>();
        public string[] HuName = new string[UtilData.CurrGamePalyerCnt];
        public int[] Gold = new int[UtilData.CurrGamePalyerCnt];
        public int[] HuGold = new int[UtilData.CurrGamePalyerCnt];
        public int[] ShareShowGold = new int[UtilData.CurrGamePalyerCnt];
        public int[] GangGlod = new int[UtilData.CurrGamePalyerCnt];
        public int[] PiaoGlod = new int[UtilData.CurrGamePalyerCnt];
        public long[] TotalGold = new long[UtilData.CurrGamePalyerCnt];
        public int[] UserHuType = new int[UtilData.CurrGamePalyerCnt];
        public int HuCard;
        public int[] HuCards = new int[UtilData.CurrGamePalyerCnt];				// 胡牌不结算的玩法每家手里都可能有一张引胡的牌
        public int HuType;//胡牌类型
        //郑谭 码分，
        public int[] NiaoGold = new int[UtilData.CurrGamePalyerCnt];
        public int[] ZhaNiao;
        public int[] Zhongma;

        /// <summary>
        /// 连庄信息
        /// </summary>
        public string[] LianZhuangInfo = new string[UtilData.CurrGamePalyerCnt];

        /// <summary>
        /// 连庄信息
        /// </summary>
        public int[] LianZhuangNumInfo = new int[UtilData.CurrGamePalyerCnt];

        /// <summary>
        /// 标记是不是包牌
        /// </summary>
        public bool[] IsBaoPai = new bool[UtilData.CurrGamePalyerCnt];

        /// <summary>
        /// 精品麻将 花信息
        /// </summary>
        public string[] Huaname = new string[UtilData.CurrGamePalyerCnt];
        /// <summary>
        /// 精品麻将 一共多少花
        /// </summary>
        public int[] Huacnt = new int[UtilData.CurrGamePalyerCnt];
        /// <summary>
        /// 精品麻将 一共多少台
        /// </summary>
        public int[] Taicnt = new int[UtilData.CurrGamePalyerCnt];
        /// <summary>
        /// 精品麻将的补张数组
        /// </summary>
        public List<int[]> BuZhangList = new List<int[]>();

        //友玩麻将 精分        
        public int[] JingGlods = new int[UtilData.CurrGamePalyerCnt];

        //中至乐平游湖 宝分
        public int[] Baos = new int[UtilData.CurrGamePalyerCnt];

        //宝值
        public int bao;
        //冲宝标记
        public bool ChBao;
        //摸宝标记
        public bool MoBao;
        //飘胡标记
        public bool PiaoHu;
        //胡牌类型
        public int[] CType = new int[UtilData.CurrGamePalyerCnt];

        //胡牌类型2
        public int[] CType2 = new int[UtilData.CurrGamePalyerCnt];

        //泉州麻将胡类型
        public QzmjHuType QzmjHuType = QzmjHuType.No;

        public int[] ZhengjingArray = new int[UtilData.CurrGamePalyerCnt];
        public int[] FujingArray = new int[UtilData.CurrGamePalyerCnt];

        //public int CurrRound = 1;
        public void ParseData(ISFSObject data)
        {
            BuZhangList.Clear();
            ISFSArray players = data.GetSFSArray(RequestKey.KeyPlayerList);
            HuType = data.GetInt(RequestKey.KeyType);
            for (int i = 0; i < players.Count; i++)
            {
                ISFSObject player = players.GetSFSObject(i);
                if (player.ContainsKey(RequestKey.KeyType))
                {
                    int type = player.GetInt(RequestKey.KeyType);
                    if (type >= 1)
                    {
                        HuSeat.Add(i);
                    }

                    UserHuType[i] = type;
                }

                if (player.ContainsKey("hname"))
                {
                    HuName[i] = player.GetUtfString("hname");
                }

                if (player.ContainsKey("ctype"))
                {
                    CType[i] = player.GetInt("ctype");
                }

                if (player.ContainsKey("ctype2"))
                {
                    CType2[i] = player.GetInt("ctype2");
                }

                Gold[i] = player.GetInt(RequestKey.KeyGold);

                var huaScore = 0;
                if (player.ContainsKey(RequestKeyOther.KeyGHua))
                {
                    huaScore = player.GetInt(RequestKeyOther.KeyGHua);
                }
                var lianScore = 0;
                if (player.ContainsKey(RequestKeyOther.KeyGLian))
                {
                    lianScore = player.GetInt(RequestKeyOther.KeyGLian);
                }
                var taiScore = 0;
                if (player.ContainsKey(RequestKeyOther.KeyGTai))
                {
                    taiScore = player.GetInt(RequestKeyOther.KeyGTai);
                }
                HuGold[i] = player.GetInt(RequestKeyOther.KeyGHu) + huaScore;
                ShareShowGold[i] = huaScore + taiScore + lianScore;
                GangGlod[i] = player.GetInt(RequestKeyOther.KeyGGang);
                PiaoGlod[i] = player.GetInt(RequestKeyOther.KeyGPiao);
                //郑谭 得到码分
                NiaoGold[i] = player.GetInt(RequestKeyOther.KeyNiao);
                TotalGold[i] = player.GetLong("ttgold");

                //标记是不是包牌
                if (player.ContainsKey("baopai"))
                {
                    IsBaoPai[i] = player.GetInt("baopai") == 1;
                }
                else
                {
                    IsBaoPai[i] = false;
                }

                //标记是不是连庄
                if (player.ContainsKey("lianzhuang")) LianZhuangInfo[i] = player.GetUtfString("lianzhuang");

                if (player.ContainsKey("lianzhuangNum")) LianZhuangNumInfo[i] = player.GetInt("lianzhuangNum");

                if (player.ContainsKey("huaname")) Huaname[i] = player.GetUtfString("huaname");

                if (player.ContainsKey("huacnt")) Huacnt[i] = player.GetInt("huacnt");

                if (player.ContainsKey("taicnt")) Taicnt[i] = player.GetInt("taicnt");

                //zzyhmj 宝分
                if (player.ContainsKey("bao")) Baos[i] = player.GetInt("bao");

                //友玩麻将 精分                
                if (player.ContainsKey("jingglod")) JingGlods[i] = player.GetInt("jingglod");

                if (player.ContainsKey("buHuaList")) BuZhangList.Add(player.GetIntArray("buHuaList"));
                if (player.ContainsKey("hucard"))
                    HuCards[i] = player.GetInt("hucard");
                else
                    HuCards[i] = 0;

                if (player.ContainsKey("zjing")) ZhengjingArray[i] = player.GetInt("zjing");
                if (player.ContainsKey("fjing")) FujingArray[i] = player.GetInt("fjing");
            }

            HuCard = data.GetInt("huCard");

            if (data.ContainsKey(RequestKeyOther.KeyZhaNiao))
            {
                ZhaNiao = data.GetIntArray(RequestKeyOther.KeyZhaNiao);
            }

            if (data.ContainsKey(RequestKeyOther.KeyZhongma))
            {
                Zhongma = data.GetIntArray(RequestKeyOther.KeyZhongma);
            }
            if (data.ContainsKey(RequestKeyOther.KeyChBao))
            {
                ChBao = true;
            }
            if (data.ContainsKey(RequestKeyOther.KeyBao))
            {
                bao = data.GetInt(RequestKeyOther.KeyBao);
            }
            if (data.ContainsKey(RequestKeyOther.KeyMoBao))
            {
                MoBao = true;
            }
            if (data.ContainsKey(RequestKeyOther.KeyPiaoHu))
            {
                PiaoHu = true;
            }
        }
    }

    public class TotalResult
    {
        public int[] Seat = new int[UtilData.CurrGamePalyerCnt];
        public int[] Glod = new int[UtilData.CurrGamePalyerCnt];
        public int[] Pao = new int[UtilData.CurrGamePalyerCnt];
        public int[] Gang = new int[UtilData.CurrGamePalyerCnt];
        public int[] AnGang = new int[UtilData.CurrGamePalyerCnt];
        public int[] Hu = new int[UtilData.CurrGamePalyerCnt];
        public int[] Zimo = new int[UtilData.CurrGamePalyerCnt];
        public int[] Id = new int[UtilData.CurrGamePalyerCnt];
        public string[] Name = new string[UtilData.CurrGamePalyerCnt];
        public int[] MoBao = new int[UtilData.CurrGamePalyerCnt];
        public int[] ChBao = new int[UtilData.CurrGamePalyerCnt];
        public int[] Gangkais = new int[UtilData.CurrGamePalyerCnt];
        public int[] ZhaNiao = new int[UtilData.CurrGamePalyerCnt];

        public int BeatPaoSeat;
        public int BigWinnerSeat;
        public int RoomID;
        public int CurrRound;
        public int MaxRound;

        public void ParseData(ISFSObject data)
        {
            if (data == null)
                return;

            if (data.ContainsKey("rid"))
            {
                RoomID = data.GetInt("rid");
            }
            if (data.ContainsKey("round"))
            {
                CurrRound = data.GetInt("round");
            }
            if (data.ContainsKey("maxRound"))
            {
                CurrRound = data.GetInt("maxRound");
            }

            ISFSArray userDatas = null;
            if (data.ContainsKey("users"))
            {
                userDatas = data.GetSFSArray("users");
            }

            if (userDatas == null) return;
            for (int i = 0; i < userDatas.Size(); i++)
            {
                ISFSObject obj = userDatas.GetSFSObject(i);
                Seat[i] = obj.GetInt("seat");
                Glod[i] = obj.GetInt("gold");
                Pao[i] = obj.GetInt("pao");
                Gang[i] = obj.GetInt("gang");
                AnGang[i] = obj.GetInt("anGang");
                Hu[i] = obj.GetInt("hu");
                Zimo[i] = obj.GetInt("zimo");
                Id[i] = obj.GetInt("id");
                Name[i] = obj.GetUtfString("nick");
                MoBao[i] = obj.GetInt("mobao");
                ChBao[i] = obj.GetInt("chbao");
                Gangkais[i] = obj.GetInt("gangkai");
                if (obj.ContainsKey("niao"))
                {
                    ZhaNiao[i] = obj.GetInt("niao");
                }
            }

            int maxGold = Glod[0];
            int maxPao = Pao[0];
            BigWinnerSeat = -1;
            BeatPaoSeat = -1;
            for (int i = 0; i < Glod.Length; i++)
            {
                if (Glod[i] > maxGold)
                {
                    BigWinnerSeat = i;
                    maxGold = Glod[i];
                }

                if (Pao[i] > maxPao)
                {
                    BeatPaoSeat = i;
                    maxPao = Pao[i];
                }
            }

        }
    }

    public class OnSendCardEventData
    {
        public int laizi;
        public int fanpai;
        public int[] playermj;
        public int currChair;
        public DVoidInt reduceMjCnt;

        //骰子数
        public int[] SaiziPoint;
        //另一张赖子
        public int laizi1;
    }

    public class FindGangData
    {
        public int type;
        public int ttype;
        public int[] cards;
    }

    //泉州麻将胡类型
    public enum QzmjHuType
    {
        No,
        youjin,
        shuangyou,
        sanyou,
        sanjindao
    }

    public class QueryHulistData
    {
        //听， 游金
        public int Flag;
        public int CardValue;
        public int CardIndex;
        public int Laizi;

        //剩余的任意牌
        public int LeaveMahjongCnt;
        //可胡的牌
        public List<int> Cards;
        //可胡的牌剩余数量
        public List<int> CardsNum;

        public QueryHulistData(int flag, int cardValue, int cardType, int laizi)
        {
            Flag = flag;
            CardValue = cardValue;
            CardIndex = cardType; ;
            Laizi = laizi;
        }
    }

    //加码
    public enum OnJiamaByType
    {
        Pao = 1 << 1,
        Queyimen = 1 << 2,
        Jia = 1 << 3,
        Finish = 1 << 4,
        End = 1 << 5,
    }
}