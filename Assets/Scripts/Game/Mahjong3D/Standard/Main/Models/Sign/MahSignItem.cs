using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahSignItem : AbsMahSignItem
    {
        public SpriteRenderer Sign;

        public override void SetSprite(Sprite sprite)
        {
            if (sprite == null) return;
            Sign.sprite = sprite;
        }
    }
}