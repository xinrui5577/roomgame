using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ShowXjfdCard : MonoBehaviour
    {
        public Image CardValue;

        public void SetCardData(int value)
        {
            if (value != 0)
            {
                Sprite sp = GetSpriteByValue(value); ;
                if (sp == null)
                {
                    sp = GameCenter.Assets.GetMahjongSprite(value);
                }
                CardValue.sprite = sp;
                CardValue.SetNativeSize();
                CardValue.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            }
        }

        private Sprite GetSpriteByValue(int value)
        {
            PanelShowXjfdList panel = GameCenter.Hud.GetPanel<PanelShowXjfdList>();
            Sprite temp;
            switch (value)
            {
                case 17:
                    temp = panel.CardsSprite[0];
                    break;
                case 25:
                    temp = panel.CardsSprite[1];
                    break;
                case 33:
                    temp = panel.CardsSprite[2];
                    break;
                case 41:
                    temp = panel.CardsSprite[3];
                    break;
                case 49:
                    temp = panel.CardsSprite[4];
                    break;
                case 57:
                    temp = panel.CardsSprite[5];
                    break;
                case 65:
                    temp = panel.CardsSprite[6];
                    break;
                case 68:
                    temp = panel.CardsSprite[7];
                    break;
                case 71:
                    temp = panel.CardsSprite[8];
                    break;
                case 74:
                    temp = panel.CardsSprite[9];
                    break;
                case 81:
                    temp = panel.CardsSprite[10];
                    break;
                case 84:
                    temp = panel.CardsSprite[11];
                    break;
                case 87:
                    temp = panel.CardsSprite[12];
                    break;
                default:
                    temp = null;
                    break;
            }
            return temp;
        }
    }
}