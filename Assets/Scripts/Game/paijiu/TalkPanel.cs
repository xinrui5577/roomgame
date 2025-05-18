//using Assets.Scripts.Common.Windows;
//using Assets.Scripts.Game.paijiu.ImgPress.Main;
//using YxFramwork.Common;
//
//namespace Assets.Scripts.Game.paijiu
//{
//    public class TalkPanel : YxNguiWindow
//    {
//        /// <summary>
//        /// 输入框
//        /// </summary>
//        public UIInput TextInput;
//        /// <summary>
//        /// 当点击发送按键
//        /// </summary>
//        public void OnClickSendBtn()
//        {
//            string showValue = TextInput.value;
//            if (string.IsNullOrEmpty(showValue))
//            {
//                return;
//            }
//            App.GetRServer<PaiJiuGameServer>().SendPlayerInputString(showValue);
//            TextInput.value = "";
//            Close();
//            
//        }
//
//
//        public void OnClickCloseBtn()
//        {
//            Close();
//        }
//
//        /// <summary>
//        /// 打开聊天面板
//        /// </summary>
//        public void OpenPanel()
//        {
//            gameObject.SetActive(true);
//        }
//
//        // ReSharper disable once ArrangeTypeMemberModifiers
//        // ReSharper disable once UnusedMember.Local
//        void Awake()
//        {
//            PhizItem.OnItemClick = Close;
//            NormalTalkItem.OnItemClick = Close;
//        }
//    }
//}
