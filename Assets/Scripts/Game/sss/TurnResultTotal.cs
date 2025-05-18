using Assets.Scripts.Game.sss.Tool;
using UnityEngine;

namespace Assets.Scripts.Game.sss
{

    public class TurnResultTotal : TurnResultItem
    {
        /// <summary>
        /// 总分
        /// </summary>
        private int _totalScore;

        [SerializeField]
        protected TweenScale WinLabelTween = null;

        public override void SetValue(int win, int special = 0)
        {
            int temp = win + special + _totalScore;
            if (_totalScore != temp)
            {
                WinLabelTween.ResetToBeginning();
                WinLabelTween.PlayForward();
            }
            _totalScore = temp;      //记录总成绩
            WinLabel.text = _totalScore < 0 ? _totalScore.ToString() : "+" + _totalScore;
            Tools.SetLabelColor(WinLabel, _totalScore);
        }

        public override void Reset()
        {
            TweenPosition tp = GetComponent<TweenPosition>();
            gameObject.transform.localPosition = tp.from;
            tp.ResetToBeginning();

            WinLabel.text = "";

            WinLabelTween.ResetToBeginning();
            WinLabelTween.transform.localScale = Vector3.one;
            _totalScore = 0;
        }

        public override void MoveItem()
        {
            TweenPosition tp = GetComponent<TweenPosition>();
            tp.PlayForward();
        }
    }
}