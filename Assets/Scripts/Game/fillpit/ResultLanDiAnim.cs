using UnityEngine;

namespace Assets.Scripts.Game.fillpit
{
    public class ResultLanDiAnim : MonoBehaviour
    {
        void ResetAnimation()
        {
            var tweens = GetComponentsInChildren<UITweener>();
            var len = tweens.Length;
            for (int i = 0; i < len; i++)
            {
                var tween = tweens[i];
                tween.ResetToBeginning();
                tween.enabled = false;
            }
        }

        void PlayForward()
        {
            var tweens = GetComponentsInChildren<UITweener>();
            var len = tweens.Length;
            for (int i = 0; i < len; i++)
            {
                tweens[i].PlayForward();
            }
        }

        public void OnAnimationFinish()
        {
            gameObject.SetActive(false);
        }


        protected void OnEnable()
        {
            PlayForward();
        }

        protected void OnDisable()
        {
            ResetAnimation();
        }

        

    }
}
