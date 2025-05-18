using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Framework;
using YxFramwork.Tool;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketSelfLogItem : YxView
    {
        public NguiLabelAdapter LogTip;
        public NguiLabelAdapter LogCreatTime;
        public NguiLabelAdapter Money;
        public Color GetMoneyColor;
        public Color LostMoneyColor;


        protected override void OnFreshView()
        {
            base.OnFreshView();
            var redPacketSelfLogData = Data as RedPacketSelfLogData;
            if (redPacketSelfLogData == null) return;
            LogCreatTime.TrySetComponentValue(redPacketSelfLogData.DateCreated);
            LogTip.TrySetComponentValue(redPacketSelfLogData.Info);
            Money.TrySetComponentValue(YxUtiles.GetShowNumberForm(redPacketSelfLogData.Coin));
            var color = redPacketSelfLogData.Coin > 0 ? GetMoneyColor : LostMoneyColor;
            Money.Color = color;
        }
    }
}
