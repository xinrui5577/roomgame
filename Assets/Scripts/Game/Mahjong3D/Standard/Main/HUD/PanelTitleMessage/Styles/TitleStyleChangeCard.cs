using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class TitleStyleChangeCard : TitleStyleBase
    {
        public GameObject Tip;
        public GameObject ChangeError;

        private void Awake()
        {
            GameCenter.EventHandle.Subscriber((int)EventKeys.HideChangeCardTip, HideChangeCardTip);
            GameCenter.EventHandle.Subscriber((int)EventKeys.ChangeErrorTip, ChangeErrorTip);
        }

        public override void Show()
        {
            Tip.gameObject.SetActive(true);
        }

        public void OnConfirmFenggangClick()
        {
            //GameCenter.NetworkComponent.C2S.Custom<C2SCustom>().OnConfirmFenggangClick();
        }

        private void HideChangeCardTip(EvtHandlerArgs args)
        {
            Tip.gameObject.SetActive(false);
        }

        private void ChangeErrorTip(EvtHandlerArgs args)
        {
            StartCoroutine(ShowException());
        }

        private IEnumerator ShowException()
        {
            ChangeError.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            ChangeError.gameObject.SetActive(false);
        }
    }
}