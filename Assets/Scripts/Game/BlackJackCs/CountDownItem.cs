using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.BlackJackCs
{
    public class CountDownItem : MonoBehaviour
    {
        private bool _rotating;
        protected void OnEnable()
        {
            transform.rotation = new Quaternion(0, 0, 0, transform.rotation.w);
            if (!_rotating)
            {
                _rotating = true;
                StartCoroutine(DoRotate());
            }
        }

        /// <summary>
        /// 旋转
        /// </summary>
        /// <returns></returns>
        IEnumerator DoRotate()
        {
            float ag = 18.0f;
            int times = 0;
            float spaceTime = 0.7f / 20;

            while (_rotating)
            {
                if (times++ < 20)
                {
                    gameObject.transform.Rotate(0, 0, -ag);
                    yield return new WaitForSeconds(spaceTime);
                }
                else
                {
                    times = 0;
                    yield return new WaitForSeconds(0.3f);
                }
            }
        }

        protected void OnDisable()
        {
            _rotating = false;
            StopAllCoroutines();
        }


    }
}