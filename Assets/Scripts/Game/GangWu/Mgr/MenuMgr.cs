using System;
using System.Diagnostics.CodeAnalysis;
using Assets.Scripts.Game.GangWu.Main;
using Assets.Scripts.Game.GangWu.Tool;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Manager;
using YxFramwork.View;
using com.yxixia.utile.YxDebug;


namespace Assets.Scripts.Game.GangWu.Mgr
{
    public class MenuMgr : MonoBehaviour
    {
        /// <summary>
        /// 菜单按键数组
        /// </summary>
        public GameObject[] Buttons;
  
        /// <summary>
        /// 菜单背景tween
        /// </summary>
        public TweenPosition MenuBgTween;

        /// <summary>
        /// 用于计算按钮弹出位置
        /// </summary>
        public UISprite BgSprite;

        public GameObject CloseBtn;

        public GameObject SettingView;

        public GameObject HelpPanel;

        // Use this for initialization
        protected void Start () {
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
        }

        protected void OnClickListener(GameObject gob)
        {
            MenuBtn btnid = (MenuBtn)UIEventListener.Get(gob).parameter;
            var mgr = App.GetGameManager<GangWuGameManager>();
            var gdata = App.GetGameData<GangwuGameData>();

            switch (btnid)
            {
                case MenuBtn.OpenMenu:
                    Vector3 v = MenuBgTween.transform.localPosition;
                    MoveMenuBg(v, new Vector3(-8, v.y, v.z));
                    CloseBtn.SetActive(true);
                    break;
                case MenuBtn.BackBtn:
                    ////退出游戏按钮事件

                    if (CouldOut())
                    {
                        YxMessageBox.Show(new YxMessageBoxData()
                        {
                            Msg = "确定要退出游戏么?",
                            BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle,
                            IsTopShow = true,
                            Listener = (box, btnName) =>
                            {
                                if (btnName == YxMessageBox.BtnLeft)
                                {
                                    if (CouldOut())
                                    {
                                        App.QuitGame();
                                    }
                                    else
                                    {
                                        YxMessageBox.Show(new YxMessageBoxData()
                                        {
                                            Msg = "游戏已经开始,请在游戏结束后退出!",
                                            ShowBtnNames = new[] { YxMessageBox.BtnMiddle },
                                            IsTopShow = true,
                                        });
                                    }
                                }
                            }
                        });
                    }

                    CloseMenu();
                    break;
                    

                case MenuBtn.ChangeRoomBtn:

                    if (CouldOut())
                    {
                        foreach (var yxBaseGamePlayer in gdata.PlayerList)
                        {
                            var player = (PlayerPanel)yxBaseGamePlayer;
                            player.Reset();
                            player.RefreshPanel();
                            player.gameObject.SetActive(false);
                        }

                        App.GetRServer<GangWuGameServer>().ChangeRoom();
                        YxWindowManager.ShowWaitFor();

                    }
                    else
                    {
                        YxDebug.Log("正在游戏中,不能更换房间!");
                        YxMessageBox.Show(new YxMessageBoxData()
                            {
                                Msg = "正在游戏中,不能更换房间!",
                                IsTopShow = true,
                            });
                    }
                    CloseMenu();
                    break;
                case MenuBtn.HelpBtn:     //显示牌类按钮
                    HelpPanel.SetActive(true);
                    CloseMenu();
                    break;
                case MenuBtn.CloseBtn:
                    CloseMenu();
                    break;
                case MenuBtn.SettingBtn:
                    SettingView.SetActive(true);
                    CloseMenu();
                    break;
                case MenuBtn.DismissRoomBtn:
                    if (gdata.IsRoomGame && (gdata.IsPlayed || gdata.IsGameStart))
                    {
                        YxMessageBox.Show(new YxMessageBoxData
                        {
                            Msg = "确定要发起投票,解散房间么?",
                            BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle,
                            Listener = (box, btnName) =>
                            {
                                if (btnName == YxMessageBox.BtnRight)
                                {
                                    App.GetRServer<GangWuGameServer>().StartHandsUp(2);
                                }
                            },
                        });
                    }
                    CloseMenu();
                    break;

                case MenuBtn.HistoryBtn:

                    mgr.HistoryResultMgr.ShowHistoryView();
                    CloseMenu();
                    break;
            }
        }

        /// <summary>
        /// 非房卡模式下,检测是否可以退出房间
        /// </summary>
        /// <returns></returns>
        bool CouldOut()
        {
            var gdata = App.GetGameData<GangwuGameData>();
            var selfPanel = gdata.GetPlayer();
            if (gdata.IsGameStart && selfPanel.ReadyState 
                && gdata.GetPlayer<PlayerPanel>().CurGameType != PlayerGameType.Fold)
            {
                YxMessageBox.Show(new YxMessageBoxData
                {
                    Msg = "正在游戏中,不能更换房间!"
                });
                return false;
            }
            return true;
        }

        /// <summary>
        /// 移动菜单
        /// </summary>
        /// <param name="from">初始位置</param>
        /// <param name="to">终点位置</param>
        /// <param name="duration">移动时间</param>
        void MoveMenuBg(Vector3 from,Vector3 to ,float duration = 0.2f)
        {
            MenuBgTween.from = from;
            MenuBgTween.to = to;
            MenuBgTween.duration = duration;
            MenuBgTween.ResetToBeginning();
            MenuBgTween.PlayForward();
        }
 

        /// <summary>
        /// 关闭菜单
        /// </summary>
        public void CloseMenu()
        {
           int with = BgSprite.width;
           int line = 4;    //留出一道缝隙
           Vector3 v3 = MenuBgTween.transform.localPosition;
           Vector3 to = new Vector3(-with + line, v3.y, v3.z);
           MoveMenuBg(MenuBgTween.transform.localPosition, to);
           CloseBtn.SetActive(false);
        }

        


        [SuppressMessage("ReSharper", "InconsistentNaming")]
        protected enum MenuBtn
        {
            CloseBtn,
            OpenMenu,
            BackBtn,
            ChangeRoomBtn,
            SettingBtn,
            HelpBtn,
            ChangeBtn,
            DismissRoomBtn,
            HistoryBtn,
        }

    }
}
