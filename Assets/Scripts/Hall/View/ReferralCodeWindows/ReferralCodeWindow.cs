using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.ReferralCodeWindows
{
    public class ReferralCodeWindow : YxNguiWindow
    {
        public UIInput InputLable;

        public void OnSendInput()
        {
            if (InputLable.value == "")
            {
                var message = new YxMessageBoxData
                    {
                        Msg = "您所填写的信息不能为空",
                        //                        BtnStyle = YxMessageBox.MiddleBtnStyle,
                        //                        Listener = (box, btnName) =>
                        //                            {
                        //                                if (btnName == YxMessageBox.BtnMiddle)
                        //                                {
                        //                                    Close();
                        //                                }
                        //                            },
                    };
                YxMessageBox.Show(message);
                return;
            }
            var dic = new Dictionary<string, object>();
            dic["AffilateID"] = InputLable.value;
            Facade.Instance<TwManger>().SendAction("getAffilateRelationAward", dic, OnFreshInfo);
        }

        private void OnFreshInfo(object data)
        {
            print(data);
        }
    }
}
