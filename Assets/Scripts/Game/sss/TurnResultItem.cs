using Assets.Scripts.Game.sss.Tool;
using UnityEngine;

namespace Assets.Scripts.Game.sss
{

    public class TurnResultItem : MonoBehaviour
    {

        [SerializeField]
        protected UILabel WinLabel;

        [SerializeField]
        protected UILabel SpecialLabel;

        public virtual void SetValue(int win, int special = 0)
        {
            WinLabel.text = win < 0 ? win.ToString() : "+" + win;
            Tools.SetLabelColor(WinLabel, win);
            SpecialLabel.text = "（" + (special < 0 ? special.ToString() : "+" + special) + "）";
            Tools.SetLabelColor(SpecialLabel, special);
        }

        public virtual void MoveItem()
        {
            TweenPosition tp = GetComponent<TweenPosition>();
            tp.ResetToBeginning();
            tp.PlayForward();
        }

        public virtual void Reset()
        {
            TweenPosition tp = GetComponent<TweenPosition>();
            tp.enabled = false;
            gameObject.transform.localPosition = tp.from;
            tp.ResetToBeginning();

            WinLabel.text = "";
            SpecialLabel.text = "";
        }

    }
}
