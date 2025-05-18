using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Game.lyzz2d.Game.Component;
using Assets.Scripts.Game.lyzz2d.Game.UI;
using Assets.Scripts.Game.lyzz2d.Utils;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.View;

namespace Assets.Scripts.Game.lyzz2d.Game.GameCtrl
{
    public class Lyzz2DGameServer : YxGameServer
    {
        [HideInInspector]
        /// <summary>
        /// 是否正在投票解散
        /// </summary>
        public bool IsInHandsUp;

        public Lyzz2DGlobalData Data
        {
            get { return App.GetGameData<Lyzz2DGlobalData>(); }
        }

        public Lyzz2DGameManager Manager
        {
            get { return App.GetGameManager<Lyzz2DGameManager>(); }
        }

        [HideInInspector]
        public string GameKeyWithPoint
        {
            get { return App.GameKey + "."; }
        }

        public override void Init(Dictionary<string, Action<ISFSObject>> callBackDic)
        {
            base.Init(callBackDic);
            callBackDic[GameKeyWithPoint + RequestCmd.JoinRoom] = OnOtherJoinRoom;
            callBackDic[GameKeyWithPoint + RequestCmd.Ready] = OnGameReady;
            callBackDic[GameKeyWithPoint + RequestCmd.RollDice] = App.GetGameManager<Lyzz2DGameManager>().RollDice;
            callBackDic[GameKeyWithPoint + RequestCmd.GameOver] = OnGameOver;
            callBackDic[RequestCmd.HandsUp] = OnHandsUp;
            callBackDic[RequestCmd.UserTalk] = OnUserTalk;
            callBackDic[RequestCmd.Sound] = OnReceiveVoiceChat;
            callBackDic[RequestCmd.LocatPosition] = OnReciveGPSInfo;
        }

        private void OnOtherJoinRoom(ISFSObject currResponse)
        {
            YxDebug.Log("其他玩家加入游戏");
            if (!App.GetGameManager<Lyzz2DGameManager>().IsInitInfo)
            {
                return;
            }
            ISFSObject user;
            GameTools.TryGetValueWitheKey(currResponse, out user, RequestKey.KeyUser);
            var data = new UserData(user);
            var isExist = false;
            var index = -1;
            for (int i = 0, lenth = Data.UserDatas.Count; i < lenth; i++)
            {
                var haveUser = Data.UserDatas[i];
                var haveData = new UserData(haveUser);
                if (haveData.Seat.Equals(data.Seat))
                {
                    Data.UserDatas[i] = user;
                    isExist = true;
                    index = i;
                    break;
                }
            }
            if (!isExist)
            {
                Data.UserDatas.Add(user);
            }
            Manager.OnJoinGame(data);
            YxDebug.Log(string.Format("目前存在的玩家数量是：{0}", Data.UserDatas.Count));
        }

        #region 游戏结算

        private void OnGameOver(ISFSObject requestData)
        {
            YxDebug.Log("大结算：OnGameOver");
            if (IsInHandsUp)
            {
                HupWindow.Instance.Close();
                HupWindow.Instance.OnWindowClose = delegate { App.GetGameManager<Lyzz2DGameManager>().ShowGameOver(); };
            }
            App.GetGameManager<Lyzz2DGameManager>().OnGameOver(requestData);
        }

        #endregion

        #region 选择漂请求

        public void OnSelectPiao(int SelectNum)
        {
            var data = SFSObject.NewInstance();
            var key = GameKeyWithPoint + RequestCmd.Request;
            data.PutInt(RequestKey.KeyPiao, SelectNum);
            data.PutInt(RequestKey.KeyType, (int) EnumRequest.ShowPiao);
            SendFrameRequest(key, data);
        }

        #endregion

        #region GPS 信息

        private void OnReciveGPSInfo(ISFSObject data)
        {
            App.GetGameManager<Lyzz2DGameManager>().CheckGpsInfo(data);
        }

        #endregion

        public void ResuqestDuanMen(int num)
        {
            var request = GameTools.getSFSObject((int) EnumRequest.XuanDuanMen);
            request.PutInt(RequestKey.KeyDuanMen, num);
            SendGameRequest(request);
        }

        #region 游戏准备

