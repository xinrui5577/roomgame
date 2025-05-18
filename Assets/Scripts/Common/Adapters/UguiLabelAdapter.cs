using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;

namespace Assets.Scripts.Common.Adapters
{
    [RequireComponent(typeof(Text),typeof(ContentSizeFitter))]
    public class UguiLabelAdapter : YxBaseLabelAdapter
    {
        private Text _label;
        protected Text Label
        {
            get { return _label == null ? _label = GetComponent<Text>() : _label; }
        }

        protected override void OnText(string content)
        {
            if (Label == null) return;
            Label.text = content;
            UpdateWidth(Label);
        }
         
        private ContentSizeFitter _sizeFitter;

        public ContentSizeFitter SizeFitter
        {
            get
            {
                if (_sizeFitter == null)
                {
                    _sizeFitter = GetComponent<ContentSizeFitter>();
                }
                return _sizeFitter;
            }
        }

        private void UpdateWidth(Text label)
        { 
            var sizeFitter = SizeFitter;
            if (sizeFitter == null) return;
            var minW = (int)MaxMin.x;
            var maxW = (int)MaxMin.y;
            var minH = (int)MaxMin.z;
            var maxH = (int)MaxMin.w;
            if (MaxMin == Vector4.zero){return;}
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            var oldPos = transform.localPosition;
            if (minW > 0)
            {
                if (label.preferredWidth < minW)
                {
                    sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                    var size = label.rectTransform.sizeDelta;
                    size.x = minW;
                    label.rectTransform.sizeDelta = size;
                }
            }
            if (maxW > 0)
            {
                if (label.preferredWidth > maxW)
                {
                    sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                    var size = label.rectTransform.sizeDelta;
                    size.x = maxW;
                    label.rectTransform.sizeDelta = size;
                }
            }
            if (minH > 0)
            {
                if (label.preferredHeight < minH)
                {
                    sizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
                    var size = label.rectTransform.sizeDelta;
                    size.y = minH;
                    label.rectTransform.sizeDelta = size;
                }
            }

            if (maxH > 0)
            {
                if (label.preferredHeight > maxH)
                {
                    sizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
                    var size = label.rectTransform.sizeDelta;
                    size.y = maxH;
                    label.rectTransform.sizeDelta = size;
                }
            }
            transform.localPosition = oldPos;
        }

      
        public override void Font(Font font)
        {
            Mfont = font;
            Label.font = font;
        }

        public override void SetStyle(YxLabelStyle style)
        {
            throw new System.NotImplementedException();
        }

        public override void SetAlignment(YxEAlignment alignment)
        { 
            if (Label == null) return;
            switch (alignment)
            {
                case YxEAlignment.Automatic:
                    break;
                case YxEAlignment.Left:
                    Label.alignment = TextAnchor.MiddleLeft;
                    break;
                case YxEAlignment.Center:
                    Label.alignment = TextAnchor.MiddleCenter;
                    break;
                case YxEAlignment.Right:
                    Label.alignment = TextAnchor.MiddleRight;
                    break;
                case YxEAlignment.Justified:
                    break;
            }
        }

        public override int GetTextWidth(string content)
        {
            return (int)Label.flexibleWidth;//todo
        }

        public override void FreshStyle(YxBaseLabelAdapter labelGo)
        {
        }

        public override string Value
        {
            get { return Label == null?"": Label.text; }
        }

        public override void SetAnchor(GameObject go, int left, int bottom, int right, int top)
        {
        }

        public override int Width
        {
            get
            { 
                return Label == null ? 0 : (int)Label.preferredWidth;
            }
            set { }
        }
        public override int Height
        {
            get
            {
                return Label == null ? 0 : (int)Label.preferredHeight;
            }
            set { }
        }

        private Canvas _canvas;
        protected Canvas AdapterCanvas
        {
            get
            {
                return _canvas == null ? _canvas = GetComponent<Canvas>() : _canvas;
            }
        }
        /// <summary>
        /// 深度
        /// </summary>
        public override int Depth
        {
            get
            {
                var canvas = AdapterCanvas;
                return canvas == null ? 0 : canvas.sortingOrder;
            }
            set
            {
                var canvas = AdapterCanvas;
                if (canvas != null)
                {
                    canvas.overrideSorting = true;
                    canvas.sortingOrder = value;
                }
            }
        }

        private YxEPivot _pivot;
        public override YxEPivot Pivot
        {
            get { return _pivot; }
            set
            {
                var rt = transform as RectTransform;
                if (rt == null) { return; }
                switch (value)
                {
                    case YxEPivot.TopLeft:
                        rt.pivot = new Vector2(0, 1);
                        break;
                    case YxEPivot.Top:
                        rt.pivot = new Vector2(0.5f, 1);
                        break;
                    case YxEPivot.TopRight:
                        rt.pivot = new Vector2(1, 1);
                        break;
                    case YxEPivot.Left:
                        rt.pivot = new Vector2(0, 0.5f);
                        break;
                    case YxEPivot.Center:
                        rt.pivot = new Vector2(0.5f, 0.5f);
                        break;
                    case YxEPivot.Right:
                        rt.pivot = new Vector2(1, 0.5f);
                        break;
                    case YxEPivot.BottomLeft:
                        rt.pivot = new Vector2(0, 0);
                        break;
                    case YxEPivot.Bottom:
                        rt.pivot = new Vector2(0.5f, 0);
                        break;
                    case YxEPivot.BottomRight:
                        rt.pivot = new Vector2(1, 0.5f);
                        break;
                }
                _pivot = value;
            }
        }


        public override Color Color
        {
            get { return Label.color; }
            set { Label.color = value; }
        }

        public override YxEUIType UIType
        {
            get { return YxEUIType.Ugui; }
        }
    }
}
