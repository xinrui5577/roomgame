using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    //lisi--新增游戏开始回调--start--
    public class ActionNewGameBegin : AbsCommandAction
    {
        public void NewGameBeginAction(ISFSObject data)
        {
            if (DataCenter.Room.RoomType == MahRoomType.FanKa)
            {
                DataCenter.Room.RealityRound++;
                if (DataCenter.Room.LoopType == MahGameLoopType.Round)
                {
                    DataCenter.Room.CurrRound++;
                }
            }
        }
    }
    //lisi--end--
}