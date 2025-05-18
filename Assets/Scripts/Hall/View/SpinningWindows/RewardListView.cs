using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.SpinningWindows
{
    /// <summary>
    /// 奖励列表
    /// </summary>
    public class RewardListView : YxWindow
    {
        [Tooltip("奖品Grid")]
        public UIGrid ItemsGridPrefab;
        [Tooltip("奖品item预制体")]
        public RewardItemView RewardItemPrefab;
        [Tooltip("滚动条")]
        public UIScrollView ScrollView;
        [Tooltip("请求名称")]
        public string ActionName= "UserAwardInfo_yr";
        [Tooltip("奖励请求类型:0.中奖名单1.我的奖品")]
        public string AwardRange = "";
        private UIGrid _itemGrid; 

        protected override void OnEnable()
        {
            UpdateData();
        }
        public void UpdateData()
        {
            Dictionary<string, object> param=new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(AwardRange))
            {
                param.Add(AwardRange,name);
            }
            Facade.Instance<TwManager>().SendAction(ActionName, param, UpdateView);
        }

        protected override void OnFreshView()
        {
            List<object> list;
            if (Data is Dictionary<string,object>)
            {
                Dictionary<string, object> dic = (Dictionary<string, object>) Data;
                if (dic.ContainsKey("data"))
                {
                    list=dic["data"]as List<object>;
                }
                else
                {
                    list = null;
                }
            }
            else
            {
                list = Data as List<object>;
            }
            if (list == null) return;
            SpringPanel.Begin(ScrollView.gameObject, Vector3.zero, int.MaxValue);
            var count = list.Count;
            YxWindowUtils.CreateMonoParent(ItemsGridPrefab, ref _itemGrid);
            _itemGrid.onCustomSort = OnCustomSort;
            var tsParent = _itemGrid.transform;
            for (var i = 0; i < count; i++)
            {
                var obj = list[i];
                var dict = obj as Dictionary<string, object>;
                if(dict == null)continue;
                var item = YxWindowUtils.CreateItem(RewardItemPrefab, tsParent);
                var data = new RewardItemData(dict);
                item.UpdateViewWithCallBack(data, UpdateGrid);
            }
            UpdateGrid();
            if(ScrollView!=null)ScrollView.ResetPosition();
        }

        private void UpdateGrid(object msg=null)
        {
            if (_itemGrid == null) return;
            _itemGrid.repositionNow = true;
            _itemGrid.Reposition();
        }

        public int OnCustomSort(Transform ts1, Transform ts2)
        {
            var item1 = ts1.GetComponent<RewardItemView>();
            var item2 = ts2.GetComponent<RewardItemView>();
            var itemData1 = item1.GetData<RewardItemData>();
            var itemData2 = item2.GetData<RewardItemData>();
            if (itemData1.Status < itemData2.Status)
            {
                return -1;
            }
            if (itemData1.Status > itemData2.Status)
            {
                return 1;
            }
            return -System.String.CompareOrdinal(itemData1.CreateDt, itemData2.CreateDt);
        }

        public void OnConvert(RewardItemView item)
        {
            RewardItemData data = item.GetData<RewardItemData>();
            switch (data.Type)
            {
                case 0:
                    var win = CreateChildWindow("AddressWindow");
                    if (win == null) return;
                    win.UpdateViewWithCallBack(data.Id, item.UpdateBtns);
                    break;
                case 1:          
                case 2:
                    Facade.Instance<TwManager>().SendAction("userAddress_yr",
                        new Dictionary<string, object>()
                        {
                            {"id", data.Id}
                        },
                        msg =>
                        {
                            item.UpdateBtns(null);
                            ShowRewardInfo(msg);
                        });
                    break;
            }
        }

        public void OnShowInfo(RewardItemView item)
        {
            RewardItemData data = item.GetData<RewardItemData>();
            Facade.Instance<TwManager>().SendAction("userAddress_yr",
                      new Dictionary<string, object>()
                      {
                            {"id", data.Id}
                      },
                mes =>
                {
                   ShowRewardInfo(mes);
                });
        }

        private void ShowRewardInfo(object data)
        {
            var win = CreateChildWindow("RewardInfoWindow");
            if (win == null) return;
            win.UpdateViewWithCallBack(data,null);
        }

        public override YxEUIType UIType
        {
            get { return YxEUIType.Nguid; }
        }
    }
}
