using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Game.Ttzkf
{
    public class MenuCtrl : MonoBehaviour
    {
        /// <summary>
        /// 黑色背景（包含下面的按钮）
        /// </summary>
        public GameObject Bg;
        /// <summary>
        /// 右边的微信邀请的按钮
        /// </summary>
        public GameObject WeiChatInvateBtnRight;
        /// <summary>
        /// 离开退出道大厅
        /// </summary>
        public GameObject QuitGameBackHall;
        /// <summary>
        /// 设置界面
        /// </summary>
        public SettingCtrl SettingPanel;
        /// <summary>
        /// 规则界面
        /// </summary>
        public RuleCtrl RulePanel;
        /// <summary>
        /// 列表Grid
        /// </summary>
        public UIGrid MenuGrid;
        /// <summary>
        /// 解散房间按钮
        /// </summary>
        public GameObject Dissolve;
        /// <summary>
        /// 菜单按钮
        /// </summary>
        public GameObject MenuBtn;

        protected void Start()
        {
            if (Bg.activeSelf)
            {
                Bg.SetActive(false);
            }
        }
        /// <summary>
        /// 创建房间第一次显示其他玩家
        /// </summary>
        public void CreatRoomOtherShow()
        {
            QuitGameBackHall.SetActive(true);
            Dissolve.SetActive(false);
            WeiChatInvateBtnRight.SetActive(false);
            MenuGrid.repositionNow = true;
        }
        /// <summary>
        /// 创建房间正常显示
        /// </summary>
        public void CreatRoomNormalShow()
        {
            QuitGameBackHall.SetActive(false);
            Dissolve.SetActive(true);
            WeiChatInvateBtnRight.SetActive(false);
            MenuGrid.repositionNow = true;
        }
        /// <summary>
        /// 创建房间第一次显示roomOwner
        /// </summary>
        public void CreatRoomShow()
        {
            QuitGameBackHall.SetActive(false);
            WeiChatInvateBtnRight.SetActive(false);
            MenuGrid.repositionNow = true;
        }
        /// <summary>
        /// 创建房间重连显示
        /// </summary>
        public void NormalShow()
        {
            QuitGameBackHall.SetActive(false);
            WeiChatInvateBtnRight.SetActive(false);
            MenuGrid.repositionNow = true;
        }
        /// <summary>
        /// 娱乐房显示
        /// </summary>
        public void SpecialShow()
        {
            WeiChatInvateBtnRight.SetActive(false);
            Dissolve.SetActive(false);
            MenuGrid.repositionNow = true;
        }
        /// <summary>
        /// 点击菜单按钮
        /// </summary>
        public void OnClickMenuBtn()
        {
            MenuBtn.transform.localEulerAngles = MenuBtn.transform.localEulerAngles != Vector3.zero ? Vector3.zero : new Vector3(0, 0, 180);
            Bg.SetActive(!Bg.activeSelf);
        }

        /// <summary>
        /// 点击设置按钮
        /// </summary>
        public void OnClickSettingBtn()
        {
            SettingPanel.OnClickSettingBtn();
        }
        /// <summary>
        /// 点击离开房间按钮
        /// </summary>
        public void OnClickQuitGameBtn()
        {
            Facade.Instance<MusicManager>().Play(GameSounds.ChangeRoom);


            if (App.GameData.GStatus != YxEGameStatus.PlayAndConfine)
            {
                YxMessageBox.Show("确定要退出大厅吗?", "", (box, btnName) =>
                {
                    if (btnName == YxMessageBox.BtnLeft)
                    {
                        App.QuitGame();
                    }
                }, true, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
            }
            else
            {
                YxMessageBox.Show("您已经下注，请稍后退出",
                           null,
                           (window, btnname) =>
                           {
                           },
                           true,
                           YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle
                          );
            }

        }

        public void OnClickWeiXinChat()
        {
            var data = App.GetGameData<TtzGameData>();
            YxTools.ShareFriend(data.RoomType.ToString(), data.Rule);
        }

        /// <summary>
        /// 点击微信邀请按钮
        /// </summary>
        public void OnClickWeiChatInviteBtn()
        {
            Facade.Instance<WeChatApi>().InitWechat(App.Config.WxAppId);

            YxFramwork.Controller.UserController.Instance.GetShareInfo(info =>
            {
                info.ShareData["title"] = App.GameData.GetPlayerInfo().NickM + "-" + info.ShareData["title"];
                info.ShareData["content"] +=
                string.Format(
                    "[扑克];房间号:[{0}];{1}",
                    App.GetGameData<TtzGameData>().RoomType
                    ,
                    App.GetGameData<TtzGameData>().Rule
                );
                info.ShareData["content"] += "。速来玩吧! (仅供娱乐，禁止赌博)";
                Facade.Instance<WeChatApi>().ShareContent(info);
            });
        }
        /// <summary>
        /// 点击游戏规则按钮
        /// </summary>
        public void OnClickGameRuleBtn()
        {
            RulePanel.OnClickRuleBtn();
        }
        /// <summary>
        /// 解散房间
        /// </summary>
        public void OnDissolveClick()
        {
            var gdata = App.GetGameData<TtzGameData>();

            if (gdata.SelfSeat != 0 && gdata.CurrentRound == 0)
            {
                App.QuitGame();
            }
            else
            {
                YxMessageBox.Show(
                "确定解散房间吗?",
                null,
                (window, btnname) =>
                {
                    switch (btnname)
                    {
                        case YxMessageBox.BtnLeft:
                            App.GetRServer<TtzGameServer>().StartHandsUp(2);             // -1拒绝 2发起投票 3同意                            
                            break;
                    }
                },
                true,
                YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle
                );
            }

        }
        public void Zhanji()
        {
            App.GetGameManager<TtzGameManager>().Result.ShowBg(true);
        }
    }
}
