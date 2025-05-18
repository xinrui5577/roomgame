using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common.Interface;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.Views.RoomTrendView
{
    public class TrendTotalInfoView : YxView
    {
        public List<string> CardTypeNames;
        public UIGrid CardTypeGrid;
        public TrendLoadItem CardTypeItem;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (Data == null) return;
            SetWinType((List<ITrendReciveData>)Data);
        }

        /// <summary>
        /// 显示最近赢牌的类型
        /// </summary>
        protected void SetWinType(List<ITrendReciveData> data)
        {
            if (!CardTypeGrid && !CardTypeItem) return;
            List<TrendLoadItem> itemList = new List<TrendLoadItem>();
            foreach (Transform child in CardTypeGrid.transform)
            {
                TrendLoadItem item = child.GetComponent<TrendLoadItem>();
                if (item)
                {
                    item.gameObject.SetActive(false);
                    itemList.Add(item);
                }
            }

            for (int i = 0; i < data.Count; i++)
            {
                TrendLoadItem trendLoadItem;
                if (itemList.Count > 0 && itemList[0] != null)
                {
                    trendLoadItem = itemList[0];
                    trendLoadItem.gameObject.SetActive(true);
                    itemList.RemoveAt(0);
                }
                else
                {
                    trendLoadItem = YxWindowUtils.CreateItem(CardTypeItem, CardTypeGrid.transform);
                }

                var recordData = data[i];
                if (recordData.GetResultType() != -1)
                {
                    int colorIndex;
                    var str = GetCardType(recordData.GetResultType(), out colorIndex);
                    trendLoadItem.SetItemContent(str, colorIndex);
                    trendLoadItem.SetItemBg(colorIndex);
                }
                
                if (i == data.Count - 1)
                {
                    trendLoadItem.StartFlash();
                }
            }

            CardTypeGrid.Reposition();
            CardTypeGrid.repositionNow = true;
            if(!gameObject.activeSelf)return;
            StartCoroutine(FreshScrollBar());
        }

        IEnumerator FreshScrollBar()
        {
            yield return new WaitForSeconds(1f);
            if (GetComponent<UIScrollBar>())
            {
                GetComponent<UIScrollBar>().value = 1;
            }
        }

        private string GetCardType(int type, out int colorIndex)
        {
            colorIndex = 1;
            if (type == 0) colorIndex = 0;
            return CardTypeNames[type];
        }
    }
}
