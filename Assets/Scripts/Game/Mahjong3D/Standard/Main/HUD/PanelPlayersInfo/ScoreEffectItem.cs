using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ScoreEffectItem : MonoBehaviour
    {
        public Text AddText;
        public Text MiunsText;

        public void Show(long score)
        {
            float value = MahjongUtility.GetShowNumberFloat(score);
            string str = "a" + ParseString(value);
            if (value >= 0)
            {
                MiunsText.ExCompHide();
                AddText.ExCompShow().text = str;
            }
            else
            {
                AddText.ExCompHide();
                MiunsText.ExCompShow().text = str;
            }
            gameObject.SetActive(true);
        }

        private string ParseString(float value)
        {
            string str;
            float score = System.Math.Abs(value);
            int integer = (int)System.Math.Floor(score);
            float dec = (score * 100) - (integer * 100);
            str = integer.ToString();
            if (dec > 0)
            {
                str += "d" + dec;
            }
            return str;
        }

        public void Hide()
        {
            AddText.text = "";
            MiunsText.text = "";
            gameObject.SetActive(false);
        }
    }
}