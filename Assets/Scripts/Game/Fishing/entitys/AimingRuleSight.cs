using UnityEngine;

namespace Assets.Scripts.Game.Fishing.entitys
{
    /// <summary>
    /// 瞄准镜
    /// </summary>
    public class AimingRuleSight : MonoBehaviour
    {

        private Animator _animator;

        private Transform _target;
        private Fish _targetFish;
        private Transform _transform;


        public string AnimationName = "lock";

        public void Init()
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
            if (_transform == null)
            {
                _transform = transform;
            }
            Hide();
        }

        /// <summary>
        /// 锁定目标
        /// </summary>
        public void LockAt(Fish target)
        {
            _targetFish = target;
            if (target == null)
            {
                Hide();
                return;
            }
            _target = target.transform;
            gameObject.SetActive(true);
            if (_animator != null)
            {
                _animator.Play(AnimationName,0,0);
            }
        }

        void Update()
        {
            if (_targetFish == null || !_targetFish.Availability)
            {
                Hide();
                return;
            }
            _transform.position = _target.position;
        }

        public void Hide()
        {
            _target = null;
            _targetFish = null;
            gameObject.SetActive(false);
        }

    }
}
