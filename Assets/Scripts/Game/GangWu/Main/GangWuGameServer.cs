using System;
using System.Collections;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;

namespace Assets.Scripts.Game.GangWu.Main
{
    public class GangWuGameServer : YxGameServer
    {
        /// <summary>
        /// 交互初始化
        /// </summary>
        /// <param name="callBackDic"></param>
        public override void Init(Dictionary<string, Action<ISFSObject>> callBackDic)
        {
            //callBackDic[GameKey + "over"] = OnRoomGameOver;       //港五暂无开房模式
            callBackDic["hup"] = OnHandsUp; //港五暂时没有开房模式
        }

        /// <summary>
        /// 发送动画
        /// </summary>
        /// <param name="index"></param>
        /// <param name="seat"></param>
        public void UserAnimate(int index, int seat)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutInt("ani", index);
            iobj.PutInt("seat", App.GameData.SelfSeat);
            iobj.PutInt("otherSeat", seat); //其他的玩家座位号

            IRequest request = new ExtensionRequest("talk", iobj);

            SendRequest(request);
        }

        /// <summary>
        /// 发送语音
        /// </summary>
        /// <param name="url"></param>
        /// <param name="len"></param>
        /// <param name="seat"></param>
        public void SendVoiceChat(string url, int len, int seat)
        {
            SFSObject sfsObj = new SFSObject();
            sfsObj.PutUtfString("url", url);
            sfsObj.PutInt(RequestKey.KeySeat, seat);
            sfsObj.PutInt("len", len);
            SendFrameRequest("sound", sfsObj);
        }
         
        /// <summary>
        /// 发起和决定解散房间
        /// 2发起解散，3同意，-1拒绝
        /// </summary>
        public void DismissRoom(int yon)
        {
            YxDebug.Log("发起解散投票");
            ISFSObject iobj = new SFSObject();
            iobj.PutUtfString("cmd", "dismiss");
            iobj.PutInt("id", App.GameData.GetPlayer().Info.Id);
            iobj.PutInt(RequestKey.KeyType, yon);
            YxDebug.LogWrite("发起解散投票" + iobj);
            IRequest request = new ExtensionRequest("hup", iobj);
            SendRequest(request);
        }


        /// <summary>
        /// 房主直接解散房间
        /// </summary>
        public void DismissRoom()
        {
            YxDebug.Log("解散房间!");
            IRequest req = new ExtensionRequest("dissolve", new SFSObject());
            SendRequest(req);
        }

        void OnHandsUp(ISFSObject data)
        {
            var roomMgr = App.GetGameManager<GangWuGameManager>().RModelMgr;

            roomMgr.UpdateDismissInfo(data.GetInt("id"), data.GetInt("type"));
        }

        ///// <summary>
        ///// 发起解散投票 2发起 3同意 -1拒绝
        ///// </summary>
        public void StartHandsUp(int yon = 3)
        {
            YxDebug.Log("发起解散投票");
            ISFSObject iobj = new SFSObject();
            iobj.PutUtfString("cmd", "dismiss");
            iobj.PutInt(RequestKey.KeyType, yon);
            IRequest request = new ExtensionRequest("hup", iobj);
            SendRequest(request);
        }

        #region 发送交互

        /// <summary>
        /// 发送准备信息
        /// </summary>
        public void ReadyGame()
        {
            YxDebug.Log("发送准备!!");
            var key = GameKey + RequestCmd.Ready;
            SendFrameRequest(key, SFSObject.NewInstance());
        }

        /// <summary>
        /// 发送交互信息
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="data"></param>
        public void SendRequest(GameRequestType rt, IDictionary data)
        {
            YxDebug.Log("SendRequest == " + rt);

            if (!App.GetGameData<GangwuGameData>().IsGameInfo)
            {
                YxDebug.LogError("GameInfo还没有初始化!!");
                return;
            }
            SFSObject sfsObject = new SFSObject();
      
            switch (rt)
            {
                case GameRequestType.Bet:
                case GameRequestType.AdvanceBet:
                    if (!data.Contains("gold")) UnityEngine.Debug.LogError("没有Gold");
                    sfsObject.PutInt("gold", (int)data["gold"]);
                    sfsObject.PutInt("type", (int) rt);
                    break;
                case GameRequestType.Fold:
                    sfsObject.PutInt("type", (int) rt);
                    break;
            }

            YxDebug.Log("发送数据:");
            YxDebug.TraceSfsObject(sfsObject);

            SendGameRequest(sfsObject);
        }
         
        #endregion

        /// <summary>
        /// 发送文本聊天
        /// </summary>
        /// <param name="text">发送的消息内容</param>
        public void SendCommonText(string text)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutUtfString(RequestKey.KeyText, text);
            SendUserTalk(iobj, 0);
        }

        /// <summary>
        /// 发送表情聊天
        /// </summary>
        /// <param name="index"></param>
        public void SendUserTalk(int index)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutInt(RequestKey.KeyExp, index);
            SendUserTalk(iobj, 1);
        }

        /// <summary>
        /// 发送聊天消息
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="type">类型,0：文本消息(包括常用语) 1：常用语 2.后台状态（临时处理）</param>
        private void SendUserTalk(ISFSObject data, int type)
        {
            if (!App.GetGameData<GangwuGameData>().IsGameInfo)
            {
                return;
            }
            data.PutInt(RequestKey.KeyType, type);
            data.PutInt(RequestKey.KeySeat, App.GameData.SelfSeat);
            SendFrameRequest(RequestCmd.UserTalk, data);
        }

        public void SendPlayerInputString(string text)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutUtfString(RequestKey.KeyText, text);
            SendUserTalk(iobj, 2);
        }
    }
        /// <summary>
        /// 游戏服务交互
        /// </summary>
        public enum GameRequestType
        {
            /// <summary>
            /// 1.下注说话,服务器指定下注玩家说话,
            ///   如果前面没有下注的人,那么就一直出现BetSpeak
            /// </summary>
            BetSpeak = 1,

            /// <summary>
            /// 2.跟注说话,服务器指定跟注玩家说话,
            ///   当前面有人下注后,不会出现BetSpeak,转为FollowSpeak
            /// </summary>
            FollowSpeak = 2,

            /// <summary>
            /// 3.下注,客户端发送服务器玩家下注筹码值
            /// </summary>
            Bet = 3,

            /// <summary>
            /// 4.弃牌
            /// </summary>
            Fold = 4,

            /// <summary>
            /// 5.发牌
            /// </summary>
            Card = 5,

            /// <summary>
            /// 6.结算
            /// </summary>
            Result = 6,

            /// <summary>
            /// 7.允许准备,客户端显示准备按钮
            /// </summary>
            AllowReady = 7,

            /// <summary>
            /// 8.预加注(只发给服务器,服务器会在自动弃牌的时候检测该值,防止自动弃牌)
            /// </summary>
            AdvanceBet = 8
        }
}