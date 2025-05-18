using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using YxFramwork.Framework;
using YxFramwork.Tool;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketWithdrawLogItem : YxView
    {
        public NguiLabelAdapter CreatTime;
        public NguiLabelAdapter Money;
        public NguiLabelAdapter StateInfo;
        public NguiLabelAdapter Desc;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var redPacketWithdrawLogData = Data as RedPacketWithdrawLogData;
            if (redPacketWithdrawLogData == null) return;
            CreatTime.TrySetComponentValue(redPacketWithdrawLogData.CreateDt);
            Money.TrySetComponentValue(YxUtiles.GetShowNumberForm(redPacketWithdrawLogData.Value));
            StateInfo.TrySetComponentValue(redPacketWithdrawLogData.StatusInfo);
            Desc.TrySetComponentValue(redPacketWithdrawLogData.Desc);
        }
    }
}