using System;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;

/*===================================================
 *文件名称:     PludoGameServer.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-12-17
 *描述:        	飞行棋游戏交互服务
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo
{
    public class PludoGameServer : YxGameServer
    {
        #region UI Param
        #endregion

        #region Data Param
        protected string GameKeyWithPoint
        {
            get { return App.GameKey + "."; }
        }
        #endregion

        #region Local Data
        #endregion

        #region Life Cycle

        protected override void OnAwake()
        {
            base.OnAwake();
            Facade.EventCenter.AddEventListeners<LoaclRequest,int>(LoaclRequest.UserReadyRequest,UserReadyRequest);
            Facade.EventCenter.AddEventListeners<LoaclRequest, int>(LoaclRequest.HandUpByOwnerRequset,HandsUpByOwner);
            Facade.EventCenter.AddEventListeners<LoaclRequest,ISFSObject>(LoaclRequest.HandUpRequest, HandsUp);
            Facade.EventCenter.AddEventListeners<LoaclRequest, ISFSObject>(LoaclRequest.AutoRequest, AutoRequest);
            Facade.EventCenter.AddEventListeners<LoaclRequest, ISFSObject>(LoaclRequest.RollDiceRequest, RollDiceRequest);
            Facade.EventCenter.AddEventListeners<LoaclRequest, ISFSObject>(LoaclRequest.ChoosePlaneRequest, ChoosePlaneRequest);
        }

        public override void Gc()
        {
            Facade.EventCenter.RemoveEventListener<LoaclRequest, int>(LoaclRequest.UserReadyRequest, UserReadyRequest);
            Facade.EventCenter.RemoveEventListener<LoaclRequest, int>(LoaclRequest.HandUpByOwnerRequset, HandsUpByOwner);
            Facade.EventCenter.RemoveEventListener<LoaclRequest, ISFSObject>(LoaclRequest.HandUpRequest, HandsUp);
            Facade.EventCenter.RemoveEventListener<LoaclRequest, ISFSObject>(LoaclRequest.AutoRequest, AutoRequest);
            Facade.EventCenter.RemoveEventListener<LoaclRequest, ISFSObject>(LoaclRequest.RollDiceRequest, RollDiceRequest);
            Facade.EventCenter.RemoveEventListener<LoaclRequest, ISFSObject>(LoaclRequest.ChoosePlaneRequest, ChoosePlaneRequest);
            base.Gc();
        }

        public override void Init(Dictionary<string, Action<ISFSObject>> callBackDic)
        {
            base.Init(callBackDic);
            callBackDic[ConstantData.KeyHandsUp] = OnHandUp;
            callBackDic[GameKeyWithPoint + ConstantData.KeyGameOver] = OnGameVer;
        }

        #endregion

        #region Function

        /// <summary>
        /// 玩家准备请求
        /// </summary>
        private void UserReadyRequest(int ready)
        {
            var key = GameKeyWithPoint + RequestCmd.Ready;
            SendFrameRequest(key, new SFSObject());
        }

        /// <summary>
        /// 房主解散
        /// </summary>
        /// <param name="num">无实际意义</param>
        public void HandsUpByOwner(int num)
        {
            SendFrameRequest(ConstantData.KeyCommondHandsUpByOwner,new SFSObject());
        }
        
        /// <summary>
        /// 解散
        /// </summary>
        /// <param name="data"></param>
        public void HandsUp(ISFSObject data)
        {
            SendFrameRequest(ConstantData.KeyHandsUp, data);
        }
        /// <summary>
        /// 解散消息
        /// </summary>
        /// <param name="data"></param>
        public void OnHandUp(ISFSObject data)
        {
            Facade.EventCenter.DispatchEvent(LoaclRequest.HandUpResultInfo,data);
        }

        /// <summary>
        /// 大结算消息
        /// </summary>
        /// <param name="data"></param>
        public void OnGameVer(ISFSObject data)
        {
            Facade.EventCenter.DispatchEvent(LoaclRequest.GameOverGetData, data);
        }

        /// <summary>
        /// 托管
        /// </summary>
        /// <param name="data"></param>
        public void AutoRequest(ISFSObject data)
        {
            SendGameRequest(data);
        }

        /// <summary>
        /// 选择飞机
        /// </summary>
        /// <param name="data"></param>
        public void ChoosePlaneRequest(ISFSObject data)
        {
            SendGameRequest(data);
        }

        /// <summary>
        /// 打骰子请求
        /// </summary>
        /// <param name="data"></param>
        public void RollDiceRequest(ISFSObject data)
        {
            SendGameRequest(data);
        }

        #endregion
    }

    /// <summary>
    /// 游戏交互ID(Main Id)
    /// </summary>
    public enum EnumGameServer
    {
        RollDicRequest=2,                                 //打骰子
        ChoosePlabe=4,                                    //选择飞机                                      //游戏结束（大结算）
        ChoosePlaneResult=16,                             //选择飞机结果
        CurrentPlayer=32,                                 //当前的用户（选择飞机后的执行结果）
        Result=64,                                        //小结算
        Auto=128,                                         //托管
        GameStart=256,                                    //游戏开始
        ControlPoint =512,                                //遥控骰子
    }

    /// <summary>
    /// 投票状态
    /// </summary>
    public enum HandUpStatus
    {
        DisAgree=-1,                //不同意
        Start=2,                    //发起投票
        Agree=3,                    //同意
        Wait,                       //等待
    }

}
