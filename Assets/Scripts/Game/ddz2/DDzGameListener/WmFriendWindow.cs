using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using UnityEngine;

namespace Assets.Scripts.Game.ddz2.DDzGameListener
{
    public class WmFriendWindow : YxNguiWindow
    {
        public WmFriendItem WmItem;

        public UIGrid ParentGrid;


        protected override void OnFreshView()
        {
            base.OnFreshView();
            var userList = (List<object>) Data;

            foreach (var userData in userList)
            {
                var item = Instantiate(WmItem);
                item.transform.parent = ParentGrid.transform;
                item.transform.localScale = Vector3.one;
                item.gameObject.SetActive(true);
                item.SetData(userData);
            }
            ParentGrid.repositionNow = true;
            ParentGrid.Reposition();
        }

       
    }
}
