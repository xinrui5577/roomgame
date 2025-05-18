using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class UIShowMahjong : MonoBehaviour
    {
        public Image Mahjong;

        public void SetActive(bool active, int value = 0)
        {
            gameObject.SetActive(active);
            if (active)
            {
                Mahjong.sprite = GameCenter.Assets.GetMahjongSprite(value);
                Mahjong.SetNativeSize();
            }
        }
    }
}