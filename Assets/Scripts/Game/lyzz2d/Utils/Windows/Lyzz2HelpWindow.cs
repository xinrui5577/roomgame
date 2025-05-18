using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using Assets.Scripts.Game.lyzz2d.Game.Item;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lyzz2d.Utils.Windows
{
    public class Lyzz2HelpWindow : YxNguiWindow
    {
        [SerializeField] private UIGrid _grid;

        [SerializeField] private MahjongItem _item;

        private List<int> cards;

        protected override void OnShow()
        {
            base.OnShow();
            cards = App.GetGameData<Lyzz2DGlobalData>().TypeList;
            var childList = _grid.GetChildList();
            for (int i = 0, max = childList.Count; i < max; i++)
            {
                var item = childList[i].GetComponent<MahjongItem>();
                var HelpItem = item.gameObject.GetComponent<HelpItem>();
                if (i >= cards.Count)
                {
                    item.gameObject.SetActive(false);
                }
                else
                {
                    item.Value = cards[i];
                    item.SelfData.MahjongLayer = 100;
                    HelpItem.MahjongValueName = (EnumMahjongValue) item.Value;
                }
            }
        }
    }
}