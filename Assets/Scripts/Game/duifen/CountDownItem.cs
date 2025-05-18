using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.duifen
{
    public class CountDownItem : MonoBehaviour {
    
        // ReSharper disable once UnusedMember.Local
        void OnEnable()
        {
            Debug.Log("In Enable!!");
            transform.rotation = new Quaternion(0,0,0,transform.rotation.w);
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
        }

        // ReSharper disable once UnusedMember.Local
        void OnDisable()
        {
            Debug.Log("In Disable!!");
            StopCoroutine(DoRotate());
        }


    }
}
