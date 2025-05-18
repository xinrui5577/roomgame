using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Tea
{
    public class TeaListCtrl : MonoBehaviour
    {
        public TweenPosition CurrentObj;

        public BoxCollider CurrentBg;

        public UIScrollView ScrollView;

        public UIGrid TeaListGrid;

        public TeaListItem ItemPrefab;

        private int _totalCount;
        private int _curPageNum = 1;
        private int _curIndex;

        protected void Awake()
        {
//            RequestGroupList();
        }

        public void RequestGroupList()
        {
            var dic = new Dictionary<string, object>();
            dic["p"] = 1;
            Facade.Instance<TwManager>().SendAction("group.getGroupList", dic, UpdateViewData);

            if (ScrollView != null)
            {
                ScrollView.onStoppedMoving = OnDragFinished;
            }
        }

        private void UpdateViewData(object msg)
        {
            var info = msg as Dictionary<string, object>;
            if (info == null) return;
            var data = info.ContainsKey("data") ? info["data"] : null;
            var list = data as List<object>;
            if (list == null || list.Count < 1)
            {
                return;
            }
            OnSetItems(list, TeaListGrid);
        }
        private void OnSetItems(IList<object> itemList, UIGrid grid, int startIndex = 0)
        {
            var count = itemList.Count;
            _curIndex = startIndex;
            for (var i = 0; i < count; i++)
            {
                var obj = itemList[i];
                if (obj == null) continue;
                var item = YxWindowUtils.CreateItem(ItemPrefab, grid.transform);
                item.Id = (_curIndex + 1).ToString();
                item.UpdateView(obj);
                _curIndex++;
            }
            grid.repositionNow = true;
            grid.Reposition();
        }

        private void OnDragFinished()
        {
            if(TeaListGrid.transform.childCount== _totalCount&& _totalCount!=-1)return;
            ScrollView.UpdateScrollbars(true);
            var constraint = ScrollView.panel.CalculateConstrainOffset(ScrollView.bounds.min, ScrollView.bounds.min);
            if (constraint.y <= 1f)
            {
                var dic = new Dictionary<string, object>();
                dic["p"] = ++_curPageNum;
                Facade.Instance<TwManager>().SendAction("group.getGroupList", dic, AddItems);
            }
        }


        private void AddItems(object msg)
        {
            var info = msg as Dictionary<string, object>;
            if (info == null) return;
            _totalCount = info.ContainsKey("totalCount") ? int.Parse(info["totalCount"].ToString()) : -1;   
            var data = info.ContainsKey("data") ? info["data"] : null;
            var list = data as List<object>;
            if (list == null) return;
            OnSetItems(list, TeaListGrid, _curIndex);
        }


        public void OnOpen()
        {
            RequestGroupList();
            CurrentBg.enabled = true;
            CurrentObj.enabled = true;
            CurrentObj.PlayForward();
            CurrentObj.onFinished.Clear();
        }

        public void OnClose()
        {
          
            CurrentBg.enabled = false;
            CurrentObj.PlayReverse();
            CurrentObj.onFinished.Add(new EventDelegate(() =>
            {
                TeaListGrid.transform.DestroyChildren();
            }));
        }

    }
}
