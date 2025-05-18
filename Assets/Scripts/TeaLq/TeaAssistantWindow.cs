using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.Utiles;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.TeaLq
{
    public class TeaAssistantWindow : YxNguiWindow
    {
        public UITable TeaMemberListTable;
        public TeaMemberListItem TeaMemberListItem;
        public UIScrollView ScrollView;

        private bool _request;
        private int _curPageNum = 1;
        private int _totalCount;


        protected override void OnStart()
        {
            base.OnStart();
            _curPageNum = 1;
            ClearTable();
            SendAction();

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
                    var currentCount = TeaMemberListTable.transform.childCount;
                    if (_totalCount == currentCount)
                    {
                        return;
                    }
                    SendAction();
                    _request = true;
                }
            }
        }

        private void SendAction()
        {
            var dic = new Dictionary<string, object>();
            dic["p"] = _curPageNum++;
            dic["assistant"] = 1;
            dic["id"] = TeaMainPanel.CurTeaId;
            Facade.Instance<TwManager>().SendAction("group.teaHouseMember", dic, UpdateView);
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var data = Data as Dictionary<string, object>;
            if (data == null) return;
            if (data.ContainsKey("totalCount"))
            {
                _totalCount = int.Parse(data["totalCount"].ToString());
            }
            var lists = data["data"] as List<object>;
            if (lists == null) return;

            foreach (var list in lists)
            {
                var info = list as Dictionary<string, object>;
                if (info == null) return;
                var teaMemberListData = new TeaMemberListData(info);
                var item = YxWindowUtils.CreateItem(TeaMemberListItem, TeaMemberListTable.transform);
                item.UpdateView(teaMemberListData);
            }
            TeaMemberListTable.repositionNow = true;
        }

        private void ClearTable()
        {
            while (TeaMemberListTable.transform.childCount > 0)
            {
                DestroyImmediate(TeaMemberListTable.transform.GetChild(0).gameObject);
            }

        }

        public void OnAddMember(TeaMemberListItem item)
        {
            var dic = new Dictionary<string, object>();
            dic["type"] = 1;
            dic["user_id"] = item.UserId.Value;
            dic["id"] = TeaMainPanel.CurTeaId;
            Facade.Instance<TwManager>().SendAction("group.addAssistant", dic, data =>
            {
                var obj = data as Dictionary<string, object>;
                if(obj ==null)return;
                var info = obj["info"].ToString();
                YxMessageBox.Show(info);
                DestroyImmediate(item.gameObject);
                TeaMemberListTable.repositionNow = true;
            }, true, null, false);
        }
    }
}
