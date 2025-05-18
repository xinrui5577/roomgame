using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.Mahjong2D.Common.MahJongCommon;
using Assets.Scripts.Game.Mahjong2D.Common.UI;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Request;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Gps;
using Assets.Scripts.Game.Mahjong2D.Game.Component.GameOver;
using Assets.Scripts.Game.Mahjong2D.Game.Component.GameResult;
using Assets.Scripts.Game.Mahjong2D.Game.Component.GameTable;
using Assets.Scripts.Game.Mahjong2D.Game.Component.Piao;
using Assets.Scripts.Game.Mahjong2D.Game.Data;
using Assets.Scripts.Game.Mahjong2D.Game.Item;
using Assets.Scripts.Game.Mahjong2D.Game.Player;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Game.Mahjong2D.Game.GameCtrl
{
    // 麻将，顺拿逆打，牌堆是顺时针拿牌的，打牌是逆时针的
    // 注意：长分割线注释是按照游戏开始到结束流程划分，添加或修改代码的时候不要乱写！
    public class Mahjong2DGameManager : YxGameManager
    {
        /// <summary>
        /// 用于处理游戏中玩家相关的数据，与对应方向的玩家相关联
        /// </summary>
        [HideInInspector]
        public MahjongPlayer[] Players;

        /// <summary>
        /// 当前
        /// </summary>
        public MahjongPlayerCtrl SelfPlayer;

        [SerializeField]
        /// <summary>
        /// 右边
        /// </summary>
        private MahjongPlayer RightPlayer;

        [SerializeField]
        /// <summary>
        /// 对面
        /// </summary>
        private MahjongPlayer OppsetPlayer;

        [SerializeField]
        /// <summary>
        /// 左边
        /// </summary>
        private MahjongPlayer LeftPlayer;

        /// <summary>
        /// 玩家头像
        /// </summary>
        [SerializeField]
        private UserInfoPanel[] _userinfoPanels;

        /// <summary>
        /// 准备菜单
        /// </summary>
        [SerializeField]
        private CpghChar[] _cpghChars;

        /// <summary>
        /// 癞子牌
        /// </summary>
        [HideInInspector]
        public int LaiZiNum;

        /// <summary>
        /// 宝牌
        /// </summary>
        [HideInInspector]
        public List<int> BaoCards = new List<int>();

        /// <summary>
        /// 翻牌
        /// </summary>
        [HideInInspector]
        public int FanNum;
        /// <summary>
        /// 默认麻将颜色
        /// </summary>
        public EnumMahJongColorType DefMahjongColor = EnumMahJongColorType.G;
        /// <summary>
        /// 默认麻将牌值样式
        /// </summary>
        public EnumMahJongValueType DefMahjongValueType = EnumMahJongValueType.A;

        /// <summary>
        /// 是否播放分牌动画
        /// </summary>
        public bool IsShowAllocateAnimation = false;

        /// <summary>
        /// 游戏状态
        /// </summary>
        private TotalState gameState;

        /// <summary>
        /// 游戏焦点
        /// </summary>
        private bool _focus = false;

        /// <summary>
        /// 暂停状态
        /// </summary>
        private bool _pause = false;

        /// <summary>
        /// 当前用来检查的类型
        /// </summary>
        private int _checkType;

        /// <summary>
        /// 当前用来检查的牌
        /// </summary>
        private int _checkCard;

        /// <summary>
        /// 判断本局是否显示过买断门，用来控制漂显示
        /// </summary>
        private bool _isShowDuanMen = false;

        /// <summary>
        /// 临时存储的加刚信息
        /// </summary>
        private int[] _piaos;
        [Tooltip("分张延迟时间")]
        public float FenZhangDeltaTime = 0;
        [Tooltip("分张提示动画显示时间")]
        public float FenZhangNoticeTime = 0;
        /// <summary>
        /// 最后一圈标识
        /// </summary>
        public bool IsLastRound
        {
            private set;get;
        }

        /// <summary>
        /// 某个人需要一张牌，用来吃碰杠
        /// </summary>
        public Action OnSomeOneNeedCard;

        public List<int> TingOutCards
        {
            get { return Data.TingOutCards; }
            set { Data.TingOutCards = value; }
        }

        [Tooltip("游戏状态变化")]
        public List<EventDelegate> OnGameStatusChange=new List<EventDelegate>(); 
        [Tooltip("分张事件")]
        public List<EventDelegate> OnFenZhangAction=new List<EventDelegate>();
        [Tooltip("二人场")]
        public List<EventDelegate> OnTwoPlayerAction = new List<EventDelegate>();
        [Tooltip("最后一局提示")]
        public List<EventDelegate> OnLastRoundAction = new List<EventDelegate>();
        [Tooltip("二人场打出牌数量")]
        public int TwoPlayerOutCardPerLine = 18;
        /// <summary>
        /// 潇洒
        /// </summary>
        public int Xs;
        /// <summary>
        /// 麻将灵活度，拖拽麻将生效范围
        /// </summary>
        public float ItemDragDistance = 0.1f;
        /// <summary>
        /// 检测op时重置方向
        /// </summary>
        public bool DirectionResetOnCheckOp=false;
        /// <summary>
        /// 等待阶段显示准备状态
        /// </summary>
        public bool ShowReadyOnWait = false;
        /// <summary>
        /// 是否为特殊分张（分张数据为intArray,目前只有调兵山麻将如此处理,使用资源开关控制）
        /// </summary>
        public bool SpecialFenZhang = false;
        [Tooltip("根据风杠数量显示旋风杠（分别显示三风与四风按钮）")]
        public bool ShowFengWithNum=false;
        /// <summary>
        /// 立杠列表
        /// </summary>
        [HideInInspector]
        public List<int> LiGangList=new List<int>();
        [Tooltip("打牌自动落下，默认值为false")]
        public bool ThrowCardAutoDown = false;

        /// <summary>
        /// GameInfo是否初始化
        /// </summary>
        public bool IsInfoInit
        {
            private set;
            get;
        }

        /// <summary>
        /// 是否游戏中
        /// </summary>
        [HideInInspector]
        public TotalState GameTotalState
        {
            get { return gameState; }
            set
            {
                gameState = value;
                switch (value)
                {
                    case TotalState.Init:
                        MenuController.Instance.ChangeBtnStatusAfterStartingGame();
                        break;
                    case TotalState.Waiting:
                        MenuController.Instance.ChangeBtnStatusAfterStartingGame();
                        InitGameData();
                        App.GetRServer<Mahjong2DGameServer>().Ready();
                        break;
                    case TotalState.Gaming:
                        MenuController.Instance.ChangeBtnStatusAfterStartingGame();
                        foreach (var player in Players)
                        {
                            if (player != null && player.UserInfo != null)
                            {
                                player.HideReady();
                                player.CurrentInfoPanel.Refresh(IsGameing);
                                player.SetUserHead(player.UserInfo.IsOnLine, true);
                            }
                        }
                        break;
                    case TotalState.Account:
                        Data.IsInRobot = false;
                        RobotUi.Instance.ShowAutoState(false);
                        if (Players != null) 
                        {
                            foreach (var player in Players)
                            {
                                if (player)
                                {
                                    player.ChangeAutoState(false);
                                    if (player.UserInfo != null)
                                    {
                                        player.HasTing = false;
                                    }
                                }
                            }
                        }
                        SelfPlayer.ClearMenu();
                        ShowChiPaiInfo.Instance.Show(false);
                        break;
                    case TotalState.Over:
                        break;
                }
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(OnGameStatusChange.WaitExcuteCalls());
                }
            }
        }

        public Mahjong2DGameData Data
        {
            get
            {
                return App.GetGameData<Mahjong2DGameData>();
            }
        }

        public Mahjong2DGameServer Mahjong2DGame
        {
            get { return App.GetRServer<Mahjong2DGameServer>(); }
        }

        public CurrentGameType GameType
        {
            get { return Data.CurrentGame; }
        }

        public bool ChatVioceToggle
        {
            set { Data.IsChatVoiceOn = value; }
            get { return Data.IsChatVoiceOn; }
        }

        public bool IsGameing
        {
            get { return GameTotalState == TotalState.Gaming; } 
        }

        public bool IsWaiting
        {
            get { return GameTotalState == TotalState.Waiting; } 
        }

        public bool IsInit
        {
            get { return GameTotalState == TotalState.Init; }
        }

        public bool IsAccount
        {
            get { return GameTotalState == TotalState.Account; }
        }

        public bool IsGameOver
        {
            get { return GameTotalState == TotalState.Over; }
        }

        public int CurSeat
        {
            get
            {
                if (SelfPlayer!=null)
                {
                    return SelfPlayer.UserSeat;
                }
                return 0;
            }
        }

        public int Banker0
        {
            get { return Data.Bank0; }
        }

        /// <summary>
        /// 打出最后一张牌的玩家的座位号（实际座位），用来从对应玩家处获得打出
        /// </summary>
        private int _lastOutCardUserSeatID = 0;

        [HideInInspector]
        /// <summary>
        /// 本圈的庄
        /// </summary>
        public int StartPosition
        {
            get { return Data.Bank; }
            set
            {
                if (Players != null && Players.Length > 0)
                {
                    Players[Data.Bank].IsZhuang = false;
                    DealQuan(value);
                    Data.Bank = value;
                    Players[Data.Bank].IsZhuang = true;
                }
                else
                {
                    Debug.LogError(string.Format("设置庄的时候出现异常，庄的座位号是{0}",value));
                }
            }
        }

        /// <summary>
        /// 当前玩家座位号
        /// </summary>
        public int CurrentSeat
        {
            get { return SelfPlayer.UserSeat; }
        }

        /// <summary>
        /// 当前游戏需要玩家人数
        /// </summary>
        public int PlayerNumber
        {
            get { return Data.PlayerNum; }
        }

        /// <summary>
        /// 当前牌桌中一圈最后一个玩家的实际座位号
        /// </summary>
        public int QuanLastSeat
        {
            get { return (PlayerNumber - 1 + Data.Bank0) % PlayerNumber; }
        }

        public int CurDirectionIndex
        {
            get { return SelfPlayer.ShowDirectionIndex; }
            set { SelfPlayer.ShowDirectionIndex = value; }
        }

        /// <summary>
        /// 牌局中是否有人有宝
        /// </summary>
        [HideInInspector]
        public bool IsTingExist { set; get; }

        public EnumGameKeys CurGameKey
        {
            get { return (EnumGameKeys)Enum.Parse(typeof(EnumGameKeys), App.GameKey); }

        }

        /// <summary>
        /// 墙牌的获得位置（未使用）
        /// </summary>
        [HideInInspector]
        public int getPosition;

        /// <summary>
        /// 跟庄的人数
        /// </summary>
        private int _genZhuangNum;
        [Tooltip("托管模式是否为本地控制")]
        public bool AutoStateByLocal = true;

        protected override void OnAwake()
        {
            base.OnAwake();
            EnumMahJongColorType color = (EnumMahJongColorType)PlayerPrefs.GetInt(ConstantData.KeyMahjongBgType,(int)DefMahjongColor);
            EnumMahJongValueType valueType =(EnumMahJongValueType)PlayerPrefs.GetInt(ConstantData.KeyMahjongValueType,(int)DefMahjongValueType);
            MahjongItem.OnChangeBgType(color);
            MahjongItem.OnChangeValueType(valueType);
            InitShowSeat();
            InitWaitFor();
        }

        void InitWaitFor()
        {
            _fenZhangWaitFor = new WaitForSeconds(FenZhangDeltaTime);
            _fenZhangNoticeAniTime=new WaitForSeconds(FenZhangNoticeTime);
        }

        /// <summary>
        /// 初始化显示座位号
        /// </summary>
        void InitShowSeat()
        {
            if (SelfPlayer)
            {
                SelfPlayer.ShowSeat = 0;
            }
            if (RightPlayer)
            {
                RightPlayer.ShowSeat = 1;
            }
            if (OppsetPlayer)
            {
                OppsetPlayer.ShowSeat = 2;
            }
            if (SelfPlayer)
            {
                LeftPlayer.ShowSeat = 3;
            }
        }

        /// <summary>
        ///初始化牌桌中的信息
        /// </summary>
        public void Init()
        {
            InitGameData();
            InitState();
            InitPlayers();
            GameTable.Instance.InitInfoPanel();
            GameTable.Instance.UpdateLeftNum(GetLeftNum());
            RuleShowControl.Instance.SetRuleInfo(GameType.Rules);
            Facade.EventCenter.AddEventListeners<string,bool>(ConstantData.KeyRobotToggle, LocalStateChange);
            Ready();
        }

        #region 初始化游戏状态与玩家设置

        /// <summary>
        /// 根据实际玩家人数初始化牌桌中的座位显示
        /// </summary>
        private void InitPlayers()
        {
            List<ISFSObject> datas = Data.UserDatas;
            Players = new MahjongPlayer[PlayerNumber];
            UserData currentData = new UserData(datas[0]);
            RightPlayer.Show(false);
            OppsetPlayer.Show(false);
            LeftPlayer.Show(false);
            SelfPlayer.Show(true);
            switch (PlayerNumber) //临时处理吧，以后调整为UI控制.目前先这么搞
            {
                case 2:
                    SelfPlayer.SetOutCardsPerLenth(TwoPlayerOutCardPerLine);
                    OppsetPlayer.SetOutCardsPerLenth(TwoPlayerOutCardPerLine);
                    OppsetPlayer.Show(!OppsetPlayer.CurrentInfoPanel.NoDataHide);
                    if (gameObject.activeInHierarchy)
                    {
                        StartCoroutine(OnTwoPlayerAction.WaitExcuteCalls());
                    }
                    break;
                case 3:
                    RightPlayer.Show(!RightPlayer.CurrentInfoPanel.NoDataHide);
                    LeftPlayer.Show(!LeftPlayer.CurrentInfoPanel.NoDataHide);
                    break;
                case 4:
                    RightPlayer.Show(!RightPlayer.CurrentInfoPanel.NoDataHide);
                    OppsetPlayer.Show(!OppsetPlayer.CurrentInfoPanel.NoDataHide);
                    LeftPlayer.Show(!LeftPlayer.CurrentInfoPanel.NoDataHide);
                    break;
            }
            SelfPlayer.JoinGame(currentData,IsGameing);
            SelfPlayer.Reset();
            SureCurDirection();
            InteraptMenu.Instance.SetTarget(SelfPlayer.gameObject);
            if (datas.Count > 1)
            {
                for (int i = 1, max = datas.Count; i < max; i++)
                {
                    OnJoinGame(new UserData(datas[i]));
                }
            }
        }

        private void SureCurDirection()
        {
            int showIndex = GetDirectionSeat(CurSeat);
            switch (PlayerNumber)
            {
                case 2:
                    switch (showIndex)
                    {
                        case 0:
                            CurDirectionIndex = 0;
                            break;
                        case 1:
                            CurDirectionIndex = 2;
                            break;
                    }
                    break;
                case 3:
                case 4:
                    CurDirectionIndex = showIndex;
                    break;
            }
        }

        private int SureOtherDiretion(int Seat)
        {
            int showDirection = 0;
            int showIndex = GetDirectionSeat(Seat);
            switch (PlayerNumber)
            {
                case 2:
                    switch (showIndex)
                    {
                        case 0:
                            showDirection = 0;
                            break;
                        case 1:
                            showDirection = 2;
                            break;
                    }
                    break;
                case 3:
                    showDirection = showIndex;
                    switch (CurDirectionIndex)
                    {
                        case 0:
                            switch (showIndex)
                            {
                                case 2:
                                    showDirection = 3;
                                    break;
                            }
                            break;
                        case 1: //1的两边就是0和2不用变
                            switch (showIndex)
                            {
                                case 0:
                                    break;
                                case 2:
                                    break;
                            }
                            break;
                        case 2:
                            switch (showIndex)
                            {
                                case 0:
                                    showDirection = 3;
                                    break;
                                case 1:

                                    break;
                            }
                            break;
                    }
                    break;
                case 4: //不用变
                    showDirection = showIndex;
                    break;
            }
            return showDirection;
        }


        /// <summary>
        /// 同步Data中的状态，刷新界面显示
        /// </summary>
        private void InitState()
        {
            GameTotalState = Data.GameTotalStatus;
        }

        #endregion

        #region 准备

        private void Ready()
        {
            Invoke("DelayReady", 0.5f);
        }

        private void DelayReady()
        {

            App.GetRServer<Mahjong2DGameServer>().Ready();
        }

        public void OnGameReady(int seat)
        {
            if (App.GetGameData<Mahjong2DGameData>().IsFirstTime|| ShowReadyOnWait)
            {
                Players[seat].ReadyGame();
            }
            Players[seat].SetUserHead(true, false);
        }

        #endregion

        /// <summary>
        /// 以当前玩家为0，其他玩家的座位号
        /// </summary>
        /// <param name="userSeat"></param>
        /// <returns></returns>
        public int GetShowSeat(int userSeat)
        {
            return (userSeat - CurrentSeat + PlayerNumber) % PlayerNumber;
        }

        /// <summary>
        ///根据bank0，计算出当前玩家的方向索引
        /// </summary>
        /// <param name="userSeat"></param>
        /// <returns></returns>
        public int GetDirectionSeat(int userSeat)
        {
            return (userSeat - Banker0 + PlayerNumber) % PlayerNumber;
        }

        /// <summary>
        /// 判断圈数是否增加，需要在局数实际变化前处理
        /// </summary>
        private void DealQuan(int value)
        {
            int bank0 = Data.Bank0;
            if (StartPosition.Equals(QuanLastSeat) && value.Equals(bank0) && !Data.IsFirstTime)
            {
                if (!GameType.Quan.Equals(GameType.TotalRound))
                {
                    GameType.Quan += 1;
                }
            }
        }

        /// <summary>
        ///  其它玩家加入游戏
        /// </summary>
        /// <param name="user"></param>
        public void OnJoinGame(UserData user)
        {
            int sit = user.Seat;
            int ShowSeat = GetShowSeat(sit);
            int showDirection = SureOtherDiretion(sit);
            MahjongPlayer player = null;
            switch (PlayerNumber)
            {
                case 2:
                    player = OppsetPlayer.JoinGame(user, IsGameing);
                    break;
                case 3:
                    switch (ShowSeat)
                    {
                        case 1:
                            player = RightPlayer.JoinGame(user,IsGameing);
                            break;
                        case 2:
                            player = LeftPlayer.JoinGame(user,IsGameing);
                            break;
                    }
                    break;
                case 4:
                    switch (ShowSeat)
                    {
                        case 1:
                            player = RightPlayer.JoinGame(user,IsGameing);
                            break;
                        case 2:
                            player = OppsetPlayer.JoinGame(user,IsGameing);
                            break;
                        case 3:
                            player = LeftPlayer.JoinGame(user,IsGameing);
                            break;
                    }
                    break;
            }
            if (!IsGameing)
            {
                if (player != null)
                {
                    player.Reset();
                }
            }
            if (player != null)
            {
                player.ShowDirectionIndex = showDirection;
            }
            int playerNumber = Data.UserDatas.Count;
            if (playerNumber == Data.PlayerNum)
            {
                if (IsInit)
                {
                    GameTotalState = TotalState.Waiting;
                }
            }
            else
            {
                YxDebug.Log(string.Format("人数不满足条件,当前的游戏状态是：{0}，人数是{1}", gameState, playerNumber));
            }
        }
        
        private void InitGameData()
        {
            Data.ResetTotalNumber();
            Data.IsClickTing = false;
            LaiZiNum = 0;
            FanNum = 0;
            IsTingExist = false;
            GameTable.Instance.UpdateLeftNum(GetLeftNum());
            if (Players.Length.Equals(PlayerNumber))
            {
                for (int i = 0; i < PlayerNumber; i++)
                {
                    MahjongPlayer player = Players[i];
                    if (player != null)
                    {
                        player.Reset();
                    }
                }
            }
            _isShowDuanMen = false;
            BaoCardsControl.Instance.Reset();
            LaiZiControl.Instance.Reset();
            DuanMenControl.Instance.Reset();
            GameResult.Instance.CloseWindow();
        }
        // 断线重连
        public void OnReJoin(ISFSObject data)
        {
            #region 数据
            int roundSeq;
            int roundSeq2;
            int currentP;
            int startP;
            int op;
            int lastOtCdValue;
            int lastOutSeat;
            int lastIn;
            int nextBanker;
            int[] dicArray = new int[0];
            int[] tingOutCards;
            int opcard;
            int duanMen;
            int laizi;
            int fanNum;
            int state2;
            GameTools.TryGetValueWitheKey(data, out currentP, RequestKey.KeyCurrentPosition);
            GameTools.TryGetValueWitheKey(data, out roundSeq, RequestKey.KeySeq);
            GameTools.TryGetValueWitheKey(data, out roundSeq2, RequestKey.KeySeq2);
            GameTools.TryGetValueWitheKey(data, out lastOtCdValue, RequestKey.KeyLastOutCard);
            GameTools.TryGetValueWitheKey(data, out lastIn, RequestKey.KeyLastIn);
            GameTools.TryGetValueWitheKey(data, out startP, RequestKey.KeyStartPosition);
            GameTools.TryGetValueWitheKey(data, out dicArray, RequestKey.KeyDiceArray);
            GameTools.TryGetValueWitheKey(data, out fanNum, RequestKey.KeyFan);
            GameTools.TryGetValueWitheKey(data, out laizi, RequestKey.KeyLaiZi);
            GameTools.TryGetValueWitheKey(data, out lastOutSeat, RequestKey.KeyLastOutSeat);
            GameTools.TryGetValueWitheKey(Data.UserDatas[0], out op, RequestKey.KeyMenuOperation);
            GameTools.TryGetValueWitheKey(Data.UserDatas[0], out tingOutCards, RequestKey.KeyTingOutCards);
            GameTools.TryGetValueWitheKey(data, out duanMen, RequestKey.KeyDuanMen);
            GameTools.TryGetValueWitheKey(data, out state2, RequestKey.KeyDuanMenState);
            DealBaoData(data);
            #endregion
            #region 同步
            Data.DuanMenState = duanMen;
            if (fanNum != 0 && laizi != 0)
            {
                FanNum = fanNum;
                LaiZiNum = laizi;
            }
            if (IsWaiting)
            {
                GameTools.TryGetValueWitheKey(data, out nextBanker, RequestKey.KeyNextBank);
                CurrentPosition = nextBanker;
                startP = CurrentPosition;
            }
            else
            {
                CurrentPosition = currentP;
            }
            SetZhuangAndPosition(startP);
            for (int i = 0, lenth = Data.UserDatas.Count; i < lenth; i++)
            {
                ISFSObject userData = Data.UserDatas[i];
                int seat;
                GameTools.TryGetValueWitheKey(userData, out seat, RequestKey.KeySeat);
                var autoState= Players[seat].OnReJoin(userData);
                if (SelfPlayer&&SelfPlayer.UserSeat== seat)
                {
                    OnAutoStateChange(seat,autoState,false);
                }
                
                TryCheckLiList(userData);
            }
            BaoCardsControl.Instance.SetBaos(BaoCards, SelfPlayer.HasTing);
            GetHun(true);
            opcard = 0;
            if (roundSeq.Equals(roundSeq2)) //不是自己出牌阶段，只是等待响应别人的牌
            {
                if (lastOtCdValue != 0 && lastIn == 0)
                {
                    YxDebug.LogError(string.Format("最后一张打出的牌是：{0},座位号是{1}，名字是：{2}，牌是：{3}", lastOtCdValue, lastOutSeat,
                        Players[lastOutSeat].UserInfo.name, (EnumMahjongValue)lastOtCdValue));
                    Players[lastOutSeat].LostToken();
                    ClearCheckTypeOnLoseToken(lastOutSeat);
                    Players[lastOutSeat].SetLastOutMahjongShow(lastOtCdValue);
                    opcard = lastOtCdValue;
                }
            }
            else //自己抓牌或者别人抓牌后
            {
                if (lastIn != 0)
                {
                    Players[CurrentPosition].SetLastInCardOnReJoin(lastIn);
                    opcard = lastIn;
                }
                Players[CurrentPosition].GetToken();
                Players[CurrentPosition].RecheckLastGetIn();
            }
            _checkCard = opcard;
            TingOutCards = tingOutCards.ToList();
            CountDownGo.Instance.Begin();
            CheckGangType(data);
            CheckOp(op, SelfPlayer.UserSeat, _checkCard,true);
            if (Data.DuanMenState.Equals(ConstantData.DuanMenSelect))
            {
                DuanMenControl.Instance.SetDuanMen(0);
            }
            DealHupInfo(data);
            ShowRoundInfo();
            Players[CurrentPosition].CheckTingState();
            if(state2.Equals(ConstantData.DuanMenSelectState))
            {
                DuanMenControl.Instance.ShowDuanMenBtns(startP.Equals(SelfPlayer.UserSeat));
            }
            SetGangdi(data);
            #endregion
        }

        /// <summary>
        /// 设置杠底
        /// </summary>
        /// <param name="data"></param>
        private void SetGangdi(ISFSObject data)
        {
            if (data!=null&&data.ContainsKey(ConstantData.KeyGangDiCard))
            {
                var gangDi = 0;
                GameTools.TryGetValueWitheKey(data, out gangDi, ConstantData.KeyGangDiCard);
                if (gangDi != 0)
                {
                    Facade.EventCenter.DispatchEvent(ConstantData.KeyGangDiCard, gangDi);
                }
            }
        }

        private void DealHupInfo(ISFSObject data)
        {
            string hupInfo;
            GameTools.TryGetValueWitheKey(data, out hupInfo, RequestCmd.HandsUp);
            //接收重连解散信息
            if (!string.IsNullOrEmpty(hupInfo))
            {
                long svt;
                long hupStart;
                int time;
                GameTools.TryGetValueWitheKey(data, out svt, RequestCmd.ServerTime);
                GameTools.TryGetValueWitheKey(data, out hupStart, RequestKey.KeyHupStart);
                time = (int)(Data.HupTime - (svt - hupStart));
                time = time < 0 ? 0 : time;
                string[] ids = hupInfo.Split(',');

                for (int i = 0; i < ids.Length; i++)
                {
                    for (int j = 0, max = Players.Length; j < max; j++)
                    {
                        var id = int.Parse(ids[i]);
                        if (Players[j].UserInfo != null && id.Equals(Players[j].UserInfo.id))
                        {
                            //2发起投票 3同意 -1拒绝
                            ISFSObject hupData = new SFSObject();
                            hupData.PutUtfString(RequestKey.KeyUserName, Players[j].UserInfo.name);
                            hupData.PutInt(RequestKey.KeyType, i == 0 ? 2 : 3);
                            hupData.PutInt(RequestKey.KeyCDTime, time);
                            hupData.PutInt(RequestKey.KeyId, id);
                            App.GetRServer<Mahjong2DGameServer>().OnHandsUp(hupData);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 控制显示桌面的
        /// </summary>
        void ShowRoundInfo()
        {
            switch (CurGameKey)
            {
                case EnumGameKeys.pjmj:
                    if (!Data.IsFirstTime)
                    {
                        HelpInfoControl.Instance.OnClickChangeInfo();
                    }
                    break;
            }
        }
        // ----------------------------------- 网络消息分发
        public void ExcuteRequest(RequestData reqData)
        {
            ISFSObject param = reqData.Data;
            TryGetTingOutCards(reqData.TingOutCards);
            YxDebug.Log((EnumRequest)reqData.Type);
            switch ((EnumRequest)reqData.Type)
            {
                case EnumRequest.SelectPiao:
                    PiaoSelectPanel.Instance.ShowGameObject();
                    break;
                case EnumRequest.ShowPiao:
                    PiaoSelectPanel.Instance.ShowGameObject(false);
                    _piaos = param.GetIntArray(RequestKey.KeyPiaoList);
                    break;
                case EnumRequest.XuanDuanMen:
                    if (param.ContainsKey(RequestKey.KeyDuanMen))
                    {
                        GameTools.TryGetValueWitheKey(param, out Data.DuanMenState, RequestKey.KeyDuanMen);
                        if (Data.DuanMenState.Equals(ConstantData.DuanMenSelect))
                        {
                            DuanMenControl.Instance.SetDuanMen(ConstantData.DuanMenMoveTime);
                        }
                        else if(Data.DuanMenState.Equals(ConstantData.BuDuanMen))
                        {
                            DuanMenControl.Instance.Reset();
                        }
                    }
                    else
                    {
                        DuanMenControl.Instance.ShowDuanMenBtns(reqData.Sit.Equals(CurrentSeat));
                    }
                    break;
                case EnumRequest.ShowRate:
                    int rate;
                    GameTools.TryGetValueWitheKey(param, out rate, RequestKey.KeyRate);
                    GameTable.Instance.RateLabel.TrySetComponentValue(string.Format("x{0}", rate));
                    break;
                case EnumRequest.AlloCate:
                    ShowPiaos();
                    AlloCate(reqData);
                    GameTools.TryGetValueWitheKey(param, out FanNum, RequestKey.KeyFan);
                    GameTools.TryGetValueWitheKey(param, out LaiZiNum, RequestKey.KeyLaiZi);
                    GetHun(false, true);
                    SetGangdi(param);
                    break;
                case EnumRequest.GetCard:
                    GetInCard(reqData);
                    break;
                case EnumRequest.ThrowOutCard:
                    ThrowOutCard(reqData);
                    break;
                case EnumRequest.BaoCard:
                    GetBaoCard(param);
                    break;
                case EnumRequest.CpghMenu:
                    SelfPlayer.ClearMenu();
                    CheckOp(reqData.Op, SelfPlayer.UserSeat, _checkCard);
                    break;
                case EnumRequest.SelfGang:
                case EnumRequest.CPHType:
                    {
                        SelfPlayer.ClearMenu();
                        AllowResponse(reqData,true);
                        CheckOp(reqData.Op, reqData.Sit, reqData.OpCard);
                    }
                    break;
                case EnumRequest.JueGang:
                    {
                        if (OnSomeOneNeedCard != null)
                        {
                            OnSomeOneNeedCard();
                        }
                        for (int i = 0, lenth = Players.Length; i < lenth; i++)
                        {
                            MahjongPlayer player = Players[i];
                            if (player != null)
                            {
                                player.TryAddLastGetInCard();
                            }
                        }
                        Players[reqData.Sit].OnGetJueGang(reqData);
                        SelfPlayer.ClearMenu();
                        CheckOp(reqData.Op, reqData.Sit, reqData.OpCard);
                    }
                    break;
                case EnumRequest.LiuJu:
                    OnHupai(param, true);
                    break;
                case EnumRequest.ZiMo:
                    OnHupai(param);
                    break;
                case EnumRequest.Hu:
                    OnHupai(param);
                    break;
                case EnumRequest.QiangGangHu:
                    break;
                case EnumRequest.LiangCard:
                    break;
                case EnumRequest.HaiDi:
                    break;
                case EnumRequest.XFG:
                    if (OnSomeOneNeedCard != null)
                    {
                        OnSomeOneNeedCard();
                    }
                    for (int i = 0, lenth = Players.Length; i < lenth; i++)
                    {
                        MahjongPlayer player = Players[i];
                        if (player != null)
                        {
                            player.TryAddLastGetInCard();
                        }
                    }
                    Players[reqData.Sit].OnGetXFGang(reqData);
                    SelfPlayer.ClearMenu();
                    CheckOp(reqData.Op, reqData.Sit, reqData.OpCard);
                    break;
                case EnumRequest.Ting:
                    SelfPlayer.ClearMenu();
                    TingThrowOutCard(reqData);
                    CheckOp(reqData.Op, SelfPlayer.UserSeat, _checkCard);
                    break;
                case EnumRequest.CaiGang:
                    if (OnSomeOneNeedCard != null)
                    {
                        OnSomeOneNeedCard();
                    }
                    for (int i = 0, lenth = Players.Length; i < lenth; i++)
                    {
                        MahjongPlayer player = Players[i];
                        if (player != null)
                        {
                            player.TryAddLastGetInCard();
                        }
                    }
                    Players[reqData.Sit].OnGetCaiGang(reqData);
                    CheckOp(reqData.Op, reqData.Sit, reqData.OpCard);
                    break;
                case EnumRequest.GuoDan:
                    Players[reqData.Sit].SetDanState(reqData.Card);
                    int[] guoDanSocre ;
                    GameTools.TryGetValueWitheKey(param,out guoDanSocre,RequestKey.KeyDanScore);
                    if (guoDanSocre.Length > 0)
                    {
                        int max = guoDanSocre.Length;
                        for (int i = 0; i < max; i++)
                        {
                            int danNum = guoDanSocre[i];
                            if (danNum != 0)
                            {
                                Players[i].ShowDanEffect(danNum);
                            }
                        }
                    }
                    CheckOp(reqData.Op, reqData.Sit, reqData.OpCard);
                    SelfPlayer.ThrowCardAuto();
                    break;
                case EnumRequest.FenZhang:
                    FenZhang(reqData);
                    break;
                case EnumRequest.LiGang:
                    LiGang(reqData);
                    break;
                case EnumRequest.Auto:
                    OnRobotToggle(reqData.Data);
                    break;
                default:
                    YxDebug.LogError("unknow type");
                    break;
            }
        }

        private void ClearCheckTypeOnLoseToken(int loseSeat)
        {
            if (loseSeat == SelfPlayer.UserSeat)
            {
                _checkType = 0;
            }
        }

        public void CheckOp(int checkType, int checkSeat, int opCard,bool isRejoin=false)
        {
            if (checkType == 0)
            {
                return;
            }
            else
            {
                if (checkSeat != SelfPlayer.UserSeat)
                {
                    if (DirectionResetOnCheckOp)
                    {
                        GameTable.Instance.PlayerDirection.ResetDNXBState(false);
                        CountDownGo.Instance.Stop(true);
                    }
                    _checkType = 0;
                    return;
                }
                if (!isRejoin&& _dealData!=null&& _dealData.Data!=null)
                {
                    TryCheckLiList(_dealData.Data);
                    CheckGangType(_dealData.Data);
                }
                _checkType = checkType;
                _checkCard = opCard;
                SelfPlayer.ShowMenuByCheck(checkType, opCard, CurrentPosition);
            }
        }

        public void ReCheckOp()
        {
            CheckOp(_checkType, SelfPlayer.UserSeat, _checkCard);
        }

        public void SendDataToServer(ISFSObject data)
        {
            App.GetRServer<Mahjong2DGameServer>().SendGameRequest(data);
        }

        int count = 0;
        private float startTime;
        private float EndTime;

        public void OnNeedCard()
        {
#if (UNITY_STANDALONE_WIN&&LOCAL_DEBUG)||UNITY_EDITOR
            YxWindowManager.OpenWindow("CardHelpWindow");
#endif
        }

        public void OnAutoNeed()
        {
#if (UNITY_STANDALONE_WIN && LOCAL_DEBUG) || UNITY_EDITOR
            RobotUi.Instance.ToggleRobot();
#endif
        }

        /// <summary>
        /// 摇骰子时显示开场动画，只有在游戏第一次开始时显示。
        /// </summary>
        /// <param name="sfsObj"></param>
        public void RollDice(ISFSObject sfsObj)
        {
            int startP;
           
            if (Data.IsFirstTime)
            {
                GameBegin.Instance.PlayAni();
                Facade.Instance<MusicManager>().Play(ConstantData.VoiceGameBegin);
                HelpInfoControl.Instance.OnClickChangeInfo();
            }
            GameTools.TryGetValueWitheKey(sfsObj, out startP, RequestKey.KeyStartPosition);
            ShowLastRoundNotice();
            SetZhuangAndPosition(startP);
            GameTotalState = TotalState.Gaming;
            if (!GameType.NowRound.Equals(GameType.TotalRound))
            {               
                GameType.NowRound += 1;
            }
            else
            {
                if (Data.IsQuanExist)
                {
                    GameType.NowRound += 1;
                }
            }
            GameTable.Instance.RoundLabel.TrySetComponentValue(GameType.ShowRoundInfo);
        }

        private void SetZhuangAndPosition(int startP)
        {
            StartPosition = startP;
            GameTable.Instance.SetPlayerDir(-CurDirectionIndex, Data.IsFirstTime);
        }

        // 发牌
        private void AlloCate(RequestData request)
        {
            SelfPlayer.SetCards(request.Cards);
            foreach (MahjongPlayer player in Players)
            {
                player.AllocateCards();
            }
            SelfPlayer.SortCardOnSureLaizZi();
        }

        private void ShowLastRoundNotice()
        {
            if (IsLastRound)
            {
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(OnLastRoundAction.WaitExcuteCalls());
                }
            }
        }

        private void GetHun(bool immediate, bool RefreshWait = false)
        {
            if (LaiZiNum != 0)
            {
                YxDebug.Log("获得癞子牌");
                if (FanNum != 0)
                {
                    LaiZiControl.Instance.CreateMahjong(FanNum, immediate);
                }
                if (RefreshWait)
                {
                    Invoke("SordCardSureLaiZi", 1);
                    return;
                }
            }
            SordCardSureLaiZi();
        }

        private void SordCardSureLaiZi()
        {
            CancelInvoke("SordCardSureLaiZi");
            SelfPlayer.SortCardOnSureLaizZi();
        }

        // 收到吃碰杠胡回包
        private void AllowResponse(RequestData reqData,bool showBehavior)
        {
            if (OnSomeOneNeedCard != null)
            {
                OnSomeOneNeedCard();
            }
            CountDownGo.Instance.Begin();
            for (int i = 0, lenth = Players.Length; i < lenth; i++)
            {
                MahjongPlayer player = Players[i];
                if (player != null)
                {
                    player.TryAddLastGetInCard();
                }
            }
            CurrentPosition = reqData.Sit;
            Players[CurrentPosition].AllowRequest(reqData, showBehavior);
            SelfPlayer.ClearMenu();
        }

        public int CurrentPosition
        {
            get { return App.GetGameData<Mahjong2DGameData>().mCurrentPosition; }

            set
            {
                if (App.GetGameData<Mahjong2DGameData>()==null)
                {
                    YxDebug.LogError("GlobalData is null when Set CurrentPosition,please check");
                }
                if (Players == null)
                {
                    YxDebug.LogError("Players is null when Set CurrentPosition,please check");
                }
                App.GetGameData<Mahjong2DGameData>().mCurrentPosition = value;
                GameTable.Instance.ChangeDir(Players[CurrentPosition].ShowDirectionIndex);
            }
        }


        /// <summary>
        /// 某个玩家收到发牌,为了处理打牌与落牌节奏，将抓牌分为其他人落牌与自己抓牌两部分处理。
        /// 为了保持游戏节奏的同步性，防止从后台状态切换回游戏后的消息问题做了如下处理
        /// </summary>
        public void GetInCard(RequestData data)
        {
            _dealData = data;
            if (!ThrowCardAutoDown)
            {
                ThrowOutCardWhenGetInCard();
            }
            SomeOneGetCard();
            SetGangdi(data.Data);
        }

        /// <summary>
        /// 检测立杠列表
        /// </summary>
        /// <param name="data"></param>
        private void TryCheckLiList(ISFSObject data)
        {
            if (data!=null&&data.ContainsKey(ConstantData.KeyGangList))
            {
                LiGangList.Clear();
                var array=data.GetIntArray(ConstantData.KeyGangList);
                LiGangList.AddRange(array);
                foreach (var ligangItem in LiGangList)
                {
                    YxDebug.LogError(((EnumMahjongValue)ligangItem).ToString());
                }
            }

        }

        /// <summary>
        /// 允许抓杠
        /// </summary>
        public bool AllowAnGang { private set; get; }
        /// <summary>
        /// 允许暗杠
        /// </summary>
        public bool AllowZhuaGang { private set; get; }

        /// <summary>
        /// 是否筛选杠牌
        /// </summary>
        public bool FilTrateGang { get { return AllowZhuaGang || AllowZhuaGang; }}

        /// <summary>
        /// 检测是否允许抓杠和暗杠
        /// </summary>
        /// <param name="data"></param>
        private void CheckGangType(ISFSObject data)
        {
            if (data != null)
            {
                AllowAnGang = false;
                AllowZhuaGang = false;
                if (data.ContainsKey(ConstantData.KeyGangType))
                {
                    var value = data.GetInt(ConstantData.KeyGangType);
                    var anGangState = 1 << 1;
                    var zhuaGangState = 1 << 2;
                    AllowAnGang = (value & anGangState) == anGangState;
                    AllowZhuaGang = (value & zhuaGangState) == zhuaGangState;
                }
            }
        }

        List<KeyValuePair<int,int>> _fenzhangList=new List<KeyValuePair<int, int>>(); 
        /// <summary>
        /// 分张
        /// </summary>
        /// <param name="data"></param>
        public void FenZhang(RequestData data)
        {
            _fenzhangList = data.FenZhangData.ToList();
        }

        /// <summary>
        /// 立杠回调
        /// </summary>
        /// <param name="data"></param>
        public void LiGang(RequestData data)
        {
            SelfPlayer.ClearMenu();
            AllowResponse(data,false);
            Players[data.Sit].HasTing = true;
            Players[data.Sit].ShowTingWithEffect(false);
            Players[CurrentPosition].CpghBehavior.SetBehavior(EnumCpgType.LiGang);
            CheckOp(data.Op, SelfPlayer.UserSeat, _checkCard);
        }

        private void LocalStateChange(bool state)
        {
            if (SelfPlayer)
            {
                OnAutoStateChange(SelfPlayer.UserSeat, state,true);
            }
        }

        private void OnAutoStateChange(int seat,bool autoState,bool clearOp=false)
        {
            if (Players!=null&& Players[seat]!=null)
            {
                Players[seat].ChangeAutoState(autoState);
            }
            if (SelfPlayer != null)
            {
                if (seat == SelfPlayer.UserSeat)
                {
                    if (!AutoStateByLocal)
                    {
                        RobotUi.Instance.ShowAutoState(autoState);
                        Data.IsInRobot = autoState;
                    }
                    if (autoState)
                    {
                        if (clearOp)
                        {
                            _checkType = 0;
                        }
                        SelfPlayer.OnCancelClick();
                    }
                    SelfPlayer.ReShowMenu();
                }
            }
        }


        /// <summary>
        /// 当前处理的数据
        /// </summary>
        private RequestData _dealData;

        /// <summary>
        /// 打牌
        /// </summary>
        /// <param name="direct">是否直接打牌</param>
        private void ThrowOutCardWhenGetInCard()
        {
            if (Players[_dealData.Sit].LastOutCard != null)
            {
                Players[_dealData.Sit].OutCardDown(true);
            }
            if (!CurrentPosition.Equals(_dealData.Sit))         //非当前玩家抓牌
            {
                _lastOutCardUserSeatID = CurrentPosition;
                Players[_lastOutCardUserSeatID].LostToken();
                ClearCheckTypeOnLoseToken(_lastOutCardUserSeatID);
                Players[_lastOutCardUserSeatID].OutCardDown();
            }
        }

        /// <summary>
        /// 某个玩家获得手牌
        /// </summary>
        public void SomeOneGetCard()
        {
            SelfPlayer.ClearMenu();
            CurrentPosition = _dealData.Sit;
            _checkCard = _dealData.OpCard;
            Players[CurrentPosition].GetInCard(_checkCard);
            CountDownGo.Instance.Begin();
            CheckOp(_dealData.Op, _dealData.Sit, _dealData.OpCard);
            if (_dealData.Sit.Equals(CurrentSeat))
            {
                SelfPlayer.ThrowCardAuto();
            }
        }

        /// <summary>
        /// 真正意义的打出去
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="param"></param>
        public void ThrowOutCard(RequestData reqData)
        {
            DealOutCard(reqData.Sit, reqData.OpCard);
            GenZhuang(reqData);
            DealZhangMao(reqData.Data);
        }
        /// <summary>
        ///打牌处理
        /// </summary>
        /// <param name="sit"></param>
        /// <param name="checkCard"></param>
        private void DealOutCard(int sit,int checkCard)
        {
            foreach (var player in Players)
            {
                if (player.LastOutCard != null)
                {
                    player.OutCardDown(true);
                }
            }
            _checkCard = checkCard;
            Players[sit].OnThrowCard(_checkCard, delegate
            {
                if (ThrowCardAutoDown)
                {
                    if (Players[sit].LastOutCard != null)
                    {
                        Players[sit].OutCardDown();
                        _lastOutCardUserSeatID = sit;
                        Players[sit].LostToken();
                        ClearCheckTypeOnLoseToken(sit);
                    }
                }
            });
        }

        // 获得上次打出的牌 用以吃碰杠胡补后把这张牌移到对应操作家
        public MahjongItem GetLastOutCardItem(int value)
        {
            foreach (var player in Players)
            {
                if (player.LastOutCard != null)
                {
                    MahjongItem lastItem = player.LastOutCard;
                    if (lastItem.Value.Equals(value))
                    {
                        YxDebug.Log(string.Format("{0}找到最后一张打出的牌{1}", player.UserInfo.name, (EnumMahjongValue)lastItem.Value));
                        player.LastOutCard = null;
                        return lastItem;
                    }
                }
            }
            return null;
        }

        // 获得剩余牌数
        public int GetLeftNum()
        {
            return App.GetGameData<Mahjong2DGameData>().LeftNum;
        }

        /// <summary>
        /// 获得最新的一张牌
        /// </summary>
        /// <param name="state">是否进行显示，癞子牌进行特殊处理</param>
        /// <returns></returns>
        public MahjongItem GetNextMahjong(bool numberChange = true)
        {
            if (numberChange)
            {
                if (Data != null)
                {
                    Data.LeftNum--;
                }
                GameTable.Instance.UpdateLeftNum(GetLeftNum());
            }
            MahjongItem item = GameRes.Instance.GetNewMahjong();
            return item;
        }

        // 找到并且高亮显示牌值相同的所有已经打出的牌
        public void FindVisibleCard(int value)
        {
            ClearFlagCard();
            foreach (MahjongPlayer player in Players)
            {
                if (player != null)
                {
                    List<Transform> list = player.MahjongEnv.FindOutCardByValue(value);
                    foreach (var item in list)
                    {
                        item.GetComponent<MahjongItem>().SetColor(Color.green);
                    }
                }
            }
        }

        // 清理之前高亮显示的牌
        public void ClearFlagCard()
        {
            foreach (MahjongPlayer player in Players)
            {
                if (player != null)
                {
                    if (player.MahjongEnv != null)
                    {
                        player.MahjongEnv.ClearMahjongFlag();
                    }
                }
            }
        }
        /// <summary>
        ///处理TingOutCards
        /// </summary>
        /// <param name="arr"></param>
        public void TryGetTingOutCards(int[] arr)
        {
            if (arr != null)
            {
                if (arr.Length >= 0)
                {
                    TingOutCards = arr.ToList();
                    foreach (var item in TingOutCards)
                    {
                        YxDebug.LogError((EnumMahjongValue)item);
                    }
                }
                else
                {
                    TingOutCards.Clear();
                }
            }
        }

        // ----------------------------------- 大小结算
        // 胡牌了 小结算
        private void OnHupai(ISFSObject param, bool isRunOut = false)
        {
            GameTotalState = TotalState.Account;
            ClearCheckTypeOnLoseToken(SelfPlayer.UserSeat);
            CheckLastRoundInfo(param);
            var fenzhangData = _fenzhangList.ToList();
            OnCheckFenZhang(delegate
            {
                DealBaoData(param);
                ResultLiang(param);
                StartCoroutine(ShowResult(param, fenzhangData, isRunOut));
            });
        }

        private void CheckLastRoundInfo(ISFSObject data)
        {
            if (data.ContainsKey(RequestKey.KeyIsLastRound))
            {
                bool isLast;
                GameTools.TryGetValueWitheKey(data, out isLast, RequestKey.KeyIsLastRound);
                YxDebug.LogError("收到最后一局状态信息，值为" + isLast);
                IsLastRound = isLast;
            }
        }

        private Coroutine _fenZhangCor;
        private void OnCheckFenZhang(AsyncCallback finishCall)
        {
            if(_fenzhangList.Count>0)
            {
                if (_fenZhangCor!=null)
                {
                    StopCoroutine(_fenZhangCor);
                }
                _fenZhangCor=StartCoroutine(ShowFenZhang(finishCall));
            }
            else
            {
                if (finishCall!=null)
                {
                    finishCall(null);
                }
            }
        }
        /// <summary>
        ///  分张每家等待时间
        /// </summary>
        WaitForSeconds _fenZhangWaitFor;
        /// <summary>
        /// 分张动画等待时间
        /// </summary>
        private WaitForSeconds _fenZhangNoticeAniTime;
        IEnumerator ShowFenZhang(AsyncCallback finishCall)
        {
            if(gameObject.activeInHierarchy)
            {
                StartCoroutine(OnFenZhangAction.WaitExcuteCalls());
            }
            yield return _fenZhangNoticeAniTime;
            var count = _fenzhangList.Count;
            var lastSeat = CurrentPosition;
            for (int i = 0; i < count; i++)
            {
                var nextSeat = (lastSeat + i + 1)%PlayerNumber;
                var itemIndex = _fenzhangList.FindIndex(keyPair => keyPair.Key == nextSeat);
                if (itemIndex > -1)
                {
                    var item = _fenzhangList[itemIndex];
                    RequestData data = new RequestData(item.Key, item.Value);
                    GetInCard(data);
                    YxDebug.LogError(string.Format("玩家{0}分张,牌值是:{1}",Players[item.Key].UserInfo.name,(EnumMahjongValue)item.Value));
                    yield return _fenZhangWaitFor;
                }
            }
            _fenzhangList.Clear();
            if (finishCall != null)
            {
                finishCall(null);
            }
        }

        [Tooltip("检测胡牌类型，在需要单独显示胡牌类型音效时处理,默认值为:海捞-杠开-抢杠胡-清一色-七对-票胡")]
        public List<int> CheckCtypes = new List<int>
        {
            65536,16,1073741824,8,2,128
        };
        [Tooltip("根据Ctype播放胡牌音效")]
        public bool ShowVoiceBtCtype=false;
        public void ResultLiang(ISFSObject param)
        {
            ISFSArray users;
            ISFSArray cards;
            int _huCard;
            GameTools.TryGetValueWitheKey(param, out users, RequestKey.KeyPlayerList);
            GameTools.TryGetValueWitheKey(param, out cards, RequestKey.KeyCardsArr);
            GameTools.TryGetValueWitheKey(param, out _huCard, RequestKey.KeyHuCard);
            for (int i = 0, lenth = users.Size(); i < lenth; i++)
            {
                int type;
                ISFSObject uData = users.GetSFSObject(i);
                GameTools.TryGetValueWitheKey(uData, out type, RequestKey.KeyType);
                List<int> handCards = cards.GetIntArray(i).ToList();
                if (type > 1)
                {
                    string audioName = "";
                    int randNum = 0;
                    EnumCpgType cpgType;
                    if (param.ContainsKey(RequestKey.KeyMoBao))
                    {
                        audioName = RequestKey.KeyMoBao;
                        cpgType = EnumCpgType.MoBao;
                    }
                    else if (param.ContainsKey(RequestKey.KeyPiao))
                    {
                        audioName = RequestKey.KeyPiaoHu;
                        cpgType = EnumCpgType.PiaoHu;
                    }
                    else if (param.ContainsKey(RequestKey.KeyBao))
                    {
                        handCards.Add(_huCard);
                        audioName = RequestKey.KeyMoBao;
                        cpgType = EnumCpgType.ChongBao;
                    }
                    else if (users.GetSFSObject(i).GetBool(RequestKey.KeyLiang))
                    {
                        audioName = ConstantData.VoiceZiMo;
                        cpgType = EnumCpgType.NiuBiHu;
                        randNum = Random.Range(0, 3);
                    }
                    else
                    {
                        audioName = ConstantData.VoiceZiMo;
                        cpgType = EnumCpgType.ZiMo;
                        randNum = Random.Range(0, 3);
                    }
                    Players[i].ShowLiangPaiAtGameEnd(handCards, type.Equals(2), _huCard);
                    //胡牌音效特效
                    if (ShowVoiceBtCtype)
                    {
                        int ctype;
                        GameTools.TryGetValueWitheKey(uData, out ctype, RequestKey.KeyHuType);
                        PlayeCheckVoice(true, ctype, Players[i].UserInfo.Sex);
                    }
                    else
                    {
                        Facade.Instance<MusicManager>().Play(GameTools.GetOperationVoice(Players[i].UserInfo.Sex, audioName, randNum));
                    }
                    Players[i].ShowCpgh(cpgType);
                }
                else
                if (type == 1)
                {
                    string audioName = "";
                    int randNum = 0;
                    EnumCpgType cpgType;
                    if (param.ContainsKey(RequestKey.KeyPiao))
                    {
                        audioName = RequestKey.KeyPiaoHu;
                        cpgType = EnumCpgType.PiaoHu; 
                    }
                    else
                    if (users.GetSFSObject(i).GetBool(RequestKey.KeyLiang))
                    {
                        audioName = ConstantData.VoiceHu;
                        cpgType = EnumCpgType.NiuBiHu;
                    }
                    else
                    {
                        audioName = ConstantData.VoiceHu;
                        cpgType = EnumCpgType.Hu;

                    }
					randNum = Random.Range(0, 2);
                    int ctype;
                    GameTools.TryGetValueWitheKey(uData, out ctype, RequestKey.KeyHuType);
                    if((ctype&ConstantData.GangHu)==1)
                    {
                        cpgType = EnumCpgType.QiangGangHu;
                    }
                    Players[i].ShowCpgh(cpgType);
                    //胡牌音效特效
                    if (ShowVoiceBtCtype)
                    {
                        PlayeCheckVoice(false, ctype, Players[i].UserInfo.Sex);
                    }
                    else
                    {
                        Facade.Instance<MusicManager>().Play(GameTools.GetOperationVoice(Players[i].UserInfo.Sex, audioName, randNum));
                    }
                    Players[i].ShowLiangPaiAtGameEnd(handCards, type.Equals(2), _huCard);
                }
                else
                {
                    Players[i].ShowLiangPaiAtGameEnd(handCards, type.Equals(2), 0);
                }
            }
        }

        /// <summary>
        /// 播放检测音效
        /// </summary>
        /// <param name="zimo"></param>
        /// <param name="checkType"></param>
        /// <param name="sex"></param>
        private void PlayeCheckVoice(bool zimo,int checkType,int sex)
        {
            string playVoiceName= zimo? ConstantData.VoiceZiMo : ConstantData.VoiceHu;
            for (int i = 0; i < CheckCtypes.Count; i++)
            {
                var checkItem = CheckCtypes[i];
                if ((checkItem&checkType)==checkItem)
                {
                    playVoiceName = string.Format(ConstantData.HuFormat,checkItem);
                    break;
                }
            }
            Facade.Instance<MusicManager>().Play(GameTools.GetOperationVoice(sex, playVoiceName, 0));
            
        }

        /// <summary>
        /// 小结算显示时间
        /// </summary>
        [SerializeField]
        private float ResultShowTime = 3;
        private IEnumerator ShowResult(ISFSObject param, List<KeyValuePair<int, int>> fenZhangData, bool isRunOut = false)
        {
            yield return new WaitForSeconds(ResultShowTime);
            foreach (var player in Players)
            {
                if (player)
                {
                    player.CurrentInfoPanel.Refresh(IsGameing);
                }
            }
            GameResult.Instance.ShowResultPanel(param, fenZhangData, isRunOut);
        }

        /// <summary>
        /// 游戏结束大结算，收到大结算消息十五秒后如果小结算界面仍然显示，就先关闭小结算界面
        /// </summary>
        /// <param name="param"></param>
        public void OnGameOver(ISFSObject param)
        {
            YxDebug.LogError("---------------------------- Mahjong2DGameManager.GameOver()!---------------------------- ");
            GameTotalState = TotalState.Over;
            Facade.EventCenter.DispatchEvent(RequestCmd.GameOver,0);
            GameOver.Instance.SetUserOverData(param);
            Invoke("ShowGameOver", 15);
        }
        public void ShowGameOver()
        {
            CancelInvoke("ShowGameOver");
            GameResult.Instance.CloseWindow();
            GameOver.Instance.ShowGameOverPanel();
        }

        // ----------------------------------- 连局
        public void ContinueGame()
        {
            GameTotalState = TotalState.Waiting;
        }

        // ----------------------------------- 玩家被动退出 如果是防作弊，有人离开房间，所有玩家都需要退出房间


        public void OnClickDetail(GameObject obj)
        {
            OnClickUserHead();
        }

        private int numberCount;
        /// <summary>
        /// GPS信息
        /// </summary>
        public void OnClickUserHead()
        {
            numberCount = 0;
            if (GpsInfosCtrl.Instance.IsShow)
            {
                GpsInfosCtrl.Instance.Hide();
                for (int i = SelfPlayer.UserSeat, max = Players.Length; numberCount < max; i = (i + 1) % max, numberCount++)
                {
                    MahjongPlayer player = Players[i];
                    if (player == null || player.UserInfo == null)
                    {
                        continue;
                    }
                    var desLabel = player.CurrentInfoPanel.DesLabel;
                    var line = player.CurrentInfoPanel.GpsLine;
                    if (desLabel)
                    {
                        desLabel.gameObject.SetActive(false);
                    }
                    if (line)
                    {
                        line.SetActive(false);
                    }
                }
            }
            else
            {
                GpsInfosCtrl.Instance.Show();
                List<GpsUser> users = new List<GpsUser>();
                int uiTotal = 4;
                for (int i = CurrentSeat, max = Players.Length; numberCount < max; i = (i + 1) % max, numberCount++)
                {
                    if (i.Equals(CurrentSeat))
                    {
                        continue;
                    }
                    MahjongPlayer player = Players[i];
                    if (player != null && player.CurrentInfoPanel != null)
                    {
                        var infoPanel = Players[i].CurrentInfoPanel;
                        var userinfo = infoPanel.UserInfo;
                        if (userinfo == null || !infoPanel.gameObject.activeInHierarchy)
                        {
                            continue;
                        }
                        int nextSeat = (player.ShowSeat + 1) % uiTotal;
                        if (nextSeat.Equals(SelfPlayer.ShowSeat))
                        {
                            nextSeat = (player.ShowSeat + 2) % uiTotal;
                        }
                        GpsUser user = new GpsUser(userinfo.IsHasGpsInfo, userinfo.GpsX, userinfo.GpsY, infoPanel.DistanceLabel, infoPanel.GpsLine, player.ShowSeat, nextSeat);
                        users.Add(user);
                    }
                }
                GpsInfosCtrl.Instance.ShowDistince(users.ToArray());
                numberCount = 0;
                for (int i = SelfPlayer.UserSeat, max = Players.Length; numberCount < max; i = (i + 1) % max, numberCount++)
                {
                    MahjongPlayer player = Players[i];
                    if (player == null || player.UserInfo == null)
                    {
                        continue;
                    }
                    if (GpsInfosCtrl.Instance.IsShow)
                    {
                        player.CurrentInfoPanel.ShowAddressInfo();
                    }
                    else
                    {
                        player.CurrentInfoPanel.DesLabel.gameObject.SetActive(false);
                    }
                }
            }
        }
        /// <summary>
        /// 要求在小结算之后的头像状态显示为黑色状态，呵呵
        /// </summary>
        public void ResetReadyState()
        {
            foreach (var player in Players)
            {
                if (player != null && player.CurrentInfoPanel != null && player.UserInfo != null)
                {
                    player.SetUserHead(false, false);
                }
            }
        }

        /// <summary>
        /// 触发托管模式
        /// </summary>
        /// <param name="data"></param>
        private void OnRobotToggle(ISFSObject data)
        {
            int seat;
            bool state;
            GameTools.TryGetValueWitheKey(data, out seat, RequestKey.KeySeat);
            GameTools.TryGetValueWitheKey(data, out state, RequestKey.KeyAuto);
            OnAutoStateChange(seat,state,true);
        }
        /// <summary>
        /// 显示加倍
        /// </summary>
        private void ShowPiaos()
        {
            if (_piaos == null || _piaos.Length == 0)
            {
                return;
            }
            for (int i = 0; i < _piaos.Length; i++)
            {
                Players[i].ShowPiao(_piaos[i]);
            }
        }
        /// <summary>
        /// 听打牌处理
        /// </summary>
        /// <param name="reqData"></param>
        private void TingThrowOutCard(RequestData reqData)
        {
            int seat = reqData.Sit;
            ThrowOutCard(reqData);
            Xs = reqData.Xs;
            Players[seat].ShowTingWithEffect(true);
            if (BaoCards.Count > 0)
            {
                BaoCardsControl.Instance.SetBaos(BaoCards, SelfPlayer.HasTing);
            }
        }
        /// <summary>
        /// 获得宝牌
        /// </summary>
        /// <param name="param"></param>
        public void GetBaoCard(ISFSObject param)
        {
            int seat;
            int lastBao;
            GameTools.TryGetValueWitheKey(param, out seat, RequestKey.KeySeat);
            GameTools.TryGetValueWitheKey(param, out lastBao, RequestKey.KeyLastBao);
            DealBaoData(param);
            YxDebug.LogError(string.Format("收到获得宝牌消息，当前玩家的听状态是："+ SelfPlayer.HasTing));
            BaoCardsControl.Instance.SetBaos(BaoCards, SelfPlayer.HasTing);
            if (lastBao != 0)
            {
                Players[seat].ShowCpgh(EnumCpgType.HuanBao);
                Players[seat].ThrowBaoCard(lastBao, 2);
            }
            App.GetGameData<Mahjong2DGameData>().LeftNum--;

        }
        /// <summary>
        /// 处理宝数据
        /// </summary>
        /// <param name="obj"></param>
        public void DealBaoData(ISFSObject obj)
        {
            BaoCards.Clear();
            if (Data.IsShuangBao)
            {
                int[] baos;
                GameTools.TryGetValueWitheKey(obj, out baos, RequestKey.KeyBao);
                if (baos.Length > 0)
                {
                    YxDebug.LogError(string.Format("双宝"));
                    foreach (var bao in baos)
                    {
                        if (bao > 0)
                        {
                            BaoCards.Add(bao);
                        }
                        YxDebug.LogError(string.Format("宝牌是：{0},{1}", bao, (EnumMahjongValue)bao));
                    }
                }
            }
            else
            {
                int bao;
                GameTools.TryGetValueWitheKey(obj, out bao, RequestKey.KeyBao);
                if (bao > 0)
                {
                    BaoCards.Add(bao);
                }

            }
        }
        /// <summary>
        /// 处理长毛显示效果
        /// </summary>
        public void DealZhangMao(ISFSObject data)
        {
            int mao;
            GameTools.TryGetValueWitheKey(data,out mao,RequestKey.KeyZhangMao);
            if(mao>0)
            {
                ZhangMaoShow.Instance.ShowZhangMaoEffect();
            }
        }
        #region GPS 相关

        public void CheckGpsInfo(ISFSObject data)
        {
            int userID = data.GetInt(ConstantData.KeyUserId);
            for (int i = 0, max = Players.Length; i < max; i++)
            {
                MahjongPlayer player = Players[i];
                if (player == null)
                {
                    continue;
                }
                if (player.UserInfo != null && player.UserInfo.id.Equals(userID) && player.CurrentInfoPanel.gameObject.activeInHierarchy)
                {
                    player.UserInfo.SetGpsData(data);
                }
            }
        }
        #endregion

        /// <summary>
        /// 跟庄的显示
        /// </summary>
        /// <param name="reqData"></param>
        public void GenZhuang(RequestData reqData)
        {
            if (reqData.GenZhuang.Length != 0)
            {
                 _genZhuangNum = reqData.GenZhuang.Length;
                  for (int i = 0; i < _genZhuangNum; i++)
                  {
                      Players[i].CpghBehavior.ShowPiaoLabel(reqData.GenZhuang[i]);
                      Players[i].CurrentInfoPanel.AddGold(reqData.GenZhuang[i]);
                  }
            }
        }
        /// <summary>
        /// 玩家状态变化
        /// </summary>
        /// <param name="param"></param>
        public void OnPlayerStateChange(int seat,bool state)
        {
            if(Players!=null&&Players.Length>0)
            {
                var player = Players[seat];
                if (player&& player.UserInfo!=null)
                {
                    player.UserInfo.IsOnLine = state;
                    player.SetUserHead(state, true);
                    if (!IsGameing)
                    {
                        player.FreshReadyState();
                    }
                }
            }
        }
        
        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            ChatControl.Instance.RequestSoundkey();
            StopAllCoroutines();
            CheckLastRoundInfo(gameInfo);
            Data.InitData(gameInfo);
            Init();
            if (!Data.IsFirstTime)
            {
                OnReJoin(gameInfo);
            }
            IsInfoInit = true;
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {

        }

        public override void GameStatus(int status, ISFSObject info)
        {
           
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            RequestData reqData = new RequestData(response, SpecialFenZhang);
            ExcuteRequest(reqData);
        }

        public override void UserIdle(int localSeat, ISFSObject responseData)
        {
            base.UserIdle(localSeat, responseData);
            OnPlayerStateChange(localSeat, false);
        }

        public override void UserOnLine(int localSeat, ISFSObject responseData)
        {
            base.UserOnLine(localSeat, responseData);
            OnPlayerStateChange(localSeat, true);
        }

        public override void OnOtherPlayerJoinRoom(ISFSObject sfsObject)
        {
            base.OnOtherPlayerJoinRoom(sfsObject);
            ISFSObject user;
            GameTools.TryGetValueWitheKey(sfsObject, out user, RequestKey.KeyUser);
            UserData data = new UserData(user);
            bool isExist = false;
            for (int i = 0, lenth = Data.UserDatas.Count; i < lenth; i++)
            {
                ISFSObject haveUser = Data.UserDatas[i];
                UserData haveData = new UserData(haveUser);
                if (haveData.Seat.Equals(data.Seat))
                {
                    Data.UserDatas[i] = user;
                    isExist = true;
                    break;
                }
            }
            if (!isExist)
            {
                Data.UserDatas.Add(user);
            }
            OnJoinGame(data);
        }

        public override void UserOut(int localSeat, ISFSObject responseData)
        {
            if (Data == null || Data.UserDatas == null || Data.UserDatas.Count == 0)
            {
                return;
            }
            base.UserOut(localSeat, responseData);
            int outSeat = responseData.GetInt("seat");
            List<ISFSObject> list = Data.UserDatas.ToList();
            Data.UserDatas.Clear();
            foreach (var data in list)
            {
                int seat = data.GetInt(RequestKey.KeySeat);
                if (outSeat.Equals(seat))
                {
                    continue;
                }
                Data.UserDatas.Add(data);
            }
            Players[outSeat].OnUserLeave();

        }

        public override void OnDestroy()
        {
            Facade.EventCenter.RemoveEventListener<string,bool>(ConstantData.KeyRobotToggle, LocalStateChange);
            base.OnDestroy();
        }

        public void OnClickHelpBtn(string windowName)
        {
#if YX_DEVE
           YxWindowManager.OpenWindow(windowName);
#endif
        }
    }
}
