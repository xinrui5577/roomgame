using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.Utiles;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketSelfLogWindow : YxNguiWindow
    {
        public RedPacketSelfLogItem RedPacketSelfLogItem;
        public UIGrid RedPacketSelfLogGrid;
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
            Facade.Instance<TwManager>().SendAction("RedEnvelope.redEnvelopeLog", dic, UpdateView);
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
                var redPacketSelfLogData = new RedPacketSelfLogData(data);
                var item = YxWindowUtils.CreateItem(RedPacketSelfLogItem, RedPacketSelfLogGrid.transform);
                item.UpdateView(redPacketSelfLogData);
            }

            RedPacketSelfLogGrid.repositionNow = true;
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
                    var currentCount = RedPacketSelfLogGrid.transform.childCount;
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

    public class RedPacketSelfLogData
    {
        /// <summary>
        /// 数据信息
        /// </summary>
        public string Info;
        /// <summary>
        /// 变化的金币
        /// </summary>
        public int Coin;
        /// <summary>
        /// 数据建立的时间
        /// </summary>
        public string DateCreated;

        public RedPacketSelfLogData(object obj)
        {
            var dic = obj as Dictionary<string, object>;
            if (dic == null) return;
            if (dic.ContainsKey("info"))
            {
                Info = dic["info"].ToString();
            }
            if (dic.ContainsKey("coin_q"))
            {
                Coin = int.Parse(dic["coin_q"].ToString());
            }
            if (dic.ContainsKey("date_created"))
            {
                DateCreated = dic["date_created"].ToString();
            }
        }
    }
}
