namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class GestureMotion : BaseMotion
    {
        public override HandMotionType MotionType
        {
            get { return HandMotionType.Gesture; }
        }

        public override void OnEnter(FsmStateArgs args)
        {
        }

        public override void OnUpdate(float elapseSeconds)
        {

        }

        public override void OnLeave(bool isShutdown)
        {
            mFsm.ChangeState<IdleMotion>();
        }
    }
}
