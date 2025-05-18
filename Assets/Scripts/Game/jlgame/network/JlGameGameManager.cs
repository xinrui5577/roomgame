using Assets.Scripts.Game.jlgame.EventII;
using Assets.Scripts.Game.jlgame.Modle;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jlgame.network
{
    public class JlGameGameManager : YxGameManager
    {

        public EventObject EventObj;

        protected YxEventDispatchManager EventMng;

        protected JlGameGameTable GameData;

        protected override void OnStart()
        {
            base.OnStart();
            EventMng = Facade.EventCenter;
            GameData = App.GetGameData<JlGameGameTable>();
        }

        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            GameData.FreshCreatRoomShow();
            GameData.FreshTableCard();
            GameData.RejoinHupUp();
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {

        }

        public override void GameStatus(int status, ISFSObject info)
        {

        }

        public override void OnChangeRoomClick()
        {
            EventObj.SendEvent("TableViewEvent", "Reset", null);
            EventObj.SendEvent("TableViewEvent", "Ready", null);
            base.OnChangeRoomClick();
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            switch (type)
            {
                case JlGameTypeKey.TypeAllocate:
                    App.GameData.GStatus = YxEGameStatus.PlayAndConfine;
                    GameData.OnFaPai(response);
                    break;
                case JlGameTypeKey.TypeSpeaker:
                    App.GameData.GStatus = YxEGameStatus.PlayAndConfine;
                    GameData.OnWhoSpeak(response);
                    break;
                case JlGameTypeKey.TypeOutCard:
                    App.GameData.GStatus = YxEGameStatus.PlayAndConfine;
                    GameData.OnOutCard(response);
                    break;
                case JlGameTypeKey.TypeFoldCard:
                    App.GameData.GStatus = YxEGameStatus.PlayAndConfine;
                    GameData.OnFoldCard(response);
                    break;
                case JlGameTypeKey.TypeResult:
                    App.GameData.GStatus = YxEGameStatus.Over;
                    GameData.OnResult(response);
                    break;
                case JlGameTypeKey.TypeTrusteeship:
                    GameData.OnTrusteeship(response);
                    break;

            }
        }


        public override void BeginNewGame(ISFSObject sfsObject)
        {
            base.BeginNewGame(sfsObject);
            if (GameData.IsCreatRoom)
            {
                GameData.CreateRoomInfo.AddRound();
                EventObj.SendEvent("TableViewEvent", "FreshCurRound", GameData.CreateRoomInfo);
            }

            for (int i = 0; i < GameData.UserInfoDict.Count; i++)
            {
                GameData.GetPlayer(i).ReadyState = false;
            }

        }

        public override void BeginReady()
        {
            base.BeginReady();
            EventObj.SendEvent("TableViewEvent", "Ready", null);
        }

        public override void OnOtherPlayerJoinRoom(ISFSObject sfsObject)
        {
            base.OnOtherPlayerJoinRoom(sfsObject);
        }

        public override void UserReady(int localSeat, ISFSObject responseData)
        {
            base.UserReady(localSeat, responseData);
            GameData.GetPlayer(localSeat).ReadyState = true;
        }


        public override void UserIdle(int localSeat, ISFSObject responseData)
        {
            base.UserIdle(localSeat, responseData);
        }

        public override void UserOnLine(int localSeat, ISFSObject responseData)
        {
            base.UserIdle(localSeat, responseData);
        }

        public void OnRecive(EventData data)
        {
            switch (data.Name)
            {
                case "ChangeRoom":
                    break;
            }
        }
    }
}
