using Assets.Scripts.Common.Windows;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.slyz.Windows
{
    /// <summary>
    /// 中奖名单
    /// </summary>
    public class PrizeListWindow : YxNguiWindow
    {
        public UIScrollView ScrollView;
        public UIGrid GridPrefab;
        private UIGrid _curGrid;
        public PrizeListItemView PrizeItemPrefab;

        protected override void OnAwake()
        {
            base.OnAwake();
            InitStateTotal = 1;
            Facade.EventCenter.AddEventListeners<ESlyzEventType, object>(ESlyzEventType.FreshPrizeList, UpdateView);
        }

        protected override void OnShow()
        {
            gameObject.SetActive(true);
            base.OnShow();
        }

        protected override void OnFreshView()
        {
            if (!IsShow()) return;
            var gdata = App.GetGameData<SlyzGameData>();
            if (gdata == null) return;
            // 先删除上次添加的ITEM
            YxWindowUtils.CreateMonoParent(GridPrefab,ref _curGrid, ScrollView.transform);
            var gridTs = _curGrid.transform;
            // 本次添加ITEM 
            var pList = gdata.PrizeList;
            var len = pList.Count;
            for (var i = 0; i < len; i++)
            {
                var prefItem = YxWindowUtils.CreateItem(PrizeItemPrefab, gridTs);
                prefItem.UpdateView(pList[i]); 
            }

            _curGrid.repositionNow = true;
            _curGrid.Reposition();
            ScrollView.ResetPosition();
        }
    }
}
