using System.Collections.Generic;
using YxFramwork.Common.Model;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View
{
    /// <summary>
    /// 补全资料
    /// </summary>
    public class AdditionalWindow : YxWindow
    {
        /// <summary>
        /// 用户名Label
        /// </summary>
        public UIInput UserNameLabel;
        /// <summary>
        /// 密码Label
        /// </summary>
        public UIInput PasswordLabel;
        /// <summary>
        /// 确认密码Label
        /// </summary>
        public UIInput RePasswordLabel;

        public void OnChangeEnter()
        {
            var userName = UserNameLabel.text;
            if (string.IsNullOrEmpty(userName))
            {
                YxMessageBox.Show("用户名不能为空！！！");
                return;
            }
            var pwd = PasswordLabel.value;
            if (string.IsNullOrEmpty(pwd))
            {
                YxMessageBox.Show("密码不能为空！！！");
                return;
            }
            var rpwd = RePasswordLabel.value;
            if (rpwd!=pwd)
            {
                YxMessageBox.Show("2次输入的密码不一致！！！");
                return;
            }
            var parm = new Dictionary<string, object>();
            parm["login_m"] = userName;
            parm["password_x"] = PasswordLabel.value;
            Facade.Instance<TwManger>().SendAction("setAddUserData", parm, UpdateView);
        }

        protected override void OnFreshView()
        {
            if (Data == null) return;
            var dict = Data as Dictionary<string, object>;
            if (dict == null) return;
            if (!dict.ContainsKey("login_m")) return;
            var userModel = UserInfoModel.Instance;
            var userInfo = userModel.UserInfo;
            userInfo.LoginM = dict["login_m"].ToString();
            userModel.Save();
            YxMessageBox.Show("修改成功！！","", (box, btnName) =>
                {
                    if (btnName == YxMessageBox.BtnLeft)
                    {
                        Close();
                    }
                });
        }
    }
}
