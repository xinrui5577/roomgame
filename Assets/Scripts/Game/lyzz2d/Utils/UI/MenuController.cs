using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.lyzz2d.Game;
using Assets.Scripts.Game.lyzz2d.Game.Component;
using Assets.Scripts.Game.lyzz2d.Game.GameCtrl;
using Assets.Scripts.Game.lyzz2d.Utils.Single;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.View;

namespace Assets.Scripts.Game.lyzz2d.Utils.UI
{
    public class MenuController : MonoSingleton<MenuController>
    {
        [SerializeField] private UIButton _weiFriendBtn;

        public GameObject AutoBtn;
        public GameObject BackButton;
        public GameObject ChangeTableBtn;

        [Tooltip("桌面显示按钮")] public UIGrid TableGrid;

        private Lyzz2DGameManager Manager
        {
            get { return App.GetGameManager<Lyzz2DGameManager>(); }
        }

        public void ChangeBtnStatusAfterStartingGame()
        {
            var createRoom = Manager.Data.CurrentGame.GameRoomType == -1;
            SetOtherBtnsState(App.GetGameManager<Lyzz2DGameManager>().IsInit());
            ChangeTableBtn.SetActive(!createRoom);
            AutoBtn.SetActive(!createRoom);
            GameTable.Instance.LeftCardParent.SetActive(App.GetGameManager<Lyzz2DGameManager>().IsGameing());
            GameTable.Instance.RoundLabel.text = Manager.GameType.ShowRoundInfo;
        }

        public void OnReturnClick()
        {
            var createRoom = Manager.Data.CurrentGame.GameRoomType == -1;
            if (createRoom)
            {
                if (App.GetGameManager<Lyzz2DGameManager>().SelfPlayer.UserSeat == 0)
                {
                    YxMessageBox.Show
                        (
                            "确定退出游戏吗?",
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
                    return;
                }
            }
            else
            {
                if (App.GetGameManager<Lyzz2DGameManager>().IsGameing())
                {
                    YxMessageBox.Show("正在游戏中,不能退出游戏!");
                    return;
                }
            }
            App.QuitGameWithMsgBox();
        }

        public void OnClickAutoBtn()
        {
            YxMessageBox.Show("托管功能暂未开启");
        }

        public void OnClickChangeTableBtn()
        {
            if (App.GetGameManager<Lyzz2DGameManager>().IsGameing())
            {
                YxMessageBox.Show("正在游戏中,不能换桌!");
                return;
            }
            App.GetRServer<Lyzz2DGameServer>().ChangeRoom();
        }


        public void SetOtherBtnsState(bool show)
        {
            var createRoom = Manager.Data.CurrentGame.GameRoomType == -1;
            _weiFriendBtn.gameObject.SetActive(createRoom && show);
            BackButton.SetActive(show);
            if (TableGrid)
            {
                TableGrid.repositionNow = true;
            }
        }

        public void OnClickWeiChatBtn()
        {
            var CurrentGame = Manager.Data.CurrentGame;
            if (CurrentGame != null)
            {
                YxTools.ShareFriend(CurrentGame.ShowRoomId.ToString(), CurrentGame.RuleInfo);
            }
        }
    }
}