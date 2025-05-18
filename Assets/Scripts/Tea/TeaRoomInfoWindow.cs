using System.Collections;
using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Controller;

namespace Assets.Scripts.Tea
{
    public class TeaRoomInfoWindow : YxNguiWindow
    {
        private int RoomId;
        private string GameKey;
        public UILabel Info;
        protected override void OnFreshView()
        {
            var JoinRoomData = GetData<JoinRoomData>();
            if (JoinRoomData == null) return;
            if (JoinRoomData.Info.Contains(";")) JoinRoomData.Info = JoinRoomData.Info.Replace(";", ";\n");
            Info.text = JoinRoomData.Info;
            RoomId = JoinRoomData.roomId;
            GameKey = JoinRoomData.GameKey;
        }

        public void OnJoinRoom()
        {
            RoomListController.Instance.JoinFindRoom(RoomId, GameKey);
        }
    }

    public class JoinRoomData
    {
        public int roomId;
        public string GameKey;
        public string Info ;
    }
}