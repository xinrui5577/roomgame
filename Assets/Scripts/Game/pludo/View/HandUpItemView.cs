using Assets.Scripts.Common.Utils;
using UnityEngine;

/*===================================================
 *文件名称:     HandUpItemView.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2019-01-11
 *描述:        	投票单人信息
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo.View
{
    public class HandUpItemView : PludoFreshView
    {
        #region UI Param
        [Tooltip("操作状态标记")]
        public UISprite OperationState;
        [Tooltip("玩家")]
        public PludoGamePlayer Player;

        #endregion

        #region Data Param

        #endregion

        #region Local Data

        private HandUpItemData _curData;
        #endregion

        #region Life Cycle

        protected override void OnFreshViewWithData()
        {
            base.OnFreshViewWithData();
            _curData = Data as HandUpItemData;
            if (_curData!=null)
            {
                Player.UpdateView(_curData.Info);
                OperationState.TrySetComponentValue(_curData.Status.ToString());
            }
        }

        #endregion

        #region Function

        public void OnStateChange(HandUpStatus status)
        {
            _curData.SetHandState((int)status);
            OperationState.TrySetComponentValue(_curData.Status.ToString());
        }

        #endregion
    }
}
