using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows.TabPages;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Hall.View.CombatGains
{
    /// <summary>
    /// 战绩
    /// </summary>
    public class CombatGainsWindow : YxTabPageWindow
    {
        private readonly List<GameUnitModel> _replayKeys = new List<GameUnitModel>();
        private UIGrid _itemGrid;
        private UIGrid _itemDetailGrid;

        public string PrefixUpStateName;
        public string PrefixDownStateName;
        public GameObject NoDataSign;
        public CombatGainsItemView ItemPrefab;
        public UIGrid ItemGridPrefab;
        public UIScrollView ScrollView;
        public string SpecifyGamekey = "";
        protected override void OnAwake()
        {
            if (!CreateTabels())
            {
                CombatGainsController.Instance.CurGameKey = SpecifyGamekey;
                CombatGainsController.Instance.GetList(SpecifyGamekey, _curPageNum, UpdateViewData);
            }
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
                CombatGainsController.Instance.GetList(CombatGainsController.Instance.CurGameKey, ++_curPageNum, AddItems);
		       //Debug.LogError(_curPageNum);
            } 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>是否创建tab成功</returns>
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
                var glModle = gamelist[i];
                var gk = glModle.GameKey; 
                var tdata = new TabData
                    {
                        Name = glModle.GameName,
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

        private int _curPageNum = 1;

        public override void OnTableClick(YxTabItem tableView)
        {
            var ctr = CombatGainsController.Instance;
            _curPageNum = 1;
            if (tableView == null || !tableView.GetToggle().value) return;
            var index = int.Parse(tableView.name);
            var tdata = TabDatas[index];
            var gk = tdata.Data;
            if (gk == null) return;
            var gkey = gk.ToString();
            if (string.IsNullOrEmpty(gkey)) return;
            ctr.CurGameKey = gkey;
            CombatGainsController.Instance.GetList(gkey, _curPageNum, UpdateViewData);
//            CombatGainsController.Instance.GetGameHistory(gkey,_curPageNum,UpdateViewData);
        }

        private void UpdateViewData(object msg)
        {
            YxWindowUtils.CreateItemGrid(ItemGridPrefab, ref _itemGrid);
            var list = msg as List<object>;
            if (list == null || list.Count < 1)
            {
                if (NoDataSign != null) NoDataSign.SetActive(true);
                return;
            }
            if (NoDataSign != null) NoDataSign.SetActive(false);
            OnSetItems(list, _itemGrid);
        }

        private int _curIndex;
        private void AddItems(object msg)
        {
            var list = msg as List<object>;
            if (list == null) return;
            OnSetItems(list, _itemGrid, _curIndex);
        }

        private void OnSetItems(IList<object> itemList, UIGrid grid,int startIndex = 0)
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
