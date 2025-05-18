using YxFramwork.Framework.Core;

/*===================================================
 *文件名称:     AccumulateWindow.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-06-30
 *描述:         玩家累计界面
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.nbjl
{
    public class AccumulateWindow : RecycleWindow 
    {
        #region Data Param
        #endregion

        #region Local Data

        #endregion

        #region Life Cycle

        protected override void OnAwake()
        {
            base.OnAwake();
            Facade.EventCenter.AddEventListeners<LocalRequest, int>(LocalRequest.AccumulateCount, OnSureCountNumber);
        }

        #endregion

        #region Function
        /// <summary>
        /// 确定统计累计局数
        /// </summary>
        /// <param name="count"></param>
        private void OnSureCountNumber(int count)
        {
            AccumulateItem.CountNumber = count;
        }
        #endregion
    }
}
