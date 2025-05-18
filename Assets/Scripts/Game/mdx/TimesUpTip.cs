using UnityEngine;

namespace Assets.Scripts.Game.mdx
{
    public class TimesUpTip : MonoBehaviour
    {
       
        public UILabel TipLabel;

        public string TipFormat;

        public void ShowTimesUpTip(int num)
        {
            gameObject.SetActive(true);
            TipLabel.text = string.Format(TipFormat, num);
            TipLabel.gameObject.SetActive(true);
            var tweens = GetComponentsInChildren<UITweener>();
            int len = tweens.Length;
            for (int i = 0; i < len; i++)
            {
                var tween = tweens[i];
                tween.ResetToBeginning();
                tween.PlayForward();
            }
        }

        public void OnFinish()
        {
            TipLabel.gameObject.SetActive(false);
        }

    }
}
