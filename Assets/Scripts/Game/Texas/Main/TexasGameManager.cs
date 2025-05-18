using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.Texas.Mgr;
using Assets.Scripts.Game.Texas.skin01;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.ConstDefine;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Game.Texas.Main
{
    public class TexasGameManager : YxGameManager
    {
        /// <summary>
        /// 大盲注
        /// </summary>
        [HideInInspector]
        public int BigBanker = -1;
        /// <summary>
        /// 小盲注
        /// </summary>
        [HideInInspector]
        public int SmallBanker = -1;
        /// <summary>
        /// 公共牌位置
        /// </summary>
        public GameObject[] PublicPokerPos;
        /// <summary>
        /// 公共牌
        /// </summary>
        [HideInInspector]
        public readonly List<PokerCard> PublicPokers = new List<PokerCard>();
        /// <summary>
        /// 结算数据 key为Idx 只存赢家
        /// </summary>
        [HideInInspector]
        public Dictionary<int, List<ISFSObject>> ResultData = new Dictionary<int, List<ISFSObject>>();
        /// <summary>
        /// 结算的总时长
        /// </summary>
        [HideInInspector]
        public int ResultCd;
        #region 管理者
        public BetMgr BetMagr;
        public DealerMgr DealerMagr;
        public RModelMgr RModelMagr;
        public SpeakMgr SpeakMagr;
        public EffectsMgr EffectsMagr;
        public GetGoldMgr GetGoldMagr;
        public MenuMgr MenuMgr;
        #endregion
        /// <summary>
        /// 是否正在结算
        /// </summary>
        public bool IsResultDone;

        public HistoryResultMgr HistoryResultMgr;

        public RuleView RuleView;

        public TotalScoreView TotalScoreView;

        protected override void OnStart()
        {
            base.OnStart();
            //根据平台改scalingStyle 
            GetComponent<UIRoot>().scalingStyle =
                Application.platform == RuntimePlatform.WindowsPlayer ?
                    UIRoot.Scaling.Constrained :
                    UIRoot.Scaling.ConstrainedOnMobiles;
        }

        /// <summary>
        /// 初始化用户信息
        /// </summary>
        /// <param name="gameInfo"></param>
        public virtual void InitUser(ISFSObject gameInfo)
        {
            var gdata = App.GetGameData<TexasGameData>();
            var selfInfo = gameInfo.GetSFSObject(RequestKey.KeyUser);
            var selfSeat = gdata.SelfSeat;

            if (selfInfo.ContainsKey("cards"))
            {
                DealerMagr.SmallDeal(selfSeat, selfInfo.GetIntArray("cards"));
            }

            var userData = gameInfo.GetSFSObject("user");

            //如果游戏正在游戏中,初始化玩家下注信息
            if (gdata.GStatus > YxEGameStatus.Ready)
            {
                //下过的筹码 除当前轮
                int allBetV = 0;
                InitPanelBet(userData);
                allBetV += GetPlayerBet(userData);
                var selfPanel = gdata.GetPlayer<PlayerPanel>();
                if (selfPanel.ReadyState && selfPanel.CurGameType != PlayerGameType.Fold)
                {
                    gdata.GStatus = YxEGameStatus.PlayAndConfine;
                } 

                foreach (ISFSObject user in gameInfo.GetSFSArray("users"))
                {
                    InitPanelBet(user);
                    allBetV += GetPlayerBet(userData);
                }

                //下过筹码才有收
                if (allBetV > 0)
                {
                    BetMagr.CollectBetValue();
                }
            }
            //打开选择携带钱界面
            else if (gdata.GStatus == YxEGameStatus.Normal)
            {
                GetGoldMagr.AutoShowPanel(gdata.GetPlayerInfo());
            }

            //有这个字段,说明游戏在继续
            if (gameInfo.ContainsKey("opData"))
            {
                var opData = gameInfo.GetSFSObject("opData");
                var type = opData.GetInt(RequestKey.KeyType);
                GameResponseStatus(type, opData);

                //当前谁说话
                if (gameInfo.ContainsKey("current"))
                {
                    StartCoroutine(Speaker(gameInfo.GetInt("current")));
                }
            }

            ResultData.Clear();
        }

        private int GetPlayerBet(ISFSObject userData)
        {
            int rndGold = userData.ContainsKey("rndGold") ? userData.GetInt("rndGold") : 0;
            int ttxz = userData.ContainsKey("ttxz") ? userData.GetInt("ttxz") : 0;
            return ttxz - rndGold;
        }

        /// <summary>
        /// 初始化 房间
        /// </summary>
        public void InitRoom(ISFSObject gameInfo)
        {
            var gdata = App.GetGameData<TexasGameData>();
            //如果有maxRound，说明是开房模式
            //如果没有的话，隐藏掉内容
            if (gameInfo.ContainsKey("maxRound"))
            {
                gdata.IsRoomGame = true;
                RModelMagr.ShowRoomInfo(gameInfo);
            }
            else
            {
                gdata.IsRoomGame = false;
                RModelMagr.HideRoomInfo();
            }

            if (gameInfo.GetBool("playing"))
            {

                if (gameInfo.ContainsKey("publicCards"))
                {
                    DealerMagr.BigDeal(gameInfo.GetIntArray("publicCards"));
                }

                foreach (ISFSObject user in gameInfo.GetSFSArray("users"))
                {
                    if (user.GetBool("state"))
                    {
                        DealerMagr.SmallDeal(user.GetInt("seat"));
                    }
                }

                if (gameInfo.ContainsKey("bkp"))
                {
                    //小盲注 在下一家为大盲注
                    int bkp = gameInfo.GetInt("bkp");
                    var playerList = gdata.PlayerList;
                    var playerCount = playerList.Length;
                    var maxLen = playerCount + bkp;
                    for (var i = bkp; i < maxLen; i++)
                    {
                        var index = i % playerCount;
                        var player = gdata.GetPlayer<PlayerPanel>(index, true);
                        if (player.Info == null || !player.GetInfo<YxBaseGameUserInfo>().State)
                            continue;

                        if (SmallBanker == -1)
                        {
                            //小盲下注
                            SmallBanker = index;
                        }
                        else if (BigBanker == -1)
                        {
                            //大盲下注
                            player.Blinds.SetActive(true);
                            BigBanker = i;
                        }
                    }
                }
            }
            MenuMgr.InitOnClick();
            SetRuleView(gameInfo);
            if (TotalScoreView != null)
            {
                TotalScoreView.OnGetGameInfo(gameInfo);
            }
        }

        void SetRuleView(ISFSObject gameInfo)
        {
            if (RuleView == null) return;
            RuleView.SetRuleViewInfo(gameInfo);
        }

        void InitPanelBet(ISFSObject user)
        {
            var gdata = App.GameData;
            var player = gdata.GetPlayer<PlayerPanel>(user.GetInt("seat"), true);

            var rndGold = user.GetInt("rndGold");
            var xz = user.GetInt("ttxz") - rndGold;
            player.BetMoney = xz;
            player.PlayerBet(rndGold, false);
            if (user.GetInt("roomGold") <= 0 && player.ReadyState)
            {
                player.CurGameType = PlayerGameType.AllIn;
            }
            //服务器给的的棋牌
            if (user.GetInt("txstate") == 2)
            {
                player.CurGameType = PlayerGameType.Fold;
            }
            if (rndGold > SpeakMagr.MaxBetNum && player.ReadyState)
            {
                SpeakMagr.MaxBetNum = rndGold;
            }
        }


        private int _resultIndex;
        /// <summary>
        /// 开始结算
        /// </summary>
        public void BeginResult()
        {
            IsResultDone = true;
            _resultIndex = 0;
            InvokeRepeating("ResultSomeone", 0.4f, ResultCd > 4 ? (ResultCd - 2 / ResultData.Count + 1) : (ResultCd / ResultData.Count + 1));
        }

    /// <summary>
        /// 结算一个人
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private void ResultSomeone()
        {
            var gdata = App.GetGameData<TexasGameData>();
            gdata.IsGameStart = false;
            gdata.GStatus = YxEGameStatus.Normal;
            //初始化
            PokerResultInit();

            if (_resultIndex >= ResultData.Count)
            {
                ResultDone();
                return;
            }

            var resultData = ResultData[_resultIndex];
            var resultCount = resultData.Count;
            var selfSeat = gdata.SelfSeat;
            for (var i = 0; i < resultCount; i++)
            {
                var someone = resultData[i];
                var isGiveUp = someone.ContainsKey("isGiveUp") && someone.GetBool("isGiveUp");
                var isWinner = someone.GetInt("gold") > 0;
                int onesSeat = someone.GetInt("seat");
                var player = gdata.GetPlayer<PlayerPanel>(onesSeat, true);


                player.WinEffect.SetActive(true);
                if (someone.ContainsKey("cards"))
                {
                    var cards = someone.GetIntArray("cards");

                    player.UserBetPoker.PokerType.gameObject.SetActive(true);
                    player.UserBetPoker.PokerType.spriteName =
                        ((PokerType) someone.GetInt("type")).ToString();
                    player.UserBetPoker.PokerType.MakePixelPerfect();

                    if (isWinner)
                    {
                        player.ShowWinPoker(cards);
                    }

                    if (someone.GetInt("type") >= (int) PokerType.ct_FollHouse && !isGiveUp)
                    {
                        player.SetPlayerDepth(200);
                        foreach (var card in PublicPokers)
                        {
                            card.SetCardDepth(210);
                        }
                        EffectsMagr.PlayParticleEffect(((PokerType) someone.GetInt("type")).ToString(), 2f, true);
                    }
                }
               

                if (someone.GetInt("seat") != selfSeat || someone.GetInt("type") >= (int)PokerType.ct_FollHouse ||
                    isGiveUp) continue;
                Facade.Instance<MusicManager>().Play("win");
                EffectsMagr.PlayYouWin();
            }
            BetMagr.SendBetToWin(resultData);

            _resultIndex++;
        }

        /// <summary>
        /// 结算结束
        /// </summary>
        public void ResultDone()
        {
            PokerResultInit();

            CancelInvoke("ResultSomeone");
            //结算完成重置数据
            Reset();
            BetMagr.Reset();
            IsResultDone = false;
            YxClockManager.StopWaitPlayer();
        }

        public void CheckSelfGold()
        {
            //是否需要重新带入金币
            var gdata = App.GetGameData<TexasGameData>();
            var selfInfo = gdata.GetPlayerInfo<YxBaseGameUserInfo>();
            bool isRoomGame = gdata.IsRoomGame;
            if (selfInfo.RoomCoin < gdata.Ante * 10)
            {
                selfInfo.State = false;
                if (selfInfo.RoomCoin + selfInfo.CoinA >= gdata.Ante * 10)
                {
                    if (GetGoldMagr.AutoAdd.value)
                    {
                        GetGoldMagr.AutoMaxGold(selfInfo);
                        if (!isRoomGame)
                        {
                            App.GetRServer<TexasGameServer>().SendReadyGame();
                        }
                        else
                        {
                            gdata.GetPlayer<PlayerPanel>().ReadyState = false;
                        }
                    }
                    else
                    {
                        //打开选择携带钱界面
                        GetGoldMagr.OpenPanel(selfInfo);
                    }
                }
                else if (!isRoomGame)
                {
                    YxMessageBox.Show(new YxMessageBoxData()
                    {
                        Title = "提示",
                        Msg = "金币不足,请充值后再战!",
                        BtnStyle = YxMessageBox.MiddleBtnStyle,
                        IsTopShow = true,
                        Listener = (box, btnName) =>
                        {
                            if (btnName == YxMessageBox.BtnMiddle)
                            {
                                App.QuitGame();
                            }
                        },
                    });
                }
            }
        }

        /// <summary>
        /// 牌型结算初始化
        /// </summary>
        public void PokerResultInit()
        {
            foreach (var poker in PublicPokers)
            {
                poker.InitSelect();
            }
            var gdata = App.GameData;
            var playerList = gdata.PlayerList;
            var playerCount = playerList.Length;
            for (var i = 0; i < playerCount; i++)
            {
                var player = gdata.GetPlayer<PlayerPanel>(i);
                player.WinEffect.SetActive(false);
                player.UserBetPoker.PokerType.gameObject.SetActive(false);

                var pcLeft = player.UserBetPoker.LeftPoker;
                if (pcLeft != null)
                {
                    pcLeft.InitSelect();
                }
                var pcRight = player.UserBetPoker.RightPoker;
                if (pcRight != null)
                {
                    pcRight.InitSelect();
                }

                if (player.GetComponent<UIWidget>().depth > 100)
                {
                    var wi = player.GetComponent<UIWidget>();
                    player.SetPlayerDepth(100 - wi.depth);
                }

                foreach (var card in PublicPokers)
                {
                    card.SetCardDepth(100);
                }
            }
        }


        public bool IsPublic(int pokerVal)
        {
            foreach (var item in PublicPokers)
            {
                if (item.Id == pokerVal)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            YxClockManager.StopWaitPlayer();
            foreach (PokerCard publicPoker in PublicPokers)
            {
                DestroyImmediate(publicPoker.gameObject);
            }
            var gdata = App.GetGameData<TexasGameData>();
            gdata.GStatus = YxEGameStatus.Normal;
            PublicPokers.Clear();
            ResultData.Clear();
            SpeakMagr.Reset();
            DealerMagr.Reset();
            SmallBanker = -1;
            BigBanker = -1;

            var playerList = gdata.PlayerList;
            var playerCount = playerList.Length;
            for (var i = 0; i < playerCount; i++)
            {
                var player = gdata.GetPlayer<PlayerPanel>(i);
                player.Reset();
            }

            foreach (var pokerPos in PublicPokerPos)
            {
                pokerPos.transform.DestroyChildren();
            }
            StopAllCoroutines();
        }

        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            BetMagr.Reset();
            //初始化玩家信息
            InitUser(gameInfo);
            //初始化房间信息
            InitRoom(gameInfo);
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
        }

        public override void GameStatus(int status, ISFSObject info)
        {
        }

       

        public override void OnOtherPlayerJoinRoom(ISFSObject gameInfo)
        {
            base.OnOtherPlayerJoinRoom(gameInfo);
            if (App.GetGameData<TexasGameData>().IsRoomGame && TotalScoreView != null)
            {
                TotalScoreView.OnOtherJionin(gameInfo);
            }
        }


        public override void UserOut(int localSeat, ISFSObject responseData)
        {
            base.UserOut(localSeat, responseData);
            int serverSeat = responseData.GetInt(RequestKey.KeySeat);
            if (App.GetGameData<TexasGameData>().IsRoomGame && TotalScoreView != null)
            {
                TotalScoreView.OnUserOut(serverSeat);
            }
        }

        public override int OnChangeRoom()
        {
            base.OnChangeRoom();
            YxClockManager.StopWaitPlayer();
            var panelList = App.GameData.PlayerList;
            foreach (var panel in panelList)
            {
                panel.Info = null;
                panel.gameObject.SetActive(false);
            }
            return base.OnChangeRoom();
        }


        public override void GameResponseStatus(int type, ISFSObject response)
        {
            var gdata = App.GetGameData<TexasGameData>();
            switch ((GameRequestType)type)
            {
                case GameRequestType.Bet:

                    var bseat = response.GetInt("seat");
                    var bgold = response.GetInt("gold");
                    if (bseat == gdata.SelfSeat)
                    {
                        gdata.GStatus = YxEGameStatus.PlayAndConfine;
                    }
                    var bplayer = gdata.GetPlayer<PlayerPanel>(bseat, true);
                    bplayer.PlayerBet(bgold);
                    YxClockManager.StopToCountDown();
                    SpeakMagr.PoolNum += bgold;

                    if (bplayer.CurGameType != PlayerGameType.AllIn && bplayer.BetMoney > SpeakMagr.MaxBetNum)
                    {
                        Facade.Instance<MusicManager>().Play("addBet");
                    }
                    else if (bplayer.CurGameType != PlayerGameType.AllIn && bplayer.BetMoney == SpeakMagr.MaxBetNum)
                    {
                        bool isCall = bgold > 0;
                        Facade.Instance<MusicManager>().Play(isCall
                            ? "call"
                            : "seePoker");
                        bplayer.CurGameType = isCall ? PlayerGameType.Call : PlayerGameType.SeePoker;
                    }

                    //设置最大下注
                    SpeakMagr.MaxBetNum = bplayer.BetMoney >
                                          SpeakMagr.MaxBetNum
                        ? bplayer.BetMoney
                        : SpeakMagr.MaxBetNum;

                    YxDebug.Log("玩家 " + bseat + " 下注 : " + bgold);

                    if (response.ContainsKey("speaker"))
                    {
                        StartCoroutine(Speaker(response.GetInt("speaker"),
                            response.ContainsKey("cd")
                                ? response.GetInt("cd")
                                : App.GetGameData<TexasGameData>().SpeakCd));
                    }

                    break;

                case GameRequestType.Fold:
                    {
                        var fseat = response.GetInt("seat");
                        var fplayer = gdata.GetPlayer<PlayerPanel>(fseat, true);
                        YxClockManager.StopWaitPlayer();
                        fplayer.CurGameType = PlayerGameType.Fold;
                        //fplayer.ReadyState = false;
                        Facade.Instance<MusicManager>().Play("fold");
                        if (fseat == gdata.SelfSeat)
                        {
                            SpeakMagr.ShowNothing();
                            gdata.GStatus = YxEGameStatus.Normal;
                        }
                        YxDebug.Log("玩家 " + fseat + " 弃牌!!");
                       
                        if (response.ContainsKey("speaker"))
                        {
                            StartCoroutine(Speaker(response.GetInt("speaker"),
                                response.ContainsKey("cd")
                                    ? response.GetInt("cd")
                                    : App.GetGameData<TexasGameData>().SpeakCd));
                        }
                    }
                    break;

                case GameRequestType.HandCard:
                    //游戏开始
                    gdata.IsGameStart = true;

                    //设置自己的状态
                    var self = gdata.GetPlayer<SelfPanel>();
                    self.SetReadyBtnActive(false);

                    //游戏开始要隐藏微信邀请按钮
                    if (gdata.IsRoomGame)
                    {
                        RModelMagr.OnGameStart();
                        gdata.IsPlayed = true;
                    }

                    //下大小盲注时,游戏开始
                    if (response.ContainsKey("bkp"))
                    {
                        int round = ++gdata.CurRound;

                        RModelMagr.UpDataRoundValue(round);
                        IsResultDone = false;

                        //小盲注 在下一家为大盲注
                        var bkp = response.GetInt("bkp");
                       
                        if (gdata.GetPlayer().ReadyState)
                        {
                            gdata.GStatus = self.ReadyState ? YxEGameStatus.PlayAndConfine : YxEGameStatus.Play;
                        }

                        var playerList = gdata.PlayerList;
                        var playerCount = playerList.Length;
                        var maxCount = playerCount + bkp;
                        for (var i = bkp; i < maxCount; i++)
                        {
                            var index = i % playerCount;
                            var player = gdata.GetPlayer<PlayerPanel>(index, true);
                            if (player.Info == null || !player.ReadyState)
                            {
                                continue;
                            }

                            if (SmallBanker == -1)
                            {
                                //小盲下注
                                var xmz = player.RoomCoin >= gdata.Ante
                                    ? gdata.Ante : player.RoomCoin;
                                player.PlayerBet(xmz);
                                SmallBanker = index;
                                SpeakMagr.MaxBetNum = xmz;
                                SpeakMagr.PoolNum += xmz;
                            }
                            else if (BigBanker == -1)
                            {
                                //大盲下注
                                var dmz = player.RoomCoin >= gdata.Ante * 2
                                    ? gdata.Ante * 2
                                    : player.RoomCoin;
                                player.Blinds.SetActive(true);
                                player.PlayerBet(dmz);
                                BigBanker = i;
                                SpeakMagr.MaxBetNum = dmz > SpeakMagr.MaxBetNum
                                    ? dmz
                                    : SpeakMagr.MaxBetNum;
                                SpeakMagr.PoolNum += dmz;
                            }
                        }

                        //获得手牌
                        if (response.ContainsKey(RequestKey.KeyCards))
                        {
                            DealerMagr.BeginSmallDeal(SmallBanker, response.GetIntArray(RequestKey.KeyCards));
                        }

                        if (HistoryResultMgr != null)
                        {
                            HistoryResultMgr.CreateHistoryItem(response);
                        }
                    }
                    else
                    {
                        SpeakMagr.MaxBetNum = 0;     //每轮开始清除最大下注
                        
                        BetMagr.Invoke("CollectBet", 0.5f);

                        //发公共牌
                        DealerMagr.BeginBigDeal(response.GetIntArray(RequestKey.KeyCards));

                        if (response.ContainsKey("speaker"))
                        {
                            StartCoroutine(Speaker(response.GetInt("speaker"), gdata.SpeakCd,0.7f));
                        }
                        HideAllPlayersGameType();
                    }

                    break;


                case GameRequestType.Request:
                    {
                        gdata.GStatus = YxEGameStatus.Normal;
                        SpeakMagr.ShowNothing();
                        HideAllPlayersGameType();
                        YxClockManager.StopWaitPlayer();

                        //每轮开始清除最大下注
                        BetMagr.CollectBet();

                        var players = response.GetSFSArray("players");

                        ResultCd = response.GetInt("cd");

                        if (response.ContainsKey("croomct"))
                        {
                            RModelMagr.CalibrationTime((int)response.GetLong("croomct"));
                        }

                        var idxIndex = 0;
                        var playerDataCount = players.Count;
                        for (var i = 0; i < playerDataCount; i++)
                        {
                            var playerData = players.GetSFSObject(i);
                            //没有参与游戏的玩家直接跳过
                            if (!playerData.ContainsKey("isPlayed") || !playerData.GetBool("isPlayed"))
                                continue;
                            //弃牌玩家不会设置数据
                            bool isFold = playerData.ContainsKey("isGiveUp") && playerData.GetBool("isGiveUp");
                            if (isFold) continue;

                            //座位号
                            var seat = playerData.GetInt("seat");
                       
                            //玩家输赢
                            var winGold = Math.Abs(playerData.GetInt("xz")) + playerData.GetInt("gold");

                            var p = gdata.GetPlayer<PlayerPanel>(seat, true);
                            p.Blinds.SetActive(false);
                            if (seat != gdata.SelfSeat)
                            {
                                var cardArray = playerData.GetIntArray("cardArr");
                                p.ShowHandCards(cardArray);
                                p.UserBetPoker.TurnOverCard();
                            }

                            if (p.Info == null) continue;
                           
                            if (playerData.ContainsKey("roomGold"))
                            {
                                p.RoomCoin = playerData.GetInt("roomGold");
                            }

                            if (winGold <= 0) continue;

                            ISFSObject teax;
                            if (playerData.ContainsKey("teax"))
                            {
                                teax = playerData.GetSFSObject("teax");
                            }
                            else
                            {
                                teax = new SFSObject();
                                teax.PutInt("idx", idxIndex);
                                idxIndex++;
                            }
                            teax.PutInt("seat", seat);
                            teax.PutInt("gold", winGold);
                            teax.PutBool("isGiveUp", false);
                            teax.PutIntArray("cardArr", playerData.GetIntArray("cardArr"));

                            ////赢家拿钱
                            var teaxIndex = teax.GetInt("idx");
                            var wins = ResultData.ContainsKey(teaxIndex) ? ResultData[teaxIndex] : new List<ISFSObject>();
                            wins.Add(teax);
                            ResultData[teaxIndex] = wins;
                        }

                        if (HistoryResultMgr != null)
                        {
                            if (response.ContainsKey("curTime"))
                            {
                                HistoryResultMgr.SetHistoryItemTime(response.GetUtfString("curTime"));
                            }
                            HistoryResultMgr.GetHistoryInfo(response); //获取战绩信息
                        }

                        BeginResult();
                        if (TotalScoreView != null)
                        {
                            TotalScoreView.OnGetGameResultInfo(response);
                        }
                    }
                    break;

                case GameRequestType.Speaker:
                    var speakCd = response.GetInt("cd");
                    gdata.SpeakCd = speakCd;
                    StartCoroutine(Speaker(response.GetInt("speaker"), speakCd));
                    break;

                case GameRequestType.SetGold:

                    var setPlayer = gdata.GetPlayer<PlayerPanel>(response.GetInt("seat"), true);
                    var gold = response.GetInt(RequestKey.KeyGold);
                    setPlayer.Coin -= gold - setPlayer.RoomCoin;
                    setPlayer.RoomCoin = gold;
                    break;

                case GameRequestType.AllowReady:
                    gdata.GStatus = YxEGameStatus.Normal;
                    gdata.IsGameStart = false;
                    //清空准备列表开始接收准备信息
                    //如果仍在结算则提前结算完成
                    {
                        if (IsResultDone)
                        {
                            ResultDone();
                        }
                        var playerList = gdata.PlayerList;
                        var playerCount = playerList.Length;
                        for (var i = 0; i < playerCount; i++)
                        {
                            var player = gdata.GetPlayer<PlayerPanel>(i);
                            if (player.Info != null && player.RoomCoin <= 0)
                            {
                                player.ReadyState = false;
                            }
                        }

                        CheckSelfGold();
                    }
                    break;

                default:
                    YxDebug.Log("不存在的服务器交互!");
                    break;
            }
        }

        private void HideAllPlayersGameType()
        {
            var gdata = App.GameData;
            foreach (var localSeat in gdata.UserInfoDict.Keys)
            {
                var panel = gdata.GetPlayer<PlayerPanel>(localSeat);
                panel.HideGameType();
            }
        }

        public override void BeginNewGame(ISFSObject sfsObject)
        {
            base.BeginNewGame(sfsObject);
            if (App.GetGameData<TexasGameData>().IsRoomGame && RModelMagr != null)
            {
                RModelMagr.BeginCountDown();
            }
        }

        /// <summary>
        /// 显示指定玩家说话
        /// </summary>
        /// <param name="speaker"></param>
        /// <param name="cd"></param>
        /// <param name="waitTime"></param>
        IEnumerator Speaker(int speaker, float cd = -1, float waitTime = 0f)
        {
            yield return new WaitForSeconds(waitTime);
            var gdata = App.GetGameData<TexasGameData>();
            
            YxClockManager.BeginWaitPlayer(speaker, cd <= 0 ? gdata.SpeakCd : cd);
            if (speaker == gdata.SelfSeat)
            {
                SpeakMagr.ShowSpeak();
            }
            else
            {
                SpeakMagr.ShowSelfType();
            }
        }
    }

    public enum PokerType
    {
        ct_HighCard,        //高牌
        ct_Pair,            //一对
        ct_TwoPair,         //两对
        ct_ThreeOfAKind,    //三条
        ct_Straight,        //顺子
        ct_Flush,           //同花
        ct_FollHouse,       //葫芦
        ct_Four,            //四条
        ct_StraightFlush,   //同花顺
        ct_RoyalFlush,      //皇家同花顺
    }
}

