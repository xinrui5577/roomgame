using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using YxFramwork.Common.DataBundles;
using YxFramwork.Framework;
using YxFramwork.Tool;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketFriendChatLogItem : YxView
    {
        public NguiTextureAdapter Head;
        public NguiLabelAdapter UserName;
        public NguiLabelAdapter Money;
        public NguiLabelAdapter CreatTime;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var redPacketFriendChatLog = Data as RedPacketFriendChatLog;
            if(redPacketFriendChatLog==null)return;
            PortraitDb.SetPortrait(redPacketFriendChatLog.Avatarx, Head, 1);
            UserName.TrySetComponentValue(redPacketFriendChatLog.Nickm);
            CreatTime.TrySetComponentValue(redPacketFriendChatLog.CreateDt);
            var str = string.Format("￥{0}", YxUtiles.GetShowNumberForm(redPacketFriendChatLog.CoinNum));
            Money.TrySetComponentValue(str);
        }
    }
}
