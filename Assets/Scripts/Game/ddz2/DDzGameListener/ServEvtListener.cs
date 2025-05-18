using UnityEngine;

namespace Assets.Scripts.Game.ddz2.DDzGameListener
{
    /// <summary>
    /// 用于注册监听服务发起的消息
    /// </summary>
    public abstract class ServEvtListener : MonoBehaviour
    {
        void Awake()
        {
            OnAwake();
        }

        protected virtual void OnAwake()
        {

        }

        /// <summary>
        /// 刷新相应ui
        /// </summary>
        public abstract void RefreshUiInfo();
    }
}
