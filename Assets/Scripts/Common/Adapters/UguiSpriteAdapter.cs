using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Common.Adapters
{
    [RequireComponent(typeof(Image))]
    public class UguiSpriteAdapter : UguiImageAdapter
    {
        public Sprite[] Sprites;

        private readonly Dictionary<string,Sprite> _sprites = new Dictionary<string, Sprite>();
       
        void Awake()
        {
            var count = Sprites.Length;

            for (var i = 0; i < count; i++)
            {
                var sprite = Sprites[i];
                if (sprite == null) { continue;}
                _sprites[sprite.name] = sprite;
            }
        }

        public override void SetSpriteName(string spriteName)
        {
            UgSprite.sprite = GetSprite(spriteName);
        }

        public Sprite GetSprite(string spriteName)
        {
            return _sprites.ContainsKey(spriteName) ? _sprites[spriteName] : null;
        }
    } 
}
