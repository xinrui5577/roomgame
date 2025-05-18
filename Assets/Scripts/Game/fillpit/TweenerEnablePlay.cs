using UnityEngine;

namespace Assets.Scripts.Game.fillpit
{
    public class TweenerEnablePlay : MonoBehaviour
    {

        protected void OnEnable()
        {
            var tweens = GetComponents<UITweener>();
            if (tweens == null || tweens.Length <= 0) return;

            foreach (var tween in tweens)
            {
                tween.ResetToBeginning();
                tween.PlayForward();
            }
        }

        public void HideObj(GameObject obj)
        {
            obj.SetActive(false);
        }

    }
}
