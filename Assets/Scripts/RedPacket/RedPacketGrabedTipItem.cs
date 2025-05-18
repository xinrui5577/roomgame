using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using YxFramwork.Framework;
using YxFramwork.Manager;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketGrabedTipItem : YxView
    {
        public NguiLabelAdapter Tip;

        public string WindowName;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var data = Data as Dictionary<string,object>;
            if(data==null)return;
            var tip = data["tip"].ToString();
            Tip.TrySetComponentValue(tip);
            name= data["redID"].ToString();
        }

        public void OnClickItem()
        {
            var win = YxWindowManager.OpenWindow(WindowName);
            win.UpdateView(int.Parse(name));
        }
    }
}
