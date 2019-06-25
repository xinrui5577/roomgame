using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Common.Windows.TabPages
{
    public class YxTabPageWindow : YxNguiWindow
    {
        /// <summary>
        /// 标签Grid
        /// </summary>
        [Tooltip("页签的Grid预制体")]
        public UIGrid TabelGridPerfab;
        /// <summary>
        /// 页签数据
        /// </summary>
        [Tooltip("页签数据，以这个自动创建页签")]
        public TabData[] TabDatas;
        /// <summary>
        /// 
        /// </summary>
        [Tooltip("页签预制体")]
        public YxTabItem PerfabTableItem;
        /// <summary>
        /// 
        /// </summary>
        [Tooltip("页签的视图，用来隐藏该视图，可以设置空")]
        public Transform TabsView;
        [Tooltip("页签的视图要显示的数值，位数据")]
        public int TabSatate = -1;
        [Tooltip("页签的显示的交互api字段，通过此位数据来显示TabDatas对应的tab")]
        public string TabActionName;
        [Tooltip("界面刷新回调")]
        public List<EventDelegate> OnFreshAction=new List<EventDelegate>();
        [Tooltip("切页选中回调")]
        public List<EventDelegate> OnTabSelectAction = new List<EventDelegate>();
        private readonly Dictionary<int, YxTabItem> _tabItems = new Dictionary<int, YxTabItem>();
        private UIGrid _tabGrid;

        protected UIGrid TabGrid
        {
            get { return _tabGrid; }
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            if (!string.IsNullOrEmpty(TabActionName))
            {
                InitStateTotal++;
                Facade.Instance<TwManger>()
                      .SendAction(TabActionName, new Dictionary<string, object>(), UpdateView, false);
            }
            else
            {
                InitStateTotal++;
                UpdateView(TabSatate);
            }
        }

        protected override void OnFreshView()
        {
            if (Data == null) return;
            if (int.TryParse(Data.ToString(), out TabSatate))
            {
                UpdateTabs(TabDatas);
            }
            else
            {
                ActionCallBack();
            }
            StartCoroutine(YxTools.WaitExcuteCalls(OnFreshAction));
        }

        protected virtual void ActionCallBack()
        {
        }

        protected void UpdateTabs(IList<TabData> tabDatas)
        {
            YxWindowUtils.CreateItemGrid(TabelGridPerfab, ref _tabGrid);
            var count = tabDatas.Count;
            var gridTs = _tabGrid.transform;
            _tabItems.Clear();
            for (var i = 0; i < count; i++)
            {
                if((TabSatate & (1<<i)) == 0)continue;
                var data = tabDatas[i];
                var item = YxWindowUtils.CreateItem(PerfabTableItem, gridTs);
                item.UpdateView(data);
                item.name = i.ToString(CultureInfo.InvariantCulture);
                _tabItems[i] = item;
            }
            if (TabsView != null)
            {
                TabsView.localScale = _tabItems.Count < 2 ? new Vector3(0, 1, 1) : Vector3.one;
            }
            StartCoroutine(SelectFirst());
            _tabGrid.repositionNow = true;
            _tabGrid.Reposition();
        }

        private IEnumerator SelectFirst()
        {
            yield return new WaitForEndOfFrame();
            foreach (var itemData in _tabItems)
            {
                var item = itemData.Value;
                if (item == null) continue;
                if (!item.GetToggle().startsActive) continue;
                item.OnSelected();
                yield break;
            }
            foreach (var itemData in _tabItems)
            {
                var item = itemData.Value;
                if (item == null) continue;
                item.OnSelected();
                yield break;
            }
        }

        /// <summary>
        /// 切换标签
        /// </summary>
        public void OnChangeTab(string index)
        {
            var  itemIndex = GetIndex(index);
            if (itemIndex >= _tabItems.Count) return;
            var item = _tabItems[itemIndex];
            if (item == null) return;
            item.OnSelected();
        }

        protected virtual int GetIndex(string index)
        {
            int itemIndex;
            return !int.TryParse(index, out itemIndex) ? 0 : itemIndex;
        }

        public virtual void OnTableClick(YxTabItem tableView)
        {
            if (tableView == null) return;
            tableView.OnSelected();
            if (tableView.GetToggle().value.Equals(true))
            {
                StartCoroutine(YxTools.WaitExcuteCalls(OnTabSelectAction));
            }
        }

        public virtual void OnTabSelect(YxTabItem tableView)
        {
            if (tableView == null) return;
            if (tableView.GetToggle().value.Equals(true))
            {
                TabSelectAction(tableView);
                StartCoroutine(YxTools.WaitExcuteCalls(OnTabSelectAction));
            }
        }

        protected virtual void TabSelectAction(YxTabItem tableView)
        {
            
        }
    }

    [Serializable]
    public class TabData
    {
        /// <summary>
        /// tab名称
        /// </summary>
        [Tooltip("tab名称")]
        public string Name;
        /// <summary>
        /// 对应视图
        /// </summary>
        [Tooltip("对应视图")]
        public YxView View;
        /// <summary>
        /// 没选中状态（sprite名称），可为空
        /// </summary>
        [Tooltip("没选中状态（sprite名称），可为空")]
        public string UpStateName;
        /// <summary>
        /// 选中状态（sprite名称），可为空
        /// </summary>
        [Tooltip("选中状态（sprite名称），可为空")]
        public string DownStateName;
        /// <summary>
        /// 默认选中
        /// </summary>
        [Tooltip("默认选中")]
        public bool StarttingState;
        [SerializeField]
        public object Data;

        public int Index;
    }
}
