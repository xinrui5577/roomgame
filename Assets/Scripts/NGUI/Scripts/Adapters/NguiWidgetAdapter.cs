using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;

namespace Assets.Scripts.Common.Adapters
{
    public class NguiWidgetAdapter : YxBaseWidgetAdapter
    { 
        private UIWidget _widget;
        protected UIWidget Widget
        {
            get
            {
                return _widget == null ? _widget = GetComponent<UIWidget>() : _widget;
            }
        }

        public override YxEUIType UIType
        {
            get { return YxEUIType.Nguid; }
        }

        public override void SetAnchor(GameObject go, int left, int bottom, int right, int top)
        {
            if (Widget == null) { return; }
            Widget.updateAnchors = UIRect.AnchorUpdate.OnUpdate;
            Widget.SetAnchor(go, left, bottom, right, top);
        }

        public override int Width
        {
            get
            {
                return Widget == null ? 0 : Widget.width;
            }
            set {
                if (Widget != null)
                {
                    Widget.width = value;
                }
            }
        }
        public override int Height
        {
            get
            {
                return Widget == null ? 0 : Widget.height;
            }
            set
            {
                if (Widget != null)
                {
                    Widget.height = value;
                }
            }
        }
        public override int Depth
        {
            get
            {
                return Widget == null ? 0 : Widget.depth;
            }
            set
            {
                if (Widget != null)
                {
                    Widget.depth = value;
                }
            }
        }
        public override Color Color
        {
            get
            {
                return Widget == null ? Color.white : Widget.color;
            }
            set
            {
                if (Widget != null)
                {
                    Widget.color = value;
                }
            }
        }
        public override YxEPivot Pivot {
            get
            {
                return Widget == null ? YxEPivot.Center : (YxEPivot)(int)Widget.pivot;
            }
            set
            {
                if (Widget != null)
                {
                    Widget.pivot = (UIWidget.Pivot)(int)value;
                }
            }
        }
    }
}
