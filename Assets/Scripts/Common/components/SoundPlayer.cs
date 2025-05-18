using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Common.components
{
    /// <summary>
    /// 播放声音
    /// </summary>
    public class SoundPlayer : MonoBehaviour
    {
        /// <summary>
        /// 声音名称
        /// </summary>
        public string SoundName;
        public string Source = "AudioSources";

        public float Delay;

        // Use this for initialization
        void OnEnable()
        {
            if (Delay > 0)
            {
                Invoke("PlaySound",Delay);
                return;
            }
            PlaySound();
        }


        public void PlaySound()
        {
            Facade.Instance<MusicManager>().Play(SoundName, Source);
        }
    }
}
