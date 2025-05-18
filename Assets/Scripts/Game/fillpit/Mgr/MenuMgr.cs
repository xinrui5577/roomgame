using System;
using System.Diagnostics.CodeAnalysis;
using Assets.Scripts.Game.fillpit.Tool;
using UnityEngine;
using YxFramwork.Manager;
using YxFramwork.View;
using Assets.Scripts.Game.fillpit.ImgPress.Main;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using YxFramwork.Common;

// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable UnusedMember.Local

namespace Assets.Scripts.Game.fillpit.Mgr
{
    public class MenuMgr : MonoBehaviour
    {
        /// <summary>
        /// 菜单按键数组
        /// </summary>
        public GameObject[] Buttons;

        /// <summary>
        /// 不能开始游戏对话框
        /// </summary>
        public UILabel CantStartLabel = null;

        /// <summary>
        /// 设置菜单
        /// </summary>
        [SerializeField]
        protected GameObject SettingPanel = null;


        public void OnInitRoom(ISFSObject data)
        {
            if (data.ContainsKey("rid"))
            {
                SetGobActive(GetGob(MenuBtn.ChangeBtn.ToString()), false);
            }
            if (data.ContainsKey("round"))
            {
                if (data.GetInt("round") > 0)
                {
                    OnGameBegin();
                }
            
            }
            InitOnClick();
        }

        public virtual void OnGameBegin()
        {
            //房卡模式,隐藏更换房间按钮
            if (!App.GetGameData<FillpitGameData>().IsRoomGame) return;
            SetGobActive(CantStartLabel.gameObject, false);
            var changeBtn = GetGob(MenuBtn.ChangeBtn.ToString());
            if (changeBtn == null)
            {
                return;
            }
            SetGobActive(changeBtn, false);
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

        int _minGold = 0;
        protected virtual void OnClickListener(GameObject gob)
        {
            MenuBtn btnid = (MenuBtn)UIEventListener.Get(gob).parameter;
            switch (btnid)
            {
                case MenuBtn.DismissRoomBtn:
                    OnClickDismissRoomBtn();
                    break;

                case MenuBtn.SettingBtn:
                    OnCLickSettingBtn();
                    break;
    
                case MenuBtn.ChangeBtn:
                    OnClickChangeBtn();
                    break;

            }
        }


        public void OnClickDismissRoomBtn()
        {
            App.GetGameManager<FillpitGameManager>().DismissRoomMgr.SetDismissRoomBtn();
        }

        public void OnCLickSettingBtn()
        {
            if (SettingPanel != null)
                SettingPanel.SetActive(true);
        }

        public void OnClickChangeBtn()
        {
            var gdata = App.GetGameData<FillpitGameData>();
            if (gdata.IsGameing && gdata.GetPlayer().ReadyState)
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
                App.RServer.ChangeRoom();
                YxWindowManager.ShowWaitFor();
            }
        }


        public void OnClickReadyBtn()
        {
            if (App.GetGameData<FillpitGameData>().GetPlayerInfo().CoinA < _minGold)
            {
                App.GetRServer<FillpitGameServer>().ReadyGame();
            }
            else
            {
                YxMessageBox.Show(new YxMessageBoxData()
                {
                    Msg = "抱歉,您没有足够的筹码,无法准备!!",
                    Listener = (box, btnName) =>
                    {
                        if (btnName == YxMessageBox.BtnMiddle)
                        {
                            App.QuitGame();
                        }
                    },
                    IsTopShow = true,
                    ShowBtnNames = new [] { YxMessageBox.BtnMiddle },
                });
            }
        }


        protected void SetGobActive(GameObject gob, bool active)
        {
            if (gob == null) return;
            gob.SetActive(active);
        }


        protected GameObject GetGob(string btnName)
        {
            foreach (var btn in Buttons)
            {
                if (btn.name == btnName)
                    return btn;
            }
            return null;
        }

        public void ShowObj(GameObject obj)
        {
            obj.SetActive(true);
        }

        public void HideObj(GameObject obj)
        {
            obj.SetActive(false);
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        protected enum MenuBtn
        {
            CloseBtn,
            OpenMenu,
            BackBtn,
            SettingBtn,
            HelpBtn,
            ChangeBtn,
            DismissRoomBtn,
            HistoryBtn,
        }
    }
}
