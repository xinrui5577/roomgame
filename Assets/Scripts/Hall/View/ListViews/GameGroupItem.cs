using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.View.YxListViews;

namespace Assets.Scripts.Hall.View.ListViews
{
    public class GameGroupItem : YxBaseListItem
    {
        public int[] DataFormat;

        /// <summary>
        /// 组的类型
        /// </summary>
        /// <param name="groupTypeStr"></param>
        public void OnGameGroupClick(string groupTypeStr)
        {
            int groupType;
            if (!int.TryParse(groupTypeStr, out groupType)) return;
            var index = GameListModel.Instance.GetGroupIndexByType(groupType);
            GameListController.Instance.ShowGameList(index, DataFormat);
        }

        /// <summary>
        /// 组的类型
        /// </summary>
        /// <param name="toggle"></param>
        public void OnGameGroupToggle(YxBaseToggleAdapter toggle)
        {
            if (!toggle.Value) return;
            var groupTypeStr = toggle.name;
            int groupType;
            if (!int.TryParse(groupTypeStr, out groupType)) return;
            var index = GameListModel.Instance.GetGroupIndexByType(groupType);
            GameListController.Instance.ShowGameList(index, DataFormat);
        }

        protected override void OnFreshView()
        {
        }

        protected override void FreshData()
        {
        }

        public override void SetColor(Color color)
        {
        }

        public override void AwakAction(bool isAction)
        {
        }

        protected override void OnMoveAction(bool isMove)
        {
        }
    }
}
