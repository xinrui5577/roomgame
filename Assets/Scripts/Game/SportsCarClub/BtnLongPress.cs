using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using System.Collections;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Game.SportsCarClub
{
    public class BtnLongPress : UIButton
    {
        //长按事件处理
        public float interval = .03f;

        private bool isPressed;

        private float longPressedUpdate;

        protected override void OnPress(bool isPress)
        {
            base.OnPress(isPress);
            if (isPress)
            {
                StartCoroutine(setLongPressedTrue());
                //Debug.LogError("isPressed-------------");
            }
            else
            {
                StopAllCoroutines();
                isPressed = false;
            }
        }

        protected override void OnClick()
        {
            base.OnClick();
            StopAllCoroutines();
        }


        void Update()
        {
            LongPressedFunc();
        }

        IEnumerator setLongPressedTrue()
        {
            yield return new WaitForSeconds(.5f);
            isPressed = true;
        }

        void LongPressedFunc()
        {
            if (isPressed && Time.time - longPressedUpdate > interval)
            {
                longPressedUpdate = Time.time;

                GetComponent<BetRegion>().OnClick();
            }
        }
    }

}