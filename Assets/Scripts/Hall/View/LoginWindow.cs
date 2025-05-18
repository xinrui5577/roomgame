using com.yxixia.utile.YxDebug;
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
using YxFramwork.Enums;
using YxFramwork.View.Develops;

namespace Assets.Scripts.Hall.View
{
    public class LoginWindow:YxBaseWindow
    { 
        public override YxEWindowName WindowName
        {
            get { return YxEWindowName.Login; }
        }

        protected override IBaseModel YxBaseModel
        {
            get { return LoginInfo.Instance; }
        }

        public UIInput UserName;

        public UILabel RemberUserId;

        public UIInput UserPwd;

        public UIToggle RemberName;
        /// <summary>
        /// 手机号登陆
        /// </summary>
        public UIInput PhoneNumber;
        /// <summary>
        /// 验证
        /// </summary>
        public UIInput VerifyCode;
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
        [Tooltip("是否需要自动登陆")]
        public bool NeedAutoLogin = true;
        protected override void OnStart()
        {
            base.OnStart();
#if UNITY_EDITOR || YX_DEVE
            gameObject.AddComponent<YxDevelopLoginWindow>();
#else 
            if(YxDebug.IsDebug)
            {
                gameObject.AddComponent<YxDevelopLoginWindow>();
            }       
#endif
        }

        protected override void OnCreate()
        {
            #region 本地信息
            var rember = false;
            App.UserId = "";
            if (RemberUserId!=null)
            {
                RemberUserId.text = Util.GetString(AppConst.PlayerPrefsUserIdStr);
            }
            if (RemberName != null)
            {
                rember = Util.GetInt(AppConst.PlayerPrefsRemenberInt) == 1 || NeedAutoLogin;
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
                ChangeOtherLogin(wechatApi.NeedWechat() && wechatApi.InitWechat() && wechatApi.IsInstalledWechat() && wechatApi.IsCheckWechatApiLevel());
            }
            else
            {
                ChangeOtherLogin(false);
            }
            #if !YX_DEVE && !UNITY_EDITOR
            if (NeedAutoLogin)
            {
                UserController.Instance.AutoLogin();
            }
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
            if (IsNeedPopProtocol(UserAgementToggle))
            {
                return;
            }
            OnLogin(UserName.value, UserPwd.value,RemberName != null && RemberName.value);
        }

        /// <summary>
        /// 手机登录
        /// </summary>
        public void OnPhoneLogin()
        {
            if (IsNeedPopProtocol(UserAgementToggle))
            {
                return;
            }
            OnLogin(PhoneNumber.value, VerifyCode.value,RemberName != null && RemberName.value,true);
        }

        private static void OnLogin(string userName,string pwd, bool remberName = true, bool isVcode = false)
        {
            
            if (string.IsNullOrEmpty(userName))
            {
                YxWindowManager.ShowMessageWindow(isVcode?"手机号不能为空！":"用户名不能为空！");
                return;
            }
            if (string.IsNullOrEmpty(pwd))
            {
                YxWindowManager.ShowMessageWindow(isVcode ?"请输入验证码" :"密码不能为空！");
                return;
            }
            //HiddenWindow();
            UserController.Instance.Login(userName, pwd, remberName, isVcode);
        } 

        /// <summary>
        /// 游客登录
        /// </summary>
        public void OnVisitor()
        {
            if (IsNeedPopProtocol(UserAgementToggle))
            {
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
            CurPanelManager.ShowWindow(YxEWindowName.Register);
            App.History.RecordHistory(YxEHistoryPathType.Register);
        }

        /// <summary>
        /// 微信登录
        /// </summary>
        public void OnWeChatLogin()
        {
            if (IsNeedPopProtocol(UserAgementToggle))
            {
                return;
            } 
            UserController.Instance.WeChatLogin();
        }

        protected override void OnShow(object o)
        {
            base.OnShow(o);
            CurtainManager.CloseCurtain();
        }

        public static bool IsNeedPopProtocol(UIToggle toggle)
        {
            if (toggle == null) return false;
            if (toggle.value) return false;
            YxWindowManager.ShowMessageWindow("请同意用户协议，否则不能注册游戏！");
            return true;
        }
    }
}

