using UnityEngine;

namespace Assets.Scripts.Game.fillpit
{
    /// <summary>
    /// 使粒子显示在NGUI上层.
    /// </summary>
    public class SetRenderQueue : MonoBehaviour
    {
        public int RenderQueue = 3000;

        protected void Start()
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
                ren.sharedMaterial.renderQueue = RenderQueue;
            }
        }
    }
}

