using System.Linq;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Game.Tbs
{
    public class MenuMgr : MonoBehaviour
    {
        /// <summary>
        /// 菜单对象
        /// </summary>
        public GameObject Menu;
        /// <summary>
        /// 菜单按键
        /// </summary>
        public UIButton MenuBtn;
        /// <summary>
        /// 当前菜单状态
        /// </summary>
        public bool IsMenuType = false;
        /// <summary>
        /// 帮助对象
        /// </summary>
        public GameObject Help;
        /// <summary>
        /// 设置对象
        /// </summary>
        public GameObject Setting;
        /// <summary>
        /// 邀请按钮
        /// </summary>
        public GameObject InviteBtn;

        protected void Start()
        {
            Refresh();
        }



        public void OpenMenu()
        {
            MenuBtn.GetComponent<UIButton>().normalSprite = "btn_up_n";
            Menu.gameObject.SetActive(true);
            IsMenuType = true;
        }

        public void CloseMenu()
        {
            MenuBtn.GetComponent<UIButton>().normalSprite = "btn_down_n";
            Menu.gameObject.SetActive(false);
            IsMenuType = false;
        }

        public void OnClickMenuBtn()
        {
            if (IsMenuType)
                CloseMenu();
            else
                OpenMenu();
        }

        public void OpenHelp()
        {
            Help.SetActive(true);
        }

        public void CloseHelp()
        {
            Help.SetActive(false);
        }

        public void OpenSetting()
        {
            Setting.SetActive(true);
        }

        public void CloseSetting()
        {
            Setting.SetActive(false);
        }

        public void Exit()
        {
            var gdata = App.GetGameData<TbsGameData>();
            if (gdata.IsCreatRoom)
            {
                var existUserNum = gdata.PlayerList.Count(t => t.GetComponent<TbsUserInfo>() != null);
                if (existUserNum > 1)
                {
                    if (!gdata.GetPlayer<TbsPlayer>().IsExit)
                    {
                        YxMessageBox.Show("正在游戏中，请勿退出");
                    }
                    else
                    {
                        //退出房间
                        YxMessageBox.Show("确定要退出游戏吗？", "", (box, btnName) =>
                        {
                            if (btnName == YxMessageBox.BtnLeft)
                            {
                                //如果是庄家退出,先发送切庄
                                if (gdata.BankerSeat == App.GameData.SelfSeat)
                                {
                                    App.GetRServer<TbsRemoteController>().SendRequest(GameRequestType.Banker, null);
                                }
                                App.QuitGame();
                            }
                        }, true, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
                    }
                }
                else
                {
                    YxMessageBox.Show("确定解散房间吗?", null, (window, btnname) =>
                    {
                        switch (btnname)
                        {
                            case YxMessageBox.BtnLeft:
                                App.GetRServer<TbsRemoteController>().StartHandsUp(2);             // -1拒绝 2发起投票 3同意                            
                                break;
                        }
                    },
              true,
              YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle
              );
                }
            }
            else
            {
                if (App.GameData.SelfSeat == -1)
                {
                    return;
                }

                if (!gdata.GetPlayer<TbsPlayer>().IsExit)
                {
                    YxMessageBox.Show("正在游戏中，请勿退出");
                }
                else
                {
                    //退出房间
                    YxMessageBox.Show("确定要退出游戏吗？", "", (box, btnName) =>
                    {
                        if (btnName == YxMessageBox.BtnLeft)
                        {
                            //如果是庄家退出,先发送切庄
                            if (gdata.BankerSeat == App.GameData.SelfSeat)
                            {
                                App.GetRServer<TbsRemoteController>().SendRequest(GameRequestType.Banker, null);
                            }
                            App.QuitGame();
                        }
                    }, true, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
                }
            }
        }

     

        public void Refresh()
        {

            MusicSlider.value = Facade.Instance<MusicManager>().MusicVolume;
            SoundSlider.value = Facade.Instance<MusicManager>().EffectVolume;
        }
        /// <summary>
        /// 音乐音量进度
        /// </summary>
        public UISlider MusicSlider;
        /// <summary>
        /// 音效音量进度
        /// </summary>
        public UISlider SoundSlider;

        public void OnMusicChange()
        {
            Facade.Instance<MusicManager>().MusicVolume = MusicSlider.value;
        }

        public void OnSoundChange()
        {
            Facade.Instance<MusicManager>().EffectVolume = SoundSlider.value;
        }

        /// <summary>
        /// 微信邀请好友
        /// </summary>
        public void OnInviteFriend()
        {
            Facade.Instance<WeChatApi>().InitWechat(App.Config.WxAppId);

            UserController.Instance.GetShareInfo(info =>
            {
                info.ShareData["content"] +=
                string.Format(
                    "[扑克];房间号:[{0}];{1}",
                    App.GetGameData<TbsGameData>().RoomType
                    ,
                    App.GetGameData<TbsGameData>().GameRule
                );
                info.ShareData["content"] += "速来玩吧!";

                Facade.Instance<WeChatApi>().ShareContent(info);
            });
        }
    }
}
