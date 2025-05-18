using UnityEngine;
using YxFramwork.Common.Adapters;

namespace Assets.Scripts.Common.Adapters
{
    public class AnimationAdapter : YxBaseAnimationAdapter
    {
        private Animation _animation;
        public Animation TheAnimation
        {
            get { return _animation == null ? _animation = GetComponent<Animation>() : _animation; }
        }

        protected override bool OnPlay(bool needAddEvent = false)
        {
            TheAnimation.Play();
            return !needAddEvent;//有需求再改
        }

        protected override bool OnPlayback(bool needAddEvent = false)
        {
            if (!string.IsNullOrEmpty(DefaultAnimationName))
            {
                var aState = TheAnimation[DefaultAnimationName];
                aState.time = aState.clip.length;
                aState.speed = -1;
                TheAnimation.Play(DefaultAnimationName);
            }
            return !needAddEvent;
        }

        public override void ResetTween()
        {
        }

        protected override bool OnPlayByName(string infoAnimationName, bool needAddEvent = false)
        {
            TheAnimation.Play(infoAnimationName);
            return !needAddEvent;//有需求再改
        }
    }
}
