using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using YxFramwork.Common.DataBundles;
using YxFramwork.Framework;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketChatInfoItem : YxView
    {
        public NguiTextureAdapter Head;
        public NguiLabelAdapter UserName;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var redPacketChatInfoData = Data as RedPacketChatInfoData;
            if (redPacketChatInfoData == null) return;
            PortraitDb.SetPortrait(redPacketChatInfoData.Avatar, Head, 1);
            UserName.TrySetComponentValue(redPacketChatInfoData.NickName);
        }
    }
}
