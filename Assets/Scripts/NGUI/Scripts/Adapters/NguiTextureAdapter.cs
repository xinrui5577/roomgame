using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;

namespace Assets.Scripts.Common.Adapters
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(UITexture))]
    public class NguiTextureAdapter : YxBaseTextureAdapter
    {
        private UITexture _texture;
        public UITexture Texture
        {
            get { return _texture == null ? _texture = GetComponent<UITexture>() : _texture; }
        }

        public override void MakePixelPerfect()
        {
            if (Texture == null) { return;}
            Texture.MakePixelPerfect();
        }

        public override void SetTexture(Texture texture)
        {
            if (Texture == null) return;
            Texture.mainTexture = texture;
            if(Snap)Texture.MakePixelPerfect();
        }

        public override Texture GetTexture()
        {
            return Texture == null ? null : Texture.mainTexture;
        }

        public override YxEUIType UIType
        {
            get { return YxEUIType.Nguid; }
        }

        public override void SetAnchor(GameObject go, int left, int bottom, int right, int top)
        { 
            if (Texture == null) { return; }
            Texture.updateAnchors = UIRect.AnchorUpdate.OnUpdate;
            Texture.SetAnchor(go, left, bottom, right, top);
        }

        public override int Width
        {
            get { return Texture == null ? 0 : Texture.width;}
            set
            {
                if (Texture == null) { return; }
                Texture.width = value;
            }
        }
        public override int Height
        {
            get { return Texture == null ? 0 : Texture.height;}
            set
            {
                if (Texture == null) { return; }
                Texture.height = value;
            }
        }

        public override int Depth
        {
            get { return Texture == null ? 0 : Texture.depth; }
            set
            {
                if (Texture == null) { return; }
                Texture.depth = value;
            }
        }

        public override Color Color {
            get { return Texture == null ? Color.white : Texture.color; }
            set { Texture.color = value; }
        }

        public override YxEPivot Pivot
        {
            get { return Texture == null ? YxEPivot.Center : (YxEPivot)(int)Texture.pivot; }
            set
            {
                if (Texture == null) { return; }
                Texture.pivot = (UIWidget.Pivot)(int)value;
            }
        }
        public override float FillAmount
        {
            get { return Texture == null ? 1 : Texture.fillAmount; }
            set { Texture.fillAmount = value; }
        }
    }
}
