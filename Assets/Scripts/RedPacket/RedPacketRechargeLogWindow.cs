using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.Utiles;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketRechargeLogWindow : YxNguiWindow
    {
        public RedPacketRechargeLogItem RedPacketRechargeLogItem;
        public UIGrid RedPacketRechargeLogGrid;
        public UIScrollView ScrollView;

        private bool _request;
        private int _curPageNum = 1;
        private int _totalCount;

        protected override void OnStart()
        {
            base.OnStart();
            GetTableList();

            if (ScrollView != null)
            {
                ScrollView.onMomentumMove = OnDragFinished;
            }
        }

        private void GetTableList()
        {
            var dic = new Dictionary<string, object>();
            dic["p"] = _curPageNum++;
            Facade.Instance<TwManager>().SendAction("RedEnvelope.orderLog", dic, UpdateView, true, null, false);
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var info = Data as Dictionary<string, object>;
            if (info == null) return;
            if (info.ContainsKey("totalCount"))
            {
                _totalCount = int.Parse(info["totalCount"].ToString());
            }
            var datas = info["data"] as List<object>;
            if (datas == null) return;
            foreach (var data in datas)
            {
                var redPacketSelfLogData = new RedPacketRechargeLogData(data);
                var item = YxWindowUtils.CreateItem(RedPacketRechargeLogItem, RedPacketRechargeLogGrid.transform);
                item.UpdateView(redPacketSelfLogData);
            }

            RedPacketRechargeLogGrid.repositionNow = true;
            _request = false;
            if (ScrollView != null && _curPageNum == 2)
            {
                ScrollView.ResetPosition();
            }
        }
        private void OnDragFinished()
        {
            ScrollView.UpdateScrollbars(true);
            var constraint = ScrollView.panel.CalculateConstrainOffset(ScrollView.bounds.min, ScrollView.bounds.min);
            if (constraint.y <= 1f)
            {
                if (!_request)
                {
                    var currentCount = RedPacketRechargeLogGrid.transform.childCount;
                    if (_totalCount == currentCount)
                    {
                        return;
                    }
                    GetTableList();
                    _request = true;
                }
            }
        }
    }

    public class RedPacketRechargeLogData
    {
        public string Info;
        public double Money;
        public string PaymentDt;


        public RedPacketRechargeLogData(object obj)
        {
            var dic = obj as Dictionary<string, object>;
            if (dic == null) return;
            if (dic.ContainsKey("info"))
            {
                Info = dic["info"].ToString();
            }
            if (dic.ContainsKey("money_a"))
            {
                Money = double.Parse(dic["money_a"].ToString());
            }
            if (dic.ContainsKey("payment_dt"))
            {
                PaymentDt = dic["payment_dt"].ToString();
            }
        }
    }
}
