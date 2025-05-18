using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketSendMoneyLogWindow : YxNguiRedPacketWindow
    {
        public RedPacketSendMoneyLogItem RedPacketSendMoneyLogItem;
        public UIGrid RedPacketSendMoneyLogGrid;
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
            Facade.Instance<TwManager>().SendAction("RedEnvelope.sendMoneyLog", dic, UpdateView, true, null, false);
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
                var redPacketSelfLogData = new RedPacketSendMoneyLogData(data);
                var item = YxWindowUtils.CreateItem(RedPacketSendMoneyLogItem, RedPacketSendMoneyLogGrid.transform);
                item.UpdateView(redPacketSelfLogData);
            }

            RedPacketSendMoneyLogGrid.repositionNow = true;
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
                    var currentCount = RedPacketSendMoneyLogGrid.transform.childCount;
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

    public class RedPacketSendMoneyLogData
    {
        public int CoinNum;
        public string CreateDt;
        public string Desc;

        public RedPacketSendMoneyLogData(object obj)
        {
            var dic = obj as Dictionary<string, object>;
            if (dic == null) return;
            if (dic.ContainsKey("coin_num_a"))
            {
                CoinNum = int.Parse(dic["coin_num_a"].ToString());
            }
            if (dic.ContainsKey("create_dt"))
            {
                CreateDt = dic["create_dt"].ToString();
            }
            if (dic.ContainsKey("desc_x"))
            {
                Desc = dic["desc_x"].ToString();
            }
        }
    }
}
