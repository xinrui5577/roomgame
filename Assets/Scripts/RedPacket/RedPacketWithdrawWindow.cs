using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using YxFramwork.Common.Model;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketWithdrawWindow : YxNguiRedPacketWindow
    {
        public UIInput BankCard;
        public UIInput BankName;
        public UIInput BankKind;
        public UIInput MobilePhone;
        public NguiLabelAdapter BtnName;
        public UIInput WithdrawCount;
        public NguiLabelAdapter TipShow;
        public string WindowName;
        public string Tip;

        protected override void OnStart()
        {
            base.OnStart();
            var userInfo = UserInfoModel.Instance.UserInfo;
            var str = string.Format(Tip, YxUtiles.GetShowNumberForm(userInfo.CoinA));
            TipShow.TrySetComponentValue(str);
            var dic = new Dictionary<string, object>();
            dic["op"] = 0;
            Facade.Instance<TwManager>().SendAction("RedEnvelope.redEnvelopeBindBank", dic, FreshWithdrawView);
        }

        private void FreshWithdrawView(object obj)
        {
            var data = obj as Dictionary<string, object>;
            if (data == null) return;
            var bankA = data.ContainsKey("bank_a") && data["bank_a"] != null;
            if (bankA)
            {
                BankCard.TrySetComponentValue(data["bank_a"].ToString());
            }

            var bankN = data.ContainsKey("bank_name") && data["bank_name"] != null;
            if (bankN)
            {
                BankName.TrySetComponentValue(data["bank_name"].ToString());
            }

            var bankK = data.ContainsKey("bank_kind") && data["bank_kind"] != null;
            if (bankK)
            {
                BankKind.TrySetComponentValue(data["bank_kind"].ToString());
            }

            var mobileN = data.ContainsKey("mobile_n") && data["mobile_n"] != null;
            if (mobileN)
            {
                MobilePhone.TrySetComponentValue(data["mobile_n"].ToString());
            }

            if (bankA && bankN && bankK && mobileN)
            {
                BtnName.TrySetComponentValue("修改信息");
            }
        }

        public void OnSaveData()
        {
            var dic = new Dictionary<string, object>();
            dic["op"] = 1;
            dic["bank_a"] = BankCard.value;
            dic["bank_name"] = BankName.value;
            dic["bank_kind"] = BankKind.value;
            dic["mobile_n"] = MobilePhone.value;
            Facade.Instance<TwManager>().SendAction("RedEnvelope.redEnvelopeBindBank", dic, FreshTip);
        }

        private void FreshTip(object obj)
        {
            var data = obj as Dictionary<string, object>;
            if(data==null)return;
            var tip = data["tip"].ToString();
            YxMessageTip.Show(tip);
            BtnName.TrySetComponentValue("修改信息");
        }

        public void OnSureWithdraw()
        {
            var dic = new Dictionary<string, object>();

            var money = int.Parse(WithdrawCount.value);

            if (money < 100 || money % 100 != 0)
            {
                YxMessageTip.Show("最低100起提且为100的倍数");
                return;
            }
            dic["depositValue"] = YxUtiles.RecoverShowNumber(money);
            dic["depositAccountBank_a"] = BankCard.value;
            dic["depositAccount"] = BankName.value;
            dic["mobile_n"] = MobilePhone.value;
            dic["depositType"] = 1;
            dic["depositAccountType"] = 2;
            var win = CreateChildWindow(WindowName);
            win.UpdateView(dic);
        }
    }
}
