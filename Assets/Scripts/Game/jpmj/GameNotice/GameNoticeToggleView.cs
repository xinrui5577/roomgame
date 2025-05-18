using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Framework;

/*===================================================
 *文件名称:     GameNoticeToggleView.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-10-30
 *描述:        	游戏公告开关
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.jpmj.GameNotice
{
    public class GameNoticeToggleView :YxView 
    {
        #region UI Param
        [Tooltip("标签名称")]
        public Text TabName;
        [Tooltip("开关")]
        public Toggle Toggle;
        #endregion

        #region Data Param
        #endregion

        #region Local Data
        /// <summary>
        /// 本地数据
        /// </summary>
        private GameNoticeItemData _localData;

        #endregion

        #region Life Cycle

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if(Data is GameNoticeItemData)
            {
                _localData = Data as GameNoticeItemData;
                if (TabName)
                {
                    TabName.text = _localData.TabName;
                }
            }
        }


        public Toggle GetToggle()
        {
            return Toggle;
        }


        #endregion

        #region Function
        #endregion
    }
}
