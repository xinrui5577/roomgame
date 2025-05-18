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
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.nbjl
{
    public class ActiveContainer : YxView
    {
        #region UI Param
        [Tooltip("需要控制显示的物体")]
        public List<GameObject> ActiveObjects;
        [Tooltip("反选物体")]
        public List<GameObject> OppActiveObjects;
        #endregion

        #region Data Param

        #endregion

        #region Local Data

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

        public void SetVisible(bool state)
        {
            foreach (var item in ActiveObjects)
            {
                item.SetActive(state);
            }
            foreach (var item in OppActiveObjects)
            {
                item.SetActive(!state);
            }
        }

        #endregion

    }
}