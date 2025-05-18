using YxFramwork.Manager;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon
{
    public class MahjongGameManager : YxGameManager
    {
        public NetWorkManager manager;

        protected override void OnAwake()
        {
            base.OnAwake();
            if (manager == null)
            {
                manager = GetComponent<NetWorkManager>();
            }
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            manager.OnServerResponse(response);
        }

        public override void UserOut(int localseat, ISFSObject response)
        {
            int seat = response.GetInt("seat");
            manager.OnUserOut(seat);
        }

        public override void UserIdle(int localseat, ISFSObject response)
        {
            int seat = response.GetInt("seat");
            manager.OnUserIdle(seat);
        }

        public override void UserOnLine(int localseat, ISFSObject response)
        {
            int seat = response.GetInt("seat");
            manager.OnUserOnLine(seat);
        }

        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            manager.OnGetGameInfo(gameInfo);
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
            manager.OnGetRejoinData(gameInfo);
        }

        public override void GameStatus(int status, ISFSObject info)
        {
           
        }

        public override void OnOtherPlayerJoinRoom(ISFSObject data)
        {
            manager.OnUserJoinRoom(data);
        }
    }
}