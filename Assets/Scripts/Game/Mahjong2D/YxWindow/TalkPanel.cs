using Assets.Scripts.Common.Windows;
using Assets.Scripts.Game.Mahjong2D.Game.GameCtrl;
using Assets.Scripts.Game.Mahjong2D.Game.Item;
using YxFramwork.Common;
using YxFramwork.View;

namespace Assets.Scripts.Game.Mahjong2D.YxWindow
{
    public class TalkPanel : YxNguiWindow
    {
        /// <summary>
        /// 输入框
        /// </summary>
        public UIInput TextInput;
        /// <summary>
        /// 玩家说的话
        /// </summary>
        public UILabel[] ChatInfo;
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
            if (App.GameKey.Equals("ykmj"))
                App.GetRServer<Mahjong2DGameServer>().SendYkHandUserTalk(showValue);
            else
                App.GetRServer<Mahjong2DGameServer>().SendUserTalk(showValue);
            TextInput.value = "";
            Close();
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
