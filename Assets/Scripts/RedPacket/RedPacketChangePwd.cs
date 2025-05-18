using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketChangePwd : YxNguiRedPacketWindow
    {
        public UIInput Pwd;
        public UILabel Tip;
        public string ActionName;

        private int _op;
        private bool _isSet;

        public void OnChangePwd()
        {
            var dic = new Dictionary<string, object>();
            dic["op"] = _op;
            dic["is_set"] = _isSet;
            dic["pwd"] = Pwd.value;
            Facade.Instance<TwManager>().SendAction(ActionName, dic, UpdateView);
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var data = Data as Dictionary<string, object>;
            if (data == null)
            {
                return;
            }
            var tip = data["tip"] as string;
            if (_op == 1)
            {
                Close();
                YxMessageTip.Show(tip);
                return;
            }
            if (data.ContainsKey("is_set"))
            {
                _isSet = bool.Parse(data["is_set"].ToString());
            }
            Tip.TrySetComponentValue(tip);
            Pwd.value = string.Empty;
            _op = 1;
        }
    }
}
