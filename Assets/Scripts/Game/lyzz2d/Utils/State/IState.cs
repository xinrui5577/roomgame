namespace Assets.Scripts.Game.lyzz2d.Utils.State
{
    public interface IState
    {
        float DuraTime { get; set; }

        IState NextState { get; set; }

        bool ExcuteState { get; set; }

        bool ReadyTochange { get; set; }

        EnumGameState StateType { get; set; }

        void Eneter();

        void Excute();

        void Exit();

        void Update();
    }
}