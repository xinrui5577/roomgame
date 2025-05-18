using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;

namespace Assets.Scripts.Common.Adapters
{
    [RequireComponent(typeof(UISprite))]
    public class NguiSpriteAdapter : YxBaseSpriteAdapter
    {
        private UISprite _sprite;
        protected UISprite Sprite
        {
            get { return _sprite == null ? _sprite = GetComponent<UISprite>() : _sprite; }
        }
 
        public override YxEUIType UIType
        {
            get { return YxEUIType.Nguid; }
        }
         
        public override int Width
        {
            get { return Sprite == null?0: Sprite.width; }
            set { if(Sprite != null) { Sprite.width = value;} }
        }

        public override int Height
        {
            get { return Sprite == null ? 0 : Sprite.height; }
            set { if (Sprite != null) { Sprite.height = value; } }
        }
        public override int Depth
        {
            get { return Sprite == null ? 0 : Sprite.depth; }
            set { if (Sprite != null) { Sprite.depth = value; } }
        }

        public override Color Color
        {
            get { return Sprite == null ? Color.white : Sprite.color; }
            set { Sprite.color = value; }
        }

        public override YxEPivot Pivot { get; set; }

        public override void MakePixelPerfect()
        {
            if (Sprite == null) { return;}
            Sprite.MakePixelPerfect();
        }

        public override void SetTexture(Texture texture)
        {
            if (Sprite == null) { return; }
            Sprite.mainTexture = texture;
        }

        public override Texture GetTexture()
        {
            return Sprite == null?null: Sprite.mainTexture;
        }

        public override float FillAmount
        {
            get { return Sprite == null ? 1 : Sprite.fillAmount; }
            set { Sprite.fillAmount = value; }
        }
        public override void SetSpriteName(string spriteName)
        {
            if (Sprite == null) { return; }
            Sprite.spriteName = spriteName;
        }

        public override void SetAtlas(YxBaseAtlasAdapter atlas)
        {
            var nguiAtlas = atlas as NguiAtlasAdapter;
            if (nguiAtlas == null) return;
            if (Sprite == null) { return; }
            Sprite.atlas = nguiAtlas.Atlas;
        }

        public override void EnableSprite(bool enable)
        {
            var s = Sprite;
            if (s != null)
            {
                s.enabled = enable;
            }
        }


        public override void SetAnchor(GameObject go, int left, int bottom, int right, int top)
        {
            var sprite = Sprite;
            if (sprite == null) { return; }
            sprite.updateAnchors = UIRect.AnchorUpdate.OnUpdate;
            sprite.SetAnchor(go, left, bottom, right, top);
        }
    }
}
