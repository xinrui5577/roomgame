/** 
 *文件名称:     MulLayoutByGrid.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-11-04 
 *描述:         UIGrid辅助类，处理多物体时布局显示，根据设置宽高动态设置Grid间距
 *历史记录: 
*/

using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Common.UI
{
    public class MulLayout : MonoBehaviour
    {
        [Tooltip("Grid")]
        public UIGrid Grid;
        [Tooltip("显示范围")]
        public UIWidget Widget;
        /// <summary>
        /// 单独Item的宽度
        /// </summary>
        private int itemWidth;
        /// <summary>
        ///  显示宽度
        /// </summary>
        private int _rangeWidth;
        /// <summary>
        /// 显示高度
        /// </summary>
        private int _rangeHeight;
        /// <summary>
        /// 行数
        /// </summary>
        private int _rowNum;
        /// <summary>
        /// 一行最多数量
        /// </summary>
        private int _maxPerLine;
        void Awake()
        {
            _rangeWidth = Widget.width;
            _rangeHeight = Widget.height;
            _maxPerLine = Grid.maxPerLine;
        }

        public void ResetLayout()
        {
            var childCount = Grid.GetChildList().Count;
            if(childCount==0)
            {
                YxDebug.LogError(string.Format("{0} child count is :{1},please check!",name,childCount));
                return;
            }
            var itemCell = _rangeWidth / (childCount > _maxPerLine ? _maxPerLine : childCount);
            Grid.cellWidth = itemCell;
            Grid.repositionNow=true;
        }
    }
}
