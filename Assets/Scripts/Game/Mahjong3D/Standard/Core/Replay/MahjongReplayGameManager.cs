using YxFramwork.Manager;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongReplayGameManager : ReplayGameManager
    {
        protected override void GameResponseStatus(object info) { }

        protected override void OnInitGame()
        {
            GetComponent<MahjongReplayGameData>().AnalysisFrameData();
            GameCenter.GameProcess.ChangeState<StateGameReplay>();
        }
    }
}