using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.paijiu
{
    public class RendererAdapter : YxBasePanelAdapter
    {
      
            private Renderer _renderer;
        private YxEUIType _uiType;

        // ReSharper disable once UnusedMember.Local
           void Awake()
            {
                OnSortingOrder(LayerOrder);
            }

            protected override void OnSortingOrder(int order)
            {
                BaseRenderer.sortingOrder = order + Order;
            }

        public override Vector4 GetBound()
        {
            throw new System.NotImplementedException();
        }

        public override int Depth { get; set; }

        public Renderer BaseRenderer
            {
                get
                {
                    return (_renderer ?? (_renderer = GetComponent<Renderer>()));
                }
            }

        public override YxEUIType UIType
        {
            get { return _uiType; }
        }
    }
}