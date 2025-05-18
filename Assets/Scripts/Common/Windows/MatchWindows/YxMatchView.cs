using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.Windows.MatchWindows
{
    /// <summary>
    /// 比赛视图
    /// </summary>
    public class YxMatchView : YxView
    {
        /// <summary>
        /// 比赛Item预制体
        /// </summary>
        public YxMatchItem PrefabMatchItem;
        /// <summary>
        /// grid预制体
        /// </summary>
        public YxBaseGridAdapter PrefabGridAdapter;
        private YxBaseGridAdapter _gridAdapter;

        public string ActionName = "getMatchList";

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (Data is string) // 比赛类型
            {
                var param = new Dictionary<string, object>();
                param["type"] = Data.ToString();
                CurTwManager.SendAction(ActionName,param,UpdateView);
                return;
            }
            YxWindowUtils.CreateMonoParent(PrefabGridAdapter, ref _gridAdapter);
            //数据
            var dict = GetData<Dictionary<string,object>>();
            if (dict == null) { return;}
            List<object> list = null;
            if (dict.Parse("list", ref list))
            {
                FreshItems(list);
            }
        }

        /// <summary>
        /// 刷新items
        /// </summary>
        /// <param name="list"></param>
        private void FreshItems(List<object> list)
        {
            var count = list.Count;
            var pts = _gridAdapter.transform;
            for (var i = 0; i < count; i++)
            {
                var itemData = new YxMatchItem.MatchItemData();
                itemData.Parse(list[i] as Dictionary<string,object>);
                var item = YxWindowUtils.CreateItem(PrefabMatchItem, pts);
                item.UpdateView(itemData);
            }
            _gridAdapter.Reposition();
        }
    }
}
