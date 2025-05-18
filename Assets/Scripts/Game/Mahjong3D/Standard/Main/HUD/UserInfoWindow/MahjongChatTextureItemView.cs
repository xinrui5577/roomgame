using YxFramwork.View;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongChatTextureItemView : ChatTextureItemView
    {
        public UISprite Sprite;

        public void SetState(bool isOn)
        {
            Sprite.color = isOn ? new Color(160 / 255f, 160 / 255f, 160 / 255f) : Color.white;
        }
    }
}
