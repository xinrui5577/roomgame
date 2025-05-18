namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class StateGameEnd : FsmState
    {
        public override void OnEnter(FsmStateArgs args)
        {
            //执行继承IGameEndCycle接口脚本
            GameCenter.Lifecycle.GameEndCycle();
            if (GameCenter.DataCenter.ConfigData.ContinueNewGame && GameCenter.DataCenter.IsGameOver)
            {
                //切换继续开局状态
                ChangeState<StateGameContinue>();
            }
        }

        public override void OnLeave(bool isShutdown) { }
    }
}