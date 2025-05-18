using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class TitleStyleBbmjtip : TitleStyleBase
    {
        public int ShowTimet = 5;
        public Transform Title;     

        public override void Show()
        {
            StartCoroutine(ShowTitleTask());
        }

        protected IEnumerator ShowTitleTask()
        {
            Title.gameObject.SetActive(true);
            yield return new WaitForSeconds(ShowTimet);
            Title.gameObject.SetActive(false);
        }
    }
}