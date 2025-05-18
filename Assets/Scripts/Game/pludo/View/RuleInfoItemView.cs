using Assets.Scripts.Common.Utils;
using UnityEngine;

/*===================================================
 *文件名称:     RuleInfoItemView.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2019-01-16
 *描述:        	规则ItemView
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo.View
{
    public class RuleInfoItemView : PludoFreshView 
    {
        #region UI Param
        [Tooltip("信息")]
        public UILabel Info;
        [Tooltip("线")]
        public UISprite Line;
        #endregion

        #region Data Param
        #endregion

        #region Local Data


        #endregion

        #region Life Cycle

        protected override void OnFreshViewWithData()
        {
            base.OnFreshViewWithData();
            if (Data is string)
            {
                Info.TrySetComponentValue(Data.ToString());
            }
        }

        public Vector3 GetLineVector3()
        {
            if (Line)
            {
                return transform.localPosition + Line.transform.localPosition;
            }
            return Vector3.zero;
        }

        #endregion

        #region Function
        #endregion
    }
}
