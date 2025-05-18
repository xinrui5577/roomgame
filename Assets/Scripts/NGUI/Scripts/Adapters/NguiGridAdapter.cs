using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;

namespace Assets.Scripts.Common.Adapters
{
    [RequireComponent(typeof(UIGrid))]
    public class NguiGridAdapter : YxBaseGridAdapter
    {
        public bool RepositionNow;
        public UIWidget FrameWidget;
        private UIGrid _grid;
        protected UIGrid Grid
        {
            get { return _grid == null ? _grid = GetComponent<UIGrid>() : _grid; }
        }
        public override YxEUIType UIType
        {
            get { return YxEUIType.Nguid; }
        }

        private void Awake()
        {
            Grid.repositionNow = RepositionNow || FrameWidget != null;
        }

        [ContextMenu("Execute")]
        public override void Reposition()
        {
            if (Grid == null) { return;}
            Grid.repositionNow = true;
            Grid.Reposition();
            if (Widget != null)
            {
                var gridTs = Grid.transform;
                var bound = NGUIMath.CalculateRelativeWidgetBounds(gridTs, gridTs);
                var size = bound.size;
                Widget.Width = (int)size.x;
                Widget.Height = (int)size.y;
            }
        }

        public override List<Transform> GetChildList()
        {
            return Grid == null ? new List<Transform>() : Grid.GetChildList();
        }

        public override void AddChild(Transform trans)
        {
            var grid = Grid;
            if (grid == null) return;
            grid.AddChild(trans);
        }

        private Vector2 _lastSize = Vector2.zero;
        protected void LateUpdate()
        {
            if (FrameWidget == null) { return; }
            var curSize = FrameWidget.localSize;
            if (_lastSize == curSize) return;
            _lastSize = FrameWidget.localSize;
            var grid = Grid;
            switch (grid.arrangement)
            {
                case UIGrid.Arrangement.Horizontal:
                    {
                        var cellWidth = grid.cellWidth;
                        grid.pivot = UIWidget.Pivot.TopLeft;
                        grid.maxPerLine = (int)(curSize.x / cellWidth);
                        grid.Reposition();
                        var localPos = grid.transform.localPosition;
                        localPos.x = -cellWidth * grid.maxPerLine / 2 + cellWidth / 2;
                        grid.transform.localPosition = localPos;
                    }
                    break;
                case UIGrid.Arrangement.Vertical:
                    {
                        grid.pivot = UIWidget.Pivot.TopLeft;
                        var cellHeight = grid.cellHeight;
                        grid.maxPerLine = (int)(curSize.y / cellHeight);
                        grid.Reposition();
                        var localPos = grid.transform.localPosition;
                        localPos.y = cellHeight * grid.maxPerLine / 2 + cellHeight / 2;
                        grid.transform.localPosition = localPos;
                    }
                    break; 
            }
        }
    }
}
