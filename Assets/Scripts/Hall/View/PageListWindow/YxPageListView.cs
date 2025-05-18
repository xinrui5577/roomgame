/** 
 *文件名称:     YxPageListView.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2018-04-18 
 *描述:         分页格式的view
 *历史记录: 
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.PageListWindow
{
    public class YxPageListView : YxView
    {
        #region UI Param
        [Tooltip("item预设")]
        public YxPageItem ListItem;
        [Tooltip("空状态显示界面")]
        public GameObject NoDataSign;
        [Tooltip("ScrollView")]
        public UIScrollView ScrollView;
        [Tooltip("布局控制")]
        public UITable Table;
        #endregion
        #region Data Param
        [Tooltip("请求名称")]
        public string ActionName;
        [Tooltip("Key页面参数")]
        public string KeyPage = "p";
        [Tooltip("Key页面数据偏移量")]
        public string KeyPageOffset="offset";
        [Tooltip("附属参数Key集合")]
        public List<string> ParamKeys=new List<string>();
        [Tooltip("附属参数Value集合")]
        public List<string> ParamValues= new List<string>();
        [Tooltip("请求是否带缓存")]
        public bool ActionWithCacheKey = false;
        [Tooltip("Id 排序(是否为逆序)，默认为正序：false")]
        public bool IdAntitone;
        [Tooltip("显示回调")]
        public List<EventDelegate> OnShowAction = new List<EventDelegate>();
        [Tooltip("隐藏回调")]
        public List<EventDelegate> OnHideAction = new List<EventDelegate>();
        [Tooltip("action首次回调")]
        public List<EventDelegate> OnFirstAction = new List<EventDelegate>();
        [Tooltip("action每次回调")]
        public List<EventDelegate> OnActionCalls = new List<EventDelegate>();
        [Tooltip("Item 删除回调")]
        public List<EventDelegate> OnItemRemoveAction=new List<EventDelegate>();
        [Tooltip("Item 改变回调")]
        public List<EventDelegate> OnItemChangeAction = new List<EventDelegate>();
        [Tooltip("无数据回调")]
        public List<EventDelegate> OnNoDataAction=new List<EventDelegate>();
        #endregion
        #region Local Data

        /// <summary>
        /// 是否存在数据的状态（用于控制显示无数据UI与存在数据UI区别相关）
        /// </summary>
        public bool HaveDataStatus { private set; get; }

        /// <summary>
        /// 是否为界面第一次打开
        /// </summary>
        private bool _isFirst = true;

        /// <summary>
        /// 当前页码（默认起始页码1）
        /// </summary>
        private int _curPage;

        /// <summary>
        /// 数据偏移量
        /// </summary>
        private int _curOffset;

        /// <summary>
        /// 总数
        /// </summary>
        private int _totalCount;

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
        /// 所有子对象数据
        /// </summary>
        protected List<YxData> ItemDatas = new List<YxData>();

        /// <summary>
        /// 所有子对象View
        /// </summary>
        protected List<YxView> ItemViews=new List<YxView>(); 

        /// <summary>
        /// 请求参数
        /// </summary>
        protected Dictionary<string, object> ActionParam = new Dictionary<string, object>();

        /// <summary>
        /// 列表中操作的Item
        /// </summary>
        protected YxPageItem CurItem;
        #endregion
        #region Life Cycle

        protected override void OnEnable()
        {
            base.OnEnable();
            FirstRequest();
        }

        protected override void OnShow()
        {
            base.OnShow();
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(OnShowAction.WaitExcuteCalls());
            }
        }

        protected override void OnHide()
        {
            base.OnHide();
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(OnHideAction.WaitExcuteCalls());
            }
        }

        /// <summary>
        /// 返回的是Item 的数据类型，需要重写对应Item的GetType方法
        /// </summary>
        /// <returns></returns>
        protected virtual Type GetItemType()
        {
            if (ListItem)
            {
                return ListItem.GetDataType();
            }
            else
            {
                return typeof(YxData);
            }
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if(Data==null)
            {
                return;
            }
            OnActionCallBack();
        }

        protected virtual void OnActionCallBack()
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(OnActionCalls.WaitExcuteCalls());
            }
            OnActionCallBackDic();
            if (ScrollView)
            {
                ScrollView.onMomentumMove();
            }
        }

        protected virtual void OnActionCallBackDic()
        {
            PageRequestData data = new PageRequestData(Data, GetItemType());
            DealPageData(data);
        }
        protected void DealPageData(PageRequestData data)
        {
            var showState = data.DataCount != 0;
            SetdataShow(showState);
            _totalCount = data.TotalCount;
            if (_isFirst)
            {
                _perPageCount = data.PageCount;
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(OnFirstAction.WaitExcuteCalls());
                }
                _isFirst = false;
            }
            _curCount += data.DataCount;
            var list = data.DataItems;
            RefreshView(list, ItemDatas.Count);
            ItemDatas.AddRange(list);
            if (CouldNotDrag() && data.ExistPageNum)
            {
                _curPage = data.PageNumber;
            }
            _request = false;
        }
        /// <summary>
        /// View 首次请求
        /// </summary>
        protected virtual void FirstRequest()
        {
            Reset();
            SendActionWithPage();
            DragWithRequest();
        }
        protected virtual void SendActionWithPage()
        {
            string cacaheKey = "";
            SetActionDic();
            if (ActionWithCacheKey)
            { 
                cacaheKey = YxTools.GetCacahKey(ActionName, ActionParam);
            }
            YxTools.SendActionWithCacheKey(ActionName, ActionParam, UpdateView, cacaheKey);
        }

        /// <summary>
        /// 设置请求参数
        /// </summary>
        protected virtual void SetActionDic()
        {
            ActionParam = new Dictionary<string, object>()
            {
                {KeyPage, ++_curPage},
                {KeyPageOffset,_curOffset}
            };
            var minCount = Math.Min(ParamKeys.Count,ParamValues.Count);
            for (int index = 0; index < minCount; index++)
            {
                var key = ParamKeys[index];
                var value = ParamValues[index];
                if (ActionParam.ContainsKey(key))
                {
                    ActionParam[key] = value;
                }
                else
                {
                    ActionParam.Add(key, value);
                }
            }
        }
        /// <summary>
        /// 刷新列表
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="needClear"></param>
        protected virtual void RefreshView(List<YxData> data, int startIndex = 0,bool needClear=false)
        {
            if (needClear)
            {
                ItemViews.Clear();
            }
            for (int i = startIndex, endIndex = data.Count + startIndex; i < endIndex; i++)
            {
                var view = Table.transform.GetChildView(i, ListItem);
                var itemData = data[i - startIndex];
                view.Id = (IdAntitone ? _totalCount - i : i + 1).ToString();
                view.UpdateView(itemData);
                ItemViews.Add(view);
            }
            Table.repositionNow = true;
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
                            if (CouldNotDrag())
                            {
                                return;
                            }
                            SendActionWithPage();
                        }
                    }
                };
            }
        }

        protected bool CouldNotDrag()
        {
            return _totalCount <= _perPageCount
                   || _curCount >= _totalCount
                   || _perPageCount * (_curPage)-_curOffset>= _totalCount;
        }

        protected virtual void Reset()
        {
            _isFirst = true;
            _curPage = 0;
            _curCount = 0;
            _curOffset = 0;
            ItemViews.Clear();
            ItemDatas.Clear();
        }

        /// <summary>
        /// 删除子对象
        /// </summary>
        /// <param name="pageItem"></param>
        public virtual void RemoveChildItem(YxPageItem pageItem)
        {
            var index = ItemViews.FindIndex(item => item != null&& item==pageItem);
            if (index > -1)
            {
                if (gameObject.activeInHierarchy)
                {
                    ItemDatas.RemoveAt(index);
                    ItemViews.RemoveAt(index);
                    _curOffset++;
                    _totalCount--;
                    pageItem.Hide();
                    StartCoroutine(OnItemRemoveAction.WaitExcuteCalls());
                }
            }
        }
        /// <summary>
        /// 改变Item
        /// </summary>
        /// <param name="pageItem"></param>
        /// <param name="data"></param>
        public virtual void ChangeChildItem(YxPageItem pageItem,YxData data)
        {
            pageItem.UpdateView(data);
            var index = ItemViews.FindIndex(item => item != null && item == pageItem);
            if (index > -1)
            {
                ItemDatas[index] = data;
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(OnItemChangeAction.WaitExcuteCalls());
                }
            }
        }

        /// <summary>
        /// 刷新页面
        /// </summary>
        public virtual void FreshPageList()
        {
             _curCount = ItemDatas.Count;
            SetdataShow(_curCount != 0);
            if (HaveDataStatus)
            {
                HideComponents();
                RefreshView(ItemDatas,0,true);
            }
        }

        /// <summary>
        /// 隐藏组件
        /// </summary>
        public virtual void HideComponents()
        {
            var list = Table.GetChildList();
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


        #endregion
        #region Function

        /// <summary>
        /// 在List中加入值(值唯一)
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool TryAddItemToList(List<string> list, string value)
        {
            var index = list.FindIndex(item => item.Equals(value));
            var state = false;
            if (index <= -1)
            {
                list.Add(value);
                state = true;
            }
            return state;
        }

        /// <summary>
        /// 设置数据状态相关显示
        /// </summary>
        /// <param name="dataShowStatus"></param>
        protected void SetdataShow(bool dataShowStatus)
        {
            HaveDataStatus = dataShowStatus;
            NoDataSign.TrySetComponentValue(!dataShowStatus);
            Table.gameObject.TrySetComponentValue(dataShowStatus);
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(OnNoDataAction.WaitExcuteCalls());
            }
        }

        #endregion

    }
}
