using System.Collections.Generic;
using Assets.Scripts.Common.UI;
using Assets.Scripts.Common.Utils;
using UnityEngine;

namespace Assets.Scripts.Hall.View.AboutRoomWindows
{
    public class RuleGroupView : NguiView
    {
        /// <summary>
        /// 组名
        /// </summary>
        [Tooltip("组名")]
        public UILabel TabTagLabel;
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
        /// 行Grid
        /// </summary>
        [Tooltip("行Grid")]
        public UIGrid RowViewGrid;
        /// <summary>
        /// 行容器
        /// </summary>
        [Tooltip("行容器")]
        public Transform RowContainer;
        /// <summary>
        /// 行容器
        /// </summary>
        [Tooltip("行容器")]
        public RuleRowView RuleRowViewPrefab;
        [Tooltip("行间的线，盛将特殊需求")]
        public GameObject _Line;

        public float _lineHeight;

        public EGroupLayout GVLayout;
        public enum EGroupLayout
        {
            Left,
            Center,
            Right
        }

        private List<RuleRowView> _curViewList;
        private int _curMaxViewCount;
        private int _viewFreshCount;

        protected override void OnFreshView()
        {
            if (Data == null) return;
            var groupData = Data as GroupData;
            if (groupData == null) return;
            if (TabTagLabel != null)
            {
                var groupName = groupData.Name;
                TabTagLabel.text = string.IsNullOrEmpty(groupName) || groupName == "none" ? "" : groupName;
                if (groupData.NameWidth > 0)
                {
                    TabTagLabel.overflowMethod = UILabel.Overflow.ShrinkContent;
                    TabTagLabel.width = groupData.NameWidth;
                }
                else
                {
                    TabTagLabel.overflowMethod = UILabel.Overflow.ResizeFreely;
                }
            } 
            var listData = groupData.RowDatas;// pair.Value as IList;
            if (listData == null) return;
            var gridTs = RowViewGrid.transform;
            var count = listData.Count;

            if (RuleRowViewPrefab == null) return;
            ReadyRepaint(count);
            for (var i = 0; i < count; i++)
            {
                var rowData = listData[i];
                if (rowData == null) continue;
                Transform rowContainer = null;
                YxWindowUtils.CreateItemParent(RowContainer, ref rowContainer, gridTs);
                var rowView = YxWindowUtils.CreateItem(RuleRowViewPrefab, rowContainer);
                if (_Line != null)
                {
                    var line = rowView.gameObject.AddChild(_Line);
                    line.SetActive(true);
                }
                _curViewList.Add(rowView);
                rowView.UpdateViewWithCallBack(rowData, FreshRowsView);
            }
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
            _curViewList = new List<RuleRowView>();
            _curMaxViewCount = maxCount;
            _viewFreshCount = 0;
        }

        private void FreshRowsView(object obj)
        {
            _viewFreshCount++;
            if (_viewFreshCount < _curMaxViewCount) return;
            if (Data == null) return;
            var groupData = Data as GroupData;
            if (groupData == null) return;
            var count = _curViewList.Count;
            for (var i = 0; i < count; i++)
            {
                var rowView = _curViewList[i];
                var rowBound = rowView.Bounds;
                var rowSize = rowBound.size;
                var rowViewTs = rowView.transform;
                var rowPos = rowViewTs.localPosition;
                switch (GVLayout)
                {
                    case EGroupLayout.Center:
                        rowPos.x = -rowSize.x / 2;
                        break;
                    case EGroupLayout.Left:
                        rowPos.x = 0;
                        break;
                    case EGroupLayout.Right:
                        rowPos.x = -rowSize.x;
                        break;
                }
                rowViewTs.localPosition = rowPos;
            }
            if (groupData.CellHeight > 0)
            {
                RowViewGrid.cellHeight = groupData.CellHeight;
            }
            if (groupData.CellWidth > 0)
            {
                RowViewGrid.cellWidth = groupData.CellWidth;
            }
            RowViewGrid.repositionNow = true;
            RowViewGrid.Reposition();
            var ngui = RowViewGrid.GetComponent<NguiView>();
            if (ngui != null)
            {
                ngui.UpdateWidget();
            }
            UpdateWidget(_uiWidget.width);
            if (CallBack != null) CallBack(null);
        }
    }
}
