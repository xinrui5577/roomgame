using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.mx97
{
    public class PrizeInfoWindow : YxNguiWindow
    {
        public GameObject FirstPage;
        public GameObject SecondPage;
        public GameObject LeftBtn;
        public GameObject RightBtn;

        protected override void OnStart()
        {
            UIEventListener.Get(LeftBtn).onClick = OnClicked;
            UIEventListener.Get(RightBtn).onClick = OnClicked;
            LeftBtn.SetActive(false);
            RightBtn.SetActive(true);
        }
	
        private void OnClicked(GameObject go)
        {
            if ( go == RightBtn)
            {
                FirstPage.GetComponent<TweenPosition>().PlayForward();
                SecondPage.GetComponent<TweenPosition>().PlayForward();
                Facade.Instance<MusicManager>().Play("button");
                RightBtn.SetActive(false);
                LeftBtn.SetActive(true);
            }
            else if (go == LeftBtn)
            {
                FirstPage.GetComponent<TweenPosition>().PlayReverse();
                SecondPage.GetComponent<TweenPosition>().PlayReverse();
                Facade.Instance<MusicManager>().Play("button");
                RightBtn.SetActive(true);
                LeftBtn.SetActive(false);
            }
        }
    }
}
