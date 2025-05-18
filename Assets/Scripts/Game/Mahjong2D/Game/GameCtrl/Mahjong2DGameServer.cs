using System;
using System.Collections.Generic;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Request;
using Assets.Scripts.Game.Mahjong2D.Game.Data;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.View;

namespace Assets.Scripts.Game.Mahjong2D.Game.GameCtrl
{
    public class Mahjong2DGameServer : YxGameServer
    {
        public Mahjong2DGameData Data
        {
            get { return App.GetGameData<Mahjong2DGameData>(); }
        }

        public Mahjong2DGameManager Manager
        {
            get { return App.GetGameManager<Mahjong2DGameManager>(); }
        }
        [HideInInspector]
        /// <summary>
        /// 是否正在投票解散
        /// </summary>
        public bool IsInHandsUp;

        [HideInInspector]
        public string GameKeyWithPoint
        {
            get { return App.GameKey + "."; }
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            Facade.EventCenter.AddEventListeners<string,KeyValuePair<int,bool>>(ConstantData.KeyRobotToggleRequest, OnAutoRequest);
        }

        public override void Gc()
        {
            base.Gc();
            Facade.EventCenter.RemoveEventListener<string, KeyValuePair<int, bool>>(ConstantData.KeyRobotToggleRequest,OnAutoRequest);
        }

        public override void Init(Dictionary<string, Action<ISFSObject>> callBackDic)
        {
            base.Init(callBackDic);
            callBackDic[GameKeyWithPoint + RequestCmd.Ready] = OnGameReady;
            callBackDic[GameKeyWithPoint + RequestCmd.RollDice] = OnRollDice;
            callBackDic[GameKeyWithPoint + RequestCmd.GameOver] = OnGameOver;
            callBackDic[RequestCmd.HandsUp] = OnHandsUp;
            callBackDic[RequestCmd.UserTalk] = OnUserTalk;
            callBackDic[RequestCmd.Sound] = OnReceiveVoiceChat;
            callBackDic[RequestCmd.LocatPosition] = OnReciveGPSInfo;
            callBackDic[RequestCmd.OnUserIdle] =CustomUserState;

        }

        private void OnRollDice(ISFSObject currResponse)
        {
            App.GetGameManager<Mahjong2DGameManager>().RollDice(currResponse);
        }


        #region 游戏准备
        public void Ready()
        {
            YxDebug.Log("发送准备请求");
            if(App.GetGameManager<Mahjong2DGameManager>().IsInit||App.GetGameManager<Mahjong2DGameManager>().IsWaiting|| App.GetGameManager<Mahjong2DGameManager>().IsAccount)
            {
                var key =GameKeyWithPoint + RequestCmd.Ready;
                SendFrameRequest(key, new SFSObject());
            }
        }
        private void OnGameReady(ISFSObject currResponse)
        {
            var seat = currResponse.GetInt(RequestKey.KeySeat);
            App.GetGameManager<Mahjong2DGameManager>().OnGameReady(seat);
        }
#endregion
#region 聊天相关
        public void SendVoiceChat(string url, int len, int seat)
        {
            SFSObject sfsObj = new SFSObject();
            sfsObj.PutUtfString("url", url);
            sfsObj.PutInt(RequestKey.KeySeat, seat);
            sfsObj.PutInt("len", len);
            SendFrameRequest(RequestCmd.Sound, sfsObj);
        }
        private void OnReceiveVoiceChat(ISFSObject requestData)
        {
            if(!App.GetGameData<Mahjong2DGameData>().IsChatVoiceOn)
            {
                return;
            }
            ChatControl.Instance.OnUserSpeak(requestData);
        }
        /// <summary>
        /// 聊天消息
        /// </summary>
        /// <param name="param"></param>
        void OnUserTalk(ISFSObject param)
        {
            ChatControl.Instance.OnUserTalk(param);
        }

        /// <summary>
        /// 发送文本聊天
        /// </summary>
        /// <param name="text"></param>
        public void SendUserTalk(string text)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutUtfString(RequestKey.KeyText, text);                             
            SendUserTalk(iobj,0);
        }
        /// <summary>
        /// 发送表情聊天
        /// </summary>
        /// <param name="index"></param>
        public void SendUserTalk(int index)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutInt(RequestKey.KeyExp, index);
            SendUserTalk(iobj,1);
        }
        /// <summary>
        /// 发送聊天消息
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="type">类型,0：文本消息(包括常用语) 1：常用语 </param>
        private void SendUserTalk(ISFSObject data, int type)
        {
            if (Manager && Manager.SelfPlayer.UserInfo!=null)
            {
                data.PutInt(RequestKey.KeyType, type);
                data.PutInt(RequestKey.KeySeat, Manager.SelfPlayer.UserSeat);
                SendFrameRequest(RequestCmd.UserTalk, data);
            }
        }

