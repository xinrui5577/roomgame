using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;

/*===================================================
 *文件名称:     AccumulateItem.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-06-30
 *描述:        	累计统计列表Item
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.nbjl
{
    public class AccumulateItem : YxView 
    {
        #region UI Param
        [Tooltip("玩家信息")]
        public NbjlPlayer Player;
        [Tooltip("累计局数")]
        public YxBaseLabelAdapter AccumulateCount;
        [Tooltip("排行标记")]
        public YxBaseLabelAdapter RankNo;

        #endregion

        #region Data Param

        [Tooltip("排行文本格式")]
        public string RankFormat = "NO.{0}";
        [Tooltip("累计局数格式")]
        public string CountNumberFormat = "近{0}局";
        [Tooltip("累计局数")]
        public static int CountNumber;

        [Tooltip("刷新Item信息")]
        public List<EventDelegate> OnItemFresh;

        /// <summary>
        /// 是否是神算子
        /// </summary>
        public bool IsSsz { get; private set; }

        #endregion

        #region Local Data

        #endregion

        #region Life Cycle

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (Data==null)
            {
                return;
            }
            var id = int.Parse(Id);
            IsSsz = id == 0;
            RankNo.TrySetComponentValue(string.Format(RankFormat, id));
            AccumulateCount.TrySetComponentValue(string.Format(CountNumberFormat,CountNumber));
            Player.UpdateView(Data);
            StartCoroutine(OnItemFresh.WaitExcuteCalls());
        }

        #endregion

        #region Function

        #endregion
    }
}
