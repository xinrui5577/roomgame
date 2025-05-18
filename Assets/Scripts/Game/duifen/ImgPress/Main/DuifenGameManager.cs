using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.duifen.Mgr;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable UnusedMember.Local

namespace Assets.Scripts.Game.duifen.ImgPress.Main
{
    public class DuifenGameManager : YxGameManager
    {

        public MenuMgr MenuMgr;

        public SpeakMgr SpeakMgr;

        public LaddyMgr LaddyMgr;

        public BetMgr BetMgr;

        public DealerMgr DealerMgr;

        public RoomResult RoomResult;

        public DismissRoomMgr DismissRoomMgr;

        public HistoryResultMgr HistoryResultMgr;

        [HideInInspector]
        public List<int> SpecialSeats = new List<int>();

        protected override void OnStart()
        {
            base.OnStart();
            //根据平台改scalingStyle
            transform.GetComponent<UIRoot>().scalingStyle = Application.platform == RuntimePlatform.WindowsPlayer
                ? UIRoot.Scaling.Constrained
                : UIRoot.Scaling.ConstrainedOnMobiles;

            //限制最大40帧
            Application.targetFrameRate = 40;
            //屏幕常亮
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
       

        /// <summary>
        /// 自己的对象
        /// </summary>
        [HideInInspector]
        public DuifenPlayerPanel SelfDuifenPlayer;

        /// <summary>
        /// 公共牌
        /// </summary>
        [HideInInspector]
        public List<PokerCard> PublicPokers = new List<PokerCard>();

        /// <summary>
        /// 结算数据 key为Idx 只存赢家
        /// </summary>
        [HideInInspector]
        public Dictionary<int, List<ISFSObject>> ResultData;

        /// <summary>
        /// 结算的总时长
        /// </summary>
        [HideInInspector]
        public int ResultCd = 0;

        public RoomInfo RoomInfo = null;

        /// <summary>
        /// 开始下注动画,设置Active即开始播放
        /// </summary>
        public GameObject BeginBetAnim = null;

        /// <summary>
        /// 当前回合数
        /// </summary>
        [HideInInspector]
        public int CurTurn;


        /// <summary>
        /// 初始化用户信息
        /// </summary>
        /// <param name="gameInfo"></param>
        public void InitUser(ISFSObject gameInfo)
        {
            var gdata = App.GameData;
            //初始化自己的信息
            ISFSObject selfIsfs = gameInfo.GetSFSObject(RequestKey.KeyUser);
            var selfPanel = gdata.GetPlayer<DuifenPlayerPanel>();
            RefreshSomeone(selfPanel, selfIsfs);

            SpeakMgr.RejionRefreshBtns(selfIsfs.GetInt("btnstatus"),
                gameInfo.ContainsKey("current") && gameInfo.GetInt("current") == gdata.SelfSeat);


            //初始化每个玩家的信息
            ISFSArray users = gameInfo.GetSFSArray(RequestKey.KeyUserList);
            foreach (ISFSObject user in users)
            {
                int seat = user.GetInt(RequestKey.KeySeat);
                var panel = gdata.GetPlayer<DuifenPlayerPanel>(seat, true);
                RefreshSomeone(panel, user);
            }

            ResultData = new Dictionary<int, List<ISFSObject>>();

        }

        /// <summary>
        /// 刷新玩家数据
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="user"></param>
        private void RefreshSomeone(DuifenPlayerPanel panel, ISFSObject user)
        {
            if (panel.ReadyState)
            {
                if (user.ContainsKey("state") && user.GetBool("state"))
                {
                    int localSeat = panel.LocalSeat;
                    ReadyLocalSeatList.Add(localSeat);
                }
            }

            panel.ConnectMark.SetActive(!(user.ContainsKey("network") && user.GetBool("network")));
            panel.ShowUserInfo();

            if (user.ContainsKey("systemAuto"))
            {
                bool isAuto = user.GetBool("systemAuto");
                panel.OnGetAutoInfo(isAuto);
            }


            if (!App.GetGameData<DuifenGlobalData>().IsGameing) //如果游戏结束,不接受数据
                return;

            ShowOnesState(panel, user);
            ShowOnesBetVal(panel, user);
            ShowOnesCards(panel, user);
        }

        /// <summary>
        /// 初始化玩家状态
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="user"></param>
        private void ShowOnesState(DuifenPlayerPanel panel, ISFSObject user)
        {
            if (user.ContainsKey("kaipai") && user.GetBool("kaipai"))
            {
                panel.ShowGameType(PlayerGameType.KaiPai);
            }
            if (user.ContainsKey("qili") && user.GetBool("qili"))
            {
                panel.ShowGameType(PlayerGameType.QiLi);
            }
            //没在游戏且有下注的玩家是弃牌玩家
            if (!user.GetBool("state") && user.GetInt("ttxz") > 0)
            {
                panel.ShowGameType(PlayerGameType.Fold);
                panel.PlayerType = (int)PlayerGameType.Fold;
                panel.Mask.SetActive(true);
            }
            
        }


        /// <summary>
        /// 显示玩家下注信息
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="user"></param>
        private void ShowOnesBetVal(DuifenPlayerPanel panel, ISFSObject user)
        {
            if (!user.ContainsKey("ttxz"))
                return;
            int betVal = user.GetInt("ttxz");
            if (betVal <= 0)
                return;
            BetMgr.PlayerBet(panel,betVal);
            panel.PlayerBet(betVal);
            LaddyMgr.AllBetMoney += betVal;

            panel.ShowBetLabel();
            panel.Coin = user.GetLong("ttgold") - betVal;
        }


        /// <summary>
        /// 显示玩家手牌信息
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="user"></param>
        private void ShowOnesCards(DuifenPlayerPanel panel, ISFSObject user)
        {
            panel.CleanCards();

            if (!user.ContainsKey("cards"))
                return;


            int[] cards = user.GetIntArray("cards");
            YxDebug.LogArray(cards);
            int[] hidden = new int[2];

            if (cards != null && cards.Length > 0)
            {

                if (user.ContainsKey("hidden"))
                {
                    hidden = user.GetIntArray("hidden");
                    YxDebug.LogArray(cards);
                }

                int[] allCards = new int[hidden.Length + cards.Length];
                if (panel.PlayerType != (int)PlayerGameType.Fold)
                {
                    Array.Copy(hidden, 0, allCards, 0, hidden.Length);
                    Array.Copy(cards, 0, allCards, hidden.Length, cards.Length);
                }

                DealerMgr.DealOnesPokers(allCards, panel);
                panel.ShowPointLabel();
            }
        }


        public void InitGameInfo(ISFSObject gameInfo)
        {
            var gdata = App.GetGameData<DuifenGlobalData>();
            if (gameInfo.ContainsKey("showGoldRate"))
            {
                gdata.ShowGoldRate = gameInfo.GetInt("showGoldRate");
            }
            if (gameInfo.ContainsKey("curante"))
            {
                gdata.Ante = gameInfo.GetInt("curante");
            }

            if (gameInfo.ContainsKey("playing"))
            {
                gdata.IsGameing = gameInfo.GetBool("playing");
            }

            if (gameInfo.ContainsKey("cargs2"))
            {
                BetMgr.PanDuan(gameInfo);
            }

            if (gameInfo.ContainsKey("minbet"))
            {
                SpeakMgr.MinBetValue = gameInfo.GetInt("minbet");
            }

            if (gameInfo.ContainsKey("ownerId"))
            {
                gdata.OwnerId = gameInfo.GetInt("ownerId");
            }
        }

        /// <summary>
        /// 初始化 房间
        /// </summary>
        public void InitRoom(ISFSObject gameInfo)
        {
            var gdata = App.GetGameData<DuifenGlobalData>();

            //获取房间配置
            if (gameInfo.ContainsKey("rid"))
            {
                gdata.IsRoomGame = true;
                RoomInfo.ShowRoomInfo(gameInfo);
                gdata.CurRound = gameInfo.GetInt("round");
                HistoryResultMgr.SetRoomInfo(gameInfo);
            }
            else
            {
                RoomInfo.gameObject.SetActive(false);
            }

            //获取低注值
            if (gameInfo.ContainsKey("ante"))
            {
                gdata.Ante = gameInfo.GetInt("ante");
            }

            if (gameInfo.ContainsKey("fpround"))
            {
                CurTurn = gameInfo.GetInt("fpround");
            }

            if (gameInfo.ContainsKey("maxRound"))
            {
                HistoryResultMgr.MaxCount = gameInfo.GetInt("maxRound");
            }
        }

        public GameObject OverHanderAnim;

        public GameObject BoomAnim;

        public GameObject WinAnim;

        public GameObject LostAnim;

        /// <summary>
        /// 播放过百动画
        /// </summary>
        /// <param name="gold"></param>
        public void PlayOverHundredAnim(int gold)
        {
            PlayHanderHundredOrBoomAnim(OverHanderAnim, gold, "guobai");
        }

        /// <summary>
        /// 播放炸弹动画
        /// </summary>
        /// <param name="gold"></param>
        public void PlayBoomAnim(int gold)
        {
            PlayHanderHundredOrBoomAnim(BoomAnim, gold, "zhadan");
        }

        /// <summary>
        /// 播放过百或炸弹动画
        /// </summary>
        /// <param name="anim"></param>
        /// <param name="gold"></param>
        /// <param name="soundName"></param>
        void PlayHanderHundredOrBoomAnim(GameObject anim, int gold, string soundName)
        {
            if (anim == null)
                return;
            anim.SetActive(true);
            SetFinish(anim, gold);
            Facade.Instance<MusicManager>().Play(soundName);
        }

        /// <summary>
        /// 设置动画结束后的内容
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="gold"></param>
        void SetFinish(GameObject obj, int gold)
        {
            if (gold == 0)
                return;
            Action finish;
            if (gold > 0)
                finish = PlayWinAnim;
            else
                finish = PlayLostAnim;
            obj.GetComponent<DelayHide>().Onfinish = finish;
        }

        /// <summary>
        /// 播放胜利动画
        /// </summary>
        public void PlayWinAnim()
        {
            PlayWinOrLost(WinAnim, "win");
        }

        /// <summary>
        /// 播放失败动画
        /// </summary>
        public void PlayLostAnim()
        {
            PlayWinOrLost(LostAnim, "lost");
        }

        void PlayWinOrLost(GameObject animGameObject, string soundName)
        {
            if (animGameObject == null)
                return;
            animGameObject.SetActive(true);
            animGameObject.GetComponent<DelayHide>().Onfinish = MoveBet;
            Facade.Instance<MusicManager>().Play(soundName);
        }

        void MoveBet()
        {
            BetMgr.MoveAllBetToSomeWhere();
            GiveSpecialMoney();
        }

        public void GiveSpecialMoney()
        {
            if (SpecialSeats.Count <= 0) return;
            var gdata = App.GetGameData<DuifenGlobalData>();
            for (int i = 0; i < SpecialSeats.Count; i++)
            {
                int specialSeat = SpecialSeats[i];
                //var specialPanel = SeatSort[specialSeat];
                var specialPanel = gdata.GetPlayer<DuifenPlayerPanel>(specialSeat, true);
                var players = gdata.PlayerList;
                for (int j = 0; j < players.Length; j++)
                {
                    if (j == specialSeat) continue;
                    var panel = gdata.GetPlayer<DuifenPlayerPanel>(j, true);
                    if (panel.PlayerType < 0 || panel.UserBetPoker.PokerCount < 5)
                    {
                        continue;
                    }
                    panel.GiveMoney(specialPanel, BetMgr.AddBetBtns[0].BetValue, BetFinishedType.Destroy, 0.5f, 0.7f);
                }

            }
        }

        /// <summary>
        /// 开始动画
        /// </summary>
        public void PlayBeginAnim()
        {
            if (App.GetGameData<DuifenGlobalData>().IsPlayed)
                return;

            BeginBetAnim.SetActive(true);
            Facade.Instance<MusicManager>().Play("start");
        }


        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            LaddyMgr.Rest();

            foreach (PokerCard publicPoker in PublicPokers)
            {
                if (publicPoker == null)
                    continue;
                Destroy(publicPoker.gameObject);
            }
            PublicPokers.Clear();
            CurTurn = 0;
            SpeakMgr.Reset();
            DealerMgr.Rest();

            BetMgr.BetParent.DestroyChildren();
            SpecialSeats.Clear();
            var playerList = App.GameData.PlayerList;
            foreach (var yxBaseGamePlayer in playerList)
            {
                var player = (DuifenPlayerPanel)yxBaseGamePlayer;
                player.Reset();
            }
        }

