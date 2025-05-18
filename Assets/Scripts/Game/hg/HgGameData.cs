using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Game.hg
{
    public class HgGameData : YxGameData
    {
        public EventObject EventObj;
        public bool BeginBet;
        public int GameRecordNum;
        public int PlayerRecordNum;
        public float UnitTime = 0.1f;
        public long AllUserWinGolds;
        public HgPlayer BankPlayer;
        public List<List<string>> RecordSpot = new List<List<string>>();
        public List<int> RecordCardType = new List<int>();
        public List<int> LuckRate = new List<int>();
        public List<int> GoldRank = new List<int>();//排行中玩家的座位号
        public List<HgUserInfo> AllUserInfos = new List<HgUserInfo>();
        /// <summary>
        /// 是否允许进行游戏
        /// </summary>
        public bool AllowPlay
        {
            get { return GetPlayerInfo(0).CoinA >= _minPlayCoin; }
        }
        /// <summary>
        /// 是否能申请上庄
        /// </summary>
        public bool ApplyBank
        {
            get
            {
                if (BankPlayer != null && BankPlayer.Info != null && BankPlayer.Info.CoinA != long.MaxValue)
                {
                    return GetPlayerInfo().CoinA > BankPlayer.Info.CoinA;
                }
                else
                {
                    return GetPlayerInfo().CoinA > _bankLimit;
                }
            }
        }
        /// <summary>
        /// 当前玩家是不是庄
        /// </summary>
        public bool IsBanker
        {
            get
            {
                return BankPlayer != null && BankPlayer.Info != null && BankPlayer.Info.Seat == GetPlayerInfo().Seat;
            }
        }

        private int _bankLimit;
        private int _minPlayCoin;


        public void OnRecive(EventData data)
        {
            switch (data.Name)
            {
                case "FreshBankInfo":
                    FreshBankInfo((BankListData)data.Data);
                    break;
            }
        }

        public override void InitCfg(ISFSObject cargs2)
        {
            base.InitCfg(cargs2);
            if (cargs2.ContainsKey("-goldrmd"))
            {
                _minPlayCoin = int.Parse(cargs2.GetUtfString("-goldrmd"));
            }
        }

        protected override void InitGameData(ISFSObject gameInfo)
        {
            base.InitGameData(gameInfo);
            GameRecordNum = gameInfo.ContainsKey("gRecordNum") ? gameInfo.GetInt("gRecordNum") : -1;

            PlayerRecordNum = gameInfo.ContainsKey("playerRecordNum") ? gameInfo.GetInt("playerRecordNum") : -1;

            var rankData = new RankData();
            rankData.SetRank(gameInfo);
            GoldRank = rankData.GoldRank;
            YxDebug.LogArray(GoldRank);
            var luckRate = gameInfo.ContainsKey("luckRate") ? gameInfo.GetIntArray("luckRate") : null;
            if (luckRate != null) LuckRate = new List<int>(luckRate);
            var record = gameInfo.ContainsKey("record") ? gameInfo.GetSFSArray("record") : null;
            if (record != null)
            {
                for (int i = 0; i < record.Count; i++)
                {
                    if (record.GetSFSObject(i) == null) continue;
                    var recordValue = new RecordValue(record.GetSFSObject(i));
                    RecordSpot.Add(recordValue.Area);
                    RecordCardType.Add(recordValue.CType);
                }
            }

            var user = gameInfo.ContainsKey("user") ? gameInfo.GetSFSObject("user") : null;
            if (user != null)
            {
                var userInfo = new HgUserInfo();
                userInfo.Parse(user);
                GetPlayer<HgPlayer>().Info = userInfo;
                AllUserInfos.Add(userInfo);
            }

            var users = gameInfo.ContainsKey("users") ? gameInfo.GetSFSArray("users") : null;
            if (users != null)
            {
                for (int i = 0; i < users.Count; i++)
                {
                    var userInfo = new HgUserInfo();
                    userInfo.Parse(users.GetSFSObject(i));
                    AllUserInfos.Add(userInfo);
                }
            }

            _bankLimit = gameInfo.ContainsKey("bankLimit") ? gameInfo.GetInt("bankLimit") : -1;
            var bankListData = new BankListData(gameInfo);
            FreshBankInfo(bankListData);
            EventObj.SendEvent("BankListEvent", "FreshBankList", bankListData);
        }

        private void FreshBankInfo(BankListData bankListData)
        {
            var banker = bankListData.Banker;
            if (banker == -1)
            {
                var bankerInfo = new HgUserInfo
                {
                    NickM = "系统庄",
                    Seat = -1,
                    CoinA = long.MaxValue,
                    TotalCount = 0,
                    WinTotalCoin = 0
                };
                if (BankPlayer != null)
                {
                    BankPlayer.Info = bankerInfo;
                }
            }
            else
            {
                foreach (var allUserInfo in bankListData.BankList)
                {
                    if (banker == allUserInfo.Seat)
                    {
                        var bankerInfo = new HgUserInfo
                        {
                            NickM = allUserInfo.UserName,
                            Seat = allUserInfo.Seat,
                            CoinA = allUserInfo.TtGold,
                        };
                        if (BankPlayer != null)
                        {
                            BankPlayer.Info = bankerInfo;
                            return;
                        }
                    }
                }
            }
        }
    }

    public enum GameState
    {
        Waiting,
        Start,
        ZhuangGold,
        RollDice,
        XiaZhu,
        Over
    }

    public enum GameResponseState
    {
        ShangZhuang = 101,//上庄请求
        XiaZhuang = 102,//下庄请求
        ZhuangChange = 103,//发送庄家列表
        ZhuangGold = 104,//上庄带钱
        BeginBet = 105,
        StopBet = 106,
        Bet = 107,//下注交互 以此发请求
        RollResult = 108,
        GameResult = 109,
        FlushBet = 110, //流式下注
        StartToStop = 111,//开始到停止下注的间隔
        ClearRecord = 112 //清除走势记录
    }

    public class RecordValue
    {
        public List<string> Area=new List<string>();
        public int CType;

        public RecordValue(ISFSObject data)
        {
            if (data.ContainsKey("area"))
            {
                Area.Add(data.GetInt("area").ToString());
            }
            else if (data.ContainsKey("winStr"))
            {
                var winStr = data.GetUtfString("winStr");

                var winStrs = winStr.Split(',');

                for (int i = 0; i < winStrs.Length; i++)
                {
                    if (i == 0&& int.Parse(winStrs[i]) != -1)
                    {
                        Area.Add("s");
                    }
                    else if (i ==1 && int.Parse(winStrs[i]) != -1)
                    {
                        Area.Add("t");
                    }
                    else if (i == 2 && int.Parse(winStrs[i]) != -1)
                    {
                        Area.Add("d");
                    }
                    else
                    {
                        Area.Add("");
                    }
                }
            }
            else if (data.ContainsKey("win"))
            {
                var area = data.GetUtfString("win");
                Area.AddRange(area.Split(','));
            }

            CType = data.ContainsKey("ctype") ? data.GetInt("ctype") : -1;
        }
    }


    public class CardValue
    {
        public int Win;
        //资源设置固定张数的牌 红黑 龙虎斗
        public List<int> BlackCards = new List<int>();
        public int BlackCardType;
        public List<int> RedCards = new List<int>();
        public int RedCardType;

        //生成牌用到 百人牛牛
        public List<int[]> Cards=new List<int[]>();//最后的牌的数据是庄家的
        public List<NnData> NnDatas=new List<NnData>();

        //白人推筒子用到
        public List<CardsData> CardsDatas=new List<CardsData>();

        public CardValue(ISFSObject data)
        {
            ISFSObject black;
            if (data.ContainsKey("black"))
            {
                black = data.GetSFSObject("black");

                var cards = black.ContainsKey("cards") ? black.GetIntArray("cards") : null;
                if (cards != null)
                {
                    BlackCards.AddRange(cards);
                }
                BlackCardType = black.ContainsKey("cardsType") ? black.GetInt("cardsType") : -1;
            }
            else if (data.ContainsKey("zhuang"))
            {
                black = data.GetSFSObject("zhuang");
                var cards = black.ContainsKey("cards") ? black.GetIntArray("cards") : null;
                if (cards != null)
                {
                    BlackCards.AddRange(cards);
                }
                BlackCardType = black.ContainsKey("value") ? black.GetInt("value") : -1;
            }

            ISFSObject red;
            if (data.ContainsKey("red"))
            {
                red = data.GetSFSObject("red");
                var cards = red.ContainsKey("cards") ? red.GetIntArray("cards") : null;
                if (cards != null)
                {
                    RedCards.AddRange(cards);
                }
                RedCardType = red.ContainsKey("cardsType") ? red.GetInt("cardsType") : -1;
            }
            else if (data.ContainsKey("xian"))
            {
                red = data.GetSFSObject("xian");
                var cards = red.ContainsKey("cards") ? red.GetIntArray("cards") : null;
                if (cards != null)
                {
                    RedCards.AddRange(cards);
                }
                RedCardType = red.ContainsKey("value") ? red.GetInt("value") : -1;
            }

            if (data.ContainsKey("cards"))
            {
                var cards = data.GetSFSArray("cards");
                if (cards != null)
                {
                    
//                       CardsData cardsDat=new CardsData(cards.GetSFSObject(cards.Count - 1));
//                    if (cardsDat.Type == -1)
//                    {
                        Cards.Add(cards.GetIntArray(cards.Count - 1));
                        for (int i = 0; i < cards.Count - 1; i++)
                        {
                            var singleCards = cards.GetIntArray(i);
                            Cards.Add(singleCards);
                        }
//                    }
//                    else
//                    {
//                        CardsDatas.Add(cardsDat);
//                        for (int i = 0; i < cards.Count - 1; i++)
//                        {
//                            CardsData cardData=new CardsData(cards.GetSFSObject(i));
//                            CardsDatas.Add(cardData);
//                        }
//                    }
                }
            }

            if (data.ContainsKey("nn"))
            {
                var nn = data.GetSFSArray("nn");
                if (nn != null)
                {
                    var lastData = nn.GetSFSObject(nn.Count-1);
                    var nData = new NnData(lastData);
                    NnDatas.Add(nData);
                    for (int i = 0; i < nn.Count; i++)
                    {
                        var nnData = nn.GetSFSObject(i);
                        var singleData=new NnData(nnData);
                        NnDatas.Add(singleData);
                    }
                }
            }
        }
    }

    public class BankListData
    {
        public int Banker;
        public List<BankData> BankList = new List<BankData>();

        public BankListData(ISFSObject data)
        {
            Banker = data.ContainsKey("banker") ? data.GetInt("banker") : -1;
            var bankers = data.ContainsKey("bankers") ? data.GetSFSArray("bankers") : null;
            if (bankers != null)
            {
                for (int i = 0; i < bankers.Count; i++)
                {
                    var bankData = new BankData(bankers.GetSFSObject(i));
                    BankList.Add(bankData);
                }
            }
        }
    }

    public class BankData
    {
        public long TtGold;
        public string UserName;
        public int Seat;

        public BankData(ISFSObject data)
        {
            TtGold = data.ContainsKey("ttgold") ? data.GetLong("ttgold") : 0;
            UserName = data.ContainsKey("username") ? data.GetUtfString("username") : "";
            Seat = data.ContainsKey("seat") ? data.GetInt("seat") : -1;
        }
    }

    public class RankData
    {
        public List<int> GoldRank = new List<int>();

        public void SetRank(ISFSObject data)
        {
            var sszSeat = data.ContainsKey("ssz") ? data.GetInt("ssz") : -1;//神算子的座位号
            var goldRank = data.ContainsKey("goldRank") ? data.GetIntArray("goldRank") : null;
            if (goldRank != null)
            {
                GoldRank = new List<int>(goldRank);
                GoldRank.Insert(0, sszSeat);
            }
        }
    }

    public class NnData
    {
        public int[] NiuCards;
        public bool Win;
        public int Rate;
        public int Niu;
        public int Type;

        public NnData(ISFSObject data)
        {
            NiuCards= data.ContainsKey("niuCards")?data.GetIntArray("niuCards"):null;
            Win = data.ContainsKey("Win") && data.GetBool("Win");
            Rate = data.ContainsKey("rate") ? data.GetInt("rate") : -1;
            Niu = data.ContainsKey("niu") ? data.GetInt("niu") : -1;
            Type = data.ContainsKey("type") ? data.GetInt("type") : -1;
        }
    }

    public class CardsData
    {
        public int[] Cards;
        public int Rate;
        public int Type;
        public int Value;

        public CardsData(ISFSObject data)
        {
            Cards = data.ContainsKey("cards") ? data.GetIntArray("cards") : null;
            Rate = data.ContainsKey("rate") ? data.GetInt("rate") : -1;
            Type = data.ContainsKey("type") ? data.GetInt("type") : -1;
            Value = data.ContainsKey("value") ? data.GetInt("value") : -1;
        }
    }


    public class ResultData
    {
        public int Win;//赢的钱
        public int[] Bpg;//庄家赢的钱
        public long BankWin;//庄家赢的钱
        public long BankerGold;//庄家现有的钱
        public long Total;//玩家的总钱数
        public List<string> WinArea=new List<string>();//赢的位置
        public int WinType;
        public int WinValue;
        public bool IsHasLuck;
        public List<string> ResultShowList=new List<string>();
        public ISFSArray Playerlist = new SFSArray();


        public ResultData(ISFSObject data)
        {
            Win = data.ContainsKey("win") ? data.GetInt("win") : -1;
            Bpg = data.ContainsKey("bpg") ? data.GetIntArray("bpg") : null;
            BankWin = data.ContainsKey("bwin") ? data.GetLong("bwin") : -1;
            BankerGold = data.ContainsKey("bankerGold") ? data.GetLong("bankerGold") : -1;
            Total = data.ContainsKey("total") ? data.GetLong("total") : -1;
            WinType = data.ContainsKey("winType") ? data.GetInt("winType") : -1;
            WinValue = data.ContainsKey("WinValue") ? data.GetInt("WinValue") : -1;
            IsHasLuck = data.ContainsKey("luck") && data.GetBool("luck");
            Playerlist = data.ContainsKey("playerlist") ? data.GetSFSArray("playerlist") : null;

            if (data.ContainsKey("winArea"))
            {
                WinArea.Add(data.GetInt("winArea").ToString());
            }
            else if (data.ContainsKey("winPos"))
            {
               var winArea= data.GetUtfString("winPos");
                WinArea.AddRange(winArea.Split(','));
            }

            if (IsHasLuck)
            {
                ResultShowList.Add("2"); 
            }
            ResultShowList.AddRange(WinArea);

            if (Playerlist != null)
            {
                var gdata = App.GetGameData<HgGameData>();
                gdata.AllUserWinGolds = 0;
                for (int i = 0; i < Playerlist.Count; i++)
                {
                    var userData = Playerlist.GetSFSObject(i);
                    var twentyBet = userData.ContainsKey("twentyBet") ? userData.GetInt("twentyBet") : -1;
                    var win = userData.ContainsKey("win") ? userData.GetInt("win") : -1;
                    var ttgold = userData.ContainsKey("ttgold") ? userData.GetLong("ttgold") : -1;
                    var userName = userData.ContainsKey("username") ? userData.GetUtfString("username") : "";
                    var seat = userData.ContainsKey("seat") ? userData.GetInt("seat") : -1;
                    var twentyWin = userData.ContainsKey("twentyWin") ? userData.GetInt("twentyWin") : -1;
                    var userId = userData.ContainsKey("userid") ? userData.GetInt("userid") : -1;

                    if (win > 0)
                    {
                        gdata.AllUserWinGolds += win;
                    }

                    for (int j = 0; j < gdata.AllUserInfos.Count; j++)
                    {
                        if (seat == gdata.AllUserInfos[j].Seat)
                        {
                            gdata.AllUserInfos[j].TwentyBet = twentyBet;
                            gdata.AllUserInfos[j].WinCoin = win;

                            gdata.AllUserInfos[j].CoinA = ttgold;
                            gdata.AllUserInfos[j].NickM = userName;
                            gdata.AllUserInfos[j].TwentyWin = twentyWin;
                            gdata.AllUserInfos[j].UserId = userId.ToString();
                        }

                    }
                }
            }
        }
    }
}
