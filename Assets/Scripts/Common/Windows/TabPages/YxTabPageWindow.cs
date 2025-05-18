using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Common.Utils;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Common.Windows.TabPages
{
    public class YxTabPageWindow : YxNguiWindow
    {
        [Tooltip("页签的Grid预制体")]
        public UIGrid TabelGridPerfab;
        [Tooltip("页签数据，以这个自动创建页签")]
        public TabData[] TabDatas;
        [Tooltip("页签预制体")]
        public YxTabItem PerfabTableItem;
        [Tooltip("页签的视图，用来隐藏该视图，可以设置空")]
        public Transform TabsView;
        [Tooltip("页签的视图要显示的数值，位数据")]
        public int TabSatate = -1;
        [Tooltip("页签的显示的交互api字段，通过此位数据来显示TabDatas对应的tab")]
        public string TabActionName;
        [SerializeField,Tooltip("视图集，视图的名称就是tab对应的Id; 0位置为默认view，找不到对应的tab Id 将使用此view")]
        private YxView[] _views; 
        [Tooltip("界面刷新回调")]
        public List<EventDelegate> OnFreshAction=new List<EventDelegate>();
        [Tooltip("切页选中回调")]
        public List<EventDelegate> OnTabSelectAction = new List<EventDelegate>();
        private readonly Dictionary<int, YxTabItem> _tabItems = new Dictionary<int, YxTabItem>();
        private UIGrid _tabGrid;
        protected readonly Dictionary<string,YxView> ViewsDictionary = new Dictionary<string, YxView>();
        protected UIGrid TabGrid
        {
            get { return _tabGrid; }
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            InitViewDict();
            if (!string.IsNullOrEmpty(TabActionName))
            {
                InitStateTotal++;
                Facade.Instance<TwManager>()
                      .SendAction(TabActionName, new Dictionary<string, object>(), UpdateView, false);
            }
            else
            {
                InitStateTotal++;
                UpdateView(TabSatate);
            }
        }

        protected void InitViewDict()
        {
            var len = _views.Length;
            ViewsDictionary.Clear();
            if (len <= 0) { return;}
            ViewsDictionary["0"] = _views[0];
            for (var i = 1; i < len; i++)
            {
                var view = _views[i];
                ViewsDictionary[view.name] = view;
            }
        }

        protected override void OnFreshView()
        {
            if (Data == null) return;
            int tabState;
            if (int.TryParse(Data.ToString(), out tabState))
            {
                TabSatate = tabState;
                UpdateTabs(TabDatas);
            }
            else
            {
                ActionCallBack();
            }
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(OnFreshAction.WaitExcuteCalls());
            }
        }

        /// <summary>
        /// 解析数据并创建TabData集合
        /// </summary>
        protected virtual void ActionCallBack()
        {
        }

        protected void UpdateTabs(IList<TabData> tabDatas)
        {
            if (TabelGridPerfab == null) { return;}
            YxWindowUtils.CreateMonoParent(TabelGridPerfab,ref _tabGrid);
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
        /// 切换标签 过时
        /// </summary>
        public void OnChangeTab(string index)
        {
            var  itemIndex = GetIndex(index);
            if (itemIndex >= _tabItems.Count) return;
            var item = _tabItems[itemIndex];
            if (item == null) return;
            item.OnSelected();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tabItem"></param>
        public void OnChageTabeClick(YxTabItem tabItem)
        { 
            if (tabItem == null) { return;}
            var toggle = tabItem.GetToggle();
            if (toggle == null || !tabItem.GetToggle().value)
            {
                var ctabData = tabItem.GetData<TabData>();
                if (ctabData == null) { return; }
                var cview = ctabData.View;
                if (cview != null)
                {
                    cview.Hide();
                }
                return;
            }
            var tabData = tabItem.GetData<TabData>();
            if (tabData == null) { return; }
            var view = tabData.View;
            var tabId = tabItem.Id;
            view = view == null && ViewsDictionary.ContainsKey(tabId) ? ViewsDictionary[tabId] : view;
            if (view == null)
            {
                if (ViewsDictionary.ContainsKey(tabId))
                {
                    view = ViewsDictionary[tabId];
                }
                else
                {
                    if (ViewsDictionary.ContainsKey("0"))
                    {
                        view = ViewsDictionary["0"];
                    }
                }
            }
            if (view == null) { return; }
            view.Show();
            OnChageTab(view, tabData);
        }

        protected virtual void OnChageTab(YxView view, TabData tabData)
        {
            if (view == null) { return;}
            view.ShowWithData(tabData);
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
            if (tableView.GetToggle().value)
            {
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(OnTabSelectAction.WaitExcuteCalls());
                }
            }
        }

        public virtual void OnTabSelect(YxTabItem tableView)
        {
            if (tableView == null) return;
            if (tableView.GetToggle().value)
            {
                TabSelectAction(tableView);
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(OnTabSelectAction.WaitExcuteCalls());
                }
            }
        }

        protected virtual void TabSelectAction(YxTabItem tableView)
        {
            
        }
    }

    [Serializable]
    public class TabData
    {
        [Tooltip("Id")]
        public string Id;
        [Tooltip("Type")]
        public string Type;
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
