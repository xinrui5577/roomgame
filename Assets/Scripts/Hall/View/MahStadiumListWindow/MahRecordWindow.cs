using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.View.MahStadiumListWindow
{
    public class MahRecordWindow : YxNguiWindow
    {
        public UITable MahRecordTable;
        public MahRecordItem MahRecordItem;
        public UIScrollView ScrollView;

        private int _totalCount;
        private int _curPageNum = 0;
        private bool _request;

        protected override void OnStart()
        {
            SendGetRecords();
            if (ScrollView != null)
            {
                ScrollView.onMomentumMove = OnDragFinished;
            }
        }

        private void SendGetRecords()
        {
            var dic = new Dictionary<string, object>();
            dic["userId"] = App.UserId;
            dic["p"] = _curPageNum++;
            Facade.Instance<TwManager>().SendAction("mahjongwm.getRecords", dic, GetTableItem);
        }

        private void GetTableItem(object data)
        {
            if (data is Dictionary<string, object>)
            {
                Dictionary<string, object> dic = (Dictionary<string, object>)data;

                _totalCount = int.Parse(dic["totalCount"].ToString());
                object obj = dic["data"];
                var gameDatas = obj as List<object>;
                if (gameDatas != null)
                {
                    foreach (var gameData in gameDatas)
                    {
                        var infoData = gameData as Dictionary<string, object>;
                        var item = YxWindowUtils.CreateItem(MahRecordItem, MahRecordTable.transform);
                        item.UpdateView(infoData);
                    }

                    MahRecordTable.repositionNow = true;
                    _request = false;
                }
            }
        }

        private void OnDragFinished()
        {
            ScrollView.UpdateScrollbars(true);
            var constraint = ScrollView.panel.CalculateConstrainOffset(ScrollView.bounds.min, ScrollView.bounds.min);
            if (constraint.y <= 1f)
            {
                if (!_request)
                {
                    SendGetRecords();
                    _request = true;
                }
            }
        }
    }
}
