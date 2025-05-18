using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ActionScoreDouble : AbsCommandAction
    {
        //加漂
        public void ShowBottompourAction(ISFSObject data)
        {
            //游戏开始
            GameCenter.GameProcess.ChangeState<StateGamePlaying>();
            //显示UI
            GameCenter.EventHandle.Dispatch((int)EventKeys.SetPlayerFlagState, new PlayerStateFlagArgs()
            {
                CtrlState = true,
                StateFlagType = (int)PlayerStateFlagType.Selecting
            });
            GameCenter.EventHandle.Dispatch((int)EventKeys.ScoreDoubleCtrl);
        }

        //显示加漂分数
        public void ShowScoreDoubleAction(ISFSObject data)
        {
            var array = data.GetIntArray("piaolist");
            var newArray = new int[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                var chair = MahjongUtility.GetChair(i);
                newArray[chair] = array[i];
            }
            ScoreDoubleArgs args = new ScoreDoubleArgs()
            {
                ScoreDoubleArray = newArray
            };
            var eventHandler = GameCenter.EventHandle;
            eventHandler.Dispatch((int)EventKeys.ScoreDoubleCtrl, args);
            eventHandler.Dispatch((int)EventKeys.SetPlayerFlagState, new PlayerStateFlagArgs() { CtrlState = false });
        }
    }
}
