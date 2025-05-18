using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using YxFramwork.Framework;
using YxFramwork.Tool;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketTeamItem : YxView
    {
        public NguiLabelAdapter UserNick;
        public NguiLabelAdapter Superior;
        public NguiLabelAdapter Grade;
        public NguiLabelAdapter GetMoney;


        protected override void OnFreshView()
        {
            base.OnFreshView();
            var redPacketTeamData = Data as RedPacketTeamData;
            if (redPacketTeamData == null) return;
            UserNick.TrySetComponentValue(redPacketTeamData.UserNick);
            Superior.TrySetComponentValue(redPacketTeamData.AffNick);
            Grade.TrySetComponentValue(redPacketTeamData.Lvl);
            GetMoney.TrySetComponentValue(YxUtiles.GetShowNumberForm(redPacketTeamData.CommissionCur));
        }
    }
}
