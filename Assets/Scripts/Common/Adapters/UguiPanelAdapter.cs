using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.Adapters
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class UguiPanelAdapter : YxBasePanelAdapter
    {
        private Canvas _panel;

        protected void Awake()
        {
            InitPanel();
        }

        private IEnumerator Start()
        {
            InitPanel();
            while (!_panel.overrideSorting)
            {
                _panel.overrideSorting = true;
                yield return null;
            }
            var rectTs = GetComponent<RectTransform>();
            rectTs.anchorMax = new Vector2(1f, 1f);
            rectTs.anchorMin = Vector2.zero;
            rectTs.anchoredPosition = Vector2.zero;
            rectTs.pivot = new Vector2(0.5f, 0.5f);
            rectTs.sizeDelta = Vector2.zero;
            rectTs.offsetMax = Vector2.zero;
            rectTs.offsetMin = Vector2.zero;
        }

        protected override void OnSortingOrder(int order)
        {
            _panel.sortingOrder = order + Order;
        }

        private void InitPanel()
        {
            if(_panel==null)_panel = GetComponent<Canvas>();
        }
    }
}
