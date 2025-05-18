using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

/*===================================================
 *文件名称:     BankListWindow.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-06-30
 *描述:        	
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.nbjl
{
    public class BankListWindow :RecycleWindow 
    {
        #region UI Param
        [Tooltip("上庄下限")]
        public YxBaseLabelAdapter BankerLimit;
        [Tooltip("移动位置")]
        public TweenPosition TweenPos;
        [Tooltip("显示背景框")]
        public UITexture Bg;
        #endregion

        #region Data Param
        [Tooltip("上庄列表默认背景高度")]
        public int BankListBgDefaultHeight = 130;
        [Tooltip("上庄列表item高度")]
        public int BankListGridHeight = 70;
        [Tooltip("上庄列表背景最大高度")]
        public int BankListBgMaxHeight = 500;
        #endregion

        #region Local Data

        #endregion

        #region Life Cycle

        protected override void OnAwake()
        {
            base.OnAwake();
            Facade.EventCenter.AddEventListeners<LocalRequest, int>(LocalRequest.BankerLimit, OnFreshBankLimit);
        }

        #endregion

        #region Function

        /// <summary>
        /// 刷新上庄限制信息
        /// </summary>
        /// <param name="bankLimit">上庄限制</param>
        private void OnFreshBankLimit(int bankLimit)
        {
            BankerLimit.TrySetComponentValue(YxUtiles.ReduceNumber(bankLimit));
        }



        /// <summary>
        /// 设置背景高度
        /// </summary>
        public void SetTextureHeight()
        {
            var count = CacheViews.Count;
            DatasExist = count != 0;
            if (count <= 1)
            {
                Bg.height = BankListBgDefaultHeight;
            }
            else
            {
                var getHeight = (count - 1) * BankListGridHeight + BankListBgDefaultHeight;
                Bg.height = getHeight > BankListBgMaxHeight ? BankListBgMaxHeight : getHeight;
            }
            var toPos = TweenPos.@from - new Vector3(0, Bg.height);
            TweenPos.to = toPos;
            if (OpenState)
            {
                TweenPos.PlayForward();
            }
        }
        #endregion
    }
}
