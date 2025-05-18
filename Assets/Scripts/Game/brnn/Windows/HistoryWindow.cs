using Assets.Scripts.Common.Windows;
using com.yxixia.utile.Utiles;
using YxFramwork.Common;

namespace Assets.Scripts.Game.brnn.Windows
{
    public class HistoryWindow : YxNguiWindow
    {
        public SetResultList HistoryItemPrefab;
        public UIGrid GridPrefab;
        protected UIGrid Grid;
        /// <summary>
        /// 排列顺序,是否是反向排序
        /// </summary>
        public bool Inverted = false;

        protected override void OnFreshView()
        {
            YxWindowUtils.CreateMonoParent(GridPrefab, ref Grid);
            var resultCtrl = App.GetGameManager<BrnnGameManager>().ResultListCtrl;
            var list = resultCtrl.HistoryData;

            if (list == null)
                return;

            if(Inverted)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    CreateItem(list[i]);
                }
            }
            else
            {
                foreach (var data in list)
                {
                    CreateItem(data);
                }
            }

            Grid.repositionNow = true;
            Grid.Reposition();
        }

        protected void CreateItem(bool[] data)
        {
            var temp = YxWindowUtils.CreateItem(HistoryItemPrefab, Grid.transform);
            temp.UpdateView(data);
        }
    }
}
