using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Assets.Scripts.Game.BlackJackCs.Tool;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

#pragma warning disable 649

namespace Assets.Scripts.Game.BlackJackCs
{
    public class SettingMenu : MonoBehaviour
    {

        [Tooltip("设置窗口对象")]
        [SerializeField]
        protected GameObject SettingView = null;

        [Tooltip("音乐音量设置")]
        [SerializeField]
        protected UISlider VolumeSlider = null;

        [Tooltip("音效音量设置")]
        [SerializeField]
        protected UISlider EffectSlider = null;


        /// <summary>
        /// 向上箭头图标
        /// </summary>
        [SerializeField]
        private GameObject _sprUp;


        /// <summary>
        /// 向下箭头图标
        /// </summary>
        [SerializeField]
        private GameObject _sprDown;

        /// <summary>
        /// 关闭下拉菜单按钮
        /// </summary>
        [SerializeField]
        private GameObject _closeListBtn;

        /// <summary>
        /// 打开下拉菜单按钮
        /// </summary>
        [SerializeField]
        private BoxCollider _openListBtn;

        /// <summary>
        /// 下拉菜单的TweenScale
        /// </summary>
        [SerializeField]
        private TweenScale _dropDownListTs;

        /// <summary>
        /// 所有菜单中，包含Tween动画的按钮
        /// </summary>
        [SerializeField]
        private List<GameObject> _tweenList;

        /// <summary>
        /// 所有按钮
        /// </summary>
        [SerializeField]
        private List<GameObject> _buttons = new List<GameObject>();
  
        protected void Start()
        {
            SettingView.gameObject.SetActive(false);
            VolumeSlider.value = Facade.Instance<MusicManager>().MusicVolume;
            EffectSlider.value = Facade.Instance<MusicManager>().EffectVolume;

            InitOnClick();
        }

        public void OnDragVolumeSlider(float volume)
        {

            Facade.Instance<MusicManager>().MusicVolume = volume;

        }

        public void OnDragEffectSlider(float volume)
        {

            Facade.Instance<MusicManager>().EffectVolume = volume;

        }

        public void OnClickChangeRoomBtn()
        {
            CloseListView();
            if (App.GetGameData<BlackJackGameData>().IsPlaying)
            {
                YxDebug.Log("正在游戏中,无法更换房间!");
                YxMessageBox.DynamicShow(new YxMessageBoxData
                {
                    Msg = "正在游戏中,无法更换房间!",
                    Delayed = 5,
                });
            }
            else
            {
                YxDebug.Log("正在更换房间....");
                App.GetRServer<BlackJackGameServer>().ChangeRoom();
            }
        }


        /// <summary>
        /// 打开设置窗口
        /// </summary>
        public void OnClickSettingBtn()
        {
            CloseListView();
            SettingView.gameObject.SetActive(true);
           
        }

        /// <summary>
        /// 关闭设置窗口
        /// </summary>
        public void OnClickCloseSettingViewBtn()
        {
            Debug.Log("Click Close Setting View");
            SettingView.SetActive(false);
        }

        /// <summary>
        /// 打开下拉菜单
        /// </summary>
        public void OnClickOpenListBtn()
        {
            _dropDownListTs.PlayForward();
            _sprUp.SetActive(true);
            _sprDown.SetActive(false);
            _closeListBtn.SetActive(true);
            _openListBtn.enabled = false;

            foreach (GameObject tween in _tweenList)
            {
                tween.SetActive(true);
                
            }
        }

        public void OnClickCloseListBtn()
        {
            CloseListView();
        }

        /// <summary>
        /// 关闭下拉菜单
        /// </summary>
        void CloseListView()
        {
            _dropDownListTs.PlayReverse();
            _sprDown.SetActive(true);
            _sprUp.SetActive(false);
            _closeListBtn.SetActive(false);
            _openListBtn.enabled = true;

            foreach(GameObject tween in _tweenList)
            {
                tween.SetActive(false);
                var ts = tween.GetComponent<TweenScale>();
                if (ts != null)
                {
                    ts.ResetToBeginning();
                }
                var tc = tween.GetComponent<TweenColor>();
                if (tc != null)
                {
                    tc.ResetToBeginning();
                }
            }
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        public void QuitGame()
        {
            CloseListView();
            if (App.GetGameData<BlackJackGameData>().IsPlaying)
            {
                YxMessageBox.DynamicShow(new YxMessageBoxData
                {
                    Msg = "正在游戏中,无法退出游戏!",
                    Delayed = 5,
                });
            }
            else
            {
                YxMessageBox.DynamicShow(new YxMessageBoxData
                {
                    Msg = "您确定要退出游戏吗!?",
                    BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle,
                    Listener = (box, btnName) =>
                    {
                        if (btnName == YxMessageBox.BtnLeft)
                        {
                            App.QuitGame();
                        }
                    },
                    IsTopShow = true,
                });
            }
        }

        ///<summary>
        ///初始化按键监听
        ///</summary>
        public void InitOnClick()
        {
            foreach (MenuBtns btnid in Enum.GetValues(typeof(MenuBtns)))
            {
                foreach (GameObject btn in _buttons)
                {
                    if (btn.name.Equals(btnid.ToString()))
                    {
                        Tools.NguiAddOnClick(btn, OnClickListener, (int)btnid);
                    }
                }
            }
        }


        protected void OnClickListener(GameObject gob)
        {
            MenuBtns btnid = (MenuBtns)UIEventListener.Get(gob).parameter;

            switch (btnid)
            {
                case MenuBtns.OpenListBtn:
                    OnClickOpenListBtn();
                    break;
                case MenuBtns.ChangeRoomBtn:

                    OnClickChangeRoomBtn();

                    break;
                case MenuBtns.SettingBtn:
                    OnClickSettingBtn();
                    break;
                case MenuBtns.QuitBtn:
                    QuitGame();
                    break;
                case MenuBtns.CloseListBtn:
                    CloseListView();
                    break;
                case MenuBtns.CloseSettingViewBtn:
                    OnClickCloseSettingViewBtn();
                    break;
            }
        }



        [SuppressMessage("ReSharper", "InconsistentNaming")]
        enum MenuBtns
        {   
            /// <summary>
            /// 打开菜单
            /// </summary>
            OpenListBtn,
            /// <summary>
            /// 换房间
            /// </summary>
            ChangeRoomBtn,
            /// <summary>
            /// 打开设置窗口
            /// </summary>
            SettingBtn,
            /// <summary>
            /// 退出游戏,返回大厅
            /// </summary>
            QuitBtn,
            /// <summary>
            /// 关闭下来菜单
            /// </summary>
            CloseListBtn,
            /// <summary>
            /// 关闭设置窗口
            /// </summary>
            CloseSettingViewBtn,
        }
    }
}