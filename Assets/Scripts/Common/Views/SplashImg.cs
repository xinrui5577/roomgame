using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using YxFramwork.Common.Adapters;

namespace Assets.Scripts.Common.Views
{
    /// <summary>
    /// 启动闪屏
    /// </summary>
    public class SplashImg : MonoBehaviour
    {
        /// <summary>
        /// 等待时间
        /// </summary>
        public float WaitTime = 1.5f;
        /// <summary>
        /// 前景
        /// </summary>
        public YxBaseTextureAdapter Foreground;
        /// <summary>
        /// 背景
        /// </summary>
        public YxBaseTextureAdapter Background;

        void Awake()
        {
            if (Foreground != null)
            {
                Foreground.MakePixelPerfect();
            }
        }

        IEnumerator Start ()
        {
            var wait = new WaitForSeconds(WaitTime);
            yield return wait;
            SceneManager.LoadScene(1);
        }
    }
}
