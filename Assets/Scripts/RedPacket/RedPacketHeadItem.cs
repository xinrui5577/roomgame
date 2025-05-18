using Assets.Scripts.Common.Adapters;
using YxFramwork.Common.DataBundles;
using YxFramwork.Framework;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketHeadItem : YxView
    {
        public NguiTextureAdapter Head;
        public string Avatar;
        protected override void OnFreshView()
        {
            base.OnFreshView();
            Avatar = Data as string;
            PortraitDb.SetPortrait(Avatar, Head, 1);
        }
    }
}
