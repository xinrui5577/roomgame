using Assets.Scripts.Common.Utils;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.View;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Framework;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.View
{
    /// <summary>
    /// 注册窗口
    /// </summary>
    public class RegisterWindow:YxBaseWindow
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public UIInput UserName;
        /// <summary>
        /// 密码
        /// </summary>
        public UIInput UserPwd;
        /// <summary>
        /// 确认密码
        /// </summary>
        public UIInput UserPwdConfirm;
        /// <summary>
        /// 昵称
        /// </summary>
        public UIInput NickName;
        /// <summary>
        /// 推广码
        /// </summary>
        public UIInput Promote;
        /// <summary>
        /// 手机号
        /// </summary>
        public UIInput PhoneNumber;
        /// <summary>
        /// 性别Toggle组
        /// </summary>
        public int SexGroup = 10000;
        /// <summary>
        /// 
        /// </summary>
        public UIGrid RowItemGrid;
        /// <summary>
        /// 用户协议
        /// </summary>
        public UIToggle ProtocolToggle;
        /// <summary>
        /// 
        /// </summary>
        public GameObject[] NeedHideObject;
        /// <summary>
        /// 验证码
        /// </summary>
        public UIInput TelVerifyLabel;
        public override YxEWindowName WindowName
        {
            get { return YxEWindowName.Register; }
        }

        protected override IBaseModel YxBaseModel
        {
            get { return null; }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            if (Promote != null && !string.IsNullOrEmpty(AppInfo.PromoteCode))//推广码
            {
                var box = Promote.GetComponent<BoxCollider>();
                if (box != null)
                    box.enabled = false;
                Promote.defaultText = AppInfo.PromoteCode;
            }

            if (App.AppStyle == YxEAppStyle.Normal) return;
            var len = NeedHideObject.Length;
            for (var i = 0; i < len; i++)
            {
                var obj = NeedHideObject[i];
                if(obj==null)continue;
                obj.SetActive(false);
            }
            if (RowItemGrid == null) return;
            RowItemGrid.repositionNow = true;
            RowItemGrid.Reposition();
        }

        protected override void OnStart()
        {
            base.OnStart();
            App.History.RecordHistory(YxEHistoryPathType.Register);
        }

        public void OnResiter()
        {
            if (LoginWindow.IsNeedPopProtocol(ProtocolToggle))
            {
                return;
            }
            var sexToggle = UIToggle.GetActiveToggle(SexGroup);
            int sex = sexToggle!=null && int.TryParse(sexToggle.name, out sex) ? sex : -1;
            var userName = UserName.value;
            var userPwd = UserPwd.value;
            var prote = string.IsNullOrEmpty(AppInfo.PromoteCode) && Promote != null
                            ? Promote.value
                            : AppInfo.PromoteCode;//推广码
            var nikeName = NickName.value; 

            if (string.IsNullOrEmpty(userName))
            {
                YxWindowManager.ShowMessageWindow("用户名不能为空！");
                return;
            }

            if (string.IsNullOrEmpty(userPwd))
            {
                YxWindowManager.ShowMessageWindow("密码不能为空！");
                return;
            }
            if (UserPwdConfirm != null)
            {
                var pwdConfirm = UserPwdConfirm.value;
                if (string.IsNullOrEmpty(pwdConfirm))
                {
                    YxWindowManager.ShowMessageWindow("请确认密码！");
                    return;
                }
                if (!pwdConfirm.Equals(userPwd))
                {
                    YxWindowManager.ShowMessageWindow("两次输入密码不一致！");
                    return;
                }
            }

            var phone = "";
            if (PhoneNumber != null && PhoneNumber.gameObject.activeSelf)
            {
                phone = PhoneNumber.value; 
            }
            var telVerify = "";
            if (TelVerifyLabel!=null)
            {
                telVerify = TelVerifyLabel.value; 
            } 
            var info = new RegisterInfo
            {
                LoginName = userName,
                Password = userPwd,
                NickM = nikeName,
                SexI = sex,
                TelVerify = telVerify,
                PhoneNumber = phone
            };
            int.TryParse(prote, out info.SpreadCode);
            UserController.Instance.Restier(info);
        }

        public void OnBackLogin()
        {
            HiddenWindow();
            App.History.RecordHistory(YxEHistoryPathType.Login);
            CurPanelManager.ShowWindow(YxEWindowName.Login);
        }
         
        /// <summary>
        /// 发送验证码
        /// </summary>
        public void OnSendVerifyCode()
        {
            if (PhoneNumber == null) return;
            var number = PhoneNumber.value;
            if (string.IsNullOrEmpty(number) || number.Length != 11)
            {
                YxMessageBox.Show("请正确输入手机号！");
            }
            UserController.Instance.SendTelVerify(number);
        }
    }
}

