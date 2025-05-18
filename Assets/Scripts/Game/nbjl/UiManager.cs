using System.Collections.Generic;
using UnityEngine;

/*===================================================
 *文件名称:     UIManager.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-06-28
 *描述:        	UI 管理。将一些与业务逻辑无关的UI效果调整功能的处理放在这里，将逻辑尽可能与UI操作分开
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.nbjl
{
    public class UiManager : MonoBehaviour 
    {
        #region UI Param
        #endregion

        #region Data Param
        #endregion

        #region Local Data
        #endregion

        #region Life Cycle
        #endregion

        #region Function
        
        /// <summary>
        /// 设置Grid 组间距离
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="cells"></param>
        public void SetGridCellWidth(UIGrid grid, List<int> cells)
        {
            int index=grid.GetChildList().Count;
            grid.cellWidth = cells[index];
        }

        /// <summary>
        /// 设置texture高度
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="baseHeight"></param>
        /// <param name="itemCount"></param>
        /// <param name="cellHeight"></param>
        /// <param name="max"></param>
        /// <param name="tween"></param>
        public void SetTextureHeight(UITexture texture,int baseHeight,int itemCount,int cellHeight,int max,TweenPosition tween)
        {
            if(itemCount<=1)
            {
                texture.height = baseHeight;
            }
            else
            {
                var getHeight= (itemCount - 1) * cellHeight+baseHeight;
                if (getHeight>max)
                {
                    texture.height = max;
                }
                else
                {
                    texture.height = getHeight;
                }
            }
            var toPos = tween.@from-new Vector3(0,texture.height);
            tween.to=toPos;
        }

        /// <summary>
        /// 显示区域移动到末尾
        /// </summary>
        /// <param name="obj"></param>
        public void MoToTail(GameObject obj)
        {
            Bounds b = NGUIMath.CalculateRelativeWidgetBounds(obj.transform);
            var x = b.center.x;
            if (x <= 0)
            {
                return;
            }
            SpringPanel.Begin(obj, new Vector3(-x * 2, 0, 0), 10);
        }

        /// <summary>
        /// 物体移动至位置0
        /// </summary>
        /// <param name="obj"></param>
        public void SpringPanelToZero(GameObject obj)
        {
            SpringPanel.Begin(obj, Vector3.zero, 10000);
        }

        #endregion
    }
}
