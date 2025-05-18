using Assets.Scripts.Game.biji.EventII;
using Assets.Scripts.Game.biji.Modle;
using Assets.Scripts.Game.biji.ui;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.biji.network
{
    public class BjGameManager : YxGameManager
    {

        public EventObject EventObj;

        protected YxEventDispatchManager EventMng;

        protected BjGameTable GameData;

        protected override void OnStart()
        {
            base.OnStart();
            EventMng = Facade.EventCenter;
            GameData = App.GetGameData<BjGameTable>();
        }

        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            GameData.FreshRoomShow();
            GameData.FreshUserOwnerIcon();
            GameData.RejoinHupUp();
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {

        }

        protected override string GetStateField()
        {
            return "status";
        }

        public override void GameStatus(int status, ISFSObject info)
        {
            switch (status)
            {
                case BjRejoinTypeKey.TypePutCard:
                    EventObj.SendEvent("TableEvent", "PutCards", null);
                    break;
                case BjRejoinTypeKey.TypeCompareCard:
                    break;
                case BjRejoinTypeKey.TypeWaitForStart:
                    EventObj.SendEvent("TableViewEvent", "Ready", null);
                    break;
            }
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            switch (type)
            {
                case BjTypeKey.TypeToStart:
                    GameData.ToStartGame();
                    break;
                case BjTypeKey.TypeSendCard:
                    GameData.AllocateCards(response);
                    App.GameData.GStatus = YxEGameStatus.PlayAndConfine;
                    break;
                case BjTypeKey.TypeCompare:
                    GameData.CompareCards(response);
                    App.GameData.GStatus = YxEGameStatus.Over;
                    break;
                case BjTypeKey.TypePutCard:
                    GameData.PutCards(response);
                    break;
                case BjTypeKey.TypeResult:
                    GameData.Result(response);
                    break;
                case BjTypeKey.TypeGiveUp:
                    GameData.GiveUp(response);
                    break;
                case BjTypeKey.TypeError:
                    GameData.Error(response);
                    break;
            }
        }

        public override void BeginReady()
        {
            base.BeginReady();
            EventObj.SendEvent("ResultViewEvent", "Ready", null);
        }

        public override void BeginNewGame(ISFSObject sfsObject)
        {
            if (GameData.IsCreatRoom)
            {
                GameData.CreateRoomInfo.AddRound();
                EventObj.SendEvent("TableViewEvent", "FreshCurRound", GameData.CreateRoomInfo);
            }

            var userDic = GameData.UserInfoDict;
            foreach (var user in userDic)
            {
                var player = (BjUserInfo)user.Value;
                GameData.GetPlayer<BjPlayer>(player.Seat, true).ReadyState = false;

                GameData.GetPlayer<BjPlayer>(player.Seat, true).CallStartSettleCards();

                GameData.GetPlayer<BjPlayer>(player.Seat, true).Reset();
            }

        }

        public override void OnOtherPlayerJoinRoom(ISFSObject sfsObject)
        {
            base.OnOtherPlayerJoinRoom(sfsObject);
        }

        public override void UserReady(int localSeat, ISFSObject responseData)
        {
            GameData.GetPlayer<BjPlayer>(localSeat).ReadyState = true;
        }

        public override void UserOnLine(int localSeat, ISFSObject responseData)
        {
            base.UserIdle(localSeat, responseData);
        }

        public void OnRecive(EventData data)
        {
            switch (data.Name)
            {
                case "Quit":
                    OnQuitGameClick();
                    break;
                case "ChangeRoom":
                    break;
            }
        }
    }
}
