using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Controller;
using YxFramwork.View;

namespace Assets.Scripts.RedPacket
{
    public class YxNguiRedPacketWindow : YxNguiWindow
    {
        public bool IsMainPanel = false;
        public string ShowOtherWindowName;
        public string ShowOtherPanelName;

        protected virtual void Update()
        {
            if ( Input.GetKeyDown(KeyCode.Escape)) // 返回键
            {
                Close();
            }
        }

        public override void Close()
        {
            if (IsMainPanel)
            {
                YxMessageBox.Show(
                    "当前账号退出登录?",
                    null,
                    (window, btnname) =>
                    {
                        switch (btnname)
                        {
                            case YxMessageBox.BtnLeft:
                                HallMainController.Instance.ChangeAccount();
                                break;
                        }
                    },
                    true,
                    YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle
                );
                return;
            }
            if (!string.IsNullOrEmpty(ShowOtherWindowName))
            {
                ShowOtherWindow(ShowOtherWindowName);
            }

            if (!string.IsNullOrEmpty(ShowOtherPanelName))
            {
                ShowOtherPanel(ShowOtherPanelName);
            }
            base.Close();
        }
    }
}
