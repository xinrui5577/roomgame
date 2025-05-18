using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.GangWu.Mgr;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.GangWu.Main
{
    public class GangWuGameManager : YxGameManager
    {

        public ResultMgr ResultMgr;

        /// <summary>
        /// 公共牌
        /// </summary>
        [HideInInspector]
        public List<PokerCard> PublicPokers = new List<PokerCard>();
   

        public RModelMgr RModelMgr;

        public HistoryResultMgr HistoryResultMgr;

        public BetMgr BetMgr;

        public SpeakMgr SpeakMgr;

        public MenuMgr MenuMgr;

        public EffectsMgr EffectsMgr;

        public DealerMgr DealerMgr;

        protected override void OnStart()
        {
            base.OnStart();
            GetComponent<UIRoot>().scalingStyle = Application.platform == RuntimePlatform.WindowsPlayer
                ? UIRoot.Scaling.Constrained
                : UIRoot.Scaling.ConstrainedOnMobiles;
        }

        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            var gdata = App.GetGameData<GangwuGameData>();
            Reset();
            //初始化玩家信息
            InitUser(gameInfo);
            //初始化房间信息
            InitRoom(gameInfo);

            gdata.IsGameInfo = true;

            YxWindowManager.HideWaitFor();
        }

        /// <summary>
        /// 初始化用户信息
        /// </summary>
        /// <param name="gameInfo"></param>
        public void InitUser(ISFSObject gameInfo)
        {
            var gdata = App.GetGameData<GangwuGameData>();
            List<int> betList = new List<int>();


            ISFSObject selfSfs = gameInfo.GetSFSObject(RequestKey.KeyUser);
            InitSomeOne(selfSfs);       //初始化部分数据
            
            //初始化个人手牌
            var selfPanel = gdata.GetPlayer<PlayerPanel>();
            if (selfSfs.ContainsKey("cards"))
            {
                int[] selfCards = selfSfs.GetIntArray("cards");

                if (selfCards.Length > 0)
                {
                    selfPanel.UserBetPoker.LeftCardValue = selfCards[0];
                    selfPanel.UserBetPoker.RightCardValue = selfCards[1];
                }
            }

            //初始化下注信息
            if (selfSfs.ContainsKey("curRoundBet"))
            {
                var selfBet = selfSfs.GetInt("curRoundBet");
                betList.Add(selfBet);
            }


            ISFSArray users = gameInfo.GetSFSArray(RequestKey.KeyUserList);
            foreach (ISFSObject user in users)
            {
                InitSomeOne(user);
                if (!user.ContainsKey("curRoundBet")) continue;
                int bet = user.GetInt("curRoundBet");
                betList.Add(bet);
            }

            if (!gameInfo.ContainsKey("totalBet")) return;

            int ttBet = gameInfo.GetInt("totalBet");
            foreach (var oneBet in betList)
            {
                ttBet -= oneBet;
            }
            BetMgr.BigBetStack.SetBet(ttBet);
        }

        public void InitSomeOne(ISFSObject info)
        {
            var gdata = App.GetGameData<GangwuGameData>();
            int userSeat = info.GetInt(RequestKey.KeySeat);
            PlayerPanel onePanel = gdata.GetPlayer<PlayerPanel>(userSeat, true);
            onePanel.SetPlayerReadyState(info.GetBool(RequestKey.KeyState));
            foreach (Transform tran in onePanel.PokersTrans)
            {
                tran.DestroyChildren();
            }

            if (info.ContainsKey("cards"))
            {
                int[] selfCards = info.GetIntArray("cards");

                if (selfCards.Length > 0)
                {
                    onePanel.UserBetPoker.LeftCardValue = selfCards[0];
                    onePanel.UserBetPoker.RightCardValue = selfCards[1];
                    DealerMgr.DealOnesPokers(selfCards, onePanel.PokersTrans, gdata.SelfSeat);
                }
            }

            if (info.ContainsKey("curRoundBet"))
            {
                var selfBet = info.GetInt("curRoundBet");
                onePanel.BetMoney = selfBet;
            }
        }


        /// <summary>
        /// 初始化 房间
        /// </summary>
        public void InitRoom(ISFSObject gameInfo)
        {
         
            if (gameInfo.ContainsKey("rid") && RModelMgr != null)
            {
                RModelMgr.ShowRoomInfo(gameInfo);
            }
            var gdata = App.GetGameData<GangwuGameData>();
            if (gdata.IsGameStart)
            {
                //当前谁说话
                if (gameInfo.ContainsKey("current"))
                {
                    App.GetGameManager<GangWuGameManager>().Speaker(gameInfo.GetInt("current"));
                }

                //下过的筹码 除当前轮
                int allBetV = 0;

                foreach (ISFSObject user in gameInfo.GetSFSArray("users"))
                {
                    PlayerPanel player = gdata.GetPlayer<PlayerPanel>(user.GetInt("seat"), true);
                    int xz = user.GetInt("ttxz") - user.GetInt("rndGold");

                    allBetV += xz;
                    if (user.GetLong("ttgold") <= 0 && player.ReadyState)
                    {
                        player.CurGameType = PlayerGameType.AllIn;
                    }

                    //服务器给的的棋牌
                    if (user.GetInt("txstate") == 2)
                    {
                        player.CurGameType = PlayerGameType.Fold;
                    }
                    player.PlayerBet(user.GetInt("rndGold"), false);
                }
                //下过筹码才有收
                if (allBetV > 0)
                {
                    BetMgr.CollectBetValue();
                }

                if (gameInfo.ContainsKey("opData"))
                {
                    var sfs = gameInfo.GetSFSObject("opData");
                    var type = sfs.GetInt(RequestKey.KeyType);
                    App.GetGameManager<GangWuGameManager>().GameResponseStatus(type, sfs);
                }
            }

            BetMgr.SetChipsTime();      //设置筹码倍数
        }


        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
        }

        public override void GameStatus(int status, ISFSObject info)
        {
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            var gdata = App.GetGameData<GangwuGameData>();
            if (!gdata.IsGameInfo)
            {
                YxDebug.LogError("GameInfo还没有初始化!!");
                return;
            }

            switch ((GameRequestType)type)
            {
                case GameRequestType.Bet:

                    if (response.ContainsKey("seat"))
                    {
                        int bseat = response.GetInt("seat");
                        int bgold = response.GetInt("gold");
                        PlayerPanel onePanel = gdata.GetPlayer<PlayerPanel>(bseat, true);
                        if (bgold > 0)
                        {
                            if (bgold + onePanel.BetMoney == SpeakMgr.MaxBetNum)
                            {
                                onePanel.ShowGameType("call");
                            }
                            else if (onePanel.Info != null && bgold == onePanel.Coin)
                            {
                                onePanel.CurGameType = PlayerGameType.AllIn;
                                onePanel.ShowGameType("allIn");
                            }
                            else
                            {
                                onePanel.ShowGameType("");
                            }
                        }
                        else
                        {
                            onePanel.ShowGameType("seePoker");
                        }
                        onePanel.PlayerBet(bgold);

                        if (onePanel.CurGameType != PlayerGameType.AllIn
                            && onePanel.BetMoney > SpeakMgr.MaxBetNum)
                        {
                            Facade.Instance<MusicManager>().Play("addBet");
                            onePanel.ShowGameType(string.Empty);
                        }
                        else if (onePanel.CurGameType != PlayerGameType.AllIn
                                 && onePanel.BetMoney == SpeakMgr.MaxBetNum)
                        {
                            Facade.Instance<MusicManager>().Play(bgold > 0
                                ? "call"
                                : "seePoker");
                        }

                        //设置最大下注
                        SpeakMgr.MaxBetNum = onePanel.BetMoney >
                                                  SpeakMgr.MaxBetNum
                            ? onePanel.BetMoney
                            : SpeakMgr.MaxBetNum;


                        if (bseat == gdata.SelfSeat)
                        {
                            SpeakMgr.ShowAuto();
                            SpeakMgr.AddBetSum = 0;
                        }

                    }
                    //包含users说明游戏开始
                    else if (response.ContainsKey("users"))
                    {
                        ++gdata.CurRound;
                        int[] beginSeats = response.GetIntArray("users");
                        int beginBetGold = response.GetInt("guoBet");
                        gdata.Ante = response.GetInt("curante");
                        gdata.IsGameStart = true;
                        gdata.IsPlayed = true;
                        foreach (int seat in beginSeats)
                        {
                            PlayerPanel panel = gdata.GetPlayer<PlayerPanel>(seat, true);
                            if (panel.Info != null)
                            {
                                panel.SetPlayerReadyState(true);
                                panel.PlayerBet(beginBetGold);
                            }
                        }
                        HistoryResultMgr.CreateHistoryItem(beginSeats);
                        //隐藏准备按钮
                        gdata.GetPlayer<SelfPlayerPanel>().OnGameStart();
                    }
                    break;



                case GameRequestType.Fold:
                    int fseat = response.GetInt("seat");
                    PlayerPanel foldPanel = gdata.GetPlayer<PlayerPanel>(fseat, true);
                    foldPanel.CurGameType = PlayerGameType.Fold;
                    foldPanel.Mask.SetActive(true);
                    foldPanel.BetLabel.gameObject.SetActive(foldPanel.BetMoney > 0);
                    foldPanel.ShowGameType("fold");
                    if (fseat == gdata.SelfSeat)
                    {
                        SpeakMgr.ShowNothing();
                    }
                    break;


                case GameRequestType.Card:

                    if (response.ContainsKey("selfCard"))
                    {
                        int[] selfCards = response.GetIntArray("selfCard");
                        if (selfCards.Length > 0)
                        {
                            gdata.GetPlayer<PlayerPanel>().UserBetPoker.LeftCardValue = selfCards[0];
                        }
                    }

                    if (response.ContainsKey("cards"))
                    {
                        int[] cardSeats = response.GetIntArray("seats");
                        int[] cards = response.GetIntArray("cards");

                        int cardCount = response.GetInt("cardCount");
                        gdata.CardCount = cardCount;

                        //如果为cardCount是2,则是第一轮发牌,需要先发一轮暗牌
                        if (cardCount <= 2)
                        {
                            int[] tempCards = new int[cardSeats.Length];

                            for (int i = 0; i < cardSeats.Length; i++)
                            {
                                if (cardSeats[i] == gdata.SelfSeat)
                                {
                                    tempCards[i] = gdata.GetPlayer<PlayerPanel>().UserBetPoker.LeftCardValue;
                                }
                            }

                            DealerMgr.BeginBigDeal(tempCards, cardSeats, 0);
                        }

                        DealerMgr.BeginBigDeal(cards, cardSeats, cardCount - 1);

                        //初始化玩家当前状态信息
                        foreach (int seat in cardSeats)
                        {
                            gdata.GetPlayer<PlayerPanel>(seat, true).ShowGameType(string.Empty);
                        }
                    }

                    //每轮开始清除最大下注
                    SpeakMgr.MaxBetNum = 0;
                    SpeakMgr.AddBetSum = 0;
                    BetMgr.CollectBet();
                    foreach (var yxBaseGamePlayer in gdata.PlayerList)
                    {
                        var panel = (PlayerPanel)yxBaseGamePlayer;
                        panel.BetMoney = 0;
                        panel.BetLabel.text = panel.BetMoney.ToString();
                    }

                    SpeakMgr.BetRemenber.Clear();

                    break;


                case GameRequestType.Result:
                    gdata.AllocateFour = false;
                    //当接到结算时,由于要排除由于手机卡顿或者由于有玩家手上筹码过少,引起的直接结算,这里将手牌取消动画显示
                    DealerMgr.OnResult();

                    ResultMgr.OnGameResult(response.GetSFSArray("result"));
                    StartCoroutine(ShowResultView());

                    DoResult(response);

                    HistoryResultMgr.GetHistoryInfo(response);

                    YxClockManager.StopWaitPlayer();
                    break;


                case GameRequestType.BetSpeak:

                    int betSpeakSeat = response.GetInt("seat");
                    var betSpeaker = gdata.GetPlayer<PlayerPanel>(betSpeakSeat, true);

                    if (!betSpeaker.ReadyState)
                        return;

                    gdata.AllocateFour = response.ContainsKey("allocateFour") && response.GetBool("allocateFour");

                    gdata.SpeakCd = response.GetInt("cd");
                    betSpeaker.ShowGameType(string.Empty);
                    Speaker(betSpeakSeat, response.GetInt("cd"));
                    if (betSpeakSeat == gdata.SelfSeat)
                    {
                        betSpeaker.BetLabel.gameObject.SetActive(true);
                        SpeakMgr.ShowSpeak();
                    }
                    break;



                case GameRequestType.FollowSpeak:

                    int followSpeakSeat = response.GetInt("seat");
                    var followSpeaker = gdata.GetPlayer<PlayerPanel>(followSpeakSeat, true);
                    if (!followSpeaker.ReadyState)
                        return;

                    gdata.SpeakCd = response.GetInt("cd");
                    gdata.AllocateFour = response.GetBool("allocateFour");
                    followSpeaker.ShowGameType(string.Empty);

                    Speaker(followSpeakSeat, response.GetInt("cd"));
                    if (followSpeakSeat == gdata.SelfSeat)
                    {
                        followSpeaker.BetLabel.gameObject.SetActive(true);
                        SpeakMgr.ShowSpeak();
                    }
                    break;

                case GameRequestType.AllowReady:
                    gdata.AllocateFour = false;
                    gdata.IsGameStart = false;

                    _waitResult = false;   //允许显示结算
                   
                    YxClockManager.StopWaitPlayer();
                    break;

                default:
                    YxDebug.Log("不存在的服务器交互!");
                    break;
            }
        }


        private bool _waitResult = true;

        IEnumerator ShowResultView()
        {
            yield return new WaitForSeconds(1.5f);
            while (_waitResult)
            {
                yield return new WaitForFixedUpdate();
            }
            _waitResult = true;
            ResultMgr.ShowResultView();
            Reset();
        }


        /// <summary>
        /// 港式五张的结算方法
        /// </summary>
        public void DoResult(ISFSObject data)
        {
            var gdata = App.GetGameData<GangwuGameData>();

            ISFSArray isfsArr = data.GetSFSArray("result");
            List<Transform> wheres = new List<Transform>();
            foreach (ISFSObject isfsObj in isfsArr)
            {
                int win = isfsObj.GetInt("win");
                int seat = isfsObj.GetInt("seat");
                PlayerPanel panel = gdata.GetPlayer<PlayerPanel>(seat, true);

                //标记赢家的位置信息
                if (panel.Info != null)
                {
                    panel.Coin = isfsObj.GetLong("ttgold");
                    panel.RefreshPanel();
                }

                if (panel.PokersTrans[0].childCount > 0)
                {
                    int[] cards = isfsObj.GetIntArray("cards");
                    PokerCard poker = panel.PokersTrans[0].GetChild(0).GetComponent<PokerCard>();

                    poker.SetCardId(cards[0]);
                    poker.SetCardFront();
                }

                if (win > 0)
                {
                    var resultSeat = seat;
                    var pokerType = panel.UserBetPoker.PokerType;
                    if (panel.PokersTrans[panel.PokersTrans.Length - 1].childCount > 0)
                    {
                        pokerType.spriteName = "ct_" + isfsObj.GetUtfString("cardsName");
                        pokerType.MakePixelPerfect();
                        pokerType.transform.localScale = resultSeat == gdata.SelfSeat ? Vector3.one : new Vector3(1.5f, 1.5f, 1.5f);
                        pokerType.gameObject.SetActive(true);
                    }
                    panel.WinEffect.SetActive(true);

                    //如果是特殊牌型,需要播放牌型特效
                    if (EffectsMgr.NeedShowEffect(isfsObj.GetUtfString("cardsName")))
                        EffectsMgr.PlayParticleEffect("ct_" + isfsObj.GetUtfString("cardsName"), 2f, true);

                    if (resultSeat == gdata.SelfSeat)
                    {
                        Facade.Instance<MusicManager>().Play("win");
                        EffectsMgr.PlayYouWin();
                    }

                    wheres.Add(panel.UserIcon.transform);
                }
            }
            if (wheres.Count > 0)
            {
                BetMgr.BigBetStack.SendBetToSomewhere(wheres);
            }
        }
   



        /// <summary>
        /// 显示指定玩家说话
        /// </summary>
        /// <param name="speaker"></param>
        /// <param name="cd"></param>
        public void Speaker(int speaker, float cd = -1)
        {
            var speakerPanel = App.GameData.GetPlayer<PlayerPanel>(speaker, true);

            if (speakerPanel.Info == null)
            {
                YxDebug.Log("座位上没有玩家!");
                return;
            }

            YxClockManager.BeginWaitPlayer(speaker, cd <= 0
                ? App.GetGameData<GangwuGameData>().SpeakCd
                : cd);
        }

        public override void OnOtherPlayerJoinRoom(ISFSObject sfsObject)
        {
            var gdata = App.GetGameData<GangwuGameData>();
            if (!gdata.IsGameInfo)
            {
                YxDebug.LogError("GameInfo还没有初始化!!");
                return;
            }

            base.OnOtherPlayerJoinRoom(sfsObject);
            var data = sfsObject.GetSFSObject(RequestKey.KeyUser);
            int seat = data.GetInt(RequestKey.KeySeat);
            var panel = gdata.GetPlayer<PlayerPanel>(seat, true);
            panel.SetPlayerReadyState(false);
        }

        public override void BeginReady()
        {
            base.BeginReady();
            var playerList = App.GetGameData<GangwuGameData>().PlayerList;
            foreach (var play in playerList)
            {
                var panel = (PlayerPanel)play;
                panel.SetPlayerReadyState(false);
            }
        }

        public override int OnChangeRoom()
        {
            var gdata = App.GetGameData<GangwuGameData>();
            var playerList = gdata.PlayerList;
            gdata.IsGameInfo = false;
           
            BetMgr.Reset();
            Reset();
            gdata.CurRound = 0;
            foreach (var yxBaseGamePlayer in playerList)
            {
                var player = (PlayerPanel)yxBaseGamePlayer;
                player.Reset();
                player.Info = null;
                player.RefreshPanel();
                player.gameObject.SetActive(false);
            }
            HistoryResultMgr.Reset();
            return base.OnChangeRoom();
        }

        public override void UserReady(int localSeat, ISFSObject responseData)
        {
            base.UserReady(localSeat, responseData);
            var gdata = App.GetGameData<GangwuGameData>();
            if (gdata.IsGameStart)
            {
                return;
            }

            YxDebug.Log("Seat " + responseData.GetInt("seat") + " is ready!");
            PlayerPanel panel = gdata.GetPlayer<PlayerPanel>(responseData.GetInt("seat"), true);
            panel.SetPlayerReadyState(true);
        }
      

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            if (PublicPokers.Count > 0)
            {
                foreach (PokerCard publicPoker in PublicPokers)
                {
                    if (publicPoker != null && publicPoker.gameObject != null)
                        Destroy(publicPoker.gameObject);
                }
            }

            YxClockManager.StopWaitPlayer();
            PublicPokers.Clear();
            SpeakMgr.Reset();
            DealerMgr.Rest();

            var playerList = App.GetGameData<GangwuGameData>().PlayerList;
            foreach (var yxBaseGamePlayer in playerList)
            {
                var player = (PlayerPanel)yxBaseGamePlayer;
                player.Reset();
            }
            StopAllCoroutines();
        }
    }

    // ReSharper disable InconsistentNaming
    public enum PokerType
    {

        ct_GaoPai = 0,         //高牌
        ct_YiDui,              //一对
        ct_LiangDui,           //两对
        ct_SanTiao,            //三条
        ct_ShunZi,             //顺子
        ct_TongHua,            //同花
        ct_Hulu,               //葫芦
        ct_SiTiao,             //四条
        ct_TongHuaShun,        //同花顺
        ct_HJTongHuaShun,      //皇家同花顺
        //ct_WuTiao              //测试特殊牌型动画使用
    }
}
