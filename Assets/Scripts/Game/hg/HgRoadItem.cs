using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using UnityEngine;

namespace Assets.Scripts.Game.hg
{
    public class HgRoadItem : MonoBehaviour
    {
        public UISprite Item;
        public UIGrid ItemGrid;
        public bool ShowWinOrLose;

        public void CreatItem(List<string> winAreas)
        {
            for (int i = 0; i < winAreas.Count; i++)
            {
                var item = YxWindowUtils.CreateItem(Item, ItemGrid.transform);
                if (ShowWinOrLose)
                {
                    item.spriteName = winAreas[i].Equals("") ? "dishLose" : "dishWin";
                }
                else
                {
                    item.spriteName = string.Format("dish{0}", winAreas[i]);
                }
            }

            ItemGrid.repositionNow = true;
        }
    }
}
