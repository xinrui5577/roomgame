using Sfs2X.Entities.Data;
using YxFramwork.Manager;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahGameManager : YxGameManager
    {
        //退出
        public event Action<int, ISFSObject> UserOutEvent;
        //准备
        public event Action<int, ISFSObject> UserReadyEvent;
        //切换状态
        public event Action<int, ISFSObject> GameStateEvent;
        //其他玩家加入房间
        public event Action<ISFSObject> OnOtherPlayerJoinRoomEvent;
        //获取游戏数据
        public event Action<ISFSObject> OnGetGameInfoEvent;

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            GameCenter.GameLogic.GameResponse(type, response);
        }

        public override void UserOut(int localseat, ISFSObject response)
        {
            UserOutEvent(localseat, response);
        }

        public override void UserReady(int localseat, ISFSObject response)
        {
            base.UserReady(localseat, response);
            UserReadyEvent(localseat, response);
        }

        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            OnGetGameInfoEvent(gameInfo);
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
            GameCenter.GameLogic.RefreshResponseQueue();
        }

        public override void GameStatus(int status, ISFSObject info)
        {
            GameStateEvent(status, info);
        }

        public override void OnOtherPlayerJoinRoom(ISFSObject data)
        {
            OnOtherPlayerJoinRoomEvent(data);
        }

        public override int OnChangeRoom()
        {
            // 娱乐房，切换行间 清理所有消息 设置标志位
            GameCenter.Instance.YuLeBoutState = true;
            GameCenter.Instance.SetIgonreReconnectState(false);
            GameCenter.GameLogic.RefreshResponseQueue();
            return base.OnChangeRoom();
        }

        public override bool OnShowOutEvent(int type, string message)
        {
            if (GameCenter.DataCenter.Room.RoomType == MahRoomType.YuLe 
                && GameCenter.GameProcess.IsCurrState<StateGameEnd>())
            {
                return type == 3;
            }
            return false;
        }

        //lisi--新增为了把局数增加换到这里--start--
        public override void BeginNewGame(ISFSObject sfsObject)
        {
            base.BeginNewGame(sfsObject);
            GameCenter.GameLogic.GameResponse(NetworkProls.NewGameBegin, sfsObject);
        }
        //lisi--end--
    }
}