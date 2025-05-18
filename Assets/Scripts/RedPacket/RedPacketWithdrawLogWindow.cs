using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketWithdrawLogWindow : YxNguiRedPacketWindow
    {
        public RedPacketWithdrawLogItem RedPacketWithdrawLogItem;
        public UIGrid RedPacketWithdrawLogGrid;
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
            Facade.Instance<TwManager>().SendAction("RedEnvelope.depositLog", new Dictionary<string, object>(), UpdateView);
        }
        protected override void OnFreshView()
        {
            base.OnFreshView();
            var dic = Data as Dictionary<string, object>;
            if (dic == null) return;
            if (dic.ContainsKey("totalCount"))
            {
                _totalCount = int.Parse(dic["totalCount"].ToString());
            }
            var datas = dic["data"] as List<object>;
            if (datas == null) return;
            foreach (var data in datas)
            {
                var redPacketSelfLogData = new RedPacketWithdrawLogData(data);
                var item = YxWindowUtils.CreateItem(RedPacketWithdrawLogItem, RedPacketWithdrawLogGrid.transform);
                item.UpdateView(redPacketSelfLogData);
            }

            RedPacketWithdrawLogGrid.repositionNow = true;

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
                    var currentCount = RedPacketWithdrawLogGrid.transform.childCount;
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

    public class RedPacketWithdrawLogData
    {
        public int Value;
        public string StatusInfo;
        public string Desc;
        public string CreateDt;

        public RedPacketWithdrawLogData(object obj)
        {
            var data = obj as Dictionary<string, object>;
            if (data == null) return;
            if (data.ContainsKey("value"))
            {
                Value = int.Parse(data["value"].ToString());
            }
            if (data.ContainsKey("status_info"))
            {
                StatusInfo = data["status_info"].ToString();
            }
            if (data.ContainsKey("desc_x"))
            {
                Desc = data["desc_x"].ToString();
            }
            if (data.ContainsKey("create_dt"))
            {
                CreateDt = data["create_dt"].ToString();
            }
        }
    }
}
