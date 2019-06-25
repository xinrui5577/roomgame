using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Common;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.ShareWindow
{
    public class TaskShareView : TaskBasseView
    {
        //todo 临时使用
        /// <summary>
        /// 分享窗口
        /// </summary>
        public ShareWindow Window;

        public bool CheckWechatValidity(WeChatApi api)
        {
            if (!Application.isMobilePlatform)
            {
                YxMessageBox.Show("非移动设备暂时不支持分享！");
                return false;
            }
            if (!api.IsInstalledWechat())
            {
                YxMessageBox.Show("您的设备没有安装微信，请安装后再分享！");
                return false;
            }
            if (!api.IsCheckWechatApiLevel())
            {
                YxMessageBox.Show("您的设备上的微信版本不符合微信最低版本，\n请更新微信后再分享！");
                return false;
            }
            return true;
        }

        private void SendWechatShare(ShareInfo info)
        {
            var wechatApi = Facade.Instance<WeChatApi>();
            var appId = App.Config.WxAppId;
            if (string.IsNullOrEmpty(appId)) return;
            wechatApi.InitWechat(appId);
            wechatApi.ShareContent(info, Window.Shares[(int)info.Plat].OnShareSuccess, null, OnShareFailed);
        }

        private void OnShareFailed(string obj)
        {
            YxMessageBox.Show("非常抱歉，分享失败了！");
        }

        protected void OnShareSuccess(object msg)
        {
            //发送分享成功
            YxWindowManager.ShowWaitFor();
            var parm = new Dictionary<string, object>
                {
                {"option", 2},
                {"bundle_id", Application.bundleIdentifier},
                {"share_plat", ((int)SharePlat.WxSenceTimeLine).ToString(CultureInfo.InvariantCulture)},
            };
            Facade.Instance<TwManger>().SendAction("shareAwards", parm, str => YxWindowManager.HideWaitFor());
        }

        #region 朋友圈分享
        /// <summary>
        /// 朋友圈分享
        /// </summary>
        public void OnSharefriends()
        {
            var wechatApi = Facade.Instance<WeChatApi>();
            if (!CheckWechatValidity(wechatApi)) return;
            UserController.Instance.GetShareInfo(SendWechatShare, ShareType.Website, SharePlat.WxSenceTimeLine);
        }
        #endregion

        #region 会话分享

        /// <summary>
        /// 微信分享
        /// </summary>
        public void OnShareWeChat()
        {
            var wechatApi = Facade.Instance<WeChatApi>();
            if (!CheckWechatValidity(wechatApi)) return;
            UserController.Instance.GetShareInfo(SendWechatShare);
        }

        #endregion
    }
}
