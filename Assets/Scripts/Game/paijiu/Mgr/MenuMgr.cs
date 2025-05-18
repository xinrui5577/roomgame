using Assets.Scripts.Game.paijiu.Tool;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using System;
using System.Diagnostics.CodeAnalysis;
using Assets.Scripts.Game.paijiu.ImgPress.Main;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Manager;
using YxFramwork.View;



namespace Assets.Scripts.Game.paijiu.Mgr
{
    public class MenuMgr : MonoBehaviour
    {
        /// <summary>
        /// 菜单按键数组
        /// </summary>
        public GameObject[] Buttons;
        /// <summary>
        /// 菜单父节点
        /// </summary>
        public GameObject CloseBtn;
        /// <summary>
        /// 菜单背景tween
        /// </summary>
        public TweenScale MenuListBgTween;
        /// <summary>
        /// 按键的tweens
        /// </summary>
        public GameObject[] BtnTweens;

        public GameObject BackBtn;

        public GameObject ChangeBtn;

        public UIGrid MenuPanelGrid;

        public GameObject PanelSetting;

        public GameObject PanelHelp;


        // Use this for initialization
        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once UnusedMember.Local
        void Start()
        {
            InitOnClick();
        }

        /// <summary>
        /// 初始化按键监听
        /// </summary>
        public void InitOnClick()
        {
            foreach (MenuBtn btnid in Enum.GetValues(typeof(MenuBtn)))
            {
                foreach (GameObject btn in Buttons)
                {
                    if (btn.name.Equals(btnid.ToString()))
                    {
                        Tools.NguiAddOnClick(btn, OnClickListener, (int)btnid);
                        break;
                    }
                }
            }
            MenuPanelGrid.Reposition();
        }

        protected void OnClickListener(GameObject gob)
        {
            MenuBtn btnid = (MenuBtn)UIEventListener.Get(gob).parameter;

            switch (btnid)
            {
                case MenuBtn.Menu_Btn:
                    CloseBtn.SetActive(true);

                    foreach (GameObject tween in BtnTweens)
                    {
                        tween.GetComponent<TweenColor>().ResetToBeginning();
                        tween.GetComponent<TweenScale>().ResetToBeginning();
                    }

                    if (App.GetGameData<PaiJiuGameData>().IsRoomGame)
                    {
                        if (BackBtn != null)
                        {
                            if (App.GameData.SelfSeat == 0 || App.GetGameData<PaiJiuGameData>().IsPlayed)
                            {
                                string sprName = "dismiss";
                                UIButton btn = BackBtn.GetComponent<UIButton>();
                                btn.normalSprite = sprName + "_up";
                                btn.pressedSprite = sprName + "_over";
                                btn.hoverSprite = sprName + "_over";
                                btn.disabledSprite = sprName + "_up";
                            }
                        }

                        if (ChangeBtn != null)
                        {
                            ChangeBtn.SetActive(false);
                        }

                        MenuPanelGrid.cellHeight = 100;
                    }

                    MenuListBgTween.PlayForward();
                    break;

                case MenuBtn.Back_Btn:
                    //开放模式下,走解散房间,否则更换房间
                    if (App.GetGameData<PaiJiuGameData>().IsRoomGame && (App.GameData.SelfSeat == 0 || App.GetGameData<PaiJiuGameData>().IsPlayed))
                    {
                        DismissRoom();
                    }
                    else
                    {
                        QuitRoom();
                    }
                    CloseMenu();
                    break;

                case MenuBtn.Change_Btn:
                    ChangeRoom();
                    CloseMenu();
                    break;
                case MenuBtn.Help_Btn:
                    OnClickHelpBtn();
                    CloseMenu();
                    break;
                case MenuBtn.Close_Btn:
                    CloseMenu();
                    break;

                case MenuBtn.Setting_Btn:
                    OnClickSettingBtn();
                    CloseMenu();
                    break;

                case MenuBtn.History_Btn:
                    CloseMenu();
                    break;
            }
        }


