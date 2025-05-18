using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class LianghuGrid : MonoBehaviour
    {
        public UIItemsManager ItemStore;
        public RectTransform Frame;

        private bool mFlag;

        private void Awake()
        {
            ItemStore.CloseAndHideItems = true;
        }

        public void OnReset()
        {
            ItemStore.HideItems();

            mFlag = false;
            Frame.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        public void FrameCtrl()
        {
            mFlag = !mFlag;
            Frame.gameObject.SetActive(mFlag);
        }

        public void SetHuCard(IList<int> cards)
        {
            gameObject.SetActive(true);

            FrameCtrl();
            SetGridLayout(cards.Count);
            for (int i = 0; i < cards.Count; i++)
            {
                var item = ItemStore.GetItem<UISmallCardItem>(i);
                if (item != null)
                {
                    item.SetCard(cards[i]);
                }
            }
        }

        private void SetGridLayout(int num)
        {
            var newSize = Frame.sizeDelta;
            newSize.x = 125 + num * 48;
            Frame.sizeDelta = newSize;
        }
    }
}
