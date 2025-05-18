using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using YxFramwork.Framework;
using YxFramwork.Tool;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketRechargeLogItem : YxView
    {
        public NguiLabelAdapter Info;
        public NguiLabelAdapter Money;
        public NguiLabelAdapter PaymentTime;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var redPacketRechargeLogData = Data as RedPacketRechargeLogData;
            if (redPacketRechargeLogData == null) return;
            Info.TrySetComponentValue(redPacketRechargeLogData.Info);
            Money.TrySetComponentValue(YxUtiles.GetShowNumberForm((long)redPacketRechargeLogData.Money));
            PaymentTime.TrySetComponentValue(redPacketRechargeLogData.PaymentDt);
        }
    }
}
