using System;
using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.DDzGameListener.InfoPanel;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Common.Utils;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.ddz2.InheritCommon
{
    public class DdzGameData : YxGameData
    {

        /// <summary>
        /// 卸载场景时清理容易导致程序崩溃的粒子特效资源
        /// </summary>
        protected override void OnGc()
        {
            base.OnGc();
            ClearParticalGob();
        }


        protected override void OnAwake()
        {
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeAllocate, OnTypeAllocate);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeOneRoundOver,
                OnTypeOneRoundOver);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeFlow, OnTypeFlow);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyGetGameInfo, OnGetGameInfo);
        }

        private bool _robModel;

        public bool RobModel
        {
            get { return _robModel; }
        }

        /// <summary>
        /// 进入游戏,接受到数据
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnGetGameInfo(DdzbaseEventArgs args)
        {
            var sfsObj = args.IsfObjData;
            if (sfsObj == null) return;
            if (!sfsObj.ContainsKey("cargs2")) return;
            var cargs2 = sfsObj.GetSFSObject("cargs2");
            if (cargs2 == null) return;
            if (!cargs2.ContainsKey("-qt")) return;
            var qt = cargs2.GetUtfString("-qt");
            _robModel = qt.Equals("1") || qt.Equals("2");
        }

        /// <summary>
        /// 没人叫分,流局、黄庄
        /// </summary>
        /// <param name="obj"></param>
        private void OnTypeFlow(DdzbaseEventArgs obj)
        {
            CurrentRound--;
        }


        private void OnTypeOneRoundOver(DdzbaseEventArgs obj)
        {
            IsGameStart = false;
        }


        /// <summary>
        /// 当有手牌数更新时,第一个int 是 userSeat 第二个 int 是 leftHandCdNum 
        /// </summary>
        private Action<int> _onHdCdsChange;

        /// <summary>
        /// 手牌数更新事件
        /// </summary>
        public Action<int> OnhandCdsNumChanged
        {
            set { _onHdCdsChange += value; }
        }

        /// <summary>
        /// 调用手牌数量改变全局事件
        /// </summary>
        /// <param name="userSeat"></param>
        public void OnHdcdsChange(int userSeat)
        {
            if (_onHdCdsChange == null) return;

            _onHdCdsChange(userSeat);
        }

        /// <summary>
        /// 当玩家分数改变时 第一个int 是 userSeat 第二个 int 是 scoreGold 
        /// </summary>
        private Action<int, int> _onUserScoreChanged;

        public Action<int, int> OnUserScoreChanged
        {
            set { _onUserScoreChanged += value; }
        }

        /// <summary>
        /// 调用玩家数据改变全局事件
        /// </summary>
        /// <param name="userSeat"></param>
        /// <param name="scoreGold"></param>
        public void OnUserSocreChanged(int userSeat, int scoreGold)
        {
            if (_onUserScoreChanged == null) return;

            _onUserScoreChanged(userSeat, scoreGold);
        }

        //清理例子特效
        private Action _onClearParticalGob;

        public Action OnClearParticalGob
        {
            set { _onClearParticalGob += value; }
        }

        /// <summary>
        /// 清理例子特效，防止返回大厅崩溃
        /// </summary>
        public void ClearParticalGob()
        {
            _onClearParticalGob();
        }



        /// <summary>
        /// 是否播放语音
        /// </summary>
        public bool IsPlayVoiceChat;

        /// <summary>
        /// 获得所有参与游戏玩家的名字
        /// </summary>
        /// <returns></returns>
        public string[] GetUsersName()
        {
            var len = PlayerList.Length;
            var userNames = new string[len];
            for (int i = 0; i < len; i++)
            {
                userNames[i] = PlayerList[i].Nick;
            }

            return userNames;
        }


        /// <summary>
        /// 是否是开房游戏,默认是开房
        /// </summary>
        public bool IsRoomGame { get; set; }

        /// <summary>
        /// 房间号
        /// </summary>
        public int RoomId { get; set; }

        /// <summary>
        /// 本房间是否已经开始游戏
        /// </summary>
        public bool RoomGameStarted
        {
            get { return CurrentRound > 0; }
        }

        /// <summary>
        /// 玩家最大人数
        /// </summary>
        public int PlayerMaxNum
        {
            get { return _playerMaxNum; } //默认3个
        }

        private int _playerMaxNum = 3;


        /// <summary>
        /// 获取玩家下家座位号
        /// </summary>
        /// <param name="seaverSeat"></param>
        /// <returns></returns>
        public int GetLaterHand(int seaverSeat)
        {
            return (seaverSeat + 1) % PlayerMaxNum;
        }

        /// <summary>
        /// 获取自己下家座位号
        /// </summary>
        public int GetLaterHand()
        {
            return (SelfSeat + 1) % PlayerMaxNum;
        }


        /// <summary>
        /// 获取玩家上家座位号
        /// </summary>
        /// <param name="searverSeat"></param>
        /// <returns></returns>
        public int GetEarlyHand(int searverSeat)
        {
            return (searverSeat + PlayerMaxNum - 1) % PlayerMaxNum;
        }

        /// <summary>
        /// 获取自己上家座位号
        /// </summary>
        public int GetEarlyHand()
        {
            return (SelfSeat + PlayerMaxNum - 1) % PlayerMaxNum;
        }

        /// <summary>
        /// 加倍类型 0不加倍 1正常加倍 2农民加倍
        /// </summary>
        public int JiaBeiType { get; set; }


        /// <summary>
        /// 开始发牌,标示着游戏开始了
        /// </summary>
        /// <param name="args"></param>
        protected void OnTypeAllocate(DdzbaseEventArgs args)
        {
            IsGameStart = true;
        }

        public bool AllReady()
        {
            //所有玩家都准备
            foreach (var player in PlayerList)
            {
                if (player.Info == null)
                {
                    return false;
                }
                if (player.ReadyState == false)
                {
                    return false;
                }
            }
            return true;
        }

        public bool FinishRoomInfo;

        /// <summary>
        /// 房主Id
        /// </summary>
        private int _owerId;

        public bool IsRoomOwner
        {
            get
            {
                var selfInfo = GetPlayerInfo();
                return selfInfo.Id == _owerId;
            }
        }

        [HideInInspector]
        public int CurrentRound;     

        protected override YxBaseGameUserInfo OnInitUser(ISFSObject userData)
        {
            var userInfo = new DdzUserInfo();
            userInfo.Parse(userData);

            return userInfo;
        }


        protected override void InitGameData(ISFSObject gameInfo)
        {
            FinishRoomInfo = false;
            base.InitGameData(gameInfo);

            IsRoomGame = gameInfo.ContainsKey(NewRequestKey.KeyGtype) &&
                         gameInfo.GetInt(NewRequestKey.KeyGtype) == -1;

            JiaBeiType = gameInfo.GetInt(NewRequestKey.KeyJiaBei);

            if (gameInfo.ContainsKey(NewRequestKey.KeyPlayerNum))
            {
                _playerMaxNum = gameInfo.GetInt(NewRequestKey.KeyPlayerNum);
            }

            if (gameInfo.ContainsKey(NewRequestKey.KeyOwerId))
            {
                _owerId = gameInfo.GetInt(NewRequestKey.KeyOwerId);
            }


            if (gameInfo.ContainsKey(NewRequestKey.KeyCurRound))
            {
                CurrentRound = gameInfo.GetInt(NewRequestKey.KeyCurRound);
            }

            if (gameInfo.ContainsKey(NewRequestKey.KeyRoomId))
            {
                RoomId = gameInfo.GetInt(NewRequestKey.KeyRoomId);
            }

            IsGameStart = gameInfo.ContainsKey(NewRequestKey.KeyGameStatus)
                          &&
                          (gameInfo.GetInt(NewRequestKey.KeyGameStatus) > GlobalConstKey.StatusIdle &&
                           gameInfo.GetInt(NewRequestKey.KeyGameStatus) <= GlobalConstKey.StatusPlay);
        }

        public YxBaseGameUserInfo GetOnePlayerInfo(int seat = -1, bool isServerseat = false)
        {
            return GetPlayerInfo(seat, isServerseat) ?? GetLastGamePlayerInfo(seat, isServerseat);
        }
    }

    /// <summary>
    /// 一些服务器返回的值的判断
    /// </summary>
    public class GlobalConstKey
    {
        //游戏的status
        /// <summary>
        /// 人员还没有都准备时
        /// </summary>
        public const int StatusIdle = 0;
        /// <summary>
        /// 选择庄家阶段
        /// </summary>
        public const int StatusChoseBanker = 1;

        /// <summary>
        /// 加倍阶段
        /// </summary>
        public const int StatusDouble = 2;

        /// <summary>
        /// 出牌阶段
        /// </summary>
        public const int StatusPlay = 3;


        ///// <summary>
        ///// 游戏类型
        ///// </summary>
        //public enum GameType
        //{
        //    /// <summary>
        //    /// 叫分
        //    /// </summary>
        //    CallScore = 0,
        //    /// <summary>
        //    /// 踢地主
        //    /// </summary>
        //    Kick,
        //    /// <summary>
        //    /// 抢地主
        //    /// </summary>
        //    Grab,
        //    /// <summary>
        //    /// 叫分带流局
        //    /// </summary>
        //    CallScoreWithFlow,
        //}

        /*        /// <summary>
                /// 叫分带流局
                /// </summary>
                public const int CallScoreWithFlow = 3;*/



        //OnServerResponse的各种类型-------------------start
        /// <summary>
        /// 1.发牌
        /// </summary>
        public const int TypeAllocate = 1;

        /// <summary>
        /// 2.抢地主
        /// </summary>
        public const int TypeGrab = 2;

        /// <summary>
        /// 3.出牌
        /// </summary>
        public const int TypeOutCard = 3;

        /// <summary>
        /// 4.不出
        /// </summary>
        public const int TypePass = 4;

        /// <summary>
        /// 5.抢到地主,先出牌
        /// </summary>
        public const int TypeFirstOut = 5;

        /// <summary>
        /// 6.一局游戏结束 显示结算
        /// </summary>
        public const int TypeOneRoundOver = 6;

        /// <summary>
        /// 7.指定抢地主或者叫分
        /// </summary>
        public const int TypeGrabSpeaker = 7;

        /// <summary>
        /// 8.流局黄庄
        /// </summary>
        public const int TypeFlow = 8;

        /// <summary>
        /// 9.加倍
        /// </summary>
        public const int TypeDouble = 9;

        /// <summary>
        /// 10.加倍结束
        /// </summary>
        public const int TypeDoubleOver = 10;


        /// <summary>
        /// 11.托管
        /// </summary>
        public const int TypeAuto = 11;


        /// <summary>
        /// 错误重连
        /// </summary>
        public const int ErrorRejion = 12;



        /// <summary>
        /// 向服务器发送的请求key名
        /// </summary>
        public const string C_Type = "type";
        public const string C_Cards = "cards";
        public const string C_Sit = "seat";
        public const string C_Score = "score";
        public const string C_Magic = "laizi";


        public const string KeyGetGameInfo = "getGameInfo";
        public const string KeyOnRejoin = "onGameRejoin";
        public const string KeyOnUserOut = "userOut";
        public const string KeyShowReadyBtn = "keyShowReadyBtn";
        public const string KeyOnBeginNewGame = "keyOnBeginNewGame";
        public const string KeyOnUserReady = "onUserReady";
        public const string KeyOnHandUp = "onHandUp";
        public const string KeyRoomGameOver = "roomGameOver";
        public const string KeyHdCds = "hdcds";
        public const string KeyRemind = "timeRemind";
        public const string KeySelfAuto = "selfAuto";

        public const string Cmd = "cmd";
        public const string Hup = "hup";
        public const string PlaySound = "keyPlaySound";
        public const string PlaySoundAndPauseBgSound = "keyPlayAPauseBgSound";
        public const string AllowReady = "allowReady";

        public const string CheckLuckyCards = "checkLuckyCards";
        public const string CheckLuckyResult = "checkLuckyResult";
    }
}
