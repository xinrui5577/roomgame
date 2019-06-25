using Assets.Scripts.Common.Utils;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.View;
using UnityEngine;
using YxFramwork.Controller;
using YxFramwork.Framework;

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
        /// 昵称
        /// </summary>
        public UIInput NickName;
        /// <summary>
        /// 推广码
        /// </summary>
        public UIInput Promote;
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

        public override WindowName WindowName
        {
            get { return WindowName.Register; }
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
                {
                    box.enabled = false;
                }
                Promote.defaultText = AppInfo.PromoteCode;
            }

            if (App.AppStyle == EAppStyle.Normal) return;
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
          
        public void OnResiter()
        {
            if (IsNeedPopProtocol()) return;
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
                YxMessageBox.Show("用户名不能为空！");
                return;
            }

            if (string.IsNullOrEmpty(userPwd))
            {
                YxMessageBox.Show("密码不能为空！");
                return;
            }

            if (string.IsNullOrEmpty(nikeName))
            {
                YxMessageBox.Show("昵称不能为空！");
                return;
            }
            Debug.Log(sex);
            UserController.Instance.Restier(userName, userPwd, nikeName, prote, sex);
        }

        public void OnBackLogin()
        {
            HiddenWindow();
            PanelManager.ShowWindow(WindowName.Login);
        }

        /// <summary>
        /// 是否需要用户协议
        /// </summary>
        protected bool IsNeedPopProtocol()
        {
            if (ProtocolToggle == null) return false;
            if (!ProtocolToggle.value) return false;
            YxMessageBox.Show("请同意用户协议，否则不能注册游戏！");
            return true;
        }
    }
}

