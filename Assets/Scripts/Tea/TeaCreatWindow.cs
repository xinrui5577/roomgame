using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Tea
{
    public class TeaCreatWindow : YxNguiWindow
    {
        /// <summary>
        /// 茶馆名称
        /// </summary>
        public UIInput TeaNameInput;
        /// <summary>
        /// 微信号
        /// </summary>
        public UIInput WeiXinInput;
        /// <summary>
        /// 手机号
        /// </summary>
        public UIInput MobileInput;
        /// <summary>
        /// 验证码
        /// </summary>
        public UIInput SecurityCodeInput;
        /// <summary>
        /// 茶馆描述
        /// </summary>
        public UIInput TeaSignInput;


        public void OnSendSms()
        {
            if (MobileInput.value.Equals(""))
            {
                YxMessageBox.Show("请输入手机号");
                return;
            }
            var dic=new Dictionary<string,object>();
            dic["mobile_n"] = MobileInput.value;
            Facade.Instance<TwManager>().SendAction("group.teaSendSms", dic,null);
        }

        public void OnCreatTea()
        {
            if (TeaNameInput.value.Equals(""))
            {
                YxMessageBox.Show("请输入馆名！！！");
                return;
            }

            if (WeiXinInput.value.Equals(""))
            {
                YxMessageBox.Show("请输入微信号！！！");
                return;
            }

            if (SecurityCodeInput&&SecurityCodeInput.value.Equals(""))
            {
                YxMessageBox.Show("请输入验证码！！！");
                return;
            }

            var dic = new Dictionary<string, object>();
            dic["tea_name"] = TeaNameInput.value;
            dic["wechat"] = WeiXinInput.value;
            if (MobileInput)
            {
                dic["telephone_m"] = MobileInput.value;
                dic["verify"] = SecurityCodeInput.value;
            }
            dic["group_sign"] = TeaSignInput.value;

            Facade.Instance<TwManager>().SendAction("group.creatTeaHouse", dic, data =>{ Close(); });
        }


    }
}
