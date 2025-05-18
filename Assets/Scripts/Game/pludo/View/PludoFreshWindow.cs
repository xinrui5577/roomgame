using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows;
using UnityEngine;

/*===================================================
 *文件名称:     PludoFreshWindow.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2019-01-14
 *描述:        	可刷新窗口
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo.View
{
    public class PludoFreshWindow:YxNguiWindow 
    {
        #region UI Param
        #endregion

        #region Data Param
        [Tooltip("界面刷新事件")]
        public List<EventDelegate> OnViewFreshAction = new List<EventDelegate>();
        #endregion

        #region Local Data
        #endregion

        #region Life Cycle
        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (Data != null)
            {
                OnFreshViewWithData();
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(OnViewFreshAction.WaitExcuteCalls());
                }
            }
        }
        #endregion

        #region Function
        protected virtual void OnFreshViewWithData()
        {

        }
        #endregion
    }
}
