using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.Views.RoomTrendView
{
    public class TrendOtherLoadView : YxView
    {
        [Tooltip("其他路的Item的父物体")]
        public Transform OtherLoad;
        [Tooltip("其他路的Item")]
        public TrendLoadItem TrendOtherLoadItem;
        [Tooltip("是否显示和")]
        public bool ShowDraw;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if(Data==null)return;
            SetOtherLoad((RoadNodeTable)Data);
        }

        /// <summary>
        /// 大路显示
        /// </summary>
        protected virtual void SetOtherLoad(RoadNodeTable roadNodeTable)
        {
            List<TrendLoadItem> loadItemList = new List<TrendLoadItem>();
            foreach (Transform child in OtherLoad.transform)
            {
                TrendLoadItem item = child.GetComponent<TrendLoadItem>();
                if (item)
                {
                    item.gameObject.SetActive(false);
                    loadItemList.Add(item);
                }
            }

            var widthX = OtherLoad.GetComponent<UIGrid>().cellWidth;
            var widthY = OtherLoad.GetComponent<UIGrid>().cellHeight;

            for (int i = 0; i < roadNodeTable.Nodes.Count; i++)
            {
                TrendLoadItem trendLoadItem;
                if (loadItemList.Count > 0 && loadItemList[0] != null)
                {
                    trendLoadItem = loadItemList[0];
                    trendLoadItem.gameObject.SetActive(false);
                    loadItemList.RemoveAt(0);
                }
                else
                {
                    trendLoadItem = YxWindowUtils.CreateItem(TrendOtherLoadItem, OtherLoad);
                }

                var recordData = roadNodeTable.Nodes[i];
                var area = recordData.IsRed ? 0 : 1;
                trendLoadItem.SetItemBg(area);
                trendLoadItem.transform.localPosition = new Vector3((recordData.X - 1) * widthX, (recordData.Y - 1) * -widthY, 0);

                var hCount = roadNodeTable.Nodes[i].DrawCount;
                if (ShowDraw&& hCount > 0)
                {
                    trendLoadItem.GetComponentInChildren<UILabel>().text = hCount.ToString();
                }
                if (i == roadNodeTable.Nodes.Count - 1)
                {
                    trendLoadItem.StartFlash();
                }
            }
        }
    }
}