        internal void StopCountDown()
        {
            var playerList = App.GameData.PlayerList;
            foreach (var yxBaseGamePlayer in playerList)
            {
                var player = (DuifenPlayerPanel)yxBaseGamePlayer;
                player.StopCountDown();
            }
        }


        /// <summary>
        /// 除了放弃的玩家外,其他玩家的状态都隐藏
        /// </summary>
        public void HideAllPlayerGameType(bool hideFold)
        {
            var playerList = App.GameData.PlayerList;
            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < playerList.Length; i++)
            {
                var panel = (DuifenPlayerPanel)playerList[i];
                panel.HideGameType(hideFold);
            }
        }



        /// <summary>
        /// 记录游戏下注的玩家
        /// </summary>
        [HideInInspector]
        public int[] BegiBetSeatsNum = null;



        public void PlayOnesSound(string audioName, int serverSeat)
        {
            //MusicManager.Instance.PlayPlayerSound(audioName, SeatSort[localSeat].CurUserInfo.Sex);
            var gdata = App.GameData;
            var userInfo = gdata.GetPlayerInfo(serverSeat,true);
            if (userInfo == null) return;
            int sex = userInfo.SexI;
            string source = sex == 0 ? "woman" : "man";
            Facade.Instance<MusicManager>().Play(audioName, source);
        }





        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            var gdata = App.GetGameData<DuifenGlobalData>();
            gdata.IsGameInfo = false;

