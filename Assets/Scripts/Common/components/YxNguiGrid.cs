using UnityEngine;

namespace Assets.Scripts.Common.components
{
    public class YxNguiGrid : UIGrid
    {
        [ContextMenu("Execute")]
        public override void Reposition()
        {
            Makeup();
            base.Reposition();
        }

        public void Makeup()
        {
            if (arrangement != Arrangement.Horizontal) return;
            var size = ParentPanel.width;
            var maxCount = (int)(size / cellWidth);
            maxPerLine = maxCount;
            UpdatePosition(maxCount*cellWidth);
        }

        private void UpdatePosition(float width)
        {
            var lp = transform.localPosition;
            lp.x = -width/2;
            transform.localPosition = lp;
        }

        public void UpdateGrid()
        {
            repositionNow = true;
            Reposition();
        }

        private UIPanel _parentWidget; 
        private UIPanel ParentPanel
        {
            get
            {
                return _parentWidget ?? transform.GetComponentInParent<UIPanel>();
            }
        }
    }
}
