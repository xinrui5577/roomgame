using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketWelfareWindow : YxNguiRedPacketWindow
    {
        public RedPacketWelfareItem RedPacketWelfareItem;
        public UITable RedPacketWelfareTable;

        protected override void OnStart()
        {
            base.OnStart();
            Facade.Instance<TwManager>().SendAction("RedEnvelope.redEnvelopeWelfare", new Dictionary<string, object>(), UpdateView, true, null, false);
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var info = Data as Dictionary<string, object>;
            if (info == null) return;
            var datas = info["data"] as List<object>;
            if(datas == null)return;
            foreach (var data in datas)
            {
                var redPacketWelfareData = new RedPacketWelfareData(data);
                var win = YxWindowUtils.CreateItem(RedPacketWelfareItem, RedPacketWelfareTable.transform);
                win.UpdateView(redPacketWelfareData);
            }

            RedPacketWelfareTable.repositionNow = true;
        }
    }

    public class RedPacketWelfareData
    {
        public int RedId;
        public int Money;
        public int Status;
        public string DateCreated;

        public RedPacketWelfareData(object obj)
        {
            var data = obj as Dictionary<string, object>;
            if (data == null) return;
            if (data.ContainsKey("id"))
            {
                RedId = int.Parse(data["id"].ToString());
            }

            if (data.ContainsKey("money_a"))
            {
                Money = int.Parse(data["money_a"].ToString());
            }

            if (data.ContainsKey("status_i"))
            {
                Status = int.Parse(data["status_i"].ToString());
            }

            if (data.ContainsKey("date_created"))
            {
                DateCreated = data["date_created"].ToString();
            }

        }
    }
}
