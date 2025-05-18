using System;
using UnityEngine;
using System.Globalization;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using Sfs2X.Entities.Data;
using YxFramwork.Tool;
using Assets.Scripts.Common.Adapters;

namespace Assets.Scripts.Game.BaiTuan.skin02
{
    public class ProgressCtrl02 : ProgressCtrl
    {
        public GameObject Clock;
        public NguiLabelAdapter BankerLimit;
        public UISprite CountDownSprite;
        //去除不必要的变量
        public override void BeginCountdown()
        {
            Clock.SetActive(true);
            Clock.GetComponent<TweenRotation>().enabled = false;
            Clock.transform.localEulerAngles = Vector3.zero;
            CancelInvoke("CyclePerform_Num");
            InvokeRepeating("CyclePerform_Num", 0, 1);
        }

        //去除不必要变量
        public override void EndCountdown()
        {
            Clock.SetActive(false);
            CancelInvoke("CyclePerform_Num");
            App.GetGameData<BtwGameData>().BeginBet = false;
        }

        public override void CyclePerform_Num()
        {

            int time = Int32.Parse(CountdownNum.text) - 1;
            var tr = Clock.GetComponent<TweenRotation>();
            if (time == 5)
            {
                Facade.Instance<MusicManager>().Play("hurryup");
                tr.ResetToBeginning();
                tr.PlayForward();
            }
            CountdownNum.text = (time).ToString(CultureInfo.InvariantCulture);
            CountDownSprite.fillAmount -= _cdStep;
            if (Int32.Parse(CountdownNum.text) > 0) return;
            CountdownNum.text = "0";
            CountDownSprite.fillAmount = 0;

            EndCountdown();
        }

        private float _cdStep = 0;

        public override void ReSetCountdown(int s)
        {
            base.ReSetCountdown(s);
            _cdStep = 1f / s;
            CountDownSprite.fillAmount = 1;
        }
        public void SetBankerLimitLabel(int gold)
        {
            BankerLimit.Text(YxUtiles.ReduceNumber(gold));
        }
    }
}