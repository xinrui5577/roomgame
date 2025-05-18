using UnityEngine;
using System.Collections;
using com.yxixia.utile.YxDebug;


namespace Assets.Scripts.Game.fillpit
{
    public class CountDownItem : MonoBehaviour
    {
        /// <summary>
        /// 开始倒计时动画
        /// </summary>
        protected void OnEnable()
        {
            transform.rotation = new Quaternion(0, 0, 0, transform.rotation.w);
            StartCoroutine(DoRotate());
        }

        /// <summary>
        /// 旋转
        /// </summary>
        /// <returns></returns>
        IEnumerator DoRotate()
        {
            float Ag = 18.0f;
            int times = 0;
            float spaceTime = 0.7f / 20;

            while (true)
            {
                if (times++ < 20)
                {
                    gameObject.transform.Rotate(0, 0, -Ag);
                    yield return new WaitForSeconds(spaceTime);
                }
                else
                {
                    times = 0;
                    yield return new WaitForSeconds(0.3f);
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        protected void OnDisable()
        {
            YxDebug.Log("In Disable!!");
            StopCoroutine(DoRotate());
        }
    }
}