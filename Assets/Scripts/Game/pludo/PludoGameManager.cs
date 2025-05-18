using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Utils;
using YxFramwork.ConstDefine;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

/*===================================================
 *文件名称:     PludoGameManager.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-12-17
 *描述:        	飞行棋游戏管理类
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo
{
    public class PludoGameManager : YxGameManager
    {
        #region UI Param
        #endregion

        #region Data Param
        [Tooltip("配置信息，本地调整数据")]
        public GameConfig GameConifig;
        [Tooltip("遥控骰子按钮")]
        public UIButton ControlDiceBtn;
        [Tooltip("玩家准备")]
        public List<EventDelegate> CurUserReadyAction = new List<EventDelegate>();
        [Tooltip("玩家邀请")]
        public List<EventDelegate> CurUserInviteAction = new List<EventDelegate>();
        [Tooltip("游戏开始")]
        public List<EventDelegate> GameBeginAction = new List<EventDelegate>();
        [Tooltip("游戏中状态事件")]
        public List<EventDelegate> InGameingAction = new List<EventDelegate>();
        [Tooltip("当前玩家状态变化事件")]
        public List<EventDelegate> SelfPlayerStateAction = new List<EventDelegate>();
        public PludoGameState CurGameState
        {
            get
            {
                return _curGameStatus;
            }
            set
            {
                _curGameStatus = value;
                switch (_curGameStatus)
                {
                    case PludoGameState.NeedReady:
                        OnNeedReady();
                        InGameingState = false;
                        break;
                    case PludoGameState.ReadyToPlayiing:
                        CheckWaitState();
                        InGameingState = false;
                        break;
                    case PludoGameState.Playing:
                        OnPlaying();
                        InGameingState = true;
                        break;
                    default:
                        InGameingState = false;
                        break;
                }
                ChcekInGameingState();
            }
        }
        /// <summary>
        /// 当前玩家准备状态
        /// </summary>
        public bool CurReadyState { private set; get; }

        /// <summary>
        /// 当前玩家邀请状态
        /// </summary>
        public bool CurInviteState
        {
            get
            {
                if (GameData)
                {
                    if(GameData.IsCreateRoom)
                    return GameData.UserInfoDict.Count < GameData.SeatTotalCount;
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public bool InGameingState {private set; get;}

        #endregion

        #region Local Data
        /// <summary>
        /// 游戏状态
        /// </summary>
        private PludoGameState _curGameStatus = PludoGameState.Init;
 

        private PludoGameData GameData
        {
            get
            {
                return App.GetGameData<PludoGameData>();
            }
        }



        #endregion

        #region Life Cycle

        protected override void OnAwake()
        {
            base.OnAwake();
            Facade.EventCenter.AddEventListeners<LoaclRequest, ISFSObject>(LoaclRequest.HandUpResultInfo,OnHandUp);
            Facade.EventCenter.AddEventListeners<LoaclRequest, int>(LoaclRequest.ChangRoom,OnChangeRoom);
            Facade.EventCenter.AddEventListeners<LoaclRequest, int>(LoaclRequest.GameOverShow, ShowGameOver);
            Facade.EventCenter.AddEventListeners<LoaclRequest, ISFSObject>(LoaclRequest.GameOverGetData,OnGetOverData);
            Facade.EventCenter.AddEventListeners<LoaclRequest, SharePlat>(LoaclRequest.ShareImage, ShootScreen);
            Facade.EventCenter.AddEventListeners<LoaclRequest, int>(LoaclRequest.HandUpLocalStart,StarHandUp);
            Facade.EventCenter.AddEventListeners<LoaclRequest, int>(LoaclRequest.FreshSelfPlayerAction, FreshSelfPlayerAction);
            Facade.EventCenter.AddEventListeners<LoaclRequest, int>(LoaclRequest.NeedToReady,OnNeedToReady);
            waitForScreen = new WaitForEndOfFrame();
        }

        public override void OnDestroy()
        {
            Facade.EventCenter.RemoveEventListener<LoaclRequest, ISFSObject>(LoaclRequest.HandUpResultInfo, OnHandUp);
            Facade.EventCenter.RemoveEventListener<LoaclRequest, int>(LoaclRequest.ChangRoom, OnChangeRoom);
            Facade.EventCenter.RemoveEventListener<LoaclRequest, int>(LoaclRequest.GameOverShow, ShowGameOver);
            Facade.EventCenter.RemoveEventListener<LoaclRequest, ISFSObject>(LoaclRequest.GameOverGetData, OnGetOverData);
            Facade.EventCenter.RemoveEventListener<LoaclRequest, SharePlat>(LoaclRequest.ShareImage, ShootScreen);
            Facade.EventCenter.RemoveEventListener<LoaclRequest, int>(LoaclRequest.HandUpLocalStart, StarHandUp);
            Facade.EventCenter.RemoveEventListener<LoaclRequest, int>(LoaclRequest.FreshSelfPlayerAction, FreshSelfPlayerAction);
            Facade.EventCenter.RemoveEventListener<LoaclRequest, int>(LoaclRequest.NeedToReady, OnNeedToReady);
            base.OnDestroy();
        }

        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            Facade.EventCenter.DispatchEvent(LoaclRequest.MapInit, GameData.MapInfo);
            Facade.EventCenter.DispatchEvent(LoaclRequest.RoomInfo,GameData.RoomInfo);
            DealPludoGameStatus(gameInfo);
            GameData.GetHandUpDataRejoin(gameInfo);
            Facade.EventCenter.DispatchEvent(LoaclRequest.RuleInfoInit, GameData.RoomInfo.Rule);
            Facade.EventCenter.DispatchEvent(LoaclRequest.FreshSelfPlayerAction,ConstantData.IntDefValue); 
            Facade.EventCenter.DispatchEvent(LoaclRequest.FreshCostNumAction, GameData.ControlPointConsume);
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {

        }

        public override void GameStatus(int status, ISFSObject info)
        {
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            var newMessage=new MessageData()
            {
                MessageType = type,
                Data = response,
                MessageState = MessageState.Init,
                ExcuteAction = ExcuteMessage
            };
            //MessageCenter.Equeue(newMessage);
            ExcuteMessage(newMessage);
        }

        public void ExcuteMessage(MessageData data)
        {
            var type = (EnumGameServer)data.MessageType;
            var response = data.Data;
            switch (type)
            {
                case EnumGameServer.Auto:
                    OnAuto(response);
                    break;
                case EnumGameServer.GameStart:
                    OnGameStart(response);
                    break;
                case EnumGameServer.CurrentPlayer:
                    OnChangeCurrentUser(response);
                    break;
                case EnumGameServer.RollDicRequest:
                    OnShowRollResult(response);
                    break;
                case EnumGameServer.ChoosePlaneResult:
                    OnChoosePlaneResult(response);
                    break;
                case EnumGameServer.Result:
                    OnGameResult(response);
                    break;
                case EnumGameServer.ControlPoint:
                    ControlDicResult(response);
                    break;
                default:
                    Debug.LogError("未识别游戏交互，类型是："+ type);
                    break;
            }

        }



        public override void UserReady(int localSeat, ISFSObject responseData)
        {
            base.UserReady(localSeat, responseData);
            if (localSeat==ConstantData.CurUiSeat)
            {
                CurGameState=PludoGameState.ReadyToPlayiing;
            }
        }

        public override void OnOtherPlayerJoinRoom(ISFSObject sfsObject)
        {
            base.OnOtherPlayerJoinRoom(sfsObject);
            CheckWaitState();
        }

        public override void UserOut(int localSeat, ISFSObject responseData)
        {
            base.UserOut(localSeat, responseData);
            CheckWaitState();
        }

        #endregion

        #region Function

        #region 游戏流程
        /// <summary>
        /// 处理游戏流程（确定游戏状态）
        /// </summary>
        /// <param name="gameInfo"></param>
        private void DealPludoGameStatus(ISFSObject gameInfo)
        {
            var count = GameData.UserInfoDict.Count;
            if (count < GameData.SeatTotalCount)
            {
                CurGameState = GameData.MainUserInfo.State? PludoGameState.ReadyToPlayiing:PludoGameState.NeedReady;
            }
            else
            {
                if (GameData.IsCreateRoom&& GameData.CreateRoomInfo.CurRound == ConstantData.IntValue)
                {
                    CurGameState = PludoGameState.NeedReady;
                }
                else
                {
                    var curPlayerReadyState = GameData.MainUserInfo.State;
                    if (curPlayerReadyState)
                    {
                        bool readyState = true;
                        foreach (var item in GameData.UserInfoDict)
                        {
                            if (item.Value.State == false)
                            {
                                readyState = false;
                                break;
                            }
                        }
                        if (readyState)
                        {
                            CurGameState = PludoGameState.Playing;
                            GameData.GetOperation(gameInfo);
                        }
                        else
                        {
                            CurGameState = PludoGameState.ReadyToPlayiing;
                        }
                    }
                    else
                    {
                        CurGameState = PludoGameState.NeedReady;
                    }
                }
            }
        }
          

        /// <summary>
        /// 玩家未准备状态
        /// </summary>
        private void OnNeedReady()
        {
            GameData.IsGameStart = false;
            CheckWaitState();
        }

        /// <summary>
        /// 游戏阶段
        /// </summary>
        private void OnPlaying()
        {
            GameData.IsGameStart = true;
            CheckWaitState();
        }

        #endregion
        #region UI 操作

        /// <summary>
        /// 点击邀请好友按钮
        /// </summary>
        public void OnClickInviteBtn()
        {
            YxTools.ShareFriend
                (
                    GameData.RoomInfo.CreateRoomInfo.RoomId.ToString(),
                    GameData.RoomInfo.Rule
                );
        }

        /// <summary>
        /// 点击准备按钮
        /// </summary>
        public void OnClickReadyBtn()
        {
            Facade.EventCenter.DispatchEvent(LoaclRequest.UserReadyRequest, ConstantData.IntDefValue);
        }

        /// <summary>
        /// 飞机起飞
        /// </summary>
        public void OnPlaneStartFlyClick()
        {
            GameData.MainUser.OnPlaneStartFly();
        }
        /// <summary>
        /// 点击打骰子
        /// </summary>
        public void OnClickRollDice()
        {
            GameData.MainUser.OnClickRollDice();
        }
        /// <summary>
        /// 遥控骰子
        /// </summary>
        /// <param name="point"></param>
        public void OnClickConrtolDicePoint(string point)
        {
            GameData.MainUser.OnClickConrtolDicePoint(point);
        }
        /// <summary>
        /// 点击托管
        /// </summary>
        public void OnClickAuto()
        {
            ISFSObject data = new SFSObject();
            data.PutInt(RequestKey.KeyType, (int)EnumGameServer.Auto);
            data.PutBool(ConstantData.KeyAuto,!GameData.AutoState);
            Facade.EventCenter.DispatchEvent(LoaclRequest.AutoRequest, data);
        }

        /// <summary>
        /// 点击设置按钮
        /// </summary>
        public void OnClickSettingBtn()
        {
            var window = YxWindowManager.OpenWindow(GameConifig.SettingWindoName);
            if (window)
            {
                window.UpdateView(GameData.SettingInfo);
            }
        }

        public void OnPlaneClick(int planeId)
        {
            GameData.MainUser.OnPlaneClick(planeId);
        }

        #endregion
        #region 回调处理
        /// <summary>
        /// 游戏开始消息处理
        /// 更改游戏状态为游戏中状态，显示开始提示，并初始化飞机信息
        /// </summary>
        /// <param name="data"></param>
        private void OnGameStart(ISFSObject data)
        {
            CurGameState = PludoGameState.Playing;
            GameData.OnGameStart(data);
            Facade.EventCenter.DispatchEvent(LoaclRequest.PlaneInit, ConstantData.IntDefValue);
            Facade.EventCenter.DispatchEvent(LoaclRequest.RoomInfo, GameData.RoomInfo);
            ConstantData.PlaySoundBySex(GameData.MainUser.Info.SexI,ConstantData.KeyGameBegin);
            StartCoroutine(GameBeginAction.WaitExcuteCalls());
        }

        /// <summary>
        /// 需要准备
        /// </summary>
        /// <param name="data"></param>
        private void OnNeedToReady(int data)
        {
            CurGameState=PludoGameState.NeedReady;
        }

        /// <summary>
        /// 切换当前玩家
        /// 上一个当前玩家状态重置（Cd状态与操作）
        /// </summary>
        /// <param name="data"></param>
        private void OnChangeCurrentUser(ISFSObject data)
        {
            if (GameData.CurOpUser)
            {
                GameData.CurOpUser.OnPlayerSleep();
            }
            GameData.SetCurPlayer(data);
            GameData.CurOpUser.OnWaitRollDice();
        }

        /// <summary>
        /// 显示打骰子结果
        /// </summary>
        /// <param name="data"></param>
        private void OnShowRollResult(ISFSObject data)
        {
            GameData.OnShowRollResult(data);
            GameData.CurOpUser.OnShowRollResult();
        }
        #endregion
        
        /// <summary>
        /// 选择飞机回调
        /// </summary>
        /// <param name="data"></param>
        private void OnChoosePlaneResult(ISFSObject data)
        {
            GameData.OnChoosePlane(data);
            GameData.CurOpUser.OnPlayerSleep();
            Facade.EventCenter.DispatchEvent(LoaclRequest.ChoosePlaneResult,GameData.ChooseResult);
        }

        /// <summary>
        /// 遥控骰子失败交互
        /// </summary>
        /// <param name="data"></param>
        private void ControlDicResult(ISFSObject data)
        {
            if (data.ContainsKey(RequestKey.KeyMessage))
            {
                string errmeg = "";
                SfsHelper.Parse(data, RequestKey.KeyMessage, ref errmeg);
                YxMessageTip.Show(errmeg);
                GameData.MainUser.OnControlDiceFail();
            }
        }

        /// <summary>
        /// 游戏结束回调（小结算）
        /// </summary>
        /// <param name="data"></param>
        private void OnGameResult(ISFSObject data)
        {
            CurGameState = PludoGameState.NeedReady;
            GameData.OnGameResult(data);
            var window=YxWindowManager.OpenWindow(GameConifig.GameResultWindoName);
            if (window)
            {
                window.UpdateView(GameData.ResultData);
            }
        }

        /// <summary>
        /// 收到大结算数据
        /// </summary>
        /// <param name="data"></param>
        private void OnGetOverData(ISFSObject data)
        {
            GameData.OnGetGameOverData(data);
            CurGameState = PludoGameState.Over;
            ShowGameOver(ConstantData.IntDefValue);
        }

        /// <summary>
        /// 显示大结算界面
        /// </summary>
        /// <param name="data"></param>
        private void ShowGameOver(int data)
        {
            var gameOverWindow = YxWindowManager.OpenWindow(GameConifig.GameOverWindowName);
            if (gameOverWindow)
            {
                gameOverWindow.UpdateView(GameData.OverData);
            }
        }

        /// <summary>
        /// 托管
        /// </summary>
        /// <param name="data"></param>
        private void OnAuto(ISFSObject data)
        {
            GameData.OnAutoStateChange(data);
        }
        /// <summary>
        /// 投票消息
        /// </summary>
        /// <param name="data"></param>
        private void OnHandUp(ISFSObject data)
        {
            GameData.OnGetHandUpDataInGame(data);
        }
        /// <summary>
        /// 更换房间
        /// </summary>
        /// <param name="data"></param>
        private void OnChangeRoom(int data)
        {
            OnChangeRoomClick();
        }
        private YieldInstruction waitForScreen;
        private void ShootScreen(SharePlat sharePlat)
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(YxTools.ShareSceenImage(sharePlat,waitForScreen));
            }
        }

        /// <summary>
        /// 发起投票
        /// </summary>
        /// <param name="data"></param>
        private void StarHandUp(int data)
        {
            var handWindow = YxWindowManager.OpenWindow(GameConifig.HandWindoName);
            if (handWindow)
            {
                handWindow.UpdateView(GameData.HandData);
            }
        }

        public void SetControlDiceEnable(bool state)
        {
          
        }

        private void FreshSelfPlayerAction(int data)
        {
            if (ControlDiceBtn)
            {
                ControlDiceBtn.isEnabled = GameData.MainUser.CouldUseControlDice;
            }
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(SelfPlayerStateAction.WaitExcuteCalls());
            }
        }

        #region 公用方法

        /// <summary>
        /// 准备状态监测（监测是否满足现实邀请按钮状态与显示准备状态）
        /// </summary>
        private void CheckWaitState()
        {
            if (GameData&& GameData.MainUserInfo!=null)
            {
                CheckPlayersInviteStatus();
                OnReadyStateChange(GameData.MainUserInfo.State);
                foreach (var player in GameData.PlayerList)
                {
                    if (player)
                    {
                        player.ReadyState = player.ReadyState;
                    }
                }
            }
        }
        /// <summary>
        /// 准备状态检测
        /// </summary>
        /// <param name="state"></param>
        private void OnReadyStateChange(bool state)
        {
            CurReadyState = state;
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(CurUserReadyAction.WaitExcuteCalls());
            }
        }

        /// <summary>
        /// 邀请状态检测
        /// </summary>
        private void CheckPlayersInviteStatus()
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(CurUserInviteAction.WaitExcuteCalls());
            }
        }

        /// <summary>
        /// 检测是否处于游戏中状态
        /// </summary>
        private void ChcekInGameingState()
        {
            App.GameData.GStatus =InGameingState?YxEGameStatus.PlayAndConfine:YxEGameStatus.Normal;
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(InGameingAction.WaitExcuteCalls());
            }
        }

        /// <summary>
        /// 强制重连
        /// </summary>
        private void SendReJoinGame()
        {
            App.RServer.SendReJoinGame();
        }

        #endregion

        #region Func Tools
        /// <summary>
        /// 获取剩余时间
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="nowTime">当前时间</param>
        /// <param name="cdTime">Cd时间</param>
        /// <returns></returns>
        public static long GetLeftTime(long startTime, long nowTime, long cdTime)
        {
            return cdTime - nowTime - startTime;
        }


        public static void ShowRollDiceAnimation(GameObject obj,int finishPoint, Vector3 from, EventDelegate moveFinish)
        {
            TweenPosition.Begin(obj, 5, from);
        }
        #endregion

        #endregion
    }
    /// <summary>
    /// 本地交互事件
    /// </summary>
    public enum LoaclRequest
    {
        RoomInfo,                          //房间信息
        MapInit,                           //地图初始化
        RuleInfoInit,                      //规则信息初始化
        PlaneInit,                         //飞机信息初始化（牌局开始）
        FreshStartNum,                     //刷新人物星星数
        FreshSelfPlayerAction,             //刷新当前玩家操作按钮
        FreshCostNumAction,                //刷新钻石消耗显示事件
        UserReadyRequest,                  //玩家准备请求
        ChangRoom,                         //更换房间
        HandUpByOwnerRequset,              //房主投票请求
        HandUpRequest,                     //常规投票请求
        HandUpResultInfo,                  //投票结果信息
        HandUpLocalStart,                  //投票本地发起
        HandUpLocalMessage,                //投票其它玩家信息
        SettingInfoChange,                 //设置信息变化（解散按钮显示）
        RollDiceRequest,                   //打骰子请求
        RollDiceCallBack,                  //打骰子消息处理
        ChoosePlaneRequest,                //选择飞机请求
        ChoosePlaneResult,                 //选择飞机结果处理
        AutoRequest,                       //托管请求
        NeedToReady,                       //需要准备（小结算点击继续后触发）
        GameOverGetData,                   //游戏大结算获得数据
        GameOverShow,                      //游戏大结算显示
        ShareImage,                        //分享图片
    }

    /// <summary>
    /// 飞行棋游戏流程
    /// </summary>
    public enum PludoGameState
    {
        Init,                               //初始化状态（无状态）
        NeedReady,                          //需要玩家准备（当前玩家未准备）
        ReadyToPlayiing,                    //准备游戏阶段（其它玩家未准备）
        Playing,                            //游戏中
        Over,                               //游戏结束
    }
}
