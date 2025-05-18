using Assets.Scripts.Common.UI;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.lyzz2d.Game;
using Assets.Scripts.Game.lyzz2d.Game.GameCtrl;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.View;

namespace Assets.Scripts.Game.lyzz2d.Utils.Windows
{
    public class Lyzz2DSettingWindow : SettingWindow
    {
        [Tooltip("语音开关")] public UIToggle ChatToggle;

        [Tooltip("解散按钮")] public UIButton DessolveButton;

        [Tooltip("返回按钮")] public GameObject ReturnButton;

        Lyzz2DGameManager manager
        {
            get { return App.GetGameManager<Lyzz2DGameManager>(); }
        }

        Lyzz2DGlobalData data
        {
            get { return App.GetGameData<Lyzz2DGlobalData>(); }
        }

        protected override void OnStart()
        {
            base.OnStart();
            ChatToggle.value = manager.ChatVioceToggle;
        }

        protected override void OnEnable()
        {
            var CreateRoom = data.CurrentGame.GameRoomType == -1;
            DessolveButton.gameObject.SetActive(CreateRoom);
            YxTools.TrySetComponentValue(ReturnButton, !CreateRoom);
            if (manager.IsInit())
            {
                DessolveButton.GetComponent<BoxCollider>().enabled = manager.SelfPlayer.UserInfo.id == data.CurrentGame.OwnerId;
            }
        }

        public void OnDissolveClick()
        {
            Close();
            YxMessageBox.Show(
                "确定解散房间吗?",
                null,
                (window, btnname) =>
                {
                    switch (btnname)
                    {
                        case YxMessageBox.BtnLeft:
                            App.GetRServer<Lyzz2DGameServer>().StartHandsUp(2); // -1拒绝 2发起投票 3同意
                            break;
                    }
                },
                true,
                YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle
                );
        }

        public void OnClickChatToggle()
        {
            manager.ChatVioceToggle = ChatToggle.value;
        }

        public void QuitGame()
        {
            if (manager.IsGameing())
            {
                YxMessageBox.Show("正在游戏中,不能退出游戏!");
                return;
            }
            App.QuitGameWithMsgBox();
        }
    }
}