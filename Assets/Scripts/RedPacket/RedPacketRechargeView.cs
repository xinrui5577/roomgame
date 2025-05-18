using System.Collections.Generic;
using Assets.Scripts.Common;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketRechargeView : YxView
    {
        public void OnRecharge()
        {
            var cfg = App.Config as SysConfig;
            if (cfg == null) return;
            var info = LoginInfo.Instance;
            var dic = new Dictionary<string, object>();
            dic["uid"] = info.user_id;
            dic["token"] = info.token;
            dic["ctoken"] = info.ctoken;
            Facade.Instance<TwManager>().SendAction("RedEnvelope.openRecharge", dic, OpenUrl);
        }

        private void OpenUrl(object obj)
        {
            var data = obj as Dictionary<string, object>;
            if (data == null) return;
            var url = data["url"] as string;
            if (string.IsNullOrEmpty(url)) return;
            Application.OpenURL(url);
        }
    }
}