#endregion
#region 投票相关
        // 发起解散投票
        public void StartHandsUp(int yon)
        {
            if (!App.GetGameManager<Mahjong2DGameManager>().IsGameing && Data.IsFirstTime)
            {
                    //局外只有房主才能解散，其他玩家只能退出
                    if (App.GetGameManager<Mahjong2DGameManager>().SelfPlayer.UserInfo.id == App.GetGameData<Mahjong2DGameData>().OwnerId)
                    {
                        YxDebug.Log("局外房主申请");
                        SendFrameRequest("dissolve", new SFSObject());
                    }
                    else
                    {
                        YxMessageBox.Show("非房主不能发起解散!",null,null,true);
                    }
            }
            else
            {
                //局内任何玩家都可以发起投票
                ISFSObject iobj = new SFSObject();
                iobj.PutUtfString(RequestKey.Cmd, "dismiss");
                iobj.PutInt(RequestKey.KeyType, yon);
                iobj.PutInt(RequestKey.KeyId,App.GetGameManager<Mahjong2DGameManager>().SelfPlayer.UserInfo.id);
                SendFrameRequest(RequestCmd.HandsUp, iobj);
            }
        }
        // 返回投票
        public void OnHandsUp(ISFSObject requestData)
        {
            HupData data=new HupData()
            {
                Name = requestData.GetUtfString(RequestKey.KeyUserName),
                ID  = requestData.GetInt(RequestKey.KeyId),
                Operation = requestData.GetInt(RequestKey.KeyType)
            };
            int time = Data.HupTime;            
            if (requestData.ContainsKey(RequestKey.KeyCDTime))
            {
                time = requestData.GetInt(RequestKey.KeyCDTime);
            }
            HupWindow.Instance.ShowHandUp(data,time);
        }

        private void CustomUserState(ISFSObject  data)
        {
            if (Manager)
            {
                int seat = data.GetInt("seat");
                bool focus = data.GetBool("focus");
                if (focus)
                    Manager.UserOnLine(seat, data);
                else
                    Manager.UserIdle(seat, data);
            }
        }

        #endregion
#region 游戏结算
        private void OnGameOver(ISFSObject requestData)
        {
            YxDebug.Log("大结算：OnGameOver");
            App.GetGameManager<Mahjong2DGameManager>().OnGameOver(requestData);
            if (IsInHandsUp)
            {
                HupWindow.Instance.OnWindowClose = delegate
                {
                    App.GetGameManager<Mahjong2DGameManager>().ShowGameOver();
                };
                HupWindow.Instance.Close();
            }  
        }

#endregion

#region 选择漂请求

        public void OnSelectPiao(int SelectNum)
        {
            SFSObject data = SFSObject.NewInstance();
            var key = GameKeyWithPoint + RequestCmd.Request;
            data.PutInt(RequestKey.KeyPiao, SelectNum);
            data.PutInt(RequestKey.KeyType,(int)EnumRequest.ShowPiao);
            SendFrameRequest(key,data);
        }

#endregion
#region GPS 信息

        void OnReciveGPSInfo(ISFSObject data)
        {
            App.GetGameManager<Mahjong2DGameManager>().CheckGpsInfo(data);
        }

#endregion
        /// <summary>
        /// 选段门消息发送
        /// </summary>
        /// <param name="num"></param>
        public void ResuqestDuanMen(int num)
        {
            SFSObject request = GameTools.getSFSObject((int)EnumRequest.XuanDuanMen);
            request.PutInt(RequestKey.KeyDuanMen, num);
            SendGameRequest(request);
        }

        public void RequestGuoDan(int select)
        {
            SFSObject request = GameTools.getSFSObject((int)EnumRequest.GuoDanSelect);
            if (select.Equals(ConstantData.DanSelectNum))
            {
                request.PutInt(RequestKey.KeyOk, ConstantData.DanSelectNum);
            }
            SendGameRequest(request);
        }

        /// <summary>
        /// 发送YK非手写聊天
        /// </summary>
        /// <param name="index"></param>
        public void SendYkUserTalk(int index)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutInt(RequestKey.KeySeat, App.GetGameManager<Mahjong2DGameManager>().SelfPlayer.UserSeat);
            iobj.PutInt(RequestKey.KeyExp, index);
            SendFrameRequest(RequestCmd.UserTalk, iobj);
        }
        /// <summary>
        /// 发送营口麻将的手写文字
        /// </summary>
        /// <param name="text"></param>
         public void SendYkHandUserTalk(string text)
         {
             ISFSObject iobj = new SFSObject();
             iobj.PutInt(RequestKey.KeySeat, App.GetGameManager<Mahjong2DGameManager>().SelfPlayer.UserSeat);
             iobj.PutUtfString(RequestKey.KeyText, text);          
             SendFrameRequest(RequestCmd.UserTalk, iobj);
         }

        private void OnAutoRequest(KeyValuePair<int,bool> autoStateChange)
        {
            ISFSObject iobj = GameTools.getSFSObject((int)EnumRequest.Auto);
            iobj.PutBool(RequestKey.KeyAuto, autoStateChange.Value);
            SendGameRequest(iobj);
        }
    }
}

