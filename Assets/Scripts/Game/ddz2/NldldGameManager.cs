using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.DDzGameListener.InfoPanel;
using Assets.Scripts.Game.ddz2.InheritCommon;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.ddz2
{
    public class NldldGameManager : YxGameManager
    {

        public void PlayOnesSound(string audioName, int seat)
        {
            var sex = App.GetGameData<DdzGameData>().GetOnePlayerInfo(seat, true).SexI;
            var source = sex == 0 ? "woman" : "man";
            Facade.Instance<MusicManager>().Play(audioName, source);
        }

        public override void UserReady(int localSeat, ISFSObject responseData)
        {
            base.UserReady(localSeat, responseData);
            var eventArgs = new DdzbaseEventArgs(responseData);
            Facade.EventCenter.DispatchEvent(GlobalConstKey.KeyOnUserReady, eventArgs);
        }

        public override void BeginNewGame(ISFSObject sfsObject)
        {
            base.BeginNewGame(sfsObject);
            App.GetGameData<DdzGameData>().CurrentRound++;
            Facade.EventCenter.DispatchEvent<string, DdzbaseEventArgs>(GlobalConstKey.KeyOnBeginNewGame);
        }


        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            var eventArgs = new DdzbaseEventArgs(gameInfo);
            Facade.EventCenter.DispatchEvent(GlobalConstKey.KeyGetGameInfo, eventArgs);
            App.GetGameData<DdzGameData>().FinishRoomInfo = true;
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
            var eventArgs = new DdzbaseEventArgs(gameInfo);

            Facade.EventCenter.DispatchEvent(GlobalConstKey.KeyOnRejoin, eventArgs);
        }

        public override int OnChangeRoom()
        {
            App.GetGameData<DdzGameData>().FinishRoomInfo = false;
            return base.OnChangeRoom();
        }

        public override void GameStatus(int status, ISFSObject info)
        {
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            var eventArgs = new DdzbaseEventArgs(response);

            Facade.EventCenter.DispatchEvent(type, eventArgs);
        }

        public override void UserOut(int localSeat, ISFSObject responseData)
        {
            base.UserOut(localSeat, responseData);

            DdzbaseEventArgs args = new DdzbaseEventArgs(responseData);
            Facade.EventCenter.DispatchEvent(GlobalConstKey.KeyOnUserOut, args);
        }


        public override void UserIdle(int localSeat, ISFSObject responseData)
        {
            base.UserIdle(localSeat, responseData);

            App.GetGameData<DdzGameData>().GetPlayer<DdzPlayer>(localSeat).UserIdle();
        }

        public override void UserOnLine(int localSeat, ISFSObject responseData)
        {
            base.UserOnLine(localSeat, responseData);

            App.GetGameData<DdzGameData>().GetPlayer<DdzPlayer>(localSeat).UserOnLine();
        }

        public override void OnOtherPlayerJoinRoom(ISFSObject responseData)
        {
            ISFSObject user = responseData.GetSFSObject(RequestKey.KeyUser);
            int seat = user.GetInt(RequestKey.KeySeat);
            var gdata = App.GetGameData<DdzGameData>();
            //如果游戏开始,玩家属于重连,不用刷新数据
            if (gdata.IsGameStart)
            {
                var player = gdata.GetPlayer<DdzPlayer>(seat, true);
                player.UserJoinRoom();
                return;
            }
            base.OnOtherPlayerJoinRoom(responseData);
        }

        public override void BeginReady()
        {
            base.BeginReady();
            Facade.EventCenter.DispatchEvent<string, DdzbaseEventArgs>(GlobalConstKey.AllowReady);
        }



        public override void OnDestroy()
        {
            base.OnDestroy();
            Facade.EventCenter.RemoveAllEventListener();
            Destroy(this);
        }
    }
}
