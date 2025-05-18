/** 
 *文件名称:     ActiveContainer.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2018-04-18 
 *描述:         显隐控制类
 *历史记录: 
*/

using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.Mahjong2D.Common.UI
{
    public class ActiveContainer : YxView
    {
        #region UI Param
        [Tooltip("需要控制显示的物体")]
        public List<GameObject> ActiveObjects;
        [Tooltip("反选物体")]
        public List<GameObject> OppActiveObjects;
        [Tooltip("显示事件")]
        public List<EventDelegate> OnShowAction = new List<EventDelegate>();
        [Tooltip("隐藏事件")]
        public List<EventDelegate> OnHideAction=new List<EventDelegate>();
        #endregion

        #region Data Param
        [Tooltip("开关限制数值,数值开关")]
        public float LimitStateValue = 0.5f;
        #endregion

        #region Local Data
        /// <summary>
        /// 当前数值状态
        /// </summary>
        private float _nowFloatState;

        /// <summary>
        /// 当前数值状态
        /// </summary>
        public float NowFloatState
        {
            get
            {
                return _nowFloatState;
            }
        }

        /// <summary>
        /// 相反数值状态
        /// </summary>
        public float OppFloatState
        {
            get { return LimitStateValue*2 - _nowFloatState; }
        }

        #endregion

        #region Life Cycle

        public void ShowObjects()
        {
            SetVisible(true);
        }

        public void HideObjects()
        {
            SetVisible(false);
        }

        #endregion

        #region Function

        /// <summary>
        /// 设置可见
        /// </summary>
        /// <param name="state"></param>
        public void SetVisible(bool state)
        {
            foreach (var item in ActiveObjects)
            {
                if (item)
                {
                    item.SetActive(state);
                }
            }
            foreach (var item in OppActiveObjects)
            {
                if (item)
                {
                    item.SetActive(!state);
                }
            }
            if (gameObject.activeInHierarchy)
            {
                if (state)
                {
                    StartCoroutine(OnShowAction.WaitExcuteCalls());
                }
                else
                {
                    StartCoroutine(OnHideAction.WaitExcuteCalls());
                }
               
            }
        }

        /// <summary>
        /// 设置可见(数值版)
        /// </summary>
        /// <param name="value"></param>
        public void SetVisibleByFloat(float value)
        {
            _nowFloatState = value;
            SetVisible(value >= LimitStateValue);
        }


        #endregion

    }
}
