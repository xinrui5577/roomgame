/** 
 *文件名称:     YxPageListWindow.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2018-01-25 
 *描述:         多tab页，且单独view中支持拖拽刷新请求操作
 *历史记录: 
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows.TabPages;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Framework;
using Object = System.Object;

namespace Assets.Scripts.Hall.View.PageListWindow
{
    [Obsolete("优先使用YxPageListView和YxTabPageWindow组合处理,本脚本弃用,继承本脚本的功能会陆续修改")]
    public class YxPageListWindow : YxTabPageWindow
    {
        #region UI Param

        [Tooltip("布局控制")]
        public UITable Table;
        [Tooltip("item预设")]
        public YxView ListItem;
        [Tooltip("ScrollView")]
        public UIScrollView ScrollView;
        [Tooltip("有状态显示界面")]
        public GameObject HasDataSign;
        [Tooltip("空状态显示界面")]
        public GameObject NoDataSign;
        #endregion

        #region Data Param

        [Tooltip("Key页面参数")]
        public string KeyPage = "p";

        [Tooltip("请求是否带缓存")] public bool ActionWithCacheKey = false;

        [Tooltip("页签数据是否需要发送请求")]
        public bool TabNeedAction;
        [Tooltip("唤醒时是否发送请求")]
        public bool RequestOnAwake = true;
        [Tooltip("Id 排序(是否为逆序)，默认为正序：false")]
        public bool IdAntitone;
        [Tooltip("显示回调")]
        public List<EventDelegate> OnShowAction = new List<EventDelegate>();
        [Tooltip("隐藏回调")]
        public List<EventDelegate> OnHideAction = new List<EventDelegate>();
        [Tooltip("action首次回调")]
        public List<EventDelegate> OnFirstAction = new List<EventDelegate>();
        #endregion

        #region Local Data

        /// <summary>
        /// 是否为界面第一次打开
        /// </summary>
        private bool _isFirst = true;

        /// <summary>
        /// 当前页码（默认起始页码1）
        /// </summary>
        private int _curPage;

        /// <summary>
        /// 总数
        /// </summary>
        protected int _totalCount;

        /// <summary>
        /// 当前页数据数量
        /// </summary>
        private int _curCount;

        /// <summary>
        /// 每页记录数量（每页请求数量上限）
        /// </summary>
        private int _perPageCount;

        /// <summary>
        /// 请求是否发送中
        /// </summary>
        private bool _request;

        /// <summary>
        /// 当前所有的对象
        /// </summary>
        protected List<YxData> Items = new List<YxData>();

        /// <summary>
        /// 请求参数
        /// </summary>
        protected Dictionary<string, object> ActionParam=new Dictionary<string, object>();
        #endregion

        #region Life Cycle

        protected override void OnAwake()
        {
            if (!PerfabTableItem)
            {
                if (RequestOnAwake)
                {
                    FirstRequest();
                }             
            }
            else
            {
                if (TabNeedAction)
                {
                    
                }
                else
                {
                    DealTabsData();
                    RefreshTabs();
                }
            }
        }

        protected override void ActionCallBack()
        {
            base.ActionCallBack();
            if (Data is Dictionary<string, object>)
            {
                OnActionCallBackDic();
            }
            if (Data is string)
            {
                
            }
        }

        protected override void OnShow()
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(OnShowAction.WaitExcuteCalls());
            }
        }

        /// <summary>
        /// 调整onhide与设置父级active 逻辑
        /// </summary>
        public override void Hide()
        {
            OnHide();
            gameObject.SetActive(false);
        }

        protected override void OnHide()
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(YxTools.WaitExcuteCalls(OnHideAction));
            }
        }

        #endregion

        #region Function

        protected virtual void OnActionCallBackDic()
        {
            PageRequestData data = new PageRequestData(Data, GetItemType());
            DealPageData(data);  
        }

        protected void DealPageData(PageRequestData data)
        {
            var showState = data.DataCount != 0;
            NoDataSign.TrySetComponentValue(!showState);
            HasDataSign.TrySetComponentValue(showState);
            Table.gameObject.TrySetComponentValue(showState);
            if (_isFirst)
            {
                _totalCount = data.TotalCount;
                _perPageCount = data.PageCount;
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(OnFirstAction.WaitExcuteCalls());
                }
                _isFirst = false;
            }
            _curCount += data.DataCount;
            var list = data.DataItems;
            RefreshView(list, Items.Count);
            Items.AddRange(list);
            if (CouldNotDrad() && data.ExistPageNum)
            {
                _curPage = data.PageNumber;
            }
            _request = false;
        }

        protected virtual Type GetItemType()
        {
            return typeof(YxData);
        }

        protected virtual void SetActionDic()
        {
            ActionParam = new Dictionary<string, object>()  {
                {KeyPage, ++_curPage}
            };
        }

        protected virtual void SendActionWithPage()
        {
            string cacaheKey = "";
            SetActionDic();
            if (ActionWithCacheKey)
            {
                YxDebug.LogDictionary(ActionParam);
                cacaheKey = YxTools.GetCacahKey(TabActionName,ActionParam);
            }
            YxTools.SendActionWithCacheKey(TabActionName, ActionParam, UpdateView, cacaheKey);
        }

        protected void DragWithRequest()
        {
            if (ScrollView)
            {
                ScrollView.onMomentumMove += delegate
                {
                    if (!_request)
                    {
                        ScrollView.UpdateScrollbars(true);
                        var constraint = ScrollView.panel.CalculateConstrainOffset(ScrollView.bounds.min,
                            ScrollView.bounds.min);
                        if (constraint.y <= 1)
                        {
                            _request = true;
                            if (CouldNotDrad())
                            {
                                return;
                            }
                            SendActionWithPage();
                        }
                    }
                };
            }
        }

        private bool CouldNotDrad()
        {
            return _totalCount <= _perPageCount
                   || _curCount >= _totalCount
                   || _perPageCount * (_curPage) >= _totalCount;
        }

        protected virtual void RefreshView(List<YxData> data, int startIndex = 0)
        {

            for (int i = startIndex, endIndex = data.Count + startIndex; i < endIndex; i++)
            {
                var view = Table.transform.GetChildView(i, ListItem);
                var itemData = data[i - startIndex];
                view.Id = (IdAntitone? _totalCount - i :i+1).ToString();
                view.UpdateView(itemData);
            }
            Table.repositionNow = true;
        }

        /// <summary>
        /// 处理tab数据
        /// </summary>
        protected virtual void DealTabsData()
        {

        }

        /// <summary>
        /// 刷新Tabs显示
        /// </summary>
        protected virtual void RefreshTabs()
        {
            UpdateView(TabSatate);
        }

        protected virtual void FirstRequest()
        {
            Reset();
            SendActionWithPage();
            DragWithRequest();
        }

        public virtual void HideComponents()
        {
            HideTableItems();
        }

        protected virtual void HideTableItems()
        {
            var list=Table.GetChildList();
            foreach (var item in list)
            {
                HideItem(item);
                var view = item.GetComponent<YxView>();
                if (view)
                {
                    view.Hide();
                }
                else
                {
                    item.gameObject.SetActive(false);
                }  
            }
        }

        protected virtual void HideItem(Transform item)
        {

        }

        protected virtual void Reset()
        {
            _isFirst = true;
            _curPage = 0;
            _curCount = 0;
            Items.Clear();
        }

        #endregion
    }

    /// <summary>
    /// 分页式请求数据基本结构
    /// </summary>
    public class PageRequestData : YxData
    {
        /// <summary>
        /// key数据总数量
        /// </summary>
        private const string KeyTotalCount = "totalCount";
        /// <summary>
        /// key主体数据量
        /// </summary>
        private const string KeyCount = "count";
        /// <summary>
        /// key每个分页的上限数量
        /// </summary>
        private const string KeyPageCount = "pCount";
        /// <summary>
        /// Key 页码
        /// </summary>
        private const string KeyPageNumber = "p";
        /// <summary>
        /// 当前页数据数量
        /// </summary>
        private int _dataCount;
        /// <summary>
        /// 每页数量
        /// </summary>
        private int _pageCount;
        /// <summary>
        /// 数据总数量
        /// </summary>
        private int _totalCount;
        /// <summary>
        /// 页码（用于校验，以返回页码为准）
        /// </summary>
        private int _pageNum;

        public int TotalCount
        {
            get { return _totalCount; }
        }

        public int PageCount
        {
            get { return _pageCount; }
        }

        public int DataCount
        {
            get { return _dataCount; }
        }

        /// <summary>
        /// 是否存在页码同步
        /// </summary>
        public bool ExistPageNum
        {
            get { return _pageNum != 0; }
        }

        public int PageNumber
        {
            get
            {
                return _pageNum;
            }
        }

        protected override void ParseData(Dictionary<string, object> dic)
        {
            base.ParseData(dic);
            dic.TryGetValueWitheKey(out _dataCount, KeyCount);
            dic.TryGetValueWitheKey(out _pageCount, KeyPageCount);
            dic.TryGetValueWitheKey(out _pageNum, KeyPageNumber);
            dic.TryGetValueWitheKey(out _totalCount, KeyTotalCount);
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="type">主体数据转换类型</param>
        public PageRequestData(object data, Type type) : base(data,type)
        {
        }

    }
    /// <summary>
    /// 传递数据处理
    /// </summary>
    public class YxData : Object
    {
        /// <summary>
        /// key主体数据
        /// </summary>
        private const string KeyData = "data";
        /// <summary>
        /// 类型
        /// </summary>
        protected Type Type; 
        /// <summary>
        /// 主体数据(List格式)
        /// </summary>
        protected List<YxData> Items ;
        /// <summary>
        /// 主体数据(Dic格式)
        /// </summary>
        protected Dictionary<string,YxData> Dic; 

        public List<YxData> DataItems
        {
            get
            {
                return Items;
            }
        }

        public Dictionary<string, YxData> DataDic
        {
            get
            {
                return Dic;
            }
        }


        public YxData(object data)
        {
            TryParse(data);
        }

        public YxData(object data, Type type)
        {
            Type = type;
            TryParse(data);
        }

        protected void TryParse(object data)
        {
            if (data is Dictionary<string, object>)
            {
                var dic = data as Dictionary<string, object>;
                ParseData(dic);
            }
            else
            {
                YxDebug.LogError("YxData is not a dictionary<string,object> ,please check!");
            }
        }

        protected virtual List<YxData> GetDatas(List<object> list) 
        {
            var canChange = typeof(YxData).IsAssignableFrom(Type);
            var datas = new List<YxData>();
            if (canChange)
            {
                for (int i = 0, count = list.Count; i < count; i++)
                {
                    var paramData = list[i];
                    var data = (YxData)Activator.CreateInstance(Type, new object[]
                    {
                        paramData
                    });
                    datas.Add(data);
                }
            }
            return datas;
        }

        protected virtual Dictionary<string, YxData> GetDatas(Dictionary<string, object> dic)
        {
            var canChange = typeof(YxData).IsAssignableFrom(Type);
            var datas = new Dictionary<string, YxData>();
            if (canChange)
            {
                foreach (var item in dic)
                {
                    var data = (YxData)Activator.CreateInstance(
                        Type, new object[]
                        {
                        item.Value
                         });
                    datas.Add(item.Key,data);
                }
            }
            return datas;
        }

        /// <summary>
        /// 单参数转换
        /// </summary>
        protected virtual void ParseData(Dictionary<string, object> dic)
        {
            TryGetList(dic);
            TryGetDic(dic);
        }

        protected virtual void TryGetList(Dictionary<string, object> dic)
        {
            List<object> list;
            dic.TryGetValueWitheKey(out list,KeyData); 
            Items = GetDatas(list);
        }

        protected virtual void TryGetDic(Dictionary<string, object> dic)
        {
            Dictionary<string, object> dataDic;
            dic.TryGetValueWitheKey(out dataDic, KeyData);
            Dic = GetDatas(dataDic);
        }
    }
}