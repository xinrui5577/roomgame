using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Common.Utils;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Game.SportsCarClub
{
    public class RankCtrl : MonoBehaviour
    {
        public GameObject ShowParent;
        public GameObject RankBtn;
        public UIGrid UserRankGrid;
        public RankItem UserRankItem;
        public RankItem SelfRankItem;
        public int IntervalTime=30;

        private static RankCtrl _instance;
        private int _gType;
        public static RankCtrl GetInstance()
        {
            return _instance ?? (_instance = new RankCtrl());
        }

        protected void Awake()
        {
            _instance = this;
            TimeCtrl();
        }
        /// <summary>
        /// gameinfo 的时候设置数据
        /// </summary>
        /// <param name="gType"></param>
        public void SetData(int gType)
        {
            _gType = gType;
            RankBtn.SetActive(true);
        }
        public void ShowWindow()
        {
            ShowParent.SetActive(!ShowParent.activeSelf);
        }

        public void RequestRankData()
        {
            if (IntervalTime < 30)
            {
                YxMessageBox.Show(string.Format("请{0}秒后重试", IntervalTime));
                return;
            }
            ShowWindow();
            while (UserRankGrid.transform.childCount>0)
            {
                DestroyImmediate(UserRankGrid.transform.GetChild(0).gameObject);
            }
            var dict = new Dictionary<string, object>();
            dict["gamekey"] = App.GameKey;
            dict["room_id"] = _gType;
            Facade.Instance<TwManager>().SendAction("GameDataListRequest", dict, data =>
            {
                if (data == null) return;
                var rankData = data as Dictionary<string, object>;
                if (rankData == null) return;
                var userData = rankData.ContainsKey("data") ? rankData["data"] : null;
                var self = rankData.ContainsKey("self") ? rankData["self"] : null;
                if (self == null) return;
                var selfData = self as Dictionary<string, object>;
                if (selfData == null) return;
                var selfRank = "未入榜";
                var nickName = selfData.ContainsKey("nick_m") ? selfData["nick_m"].ToString() : "";
                var userId = selfData.ContainsKey("user_id") ? selfData["user_id"].ToString() : "";
                var winner = selfData.ContainsKey("winner") ? selfData["winner"].ToString() : "";
                if (userData == null) return;
                if (userData is List<object>)
                {
                    var gameDataList = userData as List<object>;
                    var index = 0;
                    foreach (var item in gameDataList)
                    {
                        index++;
                        if (item is Dictionary<string, object>)
                        {
                            var itemData = item as Dictionary<string, object>;
                            var nickN = itemData.ContainsKey("nick_m") ? itemData["nick_m"].ToString() : "";
                            var userI = itemData.ContainsKey("user_id") ? itemData["user_id"].ToString() : "";
                            var win = itemData.ContainsKey("winner") ? itemData["winner"].ToString() : "";
                         
                            var obj=YxWindowUtils.CreateItem(UserRankItem,UserRankGrid.transform);
                            obj.SetData(index.ToString(CultureInfo.InvariantCulture), nickN, win);
                            if (nickN.Equals(nickName)) selfRank = index.ToString(CultureInfo.InvariantCulture);
                        }
                    }
                    UserRankGrid.repositionNow = true;
                }
                SelfRankItem.SetData(selfRank, nickName, winner);
                TimeCtrl();
            });
        }

        private void TimeCtrl()
        {
            InvokeRepeating("TimeChange", 0, 1);
        }
        private void TimeChange()
        {
            IntervalTime--;
            if (IntervalTime == 0)
            {
                CancelInvoke("TimeChange");
                IntervalTime = 30;
            }
        }
    }
}
