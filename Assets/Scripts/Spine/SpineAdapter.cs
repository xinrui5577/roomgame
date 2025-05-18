using DG.Tweening;
using Spine;
using Spine.Unity;
using YxFramwork.Common.Adapters;

namespace Assets.Scripts.Spine
{
    public class SpineAdapter : YxBaseAnimationAdapter
    {
        private SkeletonAnimation _skAnimation;

         
        public SkeletonAnimation SkAnimation {
            get
            {
                if (_skAnimation == null)
                {
                    _skAnimation = GetComponent<SkeletonAnimation>();
                }
                return _skAnimation;
            }
        }

        void Start()
        {
        }

        protected override bool OnPlay(bool needAddEvent = false)
        {
            var skAnimation = SkAnimation;
            if (skAnimation == null) return true;
            skAnimation.DOPlayForward();
            return needAddEvent;
        }

        protected override bool OnPlayback(bool needAddEvent = false)
        {
            var skAnimation = SkAnimation;
            if (skAnimation == null) return true;
            skAnimation.DOPlayForward();
            return needAddEvent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoAnimationName"></param>
        /// <param name="needAddEvent"></param>
        /// <returns></returns>
        protected override bool OnPlayByName(string infoAnimationName, bool needAddEvent = false)
        {
            var skAnimation = SkAnimation;
            if (skAnimation == null) return true;
            skAnimation.AnimationState.SetAnimation(0, infoAnimationName, false);
            if (needAddEvent)
            {
                SkAnimation.AnimationState.Complete -= CompleteEvent;
                SkAnimation.AnimationState.Complete += CompleteEvent;
                return false;
            }
            return true;
        }

        public override void ResetTween()
        {
        }


        public void CompleteEvent(TrackEntry trankEntry)
        {
            SkAnimation.AnimationState.Complete -= CompleteEvent;
            CallBack();
            if (SkAnimation == null) return; 
            SkAnimation.AnimationState.SetAnimation(0, DefaultAnimationName, true);
        }
    }
}
