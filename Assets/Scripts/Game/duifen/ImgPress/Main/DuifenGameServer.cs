using System;
using System.Collections;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;

namespace Assets.Scripts.Game.duifen.ImgPress.Main
{
    public class DuifenGameServer : YxGameServer
    {



        /// <summary>
        /// 交互初始化
        /// </summary>
        /// <param name="callBackDic"></param>
        public override void Init(Dictionary<string, Action<ISFSObject>> callBackDic)
        {
            //callBackDic[GameKey + RequestCmd.JoinRoom] = OnOtherJoinRoom;
            //callBackDic[GameKey + RequestCmd.Ready] = OnReady;
            callBackDic["hup"] = OnHandsUp;
            callBackDic[GameKey + "over"] = OnGameOver;
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
        /// <param name="rt">游戏状态</param>
        /// <param name="data">下注金额键值对,key为gold</param>
        public void SendRequest(GameRequestType rt, IDictionary data)
        {
            YxDebug.Log("!!SendRequest == " + rt);

            SFSObject sfsObject = new SFSObject();
            switch (rt)
            {
                case GameRequestType.Bet:
                    sfsObject.PutInt("gold", (int)data["gold"]);
                    sfsObject.PutInt("seat", (int)data["seat"]);
                    sfsObject.PutInt("type", (int)rt);
                    break;

                case GameRequestType.Fold:
                    sfsObject.PutInt("type", (int)rt);
                    break;
                case GameRequestType.KaiPai:
                    sfsObject.PutInt("gold", (int)data["gold"]);
                    sfsObject.PutInt("seat", (int)data["seat"]);
                    sfsObject.PutInt("type", (int)GameRequestType.Bet);
                    sfsObject.PutBool("kaipai", true);
                    break;
                case GameRequestType.QiLi:
                    sfsObject.PutInt("gold", (int)data["gold"]);
                    sfsObject.PutInt("seat", (int)data["seat"]);
                    sfsObject.PutInt("type", (int)GameRequestType.Bet);
                    break;
                case GameRequestType.CouldStart:
                    sfsObject.PutInt("type", (int)GameRequestType.CouldStart);
                    break;

                case GameRequestType.SystemAuto:
                    sfsObject.PutInt("type", (int)GameRequestType.SystemAuto);
                    break;
            }
            YxDebug.Log("发送数据:");
            YxDebug.TraceSfsObject(sfsObject);

            SendGameRequest(sfsObject);
        }


        /// <summary>
        /// 有玩家要求解散房间
        /// </summary>
        /// <param name="data"></param>
        void OnHandsUp(ISFSObject data)
        {
            App.GetGameManager<DuifenGameManager>().DismissRoomMgr.UpdateDismissInfo(data.GetInt("id"), data.GetInt("type"));
        }

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

        public void SendPlayerInputString(string text)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutUtfString(RequestKey.KeyText, text);
            SendUserTalk(iobj, 2);
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
            iobj.PutInt("otherSeat", seat);//其他的玩家座位号

            IRequest request = new ExtensionRequest("talk", iobj);

            SendRequest(request);
        }


        /// <summary>
        /// 发送聊天消息
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="type">类型,0：文本消息(包括常用语) 1：常用语 2.后台状态（临时处理）</param>
        private void SendUserTalk(ISFSObject data, int type)
        {
            if (!App.GetGameData<DuifenGlobalData>().IsGameInfo)
            {
                return;
            }
            data.PutInt(RequestKey.KeyType, type);
            data.PutInt(RequestKey.KeySeat, App.GameData.SelfSeat);
            SendFrameRequest(RequestCmd.UserTalk, data);
        }

        /// <summary>
        /// 发送表情聊天
        /// </summary>
        /// <param name="index"></param>
        public void SendUserPhiz(int index)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutInt(RequestKey.KeyExp, index);
            SendUserTalk(iobj, 1);
        }
        /// <summary>
        /// 当游戏结束时,运行处理数据
        /// </summary>
        /// <param name="data"></param>
        void OnGameOver(ISFSObject data)
        {
            var manager = App.GetGameManager<DuifenGameManager>();
            manager.RoomResult.ShowResultView(data);
            manager.StopCountDown();

            var gdata = App.GetGameData<DuifenGlobalData>();
            gdata.IsGameing = false;
        }


        /// <summary>
        /// 发起和决定解散房间
        /// 2发起解散，3同意，-1拒绝
        /// </summary>
        public void DismissRoom(int yon)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutUtfString("cmd", "dismiss");
            iobj.PutInt("id", App.GameData.GetPlayerInfo().Id);
            iobj.PutInt(RequestKey.KeyType, yon);

            IRequest request = new ExtensionRequest("hup", iobj);
            App.GetRServer<DuifenGameServer>().SendRequest(request);
        }
    }


}