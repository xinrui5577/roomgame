using System.Collections.Generic;
using Assets.Scripts.Common.UI;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;

/*===================================================
 *文件名称:     PludoSettingWindo.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2019-01-15
 *描述:        	
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo.View
{
    public class PludoSettingWindow : SettingWindow 
    {
        #region UI Param
        #endregion

        #region Data Param
        [Tooltip("按钮状态变化")]
        public List<EventDelegate> OnBtnStateChange=new List<EventDelegate>();

        public SettingInfo CurInfo;

        /// <summary>
        /// 是否显示解散按钮
        /// </summary>
        public bool ShowDissloveBtn { private set; get;}
        #endregion

        #region Local Data

        #endregion

        #region Life Cycle

        protected override void OnAwake()
        {
            base.OnAwake();
            Facade.EventCenter.AddEventListeners<LoaclRequest, SettingInfo>(LoaclRequest.SettingInfoChange,OnSettingInfoChange);
        }

        public override void OnDestroy()
        {
            Facade.EventCenter.RemoveEventListener<LoaclRequest, SettingInfo>(LoaclRequest.SettingInfoChange, OnSettingInfoChange);
            base.OnDestroy();
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            CurInfo = Data as SettingInfo;
            if (CurInfo != null)
            {
                CheckBtnState();
            }
        }

        #endregion

        #region Function

        private void OnSettingInfoChange(SettingInfo info)
        {
            CurInfo = info;
            CheckBtnState();
        }

        /// <summary>
        /// 检测按钮状态
        /// </summary>
        private void CheckBtnState()
        {
            if (CurInfo != null)
            {
                ShowDissloveBtn= CurInfo.IsCreatRoom && (CurInfo.IsGameStart || CurInfo.IsOwener);
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(OnBtnStateChange.WaitExcuteCalls());
                }
            }
        }
        /// <summary>
        /// 点击解散按钮
        /// </summary>
        public void OnClickDissloveBtn()
        {
            if (CurInfo != null)
            {
                if (CurInfo.IsGameStart)
                {
                    HandUpWindow.StartHandUp(CurInfo.UserId);
                }
                else
                {
                    HandUpWindow.OwnerHandup();
                }
            }
        }

        /// <summary>
        /// 点击退出按钮
        /// </summary>
        public void OnClickQuitBtn()
        {
            App.QuitGameWithMsgBox();
        }

        #endregion
    }

    public class SettingInfo
    {
        /// <summary>
        /// 是否为创建房间模式
        /// </summary>
        public bool IsCreatRoom;
        /// <summary>
        /// 是否为房主
        /// </summary>
        public bool IsOwener;
        /// <summary>
        /// 游戏是否为开始状态
        /// </summary>
        public bool IsGameStart;
        /// <summary>
        /// 玩家ID
        /// </summary>
        public int UserId;
    }
}
