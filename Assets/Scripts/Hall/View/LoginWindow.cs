using Assets.Scripts.Common;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Common.Utils;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;
using com.yxixia.utile.Utiles;

namespace Assets.Scripts.Hall.View
{
    public class LoginWindow:YxBaseWindow
    { 
        public override WindowName WindowName
        {
            get { return WindowName.Login; }
        }

        protected override IBaseModel YxBaseModel
        {
            get { return LoginInfo.Instance; }
        }

        public UIInput UserName;

        public UIInput UserPwd;

        public UIToggle RemberName;

        /// <summary>
        /// 用户协议
        /// </summary>
        public UIToggle UserAgementToggle;

        /// <summary>
        /// 游客登录
        /// </summary>
        public GameObject BtnVisitor;
        /// <summary>
        /// 微信
        /// </summary>
        public GameObject BtnWechat;

        private void OnLoginEvent(object msg)
        {
        }

        protected override void OnCreate()
        {
            #region 初始化组件 
            AddListien("onLogin", OnLoginEvent);
            #endregion
            #region 本地信息

            var rember = false;
            if (RemberName != null)
            {
                
                rember = Util.GetInt(AppConst.PlayerPrefsRemenberInt) == 1;
                RemberName.value = rember;
            }
            if (rember)
            {
                if (UserName != null)
                {
                    UserName.value = Util.GetString(AppConst.PlayerPrefsUserNameStr);
                    if (UserPwd != null)
                    {
                       UserPwd.value = Util.GetString(AppConst.PlayerPrefsPasswordStr);
                    }
                }
            }
            else
            {
                if (UserName != null)
                {
                    UserName.value = "";
                }
                if (UserPwd != null)
                {
                    UserPwd.value = "";
                }
            } 
            if (!string.IsNullOrEmpty(App.Config.WxAppId) && Application.isMobilePlatform && App.Config.HasWechatLogin) //&& App.Config.HasWechatLogin)
            {
                var wechatApi = Facade.Instance<WeChatApi>();
                ChangeOtherLogin(wechatApi.InitWechat() && wechatApi.IsInstalledWechat() && wechatApi.IsCheckWechatApiLevel());
            }
            else
            {
                ChangeOtherLogin(false);
            }
#if !YX_DEVE && !UNITY_EDITOR
            UserController.Instance.AutoLogin();
#endif
#endregion
        }



        private void ChangeOtherLogin(bool isWechat)
        {
            if (BtnVisitor!=null) BtnVisitor.gameObject.SetActive(!isWechat);
            if (BtnWechat != null)
            {
                BtnWechat.gameObject.SetActive(isWechat);
                if (isWechat)
                {
                    Facade.Instance<WeChatApi>().InitWechat();
                    
                }
            }
        }

        /// <summary>
        /// 账号登录
        /// </summary>
        public void OnLogin()
        {
            if (string.IsNullOrEmpty(UserName.value))
            {
                YxMessageBox.Show("用户名不能为空！");
                return;
            }
            if (string.IsNullOrEmpty(UserPwd.value))
            {
                YxMessageBox.Show("密码不能为空！");
                return;
            }
            //HiddenWindow();
            UserController.Instance.Login(UserName.value, UserPwd.value, RemberName != null && RemberName.value);
        }

        /// <summary>
        /// 游客登录
        /// </summary>
        public void OnVisitor()
        {
            if (UserAgementToggle != null && UserAgementToggle.value==false)
            {
                YxMessageBox.Show("必须同意用户协议");
                return;
            }
            UserController.Instance.VisitorLogin();
        }

        /// <summary>
        /// 注册
        /// </summary>
        public void OnRester()
        {
            HiddenWindow();
            PanelManager.ShowWindow(WindowName.Register);
        }

        /// <summary>
        /// 微信登录
        /// </summary>
        public void OnWeChatLogin()
        {
            if (UserAgementToggle != null && UserAgementToggle.value == false)
            {
                YxMessageBox.Show("必须同意用户协议");
                return;
            }

            UserController.Instance.WeChatLogin();
        }

        protected override void OnShow(object o)
        {
            base.OnShow(o);
            App.GameKey = App.HallName;
            CurtainManager.CloseCurtain();
        }

#if UNITY_EDITOR || YX_DEVE
        private bool _isShow;

        protected void OnGUI()
        {
            GlobalUtile.ResizeGUIMatrix();
            var sh = Screen.height;
            var fontStyle = new GUIStyle
                {
                    normal =
                        {
//                            background = null,
                            textColor = Color.white
                        }
//                    fontSize = rowH
                };  

            GUILayout.BeginHorizontal();
            if (GUI.Button(new Rect(0, 10, 10, sh), ""))
            {
                _isShow = !_isShow;
            } 
            GUILayout.EndHorizontal();

            if (!_isShow) return;
            const int gx = 30;
            GUILayout.BeginHorizontal();
            GUI.Label(new Rect(gx, 10, 40, 20), "IP", fontStyle);
            var oldIp = GUI.TextField(new Rect(gx + 55, 10, 100, 20), PlayerPrefs.GetString("login_editor_ip"));
            PlayerPrefs.SetString("login_editor_ip", oldIp);
            var isUse = PlayerPrefs.GetInt("login_editor_use") == 1;
            isUse = GUI.Toggle(new Rect(gx + 170, 10, 25, 25), isUse, "");
            PlayerPrefs.SetInt("login_editor_use", isUse ? 1 : 0);
            if (isUse)
            {
                App.DevGameServer = oldIp;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUI.Label(new Rect(gx, 35, 40, 20), "用户名", fontStyle);
            var newName = GUI.TextField(new Rect(gx + 55, 35, 100, 20), PlayerPrefs.GetString("login_editor_userName"));
            PlayerPrefs.SetString("login_editor_userName", newName);
            UserName.value = newName;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUI.Label(new Rect(gx, 60, 40, 20), "密  码", fontStyle);
            var pwd = GUI.TextField(new Rect(gx + 55, 60, 100, 20), PlayerPrefs.GetString("login_editor_userPwd"));
            PlayerPrefs.SetString("login_editor_userPwd", pwd);
            UserPwd.value = pwd;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUI.Button(new Rect(gx, 95, 40, 20), "登 录"))
            { 
                OnLogin();
            }
            if (GUI.Button(new Rect(gx + 90, 95, 40, 20), "游 客"))
            {
                OnVisitor();
            }
            GUILayout.EndHorizontal();
        }
#endif
    }
}

