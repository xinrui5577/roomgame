using Assets.Scripts.Common.Windows;
using Assets.Scripts.Game.lyzz2d.Game.GameCtrl;
using Assets.Scripts.Game.lyzz2d.Game.Item;
using YxFramwork.Common;
using YxFramwork.View;

namespace Assets.Scripts.Game.lyzz2d.Game.Talk
{
    public class TalkPanel : YxNguiWindow
    {
        /// <summary>
        ///     输入框
        /// </summary>
        public UIInput TextInput;

        /// <summary>
        ///     当点击发送按键
        /// </summary>
        public void OnClickSendBtn()
        {
            var showValue = TextInput.value;
            if (string.IsNullOrEmpty(showValue))
            {
                YxMessageBox.Show("输入不能为空");
                return;
            }
            App.GetRServer<Lyzz2DGameServer>().SendUserTalk(showValue);
            TextInput.value = "";
            Close();
        }

        /// <summary>
        ///     打开聊天面板
        /// </summary>
        public void OpenPanel()
        {
            gameObject.SetActive(true);
        }

        private void Awake()
        {
            PhizItem.OnItemClick = Close;
            NormalTalkItem.OnItemClick = Close;
        }
    }
}