using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.Utils;
using YxFramwork.Enums;

namespace Assets.Scripts.Common.Adapters
{
    [RequireComponent(typeof(UIPanel))]
    public class NguiPanelAdapter : YxBasePanelAdapter
    {
        private UIPanel _panel;
        protected UIPanel Panel
        {
            get { return _panel == null ? _panel = GetComponent<UIPanel>() : _panel; }
        }       
        
        /// <summary>
        /// 深度
        /// </summary>
        public override int Depth {
            get
            {
                var panel = Panel;
                return panel == null ? 0: panel.depth;
            }
            set
            {
                var panel = Panel;
                if (panel != null)
                {
                    panel.depth = value;
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            var mainCamera = Util.GetMainCamera();
            if (mainCamera == null) return;
            var uiCamera = mainCamera.GetComponent<UICamera>();
            if (uiCamera == null) return;
            var i = uiCamera.eventReceiverMask | (1 << gameObject.layer);
            uiCamera.eventReceiverMask = i;
        }

        protected override void OnSortingOrder(int order)
        {
            var panel = Panel;
            if (panel == null) { return;}
            panel.depth = order + Order;
            panel.sortingOrder = order;
        }

        public override Vector4 GetBound()
        {
            var panel = Panel;
            if (panel == null) return new Vector4();
            var bounds = panel.baseClipRegion;
            var temp = bounds.z / 2;
            var tempC = bounds.x;
            bounds.x -= temp;
            bounds.z = tempC + temp;
            /****************************/
            temp = bounds.w / 2;
            tempC = bounds.y;
            bounds.y -= temp;
            bounds.w = tempC + temp;
            return bounds;
        }

        public override YxEUIType UIType
        {
            get { return YxEUIType.Nguid; }
        }
    }
}
