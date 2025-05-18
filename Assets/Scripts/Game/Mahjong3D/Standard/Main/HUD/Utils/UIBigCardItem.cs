using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class UIBigCardItem : MonoBehaviour
    {
        public Image Card;

        public void SetCard(int card)
        {
            gameObject.SetActive(true);
            Card.sprite = GameCenter.Assets.GetMahjongSprite(card);
            Card.SetNativeSize();
        }
    }
}