using UnityEngine;

namespace Assets.Scripts.Game.ddz2.DDz2Common
{
    /// <summary>
    /// 使粒子显示在NGUI上层.
    /// </summary>
    public class SetRenderQueue : MonoBehaviour
    {
        public int RenderQueue = 3000;

        void Start()
        {
            var rens = gameObject.GetComponentsInChildren<Renderer>();

            foreach (Renderer ren in rens)
            {
                ren.material.renderQueue = RenderQueue;
            }

          
        }
   

        public void SetQueue()
        {
            var rens = gameObject.GetComponentsInChildren<Renderer>();

            foreach (Renderer ren in rens)
            {
                ren.material.renderQueue = RenderQueue;
            }
        }
    }
}
