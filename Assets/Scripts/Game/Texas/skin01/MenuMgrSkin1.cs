using Assets.Scripts.Common.UI;
using Assets.Scripts.Game.Texas.Main;
using Assets.Scripts.Game.Texas.Mgr;
using Assets.Scripts.Game.Texas.Tool;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.View;

namespace Assets.Scripts.Game.Texas.skin01
{
    public class MenuMgrSkin1 : MenuMgr
    {
        public SettingWindow SettingWin;


        protected override void OnClickListener(GameObject gob)
        {
            var btnid = (MenuBtn)UIEventListener.Get(gob).parameter;
            var gdata = App.GetGameData<TexasGameData>();
            switch (btnid)
            {
                case MenuBtn.Menu_Btn:
                    MenuFather.SetActive(true);

                    foreach (var tween in BtnTweens)
                    {
                        tween.GetComponent<TweenColor>().ResetToBeginning();
                        tween.GetComponent<TweenScale>().ResetToBeginning();
                    }

                    if (gdata.IsRoomGame)
                    {
                        //房卡没有换房
                        var changeBtn = Tools.GobSelectName(Buttons, MenuBtn.Change_Btn.ToString());
                        if (changeBtn != null)
                        {
                            changeBtn.SetActive(false);
                        }
                        MenuPanelGrid.cellHeight = 100;
                        
                        //解散房间
                        if (gdata.IsRoomOwner || gdata.IsPlayed)
                        {
                            var backBtn = Tools.GobSelectName(Buttons, MenuBtn.Back_Btn.ToString());
                            if (backBtn != null)
                            {
                                var sprName = "dismiss";
                                var btn = backBtn.GetComponent<UIButton>();
                                btn.normalSprite = sprName + "_up";
                                btn.pressedSprite = sprName + "_over";
                                btn.hoverSprite = sprName + "_over";
                                btn.disabledSprite = sprName + "_up";
                            }
                        }
                    }

                    MenuBgTween.PlayForward();
                    break;
                case MenuBtn.Back_Btn:

                    //开放模式下,走解散房间,否则更换房间
                    if (gdata.IsRoomGame)
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
                case MenuBtn.PokerType_Btn:
                    if (HelpPanel != null)
                    {
                        HelpPanel.SetActive(true);
                    }
                    CloseMenu();
                    break;
                case MenuBtn.Close_Btn:
                    CloseMenu();
                    break;
                case MenuBtn.AddGold_Btn:
                    {
                        var selfInfo = gdata.GetPlayerInfo();
                        if (gdata.GStatus > YxEGameStatus.Over && selfInfo.State)
                        {
                            YxMessageBox.Show(new YxMessageBoxData
                            {
                                Msg = "正在游戏中,不能添加筹码!",
                                IsTopShow = true,
                                Delayed = 5,
                            });
                        }
                        else
                        {
                            //打开选择携带钱界面
                            App.GetGameManager<TexasGameManager>().GetGoldMagr.OpenPanel(selfInfo);
                        }
                    }
                    break;

                case MenuBtn.Setting_Btn:
                    OnClickSettingBtn();
                    CloseMenu();
                    break;

                case MenuBtn.History_Btn:
                    var historyResultMgr = App.GetGameManager<TexasGameManager>().HistoryResultMgr;
                    if (historyResultMgr != null)
                    {
                        historyResultMgr.ShowHistoryView();
                    }
                    CloseMenu();
                    break;
                case MenuBtn.HelpBtn:

                    break;
            }
        }


        /// <summary>
        /// 投票解散房间
        /// </summary>
        private void DismissRoom()
        {
            var gdata = App.GetGameData<TexasGameData>();
            //游戏开始了就需要发起投票，否则玩家可以自主决定
            if (gdata.IsPlayed)
            {
                YxMessageBox.Show(new YxMessageBoxData
                {
                    Msg = "确定要发起投票,解散房间么?",
                    BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle,
                    IsTopShow = true,
                    Listener = (box, btnName) =>
                    {
                        if (btnName == YxMessageBox.BtnLeft)
                        {
                            var gMgr = App.GetGameManager<TexasGameManager>();
                            if (!gMgr.RModelMagr.DismissRoom.activeSelf)
                            {
                                App.GetRServer<TexasGameServer>().DismissRoom(2);
                                App.GetRServer<TexasGameServer>().DismissRoom(3);
                            }
                            else
                            {
                                Debug.Log("确定要发起投票,解散房间么?");
                                YxMessageBox.Show(new YxMessageBoxData
                                {
                                    Msg = "确定要发起投票,解散房间么?",
                                    Delayed = 5,
                                });
                                gMgr.RModelMagr.DismissRoom.transform.GetChild(0).gameObject.SetActive(true);
                            }
                        }
                    },
                });
            }
            else
            {
                //房主可以解散房间,玩家则是自己退出房间
                if (gdata.IsRoomOwner)
                {
                    YxMessageBox.Show(new YxMessageBoxData
                    {
                        Msg = "确定要解散房间吗?",
                        BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle,
                        IsTopShow = true,
                        Listener = (box, btnName) =>
                        {
                            if (btnName == YxMessageBox.BtnLeft)
                            {
                                App.GetRServer<TexasGameServer>().DismissRoom();
                            }
                        },
                    });
                }
                else
                {
                    YxMessageBox.Show(new YxMessageBoxData
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
            var gdata = App.GetGameData<TexasGameData>();
            if (gdata.GStatus == YxEGameStatus.PlayAndConfine && gdata.GetPlayerInfo().State)
            {
                YxDebug.Log(" === 正在游戏中，结束后再退出!!! === ");
                YxMessageBox.Show(new YxMessageBoxData
                {
                    Msg = "正在游戏中，结束后再退出!!!",
                    IsTopShow = true,
                    Delayed = 5,
                });
            }
            else
            {
                App.QuitGameWithMsgBox();
            }
        }

        /// <summary>
        /// 更换房间
        /// </summary>
        private void ChangeRoom()
        {
            var gdata = App.GetGameData<TexasGameData>();
            
            if (gdata.GStatus == YxEGameStatus.PlayAndConfine && gdata.GetPlayerInfo().State)
            {
                YxDebug.Log("正在游戏中,无法更换房间!!");
                YxMessageBox.Show(new YxMessageBoxData
                {
                    Msg = "正在游戏中，不能换房间!!!",
                    IsTopShow = true,
                    Delayed = 5,
                });
            }
            else
            {
                var gMgr = App.GetGameManager<TexasGameManager>();
                YxDebug.Log("允许更换房间!!");
                if (gMgr.IsResultDone)
                {
                    gMgr.ResultDone();
                }
                gMgr.Reset();
                gMgr.BetMagr.Reset();

                App.GetRServer<TexasGameServer>().ChangeRoom();
            }
        }


        public override void CloseMenu()
        {
            foreach (var btn in BtnTweens)
            {
                btn.GetComponent<TweenColor>().ResetToBeginning();
            }
            base.CloseMenu();
        }


        public void OnClickSettingBtn()
        {
            if (SettingWin == null)
            {
                YxDebug.Log(" === Panel_Setting === ");
                return;
            }
            SettingWin.Show();
        }
    }
}
