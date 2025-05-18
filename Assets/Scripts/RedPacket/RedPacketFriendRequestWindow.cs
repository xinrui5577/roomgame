using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketFriendRequestWindow : YxNguiRedPacketWindow
    {
        public RedPacketFriendsItem RedPacketFriendsItem;
        public UIGrid RedPacketGrid;

        private GameObject _curGameObject;


        protected override void OnStart()
        {
            base.OnStart();
            var dic = new Dictionary<string, object>();
            dic["op"] = 0;
            Facade.Instance<TwManager>().SendAction("RedEnvelope.friendRequest", dic, FreshFriendRequest, true, null, false);
        }

        private void FreshFriendRequest(object obj)
        {
            var dic = obj as Dictionary<string, object>;
            if (dic == null) return;
            var datas = dic["data"] as List<object>;
            if (datas == null) return;
            foreach (var data in datas)
            {
                var item = YxWindowUtils.CreateItem(RedPacketFriendsItem, RedPacketGrid.transform);
                var redPacketFriendData = new RedPacketFriendData(data);
                item.UpdateView(redPacketFriendData);
            }

            RedPacketGrid.repositionNow = true;
        }


        public void OnReceiveBtn(GameObject obj)
        {
            _curGameObject = obj;
            var dic = new Dictionary<string, object>();
            dic["other_id"] = obj.name;
            dic["option"] = 2;
            Facade.Instance<TwManager>().SendAction("RedEnvelope.opFriend", dic, FreshView);
        }
        public void OnRefuseBtn(GameObject obj)
        {
            _curGameObject = obj;
            var dic = new Dictionary<string, object>();
            dic["other_id"] = obj.name;
            dic["option"] = 3;
            Facade.Instance<TwManager>().SendAction("RedEnvelope.opFriend", dic, FreshView);
        }

        private void FreshView(object obj)
        {
            DestroyImmediate(_curGameObject);
            RedPacketGrid.repositionNow = true;
        }
    }
}
