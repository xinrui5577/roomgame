using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework.Core;

/*===================================================
 *文件名称:     PludoGameInfoPanel.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-12-20
 *描述:         飞行棋游戏信息面板
 *              包括信息：1.房间号 2.最大局数3.当前局数.4.房间规则
 *              其中123显示区域为主界面4为弹出界面
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo.View
{
    public class PludoGameInfoPanel : PludoFreshView
    {
        #region UI Param
        [Tooltip("房间号")]
        public YxBaseLabelAdapter RoomId;
        [Tooltip("局数信息")]
        public YxBaseLabelAdapter RoundInfo;
        [Tooltip("房间名称")]
        public YxBaseLabelAdapter RoomName;
        #endregion

        #region Data Param
        [Tooltip("局数格式")]
        public string RoundFormat = "   局数:{0}/{1}";
        [Tooltip("房间号格式")]
        public string RoomIdFormat = "房间号:{0}";
        [Tooltip("房间名称格式")]
        public string RoomNameFormat = "{0}";
        /// <summary>
        /// 是否为创建房间
        /// </summary>
        public bool IsCreateRoom { get; private set; }

        #endregion

        #region Local Data

        #endregion

        #region Life Cycle

        protected override void OnAwake()
        {
            base.OnAwake();
            Facade.EventCenter.AddEventListeners<LoaclRequest, RoomInfo>(LoaclRequest.RoomInfo, OnRoomInfoChange);
        }

        public override void OnDestroy()
        {
            Facade.EventCenter.RemoveEventListener<LoaclRequest, RoomInfo>(LoaclRequest.RoomInfo, OnRoomInfoChange);
            base.OnDestroy();
        }

        protected override void OnFreshViewWithData()
        {
            base.OnFreshViewWithData();
            var info = Data as RoomInfo;
            if (info != null)
            {
                if(info.IsCreateRoom)
                {
                    RoomId.TrySetComponentValue(string.Format(RoomIdFormat, info.CreateRoomInfo.RoomId));
                    RoundInfo.TrySetComponentValue(string.Format(RoundFormat, info.CreateRoomInfo.CurRound, info.CreateRoomInfo.MaxRound));
                }
                else
                {
                    RoomName.TrySetComponentValue(string.Format(RoomNameFormat, info.RoomName));
                }

                IsCreateRoom = info.IsCreateRoom;
            }
        }

        #endregion

        #region Function
        /// <summary>
        /// 房间信息初始化
        /// </summary>
        /// <param name="info"></param>
        private void OnRoomInfoChange(RoomInfo info)
        {
            UpdateView(info);
        }

        #endregion
    }
}
