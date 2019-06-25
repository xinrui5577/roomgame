using System.Collections.Generic;
using Assets.Scripts.Common.UI;
using Assets.Scripts.Common.Utils;
using UnityEngine;

namespace Assets.Scripts.Hall.View.AboutRoomWindows
{
    public class RuleRowView : NguiView
    {
        /// <summary>
        /// 多选预制
        /// </summary>
        [Tooltip("多选预制")]
        public NguiCheckBox CheckBoxPerfab;
        /// <summary>
        /// 单选预制
        /// </summary>
        [Tooltip("单选预制")]
        public NguiCheckBox RadioPerfab;
        /// <summary>
        /// 按钮预制
        /// </summary>
        [Tooltip("按钮预制")]
        public NguiCheckBox ButtonPerfab;
        /// <summary>
        /// label预制
        /// </summary>
        [Tooltip("label预制")]
        public NguiCRLabel LabelPerfab;
        /// <summary>
        /// 范围预制
        /// </summary>
        [Tooltip("范围预制")]
        public NguiRange RangePerfab;

        [Tooltip("行容器")]
        public Transform RowContainer;

        private List<NguiCRComponent> _curViewList;
        private int _curMaxViewCount;
        private int _viewFreshCount;
        private Transform _rowContainer;

        protected override void OnAwake()
        {
            InitStateTotal = 2;
        }

        protected override void OnFreshView()
        {
            var rowDatas = GetData<RowData>();
            if (rowDatas == null) return;
            YxWindowUtils.CreateItemParent(RowContainer, ref _rowContainer,transform);
            var rowItems = rowDatas.Items;
            var count = rowItems.Count;
            ReadyRepaint(count);
            for (var i = 0; i < count; i++)
            {
                var rowInfo = rowItems[i]; 
                var key = rowInfo.Key;
                if (NeedHide(rowInfo))
                {
                    FreshRowView(null);
                    continue;
                }
                bool state;
                var view = CreateRuleItemView(rowInfo,out state);

                if (view == null) return; 
                view.name = key;
                _curViewList.Add(view);
                view.gameObject.SetActive(false);
                var ts = view.transform;
                ts.parent = _rowContainer;
                view.UpdateViewWithCallBack(rowInfo, FreshRowView);
            }
        }

        protected NguiCRComponent CreateRuleItemView(ItemData rowInfo,out bool state)
        {
            NguiCRComponent view = null;
            state = false;
            var ruleInfo = rowInfo.Parent;
            var id = rowInfo.Id;
            switch (rowInfo.Type)
            {
                case RuleItemType.checkbox:
                    {
                        view = Instantiate(CheckBoxPerfab);
                        rowInfo.View = view;
                        view.Id = id;
                        rowInfo.State = RuleInfo.GetItemState(ruleInfo.CurTabItemId, rowInfo.Id, rowInfo.State);
                    }
                    break;
                case RuleItemType.radio:
                    {
                        view = Instantiate(RadioPerfab);
                        rowInfo.View = view;
                        view.Id = id;
                        rowInfo.State = RuleInfo.GetItemState(ruleInfo.CurTabItemId, rowInfo.Id, rowInfo.State);
                    }
                    break;
                case RuleItemType.button:
                    {
                        view = Instantiate(ButtonPerfab);
                        rowInfo.View = view;
                        view.Id = id;
                        rowInfo.State = ruleInfo.CurTabItemId == id;//ruleInfo.TabDefaultIndex>-1 ? i == ruleInfo.TabDefaultIndex: 
                    }
                    break;
                case RuleItemType.range:
                    view = Instantiate(RangePerfab);
                    rowInfo.View = view;
                    view.Id = id;
                    break; 
                case RuleItemType.label:
                    view = Instantiate(LabelPerfab);
                    rowInfo.View = view;
                    view.Id = id;
                    break; 
            } 
            return view;
        }

        private static bool NeedHide(ItemData itemData)
        {
            var parent = itemData.Parent;
            if (parent == null) return false;
            var itemId = itemData.Id;
            var tabs = parent.Tabs;
            if (tabs == null) return false;
            var tabId = parent.CurTabItemId;
            if (!tabs.ContainsKey(tabId)) return false;
            var hides = parent.Tabs[tabId];
            foreach (var hideId in hides)
            {
                if (itemId == hideId)
                {
                    return true;
                }
            }
            return false;
        }

        private void ReadyRepaint(int maxCount)
        {
            _curViewList = new List<NguiCRComponent>();
            _curMaxViewCount = maxCount;
            _viewFreshCount = 0;
        }

        private void FreshRowView(object obj)
        {
            _viewFreshCount++;
            if (_viewFreshCount < _curMaxViewCount) return;
            var rowDatas = GetData<RowData>();
            if (rowDatas == null) return;
            var count = _curViewList.Count;
            var curlpos = Vector3.zero; 
            for (var i = 0; i < count; i++)
            {
                var view = _curViewList[i];
                if(view==null)continue;
                var ts = view.transform;
                view.gameObject.SetActive(true);
                ts.localScale = Vector3.one;
                ts.localPosition = curlpos;
                var size = view.BoundSize;
                curlpos.x = curlpos.x + size.x + rowDatas.Spacing; 
            }
            var ngui = RowContainer.GetComponent<NguiView>();
            if (ngui != null)
            {
                ngui.UpdateWidget();
            }
            if (CallBack != null) CallBack(null);
        }
    }
}
