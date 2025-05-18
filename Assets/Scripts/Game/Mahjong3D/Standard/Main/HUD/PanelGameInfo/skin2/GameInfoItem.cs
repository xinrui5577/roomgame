using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class GameInfoItem : MonoBehaviour
    {
        public Text Txt;
        public LayoutElement Layout;

        public string Context
        {
            set
            {
                Txt.text = value;
            }
        }

        public float MinHeight
        {
            set
            {
                Layout.minHeight = value;
            }
        }
    }
}