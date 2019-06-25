using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Hall.View.AboutRoomWindows
{
    public class CreateRoomListWindow : YxNguiWindow
    {
        public UIToggle RankToggle;
        public UIToggle RoomToggle;
        public UIGrid RoomInfoGrid;
        public CreateRoomListItem CreateRoomListItem;

        void Start()
        {
            var dic = new Dictionary<string, object>();
            dic["gameKey"] = App.GameKey;
            Facade.Instance<TwManger>().SendAction("room.getMyRooms", dic, OnInitPanelCtrl);
        }

        private void OnInitPanelCtrl(object data)
        {
            if (!(data is List<object>))
            {
                if (RankToggle == null) return;
                RankToggle.startsActive = true;
                RankToggle.value = true;
                RankToggle.GetComponent<UIToggledObjects>().activate[0].SetActive(true);
                return;
            }
            else
            {
                if (RoomToggle == null) return;
                RoomToggle.startsActive = true;
                RoomToggle.value = true;
                RoomToggle.GetComponent<UIToggledObjects>().activate[0].SetActive(true);
            }
        }

      
        public void OnRoomRankBtn(UIToggle toggle)
        {
            if (!toggle.value) return;
            var dic = new Dictionary<string, object>();
            dic["gameKey"] = App.GameKey;
            Facade.Instance<TwManger>().SendAction("room.getMyRooms", dic, OnFreshRoomInfo);
        }

        private void OnFreshRoomInfo(object data)
        {
            if (data == null) return;
            while (RoomInfoGrid.transform.childCount > 0)
            {
                DestroyImmediate(RoomInfoGrid.transform.GetChild(0).gameObject);
            }
            if (!(data is List<object>)) return;

            var dataInfos = data as List<object>;
            foreach (var dataInfo in dataInfos)
            {
                if (!(dataInfo is Dictionary<string, object>)) continue;
                var rInfo = dataInfo as Dictionary<string, object>;
                var userNum = 0;
                var id = rInfo.ContainsKey("id") ? rInfo["id"].ToString() : "";
                var rndId = rInfo.ContainsKey("rndId") ? rInfo["rndId"].ToString() : "";
                var gameName = rInfo.ContainsKey("name") ? rInfo["name"].ToString() : "";
                var gamekey = rInfo.ContainsKey("gamekey") ? rInfo["gamekey"].ToString() : "";
                var round = rInfo.ContainsKey("round") ? rInfo["round"].ToString() : "";
                var capacity = rInfo.ContainsKey("capacity") ? rInfo["capacity"].ToString() : "";
                var status = rInfo.ContainsKey("status") ? rInfo["status"].ToString() : "";
                var users = rInfo.ContainsKey("users") ? rInfo["users"] : null;
                var ante = rInfo.ContainsKey("ante") ? rInfo["ante"].ToString() : "1";
                if (users is List<object>)
                {
                    var userNames = users as List<object>;
                    userNum += userNames.Count(userName => (string) userName != "");
                }
                var avatars = rInfo.ContainsKey("avatars") ? rInfo["avatars"].ToString() : "";
                var info = rInfo.ContainsKey("info") ? rInfo["info"].ToString() : "";
                var currentRound = string.Format("{0}/{1}", status, round);
                var currentUserCount = string.Format("{0}/{1}", userNum, capacity);
                var obj = YxWindowUtils.CreateGameObject(CreateRoomListItem.gameObject, RoomInfoGrid.transform);
                obj.name = id;
                obj.GetComponent<CreateRoomListItem>().InitData(rndId, info, ante, currentRound, currentUserCount, gamekey);
                var joinRoom = UIEventListener.Get(obj);
                joinRoom.onClick = OnJoinRoom;
                joinRoom.parameter = obj;
                var weiChat = UIEventListener.Get(obj.GetComponent<CreateRoomListItem>().BtnWeiChat.gameObject);
                weiChat.onClick = OnClickWeiChatInviteBtn;
                weiChat.parameter = obj;
            }
            RoomInfoGrid.cellHeight = 90;
            RoomInfoGrid.repositionNow = true;
            RoomInfoGrid.Reposition();
        }

        private void OnJoinRoom(GameObject obj)
        {
            RoomListController.Instance.JoinFindRoom(int.Parse(obj.name), obj.GetComponent<CreateRoomListItem>().GameKey.text);
        }

        private void OnClickWeiChatInviteBtn(GameObject obj)
        {
            Facade.Instance<WeChatApi>().InitWechat(App.Config.WxAppId);

            UserController.Instance.GetShareInfo(info =>
            {
                var parent = obj.GetComponentInParent<CreateRoomListItem>();
                var playRule = parent.PlayRule.text;
                var roomId = parent.RoomId.text;
                info.ShareData["content"] += string.Format("[麻将];房间号:[{0}];{1}", roomId, playRule);
                Facade.Instance<WeChatApi>().ShareContent(info);
            });
        }
    }
}
