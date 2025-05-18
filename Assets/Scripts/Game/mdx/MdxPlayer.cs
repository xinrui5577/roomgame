using System.Collections;
using Assets.Scripts.Common.Adapters;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.mdx
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public class MdxPlayer : YxBaseGamePlayer
    {
        public NguiLabelAdapter SelfWinLabel;

        public Color WinColor;

        public Color LostColor;
       
        public void ShowSelfWinLabel(int gold)
        {
            if (SelfWinLabel == null) return;
            if (gold == 0) return;
            SelfWinLabel.Text(gold);
            SelfWinLabel.Label.color = gold > 0 ? WinColor : LostColor;
            SelfWinLabel.gameObject.SetActive(true);
            var tweens = SelfWinLabel.GetComponents<UITweener>();
            foreach (var tween in tweens)
            {
                tween.ResetToBeginning();
                tween.PlayForward();
            }
        }

        public void OnFinish()
        {
            SelfWinLabel.gameObject.SetActive(false);
        }

        private bool _isWaiting;

        public void PlayWait()
        {
            if (!_isWaiting)
            {
                _isWaiting = true;
                StartCoroutine(Waiting());
            }
        }

        public void StopWaiting()
        {
            _isWaiting = false;
            StopAllCoroutines();
        }

        private IEnumerator Waiting()
        {
            string waitText = "等待抢庄";
            while (_isWaiting)
            {
                NameLabel.Text(string.Format("{0}", waitText));
                yield return new WaitForSeconds(0.5f);
                NameLabel.Text(string.Format("{0}.", waitText));
                yield return new WaitForSeconds(0.5f);
                NameLabel.Text(string.Format("{0}..", waitText));
                yield return new WaitForSeconds(0.5f);
                NameLabel.Text(string.Format("{0}...", waitText));
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
