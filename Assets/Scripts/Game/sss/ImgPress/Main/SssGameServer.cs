using System;
using System.Collections;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;

namespace Assets.Scripts.Game.sss.ImgPress.Main
{
    public class SssGameServer : YxGameServer
    {
        /// <summary>
        /// 交互初始化
        /// </summary>
        /// <param name="callBackDic"></param>
        public override void Init(Dictionary<string, Action<ISFSObject>> callBackDic)
        {
            callBackDic["hup"] = OnHandsUp;
            callBackDic[GameKey + "over"] = OnGameOver;
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
        /// <param name="rt">游戏状态</param>
        /// <param name="data">下注金额键值对,key为gold</param>
        public void SendRequest(int rt, IDictionary data)
        {
            YxDebug.Log("!!SendRequest == " + rt);
            if (!HasGetGameInfo)
            {
                YxDebug.LogError("GameInfo还没有初始化完成!!");
                return;
            }
            SFSObject sfsObject = new SFSObject();

            switch (rt)
            {
                //发送选择的牌型
                case GameRequestType.CouldStart:
                    sfsObject.PutInt("type", rt);
                    break;
            }

            YxDebug.Log("发送数据:");
            YxDebug.TraceSfsObject(sfsObject);

            SendGameRequest(sfsObject);
        }

        #endregion

        /// <summary>
        /// 有玩家要求解散房间
        /// </summary>
        /// <param name="data"></param>
        void OnHandsUp(ISFSObject data)
        {
            App.GetGameManager<SssGameManager>().DismissRoomMgr.UpdateDismissInfo(data.GetInt("id"), data.GetInt("type"));
        }

        /// <summary>
        /// 当游戏结束时,运行处理数据
        /// </summary>
        /// <param name="data"></param>
        void OnGameOver(ISFSObject data)
        {
            var main = App.GetGameManager<SssGameManager>();
            main.Reset();
            main.SummaryMgr.Init(data);

            var gdata = App.GetGameData<SssGameData>();
            gdata.IsPlaying = false;
        }
    }

    /// <summary>
    /// 游戏服务交互
    /// </summary>
    public class GameRequestType
    {
        /// <summary>
        /// 0.空
        /// </summary>
        public const int None = 0;

        /// <summary>
        /// 1.发牌
        /// </summary>
        public const int Cards = 1;

        /// <summary>
        /// 2.比牌
        /// </summary>
        public const int Match = 2;

        /// <summary>
        /// 3.结算
        /// </summary>
        public const int Result = 3;

        /// <summary>
        /// 4.选择牌型结束
        /// </summary>
        public const int FinishChoise = 4;

        /// <summary>
        /// 5.允许准备
        /// </summary>
        public const int AllowReady = 5;

        /// <summary>
        /// 6.允许开始游戏
        /// </summary>
        public const int CouldStart = 6;
    }
}