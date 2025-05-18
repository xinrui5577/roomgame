using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;

namespace Assets.Scripts.Common.Adapters
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class UguiPanelAdapter : YxBasePanelAdapter
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsMainPanel;

        /// <summary>
        /// 是否需要覆盖
        /// </summary>
        public bool OverrideSorting = true;
        private Canvas _panel;
        protected Canvas Panel
        {
            get { return _panel == null ? _panel = GetComponent<Canvas>() : _panel; }
        }

        protected override void Awake()
        {
            base.Awake(); 
            var panel = Panel;
            if (panel == null) return;
            if (OverrideSorting)
            {
                panel.overrideSorting = true;
            }
            if (!IsMainPanel) return;
            var rectTs = GetComponent<RectTransform>();
            rectTs.anchorMax = new Vector2(1f, 1f);
            rectTs.anchorMin = Vector2.zero;
            rectTs.anchoredPosition = Vector2.zero;
            rectTs.pivot = new Vector2(0.5f, 0.5f);
            rectTs.sizeDelta = Vector2.zero;
            rectTs.offsetMax = Vector2.zero;
            rectTs.offsetMin = Vector2.zero;
        }

        private IEnumerator Start()
        {
            var panel = Panel;
            if (OverrideSorting)
            {
                while (!panel.overrideSorting)
                {
                    panel.overrideSorting = true; 
                    yield return null;
                }
            } 
            if (!IsMainPanel) yield break; 
            var rectTs = GetComponent<RectTransform>();
            while (!(transform.parent is RectTransform))
            { 
                yield return null;
            }
            rectTs.anchorMax = new Vector2(1f, 1f);
            rectTs.anchorMin = Vector2.zero;
            rectTs.anchoredPosition = Vector2.zero;
            rectTs.pivot = new Vector2(0.5f, 0.5f);
            rectTs.sizeDelta = Vector2.zero;
            rectTs.offsetMax = Vector2.zero;
            rectTs.offsetMin = Vector2.zero;
            rectTs.localPosition = Vector3.zero;
        }

        void OnEnable()
        {
            transform.localPosition = Vector3.zero;
        }

        protected override void OnSortingOrder(int order)
        {
            Panel.sortingOrder = order + Order;
        }

        public override Vector4 GetBound()
        {
            return Vector4.zero;
        }

        public override int Depth { get; set; }

        public override YxEUIType UIType
        {
            get { return YxEUIType.Ugui; }
        }
    }
}
