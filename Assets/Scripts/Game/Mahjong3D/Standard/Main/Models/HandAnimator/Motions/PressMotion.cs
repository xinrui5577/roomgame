using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class PressMotion : BaseMotion
    {
        public override HandMotionType MotionType
        {
            get { return HandMotionType.Press; }
        }

        private const string mGameStart = "gamestart";

        public override void OnEnter(FsmStateArgs args)
        {
            mAnimator.SetBool(mGameStart, true);
        }

        public override void OnUpdate(float elapseSeconds)
        {
            ChangeIdleState();
        }

        public override void OnLeave(bool isShutdown)
        {
        }
    }
}
