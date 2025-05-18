using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;
using YxFramwork.Tool;

/*===================================================
 *文件名称:     BankListItem.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-06-28
 *描述:        	上庄列表Item
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.nbjl
{
    public class BankListItem : YxView 
    {
        #region UI Param
        [Tooltip("货币数量")]
        public YxBaseLabelAdapter Value;
        [Tooltip("玩家昵称")]
        public YxBaseLabelAdapter PlayerName;

        #endregion

        #region Data Param

        #endregion

        #region Local Data

        #endregion

        #region Life Cycle

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var getInfo = Data as NbjlPlayerInfo;
            if (getInfo != null)
            {
                Value.TrySetComponentValue(YxUtiles.ReduceNumber(getInfo.CoinA));
                PlayerName.TrySetComponentValue(getInfo.NickM);
            }
        }

        #endregion

        #region Function

        #endregion
    }
}
