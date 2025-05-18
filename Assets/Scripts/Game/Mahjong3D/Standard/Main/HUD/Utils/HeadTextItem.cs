using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class HeadTextItem : MonoBehaviour
    {
        public Text Score;

        public void SetContext(string context)
        {
            gameObject.SetActive(true);
            Score.text = context;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}