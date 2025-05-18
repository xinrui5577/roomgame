using System.Collections.Generic;
using Assets.Scripts.Common.Windows.TabPages;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.View.NnRecordWindow
{
    public class NnRecordWindow : YxTabPageWindow
    {
        private UIGrid _itemGrid;
        public string PrefixUpStateName;
        public string PrefixDownStateName;
        public GameObject NoDataSign;
        public NnRecordItemView ItemPrefab;
        public UIGrid ItemGridPrefab;
        public UIScrollView ScrollView;
        public string SpecifyGamekey = "";

        private int _curPageNum = 1;
        private int _curIndex;

        protected override void OnAwake()
        {
            if (!CreateTabels())
            {
                 CombatGainsController.Instance.CurGameKey = SpecifyGamekey;
                var dic = new Dictionary<string, object>();
                dic["game_key"] = "";
                dic["p"] = _curPageNum;
                Facade.Instance<TwManager>().SendAction("historyrequest", dic, UpdateViewData);
            }
            if (ScrollView != null)
            {
                ScrollView.onStoppedMoving = OnDragFinished;
            }
        }

        private void OnDragFinished()
        {
            ScrollView.UpdateScrollbars(true);
            var constraint = ScrollView.panel.CalculateConstrainOffset(ScrollView.bounds.min, ScrollView.bounds.min);
            if (constraint.y <= 1f)
            {
                var dic = new Dictionary<string, object>();
                dic["game_key"] = "";
                dic["p"] = ++_curPageNum;
                Facade.Instance<TwManager>().SendAction("historyrequest", dic, AddItems);

            }
        }

        private void AddItems(object msg)
        {
            var info = msg as Dictionary<string, object>;
            if (info == null) return;
            var data = info.ContainsKey("data") ? info["data"] : null;
            var list = data as List<object>;
            if (list == null) return;
            OnSetItems(list, _itemGrid, _curIndex);
        }

        private bool CreateTabels()
        {
            if (PerfabTableItem == null) return false;
            var dict = GameListModel.Instance.GameUnitModels;
            var gamelist = new List<GameUnitModel>();
            foreach (var keyValue in dict)
            {
                var model = keyValue.Value;
                if (model.RoomKind < 1) continue;
                gamelist.Add(model);
            }
            var count = gamelist.Count;
            TabDatas = new TabData[count];
            for (var i = 0; i < count; i++)
            {
                var gk = gamelist[i].GameKey;
                var tdata = new TabData
                {
                    UpStateName = string.Format("{0}{1}", PrefixUpStateName, gk),
                    DownStateName = string.Format("{0}{1}", PrefixDownStateName, gk),
                    Data = gk
                };
                TabDatas[i] = tdata;
            }
            if (TabDatas.Length < 1) return false;
            var td = TabDatas[0];
            td.StarttingState = true;
            UpdateView(-1);
            return true;
        }

        private void UpdateViewData(object msg)
        {
            YxWindowUtils.CreateMonoParent(ItemGridPrefab, ref _itemGrid);
            var info = msg as Dictionary<string, object>;
            if(info==null)return;
            var data = info.ContainsKey("data") ? info["data"] : null;
            var list = data as List<object>;
            if (list == null || list.Count < 1)
            {
                if (NoDataSign != null) NoDataSign.SetActive(true);
                return;
            }
            if (NoDataSign != null) NoDataSign.SetActive(false);
            OnSetItems(list, _itemGrid);
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
    }
}
