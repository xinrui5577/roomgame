using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

/*===================================================
 *文件名称:     ResultWindow.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-06-26
 *描述:        	百家乐结算面板
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.nbjl
{
    public class ResultWindow : YxNguiWindow 
    {
        #region UI Param
        [Tooltip("庄家记录分数文本")]
        public UILabel BankerScoreLabel;
        [Tooltip("玩家分数记录文本")]
        public UILabel PlayerScoreLabel;

        #endregion

        #region Data Param
        [Tooltip("胜利文本格式")]
        public string WinLabelFormat = "+{0}";
        [Tooltip("失败文本格式")]
        public string LoseLabelFormat = "-{0}";
        [Tooltip("胜利文本颜色")]
        public Color WinLabelColor=Color.green;
        [Tooltip("失败文本颜色")]
        public Color LoseLabelColor = Color.red;

        #endregion

        #region Local Data

        protected override void OnAwake()
        {
            base.OnAwake();
            Facade.EventCenter.AddEventListeners<LocalRequest, int>(LocalRequest.Init, OnInit);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Facade.EventCenter.RemoveEventListener<LocalRequest, int>(LocalRequest.Init, OnInit);
        }

        #endregion

        #region Life Cycle
        protected override void OnFreshView()
        {
            var bankerScore= App.GetGameData<NbjlGameData>().BankerWinGold;
            var playerScore = App.GetGameData<NbjlGameData>().Win;
            Color cacheColor;
            if (BankerScoreLabel.TrySetComponentValue(GetLabelStyle(bankerScore,out cacheColor)))
            {
                BankerScoreLabel.color = cacheColor;
            }
            if (PlayerScoreLabel.TrySetComponentValue(GetLabelStyle(playerScore, out cacheColor)))
            {
                PlayerScoreLabel.color = cacheColor;
            }

        }

        #endregion

        #region Function
        /// <summary>
        /// 获取文本样式
        /// </summary>
        /// <param name="score"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        private string GetLabelStyle(long score,out Color color)
        {
            var state = score > 0;
            var reduceNum = YxUtiles.ReduceNumber(score);
            color = state ? WinLabelColor : LoseLabelColor;
            var returnStr = state ? string.Format(WinLabelFormat, reduceNum) : string.Format(LoseLabelFormat, reduceNum);
            return returnStr;
        }

        /// <summary>
        /// 初始化消息
        /// </summary>
        /// <param name="num"></param>
        private void OnInit(int num)
        {
            Close();
        }

        #endregion
    }
}
