using Assets.Scripts.Game.fillpit.ImgPress.Main;
using Assets.Scripts.Game.fillpit.Mgr;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Game.fillpit.skin1
{
    public class MenuMgrSk1 : MenuMgr
    {
  
        /// <summary>
        /// 菜单背景tween
        /// </summary>
        public TweenPosition MenuBgTween;

        /// <summary>
        /// 用于计算按钮弹出位置
        /// </summary>
        public UISprite BgSprite;

        public GameObject CloseBtn;

        public GameObject HelpPanel;

        public string DismissSpNameUp;

        public string DismissSpNameOver;


        public override void OnGameBegin()
        {
            if (!App.GetGameData<FillpitGameData>().IsRoomGame) return;

            var backBtn = GetGob(MenuBtn.BackBtn.ToString());
            if (backBtn == null) return;
            var btn = backBtn.GetComponent<UIButton>();
            btn.normalSprite = DismissSpNameUp;
            btn.hoverSprite = DismissSpNameOver;
            btn.pressedSprite = DismissSpNameOver;
        }

     
        protected override void OnClickListener(GameObject gob)
        {
            MenuBtn btnid = (MenuBtn)UIEventListener.Get(gob).parameter;
            var gdata = App.GetGameData<FillpitGameData>();
            var gserver = App.GetRServer<FillpitGameServer>();
            switch (btnid)
            {
                case MenuBtn.OpenMenu:
                    Vector3 v = MenuBgTween.transform.localPosition;
                    MoveMenuBg(v, new Vector3(-8, v.y, v.z));
                    CloseBtn.SetActive(true);
                    break;
                case MenuBtn.BackBtn:
                    bool isPlayed = gdata.IsPlayed;
                    bool isRoomOwner = gdata.IsRoomOwner;

                    if (!gdata.IsRoomGame)
                    {
                        if (CouldOut())
                        {
                            YxMessageBox.Show(new YxMessageBoxData
                            {
                                Msg = "您确定要退出游戏吗?",
                                BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle,
                                Listener = (box, btnName) =>
                                {
                                    if (btnName == YxMessageBox.BtnLeft)
                                    {
                                        if (CouldOut())
                                        {
                                            App.QuitGame();
                                        }
                                    }
                                }
                            });
                        }
                        return;
                    }

                    if (isPlayed)
                    {
                        YxMessageBox.Show(new YxMessageBoxData()
                        {
                            Msg = "确定要发起投票,解散房间么?",
                            BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle,
                            Listener = (box, btnName) =>
                            {
                                if (btnName == YxMessageBox.BtnLeft)
                                {
                                    gserver.SendHandsUp(2);  //房间游戏已开始,发起投票
                                }
                            }
                        });
                    }
                    else if (isRoomOwner)
                    {
                        //房卡游戏没有开始,房主解散房间
                        YxMessageBox.Show(new YxMessageBoxData()
                        {
                            Msg = "确定要解散房间吗?",
                            Listener = (box, btnName) =>
                            {
                                if (btnName == YxMessageBox.BtnLeft)
                                {

                                    if (CouldOut())
                                    {
                                        IRequest req = new ExtensionRequest("dissolve", new SFSObject());
                                        App.GetRServer<FillpitGameServer>().SendRequest(req);
                                    }
                                }
                            },
                            IsTopShow = true,
                            BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle
                        });
                    }
                    else
                    {
                        if (CouldOut())
                        {
                            App.QuitGame();
                        }
                    }

                    CloseMenu();
                    break;
                    

                case MenuBtn.ChangeBtn:

                    if (CouldOut())
                    {
                        foreach (var yxBaseGamePlayer in gdata.PlayerList)
                        {
                            var player = (PlayerPanel)yxBaseGamePlayer;
                            player.Reset();
                            player.gameObject.SetActive(false);
                        }

                        gserver.ChangeRoom();
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
                    if (HelpPanel != null)
                    {
                        HelpPanel.SetActive(true);
                    }
                    CloseMenu();
                    break;
                case MenuBtn.CloseBtn:
                    CloseMenu();
                    break;
                case MenuBtn.SettingBtn:
                    SettingPanel.SetActive(true);
                    CloseMenu();
                    break;
                case MenuBtn.DismissRoomBtn:

                    if (gdata.IsRoomGame && (gdata.IsPlayed || gdata.IsGameing))
                    {
                        YxMessageBox.Show(new YxMessageBoxData
                        {
                            Msg = "确定要发起投票,解散房间么?",
                            BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle,
                            Listener = (box, btnName) =>
                            {
                                if (btnName == YxMessageBox.BtnRight)
                                {
                                    gserver.SendHandsUp(2);
                                }
                            },
                        });
                    }
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
            var gdata = App.GetGameData<FillpitGameData>();
            var selfPanel = gdata.GetPlayer();
            if (gdata.IsGameStart && selfPanel.ReadyState 
                && gdata.GetPlayer<PlayerPanel>().PlayerType != 3)
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
    
    }
}
