using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.View;

namespace Assets.Scripts.Common.UI
{
    public class MenuMoreView : YxBubble
    {
        public YxBaseGridAdapter Grid;

        public void AddMenu(Transform btn)
        {
            Grid.AddChild(btn);
        }

        public List<Transform> GetMenus()
        {
            return Grid.GetChildList();
        }

        protected override void OnFreshView()
        {
            ShowBubble(true);
            Grid.Reposition();
            base.OnFreshView();
        }
    }
}
