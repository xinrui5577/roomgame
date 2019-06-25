using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.View.ShopWindows
{
    public class ShopRecords : YxNguiWindow
    {

        public UIGrid TradeGrid;
        public TradeItem TradeItem;

        void OnEnable()
        {
            OnRechargeRecord();
        }

        public void OnRechargeRecord()
        {
            Facade.Instance<TwManger>().SendAction("buyRecord", new Dictionary<string, object>(), OnFreshBuyReCord);
        }

        private void OnFreshBuyReCord(object data)
        {
            while (TradeGrid.transform.childCount > 0)
            {
                DestroyImmediate(TradeGrid.transform.GetChild(0).gameObject);
            }
            if (data == null) return;
            if (!(data is List<object>)) return;
            var recordInfos = data as List<object>;
            foreach (var recordInfo in recordInfos)
            {
                if (recordInfo == null) return;
                var recordData = (Dictionary<string, object>)recordInfo;

                var id = recordData.ContainsKey("id") ? recordData["id"].ToString() : "";
                while (id.Length < 6)
                {
                    id = id.Insert(0, "0");
                }
                var gameMoney = recordData.ContainsKey("game_money_a") ? recordData["game_money_a"].ToString() : null;
                var status = recordData.ContainsKey("status_c") ? recordData["status_c"].ToString() : null;
                var creatDt = recordData.ContainsKey("create_dt") ? recordData["create_dt"].ToString() : "";
                var obj = CreatObj(TradeItem.gameObject, TradeGrid.transform);
                obj.GetComponent<TradeItem>().InitData(id, gameMoney, status, creatDt);
            }
            TradeGrid.repositionNow = true;
            TradeGrid.Reposition();
        }

        private GameObject CreatObj(GameObject child, Transform parent)
        {
            var obj = Instantiate(child);
            obj.SetActive(true);
            obj.transform.parent = parent;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.transform.localRotation = Quaternion.identity;
            return obj;
        }
    }
}
