using System;
using Assets.Scripts.Common.Windows.TabPages;
using YxFramwork.Common.Model;
using System.Collections.Generic;
using Assets.Scripts.Hall.View.PageListWindow;
using UnityEngine;

namespace Assets.Scripts.Hall.View.RecordWindows
{
    public class DbsTotalRecordWindow : YxPageListWindow
    {
        /// <summary>
        /// Key gamekey
        /// </summary>
        private const string KeyGameKey = "game_key";

        [Tooltip("详情界面")]
        public DbsRecordDetialWindow DetailWindow;

        [Tooltip("请求带GameKey")]
        public bool ActionWithGameKey = true;

        /// <summary>
        /// 当前gamekey
        /// </summary>
        private string _curGameKey;
        /// <summary>
        /// 当前数据
        /// </summary>
        private TotalRecordData _curData;
        protected override void TabSelectAction(YxTabItem tableView)
        {
            _curGameKey = tableView.GetData<TabData>().Data.ToString();
            FirstRequest();
        }

        protected override void SetActionDic()
        {
            base.SetActionDic();
            if (ActionWithGameKey)
            {
                ActionParam[KeyGameKey] = _curGameKey;
            }

        }

        protected override Type GetItemType()
        {
            return typeof(TotalRecordItemData);
        }

        protected override void DealTabsData()
        {
            var dict = GameListModel.Instance.GameUnitModels;
            var gamelist = new List<GameUnitModel>();
            foreach (var keyValue in dict)
            {
                var model = keyValue.Value;
                if (model.RoomKind < 1) continue;
                gamelist.Add(model);
            }
            var count = gamelist.Count;
            TabDatas = new TabData[count];
            for (var i = 0; i < count; i++)
            {
                var gk = gamelist[i].GameKey;
                var tdata = new TabData
                {
                    Name = gamelist[i].GameName,
                    Data = gk
                };
                TabDatas[i] = tdata;
            }
            if (TabDatas.Length < 1) return;
            var td = TabDatas[0];
            td.StarttingState = true;
        }
        protected override void OnActionCallBackDic()
        {
            _curData = new TotalRecordData(Data, GetItemType());
            TotalRecordItem.PlayBack = _curData.PlayBack;
            DealPageData(_curData);
        }
        public virtual void ShowDetailWindow(TotalRecordItem item)
        {
            DetailWindow.TabActionName = item.DetailFunc;
            DetailWindow.RequestOnAwake = false;
            DetailWindow.SendFirstAction(item.CurGameKey, item.CurRoomId, item.DetailPlayBack);
        }

        protected override void HideItem(Transform item)
        {
            base.HideItem(item);
            TotalRecordItem recordItem = item.GetComponent<TotalRecordItem>();
            if (recordItem)
            {

            }
        }
    }
}