using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.biji.EventII;
using Assets.Scripts.Game.biji.Modle;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;

namespace Assets.Scripts.Game.biji.network
{
    public class BjGameServer : YxGameServer
    {
        public EventObject EventObj;

        protected BjGameTable GameData;
        public override void Init(Dictionary<string, Action<ISFSObject>> callBackDic)
        {
            GameData = App.GetGameData<BjGameTable>();
            callBackDic["hup"] = OnHandsUp;
            callBackDic[GameKey + "over"] = OnGameOver;
        }

        public void OnRecevie(EventData data)
        {
            switch (data.Name)
            {
                case "StartGame":
                    SendRequestStartGame();
                    break;
                case "ReadyReq":
                    SendRequestReady(data);
                    break;
                case "HupReq":
                    SendRequestHandsUp(data.Data);
                    break;
                case "SendCards":
                    OnOutCards(data.Data);
                    break;
                case "GiveUp":
                    OnGiveUp();
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

        //换房
        protected void SendRequestChangeRoom(object data)
        {
            ChangeRoom();
        }

        //投票 解散房间 2发起 3同意 -1拒绝
        public void SendRequestHandsUp(object data)
        {
            int type = (int)data;
            BjUserInfo userInfo = GameData.GetPlayerInfo<BjUserInfo>();
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
                    EventObj.SendEvent("GameManagerEvent", "Quit", null);
                }
            }
        }
        //客户端发送给服务器的-----------------------------------------------------------------------------
        void OnGameOver(ISFSObject data)
        {
            StartCoroutine(WaitGameOver(data));
        }

        IEnumerator WaitGameOver(ISFSObject data)
        {
            yield return new WaitForSeconds(3);
            EventObj.SendEvent("ResultViewEvent", "Close", null);
            EventObj.SendEvent("HupUpViewEvent", "Close", null);
            EventObj.SendEvent("SelectCardsViewEvent", "Close", null);
            List<TtResultUserData> userTotalData = new List<TtResultUserData>();

            var users = data.GetSFSArray("users");
            var ownerId = data.GetInt("ownerId");
            var winner = data.GetInt("winner");
            var svt = data.GetLong("svt");

            ISFSObject iobj = new SFSObject();
            iobj.PutInt("roomId", GameData.CreateRoomInfo.RoomId);
            iobj.PutLong("startTime", svt);
            iobj.PutUtfString("gameKey", App.GameKey);
            EventObj.SendEvent("TtResultViewEvent", "RoomData", iobj);
            for (int i = 0; i < users.Count; i++)
            {
                TtResultUserData ttResultUserData = new TtResultUserData(users.GetSFSObject(i));
                if (ttResultUserData.UserId == -1) continue;
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

            EventObj.SendEvent("TtResultViewEvent", "Show", userTotalData);
        }

        public void OnHandsUp(ISFSObject requestData)
        {
            var hupData = new HupData(requestData);
            hupData.IsSelf = hupData.UserId == int.Parse(GameData.GetPlayerInfo().UserId);
            hupData.CdTime = GameData.CdTime;
            hupData.IsRoomOwner = GameData.OwnerId == hupData.UserId;
            EventObj.SendEvent("HupUpViewEvent", "HupUpResponse", hupData);
        }


        public void OnOutCards(object data)
        {
            var cardData = (ISFSObject)data;
            SendGameRequest(cardData);
        }

        public void OnGiveUp()
        {
            ISFSObject sendData = new SFSObject();
            sendData.PutInt("type", 7);
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sendData));
        }

        public void SendRequestStartGame()
        {
            ISFSObject sendData = new SFSObject();
            sendData.PutInt("type", 1);
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sendData));
        }
    }
}
