using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketSendMoneyWindow : YxNguiRedPacketWindow
    {
        public List<UIToggle> RedPacketNums;
        public NguiLabelAdapter SendMoney;
        public NguiLabelAdapter SendMoneyShow;
        public List<UIToggle> ThunderNums;
        public NguiLabelAdapter SendLimintTip;
        private int _sendMixMoney;
        private int _sendMaxMoney;
        public string WindowName;

        protected override void OnStart()
        {
            base.OnStart();
            Facade.Instance<TwManager>().SendAction("RedEnvelope.sendRedEnvelopeLimit", new Dictionary<string, object>(), FreshSendLimit);

        }

        private void FreshSendLimit(object obj)
        {
            var data = obj as Dictionary<string, object>;
            if(data==null)return;
            _sendMixMoney = int.Parse(data["minMoney"].ToString());
            _sendMaxMoney = int.Parse(data["maxMoney"].ToString());
            var tip = data["desc"].ToString();
            SendLimintTip.Text(tip);
        }

        public void OnSendRedPacket()
        {
            var dic = new Dictionary<string, object>();
            foreach (var num in RedPacketNums)
            {
                if (num.value)
                {
                    dic["grab_n"] = num.name;
                }
            }

            var str = "";
            foreach (var num in ThunderNums)
            {
                if (num.value)
                {
                    var count = string.Format("{0},", num.name);
                    str += count;
                }
            }

            if (string.IsNullOrEmpty(str))
            {
                YxMessageTip.Show("请选择中雷号码");
                return;
            }
            str = str.Substring(0, str.Length - 1);
            dic["inmine_num"] = str;
            if (string.IsNullOrEmpty(SendMoney.Value)||SendMoney.Value.Equals("0.00"))
            {
                YxMessageTip.Show("请设置金额");
                return;
            }

            if (int.Parse(SendMoney.Value) < _sendMixMoney || int.Parse(SendMoney.Value) > _sendMaxMoney)
            {
                var sendTip = string.Format("红包的范围最低{0}最大{1}!", _sendMixMoney, _sendMaxMoney);
                YxMessageTip.Show(sendTip);
                return;
            }

            dic["money_a"] = YxUtiles.RecoverShowNumber(Double.Parse(SendMoney.Value));
            var win= CreateChildWindow(WindowName);
            win.UpdateView(dic);
        }

        public void OnChangeToggleGroup(UIToggle toggle)
        {
            if (toggle.value && int.Parse(toggle.name) == 7)
            {
                foreach (var thunderNum in ThunderNums)
                {
                    thunderNum.group = 10;
                }
            }

            if (toggle.value && int.Parse(toggle.name) == 9)
            {
                foreach (var thunderNum in ThunderNums)
                {
                    thunderNum.group = 0;
                }
            }
        }

        public void OnChangeValue()
        {
            SendMoneyShow.TrySetComponentValue(SendMoney.Value);
        }
    }
}
