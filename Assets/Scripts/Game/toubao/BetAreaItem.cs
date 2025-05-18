using System.Collections;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.toubao
{
    public class BetAreaItem : MonoBehaviour
    {
        public int Rate;
        public UILabel MyBet;
        public UILabel AllBet;

        private AnimatedAlpha alpha;
        public float speed = 0.5f;
        private bool TurnOn = false;



        void Start()
        {
            StopAnimate();
        }

        public void StopAnimate()
        {
            if (TurnOn)
            {
                StopAllCoroutines();
                TurnOn = false;
            }
            alpha = transform.GetComponent<AnimatedAlpha>();
            alpha.alpha = 0.1f;
        }

        public void PlayTurn()
        {
            if (TurnOn)
            {
                return;
            }
            TurnOn = true;
            StartCoroutine(PlayTurnIe());
        }

        protected IEnumerator PlayTurnIe()
        {
            while (true)
            {
                yield return new WaitForSeconds(speed);
                alpha.alpha = alpha.alpha == 1 ? 0.1f : 1;
            }
        }

        void OnClick()
        {

            bool flag = App.GetGameData<GlobalData>().CanBet;
            if (!flag)
            {
                return;
            }
            UISprite sprite = transform.GetComponent<UISprite>();
            if (sprite == null) return;
            App.GetGameManager<TouBaoGameManager>().OnBetDownClick(sprite, Rate);
        }

    }
}
