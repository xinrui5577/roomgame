using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class GameInfoLoopType : MonoBehaviour
    {
        public Sprite[] Sprites;
        public Image Title;
        public Text Record;

        public string SetRecord
        {
            set
            {
                Record.text = value;
            }
        }

        public int SetloopType
        {
            set
            {
                Title.sprite = Sprites[value];
            }
        }
    }
}