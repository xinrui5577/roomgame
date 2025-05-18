using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows;
using YxFramwork.Common.Model;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketWithdrawPayPwdWindow : YxNguiRedPacketWindow
    {
        public NguiLabelAdapter WithdrawMoney;
        public NguiLabelAdapter ResidueMoney;
        public UIInput Pwd;
        public string CloseWindowName;
        public string ShowWindowName;

        private Dictionary<string, object> _pararmDic;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            _pararmDic = Data as Dictionary<string, object>;
            if (_pararmDic == null) return;
            var trueMoney = int.Parse(_pararmDic["depositValue"].ToString());
            WithdrawMoney.TrySetComponentValue(YxUtiles.GetShowNumberForm(trueMoney));
            var userInfo = UserInfoModel.Instance.UserInfo;
            var residueMoney = userInfo.CoinA - trueMoney;
            ResidueMoney.TrySetComponentValue(YxUtiles.GetShowNumberForm(residueMoney));
        }

        public void OnWithdraw()
        {
            if (Pwd.value.Length == 6)
            {
                var dic = _pararmDic;
                dic["password"] = Pwd.value;
                Facade.Instance<TwManager>().SendAction("RedEnvelope.redEnvelopeWithdraw", dic, data =>
                {
                    CloseOtherWindow(CloseWindowName);
                    ShowOtherWindow(ShowWindowName);
                    Close();
                });
            }
        }
    }
}