        /// <summary>
        /// 投票解散房间
        /// </summary>
        private void DismissRoom()
        {

            //用于测试
            //YxDebug.Log(" ============= DismissRoom ========== ");
            //App.GetRServer<DuifenGameServer>().DismissRoom(2);
            //return;
            //

            DismissRoomMgr dismissRoomMgr = App.GetGameManager<PaiJiuGameManager>().DismissRoomMgr;
            //游戏开始了就需要发起投票，否则玩家可以自主决定
            if (App.GetGameData<PaiJiuGameData>().IsPlayed)
            {
                YxMessageBox.Show(new YxMessageBoxData()
                {
                    Msg = "确定要发起投票,解散房间么?",
                    BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle,
                    IsTopShow = true,
                    Listener = (box, btnName) =>
                    {
                        if (btnName == YxMessageBox.BtnLeft)
                        {
                            if (!dismissRoomMgr.Container.activeSelf)
                            {
                                App.GetRServer<PaiJiuGameServer>().DismissRoom(2);
                                App.GetRServer<PaiJiuGameServer>().DismissRoom(3);
                            }
                            else
                            {
                                Debug.Log("投票已发起!");
                                YxMessageBox.Show(new YxMessageBoxData
                                {
                                    Msg = "投票已发起!",
                                    Delayed = 5,
                                });
                                dismissRoomMgr.Container.transform.GetChild(0).gameObject.SetActive(true);
                            }
                        }
                    },
                });
            }
            else
            {
                //房主可以解散房间,玩家则是自己退出房间
                if (App.GameData.SelfSeat == 0)
                {

                    YxMessageBox.Show(new YxMessageBoxData()
                    {
                        Msg = "确定要解散房间吗?",
                        BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle,
                        IsTopShow = true,
                        Listener = (box, btnName) =>
                        {
                            if (btnName == YxMessageBox.BtnLeft)
                            {
                                IRequest req = new ExtensionRequest("dissolve", new SFSObject());
                                App.GetRServer<PaiJiuGameServer>().SendRequest(req);
                            }
                        },
                    });

                }
                else
                {
                    YxMessageBox.Show(new YxMessageBoxData()
                    {
                        Msg = "确定要退出房间么?",
                        BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle,
                        IsTopShow = true,
                        Listener = (box, btnName) =>
                        {
                            if (btnName == YxMessageBox.BtnLeft)
                            {
                                App.QuitGame();
                            }
                        },
                    });
                }
            }
        }

        /// <summary>
        /// 退出房间
        /// </summary>
        private void QuitRoom()
        {

            if (App.GetGameData<PaiJiuGameData>().IsGameing && App.GetGameData<PaiJiuGameData>().GetPlayer<PaiJiuPlayer>().IsReady)
            {
                YxDebug.Log(" === 正在游戏中，结束后再退出!!! === ");
                YxMessageBox.Show(new YxMessageBoxData()
                {
                    Msg = "正在游戏中，结束后再退出!!!",
                    IsTopShow = true,
                    Delayed = 5,
                });
            }
            else
            {
                YxDebug.Log(" === 允许退出游戏!!! === ");
                YxMessageBox.Show(new YxMessageBoxData()
                {
                    Msg = "确定要退出游戏吗?",
                    IsTopShow = true,
                    Delayed = 5,
                    BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle,
                    Listener = (box, btnName) =>
                    {
                        if (btnName == YxMessageBox.BtnLeft)
                        {
                            App.QuitGame();
                        }
                    },
                });
            }
        }

        /// <summary>
        /// 更换房间
        /// </summary>
        private void ChangeRoom()
        {
            var gdata = App.GetGameData<PaiJiuGameData>();

            if (gdata.IsGameing && gdata.GetPlayer<PaiJiuPlayer>().IsReady)
            {
                YxDebug.Log("正在游戏中,无法更换房间!!");
                YxMessageBox.Show(new YxMessageBoxData()
                {
                    Msg = "正在游戏中，不能换房间!!!",
                    IsTopShow = true,
                    Delayed = 5,
                });
            }
            else
            {
                YxDebug.Log("允许更换房间!!");


                foreach (var player in gdata.PlayerList)
                {
                    var user = (PaiJiuPlayer)player;
                    user.Info = null;
                    user.RefreshPanel();
                    user.gameObject.SetActive(false);
                }
                App.GetRServer<PaiJiuGameServer>().OnAllowEnter();
                YxWindowManager.ShowWaitFor();
            }
        }


        public void CloseMenu()
        {
            foreach (var btn in BtnTweens)
            {
                btn.GetComponent<TweenColor>().ResetToBeginning();
            }
            CloseBtn.SetActive(false);
            MenuListBgTween.PlayReverse();
        }


        public void OnClickSettingBtn()
        {
            if (PanelHelp == null)
            {
                YxDebug.Log(" === Panel_Setting === ");
                return;
            }

            PanelSetting.SetActive(true);
        }


        public void OnClickHelpBtn()
        {
            if (PanelSetting == null)
            {
                YxDebug.Log(" === Panel_Setting === ");
                return;
            }
            PanelHelp.SetActive(true);
        }


        [SuppressMessage("ReSharper", "InconsistentNaming")]
        protected enum MenuBtn
        {
            Menu_Btn,
            Back_Btn,
            Change_Btn,
            Help_Btn,
            Close_Btn,
            AddGold_Btn,
            History_Btn,
            Setting_Btn,
        }
    }
}
