using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

/*===================================================
 *文件名称:     NbjlGameManager.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-06-26
 *描述:        	百家乐2DGameManager
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.nbjl
{
    public class NbjlGameManager : YxGameManager 
    {
        #region UI Param

        #endregion

        #region Data Param
        [Tooltip("配置信息，本地调整数据")]
        public GameConfig GameConifig;

        /// <summary>
        /// 当前选中筹码的索引
        /// </summary>
        public int SelectChipIndex { get; set; }

        /// <summary>
        /// 当前选中筹码值
        /// </summary>
        public int CurrentChipValue
        {
            get { return Data.AnteRate[SelectChipIndex]; }
        }


        #endregion

        #region Local Data
        private NbjlGameData Data
        {
            get
            {
                return App.GetGameData<NbjlGameData>();
            }
        }

        #endregion

        #region Request

        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            Data.FreshPlayers = false;
            FireInitEvent();
            switch (Data.ReqStatus)
            {
                case ServerRequest.ReqBeginBet:
                    OnBeginBet(gameInfo);
                    PlayersBet(Data.GameInfoBetDatas);
                    break;
                case ServerRequest.ReqGiveCards:
                    if (gameInfo.ContainsKey(ConstantData.KeyRollResult))
                    {
                        var rollResult = gameInfo.GetSFSObject(ConstantData.KeyRollResult);
                        OnCardsResult(rollResult, ConstantData.KeyQuickModel);
                    }
                    PlayersBet(Data.GameInfoBetDatas);
                    break;
                case ServerRequest.ChinaUnicom:
                    Facade.EventCenter.DispatchEvent(LocalRequest.Init, 1);
                    break;
                case ServerRequest.ChinaMobile:
                    OnChinaMoble();
                    break;
            }
            Data.IsInitData = false;
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
            YxDebug.LogError("OnGetRejoinInfo");
        }

        public override void GameStatus(int status, ISFSObject info)
        {
            YxDebug.LogError("GameStatus");
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            switch ((ServerRequest)type)
            {
                case ServerRequest.ClearRecord:
                    OnClearHistory();
                    break;
                case ServerRequest.ChinaMobile:
                    OnChinaMoble();
                    break;
                case ServerRequest.ReqBankerList:
                    OnBankerList(response);
                    break;
                case ServerRequest.ReqBeginBet:
                    Data.ReqStatus = (ServerRequest)type;
                    OnBeginBet(response);
                    break;
                case ServerRequest.ReqEndBet:
                    Data.ReqStatus = (ServerRequest)type;
                    OnEndBet();
                    break;
                case ServerRequest.ReqBet:
                    OnBet(response);
                    break;
                case ServerRequest.ReqGiveCards:
                    Data.ReqStatus = (ServerRequest)type;
                    OnCardsResult(response,ConstantData.KeyNormalModel);
                    break;
                case ServerRequest.ReqResult:
                    Data.ReqStatus = (ServerRequest)type;
                    OnGameResult(response);
                    break;
                case ServerRequest.FlushBet:
                    OnFlush(response);
                    break;

            }
        }
        /// <summary>
        /// 10086
        /// </summary>
        /// <param name="sfsObject"></param>
        public override void BeginNewGame(ISFSObject sfsObject)
        {
            base.BeginNewGame(sfsObject);
            OnChinaMoble();
        }

        /// <summary>
        /// 其它玩家进入房间
        /// </summary>
        /// <param name="sfsObject"></param>
        public override void OnOtherPlayerJoinRoom(ISFSObject sfsObject)
        {
            base.OnOtherPlayerJoinRoom(sfsObject);
            Data.OnUserJoin(sfsObject);
        }

        public override void UserOut(int localSeat, ISFSObject responseData)
        {
            base.UserOut(localSeat, responseData);
            Data.OnUserOut(responseData);
        }

        #endregion

        #region Function

        /// <summary>
        /// 发送启动信息
        /// </summary>
        private void FireInitEvent()
        {
            //Debug.LogError("-------------------------------初始化消息----------------------------------");
            //上庄限制
            Facade.EventCenter.DispatchEvent(LocalRequest.BankerLimit, Data.BankerLimit);
            Facade.EventCenter.DispatchEvent(LocalRequest.AccumulateCount, Data.AccumulateCount);
            Facade.EventCenter.DispatchEvent(LocalRequest.AnteRate, Data.AnteRate);
            FireRateValues();
            FireBankerEvent();
            FireRecords();
            FirePlayerInfos();
            FireGoldRank();
        }

        /// <summary>
        /// 下注区域倍率
        /// </summary>
        private void FireRateValues()
        {
            var count = GameConifig.RateNames.Length;
            for (int i = 0;i<count; i++)
            {
                Facade.EventCenter.DispatchEvent(GameConifig.RateNames[i], Data.RateValues[i]);
            }
        }

        private void OnClearHistory()
        {
            YxMessageTip.Show(GameConifig.NewBigRoundNotice);
            Facade.EventCenter.DispatchEvent(LocalRequest.Records,new List<TrendData>());
        }

        /// <summary>
        /// 中国移动带人来
        /// </summary>
        private void OnChinaMoble()
        {
            Data.Round++;
            Data.FreshPlayers = true;
            Data.ReqStatus =ServerRequest.ChinaMobile;
            CancelInvoke();
            Facade.EventCenter.DispatchEvent(LocalRequest.Init, 1);
        }

        /// <summary>
        /// 庄家列表变动（上庄与下庄后都会发生变化，实时以庄家列表信息为主）
        /// </summary>
        /// <param name="data"></param>
        private void OnBankerList(ISFSObject data)
        {
            //Debug.LogError("-------------------------------上庄列表消息----------------------------------")
            Data.GetBanerInfos(data);
            Data.GetOnLinePlayerList(data,Data.FreshPlayers);
            FireBankerEvent();
            if (Data.FreshPlayers)
            {
                if (!Data.RoundPlayers.SequenceEqual(Data.Players))
                {
                    Data.RoundPlayersFresh();
                    FirePlayerInfos();
                }
            }
          
        }

        /// <summary>
        /// 上庄列表事件发送
        /// </summary>
        private void FireBankerEvent()
        {
            if (Data.IsInitData)
            {
                Facade.EventCenter.DispatchEvent(LocalRequest.BankerState, Data.IsOnBanker);
                Facade.EventCenter.DispatchEvent(LocalRequest.BankerInfo, Data.BankerInfo);
                Facade.EventCenter.DispatchEvent(LocalRequest.BankerWaitList, Data.WaitBankers.ToList());
            }
            else
            {
                if (Data.FreshPlayers)
                {
                    Facade.EventCenter.DispatchEvent(LocalRequest.BankerInfo, Data.BankerInfo);
                    Facade.EventCenter.DispatchEvent(LocalRequest.BankerState, Data.IsOnBanker);
                }
                if (Data.WaitBankerListChange)
                {
                    Facade.EventCenter.DispatchEvent(LocalRequest.BankerWaitList, Data.WaitBankers.ToList());
                    Data.WaitBankerListChange = false;
                }
                Facade.EventCenter.DispatchEvent(LocalRequest.ApplyStateChange, Data.IsApplyBanker);
            }
        }
        
        /// <summary>
        /// 开始下注阶段
        /// </summary>
        /// <param name="data"></param>
        private void OnBeginBet(ISFSObject data)
        {
            Data.FreshPlayers = true;
            Data.GetGoldRank(data);
            Data.RoundPlayersFresh();
            FirePlayerInfos();
            FireGoldRank();
            Data.FreshWaitBankerList();
            FireBankerEvent();
            Facade.EventCenter.DispatchEvent(LocalRequest.BankerWaitList, Data.WaitBankers.ToList());
            Facade.EventCenter.DispatchEvent(LocalRequest.ReqBeginBet, 1);
            GetCd(data);
        }

        /// <summary>
        /// 金币排行刷新
        /// </summary>
        private void FireGoldRank()
        {
            Facade.EventCenter.DispatchEvent(LocalRequest.OnLinePlayerList, Data.RoundPlayers.ConvertAll<IRecycleData>(item => item));
        }

        /// <summary>
        /// 回放记录
        /// </summary>
        private void FireRecords()
        {
            Facade.EventCenter.DispatchEvent(LocalRequest.Records, Data.Record);
        }

        /// <summary>
        /// 单局回放记录
        /// </summary>
        IEnumerator FireSingleRecord(string result)
        {
            yield return new WaitForSeconds(1);
            Facade.Instance<MusicManager>().Play(result);
            Facade.EventCenter.DispatchEvent(LocalRequest.SingleRecord, Data.Record.Last());
        }


        /// <summary>
        /// Cd事件设置
        /// </summary>
        private void GetCd(ISFSObject data)
        {
            Data.GetCd(data);
            Facade.EventCenter.DispatchEvent(LocalRequest.Cd, Data.Cd);
        }

        /// <summary>
        /// 玩家下注
        /// </summary>
        /// <param name="data"></param>
        private void OnBet(ISFSObject data)
        {
            Data.GetBetData(data);
            Facade.Instance<MusicManager>().Play(ConstantData.KeySoundBet);
            Facade.EventCenter.DispatchEvent(LocalRequest.ReqBet, Data.CurBetData);
        }

        /// <summary>
        /// 流式下注
        /// </summary>
        /// <param name="data"></param>
        private void OnFlush(ISFSObject data)
        {
            Data.GetBetDatas(data);
            PlayersBet(Data.FlushBetDatas);
        }
        

        /// <summary>
        /// 玩家批量下注
        /// </summary>
        /// <param name="datas"></param>
        private void PlayersBet(List<BetData> datas) 
        {
            Facade.Instance<MusicManager>().Play(ConstantData.KeySoundFlushBet);
            foreach (var betData in datas)
            {
                Facade.EventCenter.DispatchEvent(LocalRequest.ReqBet, betData);
            }
        }

        /// <summary>
        /// 停止下注
        /// </summary>
        private void OnEndBet()
        {
            Data.FreshPlayers = false;
            Facade.EventCenter.DispatchEvent(LocalRequest.ReqEndBet,1);
        }

        /// <summary>
        ///  显示牌结果阶段
        /// </summary>
        /// <param name="data"></param>
        private void OnCardsResult(ISFSObject data,int modeType)
        {
            YxDebug.LogError("时间："+Time.realtimeSinceStartup);
            Data.FreshPlayers = false;
            Data.GetCardsResult(data);
            Facade.EventCenter.DispatchEvent(LocalRequest.ReqGiveCards, modeType);
            foreach (var cardsInfo in Data.CardInfos)
            {
                Facade.EventCenter.DispatchEvent(cardsInfo.CardsType, cardsInfo);
            }
            GetCd(data);
        }

        /// <summary>
        /// 结算信息
        /// </summary>
        /// <param name="data"></param>
        private void OnGameResult(ISFSObject data)
        {
            YxDebug.LogError("时间:"+Time.realtimeSinceStartup);
            Data.FreshPlayers = false;
            Data.GetScoreResult(data);
            Facade.EventCenter.DispatchEvent(LocalRequest.ReqResult, 1);
            FirePlayerInfos();
            FireBankerEvent();
            App.GameData.GStatus = YxEGameStatus.Normal;
        }

        /// <summary>
        /// 玩家信息变化
        /// </summary>
        private void FirePlayerInfos()
        {
            Facade.EventCenter.DispatchEvent(LocalRequest.PlayerInfos, Data.RoundPlayers);
        }


        /// <summary>
        /// 下注阶段检测
        /// </summary>
        /// <returns></returns>
        public bool CheckBetState()
        {
            var canBet = true;
            if (Data.IsOnBanker)
            {
                YxMessageTip.Show(GameConifig.OnBankerCanNotBet);
                canBet = false;
            }
            if (Data.ReqStatus != ServerRequest.ReqBeginBet)
            {
                YxMessageTip.Show(GameConifig.OutOfBetStateNotice);
                canBet = false;
            }
            return canBet;
        }

        /// <summary>
        /// 尝试下注
        /// </summary>
        public bool CheckBet()
        {
            if (CurrentChipValue > App.GameData.GetPlayerInfo().CoinA)
            {
                YxMessageTip.Show(GameConifig.MoneyIsNotEnough);
                return false;
            }
            return true;
            
        }
        /// <summary>
        /// 重新下注
        /// </summary>
        public void ReBets()
        {
            if (CheckBetState())
            {
                var history = Data.BetHistory.HistoryBets;
                var checkValue = Data.BetHistory.Sum();
                if (checkValue==0)
                {
                    YxMessageTip.Show(GameConifig.HaveNotBet);
                    return;
                }
                if (checkValue > App.GameData.GetPlayerInfo().CoinA)
                {
                    YxMessageTip.Show(GameConifig.MoneyIsNotEnough);
                    return;
                }
                App.GetRServer<NbjlGameServer>().UserBets(history);
            }
        }

        /// <summary>
        /// 申请上庄
        /// </summary>
        public void ApplyBanker()
        {
            if (Data.CurPlayerInfo.CoinA<Data.BankerLimit)
            {
                YxMessageTip.Show(GameConifig.BankerGoldNotEnough);
                return;
            }
            App.GetRServer<NbjlGameServer>().ApplyBanker();
        }

        /// <summary>
        /// 申请下庄
        /// </summary>
        public void ApplyQuitBanker()
        {
            if (Data.IsOnBanker)
            {
                YxMessageTip.Show(GameConifig.QuitBankerNotice);
            }
            App.GetRServer<NbjlGameServer>().ApplyQuitBanker();
        }

        /// <summary>
        /// 牌显示完成数量
        /// </summary>
        private int _cardShowFinishedNum;
        /// <summary>
        /// 显示牌结束操作，等庄家闲家翻牌完毕后执行
        /// </summary>
        public void OnCardShowFinished()
        {
            _cardShowFinishedNum += 1;
            if (_cardShowFinishedNum>=2)
            {
                _cardShowFinishedNum = 0;
                Invoke("ShowCardXianNum",GameConifig.CardShowFinishedWaitTime);
            }
            
        }

        private void ShowCardXianNum()
        {
            Facade.Instance<MusicManager>().Play(string.Format(GameConifig.XianNumFormat,Data.CardInfos[1].Result));
            Invoke("ShowCardZhuangNum", 1);
            CancelInvoke("ShowCardXianNum");
        }

        private void ShowCardZhuangNum()
        {
            Facade.Instance<MusicManager>().Play(string.Format(GameConifig.ZhuangNumFormat, Data.CardInfos[0].Result));
            var zhuangScore= Data.CardInfos[0].Result;
            var xianScore = Data.CardInfos[1].Result;
            string result;
            if (zhuangScore>xianScore)
            {
                result = ConstantData.KeySoungBankerWin;
            }
            else if (zhuangScore== xianScore)
            {
                result = ConstantData.KeySoundEqule;
               
            }
            else
            {
                result = ConstantData.KeySoungLeisureWin;
            }
            StartCoroutine(FireSingleRecord(result));
            CancelInvoke("ShowCardZhuangNum");
        }


        #endregion
        /// <summary>
        /// Test
        /// </summary>
        void Update()
        {
            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //    List<CardsData> cardData = new List<CardsData>();
            //    CardsData zhuangInfo = new CardsData(CardResultType.Zhuang, 5, false, 3, false, new int[] { 26, 20, 17 });
            //    CardsData xianInfo = new CardsData(CardResultType.Xian, 7, false, 2, false, new int[] { 21, 18, 0 });
            //    cardData.Add(zhuangInfo);
            //    cardData.Add(xianInfo);
            //    Data.CardInfos = cardData.ToList();
            //    foreach (var cardsInfo in cardData)
            //    {
            //        Facade.EventCenter.DispatchEvent(cardsInfo.CardsType, cardsInfo);
            //    }
            //}
            //if (Input.GetKeyDown(KeyCode.S))
            //{
            //    List<CardsData> cardData = new List<CardsData>();
            //    CardsData zhuangInfo = new CardsData(CardResultType.Zhuang, 5, false, 2, false, new int[] { 26, 20, 0 });
            //    CardsData xianInfo = new CardsData(CardResultType.Xian, 7, false, 3, false, new int[] { 21, 18, 17 });
            //    cardData.Add(zhuangInfo);
            //    cardData.Add(xianInfo);
            //    Data.CardInfos = cardData.ToList();
            //    foreach (var cardsInfo in cardData)
            //    {
            //        Facade.EventCenter.DispatchEvent(cardsInfo.CardsType, cardsInfo);
            //    }
            //}
            //if (Input.GetKeyDown(KeyCode.D))
            //{
            //    List<CardsData> cardData = new List<CardsData>();
            //    CardsData zhuangInfo = new CardsData(CardResultType.Zhuang, 5, false, 3, false, new int[] { 26, 20, 17 });
            //    CardsData xianInfo = new CardsData(CardResultType.Xian, 7, false, 3, false, new int[] { 21, 18, 17 });
            //    cardData.Add(zhuangInfo);
            //    cardData.Add(xianInfo);
            //    Data.CardInfos = cardData.ToList();
            //    foreach (var cardsInfo in cardData)
            //    {
            //        Facade.EventCenter.DispatchEvent(cardsInfo.CardsType, cardsInfo);
            //    }
            //}
            //if (Input.GetKeyDown(KeyCode.W))
            //{
            //    List<CardsData> cardData = new List<CardsData>();
            //    CardsData zhuangInfo = new CardsData(CardResultType.Zhuang, 5, false, 2, false, new int[] { 26, 20, 0 });
            //    CardsData xianInfo = new CardsData(CardResultType.Xian, 7, false, 2, false, new int[] { 21, 18, 0 });
            //    cardData.Add(zhuangInfo);
            //    cardData.Add(xianInfo);
            //    Data.CardInfos = cardData.ToList();
            //    foreach (var cardsInfo in cardData)
            //    {
            //        Facade.EventCenter.DispatchEvent(cardsInfo.CardsType, cardsInfo);
            //    }
            //}

            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //    TrendData data = new TrendData();
            //    data.Win = "z";
            //    data.ZhuangDui = true;
            //    data.XianDui = true;
            //    Facade.EventCenter.DispatchEvent(LocalRequest.SingleRecord, data);
            //}
            //if (Input.GetKeyDown(KeyCode.Z))
            //{
            //    var trend = new TrendData().SetTrendData(1);
            //    Facade.EventCenter.DispatchEvent(LocalRequest.SingleRecord, trend);
            //}
            //if (Input.GetKeyDown(KeyCode.X))
            //{
            //    var trend = new TrendData().SetTrendData(2);
            //    Facade.EventCenter.DispatchEvent(LocalRequest.SingleRecord, trend);
            //}
            //if (Input.GetKeyDown(KeyCode.H))
            //{
            //    var trend = new TrendData().SetTrendData(4);
            //    Facade.EventCenter.DispatchEvent(LocalRequest.SingleRecord, trend);
            //}
            //if (Input.GetKeyDown(KeyCode.Q))
            //{
            //    var testDatas = new List<TrendData>()
            //    {
            //        new TrendData().SetTrendData(2),
            //        new TrendData().SetTrendData(2),
            //        new TrendData().SetTrendData(2),
            //        new TrendData().SetTrendData(4),
            //        new TrendData().SetTrendData(1),
            //        new TrendData().SetTrendData(1),
            //        new TrendData().SetTrendData(2),
            //        new TrendData().SetTrendData(1),
            //        new TrendData().SetTrendData(1),
            //        new TrendData().SetTrendData(2),
            //        new TrendData().SetTrendData(2),
            //        new TrendData().SetTrendData(1),
            //        new TrendData().SetTrendData(4),
            //        new TrendData().SetTrendData(1),
            //        new TrendData().SetTrendData(1),
            //        new TrendData().SetTrendData(1),
            //        new TrendData().SetTrendData(1),
            //        new TrendData().SetTrendData(1),
            //        new TrendData().SetTrendData(1),
            //        new TrendData().SetTrendData(1),
            //        new TrendData().SetTrendData(1),
            //        new TrendData().SetTrendData(1),
            //        new TrendData().SetTrendData(2),
            //        new TrendData().SetTrendData(1),
            //        new TrendData().SetTrendData(1),
            //        new TrendData().SetTrendData(2)
            //    };
            //    Facade.EventCenter.DispatchEvent(LocalRequest.Records, testDatas);
            //}

            //if (Input.GetKeyDown(KeyCode.W))
            //{
            //    var testDatas = new List<TrendData>()
            //    {
            //        new TrendData().SetTrendData(1),
            //        new TrendData().SetTrendData(1),
            //        new TrendData().SetTrendData(1),
            //        new TrendData().SetTrendData(4),
            //        new TrendData().SetTrendData(1),
            //        new TrendData().SetTrendData(2),
            //        new TrendData().SetTrendData(2),
            //        new TrendData().SetTrendData(1),
            //        new TrendData().SetTrendData(1),
            //        new TrendData().SetTrendData(1),
            //        new TrendData().SetTrendData(2),
            //        new TrendData().SetTrendData(2),
            //        new TrendData().SetTrendData(2),
            //        new TrendData().SetTrendData(2),
            //        new TrendData().SetTrendData(2),
            //        new TrendData().SetTrendData(1)
            //    };
            //    Facade.EventCenter.DispatchEvent(LocalRequest.Records, testDatas);
            //}


            //if (Input.GetKeyDown(KeyCode.S))
            //{

            //}
            //if (Input.GetKeyDown(KeyCode.C))
            //{
            //    var testDatas = new List<TrendData>();
            //    Facade.EventCenter.DispatchEvent(LocalRequest.Records, testDatas);
            //}
            //if (Input.GetKeyDown(KeyCode.U))
            //{
            //    App.GetRServer<NbjlGameServer>().ApplyBanker();
            //}
            //if (Input.GetKeyDown(KeyCode.D))
            //{
            //    App.GetRServer<NbjlGameServer>().ApplyQuitBanker();

            //}
        }
    }
    /// <summary>
    /// 服务器交互
    /// </summary>
    public enum ServerRequest
    {
        ClearRecord=0,                              //清除走势信息
        ReqApplyBanker = 101,                       //申请上庄
        ReqApplyQuitBanker = 102,                   //申请下庄
        ReqBankerList = 103,                        //庄家列表
        ReqBankerWithGold = 104,                     //上庄带钱
        ReqBeginBet = 105,                          //开始下注
        ReqEndBet = 106,                            //停止下注
        ReqBet = 107,                               //下注请求
        ReqGiveCards = 108,                         //显示结果（翻牌）
        ReqResult = 109,                            //显示结果(金币变化)
        FlushBet = 110,                             //流式下注
        ChinaUnicom=10010,                          //初始化牌局
        ChinaMobile=10086,                          //貌似会发玩家列表（Users）
    }

    /// <summary>
    /// 本地消息
    /// </summary>
    public enum LocalRequest
    {
        /// <summary>
        /// 申请下注
        /// </summary>
        ReqBet,
        /// <summary>
        /// 开始下注阶段
        /// </summary>
        ReqBeginBet,
        /// <summary>
        /// 结束下注
        /// </summary>
        ReqEndBet,
        /// <summary>
        /// 显示牌提示
        /// </summary>
        ReqGiveCards,
        /// <summary>
        /// 分数结果
        /// </summary>
        ReqResult,
        /// <summary>
        /// 上庄信息
        /// </summary>
        BankerInfo,
        /// <summary>
        /// 申请上庄状态变化
        /// </summary>
        ApplyStateChange,
        /// <summary>
        /// 当前玩家上庄状态
        /// </summary>
        BankerState,
        /// <summary>
        /// 上庄限制
        /// </summary>
        BankerLimit,
        /// <summary>
        /// 等待上庄列表
        /// </summary>
        BankerWaitList,
        /// <summary>
        /// 在线玩家信息
        /// </summary>
        OnLinePlayerList,
        /// <summary>
        /// 累计统计局数
        /// </summary>
        AccumulateCount,
        /// <summary>
        /// 玩家信息变化
        /// </summary>
        PlayerInfos,
        /// <summary>
        /// 初始化标记
        /// </summary>
        Init,
        /// <summary>
        /// 显示CD 标识
        /// </summary>
        Cd,
        /// <summary>
        /// 下注筹码
        /// </summary>
        AnteRate,
        /// <summary>
        /// 下注区域倍率
        /// </summary>
        RateValues,
        /// <summary>
        /// 回放信息
        /// </summary>
        Records,
        /// <summary>
        /// 单局回放数据
        /// </summary>
        SingleRecord,
        /// <summary>
        /// 珠盘路批量数据
        /// </summary>
        BeadRoadList,
        /// <summary>
        /// 大路图批量走势
        /// </summary>
        BigRoadList,
        /// <summary>
        /// 庄家两张牌
        /// </summary>
        LeisureDouble,
    }

    /// <summary>
    /// 游戏状态
    /// </summary>
    public enum GameStatue
    {
        /// <summary>
        /// 等待阶段
        /// </summary>
        Wait,
        /// <summary>
        /// 游戏开始阶段
        /// </summary>
        GameStart,
        /// <summary>
        /// 庄家带钱进入阶段
        /// </summary>
        BankerGold,
        /// <summary>
        /// 下注阶段
        /// </summary>
        Bet,
        /// <summary>
        /// 显示牌结果阶段
        /// </summary>
        CardResult, 
        /// <summary>
        /// 显示输赢结果阶段
        /// </summary>
        Account,
    }
}