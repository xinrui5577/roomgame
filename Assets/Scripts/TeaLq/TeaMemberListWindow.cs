using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.TeaLq
{
    public class TeaMemberListWindow : YxNguiWindow
    {
        public string SendActionKey;

        public UITable TeaMemberListTable;
        public TeaMemberListItem TeaMemberListItem;
        public UIScrollView ScrollView;
        public string Type;

        private bool _request;
        private int _curPageNum = 1;
        private int _totalCount;

        public void OnSelect(UIToggle toggle)
        {
            if (toggle.value)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (ScrollView != null)
            {
                ScrollView.panel.clipOffset = Vector2.zero;
                ScrollView.transform.localPosition = new Vector3(ScrollView.transform.localPosition.x, 0, ScrollView.transform.localPosition.z);
            }
            _request = false;
        }

        protected override void OnShow()
        {
            base.OnShow();
            _curPageNum = 1;
            ClearTable();
            SendAction();

            if (ScrollView != null)
            {
                ScrollView.onMomentumMove = OnDragFinished;
            }
        }

        private void SendAction()
        {
            var dic = new Dictionary<string, object>();
            dic["p"] = _curPageNum++;
            dic["id"] = TeaMainPanel.CurTeaId;
            if (!string.IsNullOrEmpty(Type))
            {
                dic["type"] = Type;
            }
            Facade.Instance<TwManager>().SendAction(SendActionKey, dic, UpdateView);
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
            _request = false;
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

        private void ClearTable()
        {
            while (TeaMemberListTable.transform.childCount > 0)
            {
                DestroyImmediate(TeaMemberListTable.transform.GetChild(0).gameObject);
            }
        }

        public void OnTeaHouseAgree(TeaMemberListItem item)
        {
            var dic = new Dictionary<string, object>();
            dic["status"] = 1;
            dic["user_id"] = item.UserId.Value;
            dic["id"] = TeaMainPanel.CurTeaId;
            Facade.Instance<TwManager>().SendAction("group.teaHouseAudit", dic, data =>
            {
                DestroyImmediate(item.gameObject);
                TeaMemberListTable.repositionNow = true;
            }, true, null, false);
        }

        public void OnTeaHouseRefuse(TeaMemberListItem item)
        {
            var dic = new Dictionary<string, object>();
            dic["status"] = 2;
            dic["user_id"] = item.UserId.Value;
            dic["id"] = TeaMainPanel.CurTeaId;
            Facade.Instance<TwManager>().SendAction("group.teaHouseAudit", dic, data =>
            {
                DestroyImmediate(item.gameObject);
                TeaMemberListTable.repositionNow = true;
            }, true, null, false);
        }

        public void OnDeleteMember(TeaMemberListItem item)
        {
            var dic = new Dictionary<string, object>();
            dic["type"] = 0;
            dic["user_id"] = item.UserId.Value;
            dic["id"] = TeaMainPanel.CurTeaId;
            Facade.Instance<TwManager>().SendAction("group.addAssistant", dic, data =>
            {
                var obj = data as Dictionary<string, object>;
                if (obj == null) return;
                var info = obj["info"].ToString();
                YxMessageBox.Show(info);
                DestroyImmediate(item.gameObject);
                TeaMemberListTable.repositionNow = true;
            }, true, null, false);
        }

        public void OnForbidGame(TeaMemberListItem item, string type)
        {
            var dic = new Dictionary<string, object>();
            dic["user_id"] = item.UserId.Value;
            dic["id"] = TeaMainPanel.CurTeaId;
            dic["type"] = type;
            item.OnBtnChange();
            Facade.Instance<TwManager>().SendAction("group.forbidGame", dic, null, true, null, false);
        }
    }

    public class TeaMemberListData
    {
        public string NickName;
        public int UserId;
        public int Sex;
        public string Avatar;
        public string LastLoginDt;
        public int GameRound;
        public bool Isforbid;
        public Detail Detail;
        public bool IsShowBtn;

        public TeaMemberListData(Dictionary<string, object> dic)
        {
            dic.Parse("nickname", ref NickName);
            dic.Parse("user_id", ref UserId);
            dic.Parse("sex", ref Sex);
            dic.Parse("avatar", ref Avatar);
            dic.Parse("last_login_dt", ref LastLoginDt);
            dic.Parse("game_r", ref GameRound);
            int forbid = 0;
            if (dic.Parse("forbid_g", ref forbid))
            {
                Isforbid = forbid == 1;
            }
            if (dic.ContainsKey("detail"))
            {
                Detail = new Detail(dic["detail"]); 
            }
            dic.Parse("is_show", ref IsShowBtn);
        }
    }
    public class Detail
    {
        public List<string> Descs=new List<string>();
        public Detail(object data)
        {
            var lists = data as List<object>;
            if(lists == null)return;
            foreach (var list in lists)
            {
                var dic = list as Dictionary<string, object>;
                if(dic==null)return;
                string desc="";
                dic.Parse("desc_x", ref desc);
                Descs.Add(desc);
            }
        }
    }
}
