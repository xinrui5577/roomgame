using Assets.Scripts.Common.Utils;
using UnityEngine;

/*===================================================
 *文件名称:     PludoScoreLabel.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2019-01-16
 *描述:        	
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo.View
{
    public class PludoScoreLabel : PludoFreshView 
    {
        #region UI Param
        [Tooltip("获胜分数")]
        public UILabel WinLabel;
        [Tooltip("失败分数")]
        public UILabel LoseLabel;
        /// <summary>
        /// 是否为赢家
        /// </summary>
        public bool IsWinner { get; private set; }
        #endregion

        #region Data Param
        [Tooltip("获胜分数文本格式")]
        public string WinScoreFormat = "+{0}";
        [Tooltip("失败分数文本格式")]
        public string LoseScoreFormat = "{0}";
        #endregion

        #region Local Data

        #endregion

        #region Life Cycle

        protected override void OnFreshViewWithData()
        {
            base.OnFreshViewWithData();
            if(Data is double)
            {
                double value = double.Parse(Data.ToString());
                IsWinner = value >= ConstantData.IntValue;
                WinLabel.TrySetComponentValue(string.Format(WinScoreFormat, value));
                LoseLabel.TrySetComponentValue(string.Format(LoseScoreFormat, value));
            }
        }

        #endregion

        #region Function

        #endregion
    }
}
