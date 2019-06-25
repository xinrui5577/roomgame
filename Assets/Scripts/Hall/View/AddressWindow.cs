using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View
{
    public class AddressWindow : YxNguiWindow
    {
        [Tooltip("收件人")]
        public UIInput AddresseeInput;
        [Tooltip("电话输入框")]
        public UIInput PhoneInput;
        [Tooltip("地址输入框")]
        public UIInput AddressInput;

        protected override void OnStart()
        {
            if (PlayerPrefs.HasKey("AddressWindow_addressee")) PhoneInput.value = PlayerPrefs.GetString("AddressWindow_addressee");
            if (PlayerPrefs.HasKey("AddressWindow_phone")) PhoneInput.value = PlayerPrefs.GetString("AddressWindow_phone");
            if (PlayerPrefs.HasKey("AddressWindow_address")) AddressInput.value = PlayerPrefs.GetString("AddressWindow_address");
        }

        public void OnSendContactInfo()
        {
            if (Data == null) return;
            var parm = new Dictionary<string, object>();
            var addressee = AddresseeInput.value;
            if (string.IsNullOrEmpty(addressee))
            {
                YxMessageBox.Show("亲，请输入收件人！");
                return;
            }
            var phone = PhoneInput.value;
            if (string.IsNullOrEmpty(phone))
            {
                YxMessageBox.Show("亲，请输入联系电话！");
                return;
            }
            var address = AddressInput.value;
            if (string.IsNullOrEmpty(address))
            {
                YxMessageBox.Show("亲，请输入联系地址！");
                return;
            }
            PlayerPrefs.SetString("AddressWindow_addressee", addressee);
            PlayerPrefs.SetString("AddressWindow_phone",phone);
            PlayerPrefs.SetString("AddressWindow_address", address);
            parm["id"] = Data.ToString();
            parm["phone"] = phone;
            parm["address"] = address;
            parm["name"] = addressee;
            Facade.Instance<TwManger>().SendAction("userAddress_yr", parm, OnSuccess);
        }

        private void OnSuccess(object msg)
        {
            if (CallBack != null) CallBack(msg);
            YxMessageBox.Show("提交成功！");
            Close();
        }
    }
}
