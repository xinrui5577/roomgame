using System.Collections;
using System.Collections.Generic;
using YxFramwork.Common.Model;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.TaskWindows
{
    public class TaskBindWechatView : TaskBasseView
    {
        /// <summary>
        ///微信输入
        /// </summary>
        public UIInput WechatInput;
        public UIInput WechatReInput;
        /// <summary>
        /// 微信显示
        /// </summary>
        public UILabel WechatLabel;
        protected override void OnShow()
        {
            base.OnShow();
            var weChatS = UserInfoModel.Instance.UserInfo.WeChatS;
            ChangeState(!string.IsNullOrEmpty(weChatS));
            WechatLabel.text = weChatS;
        }

      
        /// <summary>
        /// 发送绑定手机
        /// </summary>
        public void OnSendBindWechat()
        {
            var weixin = WechatInput.value;
            if (!weixin.Equals(WechatReInput.value))
            {
                YxMessageBox.Show("两次输入的微信号不相同，请重新输入", 5);
                return;
            }
            var parm = new Dictionary<string, object>();
            parm["weixin"] = weixin;
            Facade.Instance<TwManger>().SendAction("getBindWeiXinAward", parm, BoundWechatSuccess);
        }

        /// <summary>
        /// 绑定微信成功
        /// </summary>
        /// <param name="msg"></param>
        private void BoundWechatSuccess(object msg)
        {
            var pram = (IDictionary)msg;
            if (!pram.Contains("coin")) return;
            var coin = int.Parse(pram["coin"].ToString());
            if (coin > 0)
            {
                UserInfoModel.Instance.UserInfo.CoinA += coin;
                ShowInfos(msg,string.Format("恭喜您，首次绑定微信成功！！！\n奖励{0}金币！！", coin));
                var weixin = WechatInput.value;
                UserInfoModel.Instance.UserInfo.WeChatS = weixin;
                WechatLabel.text = weixin;
                ChangeState(true);
                UserInfoModel.Instance.Save(); 
                return;
            }
            ShowInfos(msg,"恭喜您，绑定微信成功！！！");
        }

        /// <summary>
        /// 发送解绑手机
        /// </summary>
        public void OnSendUnBindWechat()
        {
            var weixin = WechatLabel.text; 
            var parm = new Dictionary<string, object>();
            parm["weixin"] = weixin;
            Facade.Instance<TwManger>().SendAction("getUnBindWeiXin", parm, UnBoundWechatSuccess);
        }

        /// <summary>
        /// 绑定手机成功
        /// </summary>
        /// <param name="msg"></param>
        private void UnBoundWechatSuccess(object msg)
        {
            ShowInfos(msg, "该账号已经解除微信绑定！！！");
            UserInfoModel.Instance.UserInfo.MobileN = "";
            WechatLabel.text = "";
            ChangeState(false);
        }
    }
}
