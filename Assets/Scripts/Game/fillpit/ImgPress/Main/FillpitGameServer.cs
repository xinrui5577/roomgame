using System;
using System.Collections;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.fillpit.ImgPress.Main
{
    public class FillpitGameServer : YxGameServer
    {

        /// <summary>
        /// 交互初始化
        /// </summary>
        /// <param name="callBackDic"></param>
        public override void Init(Dictionary<string, Action<ISFSObject>> callBackDic)
        {
            //callBackDic[GameKey + RequestCmd.JoinRoom] = OnOtherJoinRoom;
            callBackDic["hup"] = OnHandsUp;
            callBackDic[GameKey + "over"] = OnGameOver;
            callBackDic["readyCd"] = ShowClock;
        }
    
        #region 接收交互
        
      

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



        #endregion

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
        /// <param name="rt">游戏状态</param>
        /// <param name="data">下注金额键值对,key为gold</param>
        public void SendRequest(GameRequestType rt, IDictionary data)
        {
            YxDebug.Log("!!SendRequest == " + rt);
            
            if (!HasGetGameInfo)
            {
                YxDebug.LogError("GameInfo还没有初始化!!");
                return;
            }
            SFSObject sfsObject = new SFSObject();

            //var selfPlayer = App.GetGameData<FillpitGameData>().GetPlayer<PlayerPanel>();

            //sfsObject 
            switch (rt)
            {
                case GameRequestType.BetSpeak:
                    sfsObject.PutInt("gold", (int)data["gold"]);
                    sfsObject.PutInt("type", (int)rt);
                    break;
                case GameRequestType.Bet:
                    sfsObject.PutInt("gold", (int)data["gold"]);
                    sfsObject.PutInt("type", (int)rt);
                    break;
                case GameRequestType.Card:
                    break;
                case GameRequestType.Fold:
                    sfsObject.PutInt("type", (int)rt);
                    break;
                case GameRequestType.FollowSpeak:
                    sfsObject.PutInt("gold", (int)data["gold"]);
                    break;
                case GameRequestType.KickSpeak:
                    sfsObject.PutInt("gold", (int)data["gold"]);
                    break;
                case GameRequestType.NotKick:
                    sfsObject.PutInt("type", (int)rt);
                    break;
                case GameRequestType.BackKick:
                    sfsObject.PutInt("gold", (int)data["gold"]);
                    break;
                case GameRequestType.StartGame:
                    sfsObject.PutInt("type", (int)rt);
                    break;
                case GameRequestType.WatchCard:
                    sfsObject.PutInt("type", (int)rt);
                    break;
            }

            //SmartManager.SmartFox.Send(new ExtensionRequest(GlobalData.GameKey + RequestCmd.Request, sfsObject));

            YxDebug.Log("发送数据:");
            YxDebug.TraceSfsObject(sfsObject);

            SendGameRequest(sfsObject);

        }
        #endregion

        void ShowClock(ISFSObject data)
        {
            if (data.ContainsKey("readyCd"))
            {
                var gdata= App.GetGameData<FillpitGameData>();
                gdata.ReadyCd = data.GetInt("readyCd");
                App.GetGameManager<FillpitGameManager>().SetClock();
            }
        }
        /// <summary>
        /// 有玩家要求解散房间
        /// </summary>
        /// <param name="data"></param>
        void OnHandsUp(ISFSObject data)
        {
            App.GetGameManager<FillpitGameManager>().DismissRoomMgr.UpdateDismissInfo(data.GetInt("id"), data.GetInt("type"));
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
        /// 发送聊天消息
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="type">类型,0：文本消息(包括常用语) 1：常用语 2.后台状态（临时处理）</param>
        private void SendUserTalk(ISFSObject data, int type)
        {
            var gdata = App.GetGameData<FillpitGameData>();
            if (!HasGetGameInfo)
            {
                return;
            }
            data.PutInt(RequestKey.KeyType, type);
            data.PutInt(RequestKey.KeySeat, gdata.SelfSeat);
            SendFrameRequest(RequestCmd.UserTalk, data);
        }

        /// <summary>
        /// 当游戏结束时,运行处理数据
        /// </summary>
        /// <param name="data"></param>
        void OnGameOver(ISFSObject data)
        {
            if (data.ContainsKey("users"))
            {
                App.GetGameManager<FillpitGameManager>().SummaryMgr.OnGameOver(data);
            }
            YxClockManager.StopWaitPlayer();
        }

        public void SendHandsUp(int yon)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutUtfString("cmd", "dismiss");
            iobj.PutInt("id", App.GetGameData<FillpitGameData>().GetPlayerInfo().Id);
            iobj.PutInt(RequestKey.KeyType, yon);

            IRequest request = new ExtensionRequest("hup", iobj);
            SendRequest(request);
        }
    }

    /// <summary>
    /// 游戏服务交互
    /// </summary>
    public enum GameRequestType
    {
        None = -1,
        /// <summary>
        /// 1.下注说话
        /// </summary>
        BetSpeak = 1,
        /// <summary>
        /// 2.下注
        /// </summary>
        Bet,
        /// <summary>
        /// 3.弃牌
        /// </summary>
        Fold,
        /// <summary>
        /// 4.发牌
        /// </summary>
        Card,
        /// <summary>
        /// 5.结算
        /// </summary>
        Result,
        /// <summary>
        /// 6.是否跟注
        /// </summary>
        FollowSpeak,
        /// <summary>
        /// 7.踢
        /// </summary>
        KickSpeak,
        /// <summary>
        /// 8.不踢
        /// </summary>
        NotKick = 8,
        /// <summary>
        /// 9.看牌小动作
        /// </summary>
        WatchCard = 9,
        /// <summary>
        /// 10.反踢
        /// </summary>
        BackKick = 10,
        /// <summary>
        /// 11.开始游戏
        /// </summary>
        StartGame = 11,
        /// <summary>
        /// 12.搓牌结束
        /// </summary>
        RubDone = 12

    }
}