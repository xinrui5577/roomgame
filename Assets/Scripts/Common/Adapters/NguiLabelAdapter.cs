using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.Adapters
{
    [ExecuteInEditMode] 
    [RequireComponent(typeof(UILabel))] 
    public class NguiLabelAdapter : YxBaseLabelAdapter
    {
        private UILabel _label; 

        public UILabel GetLabe()
        {
            return _label?? (_label=GetComponent<UILabel>());
        } 

        protected override void OnText(string content)
        {
            var label = GetLabe();
            if (label == null) return; 
            label.text = content;
            UpdateWidth(label);
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
                    label.overflowMethod = UILabel.Overflow.ResizeHeight;
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
            _label.trueTypeFont = font;
        }

        public override int GetTextWidth(string content)
        {
            var oldText = _label.text;
            var oldFlow = _label.overflowMethod;
            _label.text = content;
            _label.overflowMethod = UILabel.Overflow.ResizeFreely;
            _label.UpdateNGUIText();
            var w = _label.width;
            _label.text = oldText;
            _label.overflowMethod = oldFlow;
            return w;
        }

        public override int Width {
            get { return _label.width; }
            set { _label.width = value; }
        }
        public override int Height
        {
            get { return _label.height; }
            set { _label.height = value; }
        }

        public override Color Color {
            get { return _label.color; }
            set { _label.color = value; }
        }

        [ContextMenu("Execute")]
        public void Up1()
        {
            Font(Mfont);
        }
         
    }
}
