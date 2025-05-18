/** 
 *文件名称:     YxPageItem.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2018-04-18 
 *描述:         分页Item
 *历史记录: 
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.PageListWindow
{
    public class YxPageItem : YxView
    {
        #region UI Param
        [Tooltip("分页Item刷新处理")]
        public List<EventDelegate> OnPageItemRresh=new List<EventDelegate>(); 
        #endregion
        #region Data Param

        #endregion
        #region Local Data
        #endregion
        #region Life Cycle
        public virtual Type GetDataType()
        {
            return typeof(YxData);
        }

        protected override void OnFreshView()
        {
            if(Data==null)
            {
                return;
            }
            if(Data.GetType() == GetDataType())
            {
                OnItemFresh();
                if(gameObject.activeInHierarchy)
                {
                    StartCoroutine(OnPageItemRresh.WaitExcuteCalls());
                }
            }
        }

        protected virtual void OnItemFresh()
        {
            
        }

        #endregion
        #region Function
        #endregion

    }
}