        public void Ready()
        {
            YxDebug.Log("发送准备请求");
            if (App.GetGameManager<Lyzz2DGameManager>().IsInit() || App.GetGameManager<Lyzz2DGameManager>().IsWaiting() ||
                App.GetGameManager<Lyzz2DGameManager>().IsAccount())
            {
                var key = GameKeyWithPoint + RequestCmd.Ready;
                SendFrameRequest(key, new SFSObject());
            }
        }

        private void OnGameReady(ISFSObject currResponse)
        {
            var seat = currResponse.GetInt(RequestKey.KeySeat);
            App.GetGameManager<Lyzz2DGameManager>().OnGameReady(seat);
        }

        #endregion

        #region 聊天相关

        public void SendVoiceChat(string url, int len, int seat)
        {
            var sfsObj = new SFSObject();
            sfsObj.PutUtfString("url", url);
            sfsObj.PutInt(RequestKey.KeySeat, seat);
            sfsObj.PutInt("len", len);
            SendFrameRequest(RequestCmd.Sound, sfsObj);
        }

        private void OnReceiveVoiceChat(ISFSObject requestData)
        {
            if (!App.GetGameData<Lyzz2DGlobalData>().IsChatVoiceOn)
            {
                return;
            }
            ChatControl.Instance.OnUserSpeak(requestData);
        }

        private void OnUserTalk(ISFSObject param)
        {
            ChatControl.Instance.OnUserTalk(param);
        }

        /// <summary>
        ///     发送文本聊天
        /// </summary>
        /// <param name="text"></param>
        public void SendUserTalk(string text)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutUtfString(RequestKey.KeyText, text);
            SendUserTalk(iobj, 0);
        }

        /// <summary>
        ///     发送表情聊天
        /// </summary>
        /// <param name="index"></param>
        public void SendUserTalk(int index)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutInt(RequestKey.KeyExp, index);
            SendUserTalk(iobj, 1);
        }

        /// <summary>
        ///     发送人物状态变化请求
        /// </summary>
        /// <param name="state"></param>
        public void SendUserTalk(bool state)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutBool(RequestKey.KeyIsOnLine, state);
            SendUserTalk(iobj, 2);
        }

        /// <summary>
        ///     发送聊天消息
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="type">类型,0：文本消息(包括常用语) 1：常用语 2.后台状态（临时处理）</param>
        private void SendUserTalk(ISFSObject data, int type)
        {
            if (!Manager.IsInitInfo)
            {
                return;
            }
            data.PutInt(RequestKey.KeyType, type);
            data.PutInt(RequestKey.KeySeat, App.GetGameManager<Lyzz2DGameManager>().SelfPlayer.UserSeat);
            SendFrameRequest(RequestCmd.UserTalk, data);
            NguiLabelAdapter ADA;
        }

        #endregion

        #region 投票相关

        // 发起解散投票
        public void StartHandsUp(int yon)
        {
            //局外
            if (App.GetGameManager<Lyzz2DGameManager>().IsInit())
            {
                //局外只有房主才能解散，其他玩家只能退出
                if (App.GetGameManager<Lyzz2DGameManager>().SelfPlayer.UserInfo.id == App.GetGameData<Lyzz2DGlobalData>().CurrentGame.OwnerId)
                {
                    YxDebug.Log("局外房主申请");
                    SendFrameRequest("dissolve", new SFSObject());
                }
                else
                {
                    YxMessageBox.Show("非房主不能发起解散!");
                }
            }
            else
            {
                //局内任何玩家都可以发起投票
                ISFSObject iobj = new SFSObject();
                iobj.PutUtfString(RequestKey.Cmd, "dismiss");
                iobj.PutInt(RequestKey.KeyType, yon);
                iobj.PutInt(RequestKey.KeyId, App.GetGameManager<Lyzz2DGameManager>().SelfPlayer.UserInfo.id);
                SendFrameRequest(RequestCmd.HandsUp, iobj);
            }
        }

        // 返回投票
        public void OnHandsUp(ISFSObject requestData)
        {
            var data = new HupData
            {
                Name = requestData.GetUtfString(RequestKey.KeyUserName),
                ID = requestData.GetInt(RequestKey.KeyId),
                Operation = requestData.GetInt(RequestKey.KeyType)
            };
            var time = 300;
            if (requestData.ContainsKey(RequestKey.KeyCDTime))
            {
                time = requestData.GetInt(RequestKey.KeyCDTime);
            }
            HupWindow.Instance.ShowHandUp(data, time);
        }

        #endregion
    }
}