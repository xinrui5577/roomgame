using Assets.Scripts.Common.Windows;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.slyz.Windows
{
    /// <summary>
    /// 好牌记录
    /// </summary>
    public class RecordWindow : YxNguiWindow
    {
        public RecordItemView ItemPrefab;
        public UIScrollView ScrollView;
        /// <summary>
        /// 列表
        /// </summary>
        public UIGrid Grid;

        // Use this for initialization
        private UIGrid _itemGridParent;

        protected override void OnAwake()
        {
            base.OnAwake();
            Facade.EventCenter.AddEventListeners<ESlyzEventType, object>(ESlyzEventType.FreshRecord,UpdateView);
        }

        protected override void OnShow()
        {
            gameObject.SetActive(true);
            base.OnShow();
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (!IsShow()) return;
            var gdata = App.GetGameData<SlyzGameData>();
            var cardRecord = gdata.StartData.CardRecord;
            YxWindowUtils.CreateMonoParent(Grid, ref _itemGridParent, ScrollView.transform);
            var gridTs = _itemGridParent.transform;
            var len = cardRecord.Count;
            for (var i = 0; i < len; i++)
            {
                var record = cardRecord[i];
                var item = YxWindowUtils.CreateItem(ItemPrefab, gridTs);
                item.UpdateView(record);
            }
            _itemGridParent.repositionNow = true;
            _itemGridParent.Reposition();
        }
    }
}
