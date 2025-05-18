using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public abstract class BaseMotion : FsmState
    {
        public abstract HandMotionType MotionType { get; }

        protected Animator mAnimator;

        public override void OnInit(FsmSystem fsm)
        {
            base.OnInit(fsm);
            var motionFsm = fsm as HandMotionFsm;
            mAnimator = motionFsm.Animator;
        }

        public override void OnEnter(FsmStateArgs args) { }

        public override void OnLeave(bool isShutdown) { }

        protected void ChangeIdleState()
        {
            AnimatorStateInfo info = mAnimator.GetCurrentAnimatorStateInfo(0);
            if (info.normalizedTime >= 1.0f)
            {
                mFsm.ChangeState<IdleMotion>();
            }
        }
    }
}
