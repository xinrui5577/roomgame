using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using UnityEngine;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base
{
    public class BaseSocialSelectWrapItem : BaseSocialWrapHeadItem
    {
        /// <summary>
        /// 选中事件
        /// </summary>
        /// <param name="onlyId">Item onlyId</param>
        /// <param name="selectValue">选中值</param>
        public delegate void SelectAction(string onlyId, bool selectValue);
        /// <summary>
        /// 选择状态变化
        /// </summary>
        public SelectAction OnSelectStateChange;
        /// <summary>
        /// 选中状态(bool)
        /// </summary>
        public static string KeySelectStatus = "selectStatus";
        /// <summary>
        /// 选中类型(SelectItemState)
        /// </summary>
        public static string KeySelectType = "selectType";
        [Tooltip("选择类型变化事件(单选或多选)")]
        public List<EventDelegate> TypeChangeAction = new List<EventDelegate>();
        [Tooltip("选择状态变化事件（是否被选中）")]
        public List<EventDelegate> StatusChangeAction = new List<EventDelegate>();
        /// <summary>
        /// 当前状态选中状态
        /// </summary>
        public bool SelectStatus
        {
            get
            {
                return _selectValue;
            }
        }
        /// <summary>
        /// 当前状态选中值
        /// </summary>
        private bool _selectValue;

        protected int SelectType;

        public override void OnClick()
        {
            _selectValue = !_selectValue;
            if (Data is Dictionary<string,object>)
            {
                GetData<Dictionary<string, object>>()[KeySelectStatus] = _selectValue;
            }
            FreshSelectAction(true);
        }

        protected override void DealFreshData()
        {
            base.DealFreshData();
            FreshSelectState();
        }

        protected virtual void FreshSelectState()
        {
            ParseDicData.TryGetValueWitheKey(out _selectValue, KeySelectStatus);
            ParseDicData.TryGetValueWitheKey(out SelectType, KeySelectType);
            FreshTypeAction();
            FreshSelectAction();
        }

        private void FreshTypeAction()
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(TypeChangeAction.WaitExcuteCalls());
            }
        }
        /// <summary>
        /// 执行刷新时是否需要调用回调事件
        /// </summary>
        /// <param name="needCall"></param>
        private void FreshSelectAction(bool needCall=false)
        {
            StartCoroutine(StatusChangeAction.WaitExcuteCalls());
            if (needCall&& OnSelectStateChange!=null)
            {
                OnSelectStateChange(OnlyId, _selectValue);
            }
        }
    }
    /// <summary>
    /// 选中状态
    /// </summary>
    public enum SelectItemState
    {
        SingleSelect=1,                       //单选模式
        MulSelect=2                           //多选模式
    }

}
