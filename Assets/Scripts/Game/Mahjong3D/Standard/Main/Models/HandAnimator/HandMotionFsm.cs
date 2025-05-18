using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class HandMotionFsm : FsmSystem
    {
        public Animator Animator { get; set; }

        public void OnInit(Animator ani)
        {
            Animator = ani;

            RegisterFsmState<HuMotion>();
            RegisterFsmState<IdleMotion>();
            RegisterFsmState<PressMotion>();
            RegisterFsmState<MopaiMotion>();
            RegisterFsmState<GetCardMotion>();
            RegisterFsmState<GestureMotion>();
            RegisterFsmState<ThrowoutMotion>();
            Start<IdleMotion>();
        }
    }
}
