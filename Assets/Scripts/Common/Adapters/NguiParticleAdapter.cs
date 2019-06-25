using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.Adapters
{ 
    [RequireComponent(typeof(ParticleSystem))]
    public class NguiParticleAdapter : YxBasePanelAdapter
    {
        private Renderer _renderer;

        protected override void OnSortingOrder(int order)
        {
            GetRenderer().sortingOrder = order;
        }

        private Renderer GetRenderer()
        {
            return _renderer ?? (_renderer = GetComponent<ParticleSystem>().GetComponent<Renderer>());
        }
    }
}
