namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class GameProcessComponent : BaseComponent
    {
        public FsmSystem GameProcess { get; set; }

        public override void OnInitalization()
        {
            GameProcess = new FsmSystem();
            GameProcess.RegisterFsmState<StateGameEnd>();
            GameProcess.RegisterFsmState<StateGamePlaying>();
            GameProcess.RegisterFsmState<StateGameReady>();
            GameProcess.RegisterFsmState<StateGameReplay>();
            GameProcess.RegisterFsmState<StateGameInfoInit>();
            GameProcess.RegisterFsmState<StateGameContinue>();
            GameProcess.RegisterFsmState<StateGameReconnect>();
            GameProcess.RegisterFsmState<StateGameSceneInit>();
        }

        public void StartupProcess()
        {
            GameProcess.Start<StateGameSceneInit>();
        }

        public void ChangeState<TState>() where TState : FsmState
        {
            GameProcess.ChangeState<TState>();
        }

        public void ChangeState<TState>(FsmStateArgs args) where TState : FsmState
        {
            GameProcess.ChangeState<TState>(typeof(TState).Name, args);
        }

        public bool IsCurrState<TState>() where TState : FsmState
        {
            return GameProcess.IsCurrState<TState>();
        }
    }
}