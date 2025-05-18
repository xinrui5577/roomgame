using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Framework;

/*===================================================
 *文件名称:     PludoFreshView.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-12-20
 *描述:        	刷新View，在yxview 的基础上增加处理了刷新相关处理
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo.View
{
    public class PludoFreshView : YxView 
    {
        #region UI Param
        #endregion

        #region Data Param
        [Tooltip("界面刷新事件")]
        public List<EventDelegate> OnViewFreshAction=new List<EventDelegate>();

        #endregion

        #region Local Data
        #endregion

        #region Life Cycle

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if(Data!=null)
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
