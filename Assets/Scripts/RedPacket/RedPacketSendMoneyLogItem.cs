using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using YxFramwork.Framework;
using YxFramwork.Tool;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketSendMoneyLogItem : YxView
    {
        public NguiLabelAdapter Money;
        public NguiLabelAdapter CreateDt;
        public NguiLabelAdapter Desc;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var redPacketSendMoneyLogData = Data as RedPacketSendMoneyLogData;
            if (redPacketSendMoneyLogData == null) return;
            Money.TrySetComponentValue(YxUtiles.GetShowNumberForm(redPacketSendMoneyLogData.CoinNum));
            CreateDt.TrySetComponentValue(redPacketSendMoneyLogData.CreateDt);
            Desc.TrySetComponentValue(redPacketSendMoneyLogData.Desc);
        }
    }
}
