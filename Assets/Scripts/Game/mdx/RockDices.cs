using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.mdx
{
    public class RockDices : MonoBehaviour
    {

        public GameObject View;

        public TweenPosition TweenPos;

        public UISpriteAnimation[] RockingDices;

        public UISprite[] QuietDices;

        public string DiceSpriteNameFromat;

        private int _diceIndex;

        public void ShowRockDices()
        {
            int[] dicesVal = App.GetGameData<MdxGameData>().DiceVals;
            if (dicesVal == null) return;
            GiveDicesPoint(dicesVal);
            gameObject.SetActive(true);
            View.SetActive(true);
            TweenPos.gameObject.SetActive(true);
            TweenPos.ResetToBeginning();
            TweenPos.PlayForward();
            Facade.Instance<MusicManager>().Play("rockdices");
        }

        void GiveDicesPoint(int[] dicesVal)
        {
            if (dicesVal == null) return;
            int len = dicesVal.Length;
            for (int i = 0; i < len; i++)
            {
                QuietDices[i].spriteName = string.Format(DiceSpriteNameFromat, dicesVal[i]);
            }
        }

        private void PlayRocking()
        {
            int len = QuietDices.Length;
            for (int i = 0; i < len; i++)
            {
                var dice = RockingDices[i];
                dice.gameObject.SetActive(true);
                dice.enabled = true;
            }
        }

        private void ShowRockResult()
        {
            InvokeRepeating("ShowDiceValue", 1, 1);
        }

        protected void ShowDiceValue()
        {
            int len = QuietDices.Length;
            if (_diceIndex >= len)
            {
                CancelInvoke("ShowDiceValue");
                return;
            }
            var rockDice = RockingDices[_diceIndex];
            StopRocking(rockDice);
            rockDice.gameObject.SetActive(false);
            QuietDices[_diceIndex++].gameObject.SetActive(true);
        }


        void StopRocking(UISpriteAnimation dice)
        {
            dice.enabled = false;
            var spr = dice.GetComponent<UISprite>();
            spr.spriteName = "defult";
        }

        public void OnTweenPosFinish()
        {
            PlayRocking();
            ShowRockResult();

        }


        public void Reset()
        {
            View.SetActive(false);
            foreach (var dice in QuietDices)
            {
                dice.gameObject.SetActive(false);
            }

            foreach (var dice in RockingDices)
            {
                StopRocking(dice);
                dice.gameObject.SetActive(true);
            }
            _diceIndex = 0;
            CancelInvoke();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

    }
}
