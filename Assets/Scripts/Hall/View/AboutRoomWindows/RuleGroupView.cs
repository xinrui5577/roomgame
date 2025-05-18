using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Models.CreateRoomRules;
using Assets.Scripts.Common.UI;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.AboutRoomWindows
{
    public class RuleGroupView : FreshLayoutBaseView
    {
        /// <summary>
        /// 组名
        /// </summary>
        [Tooltip("组名")]
        public UILabel TabTagLabel;   
         
        /// <summary>
        /// 行Grid
        /// </summary>
        [Tooltip("行Grid")]
        public UIGrid RowViewGrid;
        /// <summary>
        /// 行容器
        /// </summary>
//        [Tooltip("行容器")]
//        public Transform RowContainer;
        /// <summary>
        /// 行容器
        /// </summary>
        [Tooltip("行容器")]
        public RuleRowView RuleRowViewPrefab;
        [Tooltip("组的线")]
        public GameObject Line;

//        private readonly List<RuleRowView> _curViewList = new List<RuleRowView>();

        private float _gridCellWidth;
        private float _gridCellHeight;
        private float _nameDefaultX;
        private float _nameDefaultY;
        protected override void OnAwake()
        {
            InitStateTotal = 2;
            CheckIsStart = true;
            _gridCellWidth = RowViewGrid.cellWidth;
            _gridCellHeight = RowViewGrid.cellHeight;
            if (TabTagLabel != null)
            {
                var nameLabelPos = TabTagLabel.transform.localPosition;
                _nameDefaultX = nameLabelPos.x;
                _nameDefaultY = nameLabelPos.y;
            }
            base.OnAwake();
        }

        protected override void OnStart()
        {
            var widgetAdapter = GetWidgetAdapter();
            if (widgetAdapter == null)
            {
                gameObject.AddComponent<NguiWidgetAdapter>();
            }
            base.OnStart();
        }

        protected override void InitViews()
        {
            var groupData = Data as GroupData;
            if (groupData == null || groupData.Parent.ViewIsHide(groupData.Id))
            {
                Hide();
                CallBack(IdCode);
                return;
            } 
            gameObject.SetActive(true);
            InitGroupNameLabel(groupData);
            
            if (RuleRowViewPrefab == null)
            {
                Hide();
                CallBack(IdCode);
                return;
            }
            var listData = groupData.RowDatas;// pair.Value as IList;
            if (listData == null)
            {
                Hide();
                CallBack(IdCode);
                return;
            }
            var gridTs = RowViewGrid.transform;
            gridTs.gameObject.SetActive(true);
            var dataCount = listData.Count;
            var oldCount = BufferViewCount;
            var minCount = Mathf.Min(oldCount, dataCount);
            UpdateGrid((int)groupData.CellWidth, (int)groupData.CellHeight);
            var i = FreshBufferView(0, minCount, listData);
            i = HideOdd(i, oldCount);
            CreateNewView(i, dataCount, listData, gridTs);
        }

        private void InitGroupNameLabel(GroupData groupData)
        {
            if (TabTagLabel == null) { return;}
            var gname = groupData.Name;
            var groupName = gname;
            var gnameWidth = groupData.NameWidth;
            var nameX = groupData.NameX;
            var nameY = groupData.NameY;
            TabTagLabel.text = string.IsNullOrEmpty(groupName) || groupName == "none" ? "" : groupName;
            if (gnameWidth > 0)
            {
                TabTagLabel.overflowMethod = UILabel.Overflow.ShrinkContent;
                TabTagLabel.width = gnameWidth;
            }
            else
            {
                TabTagLabel.overflowMethod = UILabel.Overflow.ResizeFreely;
            }
            var labelTs = TabTagLabel.transform;
            var pos = labelTs.localPosition;
            pos.x = float.IsNaN(nameX) ? _nameDefaultX : nameX;
            pos.y = float.IsNaN(nameY) ? _nameDefaultY : nameY;
            labelTs.localPosition = pos;
        }

        protected override void OnFreshLayout()
        {
            var groupData = Data as GroupData;
            if (groupData == null)
            {
                CallBack(IdCode);
                return;
            }
            var count = BufferViewCount;
            var totalRow = 0;
            RuleRowView rowView = null;
            var cellHeight = RowViewGrid.cellHeight;
            var maxRight = 0f;
            for (var i = 0; i < count; i++)
            {
                var view = GetBufferView(i) as RuleRowView;
                if (view == null)
                {
                    continue;
                }
                if (!view.IsShow() || !view.HasChildView())
                {
                    view.Hide();
                    continue;
                }
                rowView = view;
                totalRow++;
                var rowViewTs = rowView.transform;
                var rowPos = rowViewTs.localPosition;
                rowPos.x = 0;
                rowViewTs.localPosition = rowPos;
                var curW = rowView.Bounds.size.x;
                if (curW > maxRight)
                {
                    maxRight = curW;
                }
                view.DrawLine(cellHeight);
            }
            if (rowView != null && Line!=null)
            {
                rowView.HideLine();
            }
            var gridPos = RowViewGrid.transform.localPosition;
            var height = -(int)gridPos.y;
            if (totalRow > 0)
            {
                Show();
                
                RowViewGrid.repositionNow = true;
                RowViewGrid.Reposition();
                height += (int)(cellHeight * totalRow);
            }
            else
            {
                Hide();
            }
            var width = gridPos.x + maxRight;
            UpdateWidget(width, height);
            DrawLine(height);
            CallBack(IdCode);
        }

        protected override YxView CreateView<T>(T data, Transform pts, Vector3 pos = default(Vector3))
        {
            var itemRowData = data as ItemRowData;
            if (itemRowData != null)
            {
                itemRowData.Height = RowViewGrid.cellHeight;
            }
            var view = YxWindowUtils.CreateItem(RuleRowViewPrefab, pts, pos);
            return view;
        }

        private int UpdateGrid(int cellWidth, int cellHeight)
        {
            RowViewGrid.cellWidth = cellWidth > 0 ? cellWidth : _gridCellWidth;
            RowViewGrid.cellHeight = cellHeight > 0 ? cellHeight : _gridCellHeight;
            return (int)RowViewGrid.cellHeight;
        }

        private void DrawLine(int y)
        {
            if (Line == null) { return;}
            var pos = new Vector3(0,-y,0);
            Line.transform.localPosition = pos;
        }
    }
}
