using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;

namespace Assets.Scripts.Common.Adapters
{
    /// <inheritdoc />
    /// <summary>
    /// ·­¹öµÄÊý×Ö
    /// </summary>
    public class NguiTumbleLabelAdapter : YxBaseNumberAdapter
    {
        private UILabel _label;
        public UILabel Label
        {
            get { return _label == null ? _label = GetComponent<UILabel>() : _label; }
        }
        public override YxEUIType UIType
        {
            get { return YxEUIType.Nguid; }
        }

        public override void SetAnchor(GameObject go, int left, int bottom, int right, int top)
        {
        }

        protected override void OnText(string content)
        {
            var label = Label;
            if (label == null) return;
            label.text = content;
        }

        public override void Font(Font font)
        {
            Mfont = font;
            Label.trueTypeFont = font;
        }

        public override void SetStyle(YxLabelStyle style)
        {
        }

        public override void SetAlignment(YxEAlignment yxEAlignment)
        {
            var label = Label;
            if (label == null) return;
            label.alignment = (NGUIText.Alignment)(int)yxEAlignment;
        }

        public override int GetTextWidth(string content)
        {
            var label = Label;
            if (label == null) return 0;
            var oldText = label.text;
            var oldFlow = label.overflowMethod;
            label.text = content;
            label.overflowMethod = UILabel.Overflow.ResizeFreely;
            label.UpdateNGUIText();
            var w = label.width;
            label.text = oldText;
            label.overflowMethod = oldFlow;
            return w;
        }

        public override void FreshStyle(YxBaseLabelAdapter labelGo)
        {
        }

        public override YxEPivot Pivot
        {
            get { return Label == null ? YxEPivot.Center : (YxEPivot)(int)Label.pivot; }
            set
            {
                var label = Label;
                if (label == null) return;
                label.pivot = (UIWidget.Pivot)(int)value;
            }
        }

        public override string Value
        {
            get { return Label == null ? "" : Label.text; }
        }

        public override int Width
        {
            get
            {
                return Label == null ? 0 : Label.width;
            }
            set { Label.width = value; }
        }
        public override int Height
        {
            get
            {
                return Label == null ? 0 : Label.height;
            }
            set { Label.height = value; }
        }

        public override int Depth
        {
            get
            {
                return Label == null ? 0 : Label.depth;
            }
            set
            {
                if (Label != null)
                {
                    Label.depth = value;
                }
            }
        }

        public override Color Color
        {
            get
            {
                return Label == null ? Color.white : Label.color;
            }
            set { Label.color = value; }
        }
    }
}
