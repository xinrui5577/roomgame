using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketMyGameWindow : YxNguiRedPacketWindow
    {

        public void OnClickOpen()
        {
            Facade.Instance<TwManager>().SendAction("RedEnvelope.openUrl", new Dictionary<string, object>(),
                (data) =>
                {
                    var dic = data as Dictionary<string, object>;
                    if (dic == null) return;
                    if (dic.ContainsKey("url"))
                    {
                        var url = dic["url"].ToString();
                        Application.OpenURL(url);
                    }
                    else
                    {
                        YxMessageTip.Show("暂未开放 敬请期待");
                    }
                });
        }
    }
}
