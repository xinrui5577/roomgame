namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MopaiMotion : BaseMotion
    {
        public override HandMotionType MotionType
        {
            get { return HandMotionType.Mopai; }
        }

        private const string mMopai = "mopai";

        public override void OnEnter(FsmStateArgs args)
        {
            mAnimator.SetBool(mMopai, true);
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
