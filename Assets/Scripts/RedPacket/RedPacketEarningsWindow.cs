using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using com.yxixia.utile.Utiles;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketEarningsWindow : YxNguiRedPacketWindow
    {
        public UIToggle CurReturnMoney;
        public UIToggle YesReturnMoney;
        public NguiLabelAdapter CurAllGetMoney;
        public NguiLabelAdapter CurSendGetMoney;
        public NguiLabelAdapter CurGrabGetMnoney;
        public NguiLabelAdapter TodayAllGetMoney;
        public NguiLabelAdapter TodaySendGetMoney;
        public NguiLabelAdapter TodayGrabGetMnoney;
        public RedPacketEarningsItem RedPacketEarningsItem;
        public UIGrid RedPacketEarningsGrid;
        public UIScrollView ScrollView;

        private bool _request;
        private int _curPageNum = 1;
        private int _totalCount;

        protected override void OnStart()
        {
            base.OnStart();
            if (ScrollView != null)
            {
                ScrollView.onMomentumMove = OnDragFinished;
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
                    var currentCount = RedPacketEarningsGrid.transform.childCount;
                    if (_totalCount == currentCount)
                    {
                        return;
                    }

                    if (CurReturnMoney.value)
                    {
                        RequestMyCommission();
                    }
                    else
                    {
                        Clear();
                    }
                    _request = true;
                }
            }
        }

        public void OnToggleNow()
        {
            if (CurReturnMoney.value)
            {
                Clear();
                RequestMyCommission();
            }
        }

        private void RequestMyCommission()
        {
            var dic = new Dictionary<string, object>();
            dic["p"] = _curPageNum++;
            Facade.Instance<TwManager>().SendAction("RedEnvelope.myCommission", dic, FreshEarningsView, true, null, false);
        }

        public void OnToggleYesterday()
        {
            if (YesReturnMoney.value)
            {
                Clear();
            }
        }

        private void FreshEarningsView(object obj)
        {
            var dic = obj as Dictionary<string, object>;
            if (dic == null) return;
            if (dic.ContainsKey("totalCount"))
            {
                _totalCount = int.Parse(dic["totalCount"].ToString());
            }
            if (dic.ContainsKey("allCommission") && dic.ContainsKey("sendCommission") &&
                dic.ContainsKey("grabCommission") && dic["allCommission"] != null && dic["sendCommission"] != null && dic["grabCommission"] != null)
            {
                var allCommission = int.Parse(dic["allCommission"].ToString());
                var allCommissionToday = int.Parse(dic["allCommissionToday"].ToString());
                var sendCommission = int.Parse(dic["sendCommission"].ToString());
                var sendCommissionToday = int.Parse(dic["sendCommissionToday"].ToString());
                var grabCommission = int.Parse(dic["grabCommission"].ToString());
                var grabCommissionToday = int.Parse(dic["grabCommissionToday"].ToString());

                if (CurReturnMoney.value)
                {
                    CurAllGetMoney.TrySetComponentValue(YxUtiles.GetShowNumberForm(allCommission));
                    TodayAllGetMoney.TrySetComponentValue(YxUtiles.GetShowNumberForm(allCommissionToday));

                    CurSendGetMoney.TrySetComponentValue(YxUtiles.GetShowNumberForm(sendCommission));
                    TodaySendGetMoney.TrySetComponentValue(YxUtiles.GetShowNumberForm(sendCommissionToday));

                    CurGrabGetMnoney.TrySetComponentValue(YxUtiles.GetShowNumberForm(grabCommission));
                    TodayGrabGetMnoney.TrySetComponentValue(YxUtiles.GetShowNumberForm(grabCommissionToday));
                }
            }

            var datas = dic["data"] as List<object>;
            if (datas == null) return;
            foreach (var data in datas)
            {
                var item = YxWindowUtils.CreateItem(RedPacketEarningsItem, RedPacketEarningsGrid.transform);
                var redPacketEarningsData = new RedPacketEarningsData(data);
                item.UpdateView(redPacketEarningsData);
            }
            RedPacketEarningsGrid.repositionNow = true;

            _request = false;
            if (ScrollView != null && _curPageNum == 2)
            {
                ScrollView.ResetPosition();
            }
        }

        private void Clear()
        {
            while (RedPacketEarningsGrid.transform.childCount > 0)
            {
                DestroyImmediate(RedPacketEarningsGrid.transform.GetChild(0).gameObject);
            }
        }
    }

    public class RedPacketEarningsData
    {
        public int Commission;
        public string CreateDt;
        public int Lvl;
        public int RedId;
        public int Style;//style_i 1是发红包的佣金2是抢红包发的佣金
        public int UserId;
        public string UserNick;

        public RedPacketEarningsData(object obj)
        {
            var data = obj as Dictionary<string, object>;
            if (data == null) return;
            if (data.ContainsKey("commission"))
            {
                Commission = int.Parse(data["commission"].ToString());
            }
            if (data.ContainsKey("create_dt"))
            {
                CreateDt = data["create_dt"].ToString();
            }
            if (data.ContainsKey("lvl"))
            {
                Lvl = int.Parse(data["lvl"].ToString());
            }
            if (data.ContainsKey("red_id"))
            {
                RedId = int.Parse(data["red_id"].ToString());
            }
            if (data.ContainsKey("style_i"))
            {
                Style = int.Parse(data["style_i"].ToString());
            }
            if (data.ContainsKey("user_id"))
            {
                UserId = int.Parse(data["user_id"].ToString());
            }
            if (data.ContainsKey("user_nick"))
            {
                UserNick = data["user_nick"].ToString();
            }
        }
    }
}
