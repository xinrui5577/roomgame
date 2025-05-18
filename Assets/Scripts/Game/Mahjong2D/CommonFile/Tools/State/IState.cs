using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;

namespace Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.State
{
    public interface IState
    {
        float DuraTime { get; set; }

        void Eneter();

        void Excute();

        void Exit();

        void Update();

        IState NextState { get; set; }

        bool ExcuteState { get; set; }

        bool ReadyTochange { get; set; }

        EnumGameState StateType { get; set; }

    }
}
