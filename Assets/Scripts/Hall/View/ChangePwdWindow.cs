using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Common.Windows;
using YxFramwork.Common.Utils;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View
{
    public class ChangePwdWindow:YxNguiWindow
    {
        public UIInput OldPwd;
        public UIInput NewPwd;
        public UIInput RepPwd;


        private bool CheckValue()
        {
            if (string.IsNullOrEmpty(OldPwd.value))
            {
                //PanelManager.ShowWindow(WindowName.Warring, new WaringParm("旧密码不能为空", null, null));
                YxMessageBox.Show("旧密码不能为空");
                return false;
            }
            if (string.IsNullOrEmpty(OldPwd.value))
            {
                //PanelManager.ShowWindow(WindowName.Warring, new WaringParm("旧密码不能为空", null, null));
                YxMessageBox.Show("旧密码不能为空");
                return false;
            }
            if (string.IsNullOrEmpty(NewPwd.value))
            {
                //PanelManager.ShowWindow(WindowName.Warring, new WaringParm("新密码不能为空", null, null));
                YxMessageBox.Show("新密码不能为空");
                return false;
            }

            if (NewPwd.value != RepPwd.value)
            {
                //PanelManager.ShowWindow(WindowName.Warring, new WaringParm("两次密码不一致", null, null));
                YxMessageBox.Show("两次密码不一致");
                return false;
            }

            if (NewPwd.value == OldPwd.value)
            {
                //PanelManager.ShowWindow(WindowName.Warring, new WaringParm("两次密码不一致", null, null));
                YxMessageBox.Show("新密码与旧密码一致,请重新输入新密码!!!");
                int a = 1;
                var f = a.ToString(CultureInfo.InvariantCulture);
                return false;
            }
            return true;
        }

        public void OnSubmit()
        {
            if (!CheckValue()) return;
            SendAction();
        }

        protected virtual void SendAction()
        {
            Facade.Instance<TwManager>().SendAction("password",
                                new Dictionary<string, object>()
                                    {
                                        {"oldPassword", OldPwd.value},
                                        {"newPassword", NewPwd.value},
                                        {"confirmPassword", RepPwd.value}
                                    }, OnChangeSuccess);
        }

        private void OnChangeSuccess(object msg)
        {
            var str = (bool)msg ? "密码修改成功!!" : "密码修改失败!!";
            YxMessageBox.Show(str);
            return;
            Util.RemoveData("userName");
            Util.RemoveData("userPwd");
            //PanelManager.ShowWindow(WindowName.LoadEntrance);
        }
    }
}

