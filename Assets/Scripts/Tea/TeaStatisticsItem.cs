using Assets.Scripts.Common.Utils;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Tea
{
    public class TeaStatisticsItem : YxView
    {
        [Tooltip("玩家的ID")]
        public UILabel UserId;
        [Tooltip("玩家的昵称")]
        public UILabel UserName;
        [Tooltip("统计的单独Item")]
        public TeaStatisticsSingleItem TeaStatisticsSingleItem;
        [Tooltip("统计的单独Item的Grid")]
        public UIGrid TeaStatisticsGrid;
        [Tooltip("统计总数")]
        public UILabel Total;
        [Tooltip("统计显示")]
        public string TotalFormat;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var satisticsData = Data as SatisticsData;
            if (satisticsData == null) return;
            UserId.TrySetComponentValue(satisticsData.UserId.ToString());
            UserName.TrySetComponentValue(satisticsData.UserName);
            Total.TrySetComponentValue(string.Format("{0}{1}", satisticsData.Total, TotalFormat));
            if(satisticsData.GameList==null)return;
            foreach (var info in satisticsData.GameList)
            {
                TeaStatisticsSingleItem item = YxWindowUtils.CreateItem(TeaStatisticsSingleItem, TeaStatisticsGrid.transform);
                item.UpdateView(info);
            }
            TeaStatisticsGrid.Reposition();
        }
    }
}
