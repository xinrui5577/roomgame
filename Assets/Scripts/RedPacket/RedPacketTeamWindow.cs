using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using com.yxixia.utile.Utiles;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketTeamWindow : YxNguiRedPacketWindow
    {
        public NguiLabelAdapter AllPeople;
        public NguiLabelAdapter AllGetMoney;
        public RedPacketTeamItem RedPacketTeamItem;
        public UIGrid RedPacketTeamGrid;
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

        private void OnDragFinished()
        {
            ScrollView.UpdateScrollbars(true);
            var constraint = ScrollView.panel.CalculateConstrainOffset(ScrollView.bounds.min, ScrollView.bounds.min);
            if (constraint.y <= 1f)
            {
                if (!_request)
                {
                    var currentCount = RedPacketTeamGrid.transform.childCount;
                    if (_totalCount == currentCount)
                    {
                        return;
                    }
                    GetTableList();
                    _request = true;
                }
            }
        }
        private void GetTableList()
        {
            var dic = new Dictionary<string, object>();
            dic["p"] = _curPageNum++;
            Facade.Instance<TwManager>().SendAction("RedEnvelope.myTeam", dic, FreshTeamView, true, null, false);

        }

        private void FreshTeamView(object obj)
        {
            var info = obj as Dictionary<string, object>;
            if (info == null) return;

            if (info.ContainsKey("totalCount"))
            {
                _totalCount = int.Parse(info["totalCount"].ToString());
            }

            if (info.ContainsKey("affNum"))
            {
                var affNum = int.Parse(info["affNum"].ToString());
                AllPeople.TrySetComponentValue(affNum);
            }

            if (info.ContainsKey("allCommission"))
            {
                if (info["allCommission"] != null)
                {
                    var allCommission = int.Parse(info["allCommission"].ToString());
                    AllGetMoney.TrySetComponentValue(YxUtiles.GetShowNumberForm(allCommission));
                }
            }

            var datas = info["data"] as List<object>;
            if (datas == null) return;
            foreach (var data in datas)
            {
                var item = YxWindowUtils.CreateItem(RedPacketTeamItem, RedPacketTeamGrid.transform);
                var redPacketTeamData = new RedPacketTeamData(data);
                item.UpdateView(redPacketTeamData);
            }

            RedPacketTeamGrid.repositionNow = true;

            _request = false;
            if (ScrollView != null && _curPageNum == 2)
            {
                ScrollView.ResetPosition();
            }
        }
    }

    public class RedPacketTeamData
    {
        public int AffId;
        public string AffNick;
        public int CommissionCur;
        public string DateCreat;
        public int UserId;
        public string UserNick;
        public int Lvl;

        public RedPacketTeamData(object obj)
        {
            var data = obj as Dictionary<string, object>;
            if (data == null) return;
            if (data.ContainsKey("aff_id"))
            {
                AffId = int.Parse(data["aff_id"].ToString());
            }
            if (data.ContainsKey("aff_nick"))
            {
                AffNick = data["aff_nick"].ToString();
            }
            if (data.ContainsKey("commission_cur"))
            {
                CommissionCur = int.Parse(data["commission_cur"].ToString());
            }
            if (data.ContainsKey("date_created"))
            {
                DateCreat = data["date_created"].ToString();
            }
            if (data.ContainsKey("user_id"))
            {
                UserId = int.Parse(data["user_id"].ToString());
            }
            if (data.ContainsKey("user_nick"))
            {
                UserNick = data["user_nick"].ToString();
            }
            if (data.ContainsKey("lvl"))
            {
                Lvl = int.Parse(data["lvl"].ToString());
            }
        }
    }
}
