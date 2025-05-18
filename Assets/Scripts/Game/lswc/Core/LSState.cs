using System;

namespace Assets.Scripts.Game.lswc.Core
{
    /// <summary>
    /// 控制游戏中的各个阶段
    /// </summary>
    public class LSState
    {

        /// <summary>
        /// 阶段等待时间
        /// </summary>
        protected float _duraTime;

        public bool ExcuteState = true;

        public bool UpdateState;

        public Action OnStateFinished;

        public LSState NextState;

        public virtual void Enter()
        {
            ExcuteState = false;
        }

        public virtual void Excute()
        {
            //YxDebug.Log("<color=red>EXCUTE STATE IS</color>" + "<color=green>" + this + "</color>");
            ExcuteState = true;
        }

        public virtual void Exit()
        {
            ExcuteState = false;
            UpdateState = false;
        }

        public virtual void Update()
        {
        }

        public float DuraTime
        {
            set {  _duraTime=value; }
        }

    }
}
