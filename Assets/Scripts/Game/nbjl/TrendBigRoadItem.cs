using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common.Adapters;

namespace Assets.Scripts.Game.nbjl
{ /*===================================================
 *文件名称:     TrendBeadRoadItem.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-06-26
 *描述:         大路图走势Item
 *历史记录: 
=====================================================*/
    public class TrendBigRoadItem : TrendNormalItem 
    {
        #region UI Param
        [Tooltip("和数量")]
        public YxBaseLabelAdapter Count;
        #endregion

        #region Data Param
        #endregion

        #region Local Data
        #endregion

        #region Life Cycle

        protected override void ShowItem(RoadNode data)
        {
            base.ShowItem(data);
            var numShow = data.DrawCount > 0;
            if (Count)
            {
                Count.gameObject.TrySetComponentValue(numShow);
                Count.TrySetComponentValue(data.DrawCount);
            }
        }

        #endregion
    }

    /// <summary>
    /// 通用走势数据
    /// </summary>
    public class TrendNormalData : IRecycleData
    {
        /// <summary>
        /// 当前记录位置
        /// </summary>
        public Vector2 Vec { get; protected set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; protected set; }

        public TrendNormalData()
        {

        }

        public TrendNormalData(string type, Vector2 curVec)
        {
            Vec = curVec;
            Type = type;
        }
    }

    /// <summary>
    /// 大路图数据
    /// </summary>
    public class TrendBigRoadData: TrendNormalData
    {
        /// <summary>
        /// 累计数量
        /// </summary>
        public int Count { get;private set; }

        public void AddCount()
        {
            Count += 1;
        }

        public void SetType(string type)
        {
            Type = type;
        }
        public TrendBigRoadData(string type, Vector2 curVec,int count=0)
        {
            Vec = curVec;
            Type = type;
            Count = count;
        }
    }
}