using System.Collections.Generic;
using Assets.Scripts.Game.paijiu.Gps;
using Assets.Scripts.Game.paijiu.Mgr;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.paijiu.ImgPress.Main
{
    public class PaiJiuGameManager : YxGameManager
    {

        public MenuMgr MenuMgr;

        public SpeakMgr SpeakMgr;

        public BetMgr BetMgr;

        public DealerMgr DealerMgr;

        public GpsInfosCtrl GpsInfosCtrl;

        public HistoryResultMgr HistoryResultMgr;

        public EffectsMgr EffectsMgr;

        public RoomResult RoomResult;

        public DismissRoomMgr DismissRoomMgr;

        public TalbeData TableData;

        public CompareMgr CompareMgr;

        public CheckShowedPanel CheckShowedPanel;


        // ReSharper disable once UnusedMember.Local
        void Start()
        {
            //根据平台改scalingStyle
            transform.GetComponent<UIRoot>().scalingStyle = Application.platform == RuntimePlatform.WindowsPlayer ? UIRoot.Scaling.Constrained : UIRoot.Scaling.ConstrainedOnMobiles;

            //限制最大40帧
            Application.targetFrameRate = 40;
            //屏幕常亮
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            //MusicManager.Instance.PlayBacksound("background");

            Facade.Instance<MusicManager>().Play("background");
        }

        /// <summary>
        /// 公共牌
        /// </summary>
        [HideInInspector]
        public List<PaiJiuCard> PublicPokers = new List<PaiJiuCard>();

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
        public int CurTurn;



        /// <summary>
        /// 初始化用户信息
        /// </summary>
        /// <param name="gameInfo"></param>
        public void InitUser(ISFSObject gameInfo)
        {
            var gdata = App.GetGameData<PaiJiuGameData>();

            //如果是有牌阶段,发给所有人手牌
            if (App.GetGameData<PaiJiuGameData>().IsCardsStatus)
            {
                foreach (var seatPanel in gdata.PlayerList)
                {
                    var user = (PaiJiuPlayer)seatPanel;
                    DealerMgr.DealOnesPokers(new int[4], user);
                }
            }

            SpeakMgr.RejionRefreshBtns(gdata.GetPlayerInfo<PaiJiuUserInfo>().IsPut);
        }

        /// <summary>
        /// 刷新玩家数据
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="user"></param>
        private void RefreshSomeone(PaiJiuPlayer panel, ISFSObject user)
        {
            //刷新数据
            panel.RefreshUserInfo();
            if (panel.IsReady)
            {
                var readyList = ReadyList;
                int seat = panel.Info.Seat;
                if (!readyList.Contains(seat))
                {
                    readyList.Add(seat);
                }
            }

            GetPlayerGPSInfo(panel, user);

            if (!App.GetGameData<PaiJiuGameData>().IsGameing)   //如果游戏结束,不接受数据
                return;

            ShowOnesBetVal(panel, user);
            ShowOnesCards(panel, user);
        }



        /// <summary>
        /// 显示玩家下注信息
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="user"></param>
        private void ShowOnesBetVal(PaiJiuPlayer panel, ISFSObject user)
        {
            if (!user.ContainsKey("beat"))
                return;
            int betVal = user.GetInt("beat");
            if (betVal <= 0)
                return;
            panel.PlayerBet(betVal);

            panel.ShowBetLabel();
            panel.CoinLabel.Text(YxUtiles.ReduceNumber(user.GetLong("ttgold") - betVal));
        }


        /// <summary>
        /// 显示玩家手牌信息
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="user"></param>
        private void ShowOnesCards(PaiJiuPlayer panel, ISFSObject user)
        {
            if (App.GetGameData<PaiJiuGameData>().Status != 2)
                return;
            int[] cards = user.ContainsKey("card") ? user.GetIntArray("card") : new int[4];
            //YxDebug.Log(" ====== 打印手牌 ===== seat == " + panel.CurPaiJiuUserInfo.Seat);
            //Tool.Tools.TestDebug(cards);
            panel.UserBetPoker.SetBetPokerInfo(cards);
            if ((user.ContainsKey("isput") && user.GetInt("isput") > 0))
            {
                panel.FinishSelect();
            }
        }


        public void InitGameInfo(ISFSObject gameInfo)
        {
            var gdata = App.GetGameData<PaiJiuGameData>();
           
            if (gameInfo.ContainsKey("curante"))
            {
                gdata.Ante = gameInfo.GetInt("curante");
            }
            if (gameInfo.ContainsKey("cargs2"))
            {
                BetMgr.InitChips(gameInfo);
            }
            if (gameInfo.ContainsKey("banker"))
            {
                gdata.BankerSeat = gameInfo.GetInt("banker");
            }

           

            var status = 0;
            if (gameInfo.ContainsKey("status"))
            {
                status = gameInfo.GetInt("status");
                gdata.Status = status;
                bool isGameing = status > 0;
                gdata.IsGameing = isGameing;
                //设置时间
                if (!isGameing || !gameInfo.ContainsKey("cd")) return;
                var cd = gameInfo.GetInt("cd");
                cd -= (int)(gameInfo.GetLong("ct") - gameInfo.GetLong("st"));
                YxClockManager.BeginToCountDown(cd);
            }
        }

        /// <summary>
        /// 初始化 房间
        /// </summary>
        public void InitRoom(ISFSObject gameInfo)
        {
            var gdata = App.GetGameData<PaiJiuGameData>();

            //获取房间配置
            if (gameInfo.ContainsKey("rid"))
            {
                gdata.IsRoomGame = true;
                RoomInfo.ShowRoomInfo(gameInfo);
                gdata.CurRound = gameInfo.GetInt("round");
                //HistoryResultMgr.SetRoomInfo(gameInfo);
            }
            else
            {
                gdata.IsRoomGame = false;
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

            if (gameInfo.ContainsKey("outCards"))
            {
                CheckShowedPanel.AddCard(gameInfo.GetIntArray("outCards"));
                TableData.SubCardCount(gameInfo.GetIntArray("outCards").Length);
                //YxDebug.Log(" ==== 打印已经出现过的牌 ===== ");
                //Tool.Tools.TestDebug(gameInfo.GetIntArray("outCards"));
            }
            else
            {
                TableData.SubCardCount(0);
            }

            if (gameInfo.GetInt("status") >= 2)
            {
                TableData.SubCardCount(16);
            }
        }


        /// <summary>
        /// 开始动画
        /// </summary>
        public void PlayBeginAnim()
        {
            if (App.GetGameData<PaiJiuGameData>().IsPlayed)
                return;

            BeginBetAnim.SetActive(true);

            Facade.Instance<MusicManager>().Play("start");
        }


        /// <summary>
        /// 记录游戏下注的玩家
        /// </summary>
        [HideInInspector]
        public int[] BegiBetSeatsNum = null;

        void GetPlayerGPSInfo(PaiJiuPlayer panel, ISFSObject data)
        {
            if (data.ContainsKey("gpsx") && data.ContainsKey("gpsy"))
            {
                panel.GpsX = data.GetFloat("gpsx");
                panel.GpsY = data.GetFloat("gpsy");
                panel.HasGpsInfo = true;
            }

            if (data.ContainsKey("ip"))
            {
                panel.Ip = data.GetUtfString("ip");
            }
        }


        /// <summary>
        /// 设置玩家GPS信息
        /// </summary>
        /// <param name="data"></param>
        public void CheckGpsInfo(ISFSObject data)
        {
            int userId = data.GetInt("uid");
            var users = App.GetGameData<PaiJiuGameData>().PlayerList;
            for (int i = 0, max = users.Length; i < max; i++)
            {
                PaiJiuPlayer paiJiuPlayer = (PaiJiuPlayer)users[i];
                if (paiJiuPlayer == null)
                {
                    continue;
                }
                if (paiJiuPlayer.Info != null && paiJiuPlayer.Info.Id == userId && paiJiuPlayer.gameObject.activeInHierarchy)
                {
                    if ((data.ContainsKey("gpsx") && data.ContainsKey("gpsy")) || (data.ContainsKey("x") && data.ContainsKey("y")))
                    {
                        paiJiuPlayer.GpsX = data.ContainsKey("gpsx") ? data.GetFloat("gpsx") : data.GetFloat("x");
                        paiJiuPlayer.GpsY = data.ContainsKey("gpsy") ? data.GetFloat("gpsy") : data.GetFloat("y");
                        paiJiuPlayer.HasGpsInfo = true;

                        if (data.ContainsKey("country") && paiJiuPlayer.Info != null)
                        {
                            paiJiuPlayer.Info.Country = data.GetUtfString("country");
                        }
                    }
                    else
                    {
                        paiJiuPlayer.GpsX = -1f;
                        paiJiuPlayer.GpsY = -1f;
                        paiJiuPlayer.HasGpsInfo = false;
                    }
                }
            }
        }

        internal void OnGameStart()
        {
            var users = App.GetGameData<PaiJiuGameData>().PlayerList;
            foreach (var panel in users)
            {
                var user = (PaiJiuPlayer)panel;
                user.OnGameStart();
            }
            RoomInfo.OnGameStart();
        }

        internal void OnBet(ISFSObject data)
        {
            int bankerSeat = data.GetInt("banker");
            var gdata = App.GetGameData<PaiJiuGameData>();
            gdata.BankerSeat = bankerSeat;
            gdata.GetPlayer<PaiJiuPlayer>(bankerSeat, true).SetBankerMarkActive(true);
            if (bankerSeat != App.GameData.SelfSeat)
            {
                SpeakMgr.ShowSpeak(GameRequestType.BeginBet);
            }

            CheckShowedPanel.ResetCards(data.GetInt("cardcnt") > 16);       //一次出现发牌16张,共32张牌.如果张数为32,就需要重置出现牌信息
            YxClockManager.BeginToCountDown(data.GetInt("cd"));
            TableData.SetTalebData(data);
            OnGameStart();
        }

        internal void OnCompare(ISFSObject data)
        {
            DealerMgr.OnCompare();

            //隐藏不需要的内容
            SpeakMgr.ShowNothing();

            var gdata = App.GetGameData<PaiJiuGameData>();

            var userList = gdata.PlayerList;

            //刷新玩家比牌数据
            if (data.ContainsKey("compare"))
            {

                foreach (var sort in userList)
                {
                    var user = (PaiJiuPlayer)sort;
                    user.FinishSelect();
                }

                ISFSArray users = data.GetSFSArray("compare");
                foreach (ISFSObject user in users)
                {
                    int seat = user.GetInt("seat");
                    CompareMgr.AddSeat(seat);
                    SetUserCompareVal(user, seat);
                    AddShowedCards(user);
                }
                CompareMgr.StartCompare();
            }

            //刷新玩家手上筹码
            if (!data.ContainsKey("ttgold")) return;

            long[] ttgoldArr = data.GetLongArray("ttgold");
            YxDebug.LogArray(ttgoldArr,"Result Score ");
           
            //YxDebug.Log(" ==== 玩家总筹码刷新数据 ===== ");
            for (int i = 0; i < userList.Length; i++)
            {
                PaiJiuPlayer panel = gdata.GetPlayer<PaiJiuPlayer>(i,true);
                if (panel.Info == null)
                    continue;
                panel.ShowWinVal(ttgoldArr[i]);     //显示输赢(要在Gold赋值之前)
                panel.Info.CoinA = ttgoldArr[i];      //刷新数据
                panel.RefreshPanel();
            }
        }

        /// <summary>
        /// 显示所有玩家的比牌信息
        /// </summary>
        /// <param name="user">玩家信息</param>
        /// <param name="seat">玩家座位号</param>
        private void SetUserCompareVal(ISFSObject user, int seat)
        {
            AddOnesGroupInfo(seat, user.GetSFSObject("small"));
            AddOnesGroupInfo(seat, user.GetSFSObject("big"));
        }


        /// <summary>
        /// 添加显示过的牌组
        /// </summary>
        /// <param name="user"></param>
        private void AddShowedCards(ISFSObject user)
        {
            CheckShowedPanel.AddCard(user.GetSFSObject("small").GetIntArray("cards"));
            CheckShowedPanel.AddCard(user.GetSFSObject("big").GetIntArray("cards"));
        }


        private void AddOnesGroupInfo(int seat, ISFSObject groupInfo)
        {
            int[] cards = groupInfo.GetIntArray("cards");
            int type = groupInfo.GetInt("type");
            var gdata = App.GetGameData<PaiJiuGameData>();
            gdata.GetPlayer<PaiJiuPlayer>(seat, true).UserBetPoker.AddGoupInfo(cards, type);
        }
        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {

            foreach (PaiJiuCard publicPoker in PublicPokers)
            {
                if (publicPoker == null)
                    continue;
                Destroy(publicPoker.gameObject);
            }

            PublicPokers.Clear();
            CurTurn = 0;

            SpeakMgr.Reset();
            DealerMgr.Rest();
            BetMgr.Reset();
            TableData.Reset();
            CompareMgr.Reset();

            var userList = App.GetGameData<PaiJiuGameData>().PlayerList;

            foreach (var player in userList)
            {
                var user = (PaiJiuPlayer)player;
                user.Reset();
            }
        }

        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            Reset();

            var gdata = App.GetGameData<PaiJiuGameData>();

            //初始化房间信息
            InitGameInfo(gameInfo);

            //初始化房间信息
            InitRoom(gameInfo);

            //初始化玩家信息
            InitUser(gameInfo);

            if (App.GetGameData<PaiJiuGameData>().IsRoomGame)
            {
                if (gameInfo.ContainsKey("hupstart"))
                {
                    DismissRoomMgr.ShowDismissOnRejion(gameInfo);
                }
            }
            if (gdata.IsGameing)
            {
                TableData.SetTalebData(gameInfo);
            }

            YxWindowManager.HideWaitFor();
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
        }

        public override void GameStatus(int status, ISFSObject info)
        {
        }
        internal List<int> ReadyList = new List<int>();
        public override void GameResponseStatus(int type, ISFSObject response)
        {
            YxDebug.Log("Request == " + (GameRequestType)type);

            var gdata = App.GetGameData<PaiJiuGameData>();
            gdata.Status = type;

            switch ((GameRequestType)type)
            {
                case GameRequestType.AllowStart:
                    gdata.GetPlayer<PaiJiuPlayerSelf>().CouldStart();
                    break;

                case GameRequestType.BeginBet:
                    OnBet(response);
                    break;

                case GameRequestType.UserBet:
                    int betSeat = response.GetInt("seat");
                    gdata.GetPlayer<PaiJiuPlayer>(betSeat, true).PlayerBet(response.GetInt("gold"));
                    break;

                case GameRequestType.SendCard:
                    TableData.SubCardCount(16);
                    SpeakMgr.ShowNothing();
                    YxClockManager.BeginToCountDown(response.GetInt("cd"));
                    DealerMgr.BeginBigDeal(response.GetIntArray("card"));
                    Tool.Tools.TestDebug(response.GetIntArray("card"));
                    //SpeakMgr.ShowSpeak(GameRequestType.SendCard);
                    break;

                case GameRequestType.PutCard:
                    DealerMgr.FastDeal();
                    int putcardSeat = response.GetInt("seat");
                    gdata.GetPlayer<PaiJiuPlayer>(putcardSeat, true).FinishSelect();
                    if (putcardSeat == App.GameData.SelfSeat)
                    {
                        if (response.ContainsKey("cards"))
                        {
                            int[] putCards = new int[4];        //data.GetIntArray("cards");
                            gdata.GetPlayer<PaiJiuPlayer>(putcardSeat, true).UserBetPoker.SetBetPokerInfo(putCards);
                        }
                        SpeakMgr.ShowNothing();
                    }
                    break;

                case GameRequestType.Compare:
                    OnCompare(response);
                    break;

                case GameRequestType.AllowReady:
                    gdata.IsGameing = false;
                    Reset();
                    ReadyList.Clear();
                    break;

                case GameRequestType.GameBegin:
                    break;
            }
        }

        public override void BeginNewGame(ISFSObject sfsObject)
        {
            base.BeginNewGame(sfsObject);
            var gdata = App.GetGameData<PaiJiuGameData>();
            gdata.IsGameing = true;
            var curRound = ++App.GetGameData<PaiJiuGameData>().CurRound;
            RoomInfo.SetCurRound(curRound);
        }

        public override void UserOut(int localSeat, ISFSObject responseData)
        {
            var seat = responseData.GetInt("seat");
            var playerPanel = App.GetGameData<PaiJiuGameData>().GetPlayer<PaiJiuPlayer>(seat, true);

            playerPanel.Info = null;
            playerPanel.RefreshPanel();
            playerPanel.gameObject.SetActive(false);
            ReadyList.Remove(seat);
        }

        public override void UserOnLine(int localSeat, ISFSObject responseData)
        {
            var seat = responseData.GetInt("seat");
            var panel = App.GetGameData<PaiJiuGameData>().GetPlayer<PaiJiuPlayer>(seat, true);
            panel.Mask.SetActive(false);
        }

        public override void UserIdle(int localSeat, ISFSObject responseData)
        {
            var seat = responseData.GetInt("seat");
            var panel = App.GetGameData<PaiJiuGameData>().GetPlayer<PaiJiuPlayer>(seat, true);
            panel.Mask.SetActive(true);
        }

        public override void UserReady(int localSeat, ISFSObject responseData)
        {
            YxDebug.Log("Seat " + responseData.GetInt("seat") + " is ready!");
            var gdata = App.GetGameData<PaiJiuGameData>();
            if (!gdata.IsGameing)
            {
                int readySeat = responseData.GetInt("seat");

                //加入准备玩家
                if (!ReadyList.Contains(readySeat)) //不包括已准备玩家
                    ReadyList.Add(readySeat);

                //设置玩家的准备状态
                gdata.GetPlayer<PaiJiuPlayer>(readySeat, true).OnUserReady();

            }
        }

    }
}
