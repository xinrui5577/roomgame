using System.Collections.Generic;
using Assets.Scripts.Hall.View.BankWindows;
using UnityEngine;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Tea
{
    public class TeaLogView : YxView
    {

        public TeaLogItemView ItemPerfab;
        public UIGrid ItemGridPerfab;
        public UIScrollView ScrollView;
        public string Type;
        public string TypeValue;

        private UIGrid _curItemParent;
        private int _curPageNum = 1;
        private int _totalCount;
        private bool _request;

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
                    var currentCount = _curItemParent.transform.childCount;
                    if (_totalCount == currentCount)
                    {
                        return;
                    }
                    SendAction();
                    _request = true;
                }
            }
        }

        protected override void OnEnable()
        {
            _curPageNum = 1;
            base.OnEnable();
            if (_curItemParent != null)
            {
                Destroy(_curItemParent.gameObject);
            }
            CreateItemParent();
            SendAction();
        }

        protected void SendAction()
        {
            var dic = new Dictionary<string, object>();
            dic[Type] = TypeValue;
            dic["p"] = _curPageNum++;
            Facade.Instance<TwManager>().SendAction("group.teaSendLog", dic, UpdateView);
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            Dictionary<string, object> dic = (Dictionary<string, object>)Data;
            _totalCount = int.Parse(dic["totalCount"].ToString());
            object obj = dic["data"];
            var list = obj as List<object>;
            if (list == null) return;
         
            var count = list.Count;
            for (var i = 0; i < count; i++)
            {
                var item = Instantiate(ItemPerfab);
                var ts = item.transform;
                ts.parent = _curItemParent.transform;
                ts.localPosition = Vector3.zero;
                ts.localScale = Vector3.one;
                ts.localRotation = Quaternion.identity;
                item.gameObject.SetActive(true);
                item.UpdateView(list[i]);
            }
            _curItemParent.repositionNow = true;
            _curItemParent.Reposition();
            _request = false;
        }

        private void CreateItemParent()
        {
            var perfabTs = ItemGridPerfab.transform;
            _curItemParent = Instantiate(ItemGridPerfab);
            var ts = _curItemParent.transform;
            ts.parent = perfabTs.parent;
            ts.gameObject.SetActive(true);
            ts.localPosition = perfabTs.localPosition;
            ts.localScale = perfabTs.localScale;
            ts.localRotation = perfabTs.localRotation;
        }
    }
}
