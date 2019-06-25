using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Controller;

namespace Assets.Scripts.Hall.View.AboutRoomWindows
{
    /// <summary>
    /// 房间消息
    /// </summary>
    public class RoomInfoWindow : YxNguiWindow
    {
        public string RoomIdFormat = "{0}";
        public string OwnerFormat = "{0}";

        /// <summary>
        /// 房间号label
        /// </summary>
        public UILabel RoomIdLabel;
        /// <summary>
        /// 房主名称
        /// </summary>
        public UILabel OwnerLabel;
        /// <summary>
        /// 玩家grid
        /// </summary>
        public UIGrid UserInfoGrid;
        /// <summary>
        /// item
        /// </summary>
        public UILabel UserInfoLabelPerfab;
        /// <summary>
        /// 玩家当前人数
        /// </summary>
        public UILabel UserCount;
        private int _roomId;
        private string _gameKey;
        protected override void OnFreshView()
        {
            base.OnFreshView();
            var sfsObj = Data as Dictionary<string,object>;
            if (sfsObj == null)
            {
                return;
            }
            if (sfsObj.ContainsKey("roomId"))
            {
                _roomId = int.Parse(sfsObj["roomId"].ToString()); 
            }
            if (sfsObj.ContainsKey("_roomShowId"))
            {
                if (RoomIdLabel != null) RoomIdLabel.text = string.Format(RoomIdFormat, sfsObj["_roomShowId"]);
            }
            if (sfsObj.ContainsKey("ownerName"))
            {
                var ownerName = sfsObj["ownerName"].ToString().Split('|');
                if (OwnerLabel != null) OwnerLabel.text = string.Format(OwnerFormat, ownerName[0]);
            }
            _gameKey = sfsObj.ContainsKey("gameKey") ? sfsObj["gameKey"].ToString() : null;
            if (!sfsObj.ContainsKey("users")) return;
            var users = sfsObj["users"];
            var objects = users as List<object>;
            if (objects != null)
            {
                var list = objects;
                var count = list.Count;
                if (UserCount != null) UserCount.text = count.ToString();
                var pts = UserInfoGrid.transform;
                for (var i = 0; i < count; i++)
                {
                    var label = YxWindowUtils.CreateItem(UserInfoLabelPerfab, pts);
                    label.text = list[i].ToString();
                }
            }
            UserInfoGrid.repositionNow = true;
            UserInfoGrid.Reposition();
        }

        public void OnJoinRoom()
        {
            if (_roomId < 1) return;
            if (Data == null) return;
            RoomListController.Instance.JoinFindRoom(_roomId, _gameKey);
        }
    }
}
