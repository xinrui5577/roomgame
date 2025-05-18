using UnityEngine;
using YxFramwork.Common.Model;

namespace Assets.Scripts.Hall.View.ListViews
{
    public class DesignatedGameItem : GameListItem
    {
        [Tooltip("指定的gamekey")]
        public string DesignatedGameKey;
        [Tooltip("所属的组")]
        public int GroupType;
        protected override void OnAwake()
        {
            base.OnAwake();
            CheckIsStart = true;
            if (string.IsNullOrEmpty(DesignatedGameKey)) return;
            var list = GameListModel.Instance.Groups;
            if (GroupType > -1 && GroupType < list.Count)
            {
                var group = list[GroupType];
                var games = group.GameListModels;
                foreach (var g in games)
                {
                    if (g.GameKey != DesignatedGameKey) { continue;}
                    Model = g;
                    break;
                }
            }
        }
    }
}
