using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.bjlb.View
{
    public class TrendWindow : YxNguiWindow
    {
        public UIGrid GridPrefab;
        public UISprite ItemPrefab;

        public string DragonSpName;//43
        public string TigerSpName;//44
        public string DrawSpName;//45

        /// <summary>
        /// 排列顺序,是否是反向排序
        /// </summary>
        public bool Inverted = false;

        protected List<GameObject> _resultList = new List<GameObject>();

        private UIGrid _gridprefGrid;

        protected override void OnFreshView()
        {
            base.OnFreshView();

            YxWindowUtils.CreateMonoParent(GridPrefab, ref _gridprefGrid);
            var data = App.GetGameData<BjlGameData>().TrendConfig;
            var list = data.HistoryData;

            if (list == null)
                return;

            if (Inverted)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    CreateItem(list[i]);
                }
            }
            else
            {
                foreach (var item in list)
                {
                    CreateItem(item);
                }
            }

            _gridprefGrid.Reposition();
        }

        private void CreateItem(int trend)
        {
            var temp = Instantiate(ItemPrefab);
            var ts = temp.transform;
            var gridTs = _gridprefGrid.transform;
            ts.parent = gridTs;
            ts.localScale = Vector3.one;
            ts.localRotation = Quaternion.identity;
            if (trend > 0)//龙
            {
                temp.spriteName = DragonSpName;
            }
            else if (trend < 0)//虎
            {
                temp.spriteName = TigerSpName;
            }
            else    //和
            {
                temp.spriteName = DrawSpName;
            }
            temp.gameObject.SetActive(true);
        }
    }



    [Serializable]
    public class TrendData
    {
        public int MaxTrenCount = 15;
        public readonly List<int> HistoryData = new List<int>();
        public YxNguiWindow TrendView;

        public void AddTrend(int trend)
        {
            HistoryData.Add(trend);
            if (HistoryData.Count > MaxTrenCount)
            {
                HistoryData.RemoveAt(0);
            }
        }
    }
}
