using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketSpreadWindow : YxNguiRedPacketWindow
    {
        public YxBaseTextureAdapter PromotionTex;

        protected override void OnStart()
        {
            base.OnStart();
            Facade.Instance<TwManager>().SendAction("RedEnvelope.postQEcode", new Dictionary<string, object>(), UpdateView, true, null, false);
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var data = Data as Dictionary<string, object>;
            if (data == null) return;
            var qurl = data["QEurl"] as string;
            AsyncImage.Instance.GetAsyncImage(qurl, (texture, s) =>
            {
                PromotionTex.TrySetComponentValue(texture);
            });
        }
    }
}
