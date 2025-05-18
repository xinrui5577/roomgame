using UnityEngine;

namespace Assets.Scripts.Common.Components
{
    /// <summary>
    /// 延时销毁
    /// </summary>
    public class DestroyDelay : MonoBehaviour
    {
        public float Delay = 1F;

        public void Start()
        {
            Destroy(gameObject, Delay);
        }
    }
}
