using Assets.Scripts.Common.UI;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.Game.Data;
using Assets.Scripts.Game.Mahjong2D.Game.GameCtrl;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.View;

namespace Assets.Scripts.Game.Mahjong2D.YxWindow 
{
    public class Mahjong2DSettingWindow :SettingWindow
    {
        [Tooltip("解散按钮")]
        public UIButton DessolveButton;
        [Tooltip(" 语音开关")]
        public UIToggle ChatToggle;
        [Tooltip("按钮布局Grid,用于兼容娱乐与房卡模式按钮的布局")]
        public UIGrid BtnGrid;

        private Mahjong2DGameData Data
        {
            get { return App.GetGameData<Mahjong2DGameData>(); }
        }

        private Mahjong2DGameManager Manager
        {
            get { return App.GetGameManager<Mahjong2DGameManager>(); }
        }

        private Mahjong2DGameServer Server
        {
            get { return App.GetRServer<Mahjong2DGameServer>(); }
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            if (ChatToggle)
            {
                ChatToggle.value = App.GetGameManager<Mahjong2DGameManager>().ChatVioceToggle;
            }
        }
        /// <summary>
        /// 在显示时进行处理，控制解散房间按钮的显示状态
        /// </summary>
        protected override void OnShow()
        {
            if (Data==null|| Manager==null)
            {
                return;
            }
            bool createRoom =Data.CurrentGame.IsCreateRoom;
            if (DessolveButton)
            {
                DessolveButton.gameObject.TrySetComponentValue(createRoom);
                if (Data.IsFirstTime)
                {
                    if (Manager.SelfPlayer && Manager.SelfPlayer.UserInfo != null)
                    {
                        DessolveButton.GetComponent<BoxCollider>().enabled = Manager.SelfPlayer.UserInfo.id.Equals(Data.OwnerId);
                    }
                }
            }

            if (BtnGrid!=null)
            {
                BtnGrid.repositionNow = true;
            }
        }

        public void OnDissolveClick()
        {
            if ( Server == null)
            {
                return;
            }
            Close();
            YxMessageBox.Show(
                "确定解散房间吗?",
                null,
                (window, btnname) =>
                {
                    switch (btnname)
                    {
                        case YxMessageBox.BtnLeft:
                            Server.StartHandsUp(2);             // -1拒绝 2发起投票 3同意                            
                            break;
                    }               
                },
                true,
                YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle
                );
        }

        public void OnClickChatToggle()
        {
            if (Manager&& ChatToggle)
            {
                Manager.ChatVioceToggle = ChatToggle.value;
            }
            
        }

        /// <summary>
        /// 返回大厅按钮
        /// </summary>
        public void OnReturnClick()
        {
            if (App.GetGameManager<Mahjong2DGameManager>().IsGameing||!Data.IsFirstTime)
            {
                YxMessageBox.Show("正在游戏中,不能退出游戏!");
                return;
            }
            App.QuitGameWithMsgBox();
        }

        public void OnClickChangeBgBtn()
        {
            Facade.EventCenter.DispatchEvent(ConstantData.KeyNowBgIndex,1);
        }

    }
}
