using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Tool;

/*===================================================
 *文件名称:     PludoGameOverScoreItem.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2019-01-13
 *描述:        	飞行棋大结算分数Item
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo.View
{
    public class PludoGameOverScoreItem : PludoFreshView 
    {
        #region UI Param
        [Tooltip("分数")]
        public PludoScoreLabel ScoreNum;
        [Tooltip("Id")]
        public UILabel IdLabel;

        #endregion

        #region Data Param
        /// <summary>
        /// 显示Id开关
        /// </summary>
        public bool ShowId
        {
            private set; get;
        }

        #endregion

        #region Local Data

        #endregion

        #region Life Cycle

        protected override void OnFreshViewWithData()
        {
            base.OnFreshViewWithData();
            var scoreInfo = Data as PludoGameOverScoreItemData;
            if (scoreInfo!=null)
            {
                ShowId = scoreInfo.ShowId;
                if (ScoreNum)
                {
                    ScoreNum.UpdateView(YxUtiles.GetShowNumber(scoreInfo.ScoreNumber));
                }
                IdLabel.TrySetComponentValue(scoreInfo.IdNumber.ToString());
            }
        }

        #endregion

        #region Function

        #endregion
    }

    /// <summary>
    /// 分数数据
    /// </summary>
    public class PludoGameOverScoreItemData
    {
        public bool ShowId;

        public int IdNumber;

        public int ScoreNumber;
    }
}
