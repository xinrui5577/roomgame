using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.fillpit.ImgPress.Main;
using Assets.Scripts.Game.fillpit.Mgr;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.fillpit
{
    public class FillpitGameManager : YxGameManager
    {
        public DealerMgr DealerMgr;

        public DismissRoomMgr DismissRoomMgr;

        public SpeakMgr SpeakMgr;

        public BetMgr BetMgr;

        public LaddyMgr LaddyMgr;

        public MenuMgr MenuMgr;

        public RoomInfo RoomInfo = null;

        public SummaryMgr SummaryMgr;

        public AnimationMgr AnimationMgr;

        public WeiChatInvite WeiChatInvite;

        public GameObject Clock;
        /// <summary>
        /// 公共牌
        /// </summary>
        [HideInInspector]
        public List<PokerCard> PublicPokers;

        protected override void OnStart()
        {
            base.OnStart();
            GetComponent<UIRoot>().scalingStyle = Application.platform == RuntimePlatform.WindowsPlayer
                ? UIRoot.Scaling.Constrained
                : UIRoot.Scaling.ConstrainedOnMobiles;
        }


        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            InitRoom(gameInfo);

            InitUsers(gameInfo);

            InitGame(gameInfo);
            
            YxWindowManager.HideWaitFor();
        }

        /// <summary>
        /// 初始化游戏信息
        /// </summary>
        /// <param name="gameInfo"></param>
        private void InitGame(ISFSObject gameInfo)
        {
            var gdata = App.GetGameData<FillpitGameData>();
            if (gdata.IsRoomGame)
            {
                if (gameInfo.ContainsKey("hupstart"))
                {
                    DismissRoomMgr.ShowDismissOnRejion(gameInfo);
                }
            }

            if (gameInfo.ContainsKey("ttbet"))
            {
                LaddyMgr.AllBetMoney = gameInfo.GetInt("ttbet");
            }

            if (gameInfo.ContainsKey("lastdata"))
            {
                var lastData = gameInfo.GetSFSObject("lastdata");
                if (lastData.ContainsKey("follow"))
                {
                    gdata.LastBetValue = lastData.GetInt("follow");
                }
                var type = lastData.GetInt("type");
                GameResponseStatus(type, lastData);
            }

            if (Clock)
            {
                StartCoroutine("ReduceTime");
            }
        }

        /// <summary>
        /// 初始化所有玩家信息
        /// </summary>
        /// <param name="gameInfo"></param>
        private void InitUsers(ISFSObject gameInfo)
        {
            int maxPoint = -1;
            InitSelf(gameInfo, ref maxPoint);
            InitOthers(gameInfo, ref maxPoint);


            if (maxPoint < 0) return;
            var gdata = App.GameData;
            var playerList = gdata.PlayerList;
            foreach (var player in playerList)
            {
                if (player.Info == null)
                    continue;
                var panel = (PlayerPanel) player;
                if (!panel.ReadyState || panel.PlayerType == 3) continue;
                panel.SetMaxPoint(maxPoint);
                panel.ShowPointLabel();
            }
        }

        private void InitOthers(ISFSObject gameInfo,ref int maxPoint)
        {
            if (!gameInfo.ContainsKey("users")) return;
            ISFSArray users = gameInfo.GetSFSArray("users");
            foreach (ISFSObject user in users)
            {
                InitOne(user, ref maxPoint);
            }
        }

        /// <summary>
        /// 初始化自己的信息
        /// </summary>
        /// <param name="gameInfo"></param>
        /// <param name="maxPoint"></param>
        void InitSelf(ISFSObject gameInfo,ref int maxPoint)
        {
            if (!gameInfo.ContainsKey("user")) return;
            var gdata = App.GetGameData<FillpitGameData>();
            ISFSObject self = gameInfo.GetSFSObject("user");
            var selfPanel = gdata.GetPlayer<PlayerPanel>();

            InitOne(self,ref maxPoint);
            if (self.ContainsKey("cards"))
            {
                var cards = self.GetIntArray("cards");
                if (cards.Length > 0)
                {
                    int hidenCount = DealerMgr.HideN;
                    int[] tempArray = new int[hidenCount];
                    Array.Copy(cards, tempArray, hidenCount);
                    selfPanel.UserBetPoker.SetHandPokersValue(tempArray);
                }
            }

            if (self.ContainsKey("selfCV"))
            {
                selfPanel.SetAllCardPoint(self.GetInt("selfCV"));
            }

            //设置自己的准备状态
            if (gdata.IsRoomGame)
            {
                bool selfStart = selfPanel.Info.Seat == 0;
                bool couldStart = !gdata.IsPlayed && selfStart;
                bool isReady = selfPanel.ReadyState;
                bool isGameing = gdata.IsGameing;
                bool nmno = gdata.Nmno;
                selfPanel.SetStartBtnActive(couldStart && !isReady && !nmno);
                selfPanel.SetReadyBtnActive(!isGameing && !isReady && (!couldStart || nmno));
            }
            else
            {
                bool isReady = selfPanel.ReadyState;
                selfPanel.SetReadyBtnActive(!gdata.IsGameing && !isReady);
                selfPanel.SetStartBtnActive(false);
            }
        }


        /// <summary>
        /// 初始化某个玩家
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="maxPoint"></param>
        void InitOne(ISFSObject userInfo,ref int maxPoint)
        {
            var gdata = App.GetGameData<FillpitGameData>();
            int seat = userInfo.GetInt("seat");
            int localSeat = gdata.GetLocalSeat(seat);
            var panel = GetPlayerPanel(localSeat);
            if (userInfo.ContainsKey("cards"))
            {
                int[] cards = userInfo.GetIntArray("cards");
                YxDebug.LogArray(cards);
                DealerMgr.DealOnesPokers(cards, panel, localSeat);
            }
            if (userInfo.ContainsKey("state"))
            {
                bool isReady = userInfo.GetBool("state");
                panel.ReadyState = isReady;
                panel.ReadyStateFlag.SetActive(isReady && !gdata.IsGameing);
            }
            if (seat == gdata.Banker)
            {
                panel.SetBankIcon(true);
            }

            if (userInfo.ContainsKey("bet"))
            {
                panel.PlayerBet(userInfo.GetInt("bet"));
            }
            if (userInfo.ContainsKey("isgame") && !userInfo.GetBool("isgame"))
            {
                if (panel.ReadyState)
                {
                    panel.PlayerType = 3;
                    Color color = new Color(200 / 255f, 200 / 255f, 200 / 255f, 1);
                    panel.SetFoldCardColor(color);
                    panel.ShowGameType(GameRequestType.Fold);
                }
            }

            if (userInfo.ContainsKey("openCV"))
            {
                int openCv = userInfo.GetInt("openCV");
                panel.SetShownCardsPoint(openCv);
                if (openCv > maxPoint)
                    maxPoint = openCv;
            }
        }


        /// <summary>
        /// 初始化房间信息
        /// </summary>
        public virtual void InitRoom(ISFSObject gameInfo)
        {
            var gdata = App.GetGameData<FillpitGameData>();
            gdata.IsDoubleGame = gameInfo.ContainsKey("dr") && gameInfo.GetBool("dr");
            gdata.IsLanDi = gameInfo.ContainsKey("lastlandi") && gameInfo.GetBool("lastlandi");

            //获取房间配置
            if (gameInfo.ContainsKey("rid"))
            {
                gdata.IsRoomGame = true;
                bool played = gameInfo.GetInt("round") > 0;
                gdata.IsPlayed = played;
                gdata.RoomPlayed = gameInfo.ContainsKey("roomPlayed") && gameInfo.GetBool("roomPlayed");
                WeiChatInvite.SetWeiChatBtnActive(!played);
                RoomInfo.ShowRoomInfo(gameInfo);
            }

            //获取低注值
            if (gameInfo.ContainsKey("ante"))
            {
                gdata.BaseAnte = gameInfo.GetInt("ante");

                gdata.MaxPoolNum = gameInfo.GetInt("betLimit");
                if (gameInfo.GetBool("playing"))
                {
                    gdata.IsGameing = gameInfo.GetBool("playing");

                    //当前谁说话
                    if (gameInfo.ContainsKey("current"))
                    {
                        gdata.CurAnte = gameInfo.GetInt("current");
                        App.GetGameManager<FillpitGameManager>().Speaker(gameInfo.GetInt("current"));
                    }
                }
            }

            if (gameInfo.ContainsKey("cargs2"))
            {
                ISFSObject carg = gameInfo.GetSFSObject("cargs2");
                if (carg.ContainsKey("-hideN"))
                {
                    DealerMgr.HideN = int.Parse(carg.GetUtfString("-hideN"));
                }
                if (carg.ContainsKey("-turnresult"))
                {
                    SummaryMgr.ShowTurnResult = !carg.GetUtfString("-turnresult").Equals("0");
                }
            }

            BetMgr.SetAddBtns();
            MenuMgr.OnInitRoom(gameInfo);
        }


        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
        }

        public override void GameStatus(int status, ISFSObject info)
        {
        }

       

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            YxDebug.Log("Request == " + (GameRequestType)type);
            var gdata = App.GetGameData<FillpitGameData>();
            if (response.ContainsKey("anteRate"))
            {
                YxDebug.Log("===============================");
                YxDebug.LogArray(response.GetIntArray("anteRate"));
                BetMgr.SetAddBtns(response.GetIntArray("anteRate"));
            }

            if (!App.GetRServer<FillpitGameServer>().HasGetGameInfo)
            {
                return;
            }

            GameRequestType gameType = (GameRequestType)type;
            int selfSeat = gdata.SelfSeat;
            switch (gameType)
            {
                case GameRequestType.Bet:
                    
                    //当存在users时,说明下盲注,这个名游戏已经开始
                    if (response.ContainsKey("users"))
                    {
                        OnBetBlinds(response);
                        //投盲注,如果烂底,盲注值为0
                        gdata.IsLanDi = response.ContainsKey("landi") && response.GetBool("landi");
                    }

                    if (response.ContainsKey("banker"))
                    {
                        var banker = response.GetInt("banker");
                        var betPanel = GetPlayerPanel(banker, true);
                        betPanel.SetBankIcon(true);
                    }

                    //此为某个玩家下注
                    if (response.ContainsKey("seat"))
                    {
                        int betGold = response.GetInt("gold");
                        int betseat = response.GetInt("seat");

                        gdata.LastBetValue = betGold;

                        var betPanel = GetPlayerPanel(betseat, true);
                        betPanel.PlayerBet(betGold, true, 200);
                        betPanel.ShowGameType(gameType);
                        if (response.ContainsKey("speakerType"))
                        {
                            betPanel.Speak((GameRequestType) response.GetInt("speakerType"), betGold);
                        }
                        
                        LaddyMgr.OnPlayerBet(betGold);

                        if (betseat == selfSeat)
                        {
                            SpeakMgr.ShowNothing();
                        }
                    }


                    break;

                case GameRequestType.BetSpeak:
                    int bseat = response.GetInt("seat");   //当前座位号
                    Speaker(bseat, response.ContainsKey("cd") ? response.GetInt("cd") : 30f, GameRequestType.BetSpeak);
                    break;

                case GameRequestType.Card:

                    HideAllPalyerGameType(false);

                    int[] cardSeats = response.GetIntArray("seats");
                    DealerMgr.FirstSeat = response.GetInt("fs");

                    int curCard = response.GetInt("curCardRound");
                    LaddyMgr.OnDealCard(curCard);

                    //新游戏开始,第一轮发牌带上隐藏牌
                    if (curCard == 1)
                    {
                        var count = DealerMgr.HideN;
                        for (int i = 0; i < count; i++)
                        {
                            int[] pokers = new int[cardSeats.Length];
                            DealerMgr.BeginBigDeal(pokers, cardSeats);
                        }
                    }

                    //如果存在私人牌,则将牌赋值到对应手牌处
                    if (response.ContainsKey("selfCard") && response.GetIntArray("selfCard").Length >= 0)
                    {
                        int[] selfCards = response.GetIntArray("selfCard");
                        YxDebug.LogArray(selfCards);
                        GetPlayerPanel().UserBetPoker.SetHandPokersValue(selfCards);
                    }

                    //将每个玩家明牌点数显示出来
                    if (response.ContainsKey("openCV"))
                    {
                        int seatCount = cardSeats.Length;
                        var openCardValueArray = response.GetIntArray("openCV");
                        int openMax = -1;
                        foreach (var openVal in openCardValueArray)
                        {
                            if (openMax < openVal)
                            {
                                openMax = openVal;
                            }
                        }
                        int arrayLength = openCardValueArray.Length;
                        for (int i = 0; i < arrayLength; i++)
                        {
                            int cvSeat = cardSeats[i%seatCount];
                            int openVal = openCardValueArray[i];
                            var panel = GetPlayerPanel(cvSeat, true);
                            panel.SetShownCardsPoint(openVal);
                            panel.SetMaxPoint(openMax);
                            panel.ShowPointLabel();
                        }
                    }

                    if (response.ContainsKey("selfCV"))
                    {
                        gdata.GetPlayer<PlayerPanel>().SetAllCardPoint(response.GetInt("selfCV"));
                    }

                    //是否有公共牌,如果有,说明牌数少于在座人数
                    //公共牌用于发给其余多的人
                    if (response.ContainsKey("publicCard"))
                    {
                        DealerMgr.PublicCardId = response.GetInt("publicCard");
                    }

                    if (response.ContainsKey("cardsArr"))
                    {
                        ISFSArray cardsArray = response.GetSFSArray("cardsArr");
                        int arraycount = cardsArray.Count;
                        for (int i = 0; i < arraycount; i++)
                        {
                            DealerMgr.BeginBigDeal(cardsArray.GetIntArray(i), cardSeats);
                        }
                    }
                    else
                    {
                        DealerMgr.BeginBigDeal(response.GetIntArray("cards"), cardSeats);   //当有selfCard时,要将发牌重置到从第0张发
                    }
                  
                    HideAllPalyerGameType(false);
                    gdata.LastBetValue = 0;      //将最后一次下注重置,避免下次跟注时出现问题
                    break;

                case GameRequestType.Fold:
                    int foldSeat = response.GetInt("seat");
                    YxClockManager.StopWaitPlayer();
                    var foldPanel = GetPlayerPanel(foldSeat, true);
                    foldPanel.Speak(GameRequestType.Fold);

                    if (foldSeat == selfSeat)
                    {
                        SpeakMgr.ShowNothing();
                    }
                    YxDebug.Log("玩家 " + foldSeat + " 弃牌!!");
                    break;

                case GameRequestType.FollowSpeak:   //  跟注
                    int followSeat = response.GetInt("seat");

                    Speaker(followSeat, response.ContainsKey("cd") ? response.GetInt("cd") : 30f, gdata.LastBetValue > 0
                                ? GameRequestType.FollowSpeak
                                : GameRequestType.BetSpeak);
                    var followPanel = GetPlayerPanel(followSeat, true);
                    followPanel.HideGameType();
                    break;


                case GameRequestType.KickSpeak:
                    int kickSeat = response.GetInt("seat");
                    Speaker(kickSeat, response.ContainsKey("cd") ? response.GetInt("cd") : 30f,
                        GameRequestType.KickSpeak);
                    break;

                case GameRequestType.NotKick:

                    int notKickSeat = response.GetInt("seat");
                    YxClockManager.StopWaitPlayer();
                    var notkickspeaker = gdata.GetPlayer<PlayerPanel>(notKickSeat, true);
                    notkickspeaker.Speak(GameRequestType.NotKick);

                    if (notKickSeat == selfSeat)
                    {
                        SpeakMgr.ShowNothing();
                    }
                    break;

                case GameRequestType.Result:
                    DealerMgr.FastDeal();      //把堆积的牌全部发出去
                    gdata.IsGameing = false;
                    if (response.ContainsKey("result"))
                    {
                        YxClockManager.StopWaitPlayer();
                        bool isLandi = response.ContainsKey("landi") && response.GetBool("landi");
                        gdata.IsLanDi = isLandi;
                        AnimationMgr.SetResultLanDiAnim(isLandi);

                        HideAllPalyerGameType(true);

                        ISFSArray resultArray = response.GetSFSArray("result");
                        ShowPlayerCardsPoint(resultArray);
                        //是否是烂底
                        //如果烂底,直接显示结算结果,否则显示胜利动画
                        if (!isLandi)
                        {
                            ShowWinAnim(resultArray);
                        }
                        else
                        {
                            LaddyMgr.DeductHappys();
                        }
                        SummaryMgr.OnGameResult(resultArray);
                       
                       
                        //初始化玩家游戏币
                        foreach (ISFSObject resultItem in resultArray)
                        {
                            int resultSeat = resultItem.GetInt("seat");
                            var resultPanel = GetPlayerPanel(resultSeat, true);
                            resultPanel.OnGameResult(resultItem);
                        }
                    }

                    break;


                case GameRequestType.BackKick:

                    int backKickSeat = response.GetInt("seat");

                    Speaker(backKickSeat, response.ContainsKey("cd") ? response.GetInt("cd") : 30f,
                        GameRequestType.BackKick);
                    break;

                case GameRequestType.StartGame:
                    //开始游戏,服务器标记本房间已经开始游戏,所以本机必须发送ready
                    //为防止本机出现显示数据异常,以服务器信息为准
                    if (response.ContainsKey("isStart") && response.GetBool("isStart"))
                    {
                        App.GetRServer<FillpitGameServer>().ReadyGame();
                        WeiChatInvite.SetWeiChatBtnActive(false);
                    }
                    break;
            }
        }

        /// <summary>
        /// 显示玩家手牌点数
        /// </summary>
        /// <param name="userinfoArray"></param>
        private void ShowPlayerCardsPoint(ISFSArray userinfoArray)
        {
            //设置牌点数
            List<int> seatList = new List<int>();
            List<int> cardsValList = new List<int>();
            int maxVal = -1;
            foreach (ISFSObject userInfo in userinfoArray)
            {
                int seat = userInfo.GetInt("seat");
                bool isgame = userInfo.ContainsKey("isgame") && userInfo.GetBool("isgame");
                if (isgame && userInfo.ContainsKey("cardsValue"))
                {
                    //将每个玩家明牌点数显示出来
                    if (userInfo.ContainsKey("cardsValue"))
                    {
                        seatList.Add(seat);
                        var cardsValue = userInfo.GetInt("cardsValue");
                        cardsValList.Add(cardsValue);
                        if (cardsValue > maxVal)
                        {
                            maxVal = cardsValue;
                        }
                    }
                }
            }

            //显示牌点数
            int count = seatList.Count;
            for (int i = 0; i < count; i++)
            {
                int cvSeat = seatList[i % count];
                int openVal = cardsValList[i];
                var panel = GetPlayerPanel(cvSeat, true);

                panel.SetShownCardsPoint(openVal);
                panel.SetMaxPoint(maxVal);
                panel.ShowPointLabel();
            }
        }

        /// <summary>
        /// 显示游戏动画
        /// </summary>
        public void ShowWinAnim(ISFSArray userinfoArray)
        {
            var gdata = App.GetGameData<FillpitGameData>();
            if (gdata.IsLanDi) return; //烂底没赢家

            foreach (ISFSObject userInfo in userinfoArray)
            {
                if (userInfo.GetInt("win") > 0)
                {
                    int seat = userInfo.GetInt("seat");
                    var winPanel = GetPlayerPanel(seat, true);
                    winPanel.PlayerWin();
                    //飞筹码
                    BetMgr.MoveAllChipToSomewhere(winPanel.HeadPortrait.transform);
                    //双王通杀和四通通杀只能选择一个
                    if (gdata.Sfak && userInfo.ContainsKey("sameFour") && userInfo.GetBool("sameFour"))
                    {
                        AnimationMgr.ShowSfak();
                    }
                    else if (gdata.Dkak && userInfo.ContainsKey("doubleKing") && userInfo.GetBool("doubleKing"))
                    {
                        AnimationMgr.ShowDkak();
                    }
                }
            }
        }


        #region  通杀的本地算法
        ///// <summary>
        ///// 双王检测
        ///// </summary>
        ///// <param name="cards"></param>
        ///// <returns></returns>
        //private bool HaveDoubleKing(int[] cards)
        //{
        //    var gdata = App.GetGameData<FillpitGameData>();
        //    if (!gdata.Dkak)
        //        return false;

        //    bool haveBlackJoker = false;   //有小王
        //    bool haveColorJoker = false;   //有大王
        //    bool haveLaiZi = false;
        //    int laiziValue = gdata.LaiziValue;
        //    foreach (var cardVal in cards)
        //    {
        //        if (cardVal == 0x51)
        //        {
        //            haveBlackJoker = true;
        //        }
        //        else if (cardVal == 0x61)
        //        {
        //            haveColorJoker = true;
        //        }
        //        else if (cardVal == laiziValue)
        //        {
        //            haveLaiZi = true;
        //        }
        //    }
        //    bool laiziAk = (gdata.HaveLaiZi && haveLaiZi) && (haveBlackJoker || haveColorJoker);   //赖子通杀
        //    bool dkak = haveBlackJoker && haveColorJoker;     //双王通杀
        //    dkak = laiziAk || dkak;
        //    gdata.IsAllKill = dkak;       
        //    return dkak;
        //}

        ///// <summary>
        ///// 四通检测
        ///// </summary>
        ///// <param name="cards"></param>
        ///// <returns></returns>
        //private bool HaveFours(int[] cards)
        //{
        //    var gdata = App.GetGameData<FillpitGameData>();
        //    if (!gdata.Sfak)
        //        return false;

        //    int len = cards.Length;
        //    int loopTime = len - 4;    //四同减4
        //    bool laizi = gdata.HaveLaiZi;
        //    int laiziValue = gdata.LaiziValue;
        //    int beginCount = 0;

        //    for (int i = 0; i < loopTime; i++)
        //    {
        //        int cardVal = cards[i];

        //        if (laizi && cardVal == laiziValue)
        //        {
        //            beginCount++;
        //            continue;
        //        }
        //        int cardCount = beginCount;

        //        //去掉双王和特殊牌
        //        if (IsSpecial(cardVal))
        //        {
        //            continue;
        //        }

        //        cardVal %= 16;
        //        for (int j = i + 1; j < len; j++)
        //        {
        //            int matchCard = cards[j];
        //            if (IsSpecial(matchCard))
        //            {
        //                continue;
        //            }
        //            //两牌牌值
        //            if (cardVal == matchCard%16 || (laizi && matchCard == laiziValue))
        //            {
        //                cardCount ++;
        //            }
        //        }
        //        if (cardCount >= 3)
        //        {
        //            gdata.IsAllKill = true;
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        //bool IsSpecial(int cardVal)
        //{
        //    return cardVal == 0x51 || cardVal == 0x61 || cardVal == 0x71;
        //}
        #endregion



        ///// <summary>
        ///// 设置玩家状态,播放音效
        ///// </summary>
        ///// <param name="rt"></param>
        ///// <param name="seat"></param>
        //void ShowPlayerType(GameRequestType rt, int seat)
        //{
        //    var panel = GetPlayerPanel(seat, true);
        //    panel.Speak(rt);
        //}

        /// <summary>
        /// 盲注阶段
        /// </summary>
        /// <param name="data"></param>
        public void OnBetBlinds(ISFSObject data)
        {
            var gdata = App.GetGameData<FillpitGameData>();
            if (data.ContainsKey("curante"))
            {
                gdata.CurAnte = data.GetInt("curante");
            }

            if (data.ContainsKey("guoBet"))
            {
                GuoBet = data.GetInt("guoBet");
            }
            BegiBetSeatsNum = data.GetIntArray("users");       //已经准备的玩家座位号(由于是开房模式,准备人固定)

            //判断是否在此房间进行过游戏
            if (!App.GetGameData<FillpitGameData>().RoomPlayed)
            {
                var selfInfo = App.GetGameData<FillpitGameData>();
                for (int i = 0; i < BegiBetSeatsNum.Length; i++)
                {
                    //如果服务器发来的进行游戏的玩家位置有自己的位置,则设置true
                    if (BegiBetSeatsNum[i] == selfInfo.SelfSeat)
                    {
                        App.GetGameData<FillpitGameData>().RoomPlayed = true;
                    }
               
                }
            }

            AnimationMgr.PlayBeginAnim(data);
        }


        public override void BeginNewGame(ISFSObject sfsObject)
        {
            base.BeginNewGame(sfsObject);
            var gdata = App.GetGameData<FillpitGameData>();
            Reset(gdata.IsLanDi);
            gdata.IsGameStart = true;
            gdata.IsGameing = true;
            PlayersOnGameStart();
            if (gdata.IsRoomGame)
            {
                MenuMgr.OnGameBegin();
                WeiChatInvite.SetWeiChatBtnActive(false);
                RoomInfo.UpdateCurrentRound();
            }

            if (Clock)
            {
                Clock.SetActive(false);
                StopCoroutine("ReduceTime");
            }
        }

        /// <summary>
        /// 记录游戏下注的玩家
        /// </summary>
        [HideInInspector]
        public int[] BegiBetSeatsNum;

        /// <summary>
        /// 底注信息
        /// </summary>
        [HideInInspector]
        public int GuoBet = -1;

        public void PlayersGuoBet()
        {
            if (GuoBet > 0)
            {
                foreach (int seatNum in BegiBetSeatsNum)
                {
                    GetPlayerPanel(seatNum, true).PlayerBet(GuoBet);
                    LaddyMgr.OnPlayerBet(GuoBet); //设置荷官前的筹码显示
                }
            }
        }


        PlayerPanel GetPlayerPanel(int seat = -1, bool isServerSeat = false)
        {
            return App.GetGameData<FillpitGameData>().GetPlayer<PlayerPanel>(seat, isServerSeat);
        }

        /// <summary>
        /// 设置所有玩家的开始游戏状态
        /// </summary>
        protected void PlayersOnGameStart()
        {
            var players = App.GetGameData<FillpitGameData>().PlayerList;
            foreach (var player in players)
            {
                ((PlayerPanel)player).OnGameStart();
            }
        }

        /// <summary>
        /// 除了放弃的玩家外,其他玩家的状态都隐藏
        /// </summary>
        /// <param name="hideFold">是否隐藏弃牌状态</param>
        public void HideAllPalyerGameType(bool hideFold)
        {
            var players = App.GetGameData<FillpitGameData>().PlayerList;
            for (int i = 0; i < players.Length; i++)
            {
                var panel = (PlayerPanel)players[i];
                if (!hideFold && panel.PlayerType == 3)
                {
                }
                else
                {
                    panel.PlayerType = -1;
                    panel.HideGameType();
                }
            }
        }


        /// <summary>
        /// 显示指定玩家说话
        /// </summary>
        /// <param name="speaker"></param>
        /// <param name="cd"></param>
        /// <param name="rt"></param>
        public void Speaker(int speaker, float cd = -1, GameRequestType rt = GameRequestType.None)
        {
            var gdata = App.GetGameData<FillpitGameData>();
            if (gdata.GetPlayerInfo(speaker, true) == null)
            {
                YxDebug.Log("座位上没有玩家!");
                return;
            }

            //设置等待
            bool loop = cd > 300;
            cd = loop ? 35 : cd <= 0 ? gdata.SpeakCd : cd;
            YxClockManager.BeginWaitPlayer(speaker, cd, isLoop: loop);

            if (speaker == gdata.SelfSeat)
            {
                SpeakMgr.ShowSpeak(rt);
            }
        }

        public void SetClock()
        {
            if (Clock)
            {
                StartCoroutine("ReduceTime");
            }
        }

        private IEnumerator ReduceTime()
        {
            var readyCd = App.GetGameData<FillpitGameData>().ReadyCd;
            while (readyCd >= 0)
            {
                if (readyCd == 0)
                {
                    Debug.Log("readyCd"+ readyCd);
                    Clock.SetActive(false);
                    yield break;
                }
                else
                {
                    if (!Clock.activeSelf)
                    {
                        Clock.SetActive(true);
                    }
                    Clock.GetComponentInChildren<UILabel>().text = readyCd.ToString();
                    yield return new WaitForSeconds(1f);
                    readyCd--;
                }
            }
        }


        public override void OnOtherPlayerJoinRoom(ISFSObject gameinfo)
        {
            base.OnOtherPlayerJoinRoom(gameinfo);
            var gdata = App.GetGameData<FillpitGameData>();

            if (!gdata.IsRoomGame || !gdata.IsPlayed) return;
            ISFSObject user = gameinfo.GetSFSObject("user");
            int seat = user.GetInt("seat");
            var player = GetPlayerPanel(seat, true);
            player.OnPlayerRejoinRoom();
        }


        public override int OnChangeRoom()
        {
            Reset();
            return base.OnChangeRoom();
        }


        /// <summary>
        /// 重置
        /// </summary>
        public void Reset(bool isLanDi = false)
        {
            LaddyMgr.Rest(isLanDi);
            GuoBet = -1;

            foreach (PokerCard publicPoker in PublicPokers)
            {
                Destroy(publicPoker.gameObject);
            }

            PublicPokers = new List<PokerCard>();

            SpeakMgr.Reset();
            DealerMgr.Reset();
            AnimationMgr.Reset();
            SummaryMgr.Reset();

            var gdata = App.GetGameData<FillpitGameData>();
            //开放模式自动准备
            GetPlayerPanel().SetReadyBtnActive(!gdata.IsRoomGame);

            gdata.LastBetValue = 0;
            var players = gdata.PlayerList;

            BetMgr.BetParent.DestroyChildren();
            YxClockManager.StopWaitPlayer();

            foreach (var player in players)
            {
                var panel = (PlayerPanel)player;
                panel.Reset();
            }
        }

        public override void UserReady(int localSeat, ISFSObject responseData)
        {
            base.UserReady(localSeat, responseData);
            var gdata = App.GetGameData<FillpitGameData>();
            gdata.IsGameStart = false;
            gdata.GetPlayer<PlayerPanel>(localSeat).OnPlayerReady();
            if(localSeat == gdata.SelfLocalSeat)
            {
                SummaryMgr.HideBriefSum();
            }
        }

        public override void BeginReady()
        {
            base.BeginReady();
            SummaryMgr.CouldShowBriefSum = true;
            SummaryMgr.ShowBriefSum();
        }
    }
}
