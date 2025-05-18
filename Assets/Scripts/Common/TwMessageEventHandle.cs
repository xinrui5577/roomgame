using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using YxFramwork.Common.Interfaces;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Common
{
    /// <summary>
    /// 轮询事件
    /// </summary>
    public class TwMessageEventHandle : IEventHandle
    {
        public void Handle(int type, TwMessageEventData data)
        {
            switch (type)
            {
                case (int)TwMessageEventType.SendUserType:
                    UserSendCallBack(data);
                    break;
                case (int)TwMessageEventType.SendedUserType:
                    UserSendedCallBack(data);
                    break;
                default:
                    YxDebug.LogError("没有相关【{0}】事件处理", "TWMessageManager", null, data);
                    return;
            }
        }

        private void UserSendCallBack(TwMessageEventData data)
        {
            var messageBox = string.Format("MessageBox_{0}", data.MessageBoxType);
            YxMessageBox.Show(null, messageBox, data.Message);
        }
        private void UserSendedCallBack(TwMessageEventData data)
        {
            var messageBox = string.Format("MessageBox_{0}", data.MessageBoxType);
            YxMessageBox.Show(null, messageBox, data.Message, "", (mesBox, btnName) =>
            {
                var dic = new Dictionary<string, object>();
                var baseData = data.Data as Dictionary<string, object>;
                if (baseData == null) return;
                dic["id"] = baseData["id"];
                dic["AffilateID"] = baseData["send_user_id"];
                switch (btnName)
                {
                    case YxMessageBox.BtnLeft:
                        dic["type"] = 0;//同意
                        break;
                    case YxMessageBox.BtnRight:
                        dic["type"] = 1;//拒绝
                        break;
                    case "BtnClose":
                        dic["type"] = 2;//忽略
                        break;
                }
                Facade.Instance<TwManager>().SendAction("mahjongwm.sendItemSelect", dic, null);
            }, true, YxMessageBox.RightBtnStyle | YxMessageBox.LeftBtnStyle);
        }

    }

    public enum TwMessageEventType
    {
        SendUserType = 2,
        SendedUserType = 3,
    }
}
