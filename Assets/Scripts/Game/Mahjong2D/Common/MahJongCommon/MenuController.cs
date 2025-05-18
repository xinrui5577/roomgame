using System.Collections;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using Assets.Scripts.Game.Mahjong2D.Game.Component.GameTable;
using Assets.Scripts.Game.Mahjong2D.Game.Data;
using Assets.Scripts.Game.Mahjong2D.Game.GameCtrl;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Game.Mahjong2D.Common.MahJongCommon
{
    public class MenuController :MonoSingleton<MenuController>
    {
        public GameObject BackButton;
        public GameObject ChangeTableBtn;
        public GameObject AutoBtn;
        [SerializeField]
        private UIButton _weiFriendBtn;
        /// <summary>
        /// 微信分享与返回按钮的UI控制grid
        /// </summary>
        [SerializeField]
        private UIGrid _grid;

        private Mahjong2DGameManager Manager
        {
            get { return App.GetGameManager<Mahjong2DGameManager>(); }
        }

        private Mahjong2DGameData Data
        {
            get { return App.GetGameData<Mahjong2DGameData>(); }
        }


        public void ChangeBtnStatusAfterStartingGame()
        {
            bool createRoom = App.GetGameData<Mahjong2DGameData>().CurrentGame.IsCreateRoom;
            SetOtherBtnsState(App.GetGameManager<Mahjong2DGameManager>().IsInit);
            ChangeTableBtn.TrySetComponentValue(!createRoom);
            AutoBtn.TrySetComponentValue(!createRoom);
            GameTable.Instance.RoundLabel.TrySetComponentValue(Manager.GameType.ShowRoundInfo);
        }

        /// <summary>
        /// 返回大厅按钮
        /// </summary>
        public void OnReturnClick()
        {
            if (App.GetGameManager<Mahjong2DGameManager>().IsGameing)
            {
                YxMessageBox.Show("正在游戏中,不能退出游戏!");
                return;
            }
            App.QuitGameWithMsgBox();
        }

        /// <summary>
        /// 解散按钮
        /// </summary>
        public void OnClickHandsUp()
        {
            if (App.GetGameManager<Mahjong2DGameManager>().SelfPlayer.UserInfo.id.Equals(Data.OwnerId))
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
                                  App.GetRServer<Mahjong2DGameServer>().StartHandsUp(2);             // -1拒绝 2发起投票 3同意
                                  break;
                          }
                      },
                      true,
                      YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle
              );
            }
        }

        public void OnClickChangeTableBtn()
        {
            if(App.GetGameManager<Mahjong2DGameManager>().IsGameing)
            {
                YxMessageBox.Show("正在游戏中,不能换桌!");
                return;
            }
            App.GetRServer<Mahjong2DGameServer>().ChangeRoom();
        }

        public void SetOtherBtnsState(bool show)
        {
            BackButton.TrySetComponentValue(show);
            bool idIsEmpty = string.IsNullOrEmpty(App.Config.WxAppId);
            _weiFriendBtn.gameObject.SetActive(!idIsEmpty&&show);
            if (_grid!=null)
            {
                _grid.repositionNow = true;
            }
        }

        public void OnClickWeiChatBtn()
        {
            Facade.Instance<WeChatApi>().InitWechat(App.Config.WxAppId);
            UserController.Instance.GetShareInfo(info =>
            {
                info.ShareData["content"] += 
                string.Format(
                    "房间号:[{0}];{1},速来玩吧!",
                    App.GetGameData<Mahjong2DGameData>().CurrentGame.ShowRoomId
                    ,
                    App.GetGameData<Mahjong2DGameData>().CurrentGame.Rules
                );
                Facade.Instance<WeChatApi>().ShareContent(info);
            },ShareType.Website,SharePlat.WxSenceSession,null,App.GameKey);

        }
        private void SendWechatShare(ShareInfo info)
        {
            var wechatApi = Facade.Instance<WeChatApi>();
            string appId = App.Config.WxAppId;
            if (string.IsNullOrEmpty(appId)) return;
            wechatApi.InitWechat(appId);
            wechatApi.ShareContent(info, OnShareSuccess, null, OnShareFailed);
        }

        private void OnShareFailed(string obj)
        {
            YxMessageBox.Show("非常抱歉，分享失败了！");
        }

        private void OnShareSuccess(object msg)
        {
            var pram = (IDictionary)msg;
            if (!pram.Contains("coin")) return;
            var coin = int.Parse(pram["coin"].ToString());
            UserInfoModel.Instance.UserInfo.CoinA += coin;
            UserInfoModel.Instance.Save();
            YxMessageBox.Show(string.Format("恭喜您，分享成功！\n奖励了{0}金币！！！", coin));
        }
        private bool CheckWechatValidity(WeChatApi api)
        {
            if (!Application.isMobilePlatform)
            {
                YxMessageBox.Show("非移动设备暂时不支持分享！");
                return false;
            }
            if (!api.IsInstalledWechat())
            {
                YxMessageBox.Show("您的设备没有安装微信，请安装后再分享！");
                return false;
            }
            if (!api.IsCheckWechatApiLevel())
            {
                YxMessageBox.Show("您的设备上的微信版本不符合微信最低版本，\n请更新微信后再分享！");
                return false;
            }
            return true;
        }
    }
}
