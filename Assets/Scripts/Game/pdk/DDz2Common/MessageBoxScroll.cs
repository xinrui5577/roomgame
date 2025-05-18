using Assets.Scripts.Game.pdk.DdzEventArgs;
using Assets.Scripts.Game.pdk.InheritCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.View;

namespace Assets.Scripts.Game.pdk.DDz2Common
{

    public class MessageBoxScroll : MonoBehaviour
    {

        public GameObject BoxPanelGob;

        public GameObject LeftGobBg;
        public GameObject RightGobBg;

        public GameObject LeftGou;
        public GameObject RightGou;

        public UIButton dissmissBtn;
        public UIButton changeroomBtn;
        public List<GameObject> changeroomObj;//更换房间的组建

        private const string ShowGgBype = "showBgType";
        private GlobalData _globalData;
        void Awake()
        {
            if (PlayerPrefs.GetInt(ShowGgBype, 0) == 0)
            {
                OnClickShowLeftBg();
            }
            else
            {
                OnClickShowRightBg();
            }

        }

        void Start()
        {
            PdkGameManager.AddOnGameInfoEvt(OnGameInfo);

            if (dissmissBtn!=null)
            {
                dissmissBtn.onClick = new List<EventDelegate> { new EventDelegate(dissmissBtnonClick) };
            }
            if (changeroomBtn!=null)
            {
                changeroomBtn.onClick = new List<EventDelegate> { new EventDelegate(changeroomBtnonClick) };
            }
        }

        private void OnGameInfo(object sender, DdzbaseEventArgs args)
        {
            _globalData = App.GetGameData<GlobalData>();
            //房卡模式下,隐藏更换房间按钮
            if (dissmissBtn!=null)
            {
                SetBtnActive(dissmissBtn.gameObject, _globalData.IsRoomGame);
            }
            if (changeroomBtn!=null)
            {
                SetBtnActive(changeroomBtn.gameObject, !_globalData.IsRoomGame);
            }
            for (int i = 0; i < changeroomObj.Count; i++)
            {
                if (changeroomObj[i] != null)
                {
                    SetBtnActive(changeroomObj[i], _globalData.IsRoomGame);
                }
            }
        }
        /// <summary>
        /// 点击解散房间按钮
        /// </summary>
        public void dissmissBtnonClick()
        {

            _globalData = App.GetGameData<GlobalData>();
            //Debug.Log("<color=#1300FFFF>" + "_globalData.PlayerList~~~~~~~~~~~~~~~~~~~~~~~~" + _globalData.PlayerList.Length + "</color>");

            var isSelfOwner = App.GetGameData<GlobalData>().IsSelfIsOwer;

            var msgstr = isSelfOwner ? "确定要解散房间么？" : "确定离开房间吗？";


            YxMessageBox.Show(msgstr, "", (box, btnName) =>
            {
                if (btnName == YxMessageBox.BtnLeft)
                {
                    if (!App.GetGameData<GlobalData>().IsStartGame)
                    {
                        //App.GetGameData<GlobalData>().ClearParticalGob();

                        if (isSelfOwner)
                            GlobalData.ServInstance.DismissRoom();
                        else
                            GlobalData.ServInstance.LeaveRoom();
                        return;
                    }

                    GlobalData.ServInstance.StartHandsUp(2);
                }
            }, true, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
        }
        /// <summary>
        /// 点击更换房间按钮
        /// </summary>
        public void changeroomBtnonClick()
        {
            _globalData = App.GetGameData<GlobalData>();
            Debug.Log("<color=#1300FFFF>" + " _globalData.IsStartGame~~~~~~~~~~~~~~~~~~~~~~~~" + _globalData.IsStartGame + "</color>");

            bool isRoomGame = _globalData.IsRoomGame;

            if (isRoomGame) return;

            //非房卡模式下,直接更换房间
            if (CheckCouldOut())
            {
                App.GetRServer<Ddz2RemoteServer>().ChangeRoom();
            }
        }
        /// <summary>
        /// 非房卡模式下,检测是否可以退出游戏
        /// </summary>
        /// <returns></returns>
        bool CheckCouldOut()
        {
            _globalData = App.GetGameData<GlobalData>();

            if (_globalData == null) return false;

            bool isGameing = (_globalData.IsStartGame);
            if (isGameing)
            {
                YxMessageBox.Show(new YxMessageBoxData
                {
                    Msg = "正在游戏中,无法退出房间!"
                });
            }
            return !isGameing;
        }
        public void SetBtnActive(GameObject btn, bool active)
        {
            if (btn == null) return;
            btn.SetActive(active);
        }
        /// <summary>
        /// 当点击关闭按钮
        /// </summary>
        public void OnCloseBtnClick()
        {
            BoxPanelGob.SetActive(false);
        }

        public void OpenMessageBox()
        {
            BoxPanelGob.SetActive(true);
        }

        public void HideMessageBox()
        {
            BoxPanelGob.SetActive(false);
        }

        public void OnClickShowLeftBg()
        {
            if (LeftGobBg != null) LeftGobBg.SetActive(true);
            if (LeftGou != null) LeftGou.SetActive(true);

            if (RightGobBg != null) RightGobBg.SetActive(false);
            if (RightGou != null) RightGou.SetActive(false);

            PlayerPrefs.SetInt(ShowGgBype,0);
        }


        public void OnClickShowRightBg()
        {
            if (LeftGobBg != null) LeftGobBg.SetActive(false);
            if (LeftGou != null) LeftGou.SetActive(false);

            if (RightGobBg != null) RightGobBg.SetActive(true);
            if (RightGou != null) RightGou.SetActive(true);

            PlayerPrefs.SetInt(ShowGgBype, 1);
        }
    }
}
