using System;
using System.Diagnostics.CodeAnalysis;
using Assets.Scripts.Game.Texas.Main;
using Assets.Scripts.Game.Texas.Tool;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Manager;
using YxFramwork.View;


namespace Assets.Scripts.Game.Texas.Mgr
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
        public GameObject MenuFather;
        /// <summary>
        /// 菜单背景tween
        /// </summary>
        public TweenScale MenuBgTween;
        /// <summary>
        /// 按键的tweens
        /// </summary>
        public GameObject[] BtnTweens;

        public UIGrid MenuPanelGrid = null;

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

        protected virtual void OnClickListener(GameObject gob)
        {
            var btnid = (MenuBtn)UIEventListener.Get(gob).parameter;
            var gdata = App.GetGameData<TexasGameData>();
            switch (btnid)
            {
                case MenuBtn.Menu_Btn:
                    MenuFather.SetActive(true);
                    foreach (GameObject tween in BtnTweens)
                    {
                        tween.GetComponent<TweenColor>().ResetToBeginning();
                        tween.GetComponent<TweenScale>().ResetToBeginning();
                    }
                    MenuBgTween.PlayForward();
                    break;
                case MenuBtn.Back_Btn:

                    //分清开房模式和非开放模式
                    if (gdata.IsRoomGame)
                    {
                        //游戏开始了就需要发起投票，否则玩家可以自主决定
                        if (gdata.IsPlayed)
                        {
                            YxMessageBox.Show(new YxMessageBoxData
                            {
                                Msg = "确定要发起投票,解散房间么?",
                                Listener = (box, btnName) =>
                                {
                                    if (btnName == YxMessageBox.BtnLeft)
                                    {
                                        var gMgr = App.GetGameManager<TexasGameManager>();
                                        var rmodeMgr = gMgr.RModelMagr;
                                        Debug.Log("RModelMgr.GetInstance().DismissRoom" + rmodeMgr.DismissRoom.activeSelf);
                                        if (!rmodeMgr.DismissRoom.activeSelf)
                                        {

                                            App.GetRServer<TexasGameServer>().DismissRoom(2);
                                            App.GetRServer<TexasGameServer>().DismissRoom(3);
                                        }
                                        else
                                        {
                                            Debug.Log("确定要发起投票,解散房间么?");
                                            YxMessageBox.Show("请不要频繁发出解散请求!!", 5);
                                            rmodeMgr.DismissRoom.transform.GetChild(0).gameObject.SetActive(true);
                                        }
                                    }
                                },
                                BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle,
                                IsTopShow = true
                            });
                        }
                        else
                        {
                            //房主可以解散房间,玩家则是自己退出房间
                            if (gdata.SelfSeat == 0)
                            {
                                YxMessageBox.Show("确定要解散房间吗？", "", (box, btnName) =>
                                {
                                    if (btnName == YxMessageBox.BtnLeft)
                                    {
                                        App.GetRServer<TexasGameServer>().DismissRoom();                                             
                                    }
                                }, true, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle
                                       );
                            }
                            else
                            {
                                YxMessageBox.Show("确定要退出房间么?", "", (box, btnName) =>
                                {
                                    if (btnName == YxMessageBox.BtnLeft)
                                    {
                                        App.QuitGame();
                                    }
                                }, true, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle
                                      );
                            }
                        }
                    }
                    else
                    {
                        var selfInfo = gdata.GetPlayerInfo();
                        if (gdata.GStatus == YxEGameStatus.PlayAndConfine && selfInfo.State)
                        {
                            YxMessageBox.Show("正在游戏中，结束后再退出！！！", 5);
                        }
                        else
                        {
                            YxMessageBox.Show("确定要退出游戏吗？", "", (box, btnName) =>
                            {
                                if (btnName == YxMessageBox.BtnLeft)
                                {
                                    App.QuitGame();
                                }
                            }, true, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle, null, 5);
                        }

                    }
                    
                    CloseMenu();
                    break;
                case MenuBtn.Change_Btn:
                {
                        var selfInfo = gdata.GetPlayerInfo();
                        if (gdata.GStatus == YxEGameStatus.PlayAndConfine && selfInfo.State)
                        {
                            YxMessageBox.Show("正在游戏中，不能换房间！！！", 5);
                        }
                        else
                        {
                            var gMgr = App.GetGameManager<TexasGameManager>();
                            if (gMgr.IsResultDone)
                            {
                                gMgr.ResultDone();
                            }
                            gMgr.Reset();
                            gMgr.BetMagr.Reset();
                            App.GetRServer<TexasGameServer>().OnAllowEnter();
                            YxWindowManager.ShowWaitFor();
                        }
                        CloseMenu();
                    }
                    break;
                case MenuBtn.PokerType_Btn:
                    HelpPanel.SetActive(true);
                    CloseMenu();
                    break;
                case MenuBtn.Close_Btn:
                    CloseMenu();
                    break;
                case MenuBtn.AddGold_Btn:
                    {
                        var selfInfo = gdata.GetPlayerInfo();
                        if (gdata.GStatus == YxEGameStatus.PlayAndConfine && selfInfo.State)
                        {
                            YxMessageBox.Show("正在游戏中,不能添加筹码！", 5);
                        }
                        else
                        {
                            //打开选择携带钱界面
                            var gMgr = App.GetGameManager<TexasGameManager>();
                            gMgr.GetGoldMagr.OpenPanel(selfInfo);
                        }
                    }
                    break;
            }
        }

        public virtual void CloseMenu()
        {
            MenuFather.SetActive(false);
            MenuBgTween.PlayReverse();
        }

        public GameObject HelpPanel;

        public void OnHelpPanelClose()
        {
            HelpPanel.SetActive(false);
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        protected enum MenuBtn
        {
            Menu_Btn,
            Back_Btn,
            Change_Btn,
            PokerType_Btn,
            Close_Btn,
            AddGold_Btn,
            History_Btn,
            Setting_Btn,
            HelpBtn
        }
    }
}
