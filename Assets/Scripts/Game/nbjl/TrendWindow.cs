using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Framework.Core;

/*===================================================
 *文件名称:     TrendWindow.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-06-26
 *描述:        	走势界面
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.nbjl
{
    public class TrendWindow : RecycleWindow
    {
        [Tooltip("走势类型")]
        public EnumTrendType Type;
        [Tooltip("列表为空时处理")]
        public List<EventDelegate> OnListEmpty;
        public override void Init()
        {
            var listenerType = Type != EnumTrendType.BeadRoad ? EnumTrendType.BigRoad : EnumTrendType.BeadRoad;
            Facade.EventCenter.AddEventListeners<EnumTrendType, IRecycleData>(listenerType, OnSingleRecord);
        }

        protected override void AddChildrenToShow()
        {
            base.AddChildrenToShow();
            if (CacheData.Count == 0)
            {
                StartCoroutine(OnListEmpty.WaitExcuteCalls(true));
            }
        }

        /// <summary>
        /// 单独的回放消息
        /// </summary>
        /// <param name="data"></param>
        protected virtual void OnSingleRecord(IRecycleData data)
        {
            AddChildToShow(data);
        }
    }
}
