using UnityEngine;

namespace Assets.Scripts.Game.BlackJackCs
{
    public class InsuranceMark : MonoBehaviour {

        public void ShowInsuranceMark()
        {
            gameObject.SetActive(true);
            UISprite us = GetComponent<UISprite>();
            us.alpha = 1;

            TweenAlpha ta = TweenAlpha.Begin(gameObject, 0.5f, 0);
            ta.delay = 0.5f;
        }

        public void HideInsuranceMark()
        {
            gameObject.SetActive(false);
        }
    }
}
