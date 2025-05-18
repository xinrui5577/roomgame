using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class SubPanelHuDetail : MonoBehaviour
    {
        public Text PlayerName;
        public Text Score;

        public Transform Grid;
        public HuDetailItem Prefab;
        private List<HuDetailItem> mItems = new List<HuDetailItem>();

        public void ShowDetail(MahjongResult result, int index)
        {
            gameObject.SetActive(true);
            for (int i = 0; i < mItems.Count; i++)
            {
                mItems[i].gameObject.SetActive(false);
            }

            Score.text = result.Gold.ToString();
            PlayerName.text = result.Name;

            var list = result.Deatils;
            for (int i = 0; i < list.Count; i++)
            {
                var item = GetDetailItem(i);
                item.SetData(list[i]);
            }

            SetOffset(index);
        }

        protected HuDetailItem GetDetailItem(int chair)
        {
            HuDetailItem item = null;
            if (mItems.Count - 1 < chair)
            {
                item = Instantiate(Prefab);
                item.gameObject.SetActive(true);
                item.ExSetParent(Grid);
                mItems.Add(item);
                return item;
            }
            else
            {
                item = mItems[chair];
                item.gameObject.SetActive(true);
            }
            return item;
        }

        public void OnClose()
        {
            gameObject.SetActive(false);
            for (int i = 0; i < mItems.Count; i++)
            {
                mItems[i].gameObject.SetActive(false);
            }
        }

        protected void SetOffset(int chair)
        {
            var rect = GetComponent<RectTransform>();
            var pos = rect.anchoredPosition;

            if (chair == 0)
            {
                pos.y = 190;
            }
            else
            {
                pos.y = 110 + -140 * (chair - 1);
            }
            rect.anchoredPosition = pos;
        }

        protected void OnDisable()
        {
            OnClose();
        }
    }
}