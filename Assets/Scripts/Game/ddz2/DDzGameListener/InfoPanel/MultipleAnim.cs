using UnityEngine;


namespace Assets.Scripts.Game.ddz2.DDzGameListener.InfoPanel
{
    public class MultipleAnim : MonoBehaviour
    {
        public UILabel Label;

        public void PlayMultipleAnim(int mul)
        {
            Label.text = string.Format("x{0}", mul);
            Label.gameObject.SetActive(true);
            var tweens = Label.GetComponents<UITweener>();
            foreach (var tw in tweens)
            {
                tw.ResetToBeginning();
                tw.PlayForward();
            }
        }
    }
}