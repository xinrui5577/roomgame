using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.jlgame.EventII;
using Assets.Scripts.Game.jlgame.Modle;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jlgame.network
{
    public class JlGameGameServer : YxGameServer
    {
        public EventObject EventObj;

        protected JlGameGameTable GameData;
        public override void Init(Dictionary<string, Action<ISFSObject>> callBackDic)
        {
            GameData = App.GetGameData<JlGameGameTable>();


            callBackDic["hup"] = OnHandsUp;
            callBackDic[GameKey + "over"] = OnGameOver;
        }

        public void OnRecevie(EventData data)
        {
            switch (data.Name)
            {
                case "ReadyReq":
                    SendRequestReady(data);
                    break;
                case "OutCard":
                    SendRequestOutCard(data);
                    break;
                case "FoldCard":
                    SendRequestFoldCard(data);
                    break;
                case "Trusteeship":
                    SendRequestTrusteeship(data);
                    break;
                case "HupReq":
                    SendRequestHandsUp(data.Data);
                    break;
                case "GameDetail":
                    RequestGameDetail();
                    break;
                case "ChangeRoom":
                    SendRequestChangeRoom(data);
                    break;
            }
        }

        //客户端发送给服务器的-----------------------------------------------------------------------------
        //准备
        protected void SendRequestReady(object data)
        {
            ISFSObject isfsSata = new SFSObject();
            isfsSata.PutInt(RequestKey.KeySeat, App.GameData.SelfSeat);
            SendFrameRequest(GameKey + "ready", isfsSata);
        }

        public void SendClientRequestSpeaker()
        {
            ISFSObject isfsSata = new SFSObject();
            isfsSata.PutInt("type", 50);
            SendGameRequest(isfsSata);
        }
        //出牌
        protected void SendRequestOutCard(object data)
        {
            var cardData = (EventData) data;
            ISFSObject isfsSata = new SFSObject();
            isfsSata.PutInt("card", (int)cardData.Data);
            isfsSata.PutInt("type", 12);
            SendGameRequest(isfsSata);
        }

        protected void SendRequestFoldCard(object data)
        {
            var cardData = (EventData)data;
            ISFSObject isfsSata = new SFSObject();
            isfsSata.PutInt("card", (int)cardData.Data);
            isfsSata.PutInt("type", 13);
            SendGameRequest(isfsSata);
        }

        protected void SendRequestTrusteeship(object data)
        {
            ISFSObject isfsSata = new SFSObject();
            isfsSata.PutInt("type", 15);
            SendGameRequest(isfsSata);
        }

        //换房
        protected void SendRequestChangeRoom(object data)
        {
            ChangeRoom();
        }

        protected void SendRequestStart(object data)
        {
            ISFSObject sendData = new SFSObject();
            sendData.PutInt("type", 19);
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sendData));
        }

        //投票 解散房间 2发起 3同意 -1拒绝
        public void SendRequestHandsUp(object data)
        {
            int type = (int)data;
            JlGameUserInfo userInfo = GameData.GetPlayerInfo<JlGameUserInfo>();
            if (GameData.CreateRoomInfo.CurRound > 0)
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
            List<TtResultUserData> userTotalData = new List<TtResultUserData>();

            var users = data.GetSFSArray("users");
            var ownerId = data.GetInt("ownerId");
            var winner = data.GetInt("winner");
            for (int i = 0; i < users.Count; i++)
            {
                TtResultUserData ttResultUserData = new TtResultUserData(users.GetSFSObject(i));
                if (ownerId == ttResultUserData.UserId)
                {
                    ttResultUserData.IsRoomOwner = true;
                }

                if (winner == ttResultUserData.UserId)
                {
                    ttResultUserData.IsWinner = true;
                }
                userTotalData.Add(ttResultUserData);
            }
            EventObj.SendEvent("TtResultViewEvent", "UserData", userTotalData);
            RequestGameDetail();
        }

        public void RequestGameDetail()
        {
            var dic = new Dictionary<string, object>();
            dic["game_key_c"] = App.GameKey;
            dic["room_id"] = GameData.CreateRoomInfo.RoomId;
            Facade.Instance<TwManager>().SendAction("gameDetail", dic, ParseGameDetail);
        }

        private void ParseGameDetail(object data)
        {
            EventObj.SendEvent("HupUpViewEvent", "Close", null);
            EventObj.SendEvent("TtResultViewEvent", "TtResult", data);
        }

        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                ISFSObject dd=new SFSObject(); 
                OnGameOver(dd);
            }
        }
        
        public void OnHandsUp(ISFSObject requestData)
        {
            var hupData = new HupData(requestData);
            EventObj.SendEvent("HupUpViewEvent", "PlayerList", GameData.PlayerList);
            hupData.IsSelf = hupData.UserId == int.Parse(GameData.GetPlayerInfo().UserId);
            hupData.CdTime = GameData.CdTime;
            EventObj.SendEvent("HupUpViewEvent", "HupUpResponse", hupData);
        }
       
    }
}
