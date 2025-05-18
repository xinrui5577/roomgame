using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;

namespace Assets.Scripts.Common.Adapters
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class UguiImageAdapter : YxBaseSpriteAdapter
    {
        public Sprite[] Atlases;
        private Image _sprite;
        protected Image UgSprite
        {
            get { return _sprite == null ? _sprite = GetComponent<Image>() : _sprite; }
        }
      
        public override void MakePixelPerfect()
        {
            if (UgSprite == null) { return;}
            UgSprite.SetNativeSize(); 
        }

        public override void SetTexture(Texture texture)
        {
            if (UgSprite == null) { return; }
            var t2d = texture as Texture2D;
            if (t2d == null) { return;}
            UgSprite.sprite = Sprite.Create(t2d, new Rect(), Vector2.zero);
        }

        public override Texture GetTexture()
        {
            return UgSprite == null ? null : UgSprite.mainTexture;
        }

        public override float FillAmount { get; set; }

        public override YxEUIType UIType
        {
            get { return YxEUIType.Ugui; }
        }

        public override void SetAnchor(GameObject go, int left, int bottom, int right, int top)
        {
            var fitter = go.GetComponent<Text>();
            if (fitter != null)
            {
//                fitter.preferredWidth
            }
            var target = go.transform as RectTransform;
            if (target == null) { return; }
            var rt = transform as RectTransform;
            if (rt == null) { return; }
            rt.pivot = target.pivot;
            rt.anchoredPosition = target.anchoredPosition;
            rt.position = target.position;
            var rtLpos = rt.localPosition;
            rtLpos.x += (right + left) / 2f;
            rtLpos.y += (top + bottom) / 2f;
            rt.localPosition = rtLpos;
            var tgSize = target.sizeDelta;
            tgSize.x += right - left;
            tgSize.y += top - bottom;
            rt.sizeDelta = tgSize;
        }

        public override int Width
        {
            get
            {
                var ts = (RectTransform) transform;
                return ts == null ? 0 : (int)ts.sizeDelta.x;
            }
            set
            {
                var ts = (RectTransform)transform;
                if(ts == null)return;
                var size = ts.sizeDelta;
                size.x = value;
                ts.sizeDelta = size;
            }
        }
        public override int Height
        {
            get
            {
                var ts = (RectTransform)transform;
                return ts == null ? 0 : (int)ts.sizeDelta.y;
            }
            set
            {
                var ts = (RectTransform)transform;
                if (ts == null) return;
                var size = ts.sizeDelta;
                size.y = value;
                ts.sizeDelta = size;
            }
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
        /// …Ó∂»
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
        public override Color Color
        {
            get { return UgSprite == null ? Color.white : UgSprite.color; }
            set { UgSprite.color = value; }
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
        public override void SetSpriteName(string spriteName)
        {
            foreach (var sprite in Atlases)
            {
                if (sprite.name == spriteName)
                {
                    UgSprite.sprite = sprite;
                    return;
                }
            }
        }

        public override void SetAtlas(YxBaseAtlasAdapter atlas)
        {
        }

        public override void EnableSprite(bool enable)
        {
            var s = UgSprite;
            if (s != null)
            {
                s.enabled = enable;
            }
        }
    }
}
