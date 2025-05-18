using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Hall.View
{
    /// <summary>
    /// 
    /// </summary>
    public class TicklingWindow : YxNguiWindow
    {
        /// <summary>
        /// 反馈内容文本
        /// </summary>
        public UIInput MessageLabel;
         
        public void OnTicklingBtnClick()
        {
            if (MessageLabel == null) return;
            var msg = MessageLabel.value;
            if (string.IsNullOrEmpty(msg))
            {
                YxMessageBox.Show("亲，真的没有话说吗?请留下宝贵意见！！");
                return;
            }
            //todo 发送到服务器
            YxDebug.Log(msg);
            var parm = new Dictionary<string, object>();
            parm["msg"] = msg;
            Facade.Instance<TwManager>().SendAction("recAdvice", parm, reMsg =>
                {
                }, false);
        }
    }
}
