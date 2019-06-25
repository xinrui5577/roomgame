using UnityEngine;
using YxFramwork.Common.Utils;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.Adapters
{
    [RequireComponent(typeof(UIPanel))]
    public class NguiPanelAdapter : YxBasePanelAdapter
    {
        private UIPanel _panel;
        protected void Awake()
        {
            var mainCamera = Util.GetMainCamera();
            if (mainCamera == null) return;
            var uiCamera = mainCamera.GetComponent<UICamera>();
            if (uiCamera == null) return;
            var i = uiCamera.eventReceiverMask | (1 << gameObject.layer);
            uiCamera.eventReceiverMask = i;
        }

        protected override void OnSortingOrder(int order)
        {
            var panel = GetPanel();
            panel.depth = order + Order;
            panel.sortingOrder = order;
        }

        public UIPanel GetPanel()
        {
            return _panel ?? (_panel = GetComponent<UIPanel>());
        }
    }
}
