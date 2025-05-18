using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class UiLaiziLabel : MonoBehaviour
    {
        public Image LaiziImage;

        public int Value
        {
            set
            {
                gameObject.SetActive(true);
                LaiziImage.sprite = GameCenter.Assets.GetMahjongSprite(value);
                LaiziImage.SetNativeSize();
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}