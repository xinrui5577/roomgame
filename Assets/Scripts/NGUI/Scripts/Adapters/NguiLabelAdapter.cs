using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;

namespace Assets.Scripts.Common.Adapters
{
    [ExecuteInEditMode] 
    [RequireComponent(typeof(UILabel))] 
    public class NguiLabelAdapter : YxBaseLabelAdapter
    {
        private UILabel _label; 
        public UILabel Label
        {
            get { return _label == null ? _label = GetComponent<UILabel>() : _label; }
        }
        /// <summary>
        /// label效果用
        /// </summary>
        public UILabel EffectLabel;

        protected override void OnText(string content)
        {
            var label = Label;
            if (label == null) return;
            label.text = content;
            UpdateWidth(label);
            UpdateEffectLabel();
        }

        private void UpdateEffectLabel()
        {
            if (EffectLabel == null) { return;}
            EffectLabel.text = Content;
            UpdateWidth(EffectLabel);
        }

        private void UpdateWidth(UILabel label)
        {
            var minW = (int)MaxMin.x;
            var maxW = (int)MaxMin.y;
            var minH = (int)MaxMin.z;
            var maxH = (int)MaxMin.w;
            var oldPos = transform.localPosition;
            var oldMethod = label.overflowMethod;
            label.overflowMethod = UILabel.Overflow.ResizeFreely;
            var flag = true;
            if (minW > 0)
            {
                if (label.localSize.x < minW)
                {
                    label.overflowMethod = UILabel.Overflow.ResizeHeight;
                    label.width = minW;
                    flag = false;
                }
                else
                {
                    flag = false;
                }
            }
            if (maxW > 0)
            {
                if (label.localSize.x > maxW)
                {
                    label.overflowMethod = UILabel.Overflow.ResizeHeight;
                    label.width = maxW;
                    flag = false;
                } 
            }
            if (minH > 0)
            {
                if (label.localSize.y < minH)
                {
                    label.overflowMethod = UILabel.Overflow.ShrinkContent;
                    label.height = minH;
                    flag = false;
                }
                else
                {
                    flag = false;
                }
            }

            if (maxH > 0)
            {
                if (label.localSize.y > maxH)
                {
                    label.height = maxH;
                    label.overflowMethod = UILabel.Overflow.ClampContent;
                    flag = false;
                }
            } 
            if (flag)
            {
                label.overflowMethod = oldMethod;
            }
            transform.localPosition = oldPos;
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

        public override YxEPivot Pivot
        {
            get { return Label == null ? YxEPivot.Center : (YxEPivot)(int)Label.pivot; }
            set {
                var label = Label;
                if (label == null) return;
                label.pivot = (UIWidget.Pivot) (int) value;
            } 
        }

        public override void SetAnchor(GameObject go, int left, int bottom, int right, int top)
        { 
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
            var nlabelAdapter = labelGo as NguiLabelAdapter;
            if (nlabelAdapter == null){return;}
            var style = nlabelAdapter.Label;
            Label.color = style.color;
            Label.fontSize = style.fontSize;
            Label.bitmapFont = style.bitmapFont;
            Label.fontStyle = style.fontStyle;
            Label.gradientTop = style.gradientTop;
            Label.gradientBottom = style.gradientBottom;
            Label.applyGradient = style.applyGradient;
            Label.effectStyle= style.effectStyle;
            Label.effectColor = style.effectColor;
            Label.effectDistance = style.effectDistance;
            Label.spacingX = style.spacingX;
            Label.spacingY = style.spacingY;
            Label.floatSpacingX = style.floatSpacingX;
            Label.floatSpacingY = style.floatSpacingY;
        }

        public override string Value
        {
            get
            {
                return Label == null?"": Label.text;
            }
        }
 
        public override int Width {
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

        public override Color Color {
            get
            { 
                return Label == null? Color.white : Label.color;
            }
            set { Label.color = value; }
        }

        [ContextMenu("Execute")]
        public void Up1()
        {
            Font(Mfont);
        }

        public override YxEUIType UIType
        {
            get { return YxEUIType.Nguid; }
        }

    }
}
