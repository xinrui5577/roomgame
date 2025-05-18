namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class IdleMotion : BaseMotion
    {
        public override HandMotionType MotionType
        {
            get { return HandMotionType.Idle; }
        }

        public override void OnEnter(FsmStateArgs args)
        {
        }

        public override void OnLeave(bool isShutdown)
        {
        }
    }
}
