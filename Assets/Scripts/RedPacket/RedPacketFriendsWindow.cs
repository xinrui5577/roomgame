using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketFriendsWindow : YxNguiWindow
    {
        public RedPacketFriendsItem RedPacketFriendsItem;
        public UIGrid RedPacketGrid;
        public UILabel FriendRequestCount;

        protected override void OnEnable()
        {
            base.OnEnable();
            Facade.Instance<TwManager>().SendAction("RedEnvelope.myFriends", new Dictionary<string, object>(), FreshFriends,true,null,false);
            Facade.Instance<TwManager>().SendAction("RedEnvelope.friendRequest", new Dictionary<string, object>(), FreshFriendRequestCount, true, null, false);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Clear();
        }

        private void FreshFriends(object obj)
        {
            var dic = obj as Dictionary<string, object>;
            if (dic == null) return;
            Clear();
            var data = dic["data"];
            var count = int.Parse(dic["count"].ToString());
            var totalCount = int.Parse(dic["totalCount"].ToString());

            var dataList = data as List<object>;
            if (dataList != null)
            {
                foreach (var dataObj in dataList)
                {
                    var item = YxWindowUtils.CreateItem(RedPacketFriendsItem, RedPacketGrid.transform);
                    var redPacketFriendData = new RedPacketFriendData(dataObj);
                    item.UpdateView(redPacketFriendData);
                }
            }

            RedPacketGrid.repositionNow = true;
        }

        private void FreshFriendRequestCount(object obj)
        {
            var dic = obj as Dictionary<string, object>;
            if (dic == null) return;
            var datas = dic["data"] as List<object>;
            if (datas == null)
            {
                FriendRequestCount.gameObject.SetActive(false);
                return;
            }
            var count = datas.Count;
            FriendRequestCount.gameObject.SetActive(count != 0);
            FriendRequestCount.TrySetComponentValue(count.ToString());
        }

        private void Clear()
        {
            while (RedPacketGrid.transform.childCount>0)
            {
                DestroyImmediate(RedPacketGrid.transform.GetChild(0).gameObject);
            }
        }

        public void OpenUrl(string objName)
        {
            if (!string.IsNullOrEmpty(objName))
            {
                Application.OpenURL(objName);
            }
        }
    }

    public class RedPacketFriendData
    {
        public string Avatar;
        public string NickName;
        public string OtherNickName;
        public int UserId;
        public int RemindCount;

        public RedPacketFriendData(object obj)
        {
            var dic = obj as Dictionary<string, object>;
            if (dic != null)
            {
                if (dic.ContainsKey("avatar_x"))
                {
                    Avatar = dic["avatar_x"].ToString();
                }

                if (dic.ContainsKey("nick_m"))
                {
                    NickName = dic["nick_m"].ToString();
                }
                if (dic.ContainsKey("other_nick_m"))
                {
                    OtherNickName = dic["other_nick_m"].ToString(); 
                }
                if (dic.ContainsKey("user_id"))
                {
                    UserId = int.Parse(dic["user_id"].ToString()); 
                }
                if (dic.ContainsKey("remindCount"))
                {
                    RemindCount = int.Parse(dic["remindCount"].ToString());
                }
            }
        }
    }
}
