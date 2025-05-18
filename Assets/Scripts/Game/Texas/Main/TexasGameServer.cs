using System;
using System.Collections;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using Sfs2X.Requests;

namespace Assets.Scripts.Game.Texas.Main
{
    public class TexasGameServer : YxGameServer
    {
        /// <summary>
        /// 交互初始化
        /// </summary>
        /// <param name="callBackDic"></param>
        public override void Init(Dictionary<string, Action<ISFSObject>> callBackDic)
        {
            callBackDic[GameKey + "over"] = OnRoomGameOver;
            callBackDic["hup"] = OnHandsUp;
        }  

        /// <summary>
        /// 发起和决定解散房间
        /// 2发起解散，3同意，-1拒绝
        /// </summary>
        public void DismissRoom(int yon)
        {
            YxDebug.Log("发起解散投票");
            var iobj = new SFSObject();
            iobj.PutUtfString("cmd", "dismiss");
            iobj.PutInt("id", App.GameData.GetPlayerInfo().Id);
            iobj.PutInt(RequestKey.KeyType, yon);
            YxDebug.LogWrite("发起解散投票" + iobj);
            var request = new ExtensionRequest("hup", iobj);
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
            var gMgr = App.GetGameManager<TexasGameManager>();
            gMgr.RModelMagr.SetDismissRoomInfo(data);
        }
          
        /// <summary>
        /// 房间模式结算
        /// </summary>
        /// <param name="param"></param>
        public void OnRoomGameOver(ISFSObject param)
        {
            var gMgr = App.GetGameManager<TexasGameManager>();
            var users = param.GetSFSArray("users");
            var items = gMgr.RModelMagr.ResultItems;

            var index = 0;

            var biggestWinGold = 0;
            var biggestWinIndex = -1;
            YxDebug.LogError("----------------------------", "OnRoomGameOver");
            var gdata = App.GameData;
            foreach (ISFSObject user in users)
            {
                int id = user.GetInt("id");
                if (id < 0) continue;

                int seat = user.GetInt("seat");
                var userInfo = gdata.GetPlayerInfo(seat, true);
                if (userInfo == null) continue;

                var item = items[index];
                item.SetResultItem(user, userInfo);

                //存储最大赢钱数
                if (item.WinGold > biggestWinGold)
                {
                    biggestWinGold = item.WinGold;
                    biggestWinIndex = index;
                }
                index++;
            }
            if(biggestWinIndex >= 0)
                items[biggestWinIndex].SetBigWinnerMark(true);

            gMgr.RModelMagr.ShowRoomResult(index - 1);
        }

        #region 发送交互

        /// <summary>
        /// 发送准备信息
        /// </summary>
        public void SendReadyGame()
        {
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

            if (!HasGetGameInfo)
            {
                YxDebug.LogError("GameInfo还没有初始化!!");
                return;
            }
           
            var sfsObject = new SFSObject();
            switch (rt)
            {
                case GameRequestType.Bet:
                    {
                        sfsObject.PutInt("gold", (int)data["gold"]);
                        sfsObject.PutInt("type", (int)rt);
                    }
                    break;
                case GameRequestType.Fold:
                    sfsObject.PutInt("type", (int)rt);
                    break;
             
                case GameRequestType.SetGold:
                    {
                        sfsObject.PutInt("gold", (int)data["gold"]);
                        sfsObject.PutInt("type", (int) rt);
                    }
                    break;
            }

            YxDebug.Log("发送数据:");
            YxDebug.TraceSfsObject(sfsObject);

            SendGameRequest(sfsObject);
        }
        #endregion

    }

    /// <summary>
    /// 游戏服务交互
    /// </summary>
    public enum GameRequestType
    {
        /// <summary>
        /// 1.下注
        /// </summary>
        Bet = 1,        //下注
        /// <summary>
        /// 2.弃牌
        /// </summary>
        Fold,           //弃牌
        /// <summary>
        /// 3.发牌
        /// </summary>
        HandCard,       //发牌
        /// <summary>
        /// 4.结算
        /// </summary>
        Request,        //结算
        /// <summary>
        /// 5.说话座位
        /// </summary>
        Speaker,        //说话座位
        /// <summary>
        /// 6.设置携带金币
        /// </summary>
        SetGold,        //设置携带金币
        /// <summary>
        /// 7.可以准备了
        /// </summary>
        AllowReady,     //可以准备了
    }
}