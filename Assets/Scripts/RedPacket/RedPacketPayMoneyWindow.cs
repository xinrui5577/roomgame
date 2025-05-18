using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using YxFramwork.Common.Model;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketPayMoneyWindow : YxNguiRedPacketWindow
    {
        public NguiLabelAdapter GiveMoney;
        public NguiLabelAdapter ResidueMoney;
        public NguiLabelAdapter PwdCode;
        public string MoneyKey;
        public string ActionName;
        public string CloseWindow;
        public string ShowWindow;


        private Dictionary<string, object> _dic;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            _dic = Data as Dictionary<string, object>;
            if (_dic == null) return;
            var money = long.Parse(_dic[MoneyKey].ToString());
            GiveMoney.TrySetComponentValue(YxUtiles.GetShowNumberForm(money));
            var userInfo = UserInfoModel.Instance.UserInfo;
            var residueMoney = userInfo.CoinA - money;
            ResidueMoney.TrySetComponentValue(YxUtiles.GetShowNumberForm(residueMoney));
        }

        public void OnSendAction()
        {
            if (PwdCode.Value.Length == 6)
            {
                _dic["pwd"] = PwdCode.Value;
                Facade.Instance<TwManager>().SendAction(ActionName, _dic, (obj) =>
                 {
                     Close();
                     CloseOtherWindow(CloseWindow);
                     ShowOtherWindow(ShowWindow);
                 });
            }
        }
    }
}
