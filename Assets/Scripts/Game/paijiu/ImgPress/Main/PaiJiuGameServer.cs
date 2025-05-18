using System;
using System.Collections;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;

namespace Assets.Scripts.Game.paijiu.ImgPress.Main
{
    public class PaiJiuGameServer : YxGameServer
    {

        /// <summary>
        /// 交互初始化
        /// </summary>
        /// <param name="callBackDic"></param>
        public override void Init(Dictionary<string, Action<ISFSObject>> callBackDic)
        {
            callBackDic["hup"] = OnHandsUp;
            callBackDic[GameKey + "over"] = OnGameOver;
            callBackDic["locat"] = OnReciveGPSInfo;
        }

        #region 发送交互

        /// <summary>
        /// 发送准备信息
        /// </summary>
        public void ReadyGame()
        {
            //YxDebug.Log("发送准备!!");
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
            switch (rt)
            {
                case GameRequestType.UserBet:
                    sfsObject.PutInt("gold", (int)data["gold"]);
                    sfsObject.PutInt("seat", (int)data["seat"]);
                    sfsObject.PutInt("type", (int)rt);
                    break;

                case GameRequestType.PutCard:
                    sfsObject.PutInt("cards", (int)data["gold"]);
                    sfsObject.PutInt("seat", (int)data["seat"]);
                    sfsObject.PutInt("type", (int)GameRequestType.PutCard);
                    break;

                case GameRequestType.AllowStart:
                    sfsObject.PutInt("type", (int)GameRequestType.AllowStart);
                    break;
            }
            YxDebug.Log("发送数据:");
            SendGameRequest(sfsObject);
            YxDebug.Log(" ----- 发送结束 ----- ");
        }

        #endregion

        /// <summary>
        /// 有玩家要求解散房间
        /// </summary>
        /// <param name="data"></param>
        void OnHandsUp(ISFSObject data)
        {
            App.GetGameManager<PaiJiuGameManager>().DismissRoomMgr.UpdateDismissInfo(data.GetInt("id"), data.GetInt("type"));
        }


        /// <summary>
        /// 当游戏结束时,运行处理数据
        /// </summary>
        /// <param name="data"></param>
        void OnGameOver(ISFSObject data)
        {
            if (data.ContainsKey("users"))
            {
                App.GetGameManager<PaiJiuGameManager>().RoomResult.ShowResultView(data);
            }
        }

        /// <summary>
        /// 获取GPS信息
        /// </summary>
        /// <param name="data"></param>
        void OnReciveGPSInfo(ISFSObject data)
        {
            Debug.Log("接收到GPS信息" + data.GetInt("uid"));
            App.GetGameManager<PaiJiuGameManager>().CheckGpsInfo(data);
        }

        /// <summary>
        /// 发起和决定解散房间
        /// 2发起解散，3同意，-1拒绝
        /// </summary>
        public void DismissRoom(int yon)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutUtfString("cmd", "dismiss");
            iobj.PutInt("id", int.Parse(App.GetGameData<PaiJiuGameData>().GetPlayerInfo().UserId));
            iobj.PutInt(RequestKey.KeyType, yon);

            IRequest request = new ExtensionRequest("hup", iobj);
            App.GetRServer<PaiJiuGameServer>().SendRequest(request);
        }
    }

    /// <summary>
    /// 游戏服务交互
    /// </summary>
    public enum GameRequestType
    {
        None = -1,
        /// <summary>
        /// 1.允许开始
        /// </summary>
        AllowStart = 1,          //设置携带金币
        /// <summary>
        /// 2.开始下注
        /// </summary>
        BeginBet = 2,            //下注
        /// <summary>
        /// 3.玩家下注(服务器发布3)
        /// </summary>
        UserBet = 3,           //弃牌
        /// <summary>
        /// 3.发牌
        /// </summary>
        SendCard = 4,          //发牌
        /// <summary>
        /// 5.组牌
        /// </summary>
        PutCard = 5,        //说话座位
        /// <summary>
        /// 6.可以准备了
        /// </summary>
        AllowReady = 6,     //可以准备了
        /// <summary>
        /// 7.玩家查看已经出现的牌
        /// </summary>
        CheckCards = 7,

        /// <summary>
        /// 18.小结算
        /// </summary>
        Compare = 18,        //结算
        /// <summary>
        /// 意味着游戏开始
        /// </summary>
        GameBegin = 10086,  //获取数据(游戏开始)
    }
}