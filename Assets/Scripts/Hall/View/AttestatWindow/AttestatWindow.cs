using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.AttestatWindow
{
    /// <summary>
    /// 身份认证信息
    /// </summary>
    public class AttestatWindow : YxNguiWindow
    {
        public GameObject SureBtn;
        public UILabel UserName;
        public UILabel IdCard;
        public UILabel PhoneNum;
        public UILabel PhoneCode;
        public UILabel NeedTime;
        public UIInput RealYear;
        public UIInput RealMonth;
        public UIInput RealDay;

        public int TimeCtrl;
        public string SendActionName;

        private int _timeCtrl;
        private int _userSex = 1;

        public void OnSureBtn()
        {
            var dic = new Dictionary<string, object>();
            dic["real_m"] = UserName.text;
            dic["sex_i"] = _userSex;
            dic["real_year"] = RealYear.value;
            dic["real_month"] = RealMonth.value;
            dic["real_day"] = RealDay.value;
            dic["id_card_n"] = IdCard.text;
            dic["telephone_m"] = PhoneNum.text;
            dic["verify"] = PhoneCode.text;
            Facade.Instance<TwManager>().SendAction(SendActionName, dic, OnFreshData);
        }

        public void OnMobileSend()
        {
            if (_timeCtrl > 0)
            {
                var info = new YxMessageBoxData { Msg = "您发送验证码过于频繁，请等待" };
                YxMessageBox.Show(info);
                return;
            }
            var dic = new Dictionary<string, object>();
            dic["mobile_n"] = PhoneNum.text;
            Facade.Instance<TwManager>().SendAction("mahjongwm.telVerify", dic, data =>
            {
                _timeCtrl = TimeCtrl;
                InvokeRepeating("ComputTime", 0, 1);
            });
        }

        private void ComputTime()
        {
            if (_timeCtrl <= 0)
            {
                NeedTime.text = "";
                CancelInvoke("ComputTime");
                return;
            }
            _timeCtrl--;
            NeedTime.text = _timeCtrl + "s";
        }

        public void OnSelectSex(UIToggle toggle)
        {
            if (!toggle.value) return;
            _userSex = int.Parse(toggle.name);
        }

        private void OnFreshData(object data)
        {
            Facade.EventCenter.DispatchEvent<string, object>("IdData");
            Facade.EventCenter.DispatchEvent<string, object>("Verify");
            Close();
        }
    }
}

