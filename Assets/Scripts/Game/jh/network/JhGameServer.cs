using System;
using System.Collections.Generic;
using Assets.Scripts.Game.jh.EventII;
using Assets.Scripts.Game.jh.Modle;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using YxFramwork.Enums;

namespace Assets.Scripts.Game.jh.network
{
    public class JhGameServer : YxGameServer
    {
        public EventObject EventObj;

        protected JhGameTable GameData;
        public override void Init(Dictionary<string, Action<ISFSObject>> callBackDic)
        {
            GameData = App.GetGameData<JhGameTable>();


            callBackDic["hup"] = OnHandsUp;
            callBackDic[GameKey + "over"] = OnGameOver;
        }

        public void OnRecevie(EventData data)
        {
            switch (data.Name)
            {
                case "ReadyReq":
                    SendRequestReady(data.Data);
                    break;
                case "FollowReq":
                    SendRequestFollow(data.Data);
                    break;
                case "LookReq":
                    SendRequestLook(data.Data);
                    break;
                case "GiveUpReq":
                    SendRequestGiveUp(data.Data);
                    break;
                case "CompareReq":
                    SendRequestCompare(data.Data);
                    break;
                case "ChangeRoom":
                    SendRequestChangeRoom(data.Data);
                    break;
                case "Talker":
                    SendRequestTalk(data.Data);
                    break;
                case "StartReq":
                    SendRequestStart(data.Data);
                    break;
                case "HupReq":
                    SendRequestHandsUp(data.Data);
                    break;
                case "Gzyz":
                    SendGzyz(data.Data);
                    break;
                case "LiangPai":
                    SendLiangPai(data.Data);
                    break;
            }
        }

        //客户端发送给服务器的-----------------------------------------------------------------------------
        //准备
        private void SendLiangPai(object data)
        {
            ISFSObject isfsSata = new SFSObject();
            isfsSata.PutInt(RequestKey.KeyType, JhTypeKey.TypeShowCard);
            SendGameRequest(isfsSata);
        }
        private void SendGzyz(object data)
        {
            ISFSObject isfsSata = new SFSObject();
            isfsSata.PutInt(RequestKey.KeyType, JhTypeKey.TypeGZYZ);
            SendGameRequest(isfsSata);
        }
        protected void SendRequestReady(object data)
        {
            ISFSObject isfsSata = new SFSObject();
            isfsSata.PutInt(RequestKey.KeySeat, App.GameData.SelfSeat);
            SendFrameRequest(GameKey + "ready", isfsSata);
        }
        //跟注
        protected void SendRequestFollow(object data)
        {
            JhUserInfo userInfo = GameData.GetPlayerInfo<JhUserInfo>();
            int gold = (int?) data ?? GameData.SingleBet;
            if (gold < GameData.SingleBet)
            {
                gold = GameData.SingleBet;
            }
            if (userInfo.IsLook)
            {
                gold *= 2;
            }
            if (!GameData.IsCreatRoom&&gold > userInfo.CoinA)
            {
                return;
            }

            ISFSObject isfsSata = new SFSObject();
            isfsSata.PutInt(RequestKey.KeyGold, gold);
            isfsSata.PutInt(RequestKey.KeyType, JhTypeKey.TypeGenZhu);
            SendGameRequest(isfsSata);
        }
        //看牌
        protected void SendRequestLook(object data)
        {
            ISFSObject isfsSata = new SFSObject();
            isfsSata.PutInt(RequestKey.KeyType, JhTypeKey.TypeKanPai);
            SendGameRequest(isfsSata);
        }
        //弃牌
        protected void SendRequestGiveUp(object data)
        {
            ISFSObject isfsSata = new SFSObject();
            isfsSata.PutInt(RequestKey.KeyType, JhTypeKey.TypeQiPai);
            SendGameRequest(isfsSata);
        }
        //比牌
        protected void SendRequestCompare(object data)
        {
            ISFSObject isfsSata = new SFSObject();
            isfsSata.PutInt(RequestKey.KeyType, JhTypeKey.TypeBiPai);
            int seat = GameData.GetSeat((int) data);
            isfsSata.PutInt(JhRequestConstKey.KeyToSeat, seat);
            SendGameRequest(isfsSata);
        }
        //换房
        protected void SendRequestChangeRoom(object data)
        {
            ChangeRoom();
        }
        //说话
        protected void SendRequestTalk(object data)
        {
            var sendData = (ISFSObject)data;
            SendFrameRequest(GameKey + "talk", sendData);
        }

        protected void SendRequestStart(object data)
        {
            ISFSObject sendData = new SFSObject();
            sendData.PutInt("type", 19);
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sendData));
        }


        public void SendVoiceChat(string url, int len, int seat)
        {
            var sfsObj = new SFSObject();
            sfsObj.PutUtfString("url", url);
            sfsObj.PutInt(RequestKey.KeySeat, seat);
            sfsObj.PutInt("len", len);
            SendFrameRequest("sound", sfsObj);
        }
        public void UserTalk(string text)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutUtfString("text", text);
            iobj.PutInt("seat", App.GameData.SelfSeat);
            IRequest request = new ExtensionRequest(GameKey + "talk", iobj);
            SendRequest(request);
        }
        public void UserTalk(int index)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutInt("exp", index);
            iobj.PutInt("seat", App.GameData.SelfSeat);
            IRequest request = new ExtensionRequest(GameKey + "talk", iobj);
            SendRequest(request);
        }
        //投票 解散房间
        public void SendRequestHandsUp(object data)
        {
            int type = (int)data;
            JhUserInfo userInfo = GameData.GetPlayerInfo<JhUserInfo>();
            if (GameData.GStatus > YxEGameStatus.Normal)
            {

                ISFSObject iobj = new SFSObject();
                iobj.PutUtfString("cmd", "dismiss");
                iobj.PutInt(RequestKey.KeyType, type);
                iobj.PutInt(RequestKey.KeyId, int.Parse(userInfo.UserId));
                SendFrameRequest("hup", iobj);
            }
            else
            {
                //局外只有房主才能解散，其他玩家只能退出
                if (int.Parse(userInfo.UserId) == GameData.OwnerId)
                {
                    SendFrameRequest("dissolve", new SFSObject());
                }
                else
                {
                    EventObj.SendEvent("GameManagerEvent", "Quit",null);   
                }
            }
        }
        //客户端发送给服务器的-----------------------------------------------------------------------------
        void OnGameOver(ISFSObject data)
        {
            GameData.GStatus = YxEGameStatus.Over;
            EventObj.SendEvent("TtResultEvent", "TtResult", data);
            EventObj.SendEvent("HupUpViewEvent", "Close", null);
        }

        
        public void OnHandsUp(ISFSObject requestData)
        {
            EventObj.SendEvent("HupUpEvent", "HupUpResponse", requestData);
        }
       
    }
}
