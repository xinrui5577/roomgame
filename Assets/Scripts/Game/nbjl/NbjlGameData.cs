using System.Collections.Generic;
using System.Linq;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Utils;
using YxFramwork.Enums;

/*===================================================
 *文件名称:     NbjlGameData.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-06-26
 *描述:        	百家乐2D游戏数据
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.nbjl
{
    public class NbjlGameData : YxGameData 
    {
        #region 初始化信息
        /// <summary>
        /// 在线玩家累计局数
        /// </summary>
        public int AccumulateCount { get; private set;}

        /// <summary>
        /// 下注区域倍率
        /// </summary>
        public List<int> RateValues { get; private set;}

        /// <summary>
        /// 重连游戏状态
        /// </summary>
        public GameStatue Status { get; private set;}

        /// <summary>
        /// 游戏交互状态
        /// </summary>
        public ServerRequest ReqStatus{get; set;}

        /// <summary>
        /// 申请上庄的下限
        /// </summary>
        public int BankerLimit { get; private set; }

        /// <summary>
        /// 初始化数据状态
        /// </summary>
        public bool IsInitData { get; set; }

        #endregion
        #region 上庄相关
        /// <summary>
        /// 等待上庄列表
        /// </summary>
        public List<IRecycleData> WaitBankers { get; private set; }

        /// <summary>
        /// 庄家信息
        /// </summary>
        public NbjlPlayerInfo BankerInfo { get; private set; }

        /// <summary>
        /// 庄家座位号
        /// </summary>
        public int BankerSeat { get; private set; }

        /// <summary>
        /// 当前玩家申请上庄状态
        /// </summary>
        public bool IsApplyBanker { get;private set; }

        /// <summary>
        /// 上庄状态
        /// </summary>
        public bool IsOnBanker { get; private set; }

        /// <summary>
        /// 等待上庄列表变化
        /// </summary>
        public bool WaitBankerListChange { get; set; }

        #endregion

        #region 排行相关

        /// <summary>
        /// 玩家数据
        /// </summary>
        public List<NbjlPlayerInfo> Players { get; private set; }

        /// <summary>
        /// 单局玩家数据（游戏初始阶段同步玩家数据，直到下注结束，锁定牌局玩家直到下局开始）
        /// </summary>
        public List<NbjlPlayerInfo> RoundPlayers { get; private set; }

        /// <summary>
        /// 刷新玩家数据
        /// </summary>
        public bool FreshPlayers { get; set; }

        /// <summary>
        /// 当前玩家信息
        /// </summary>
        public NbjlPlayerInfo CurPlayerInfo { get; private set; }

        /// <summary>
        /// 神算子座位号
        /// </summary>
        public int SszSeat { get; private set; }
        #endregion

        #region 历史记录

        /// <summary>
        /// 历史记录
        /// </summary>
        public List<TrendData> Record { get; private set; }
        #endregion

        #region 牌结果
        /// <summary>
        /// 牌局结果
        /// </summary>
        public List<CardsData> CardInfos { get; set; }

        #endregion

        #region 分数结果
        /// <summary>
        /// 当前玩家得分
        /// </summary>
        public int Win { get; private set; }
        /// <summary>
        /// 庄家得分
        /// </summary>
        public int[] BankerWin { get; private set; }

        /// <summary>
        /// 庄家赢得金币（抽水后）
        /// </summary>
        public long BankerWinGold { get; private set; }

        /// <summary>
        /// 庄家是否获胜
        /// </summary>
        public bool BankerIsWin { get; private set; }

        #endregion

        #region 下注
        /// <summary>
        /// 最新下注信息
        /// </summary>
        public BetData CurBetData { get;private set; }

        /// <summary>
        /// 流式下注结果
        /// </summary>
        public List<BetData> FlushBetDatas { get; private set; }

        /// <summary>
        /// 重连下注数据
        /// </summary>
        public List<BetData> GameInfoBetDatas { get; private set; }

        /// <summary>
        /// 下注历史（成功下注记录）
        /// </summary>
        public BetHistory BetHistory { get; private set; }
        /// <summary>
        /// 局数
        /// </summary>
        public int Round { get; set; }


        #endregion

        #region CD

        /// <summary>
        /// CD 时间
        /// </summary>
        public int Cd { get; private set; }

        #endregion

        protected override void InitGameData(ISFSObject gameInfo)
        {
            IsInitData = true;
            FreshPlayers = true;
            Round = 0;
            BetHistory = new BetHistory();
            BankerSeat =ConstantData.KeyDefaultInt;
            WaitBankers = new List<IRecycleData>();
            Status = (GameStatue)gameInfo.GetInt(ConstantData.KeyStatus);
            switch (Status)
            {
                case GameStatue.Wait:
                case GameStatue.GameStart:
                case GameStatue.BankerGold:
                    ReqStatus = ServerRequest.ChinaMobile;
                    break;
                case GameStatue.Account:
                    ReqStatus = ServerRequest.ChinaUnicom;
                    break;
                case GameStatue.Bet:
                    ReqStatus = ServerRequest.ReqBeginBet;
                    break;
                case GameStatue.CardResult:
                    ReqStatus = ServerRequest.ReqGiveCards;
                    break;
            }
            BankerLimit = gameInfo.GetInt(ConstantData.KeyBankLimit);
            RateValues = gameInfo.GetIntArray(ConstantData.KeyRateValue).ToList();
            AccumulateCount = gameInfo.GetInt(ConstantData.KeyAccumulateCount);
            GetUsersList(gameInfo);
            GetBanerInfos(gameInfo);
            GetHistoryInfos(gameInfo);
            GetGoldRank(gameInfo);
            FreshUserBet();
        }

        /// <summary>
        /// 获得庄相关信息
        /// </summary>
        /// <param name="data"></param>
        public void GetBanerInfos(ISFSObject data)
        {
            var bankersData = data.GetSFSArray(ConstantData.KeyBankers);
            var newBankerSeat = data.GetInt(ConstantData.KeyBankerSeat);
            BankerInfo = null;
            IsApplyBanker = false;
            BankerSeat = newBankerSeat;
            if (bankersData.Count == 0)
            {
                WaitBankerListChange = WaitBankers.Count != 0;
                WaitBankers = new List<IRecycleData>();
            }
            else
            {
                var newWaitList=new List<NbjlPlayerInfo>();
                var changeBankers=new List<NbjlPlayerInfo>();
                foreach (ISFSObject banker in bankersData)
                {
                    var user = new NbjlPlayerInfo();
                    user.Parse(banker);
                    var seat = user.Seat;
                    if (seat == SelfSeat)
                    {
                        IsApplyBanker = true;
                    }
                    changeBankers.Add(user);
                    if (seat == BankerSeat)
                    {
                        continue;
                    }
                    newWaitList.Add(user);
                }
                var changeCount = changeBankers.Count;
                if (changeCount>0)
                {
                    var playerCount = Players.Count;
                    for (int i = 0; i < changeCount; i++)
                    {
                       var playerIndex=Players.FindIndex(item => (item != null && item.Seat == changeBankers[i].Seat));
                        if (playerIndex>=0)
                        {
                            Players[playerIndex].CoinA = changeBankers[i].CoinA;
                        }
                    }
                }
                var oldWaitBankers = WaitBankers.ConvertAll(item=>item as NbjlPlayerInfo);
                WaitBankerListChange = oldWaitBankers.Count != newWaitList.Count;
                if (!WaitBankerListChange)
                {
                    var count = oldWaitBankers.Count;
                    var state = false;
                    for (int i = 0; i < count; i++)
                    {
                        var oldItem = oldWaitBankers[i];
                        var newItem = newWaitList[i];
                        if (newItem.Seat!=oldItem.Seat||newItem.NickM!=oldItem.NickM|| newItem.CoinA != oldItem.CoinA)
                        {
                            state = true;
                            break;
                        }
                    }
                    WaitBankerListChange = state;
                }
                if (WaitBankerListChange)
                {
                    WaitBankers = newWaitList.ConvertAll(item => item as IRecycleData);
                }
            }
            if (BankerSeat != ConstantData.KeyDefaultInt)
            {
                RoundPlayersFresh();
            }
            IsOnBanker = BankerInfo != null && BankerInfo.Seat.Equals(SelfSeat);
            if (IsOnBanker)
            {
                App.GameData.GStatus = YxEGameStatus.PlayAndConfine;
                IsApplyBanker = true;
            }
        }

        public void FreshBankerInfo()
        {
            var info=RoundPlayers.Find(player => player!=null&&player.Seat == BankerSeat);
            if (info!=null)
            {
                BankerInfo = info;
            }
        }

        public void FreshWaitBankerList()
        {
            var freshBankers=new List<IRecycleData>();
            var oldBankers = WaitBankers.ConvertAll(item => item as NbjlPlayerInfo);
            foreach (var waitBanker in oldBankers)
            {
                if (waitBanker!=null)
                {
                    var getPlayer=RoundPlayers.Find(player => player != null && waitBanker.Seat == player.Seat);
                    if(getPlayer!=null)
                    {
                        freshBankers.Add(getPlayer);
                    }
                }
            }
            WaitBankers = freshBankers.ToList();
        }

        /// <summary>
        /// 获得历史记录相关信息
        /// </summary>
        /// <param name="data"></param>
        public void GetHistoryInfos(ISFSObject data)
        {
            if (data.ContainsKey(ConstantData.KeyRecord))
            {
                var record = data.GetSFSArray(ConstantData.KeyRecord);
                Record=new List<TrendData>();
                var count = record.Count;
                for (int i = 0; i < count; i++)
                {
                    var item=record.GetSFSObject(i);
                    TrendData newData=new TrendData(item);
                    Record.Add(newData);
                }
            }
        }

        /// <summary>
        /// 获得金币排行数据
        /// </summary>
        /// <param name="data"></param>
        public void GetGoldRank(ISFSObject data)
        {
            HistoryBetSet();
            var goldRank = data.GetIntArray(ConstantData.KeyGoldRank).ToList();
            SszSeat = data.GetInt(ConstantData.KeySsz);
            var totalList = new List<int> {SszSeat};
            totalList.AddRange(goldRank);
            foreach (var player in Players)
            {
                if (player!=null)
                {
                    //Debug.LogError(string.Format("座位:{0}，昵称：{1}，金币：{2}，获胜次数：{3}，下注金币:{4}", player.Seat, player.NickM, player.CoinA, player.AccumulateWin, player.AccumulateBet));
                    var seat = player.Seat;
                    var findIndex = totalList.FindIndex(item => item == seat);
                    if (findIndex <= -1)
                    {
                        totalList.Add(seat);
                    }
                }
            }
            var newPlayers= totalList.Select(t => Players.Find(player => player != null && player.Seat == t)).ToList();
            Players = newPlayers.ToList();
            RoundPlayersFresh();
        }

        /// <summary>
        /// 获得翻牌结果
        /// </summary>
        /// <param name="data"></param>
        public void GetCardsResult(ISFSObject data)
        {
            if(data==null)return;
            CardInfos = new List<CardsData>();
            TrendData newData = new TrendData();
            int zhuangScore=0;
            int xianScore=0;
            if (data.ContainsKey(ConstantData.KeyZhuang))
            {
                CardsData zhuangInfo = new CardsData(CardResultType.Zhuang, data.GetSFSObject(ConstantData.KeyZhuang));
                newData.ZhuangDui = zhuangInfo.DoubleCard;
                newData.ZhuangTian = zhuangInfo.King;
                CardInfos.Add(zhuangInfo);
                zhuangScore = zhuangInfo.Result;
            }
            if (data.ContainsKey(ConstantData.KeyXian))
            {
                var xianInfo = new CardsData(CardResultType.Xian, data.GetSFSObject(ConstantData.KeyXian));
                newData.XianDui = xianInfo.DoubleCard;
                newData.XianTian = xianInfo.King;
                CardInfos.Add(xianInfo);
                xianScore = xianInfo.Result;
            }
            if (zhuangScore==xianScore)
            {
                newData.Win = ConstantData.KeyBetEqual;
            }
            else
            {
                newData.Win = zhuangScore > xianScore ? ConstantData.KeyBetBanker : ConstantData.KeyBetLeisure;
            }
            if (Record==null)
            {
                Record=new List<TrendData>();
            }
            Record.Add(newData);
        }

        /// <summary>
        /// 获得分数结果
        /// </summary>
        /// <param name="data"></param>
        public void GetScoreResult(ISFSObject data)
        {
            GetOnLinePlayerList(data,true);
            Win = data.GetInt(ConstantData.KeyWinCount);
            BankerWin = data.GetIntArray(ConstantData.KeyBetPage);
            var count = BankerWin.Sum();
            if (data.ContainsKey(ConstantData.KeyBankerWinGold))
            {
                BankerWinGold = data.GetLong(ConstantData.KeyBankerWinGold);
            }
            else
            {
                BankerWinGold = count;
            }
            BankerIsWin = count > 0;
        }

        /// <summary>
        /// 在线玩家列表信息(玩家信息同步)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="freshRound"></param>
        public void GetOnLinePlayerList(ISFSObject data,bool freshRound)
        {
            var list = data.GetSFSArray(ConstantData.KeyPlayerList);
            if (list!=null)
            {
                var count = list.Count;
                var onLineDatas = new List<NbjlOnLinePlayerData>();
                for (int i = 0; i < count; i++)
                {
                    var item = list.GetSFSObject(i);
                    var info = new NbjlOnLinePlayerData();
                    info.Parse(item);
                    onLineDatas.Add(info);
                }
                if (onLineDatas.Count>=0)
                {
                    foreach (var player in Players)
                    {
                        if (player!=null)
                        {
                            var id = player.Id;
                            NbjlOnLinePlayerData infodata = onLineDatas.Find(item => item.UserId == id);
                            if (infodata != null)
                            {
                                player.FreshDatas(infodata);
                            }
                        }
                    }
                    if (freshRound)
                    {
                        foreach (var player in RoundPlayers)
                        {
                            if (player != null)
                            {
                                var id = player.Id;
                                NbjlOnLinePlayerData infodata = onLineDatas.Find(item => item.UserId == id);
                                if (infodata != null)
                                {
                                    player.FreshDatas(infodata);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获得玩家列表
        /// </summary>
        /// <param name="data"></param>
        public void GetUsersList(ISFSObject data)
        {
            var users = data.GetSFSArray(ConstantData.KeyUsers);
            Players=new List<NbjlPlayerInfo>();
            var count = users.Count;
            bool localExist = false;
            for (int i = 0; i < count; i++)
            {
                var items = users.GetSFSObject(i);
                var info=new NbjlPlayerInfo();
                info.Parse(items);
                Players.Add(info);
                if (info.Seat.Equals(SelfSeat))
                {
                    CurPlayerInfo = info;
                    localExist = true;
                }
            }
            if (!localExist)
            {
                var user = data.GetSFSObject(ConstantData.KeyUser);
                if (user != null)
                {
                    var userInfo = new NbjlPlayerInfo();
                    userInfo.Parse(user);
                    CurPlayerInfo = userInfo;
                }
                Players.Add(CurPlayerInfo);
            }
            RoundPlayersFresh();
        }

        private readonly string[] _pos = { "z", "x", "h", "zd", "xd" };
        /// <summary>
        /// 获取重连后玩家下注情况
        /// </summary>
        private void FreshUserBet()
        {
            GameInfoBetDatas=new List<BetData>();
            foreach (var player in RoundPlayers)
            {
                if(player!=null)
                {
                    if (player.BetGolds!=null)
                    {
                        var count = player.BetGolds.Length;
                        for (int i = 0; i < count; i++)
                        {
                            BetData data=new BetData(_pos[i],player.Seat, player.BetGolds[i]);
                            if (data.Gold>0)
                            {
                                GameInfoBetDatas.Add(data);
                            }

                        }
                    }
                }
            }
        }

        /// <summary>
        ///  玩家进入
        /// </summary>
        /// <param name="data"></param>
        public void OnUserJoin(ISFSObject data)
        {
            var info = data.GetSFSObject(ConstantData.KeyUser);
            NbjlPlayerInfo playerInfo =new NbjlPlayerInfo();
            playerInfo.Parse(info);
            Players.Add(playerInfo);
            RoundPlayersFresh();
        }

        /// <summary>
        /// 玩家退出
        /// </summary>
        /// <param name="data"></param>
        public void OnUserOut(ISFSObject data)
        {
            var seat = data.GetInt(ConstantData.KeySeat);
            var info=Players.Find(player => player!=null&&player.Seat == seat);
            if (info!=null)
            {
                Players.Remove(info);
            }
            RoundPlayersFresh();
        }


        public void RoundPlayersFresh()
        {
            if (FreshPlayers)
            {
                RoundPlayers = Players.ToList();
                FreshBankerInfo();
            }
        }

        /// <summary>
        /// 设置历史下注
        /// </summary>
        private void HistoryBetSet()
        {
            BetHistory.InitHistory();
        }

        /// <summary>
        /// 获取CD时间
        /// </summary>
        /// <param name="data"></param>
        public void GetCd(ISFSObject data)
        {
            if (data.ContainsKey(ConstantData.KeyCd))
            {
                Cd = data.GetInt(ConstantData.KeyCd);
            }
        }
        /// <summary>
        /// 获得下注数据
        /// </summary>
        public void GetBetData(ISFSObject data)
        {
            CurBetData=new BetData(data);
            if (CurBetData.Seat == SelfSeat)
            {
                if (Round != BetHistory.Round)
                {
                    BetHistory.Round = Round;
                    BetHistory.CurBets = new int[5];
                }
                switch (CurBetData.Position)
                {
                    case ConstantData.KeyBetBanker:
                        BetHistory.CurBets[0] += CurBetData.Gold;
                        break;
                    case ConstantData.KeyBetLeisure:
                        BetHistory.CurBets[1] += CurBetData.Gold;
                        break;
                    case ConstantData.KeyBetEqual:
                        BetHistory.CurBets[2] += CurBetData.Gold;
                        break;
                    case ConstantData.KeyBetBankerDouble:
                        BetHistory.CurBets[3] += CurBetData.Gold;
                        break;
                    case ConstantData.KeyBetLeisureDouble:
                        BetHistory.CurBets[4] += CurBetData.Gold;
                        break;
                }
            }
        }

        /// <summary>
        /// 流式下注
        /// </summary>
        public void GetBetDatas(ISFSObject data)
        {
            if (data.ContainsKey(ConstantData.KeyCoin))
            {
                var datas = data.GetSFSArray(ConstantData.KeyCoin);
                var count = datas.Count;
                var list=new List<BetData>();
                for (int i = 0; i < count; i++)
                {
                    ISFSObject item = datas.GetSFSObject(i);
                    var betData=new BetData(item);
                    if (betData.Seat==SelfSeat)
                    {
                        continue;
                    }
                    list.Add(betData);
                }
                FlushBetDatas = list.ToList();
            }
        }

    }
}
