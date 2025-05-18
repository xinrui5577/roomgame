using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;

namespace Assets.Scripts.Common.Adapters
{
    [RequireComponent(typeof(UIAtlas))]
    public class NguiAtlasAdapter : YxBaseAtlasAdapter
    {
        private UIAtlas _atlas;
        public UIAtlas Atlas {
            get { return _atlas == null ? _atlas = GetComponent<UIAtlas>() : _atlas; }
        }

        public override Texture GetSprite(string spriteName)
        {
            if (Atlas == null) return null;
            var spd = Atlas.GetSprite(spriteName);
            if (spd == null) return null;
            var sx = spd.x;
            var sy = spd.y;
            var sw = spd.width;
            var sh = spd.height;
            var text = (Texture2D)Atlas.texture;
            var f = new Texture2D(sw, sh) { filterMode = text.filterMode, wrapMode = text.wrapMode };
            f.SetPixels(0, 0, sw, sh, text.GetPixels(sx, text.height - sh - sy, sw, sh)); 
            f.LoadImage(f.EncodeToPNG());
            return f;
        }

        public override string[] TextureNames
        {
            get
            {
                if (Atlas == null) return new string[0];
                var sprites = Atlas.GetListOfSprites();
                return sprites == null ? new string[0] : sprites.ToArray();
            }
            protected set { }
        }


        public override YxEUIType UIType
        {
            get { return YxEUIType.Nguid; }
        }
    }
}
