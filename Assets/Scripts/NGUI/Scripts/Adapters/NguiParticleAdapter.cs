using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;

namespace Assets.Scripts.Common.Adapters
{ 
    [RequireComponent(typeof(ParticleSystem))]
    public class NguiParticleAdapter : YxBasePanelAdapter
    {
        private Renderer _renderer;
        protected Renderer Renderer
        {
            get { return _renderer == null ? _renderer = GetComponent<Renderer>() : _renderer; }
        }
      
        protected override void OnSortingOrder(int order)
        {
            var r = Renderer;
            if (r == null) { return; }
            r.sortingOrder = order;
        }

        public override Vector4 GetBound()
        {
            return Vector4.zero;
        }

        public override int Depth { get; set; }

        public override YxEUIType UIType
        {
            get { return YxEUIType.Nguid; }
        }
    }
}
