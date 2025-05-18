using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using YxFramwork.Framework;
using YxFramwork.Tool;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketEarningsItem : YxView
    {
        public NguiLabelAdapter UserNick;
        public NguiLabelAdapter GetMoney;
        public NguiLabelAdapter LowerLevel;
        public NguiLabelAdapter CreateDate;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var redPacketEarningsData = Data as RedPacketEarningsData;
            if (redPacketEarningsData == null) return;
            UserNick.TrySetComponentValue(redPacketEarningsData.UserNick);
            string str = "";
            if (redPacketEarningsData.Commission >= 0)
            {
                str = string.Format("+{0}", YxUtiles.GetShowNumberForm(redPacketEarningsData.Commission));
            }

            if (redPacketEarningsData.Style == 1)
            {
                str += "(发)";
            }
            else
            {
                str += "(抢)";
            }
            GetMoney.TrySetComponentValue(str);
            LowerLevel.TrySetComponentValue(redPacketEarningsData.Lvl);
            CreateDate.TrySetComponentValue(redPacketEarningsData.CreateDt);
        }
    }
}
