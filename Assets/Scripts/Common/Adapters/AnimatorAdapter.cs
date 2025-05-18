using System;
using UnityEngine;
using YxFramwork.Common.Adapters;

namespace Assets.Scripts.Common.Adapters
{

    public class AnimatorAdapter : YxBaseAnimationAdapter
    {
        private Animator _animator;

        public float Speed = 1;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private string _curAnimationName;
        private void Start()
        {
            if (!string.IsNullOrEmpty(_curAnimationName))
            {
                PlayByName(_curAnimationName);
            }
        }

        protected override bool OnPlay(bool needAddEvent = false)
        {
            if (_animator != null)
            {
                var flag = needAddEvent && !AddAnimationEvent(DefaultAnimationName);
                _animator.Play(DefaultAnimationName);
                _animator.speed = Speed;
                return flag;
            }
            return true;
        }

        protected override bool OnPlayback(bool needAddEvent = false)
        {
            if (_animator != null)
            {
                var flag = needAddEvent && !AddAnimationEvent(DefaultAnimationName);
                _animator.Play(DefaultAnimationName);
                _animator.speed = -Speed;
                return flag;
            }
            return true;
        }

        public override void ResetTween()
        {
        }

        protected override bool OnPlayByName(string infoAnimationName, bool needAddEvent = false)
        {
            if (_animator == null)
            {
                _curAnimationName = infoAnimationName;
                return false;
            }

            var flag = needAddEvent && !AddAnimationEvent(infoAnimationName);
            _animator.Play(infoAnimationName);
            _curAnimationName = null;
            return flag;
        }

        private bool AddAnimationEvent(string infoAnimationName)
        {
            var arr = _animator.runtimeAnimatorController.animationClips;
            foreach (var clip in arr)
            {
                if (infoAnimationName.Equals(clip.name))
                {
                    var events = new AnimationEvent
                    {
                        functionName = "CallBack",
                        time = clip.length
                    };
                    //添加第一个事件  带参数
                    clip.AddEvent(events);
                    _animator.Rebind();
                    return true;
                }
            }
            return false;
        }
    }
}
