using System;
using System.Collections.Generic;
using Assets.Scripts.Game.sanpian.DataStore;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using RequestKey = YxFramwork.ConstDefine.RequestKey;

namespace Assets.Scripts.Game.sanpian.server
{//连接远程服务器
    public class SanPianGameServer : YxGameServer
    {
        public override void Init(Dictionary<string, Action<ISFSObject>> responseDic)
        {
            responseDic[GameKey + RequestCmd.JoinRoom] = OnOtherJoinRoom;
            responseDic[GameKey + RequestCmd.Ready] = OnGameReady;
       
//            responseDic[RequestCmd.UserTalk] = OnUserTalk;
            ////开房模式总结算
            responseDic[GameKey + "over"] = OnRoomGameOver;
            ////接收语音
//            responseDic["sound"] = OnReceiveVoiceChat;
            ////投票解散
            responseDic["hup"] = OnHandsUp;
            ////收到GPS信息
            responseDic["locat"] = OnReciveGPSInfo;

        }


//        private void OnUserTalk(ISFSObject obj)
//        {
//            App.GetGameManager<SanPianGameManager>().OnTalk(obj);
//        }
//
//        private void OnReceiveVoiceChat(ISFSObject obj)
//        {
//            App.GetGameManager<SanPianGameManager>().OnSpeak(obj);
//        }

        private void OnRoomGameOver(ISFSObject obj)
        {
            App.GetGameManager<SanPianGameManager>().OnGameOver(obj);
        }

        private void OnOtherJoinRoom(ISFSObject obj)
        {
            ISFSObject user = obj.GetSFSObject(RequestKey.KeyUser);
            UserInfo info = DataParse.instance.GetUserInfo(user);
            App.GetGameManager<SanPianGameManager>().OtherUserJoin(info);
        }

        private void OnGameReady(ISFSObject obj)
        {
            int seat = obj.GetInt(RequestKey.KeySeat);
            int myseat = App.GetGameManager<SanPianGameManager>().RealPlayer.userInfo.Seat;
            if (myseat != null && myseat == seat)
            {
                App.GetGameManager<SanPianGameManager>().UIButtonCtrl.ReadyBt.SetActive(false);
            }
            App.GetGameManager<SanPianGameManager>().PlayerArr[seat].UIInfo.ReadyIcon.SetActive(true);

        }



        public void ClickReadyBt()
        {
            string key = GameKey + RequestCmd.Ready;
            IRequest request = new ExtensionRequest(key, SFSObject.NewInstance());
            SendRequest(request);
        }


        #region 投票相关
        public bool IsInHandsUp;
        // 发起解散投票
        public void StartHandsUp(int yon)
        {
            //局内任何玩家都可以发起投票
            ISFSObject iobj = new SFSObject();
            iobj.PutUtfString("cmd", "dismiss");
            iobj.PutInt(RequestKey.KeyType, yon);
            iobj.PutInt(RequestKey.KeyId, App.GetGameManager<SanPianGameManager>().RealPlayer.userInfo.Id);
            SendFrameRequest("hup", iobj);
        }
        // 返回投票
        public void OnHandsUp(ISFSObject data)
        {
            var gmanager = App.GetGameManager<SanPianGameManager>();
            if (data.ContainsKey("type") && data.GetInt("type") == 2)
            {
                gmanager.DismissRoomMgr.ShowRoomDismiss();

            }
            gmanager.DismissRoomMgr.UpdateDismissInfo(data.GetInt("id"), data.GetInt("type"));
        }

        public void SendUserTalk(string text)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutUtfString("text", text);
            iobj.PutInt(RequestKey.KeySeat, App.GetGameManager<SanPianGameManager>().RealPlayer.userInfo.Seat);
            SendFrameRequest(RequestCmd.UserTalk, iobj);
        }

        public void SendUserPhizTalk(int index)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutInt("exp", index);
            iobj.PutInt(RequestKey.KeySeat, App.GetGameManager<SanPianGameManager>().RealPlayer.userInfo.Seat);
            SendFrameRequest(RequestCmd.UserTalk, iobj);
        }

        void OnReciveGPSInfo(ISFSObject data)
        {
            YxDebug.Log("收到了GPS信息");
            App.GetGameManager<SanPianGameManager>().CheckGpsInfo(data);
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


        #endregion
    }
}
