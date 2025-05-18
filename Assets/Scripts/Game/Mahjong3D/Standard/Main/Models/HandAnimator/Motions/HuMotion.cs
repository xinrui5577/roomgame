namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class HuMotion : BaseMotion
    {
        public override HandMotionType MotionType
        {
            get { return HandMotionType.Hu; }
        }

        public override void OnEnter(FsmStateArgs args)
        {

        }

        public override void OnLeave(bool isShutdown)
        {
            mFsm.ChangeState<IdleMotion>();
        }
    }
}
