using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

/*===================================================
 *文件名称:     PludoGameOverWindow.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2019-01-13
 *描述:        	飞行棋大结算窗口
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo.View
{
    public class PludoGameOverWindow : PludoFreshWindow 
    {
        #region UI Param
        [Tooltip("大结算时间")]
        public UILabel TimeLabel;
        [Tooltip("本局游戏规则")]
        public UILabel RuleLabel;
        [Tooltip("大结算Item")]
        public YxView GameOverItemPrefab;
        [Tooltip(" Item 容器")]
        public Transform ItemParent;
        #endregion

        #region Data Param
        [Tooltip("房间信息格式")]
        public string RuleFormat = "房间信息:{0}{1}";
        [Tooltip("时间信息格式")]
        public string TimeFormat = "时间:{0}";
        [Tooltip("房间号格式化")]
        public string RoomIdFormat = "房间号:{0}";
        #endregion

        #region Local Data

        private PludoGameOverData _curData;
        #endregion

        #region Life Cycle

        protected override void OnFreshViewWithData()
        {
            base.OnFreshViewWithData();
            _curData = Data as PludoGameOverData;
            if (_curData!=null)
            {
                var count = _curData.OverDatas.Count;
                for (int i = 0; i < count; i++)
                {
                    var view = ItemParent.GetChildView(i, GameOverItemPrefab);
                    view.IdCode = i;
                    view.UpdateView(_curData.OverDatas[i]);
                }
                if (_curData.RoomInfo.IsCreateRoom)
                {
                    var idInfo = string.Format(RoomIdFormat, _curData.RoomInfo.CreateRoomInfo.RoomId);
                    RuleLabel.TrySetComponentValue(string.Format(RuleFormat, idInfo, _curData.RoomInfo.Rule));
                }
                TimeLabel.TrySetComponentValue(string.Format(TimeFormat, _curData.Time));
            }
        }

        #endregion

        #region Function

        /// <summary>
        /// 点击分享按钮（分享截图）
        /// </summary>
        public void OnClickShareBtn(string sharePlat)
        {
            SharePlat plat = (SharePlat)Enum.Parse(typeof (SharePlat), sharePlat);
            Facade.EventCenter.DispatchEvent(LoaclRequest.ShareImage, plat);
        }
        /// <summary>
        /// 点击返回大厅按钮 
        /// </summary>
        public void OnClickBackToHall()
        {
            App.QuitGameWithMsgBox();
        }

        #endregion
    }

    public class PludoGameOverData
    {
        public List<PludoGameOverItemData> OverDatas=new List<PludoGameOverItemData>();
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
