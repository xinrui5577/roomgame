using System.Collections;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.toubao
{
    public class Clock : MonoBehaviour
    {
        public UILabel Num;

        private int TimeNum = 0;

        public UISprite StateSprite;

        private string XiaZhu = "tip1";

        private string KongXian = "tip2";

        private string KaiPai = "tip3";

        void Start()
        {
            Num.text = "00";
        }

        public void SetClockNum(int value)
        {
            TimeNum = value;
            if (value < 10)
            {
                Num.text = "0" + value;
            }
            else
            {
                Num.text = "" + value;
            }
            ChangeStateTip();
            StartCoroutine(RunTimer());
        }

        IEnumerator RunTimer()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                TimeNum--;
                TimeNum = TimeNum < 0 ? 0 : TimeNum;
                if (TimeNum < 10)
                {
                    Num.text = "0" + TimeNum;
                }
                else
                {
                    Num.text = "" + TimeNum;
                }
                if (TimeNum <= 0)
                {
                    yield break;
                }
            }

        }

        public void ChangeStateTip()
        {
            switch (App.GetGameData<GlobalData>().State)
            {
                case GlobalData.GameState.XiaZhu:
                    StateSprite.spriteName = XiaZhu;
                    break;
                case GlobalData.GameState.Free:
                    StateSprite.spriteName = KongXian;
                    break;
                case GlobalData.GameState.Result:
                    StateSprite.spriteName = KaiPai;
                    break;

            }
        }
    }
}
