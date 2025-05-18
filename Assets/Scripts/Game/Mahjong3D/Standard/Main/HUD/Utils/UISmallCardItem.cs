using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class UISmallCardItem : MonoBehaviour
    {
        public Image Mahjong;

        public void SetCard(int value)
        {
            gameObject.SetActive(true);
            Mahjong.sprite = GameCenter.Assets.GetMahjongSmallSprite(value);
            Mahjong.SetNativeSize();
        }
    }
}
