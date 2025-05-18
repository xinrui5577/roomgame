using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Game.car
{
    public class CarGameData : YxGameData
    {
        public EventObject EventObj;
        public bool BeginBet;
        public int GameRecordNum;
        public int PlayerRecordNum;
        public float UnitTime = 0.1f;
        public long AllUserWinGolds;
        public CarPlayer BankPlayer;
        public List<int> RecordDatas = new List<int>();
        public List<int> GoldRank = new List<int>();//排行中玩家的座位号
        public List<CarUserInfo> AllUserInfos = new List<CarUserInfo>();
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
            var record = gameInfo.ContainsKey("record") ? gameInfo.GetSFSArray("record") : null;
            if (record != null)
            {
                for (int i = 0; i < record.Count; i++)
                {
                    if (record.GetSFSObject(i) == null) continue;
                    var recordValue = new RecordValue(record.GetSFSObject(i));
                    RecordDatas.Add(recordValue.Win);
                }
            }

            var user = gameInfo.ContainsKey("user") ? gameInfo.GetSFSObject("user") : null;
            if (user != null)
            {
                var userInfo = new CarUserInfo();
                userInfo.Parse(user);
                GetPlayer<CarPlayer>().Info = userInfo;
                AllUserInfos.Add(userInfo);
            }

            var users = gameInfo.ContainsKey("users") ? gameInfo.GetSFSArray("users") : null;
            if (users != null)
            {
                for (int i = 0; i < users.Count; i++)
                {
                    var userInfo = new CarUserInfo();
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
                var bankerInfo = new CarUserInfo
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
                        var bankerInfo = new CarUserInfo
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
        public int Win = -1;

        public RecordValue(ISFSObject data)
        {
            if (data.ContainsKey("win"))
            {
                Win = data.GetInt("win");
            }
        }
    }


    public class RollResult
    {
        public int CarIdx;
        public int Car;
        public int Cd;

        public RollResult(ISFSObject data)
        {
            CarIdx = data.ContainsKey("carIdx") ? data.GetInt("carIdx") : -1;
            Car = data.ContainsKey("car") ? data.GetInt("car") : -1;
            Cd = data.ContainsKey("cd") ? data.GetInt("cd") : -1;
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
            NiuCards = data.ContainsKey("niuCards") ? data.GetIntArray("niuCards") : null;
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
        public int Car =-1;//赢的位置
        public List<ResultUserData> Playerlist=new List<ResultUserData>();


        public ResultData(ISFSObject data)
        {
            Win = data.ContainsKey("win") ? data.GetInt("win") : -1;
            Bpg = data.ContainsKey("bpg") ? data.GetIntArray("bpg") : null;
            BankWin = data.ContainsKey("bwin") ? data.GetLong("bwin") : -1;
            BankerGold = data.ContainsKey("bankerGold") ? data.GetLong("bankerGold") : -1;
            Total = data.ContainsKey("total") ? data.GetLong("total") : -1;
            var playerlist = data.ContainsKey("playerlist") ? data.GetSFSArray("playerlist") : null;

            var gdata = App.GetGameData<CarGameData>();
            if (data.ContainsKey("car"))
            {
                Car=data.GetInt("car");
                gdata.RecordDatas.Add(Car);
            }
         
            if (playerlist != null)
            {
               
                gdata.AllUserWinGolds = 0;
                for (int i = 0; i < playerlist.Count; i++)
                {
                    var userData = playerlist.GetSFSObject(i);

                    var resultUserData = new ResultUserData(userData);
                   
                    Playerlist.Add(resultUserData);
                    if (resultUserData.Win > 0)
                    {
                        gdata.AllUserWinGolds += resultUserData.Win;
                    }

                    for (int j = 0; j < gdata.AllUserInfos.Count; j++)
                    {
                        if (resultUserData.Seat == gdata.AllUserInfos[j].Seat)
                        {
                            gdata.AllUserInfos[j].TwentyBet = resultUserData.TwentyBet;
                            gdata.AllUserInfos[j].WinCoin = resultUserData.Win;

                            gdata.AllUserInfos[j].CoinA = resultUserData.Ttgold;
                            gdata.AllUserInfos[j].NickM = resultUserData.UserName;
                            gdata.AllUserInfos[j].TwentyWin = resultUserData.TwentyWin;
                            gdata.AllUserInfos[j].UserId = resultUserData.UserId.ToString();
                        }

                    }
                }
            }
        }
    }

    public class ResultUserData
    {
        public int TwentyBet;
        public int Win;
        public long Ttgold;
        public string UserName;
        public int Seat;
        public int TwentyWin;
        public int UserId;


        public ResultUserData(ISFSObject userData)
        {
            TwentyBet = userData.ContainsKey("twentyBet") ? userData.GetInt("twentyBet") : -1;
            Win = userData.ContainsKey("win") ? userData.GetInt("win") : -1;
            Ttgold = userData.ContainsKey("ttgold") ? userData.GetLong("ttgold") : -1;
            UserName = userData.ContainsKey("username") ? userData.GetUtfString("username") : "";
            Seat = userData.ContainsKey("seat") ? userData.GetInt("seat") : -1;
            TwentyWin = userData.ContainsKey("twentyWin") ? userData.GetInt("twentyWin") : -1;
            UserId = userData.ContainsKey("userid") ? userData.GetInt("userid") : -1;
        }
    }
}
