namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class StateGameReplay : FsmState
    {
        public override void OnEnter(FsmStateArgs args)
        {
            GameCenter.Replay.InitReplayData();
            GameCenter.Replay.StartupTask();
            GameCenter.Lifecycle.ContinueReplayCycle();
            GameCenter.Replay.OnReset();
            GameCenter.Replay.InitReplayScene();
            GameCenter.EventHandle.Subscriber((int)EventKeys.ReplayRestart, ReplayRestart);
        }

        public void ReplayRestart(EvtHandlerArgs args)
        {
            GameCenter.Scene.ReplayRestart();
            GameCenter.Replay.ReplayRestart();
            GameCenter.Replay.ReplayData.OnResetFrameDatas();
        }

        public override void OnLeave(bool isShutdown) { }
    }
}