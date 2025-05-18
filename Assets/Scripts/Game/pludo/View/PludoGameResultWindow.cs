using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;

/*===================================================
 *文件名称:     PludoGameResultWindow.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-12-19
 *描述:        	飞行棋小结算
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo.View
{
    public class PludoGameResultWindow : PludoFreshWindow 
    {
        #region UI Param
        [Tooltip("结果提示")]
        public UISprite ResultNoticeSprite;
        [Tooltip("结果Item")]
        public YxView ResultItemPrefab;
        [Tooltip("Item显示父级")]
        public Transform ItemsShowParent;
        [Tooltip("游戏时间")]
        public UILabel GameTime;
        [Tooltip("游戏规则")]
        public UILabel GameRule;
        #endregion

        #region Data Param
        /// <summary>
        /// 房间信息格式
        /// </summary>
        public string RuleFormat = "房间信息:{0}";
        /// <summary>
        /// 时间信息格式
        /// </summary>
        public string TimeFormat = "时间:{0}";

        /// <summary>
        /// 是否为娱乐房
        /// </summary>
        public bool IsCreateRoom
        {
            set; get;
        }
        /// <summary>
        /// 是否为最后一局（用于控制继续按钮与返回大厅按钮的显示）
        /// </summary>
        public bool IsLastRound
        {
            set; get;
        }

        #endregion

        #region Local Data

        private PludoGameResultData _curData;
        #endregion

        #region Life Cycle

        protected override void OnFreshViewWithData()
        {
            base.OnFreshViewWithData();
            _curData = Data as PludoGameResultData;
            if (_curData != null)
            {
                ResultNoticeSprite.TrySetComponentValue(string.Format(ConstantData.ResultTitleFormat, (ItemColor)_curData.ResultList.First().Info.PlayerColor));
                var count = _curData.ResultList.Count;
                for (int i = 0; i < count; i++)
                {
                    var view = ItemsShowParent.GetChildView(i, ResultItemPrefab);
                    view.IdCode = i + 1;
                    view.UpdateView(_curData.ResultList[i]);
                }
                IsCreateRoom = _curData.RoomInfo.IsCreateRoom;
                GameRule.TrySetComponentValue(string.Format(RuleFormat, IsCreateRoom ? _curData.RoomInfo.Rule : _curData.RoomInfo.RoomName));
                GameTime.TrySetComponentValue(string.Format(TimeFormat, _curData.Time));
                IsLastRound = _curData.RoomInfo.IsLastRound;
            }
        }

        #endregion

        #region Function
        /// <summary>
        /// 点击继续游戏
        /// </summary>
        public void OnContinueClick()
        {
            Facade.EventCenter.DispatchEvent(LoaclRequest.NeedToReady, ConstantData.IntDefValue);
            Close();
        }
        /// <summary>
        /// 点击返回大厅
        /// </summary>
        public void OnClickBackToHall()
        {
            App.QuitGameWithMsgBox();
        }
        /// <summary>
        /// 点击查看详情(查看大结算信息)
        /// </summary>
        public void OnClickShowDetailBtn()
        {
            Facade.EventCenter.DispatchEvent(LoaclRequest.GameOverShow,ConstantData.IntDefValue);
        }

        #endregion
    }

    /// <summary>
    /// 飞行棋小结算数据
    /// </summary>
    public class PludoGameResultData
    {
       /// <summary>
       /// 结算数据列表
       /// </summary>
       public List<PludoGameResultItemData> ResultList=new List<PludoGameResultItemData>();
       /// <summary>
       /// 房间信息
       /// </summary>
       public RoomInfo RoomInfo;
       /// <summary>
       /// 结束时间
       /// </summary>
       public string Time;
    }
}
