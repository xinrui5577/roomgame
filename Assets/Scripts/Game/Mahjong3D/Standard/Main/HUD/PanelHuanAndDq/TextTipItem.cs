using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class TextTipItem : MonoBehaviour
    {
        public Text Text;

        public string Content
        {
            set
            {
                if (null != Text)
                {
                    Text.text = value;
                }
            }
        }
    }
}