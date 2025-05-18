using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class GetCardMotion : BaseMotion
    {
        public override HandMotionType MotionType
        {
            get { return HandMotionType.GetCard; }
        }

        private const string mGetCard = "getcard";
        private const string mFangpai = "fangpai";

        private float mTimer = 0;
        private bool mFlag;

        public override void OnEnter(FsmStateArgs args)
        {
            mAnimator.SetBool(mGetCard, true);
        }

        public override void OnUpdate(float elapseSeconds)
        {
            mTimer += elapseSeconds;
            if (mTimer > 10 && !mFlag)
            {
                mFlag = true;
                mAnimator.SetBool(mFangpai, true);
            }
        }

        public override void OnLeave(bool isShutdown)
        {
            mFsm.ChangeState<IdleMotion>();
        }
    }
}
