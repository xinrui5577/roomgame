using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;

/*===================================================
 *文件名称:     BetAreaManager.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-06-30
 *描述:         下注管理类
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.nbjl
{
    public class BetsManager: BaseMono 
    {
        #region UI Param
        [Tooltip("筹码按钮")]
        public ChipItem[] Chips;
        [Tooltip("选中颜色")]
        public Color[] SelectColors;
        [Tooltip("选中状态物体")]
        public UISprite SelectObj;
        [Tooltip("Chips 间隔")]
        public UIGrid ChipsGrid;
        #endregion

        #region Data Param
        [Tooltip("下注筹码Grid间隔")]
        public List<int> BetChipGridCells = new List<int>();
        #endregion

        #region Local Data
        /// <summary>
        /// 当前选中的索引
        /// </summary>
        private int _curSelectindex;
        #endregion

        #region Life Cycle

        protected override void OnAwake()
        {
            Facade.EventCenter.AddEventListeners<LocalRequest, List<int>>(LocalRequest.AnteRate, OnGetAnteRate);
        }

        #endregion

        #region Function
        /// <summary>
        /// 下注筹码区域倍率处理
        /// </summary>
        private void OnGetAnteRate(List<int> anteList)
        {
            _curSelectindex = -1;
            var chipCount = Chips.Length;
            var anteCount = anteList.Count;
            for (int i = 0; i < chipCount; i++)
            {
                bool state = i > anteCount;
                var chipItem = Chips[i];
                chipItem.gameObject.SetActive(!state);
                if (state)
                {
                    continue;
                }
                ChipData itemData = new ChipData()
                {
                    Type = i,
                    Value = anteList[i],
                };
                chipItem.UpdateView(itemData);
            }
            OnSelectChip(Chips[0]);
            var count = Math.Min(anteCount, chipCount);
            if(BetChipGridCells.Count>count)
            {
                ChipsGrid.cellWidth = BetChipGridCells[count];
            }
            ChipsGrid.repositionNow = true;
        }

        /// <summary>
        /// 点击选中筹码
        /// </summary>
        /// <param name="index"></param>
        public void OnSelectChip(ChipItem item)
        {
            var checkType = item.Type;
            if(checkType== _curSelectindex)
            {
                return;
            }
            _curSelectindex = checkType;
            if (SelectObj)
            {
                item.gameObject.AddChildToParent(SelectObj.gameObject);
                App.GetGameManager<NbjlGameManager>().SelectChipIndex = item.Type;
                SelectObj.color = SelectColors[item.Type];
                SelectObj.gameObject.SetActive(true);
            }
        }

        #endregion
    }
}
