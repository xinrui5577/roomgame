using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using YxFramwork.Common.DataBundles;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketFriendsItem : YxView
    {
        public UILabel OtherNiackName;
        public NguiTextureAdapter Head;
        public UILabel FriendChatLogCount;

        public string WindowName;

        private RedPacketFriendData _redPacketFriendData;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var redPacketFriendData = Data as RedPacketFriendData;
            if (redPacketFriendData == null) return;
            _redPacketFriendData = redPacketFriendData;
            PortraitDb.SetPortrait(redPacketFriendData.Avatar, Head, 1);
            OtherNiackName.TrySetComponentValue(redPacketFriendData.OtherNickName);
            name = redPacketFriendData.UserId.ToString();

            FriendChatLogCount.gameObject.SetActive(redPacketFriendData.RemindCount != 0);
            FriendChatLogCount.TrySetComponentValue(redPacketFriendData.RemindCount.ToString());
        }


        public void OnClick()
        {

            var dic = new Dictionary<string, object>();
            dic["op"] = 1;
            dic["other_id"] = _redPacketFriendData.UserId;
            Facade.Instance<TwManager>().SendAction("RedEnvelope.myFriends", dic, null, true, null, false);
            var win = YxWindowManager.OpenWindow(WindowName);
            win.UpdateView(_redPacketFriendData);
        }
    }
}
