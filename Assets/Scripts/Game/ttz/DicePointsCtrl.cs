using UnityEngine;
using System.Collections;
using YxFramwork.Common;

namespace Assets.Scripts.Game.ttz
{
    public class DicePointsCtrl : MonoBehaviour
    {
        public Animator Anim;
        public UISprite[] Dices;

        public void OnAnimEnd()
        {
            Anim.Stop();
            Anim.enabled = false;
            Invoke("Hide", 1);
        }

        protected void Hide()
        {
            App.GetGameManager<BrttzGameManager>().BrttzCardsCtrl.BeginGiveCards();
            gameObject.SetActive(false);
        }

        public void Reset()
        {
            CancelInvoke();
            gameObject.SetActive(false);
        }
    }
}