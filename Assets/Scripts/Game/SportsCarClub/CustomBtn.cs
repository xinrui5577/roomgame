/*
 * 时间：2018年8月7日16:45:06
 * 功能：挂在到按钮上，点击按钮时将按钮图片swap
 */
using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Game.SportsCarClub
{
    public class CustomBtn : UIButton
    {
        [SerializeField]
        private GameObject swapBtnPic;

        // Use this for initialization
        void Start()
        {
            swapBtnPic = tweenTarget.transform.FindChild("swapImg").gameObject;
        }

        // Update is called once per frame
        void Update()
        {

        }

        protected override void OnClick()
        {
            base.OnClick();
            swapBtnPic.GetComponent<UISprite>().enabled = true;
            tweenTarget.GetComponent<UISprite>().enabled = false;

            OnPress(false);
        }

        protected override void OnPress(bool isPressed = false)
        {
            //Debug.LogError("Onpress");
            base.OnPress(isPressed);

            if (isPressed)
            {
                swapBtnPic.GetComponent<UISprite>().enabled = true;
                tweenTarget.GetComponent<UISprite>().enabled = false;
            }
            else
            {
                swapBtnPic.GetComponent<UISprite>().enabled = false;
                tweenTarget.GetComponent<UISprite>().enabled = true;
            }
        }
    }

}