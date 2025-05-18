using Assets.Scripts.Common.Windows;
using Assets.Scripts.Game.fillpit.ImgPress.Main;
using YxFramwork.Common;
using YxFramwork.View;

namespace Assets.Scripts.Game.fillpit
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
                YxMessageBox.Show(new YxMessageBoxData()
                {
                    Msg = "不能发送空消息!?",
                    IsTopShow = true,
                    ShowBtnNames = new [] { YxMessageBox.BtnMiddle },
                });
                return;
            }
            App.GetRServer<FillpitGameServer>().SendPlayerInputString(showValue);
            TextInput.value = "";
            Close();
            
        }


        public void OnClickCloseBtn()
        {
            Close();
        }

        /// <summary>
        /// 打开聊天面板
        /// </summary>
        public void OpenPanel()
        {
            gameObject.SetActive(true);
        }

        protected void Awake()
        {
            PhizItem.OnItemClick = Close;
            NormalTalkItem.OnItemClick = Close;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            PhizItem.OnItemClick = null;
            NormalTalkItem.OnItemClick = null;
        }
    }
}
