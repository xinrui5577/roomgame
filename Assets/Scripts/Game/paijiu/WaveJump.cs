using UnityEngine;
using System.Collections;


namespace Assets.Scripts.Game.paijiu
{
    /// <summary>
    /// 从当前点Y轴移动到制动范围
    /// </summary>
    public class WaveJump : MonoBehaviour
    {
        bool _isPlaying;

        /// <summary>
        /// 移动速度,负数为向下移动
        /// </summary>
        [SerializeField]
        private float _jumpSpeed = 300;

        /// <summary>
        /// 限制,移动范围限制
        /// </summary>
        [SerializeField]
        private float _limit = 50;

        [SerializeField]
        private bool _loop = false;

        private float _loopSpaceTime = 0.5f;

        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once UnusedMember.Local
        void OnEnable()
        {
            if (!_isPlaying)
            {
                _isPlaying = true;

                StartCoroutine(DoJump());
            }
        }

        IEnumerator DoJump()
        {
            Vector3 selfPos = transform.localPosition;
            float posY = selfPos.y;
            float target = selfPos.y + _limit;
            while (posY < target)
            {
                posY += Time.deltaTime * _jumpSpeed;
                if (posY >= target)
                    posY = target;
                transform.localPosition = new Vector3(selfPos.x, posY, selfPos.z);
                yield return null;
            }

            while (posY > selfPos.y)
            {
                posY -= Time.deltaTime * _jumpSpeed;
                if (posY <= selfPos.y)
                    posY = selfPos.y;
                transform.localPosition = new Vector3(selfPos.x, posY, selfPos.z);
                yield return null;
            }
            StopJump();
        }

        public void StopJump()
        {
            StopAllCoroutines();
            _isPlaying = false;
        }



        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once UnusedMember.Local
        void OnDisable()
        {
            StopJump();
            transform.localPosition = Vector3.zero;
        }
    }
}