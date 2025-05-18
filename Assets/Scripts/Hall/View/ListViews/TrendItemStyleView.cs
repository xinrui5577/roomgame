using Assets.Scripts.Hall.Controller;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.ListViews
{
    /// <summary>
    /// 走势版桌面item
    /// </summary>
    public class TrendItemStyleView : DeskListItemStyleBaseView
    {
        /// <summary>
        /// 走势视图
        /// </summary>
        public YxView TrendView;

        public override void Init(object initData)
        {
            base.Init(initData);
            if (ItemData == null) { return; }
            HallController.Instance.SendGetGameRecordByRoomId(ItemData.Id, UpdateView);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (ItemData == null) return;
            HallController.Instance.SendGetGameRecordByRoomId(ItemData.Id, UpdateView);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ItemData = null;
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (TrendView == null) { return;}
            TrendView.Init(ItemData.Id);
            TrendView.UpdateView(Data);
        }
    }
}
