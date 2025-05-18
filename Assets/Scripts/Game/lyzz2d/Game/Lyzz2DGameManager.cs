using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.lyzz2d.Game.Component;
using Assets.Scripts.Game.lyzz2d.Game.GameCtrl;
using Assets.Scripts.Game.lyzz2d.Game.Item;
using Assets.Scripts.Game.lyzz2d.Game.UI;
using Assets.Scripts.Game.lyzz2d.Utils;
using Assets.Scripts.Game.lyzz2d.Utils.Gps;
using Assets.Scripts.Game.lyzz2d.Utils.UI;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Game.lyzz2d.Game
{
    // 麻将，顺拿逆打，牌堆是顺时针拿牌的，打牌是逆时针的
    // 注意：长分割线注释是按照游戏开始到结束流程划分，添加或修改代码的时候不要乱写！
    public class Lyzz2DGameManager : YxGameManager
    {
        /// <summary>
        ///     当前用来检查的牌
        /// </summary>
        private int _checkCard;

        /// <summary>
        ///     当前用来检查的类型
        /// </summary>
        private int _checkType;

        /// <summary>
        ///     准备菜单
        /// </summary>
        [SerializeField] private CpghChar[] _cpghChars;

        /// <summary>
        ///     游戏焦点
        /// </summary>
        private bool _focus = false;

        /// <summary>
        ///     判断本局是否显示过买断门，用来控制漂显示
        /// </summary>
        private bool _isShowDuanMen;

        /// <summary>
        ///     打出最后一张牌的玩家的座位号（实际座位），用来从对应玩家处获得打出
        /// </summary>
        private int _lastOutCardUserSeatID;

        /// <summary>
        ///     暂停状态
        /// </summary>
        private bool _pause = false;

        /// <summary>
        ///     临时存储的加刚信息
        /// </summary>
        private int[] _piaos;

        /// <summary>
        ///     玩家头像
        /// </summary>
        [SerializeField] private UserInfoPanel[] _userinfoPanels;

        private int count = 0;
        private float EndTime;

        /// <summary>
        ///     翻牌
        /// </summary>
        [HideInInspector] public int FanNum;

        /// <summary>
        ///     游戏状态
        /// </summary>
        private TotalState gameState;

        [HideInInspector]
        /// <summary>
        /// 墙牌的获得位置（未使用）
        /// </summary>
        public int getPosition;

        /// <summary>
        ///     是否进行过初始化，防止在没有收到GameInfo时就收到其他的response
        /// </summary>
        [HideInInspector] public bool IsInitInfo;

        /// <summary>
        ///     是否播放分牌动画
        /// </summary>
        public bool IsShowAllocateAnimation = false;

        /// <summary>
        ///     癞子牌
        /// </summary>
        [HideInInspector] public int LaiZiNum;

        [SerializeField]
        /// <summary>
        /// 左边
        /// </summary>
        private MahjongPlayer LeftPlayer;

        private int numberCount;

        /// <summary>
        ///     某个人需要一张牌，用来吃碰杠
        /// </summary>
        public Action OnSomeOneNeedCard;

        /// <summary>
        ///     某个人打牌
        /// </summary>
        public Action OnSomeOneThrowCard;

        [SerializeField]
        /// <summary>
        /// 对面
        /// </summary>
        private MahjongPlayer OppsetPlayer;

        [HideInInspector]
        /// <summary>
        /// 用于处理游戏中玩家相关的数据，与对应方向的玩家相关联
        /// </summary>
        public MahjongPlayer[] Players;

        /// <summary>
        ///     小结算显示时间
        /// </summary>
        [SerializeField] private readonly float ResultShowTime = 3;

        [SerializeField]
        /// <summary>
        /// 右边
        /// </summary>
        private MahjongPlayer RightPlayer;

        /// <summary>
        ///     摇骰子的数量
        /// </summary>
        [HideInInspector] public int RollDicNum;

        /// <summary>
        ///     当前
        /// </summary>
        public MahjongPlayerCtrl SelfPlayer;

        private float startTime;

        /// <summary>
        ///     是否游戏中
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
                        YxDebug.Log("Init");
                        MenuController.Instance.ChangeBtnStatusAfterStartingGame();
                        break;
                    case TotalState.Waiting:
                        YxDebug.Log("waitting");
                        MenuController.Instance.ChangeBtnStatusAfterStartingGame();
                        Reset();
                        App.GetRServer<Lyzz2DGameServer>().Ready();
                        break;
                    case TotalState.Gaming:
                        YxDebug.Log("Gaming");
                        MenuController.Instance.ChangeBtnStatusAfterStartingGame();
                        foreach (var player in Players)
                        {
                            if (player != null && player.UserInfo != null)
                            {
                                player.HideReady();
                                player.SetUserHead(player.UserInfo.IsOnLine, true);
                            }
                        }
                        break;
                    case TotalState.Account:
                        SelfPlayer.ClearMenu();
                        foreach (var player in Players)
                        {
                            if (player != null && player.UserInfo != null)
                            {
                                player.HasTing = false;
                            }
                        }
                        ShowChiPaiInfo.Instance.Show(false);
                        break;
                    case TotalState.Over:
                        break;
                }
            }
        }

        public Lyzz2DGlobalData Data
        {
            get { return App.GetGameData<Lyzz2DGlobalData>(); }
        }

        public Lyzz2DGameServer Lyzz2DGame
        {
            get { return App.GetRServer<Lyzz2DGameServer>(); }
        }

        public CurrentGameType GameType
        {
            get { return Data.CurrentGame; }
        }

        public List<int> TingOutCards
        {
            get { return Data.TingOutCards; }
            set { Data.TingOutCards = value; }
        }

        public bool ChatVioceToggle
        {
            set { Data.IsChatVoiceOn = value; }
            get { return Data.IsChatVoiceOn; }
        }

        [HideInInspector]
        /// <summary>
        /// 本圈的庄
        /// </summary>
        public int StartPosition
        {
            get { return Data.Bank; }
            set
            {
                Players[Data.Bank].IsZhuang = false;
                DealQuan(value);
                Data.Bank = value;
                Players[Data.Bank].IsZhuang = true;
            }
        }

        /// <summary>
        ///     当前玩家座位号
        /// </summary>
        public int CurrentSeat
        {
            get { return SelfPlayer.UserSeat; }
        }

        /// <summary>
        ///     当前游戏需要玩家人数
        /// </summary>
        public int PlayerNumber
        {
            get { return Data.PlayerNum; }
        }

        public int CurSeat
        {
            get { return SelfPlayer.UserSeat; }
        }

        /// <summary>
        ///     当前牌桌中一圈最后一个玩家的实际座位号
        /// </summary>
        public int QuanLastSeat
        {
            get { return (PlayerNumber - 1 + Data.Bank0)%PlayerNumber; }
        }

        /// <summary>
        ///     牌局中是否有人有宝
        /// </summary>
        [HideInInspector]
        public bool IsTingExist { set; get; }


        public int CurrentPosition
        {
            get { return App.GetGameData<Lyzz2DGlobalData>().mCurrentPosition; }

            set
            {
                App.GetGameData<Lyzz2DGlobalData>().mCurrentPosition = value;
                GameTable.Instance.ChangeDir((value - Data.Bank0 + Players.Length*4)%Players.Length);
            }
        }

        public bool IsGameing()
        {
            return GameTotalState == TotalState.Gaming;
        }

        public bool IsWaiting()
        {
            return GameTotalState == TotalState.Waiting;
        }

        public bool IsInit()
        {
            return GameTotalState == TotalState.Init;
        }

        public bool IsAccount()
        {
            return GameTotalState == TotalState.Account;
        }

        public bool IsGameOver()
        {
            return GameTotalState == TotalState.Over;
        }

        /// <summary>
        ///     初始化牌桌中的信息
        /// </summary>
        public void Init()
        {
            Reset();
            InitState();
            InitPlayers();
            GameTable.Instance.InitInfoPanel();
            Ready();
        }

        /// <summary>
        ///     计算出显示位置
        /// </summary>
        /// <param name="userSeat"></param>
        /// <returns></returns>
        public int GetShowSeat(int userSeat)
        {
            var showSeat = 0;
            return (userSeat - CurrentSeat + PlayerNumber)%PlayerNumber;
        }

        /// <summary>
        ///     判断圈数是否增加，需要在局数实际变化前处理
        /// </summary>
        private void DealQuan(int value)
        {
            var bank0 = Data.Bank0;
            if (StartPosition.Equals(QuanLastSeat) && value.Equals(bank0) && !Data.IsFirstTime)
            {
                GameType.Quan += 1;
            }
        }

        /// <summary>
        ///     其它玩家加入游戏
        /// </summary>
        /// <param name="user"></param>
        public void OnJoinGame(UserData user)
        {
            var sit = user.Seat;
            var ShowSeat = GetShowSeat(sit);
            MahjongPlayer player = null;
            switch (PlayerNumber)
            {
                case 2:
                    player = OppsetPlayer.JoinGame(user);
                    break;
                case 3:
                    switch (ShowSeat)
                    {
                        case 1:
                            player = RightPlayer.JoinGame(user);
                            break;
                        case 2:
                            player = LeftPlayer.JoinGame(user);
                            break;
                    }
                    break;
                case 4:
                    switch (ShowSeat)
                    {
                        case 1:
                            player = RightPlayer.JoinGame(user);
                            break;
                        case 2:
                            player = OppsetPlayer.JoinGame(user);
                            break;
                        case 3:
                            player = LeftPlayer.JoinGame(user);
                            break;
                    }
                    break;
                default:
                    player = null;
                    break;
            }
            if (!IsGameing())
            {
                player.Reset();
            }
            var playerNumber = Data.UserDatas.Count;
            if (playerNumber == Data.PlayerNum)
            {
                if (IsInit())
                {
                    GameTotalState = TotalState.Waiting;
                }
            }
            else
            {
                YxDebug.Log(string.Format("人数不满足条件,当前的游戏状态是：{0}，人数是{1}", gameState, playerNumber));
            }
        }

        public void Reset()
        {
            App.GetGameData<Lyzz2DGlobalData>().ResetTotalNumber();
            LaiZiNum = 0;
            FanNum = 0;
            GameTable.Instance.UpdateLeftNum(GetLeftNum());
            if (Players.Length.Equals(PlayerNumber))
            {
                for (var i = 0; i < PlayerNumber; i++)
                {
                    var player = Players[i];
                    if (player != null)
                    {
                        player.Reset();
                    }
                }
            }
            _isShowDuanMen = false;
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
            var dicArray = new int[0];
            int[] tingOutCards;
            int opcard;
            int duanMen;
            GameTools.TryGetValueWitheKey(data, out currentP, RequestKey.KeyCurrentPosition);
            GameTools.TryGetValueWitheKey(data, out roundSeq, RequestKey.KeySeq);
            GameTools.TryGetValueWitheKey(data, out roundSeq2, RequestKey.KeySeq2);
            GameTools.TryGetValueWitheKey(data, out lastOtCdValue, RequestKey.KeyLastOutCard);
            GameTools.TryGetValueWitheKey(data, out lastIn, RequestKey.KeyLastIn);
            GameTools.TryGetValueWitheKey(data, out startP, RequestKey.KeyStartPosition);
            GameTools.TryGetValueWitheKey(data, out dicArray, RequestKey.KeyDiceArray);
            GameTools.TryGetValueWitheKey(data, out FanNum, RequestKey.KeyFan);
            GameTools.TryGetValueWitheKey(data, out LaiZiNum, RequestKey.KeyLaiZi);
            GameTools.TryGetValueWitheKey(data, out lastOutSeat, RequestKey.KeyLastOutSeat);
            GameTools.TryGetValueWitheKey(Data.UserDatas[0], out op, RequestKey.KeyMenuOperation);
            GameTools.TryGetValueWitheKey(data, out duanMen, RequestKey.KeyDuanMen);
            GameTools.TryGetValueWitheKey(Data.UserDatas[0], out tingOutCards, RequestKey.KeyTingOutCards);

            #endregion

            #region 同步

            _lastOutCardUserSeatID = lastOutSeat;
            Data.DuanMenState = duanMen;
            if (IsWaiting())
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
                var userData = Data.UserDatas[i];
                int seat;
                GameTools.TryGetValueWitheKey(userData, out seat, RequestKey.KeySeat);
                Players[seat].OnReJoin(userData);
            }
            GetHun(true);
            opcard = 0;
            if (roundSeq.Equals(roundSeq2)) //不是自己出牌阶段，只是等待响应别人的牌
            {
                if (lastOtCdValue != 0 && lastIn == 0)
                {
                    YxDebug.Log(string.Format("最后一张打出的牌是：{0},座位号是{1}，名字是：{2}，牌是：{3}", lastOtCdValue, lastOutSeat,
                        Players[lastOutSeat].UserInfo.name, (EnumMahjongValue) lastOtCdValue));
                    Players[lastOutSeat].LostToken();
                    Players[lastOutSeat].SetLastOutMahjongShow(lastOtCdValue);
                    opcard = lastOtCdValue;
                }
            }
            else //自己抓牌或者别人抓牌后
            {
                Players[CurrentPosition].GetToken();
                if (lastIn != 0)
                {
                    YxDebug.Log(string.Format("最后一张抓到的牌是：{0},座位号是{1},名字是：{2}", lastIn, CurrentPosition,
                        Players[CurrentPosition].UserInfo.name));
                    Players[CurrentPosition].SetLastInCardOnReJoin(lastIn);
                    opcard = lastIn;
                }
            }
            _checkCard = opcard;
            TingOutCards = tingOutCards.ToList();
            CheckOp(op, SelfPlayer.UserSeat, _checkCard);
            if (Data.DuanMenState.Equals(ConstantData.DuanMenSelect))
            {
                DuanMenControl.Instance.SetDuanMen(0);
            }
            CountDownGo.Instance.Begin(15);
            DealHupInfo(data);

            #endregion
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
                time = (int) (300 - (svt - hupStart));
                time = time < 0 ? 0 : time;
                var ids = hupInfo.Split(',');

                for (var i = 0; i < ids.Length; i++)
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
                            App.GetRServer<Lyzz2DGameServer>().OnHandsUp(hupData);
                        }
                    }
                }
            }
        }

        // ----------------------------------- 网络消息分发
        public void OnServerResponse(ISFSObject param)
        {
        }

        public void CheckOp(int checkType, int checkSeat, int opCard)
        {
            if (checkType == 0)
            {
            }
            else
            {
                if (checkSeat != SelfPlayer.UserSeat)
                {
                    return;
                }
                _checkType = checkType;
                _checkCard = opCard;
                SelfPlayer.ShowMenuByCheck(checkType, opCard, CurrentPosition);
                CountDownGo.Instance.Begin(15);
            }
        }

        public void ReCheckOp()
        {
            CheckOp(_checkType, SelfPlayer.UserSeat, _checkCard);
        }

        public void SendDataToServer(ISFSObject data)
        {
            App.GetRServer<Lyzz2DGameServer>().SendGameRequest(data);
        }

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
        ///     摇骰子时显示开场动画，只有在游戏第一次开始时显示。
        /// </summary>
        /// <param name="sfsObj"></param>
        public void RollDice(ISFSObject sfsObj)
        {
            int startP;
            GameTools.TryGetValueWitheKey(sfsObj, out startP, RequestKey.KeyStartPosition);
            SetZhuangAndPosition(startP);
            GameTotalState = TotalState.Gaming;
            if (Data.IsFirstTime)
            {
                GameBegin.Instance.PlayAni();
                Facade.Instance<MusicManager>().Play(ConstantData.Voice_GameBegin);
            }
            Data.CurrentGame.NowRound += 1;
            GameTable.Instance.RoundLabel.text= Data.CurrentGame.ShowRoundInfo;
        }

        private void SetZhuangAndPosition(int startP)
        {
            StartPosition = startP;
            if (Data.IsFirstTime)
            {
                GameTable.Instance.SetPlayerDir(Data.Bank0 - SelfPlayer.UserSeat, true);
            }
            else
            {
                GameTable.Instance.SetPlayerDir(Data.Bank0 - SelfPlayer.UserSeat);
            }
        }

        // 发牌
        private void AlloCate(RequestData request)
        {
            SelfPlayer.SetCards(request.Cards);
            foreach (var player in Players)
            {
                player.AllocateCards();
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
        private void AllowResponse(RequestData reqData, ISFSObject param)
        {
            if (OnSomeOneNeedCard != null)
            {
                OnSomeOneNeedCard();
            }
            CountDownGo.Instance.Begin(15);
            for (int i = 0, lenth = Players.Length; i < lenth; i++)
            {
                var player = Players[i];
                if (player != null)
                {
                    player.TraAddLastGetInCard();
                }
            }
            CurrentPosition = reqData.Sit;
            Players[CurrentPosition].AllowRequest(reqData, param);
            SelfPlayer.DisabelMenu();
        }

        // 某个玩家收到发牌
        public void GetInCard(int seat, int opCard, ISFSObject param)
        {
            SelfPlayer.ClearMenu();
            if (!CurrentPosition.Equals(seat))
            {
                Players[CurrentPosition].LostToken();
                Players[CurrentPosition].OnOtherGetCard();
            }
            CurrentPosition = seat;
            Players[CurrentPosition].GetInCard(opCard, param);
            CountDownGo.Instance.Begin(15);
        }

        // 某个玩家打出了一张牌
        private void ThrowOutCard(RequestData reqData, ISFSObject param)
        {
            _lastOutCardUserSeatID = reqData.Sit;
            _checkCard = reqData.OpCard;
            if (OnSomeOneThrowCard != null)
            {
                OnSomeOneThrowCard();
            }
            Players[reqData.Sit].OnThrowCard(_checkCard);
        }

        // 获得上次打出的牌 用以吃碰杠胡补后把这张牌移到对应操作家
        public MahjongItem GetLastOutCardItem(int value)
        {
            foreach (var player in Players)
            {
                if (player.LastOutCard != null)
                {
                    var lastItem = player.LastOutCard;
                    if (lastItem.Value.Equals(value))
                    {
                        YxDebug.Log(string.Format("{0}找到最后一张打出的牌{1}", player.UserInfo.name,
                            (EnumMahjongValue) lastItem.Value));
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
            return App.GetGameData<Lyzz2DGlobalData>().LeftNum;
        }

        /// <summary>
        ///     获得最新的一张牌
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
            var item = GameRes.Instance.GetNewMahjong();
            return item;
        }

        // 找到并且高亮显示牌值相同的所有已经打出的牌
        public void FindVisibleCard(int value)
        {
            ClearFlagCard();
            foreach (var player in Players)
            {
                if (player != null)
                {
                    var list = player.MahjongEnv.FindOutCardByValue(value);
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
            foreach (var player in Players)
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


        // ----------------------------------- 大小结算
        // 胡牌了 小结算
        private void OnHupai(ISFSObject param)
        {
            GameTotalState = TotalState.Account;
            ResultLiang(param);
            StartCoroutine(ShowResult(param));
        }

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
                var uData = users.GetSFSObject(i);
                GameTools.TryGetValueWitheKey(uData, out type, RequestKey.KeyType);
                var handCards = cards.GetIntArray(i).ToList();
                if (type > 0)
                {
                    Players[i].ShowLiangPaiAtGameEnd(handCards, type.Equals(2), _huCard);
                    //胡牌音效特效
                    var audioName = type == 1 ? ConstantData.Voice_Hu : ConstantData.Voice_ZiMo;
                    var randNum = type == 1 ? Random.Range(0, 2) : Random.Range(0, 3);
                    Facade.Instance<MusicManager>()
                        .Play(GameTools.GetOperationVoice(Players[i].UserInfo.Sex, audioName, randNum));
                    Players[i].ShowCpgh(type == 1 ? Enum_CPGType.Hu : Enum_CPGType.ZiMo);
                }
                else
                {
                    Players[i].ShowLiangPaiAtGameEnd(handCards, type.Equals(2), 0);
                }
            }
        }

        private IEnumerator ShowResult(ISFSObject param)
        {
            yield return new WaitForSeconds(ResultShowTime);
            GameResult.Instance.ShowResultPanel(param);
        }

        /// <summary>
        ///     游戏结束大结算，收到大结算消息十五秒后如果小结算界面仍然显示，就先关闭小结算界面
        /// </summary>
        /// <param name="param"></param>
        public void OnGameOver(ISFSObject param)
        {
            YxDebug.Log("------> GameControl.GameOver()!");
            GameTotalState = TotalState.Over;
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

        /// <summary>
        ///     提示所有玩家离开
        /// </summary>
        public void LeaveGame()
        {
            foreach (var mahjongPlayer in Players)
            {
                if (mahjongPlayer != null)
                    mahjongPlayer.CurrentInfoPanel.UserLeave();
            }
        }

        // ----------------------------------- 玩家被动退出 如果是防作弊，有人离开房间，所有玩家都需要退出房间


        public void OnClickDetail(string seat, string windowName)
        {
            var window = YxWindowManager.OpenWindow(windowName, true, null, null);
            window.UpdateView(seat);
        }

        /// <summary>
        ///     GPS信息
        /// </summary>
        public void OnClickUserHead()
        {
            YxDebug.Log("点击玩家头像");
            numberCount = 0;
            if (GpsInfosCtrl.Instance.IsShow)
            {
                GpsInfosCtrl.Instance.Hide();
                for (int i = SelfPlayer.UserSeat, max = Players.Length;
                    numberCount < max;
                    i = (i + 1)%max, numberCount++)
                {
                    var player = Players[i];
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
                var num = 0;
                var users = new List<GpsUser>();
                for (int i = CurrentSeat, max = Players.Length; numberCount < max; i = (i + 1)%max, numberCount++)
                {
                    var player = Players[i];
                    if (player != null && player.CurrentInfoPanel != null && i != CurrentSeat)
                    {
                        num++;
                        var infoPanel = Players[i].CurrentInfoPanel;
                        var userinfo = infoPanel.UserInfo;
                        if (userinfo == null)
                        {
                            continue;
                        }
                        var nextSeat = (i + 1)%max;
                        if (nextSeat.Equals(SelfPlayer.UserSeat))
                        {
                            nextSeat = (i + 2)%max;
                        }
                        var user = new GpsUser(userinfo.IsHasGpsInfo, userinfo.GpsX, userinfo.GpsY,
                            infoPanel.DistanceLabel, infoPanel.GpsLine, i, nextSeat);
                        users.Add(user);
                    }
                }
                GpsInfosCtrl.Instance.ShowDistince(users.ToArray());
                numberCount = 0;
                for (int i = SelfPlayer.UserSeat, max = Players.Length;
                    numberCount < max;
                    i = (i + 1)%max, numberCount++)
                {
                    var player = Players[i];
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
        ///     要求在小结算之后的头像状态显示为黑色状态，呵呵
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


        private void ShowPiaos()
        {
            if (_piaos == null || _piaos.Length == 0)
            {
                return;
            }
            YxDebug.Log("piaoList 的长度是：" + _piaos.Length);
            for (var i = 0; i < _piaos.Length; i++)
            {
                YxDebug.Log(_piaos[i]);
                Players[i].ShowPiao(_piaos[i]);
            }
        }

        /// <summary>
        ///     听打牌处理
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="param"></param>
        private void TingThrowOutCard(RequestData reqData, ISFSObject param)
        {
            var seat = reqData.Sit;
            ThrowOutCard(reqData);
            Players[seat].ShowTingWithEffect(true);
        }

        /// <summary>
        ///     真正意义的打出去
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="param"></param>
        public void ThrowOutCard(RequestData reqData)
        {
            _checkCard = reqData.OpCard;
            Players[reqData.Sit].OnThrowCard(_checkCard);
        }

        /// <summary>
        ///     获得可以打出的听牌
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
                        YxDebug.LogError((EnumMahjongValue) item);
                    }
                }
                else
                {
                    TingOutCards.Clear();
                }
            }
        }

        #region GPS 相关

        public void CheckGpsInfo(ISFSObject data)
        {
            var userID = data.GetInt(ConstantData.KeyUserId);
            for (int i = 0, max = Players.Length; i < max; i++)
            {
                var player = Players[i];
                if (player == null)
                {
                    continue;
                }
                if (Players[i].UserInfo != null && Players[i].UserInfo.id.Equals(userID) &&
                    Players[i].CurrentInfoPanel.gameObject.activeInHierarchy)
                {
                    Players[i].UserInfo.SetGpsData(data);
                }
            }
        }

        #endregion

        #region 后台为离线状态

        public void OnApplicationFocus(bool fouce)
        {
            if (Lyzz2DGame != null)
            {
                Lyzz2DGame.SendUserTalk(fouce);
            }
        }

        #endregion

        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            StopAllCoroutines();
            YxDebug.LogError("当前的倍率是：" + App.ShowGoldRate);
            Data.InitData(gameInfo);
            Init();
            if (!Data.IsFirstTime)
            {
                OnReJoin(gameInfo);
            }
            IsInitInfo = true;
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
        }

        public override void UserOut(int localSeat, ISFSObject responseData)
        {
            base.UserOut(localSeat, responseData);
            YxDebug.Log(string.Format("玩家{0}退出房间", localSeat));
            var list = Data.UserDatas.ToList();
            Data.UserDatas.Clear();
            foreach (var data in list)
            {
                var curSeat = data.GetInt(RequestKey.KeySeat);
                if (localSeat.Equals(localSeat))
                {
                    continue;
                }
                Data.UserDatas.Add(data);
            }
            YxDebug.Log("当前在线的玩家的数目是" + Data.UserDatas.Count);
            Players[localSeat].SetUserHead(false, true);
        }

        public override void GameStatus(int status, ISFSObject info)
        {
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            var reqData = new RequestData(response);
            YxDebug.Log(string.Format("当前的消息类型是{0}", (EnumRequest) type));
            TryGetTingOutCards(reqData.TingOutCards);
            switch ((EnumRequest) reqData.Type)
            {
                case EnumRequest.SelectPiao:
                    YxDebug.Log("显示弹出漂的提示");
                    PiaoSelectPanel.Instance.ShowGameObject();
                    break;
                case EnumRequest.ShowPiao:
                    PiaoSelectPanel.Instance.ShowGameObject(false);
                    _piaos = response.GetIntArray(RequestKey.KeyPiaoList);
                    break;
                case EnumRequest.XuanDuanMen:
                    YxDebug.Log("选段门的时间是" + Time.timeSinceLevelLoad);
                    if (response.ContainsKey(RequestKey.KeyDuanMen))
                    {
                        GameTools.TryGetValueWitheKey(response, out Data.DuanMenState, RequestKey.KeyDuanMen);
                        if (Data.DuanMenState.Equals(ConstantData.DuanMenSelect))
                        {
                            DuanMenControl.Instance.SetDuanMen(ConstantData.DuanMenMoveTime);
                        }
                    }
                    else
                    {
                        DuanMenControl.Instance.DealDuanMen();
                    }
                    break;
                case EnumRequest.ShowRate:
                    YxDebug.Log("显示倍率");
                    int rate;
                    GameTools.TryGetValueWitheKey(response, out rate, RequestKey.KeyRate);
                    GameTable.Instance.RateLabel.text = string.Format("x{0}", rate);
                    break;
                case EnumRequest.AlloCate:
                    YxDebug.Log("选段门的时间是" + Time.timeSinceLevelLoad);
                    YxDebug.Log("开始分牌");
                    ShowPiaos();
                    AlloCate(reqData);
                    GameTools.TryGetValueWitheKey(response, out FanNum, RequestKey.KeyFan);
                    GameTools.TryGetValueWitheKey(response, out LaiZiNum, RequestKey.KeyLaiZi);
                    GetHun(false, true);
                    break;
                case EnumRequest.GetCard:
                    GetInCard(reqData.Sit, reqData.OpCard, response);
                    CheckOp(reqData.Op, reqData.Sit, reqData.OpCard);
                    if (reqData.Sit.Equals(CurrentSeat))
                    {
                        SelfPlayer.ThrowCardAuto();
                    }
                    break;
                case EnumRequest.ThrowOutCard:
                    ThrowOutCard(reqData, response);
                    break;
                case EnumRequest.CPGHMenu:
                    CheckOp(reqData.Op, SelfPlayer.UserSeat, _checkCard);
                    break;
                case EnumRequest.SelfGang:
                case EnumRequest.CPHType:
                {
                    AllowResponse(reqData, response);
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
                        var player = Players[i];
                        if (player != null)
                        {
                            player.TraAddLastGetInCard();
                        }
                    }
                    Players[reqData.Sit].OnGetJueGang(reqData);
                    CheckOp(reqData.Op, reqData.Sit, reqData.OpCard);
                }
                    break;
                case EnumRequest.LiuJu:
                    YxDebug.Log("单独的流局消息");
                    OnHupai(response);
                    break;
                case EnumRequest.ZiMo:
                    YxDebug.Log("单独的自摸消息");
                    OnHupai(response);
                    break;
                case EnumRequest.Hu:
                    YxDebug.Log("单独的胡牌消息");
                    OnHupai(response);
                    break;
                case EnumRequest.QiangGangHu:
                    YxDebug.Log("抢杠胡");
                    break;
                case EnumRequest.LiangCard:
                    YxDebug.Log("亮倒");
                    break;
                case EnumRequest.CheckCards:
                    YxDebug.Log("本地手牌出现问题，强制同步");
                    SelfPlayer.SetCheckCards(response.GetIntArray(RequestKey.KeyCards));
                    break;
                case EnumRequest.HaiDi:
                    break;
                case EnumRequest.Ting:
                    TingThrowOutCard(reqData, response);
                    CheckOp(reqData.Op, SelfPlayer.UserSeat, _checkCard);
                    break;
                default:
                    YxDebug.LogError("unknow type");
                    break;
            }
        }

        #region 初始化游戏状态与玩家设置

        /// <summary>
        ///     根据实际玩家人数初始化牌桌中的座位显示
        /// </summary>
        private void InitPlayers()
        {
            var datas = Data.UserDatas;
            Players = new MahjongPlayer[PlayerNumber];
            var currentData = new UserData(datas[0]);
            ;
            SelfPlayer.JoinGame(currentData);
            SelfPlayer.Reset();
            InteraptMenu.Instance.Target = SelfPlayer.gameObject;
            if (datas.Count > 1)
            {
                for (int i = 1, max = datas.Count; i < max; i++)
                {
                    OnJoinGame(new UserData(datas[i]));
                }
            }
        }

        /// <summary>
        ///     同步Data中的状态，刷新界面显示
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
            App.GetRServer<Lyzz2DGameServer>().Ready();
        }

        public void OnGameReady(int seat)
        {
            if (App.GetGameData<Lyzz2DGlobalData>().IsFirstTime)
            {
                Players[seat].ReadyGame();
            }
            Players[seat].SetUserHead(true, false);
        }

        #endregion
    }
}