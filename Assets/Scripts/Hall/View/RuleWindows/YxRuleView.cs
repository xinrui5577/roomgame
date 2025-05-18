
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.RuleWindows
{
    public class YxRuleView : YxView
    {
        [Tooltip("标题")]
        public string Title;
        [Tooltip("标签")]
        public string[] Tabs;
        /// <summary>
        /// 自身的Widget;
        /// </summary>
        [Tooltip("自身的Widget")]
        public YxBaseWidgetAdapter Widget;

        [Tooltip("label")]
        public YxBaseLabelAdapter[] Labels;
        [Tooltip("所有页")]
        public YxView[] Pages;


        [ContextMenu("FreshWidget")]
        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (Widget != null)
            {
                Widget.SetAnchor(transform.parent.gameObject,0,0,0,0);
            }
            FreshLabelStyle();
        }



        /// <summary>
        /// 刷新label样式
        /// </summary>
        private void FreshLabelStyle()
        {
            var ruleWin = GetData<YxRuleWindow>();
            if (ruleWin == null) return;
            var styles = ruleWin.LabelStyles;
            var count = Labels.Length;
            var defualtStyle = styles.GetElement(0);
            for (var i = 0; i < count; i++)
            {
                var style = styles.GetElement(i);
                if (style == null)
                {
                    style = defualtStyle;
                }
                var label = Labels[i];
                label.FreshStyle(style);
            }
        }
    }
}
