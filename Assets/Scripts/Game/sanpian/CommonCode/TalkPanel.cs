using Assets.Scripts.Common.Windows;
using Assets.Scripts.Game.sanpian.server;
using YxFramwork.Common;
using YxFramwork.View;

namespace Assets.Scripts.Game.sanpian.CommonCode
{
    public class TalkPanel : YxNguiWindow
    {
        /// <summary>
        /// 输入框
        /// </summary>
        public UIInput TextInput;
        /// <summary>
        /// 当点击发送按键
        /// </summary>
        public void OnClickSendBtn()
        {
            string showValue = TextInput.value;
            if (string.IsNullOrEmpty(showValue))
            {
                YxMessageBox.Show("输入不能为空");
                return;
            }
            App.GetRServer<SanPianGameServer>().SendUserTalk(showValue);
            TextInput.value = "";
        }
        /// <summary>
        /// 打开聊天面板
        /// </summary>
        public void OpenPanel()
        {
            gameObject.SetActive(true);
        }

        void Awake()
        {
            PhizItem.OnItemClick = Close;
            NormalTalkItem.OnItemClick = Close;
        }
    }
}
