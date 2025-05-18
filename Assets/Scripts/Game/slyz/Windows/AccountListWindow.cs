using Assets.Scripts.Common.Windows;
using com.yxixia.utile.Utiles;
using YxFramwork.Common;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.slyz.Windows
{
    /// <summary>
    /// 数据统计
    /// </summary>
    public class AccountListWindow : YxNguiWindow
    {
        public UIGrid GridPrefab;
        public AccountListItemView ItemPrefab;
        public UIScrollView ScrollView;
        private UIGrid _curGrid;

        protected override void OnAwake()
        {
            base.OnAwake();
            Facade.EventCenter.AddEventListeners<ESlyzEventType,object>(ESlyzEventType.FreshAccountList, UpdateView);
        }

        protected override void OnFreshView()
        {
            if (!IsShow()) return;
            var gdata = App.GetGameData<SlyzGameData>();
            if (gdata == null) return;
            YxWindowUtils.CreateMonoParent(GridPrefab, ref _curGrid);
            var gridTs = _curGrid.gameObject.transform;
            var list = gdata.StartData.CardStatistics;
            var len = list.Count;
            for (var i = 0; i < len; i++)
            {
                var item = YxWindowUtils.CreateItem(ItemPrefab, gridTs);
                item.UpdateView(list[i]);
            }
            _curGrid.repositionNow = true; 
            if(ScrollView!=null) ScrollView.ResetPosition();
        }
    }
}

