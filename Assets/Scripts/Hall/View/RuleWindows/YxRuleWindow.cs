using Assets.Scripts.Common.Windows.TabPages;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.View.RuleWindows
{
    public class YxRuleWindow : YxWindow
    { 
        public Transform ViewContainerTs;
        private Transform _containerTs;

        [Tooltip("标题Label")]
        public YxBaseLabelAdapter TitleLabelAdapter;

        public bool NodataNeedClose = true;

        public YxBaseLabelAdapter[] LabelStyles;

        public YxTabPageWindow TabPageView;


        public override YxEUIType UIType
        {
            get { return YxEUIType.Default; }
        }
         
        protected override void OnFreshView()
        {
            base.OnFreshView();
            var gk = GetData<string>();
            if (string.IsNullOrEmpty(gk))
            {
                gk = App.GameKey;
            }
            YxWindowUtils.CreateItemParent(ViewContainerTs, ref _containerTs, ViewContainerTs.parent);
            var prefix = App.Skin.GameInfo;
            var ruleName = string.Format("rule_{0}", gk);
            var namePrefix = string.Format("{0}_{1}", prefix, gk);
            var bundleName = string.Format("{0}/{1}", namePrefix, ruleName);
            var pfb = ResourceManager.LoadAsset(prefix, bundleName, ruleName);
            if (pfb == null)
            {
                prefix = App.Skin.Rule;
                bundleName = ruleName;
                pfb = ResourceManager.LoadAsset(prefix, bundleName, ruleName);
                if (pfb == null)
                {
                    if (NodataNeedClose) { Close();}
                    return;
                }
            }
            var content = YxWindowUtils.CreateGameObject(pfb, _containerTs);
            var view = content.GetComponent<YxRuleView>();
            if (view!=null)
            {
                view.UpdateView(this);
                FreshTitle(view.Title);
                FreshTabs(view);
            }
            UpOrder();
        }

        private void FreshTabs(YxRuleView view)
        {
            if (TabPageView == null) return;
            var tabs = view.Tabs;
            var tabCount = tabs.Length;
            if (tabCount <= 0)
            {
                TabPageView.transform.localScale = Vector3.zero;
                return;
            }
            TabPageView.transform.localScale = Vector3.one;
            var datas = new TabData[tabCount];
            var views = view.Pages;
            for (var i = 0; i < tabCount; i++)
            {
                var newTabData = new TabData
                {
                    Name = tabs[i],
                    View = views[i],
                    Index = i,
                    StarttingState = i == 0
                };
                datas[i] = newTabData;
            }
            TabPageView.TabDatas = datas;
            TabPageView.UpdateView(-1);
        }

        /// <summary>
        /// 刷新标题
        /// </summary>
        /// <param name="title"></param>
        private void FreshTitle(string title)
        {
            if (TitleLabelAdapter != null && !string.IsNullOrEmpty(title))
            {
                TitleLabelAdapter.Text(title);
            }
        }


        /// <inheritdoc />
        /// <summary>
        /// 设置层级
        /// </summary>
        /// <param name="order"></param>
        public override void SetOrder(int order)
        {
            Debug.LogError(order);
            if (PanelAdapter != null)
            {
                Order = order += PanelAdapter.Order;
                Debug.LogError(PanelAdapter.Order);
            }
            base.SetOrder(order);
        }
    }
}
