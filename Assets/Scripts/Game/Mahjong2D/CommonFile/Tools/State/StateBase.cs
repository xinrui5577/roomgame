using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;

namespace Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.State
{
    public class StateBase<State> : Singleton<State>, IState where State : class
    {
        private float _duraTime;

        private IState _nextState;

        private EnumGameState _cureType;

        public virtual void Eneter()
        {
            ExcuteState = true;
            ReadyTochange = false;
        }

        public virtual void Excute()
        {

        }

        public virtual void Exit()
        {
            ExcuteState = false;
            ReadyTochange = false;
        }

        public void Update()
        {

        }

        public float DuraTime { get; set; }
        public IState NextState
        {
            get { return _nextState; }
            set
            {
                //StateMachine.Instance.StateExist(value);
            }
        }
        public bool ExcuteState { get; set; }
        public bool ReadyTochange { get; set; }
        public EnumGameState StateType
        {
            get { return _cureType; }
            set { _cureType = value; }
        }
    }
}
