using UnityEngine;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ThrowoutMotion : BaseMotion
    {
        public override HandMotionType MotionType
        {
            get { return HandMotionType.Throwout; }
        }

        private const string mChupai = "chupai";

        public override void OnEnter(FsmStateArgs args)
        {
            mAnimator.SetBool(mChupai, true);
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