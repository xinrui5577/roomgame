using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using com.yxixia.utile.Utiles;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketChatInfoWindow : YxNguiRedPacketWindow
    {
        public NguiLabelAdapter OnlineNum;
        public NguiLabelAdapter GroupName;
        public NguiLabelAdapter GroupId;
        public RedPacketChatInfoItem RedPacketChatInfoItem;
        public UIGrid RedPacketChatInfoGrid;

        protected override void OnStart()
        {
            base.OnStart();
            Facade.Instance<TwManager>().SendAction("RedEnvelope.onLineUsers", new Dictionary<string, object>(), UpdateView);
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var dic = Data as Dictionary<string, object>;
            if (dic == null) return;
            if (dic.ContainsKey("onlineNum") && dic["onlineNum"] != null)
            {
                OnlineNum.TrySetComponentValue(dic["onlineNum"].ToString());
            }
            if (dic.ContainsKey("name") && dic["name"] != null)
            {
                GroupName.TrySetComponentValue(dic["name"].ToString());
            }
            if (dic.ContainsKey("num") && dic["num"] != null)
            {
                GroupId.TrySetComponentValue(dic["num"].ToString());
            }

            var datas = dic["data"] as List<object>;
            if (datas == null) return;
            foreach (var data in datas)
            {
                var item = YxWindowUtils.CreateItem(RedPacketChatInfoItem, RedPacketChatInfoGrid.transform);
                var redPacketChatInfoData = new RedPacketChatInfoData(data);
                item.UpdateView(redPacketChatInfoData);
            }

            RedPacketChatInfoGrid.repositionNow = true;
        }
    }

    public class RedPacketChatInfoData
    {
        public int UserId;
        public string NickName;
        public string Avatar;

        public RedPacketChatInfoData(object obj)
        {
            var data = obj as Dictionary<string, object>;
            if (data == null) return;
            if (data.ContainsKey("user_id"))
            {
                UserId = int.Parse(data["user_id"].ToString());
            }
            if (data.ContainsKey("nick_m"))
            {
                NickName = data["nick_m"].ToString();
            }
            if (data.ContainsKey("avatar_x"))
            {
                Avatar = data["avatar_x"].ToString();
            }
        }
    }
}