            //初始化房间信息
            InitGameInfo(gameInfo);

            //初始化房间信息
            InitRoom(gameInfo);

            //初始化玩家信息
            InitUser(gameInfo);

            //打开选择携带钱界面
            //GetGoldMgr.GetInstance().OpenPanel(GetGoldType.AddBet, duifenGameMain.GetInstance().SeatSort[duifenGameMain.GetInstance().SelfSeat].CurUserInfo);

            if (App.GetGameData<DuifenGlobalData>().IsRoomGame)
            {
                if (gameInfo.ContainsKey("hupstart"))
                {
                    DismissRoomMgr.ShowDismissOnRejion(gameInfo);
                }
            }


            if (gameInfo.GetBool("playing"))
            {

                if (gameInfo.ContainsKey("minbet"))
                {
                    int minBet = gameInfo.GetInt("minbet");
                    SpeakMgr.UpdateBetInfo(minBet);
                }

                //当前谁说话
                if (gameInfo.ContainsKey("current"))
                {
                    int speaker = gameInfo.GetInt("current");
                    Speaker(speaker);
                }
            }

            gdata.IsGameInfo = true;
            _qiliPlayed = false;

            YxWindowManager.HideWaitFor();
        }

        public override void OnOtherPlayerJoinRoom(ISFSObject sfsObject)
        {
            base.OnOtherPlayerJoinRoom(sfsObject);
            var gdata = App.GetGameData<DuifenGlobalData>();
            if (gdata.IsRoomGame)
            {
                var sfsData = sfsObject.GetSFSObject(RequestKey.KeyUser);
                var seat = sfsData.GetInt(RequestKey.KeySeat);
                gdata.GetPlayer<DuifenPlayerPanel>(seat, true).SetConnectMaskActive(false);
            }
        }

        public override void UserOut(int localSeat, ISFSObject responseData)
        {
            base.UserOut(localSeat, responseData);
            YxDebug.Log("seat[" + localSeat + "]离开了游戏!");

            var playerPanel = App.GetGameData<DuifenGlobalData>().GetPlayer<DuifenPlayerPanel>(localSeat);
            playerPanel.Info = null;
            playerPanel.gameObject.SetActive(false);
            if(ReadyLocalSeatList.Contains(localSeat)) ReadyLocalSeatList.Remove(localSeat);
        }

        

        public override void UserIdle(int localSeat, ISFSObject responseData)
        {
            base.UserIdle(localSeat, responseData);

            var gdata = App.GetGameData<DuifenGlobalData>();
            if (!gdata.IsGameInfo) return;
            var panel = gdata.GetPlayer<DuifenPlayerPanel>(localSeat);
            panel.SetConnectMaskActive(true);
        }


        public override void UserOnLine(int localSeat, ISFSObject responseData)
        {
            base.UserOnLine(localSeat, responseData);
            var panel = App.GetGameData<DuifenGlobalData>().GetPlayer<DuifenPlayerPanel>(localSeat);
            panel.SetConnectMaskActive(false);
        }

        public override void UserReady(int localSeat, ISFSObject responseData)
        {
            base.UserReady(localSeat, responseData);
            var readyPanel = App.GameData.GetPlayer<DuifenPlayerPanel>(localSeat);
            if (App.GameData.SelfLocalSeat == localSeat)
            {
                Reset();
            }

            if (!ReadyLocalSeatList.Contains(localSeat))
                ReadyLocalSeatList.Add(localSeat);

            readyPanel.OnUserReady();
        }


        public override void BeginNewGame(ISFSObject sfsObject)
        {
            base.BeginNewGame(sfsObject);
            var gdata = App.GetGameData<DuifenGlobalData>();
            gdata.CurRound ++;

            //初始化房间信息
            CurTurn = 1;
            RoomInfo.SetCurTurn(1);
            RoomInfo.SetCurRound(gdata.CurRound);
            RoomInfo.HideInvitBtn();

            // 游戏开始,刷新房间信息
            gdata.IsGameing = true;
            PlayBeginAnim();

            HistoryResultMgr.CreateHistoryItem(ReadyLocalSeatList.ToArray());
        }


        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
            Reset();
            _sendSpaceTimer = double.MinValue;
            OnGetGameInfo(gameInfo);
        }

        public override void GameStatus(int status, ISFSObject info)
        {

        }

        public override void GameResponseStatus(int type, ISFSObject data)
        {
            YxDebug.Log("Request == " + (GameRequestType)type);
            var gdata = App.GetGameData<DuifenGlobalData>();


            if (!gdata.IsGameInfo)
            {
                YxDebug.LogError("GameInfo还没有初始化!!");
                return;
            }

            if (data.ContainsKey("fpround"))
            {
                int turn = data.GetInt("fpround");
                if (CurTurn != turn)
                {
                    CurTurn = turn;
                    RoomInfo.SetCurTurn(turn);
                    SetTurnBet(0);
                }
            }

            var selfPanel = gdata.GetPlayer<DuifenPlayerPanel>();
            int selfSeat = gdata.SelfSeat;

            switch ((GameRequestType)type)
            {

                case GameRequestType.Bet:

                    int betGold = data.GetInt("gold");
                    if (data.ContainsKey("seat"))
                    {
                        int betseat = data.GetInt("seat");
                        var betPanel = gdata.GetPlayer<DuifenPlayerPanel>(betseat, true);
                        YxDebug.Log(" === 玩家 " + betseat + " 下注 " + betGold + " === ");
                        if (data.ContainsKey("kaipai") && data.GetBool("kaipai"))
                        {
                            betPanel.ShowGameType(PlayerGameType.KaiPai);
                            PlayOnesSound("kaipai", betseat);
                            if (betseat == selfSeat)
                            {
                                SpeakMgr.ShowNothing();
                            }
                        }
                        else
                        {
                            if (betGold > 0)
                            {
                                PlayOnesSound(SpeakMgr.TurnBet > 0 ? "call" : "bet", betseat);

                                SetTurnBet(betGold);

                                int lastBetVal = SpeakMgr.MinBetValue;
                                if (lastBetVal > 0)
                                {
                                    betPanel.ShowGameType(betGold > lastBetVal
                                        ? PlayerGameType.AddBet
                                        : PlayerGameType.Call);
                                }
                                SpeakMgr.UpdateBetInfo(betGold);
                            }
                            else
                            {
                                if (!_qiliPlayed)
                                {
                                    PlayOnesSound("qili", betseat);
                                    _qiliPlayed = true;
                                }

                                betPanel.ShowGameType(PlayerGameType.QiLi);
                            }
                        }
                        BetMgr.PlayerBet(betPanel, betGold, 110);
                        betPanel.PlayerBet(betGold);
                        betPanel.StopCountDown();

                        LaddyMgr.AllBetMoney += betGold; //总下注额

                        if (betseat == selfSeat)
                        {
                            SpeakMgr.HideSpeak(GameRequestType.Bet);
                        }

                        if (data.ContainsKey("card") && CurTurn < 3)
                        {
                            DealerMgr.BeginBigDeal(data.GetInt("card"), betseat, CurTurn + 2,
                                selfPanel.ShowSelfPointLabel);
                        }

                    }

                    if (data.ContainsKey("speaker"))
                    {
                        int speakerSeat = data.GetInt("speaker");
                        Speaker(speakerSeat, data.GetInt("cd"),
                            betGold > 0 ? GameRequestType.Bet : GameRequestType.QiLi);

                        if (speakerSeat == selfSeat)
                        {
                            _sendSpaceTimer = double.MinValue;
                            selfPanel.HideGameType();
                        }
                    }

                    break;

                case GameRequestType.Fold:
                    DealerMgr.FastDeal();

                    int fseat = data.GetInt("seat");
                    var foldPanel = App.GameData.GetPlayer<DuifenPlayerPanel>(fseat, true);
                    foldPanel.StopCountDown();

                    foldPanel.ReadyState = false;
                    foldPanel.ShowGameType(PlayerGameType.Fold);
                    foldPanel.PlayerFold();
                    PlayOnesSound("fold", fseat);

                    if (fseat == selfSeat)
                    {
                        _sendSpaceTimer = double.MinValue;
                        SpeakMgr.ShowNothing();
                    }

                    if (data.ContainsKey("speaker"))
                    {
                        Speaker(data.GetInt("speaker"), rt: GameRequestType.Fold);
                    }


                    YxDebug.Log("玩家 " + fseat + " 弃牌!!");
                    break;


                case GameRequestType.Start:

                    _qiliPlayed = false;
                    foreach (int seat in ReadyLocalSeatList)
                    {
                        var startPanel = gdata.GetPlayer<DuifenPlayerPanel>(seat);
                        startPanel.OnGameStart();
                        BetMgr.PlayerBet(startPanel, gdata.GuoBet, 100);
                        LaddyMgr.AllBetMoney += gdata.GuoBet;
                    }

                    SpeakMgr.RefreshBetSpeakBtns();

                    if (data.ContainsKey("hiden"))
                    {
                        //初始化玩家自己的手牌
                        DealerMgr.SetFirstTwoPokersValue(data.GetIntArray("hiden"));
                    }
                    break;

                case GameRequestType.Cards:

                    HideAllPlayerGameType(false);

                    if (data.ContainsKey("speaker"))
                    {
                        int speaker = data.GetInt("speaker");
                        if (speaker != selfSeat && selfPanel.PlayerType != 3)
                        {
                            SpeakMgr.CouldFold = true;
                            SpeakMgr.RefreshBetSpeakBtns();
                        }
                        else
                        {
                            SpeakMgr.CouldFold = false;
                        }
                        Speaker(speaker, data.GetInt("cd"), GameRequestType.Cards);
                    }

                    if (data.ContainsKey("cards"))
                    {
                        int[] cards = data.GetIntArray("cards");
                        int[] cardsSeats = data.GetIntArray("users");

                        HideAllPlayerGameType(false); //隐藏所有游戏状态

                        if (CurTurn <= 1)
                        {
                            DealerMgr.DealFirstTwoPoker(cardsSeats, selfSeat);
                        }
                        DealerMgr.BeginBigDeal(cards, cardsSeats, CurTurn + 1, selfPanel.ShowSelfPointLabel);
                    }

                    break;

                case GameRequestType.Request:
                    _qiliPlayed = false;
                    SpeakMgr.ShowNothing();

                    gdata.IsGameing = false;
                    gdata.GStatus = YxEGameStatus.Normal;
                    DealerMgr.FastDeal();
                    StopCountDown();
                    int selfWinGold = 0;

                    if (data.ContainsKey("players"))
                    {
                        ISFSArray requestPlayers = data.GetSFSArray("players");
                        for (int i = 0; i < requestPlayers.Count; i++)
                        {
                            ISFSObject playerInfo = requestPlayers.GetSFSObject(i);
                            if (!playerInfo.ContainsKey("isPlayed") && !playerInfo.GetBool("isPlayed"))
                                continue;

                            int requestSeat = playerInfo.GetInt("seat");
                            var requestPlayer = gdata.GetPlayer<DuifenPlayerPanel>(requestSeat, true);
                            int winGold = playerInfo.ContainsKey("wgold")
                                ? playerInfo.GetInt("wgold")
                                : playerInfo.GetInt("gold");
                            if (playerInfo.GetBool("isGiveUp"))
                            {
                                requestPlayer.PlayerType = 3;
                            }

                            if (requestPlayer.Info != null)
                            {
                                requestPlayer.Coin = playerInfo.GetLong("ttgold");
                            }

                            if (winGold > 0)
                            {
                                requestPlayer.WinAnimation.SetActive(true);
                                BetMgr.Winner = requestPlayer.transform;
                            }
                            if (requestSeat == selfSeat)
                            {
                                selfWinGold = winGold;
                            }
                        }
                        HistoryResultMgr.GetHistoryInfo(data); //获取历史记录信息
                    }

                    bool noSpecial = true;
                    if (CurTurn >= 3)
                    {

                        if (data.ContainsKey("rslist"))
                        {
                            ISFSArray rslistPlayers = data.GetSFSArray("rslist");
                            for (int i = 0; i < rslistPlayers.Count; i++)
                            {
                                ISFSObject player = rslistPlayers.GetSFSObject(i);
                                int rsSeat = player.GetInt("seat");
                                var pp = gdata.GetPlayer<DuifenPlayerPanel>(rsSeat, true);
                                if (pp.PlayerType == 3)
                                    continue;

                                int point = player.GetInt("value");

                                int[] cards = player.GetIntArray("cards");
                                pp.TurnCard(0, cards[0], false);
                                pp.TurnCard(1, cards[1], false);

                                if (cards[0] != 0 && cards[1] != 0)
                                {
                                    pp.ShowPointLabel(point);
                                }

                                if (point >= 100)
                                {
                                    PlayOverHundredAnim(selfWinGold);
                                    noSpecial = false;
                                    SpecialSeats.Add(rsSeat);
                                }
                                else if (pp.UserBetPoker.HaveBomb)
                                {
                                    PlayBoomAnim(selfWinGold);
                                    noSpecial = false;
                                    SpecialSeats.Add(rsSeat);
                                }
                            }
                        }
                    }

                    if (noSpecial)
                    {
                        if (selfWinGold > 0)
                        {
                            PlayWinAnim();
                        }
                        else if (selfWinGold < 0)
                        {
                            PlayLostAnim();
                        }
                    }

                    ReadyLocalSeatList.Clear();
                    break;

                case GameRequestType.AllowReady:
                    ReadyLocalSeatList.Clear();
                    var playerList = gdata.PlayerList;
                    foreach (var item in playerList)
                    {
                        item.ReadyState = false;
                    }
                    var self = selfPanel as SelfPanel;
                    if (self != null) self.ReadyBtn.gameObject.SetActive(true);
                    break;

                case GameRequestType.CouldStart:
                    if (!gdata.IsRoomGame)
                        break;

                    if (selfSeat == 0)
                    {
                        selfPanel.CouldStart();
                    }

                    break;

                case GameRequestType.SystemAuto:

                    int autoSeat = data.GetInt("seat");
                    var panel = gdata.GetPlayer<DuifenPlayerPanel>(autoSeat, true);
                    bool isAuto = data.GetBool("systemAuto");
                    panel.OnGetAutoInfo(isAuto);

                    break;
            }
        }

        void SetTurnBet(int val)
        {
            App.GetGameManager<DuifenGameManager>().SpeakMgr.TurnBet = val;
        }

        public IEnumerator DelayedSpeaker(int speaker, float time, float cd = -1)
        {
            yield return new WaitForSeconds(time);
            Speaker(speaker, cd);
        }
         

        private bool _qiliPlayed; //是否已经起立过

        /// <summary>
        /// 准备列表,本地坐标
        /// </summary>
        internal List<int> ReadyLocalSeatList = new List<int>();


        /// <summary>
        /// 按钮间隔计时
        /// </summary>
        private double _sendSpaceTimer = double.MinValue;

        /// <summary>
        /// 按键间隔,单位秒
        /// </summary>
        [Tooltip("游戏发送消息时间间隔,单位'秒'")]
        public float SpaceTime = 1;

        bool CouldSendRequest()
        {
            var t1 = new TimeSpan(DateTime.Now.Ticks).TotalSeconds;
            if (t1 - _sendSpaceTimer > SpaceTime)
            {
                _sendSpaceTimer = t1;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 显示指定玩家说话
        /// </summary>
        /// <param name="speaker"></param>
        /// <param name="cd"></param>
        /// <param name="rt"></param>
        public void Speaker(int speaker, float cd = -1, GameRequestType rt = GameRequestType.None)
        {
            var gdata = App.GameData;
            var speakerPanel = gdata.GetPlayer<DuifenPlayerPanel>(speaker, true);
            if (speakerPanel.Info == null)
            {
                YxDebug.Log("座位上没有玩家!");
                return;
            }
            speakerPanel.BeginCountDown(cd <= 0 ? App.GetGameData<DuifenGlobalData>().SpeakCd : cd);

            if (speaker == gdata.SelfSeat)
            {
                _sendSpaceTimer = double.MinValue;
                SpeakMgr.ShowSpeak(rt);
            }
        }

        public void SendRequest(GameRequestType rt, IDictionary data)
        {
            if (!App.GetGameData<DuifenGlobalData>().IsGameInfo)
            {
                YxDebug.LogError("GameInfo还没有初始化!!");
                return;
            }
            if (!CouldSendRequest())
            {
                YxDebug.Log("发送数据过于频繁,请稍后发送");
                return;
            }
            App.GetRServer<DuifenGameServer>().SendRequest(rt, data);
        }
    }

    /// <summary>
    /// 游戏服务交互
    /// </summary>
    public enum GameRequestType
    {
        None = -1,
        /// <summary>
        /// 1.下注
        /// </summary>
        Bet = 1,        //下注
        /// <summary>
        /// 2.弃牌
        /// </summary>
        Fold = 2,           //弃牌
        /// <summary>
        /// 3.发牌
        /// </summary>
        Cards = 3,       //发牌
        /// <summary>
        /// 4.结算
        /// </summary>
        Request = 4,        //结算
        /// <summary>
        /// 5.说话座位
        /// </summary>
        Speaker = 5,        //说话座位
        /// <summary>
        /// 6.盲注阶段
        /// </summary>
        Start = 6,        //设置携带金币
        /// <summary>
        /// 7.可以准备了
        /// </summary>
        AllowReady = 7,     //可以准备了
        /// <summary>
        /// 8.可以开始游戏
        /// </summary>
        CouldStart = 8,
        /// <summary>
        /// 9.自动状态
        /// </summary>
        SystemAuto = 9,



        /// <summary>
        /// 21.开牌,本地
        /// </summary>
        KaiPai = 21,
        /// <summary>
        /// 22.起立,本地
        /// </summary>
        QiLi = 22,
        /// <summary>
        /// 10086,新一局游戏的开始
        /// </summary>
        GameStart = 10086,
    }
}
