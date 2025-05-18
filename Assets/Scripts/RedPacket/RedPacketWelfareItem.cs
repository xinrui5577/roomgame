using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketWelfareItem : YxView
    {
        public NguiLabelAdapter Money;
        public NguiLabelAdapter CreatDt;
        public NguiLabelAdapter RedShowState;
        public NguiSpriteAdapter RedPacketBg;
        public string HasGetRedPacket;
        public string OpenWindowName;


        private RedPacketWelfareData _redPacketWelfareData;
        protected override void OnFreshView()
        {
            base.OnFreshView();
            _redPacketWelfareData = Data as RedPacketWelfareData;
            if(_redPacketWelfareData == null)return;
            Money.TrySetComponentValue(_redPacketWelfareData.Money);
            CreatDt.TrySetComponentValue(_redPacketWelfareData.DateCreated);
        }

        public void OnClick()
        {
            var dic = new Dictionary<string, object>();
            dic["id"] = _redPacketWelfareData.RedId;
            dic["op"] = 1;
            dic["type"] = 2;
            Facade.Instance<TwManager>().SendAction("RedEnvelope.grabRedEnvelopeWelfare", dic, FreshRedPacketState, true, null, false);
        }
        private void FreshRedPacketState(object obj)
        {
            var redPacketStateData = new RedPacketStateData(obj);
            var win = MainYxView as YxWindow;
            if (win)
            {
                var child = win.CreateChildWindow(OpenWindowName);
                child.UpdateView(redPacketStateData);
            }
            RedShowState.TrySetComponentValue("已领取");
            RedPacketBg.SetSpriteName(HasGetRedPacket);
        }
    }
}
