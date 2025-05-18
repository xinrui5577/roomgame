using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahUISignItem : AbsMahSignItem
    {
        public Image Sign;

        public override void SetSprite(Sprite sprite)
        {
            if (sprite == null) return;
            Sign.sprite = sprite;
        }
    }
}
