using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using YxFramwork.Common.Utils;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.TeaLq
{
    public class TeaHouseListView : YxView
    {
        public TeaHouseListItem TeaHouseListItem;
        public UIGrid TeaHouseListGrid;

        public UIScrollView ScrollView;

        private bool _request;
        private int _curPageNum = 1;
        private int _totalCount;


        protected override void OnStart()
        {
            base.OnStart();
            Facade.EventCenter.AddEventListener<string, object>("TeaListFresh", FreshTeaList);
            SendAction();
            if (ScrollView != null)
            {
                ScrollView.onMomentumMove = OnDragFinished;
            }
        }

        private void FreshTeaList(object obj)
        {
            _curPageNum = 1;
            SendAction();
        }

        private void SendAction()
        {
            var dic = new Dictionary<string, object>();
            dic["showHead"] = 1;
            dic["type"] = 1;
            dic["p"] = _curPageNum++;
            Facade.Instance<TwManager>().SendAction("group.teaList", dic, UpdateView,true,null,false);
        }

        private void OnDragFinished()
        {
            ScrollView.UpdateScrollbars(true);
            var constraint = ScrollView.panel.CalculateConstrainOffset(ScrollView.bounds.min, ScrollView.bounds.min);
            if (constraint.y <= 1f)
            {
                if (!_request)
                {
                    var currentCount = TeaHouseListGrid.transform.childCount;
                    if (_totalCount == currentCount)
                    {
                        return;
                    }
                    SendAction();
                    _request = true;
                }
            }
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
            if (!Util.HasKey("tea_id"))
            {
                TeaHouseListItem.Toggle.startsActive = true;
            }
            foreach (var data in datas)
            {
                var info = data as Dictionary<string, object>;
                if (info == null) return;

                var teaHouseListData = new TeaHouseListData(info);
                var item = YxWindowUtils.CreateItem(TeaHouseListItem, TeaHouseListGrid.transform);
                item.UpdateView(teaHouseListData);
            }
            TeaHouseListGrid.repositionNow = true;
        }
    }

    public class TeaHouseListData
    {
        public string Avatar;
        public int Sex;
        public string TeaName;
        public int TeaId;
        public string OwnerName;


        public TeaHouseListData(Dictionary<string, object> dic)
        {
            dic.Parse("avatar", ref Avatar);
            dic.Parse("sex", ref Sex);
            dic.Parse("tea_name", ref TeaName);
            dic.Parse("tea_id", ref TeaId);
            dic.Parse("tea_owner_name", ref OwnerName);
        }
    }
}
