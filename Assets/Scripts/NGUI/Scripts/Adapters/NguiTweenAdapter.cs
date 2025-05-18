using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;

namespace Assets.Scripts.Common.Adapters
{
    public class NguiTweenAdapter : YxBaseAnimationAdapter
    {
        public GameObject Target;

        protected override bool OnPlay(bool needAddEvent = false)
        {
            return PlayTweener(true);
        }

        protected override bool OnPlayback(bool needAddEvent = false)
        {
            return PlayTweener(false);
        }

        public override void ResetTween()
        {
            var tweeners = GetComponents<UITweener>();
            foreach (var tweener in tweeners)
            {
                tweener.ResetToBeginning();
            }
        }

        private bool PlayTweener(bool forward)
        {
            if (Target != null)
            {
                GlobalUtils.ChangeParent(Target.transform, transform);
            }
            var tweeners = GetComponents<UITweener>();
            UITweener lastTweener = null;
            float lastUseTime = 0;
            foreach (var tweener in tweeners)
            {
                tweener.ResetToBeginning();
                tweener.Play(forward);
                var curUseTime = tweener.duration + tweener.delay;
                if (curUseTime > lastUseTime)
                {
                    lastTweener = tweener;
                    lastUseTime = curUseTime;
                }
            }
            if (lastTweener != null && FinishCallBack != null)
            {
                lastTweener.SetOnFinished(CallBack);
                return false;
            }
            return true;
        }
    }
}
