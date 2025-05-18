using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base
{
    /// <summary>
    /// 可循环listView(Item 循环使用)
    /// </summary>
    public class BaseSocialWrapListView : BaseSocialView
    {
        [Tooltip("布局控制")]
        public SingleTrackWrapContent WrapContent;
        [Tooltip("ScrollView")]
        public UIScrollView ScrollView;
        [Tooltip("局部请求地址(列表局部数据)")]
        public string PartAction;
        [Tooltip("默认局部请求长度")]
        public int DefPartLenth = 3;
        [Tooltip("数据是否存在事件（控制显示列表信息还是显示无数据提示）")]
        public List<EventDelegate> HaveDataAction = new List<EventDelegate>();
        /// <summary>
        /// id集合
        /// </summary>
        protected List<string> PageIds = new List<string>();
        /// <summary>
        /// 数据集合
        /// </summary>
        protected Dictionary<string, Dictionary<string,object>> IdsDataDic = new Dictionary<string, Dictionary<string, object>>();
        /// <summary>
        /// 单次请求数据长度
        /// </summary>
        protected int PartPageLenth;

        /// <summary>
        /// 当前页面数据数量
        /// </summary>
        protected int TotalCount
        {
            private set
            {
                if (_totalCount != value)
                {
                    _totalCount = value;
                }
            }
            get { return _totalCount; }
        }

        private int _totalCount;

        /// <summary>
        /// 已获取数据总量
        /// </summary>
        protected int DataCount
        {
            private set
            {
                if (_dataCount != value)
                {
                    _dataCount = value;
                }
            }
            get { return _dataCount; }
        }

        private int _dataCount;
        /// <summary>
        /// 当前UI最大索引
        /// </summary>
        protected int MaxUiIndex;

        /// <summary>
        /// 请求是否发送中
        /// </summary>
        protected bool Request;

        /// <summary>
        ///  当前页面是否存在数据
        /// </summary>
        public bool HaveData
        {
            get {return TotalCount > 0;}
        }
        protected override void AddListeners()
        {
            base.AddListeners();
            AddEventListener<Dictionary<string, Dictionary<string, object>>>(PartAction, OnReceivePartData);
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
            RemoveEventListener<Dictionary<string, Dictionary<string, object>>>(PartAction, OnReceivePartData);
        }

        /// <summary>
        /// 刷新Wrap Item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        protected virtual void OnWrapItemFresh(BaseSocialWrapItem item, int index)
        {
            if (TotalCount > index)
            {
                var itemId = PageIds[index];
                if (IdsDataDic.ContainsKey(itemId))
                {
                    var freshData = IdsDataDic[itemId].Copy<object>();
                    var getData = item.GetData();
                    if (freshData.ValueEqual(getData))
                    {
                        return;
                    }
                    item.IdCode = index;
                    BeforeItemFresh(item);
                    item.UpdateView(freshData);
                }
                else
                {
                    YxDebug.LogEvent(SocialTools.KeyNoticeItemIdNotExist + itemId);
                }
            }
        }

        /// <summary>
        /// Item 刷新前处理
        /// </summary>
        protected virtual void BeforeItemFresh(BaseSocialWrapItem item)
        {
        }

        protected override void OnInitDataValid()
        {
            var getPageLenth = DefPartLenth;
            ResetData();
            InitGetData.TryGetStringListWithKey(out PageIds, SocialTools.KeyIds);
            InitGetData.TryGetValueWitheKey(out getPageLenth, SocialTools.KeyDataNum, getPageLenth);
            PartPageLenth = Math.Max(getPageLenth, DefPartLenth);
            ScrollCheck();
        }

        /// <summary>
        /// 刷新列表结构
        /// </summary>
        protected virtual void FreshWrapList(Dictionary<string, Dictionary<string, object>> setDic)
        {
            SetIdsDataDic(setDic.Copy<Dictionary<string, Dictionary<string, object>>>());
            MaxUiIndex = PageIds.Count;
            ScrollCheck();
            OnReceivePartData(IdsDataDic.Copy<Dictionary<string, Dictionary<string, object>>>());
        }

        protected virtual void SetIdsDataDic(Dictionary<string, Dictionary<string, object>> setDic)
        {
            IdsDataDic = setDic.Copy<Dictionary<string, Dictionary<string, object>>>();
        }

        protected void ScrollCheck()
        {
            if (DataCheck())
            {
                if (ScrollView)
                {
                    ScrollDrag();
                    ScrollView.onMomentumMove();
                }
            }
        }

        protected override Dictionary<string, object> GetPostParam()
        {
            var baseParam = base.GetPostParam();
            var reqIds = PageIds.GetRange(DataCount, Math.Min(TotalCount - DataCount, PartPageLenth));
            if (reqIds.Count == 0)
            {
                return baseParam;
            }
            baseParam.Add(SocialTools.KeyIds, reqIds);
            baseParam.Add(SocialTools.KeyDefKey, reqIds);
            return baseParam;
        }
        /// <summary>
        /// 视图拖动处理
        /// </summary>
        protected virtual void ScrollDrag()
        {
            if (ScrollView)
            {
                ScrollView.onMomentumMove += delegate
                {
                    ScrollView.UpdateScrollbars(true);
                    var constraint = ScrollView.panel.CalculateConstrainOffset(ScrollView.bounds.min, ScrollView.bounds.min);
                    if (constraint.y <= 1)
                    {
                        if (CouldNotDrag())
                        {
                            return;
                        }
                        CheckPartData();
                    }
                };
            }
        }

        protected Dictionary<string, Dictionary<string,object>> TestDic = new Dictionary<string, Dictionary<string, object>>();
        protected virtual void CheckPartData()
        {
        }



        /// <summary>
        /// 拖动限制
        /// </summary>
        /// <returns></returns>
        protected virtual bool CouldNotDrag()
        {
            return TotalCount <= PartPageLenth || DataCount >= TotalCount;
        }


        /// <summary>
        /// 接收到局部数据
        /// </summary>
        /// <param name="getDic"></param>
        protected virtual void OnReceivePartData(Dictionary<string, Dictionary<string, object>> getDic)
        {
            IdsDataDic.Clear();
            if (getDic.Count > 0)
            {
                bool isFirst = IdsDataDic.Count == 0;
                foreach (var getItem in getDic)
                {
                    InitItemData(getItem.Key, getItem.Value);
                }
                DataCount = IdsDataDic.Count;
                if (DataCount > 0)
                {
                    if (isFirst)
                    {
                        if (WrapContent)
                        {
                            WrapContent.OnItemFresh = delegate (GameObject go, int realIndex)
                            {
                                var wrapItem = go.GetComponent<BaseSocialWrapItem>();
                                if (wrapItem)
                                {
                                    var showIndex = Math.Abs(realIndex);
                                    if (TotalCount > showIndex)
                                    {
                                        wrapItem.name = showIndex.ToString();
                                        MaxUiIndex = Math.Max(showIndex, MaxUiIndex);
                                        OnWrapItemFresh(wrapItem, showIndex);
                                    }
                                }

                            };
                            WrapContent.Init(DataCount);
                        }
                    }
                    else
                    {
                        if (WrapContent)
                        {
                            WrapContent.SetRange(DataCount);
                            WrapContent.OnItemModify(ModifyType.Update);
                        }
                    }
                }
            }
            else
            {
                DataCount = IdsDataDic.Count;
                if (WrapContent)
                {
                    WrapContent.SetRange(DataCount);
                    WrapContent.OnItemModify(ModifyType.Update);
                }
            }
        }

        protected virtual void InitItemData(string key, Dictionary<string,object> value)
        {
            IdsDataDic[key] = PatchData(value);
        }

        /// <summary>
        /// 对数据进行改造
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual Dictionary<string, object> PatchData(Dictionary<string,object> data)
        {
            return data;
        }


        /// <summary>
        /// 检测数据是否存在，并执行相应事件
        /// </summary>
        /// <returns></returns>
        protected virtual bool DataCheck()
        {
            TotalCount = PageIds.Count;
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(HaveDataAction.WaitExcuteCalls());
            }
            return HaveData;
        }

        protected virtual void ItemInit(GameObject go, int wrapIndex, int realIndex)
        {
            go.name = wrapIndex.ToString();
            go.GetComponentInChildren<UILabel>().text = wrapIndex + "_" + realIndex;
        }

        protected virtual void ResetData()
        {
            DataCount = 0;
            PageIds.Clear();
            IdsDataDic.Clear();
        }

        public virtual void ChangeChatWindow(BaseSocialWrapItem item)
        {
            ChangeTalkTarget(item.OnlyId);
        }

        public virtual void MoToLast()
        {

        }

        public virtual void MoToNext()
        {

        }

        public override void OnDestroy()
        {
            if (ScrollView)
            {
                ScrollView.onMomentumMove = null;
            }
            base.OnDestroy();
        }
    }
}
