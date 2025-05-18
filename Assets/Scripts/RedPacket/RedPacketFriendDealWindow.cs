using System.Collections.Generic;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketFriendDealWindow : YxNguiRedPacketWindow
    {
        public UIInput OtherId;


        public void OnFriendRequest()
        {
            var dic = new Dictionary<string, object>();
            dic["other_id"] = OtherId.value;
            dic["option"] = 1;
            Facade.Instance<TwManager>().SendAction("RedEnvelope.opFriend", dic, UpdateView);
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
        }
    }
}
