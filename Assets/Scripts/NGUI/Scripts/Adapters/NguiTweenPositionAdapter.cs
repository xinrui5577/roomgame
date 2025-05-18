using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;

namespace Assets.Scripts.Common.Adapters
{
    public class NguiTweenPositionAdapter : YxBaseAnimationAdapter
    {
        public GameObject Target;
        public bool IsRelativePosition;
        public UIWidget RelativeWidget;
        private TweenPosition _htetweePosition;
        public TweenPosition TheTweenPosition
        {
            get {
                // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
                if (_htetweePosition == null)
                {
                    _htetweePosition = GetComponent<TweenPosition>();
                }
                return _htetweePosition;
            }
        }

        public override void ResetTween()
        {
            var tweener = TheTweenPosition;
            if (tweener == null) return;
            tweener.ResetToBeginning();
        }

        protected void Awake()
        {
            InitPosition();
        }

        protected override bool OnPlay(bool needAddEvent = false)
        {
            return PlayTweener(true);
        }

        private bool _canPlay;
        private bool _isStart;
        protected void Start()
        {
            _isStart = true;
            if (_canPlay)
            {
                PlayTweener(true);
            }
        }

        protected override bool OnPlayback(bool needAddEvent = false)
        {
            return PlayTweener(false);
        }

        private bool PlayTweener(bool forward)
        {
            _canPlay = true;
            if (!_isStart)
            {
                return FinishCallBack == null;
            }
            if (Target != null)
            {
                GlobalUtils.ChangeParent(Target.transform, transform);
            }
            var tweener = TheTweenPosition;
            tweener.Play(forward);
            if (FinishCallBack != null)
            {
                tweener.SetOnFinished(CallBack);
                return false;
            }
            return true;
        }

        private void InitPosition()
        {
            var tweener = TheTweenPosition;
            var from = tweener.from;
            if (IsRelativePosition)
            {
                var w = RelativeWidget.width;
                var h = RelativeWidget.width;
                from.x *= w;
                from.y *= h;
                tweener.from = from;
                var to = tweener.to;
                to.x *= w;
                to.y *= h;
                tweener.to = to;
            }
            transform.localPosition = from;
        }
    }
}
