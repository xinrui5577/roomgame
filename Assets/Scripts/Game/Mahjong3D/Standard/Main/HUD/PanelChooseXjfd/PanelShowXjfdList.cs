using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [UIPanelData(typeof(PanelShowXjfdList), UIPanelhierarchy.Popup)]

    public class PanelShowXjfdList : UIPanelBase, IUIPanelControl<XjfdListArgs>
    {
        public ShowXjfdListItem ItemPrefab;
        public GridLayoutGroup Grid;
        public RectTransform Content;
        public Sprite[] CardsSprite;

        private List<ShowXjfdListItem> mItems = new List<ShowXjfdListItem>();

        private void OnDisable()
        {
            for (int i = 0; i < mItems.Count; i++)
            {
                mItems[i].gameObject.SetActive(false);
            }
        }

        public void Open(XjfdListArgs args)
        {
            Open();
            ShowXjfdListItem item = null;
            var list = args.XjfdList;
            int max = 0;
            if (list == null || list.Count <= 0)
            {
                Close();
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                if (i < mItems.Count)
                {
                    item = mItems[i];
                }
                else
                {
                    item = CreateItem();
                }
                item.gameObject.SetActive(true);
                item.SetData(list[i]);
                max = list[i].Length > max ? list[i].Length : max;
            }
            SetGridSize(max, list.Count);
        }

        public void OnCancelClick()
        {
            GameCenter.Hud.GetPanel<PanelOpreateMenu>().Open();
            var mahHand = GameCenter.Scene.MahjongGroups.PlayerHand;
            var ccMahHand = mahHand.GetComponent<CcmjMahjongPlayerHand>();
            ccMahHand.ResetPlayerHandMahjong(true);
            Close();
        }

        private void SetGridSize(int max, int num)
        {
            if (max == 0)
            {
                Close();
                return;
            }

            switch (max)
            {
                case 1:
                    Grid.cellSize = new Vector2(120, 115);
                    SetGridData(num, 4);
                    break;
                case 3:
                    Grid.cellSize = new Vector2(255, 115);
                    SetGridData(num, 3);
                    break;
                case 4:
                    Grid.cellSize = new Vector2(350, 115);
                    SetGridData(num, 2);
                    break;
            }
        }

        private void SetGridData(int num, int maxNum)
        {
            int n = num % 2 != 0 ? num / 2 + 1 : num / 2;
            n = n >= 2 ? 2 : n;
            int m = num <= maxNum ? num : maxNum;
            float high = Grid.cellSize.y * n + Grid.spacing.y * (n - 1) + 85;
            float width = Grid.cellSize.x * m + Grid.spacing.x * (m - 1) + 75;
            Grid.constraintCount = m;
            Content.sizeDelta = new Vector2(width, high);
        }

        private ShowXjfdListItem CreateItem()
        {
            var obj = Instantiate(ItemPrefab);
            obj.transform.ExSetParent(Grid.transform);
            obj.gameObject.SetActive(false);
            mItems.Add(obj);
            return obj;
        }
    }
}