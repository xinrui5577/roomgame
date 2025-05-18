using UnityEngine;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.AnimationsScritp
{
    public abstract class Anim : MonoBehaviour
    {
        /// <summary>
        /// 开始播放
        /// </summary>
        public abstract void Play();

        /// <summary>
        /// 停止播放
        /// </summary>
        public abstract void Stop();
    }
}
