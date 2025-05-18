using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.Game.Data;
using Assets.Scripts.Game.Mahjong2D.Game.Item;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Mahjong2D.YxWindow
{
    public class HelpWindows :YxNguiWindow
    {
        [SerializeField]
        private MahjongItem _item;
        [SerializeField]
        private UIGrid _grid;
        private List<int> cards;
        protected override void OnFreshView()
        {
            base.OnFreshView();
            cards = App.GetGameData<Mahjong2DGameData>().TypeList;
            List<Transform> childList = _grid.GetChildList();
            for (int i = 0, max = childList.Count; i < max; i++)
            {
                MahjongItem item = childList[i].GetComponent<MahjongItem>();
                HelpItem HelpItem = item.gameObject.GetComponent<HelpItem>();
                if (i >= cards.Count)
                {
                    item.gameObject.SetActive(false);
                }
                else
                {
                    item.Value = cards[i];
                    item.SelfData.MahjongLayer = 100;
                    HelpItem.MahjongValueName = (EnumMahjongValue)item.Value;
                }
            }
        }

        protected override void OnHide()
        {
            base.OnHide();
        }
    }
}
