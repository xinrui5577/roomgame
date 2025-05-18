using UnityEngine;
using Assets.Scripts.Hall.View.PageListWindow;
using System.Collections.Generic;
using Assets.Scripts.Common.Windows.TabPages;
using YxFramwork.Common.Model;
using System;
using Assets.Scripts.TeaLq;

namespace Assets.Scripts.Tea.Page
{
    public class UserUnifyRankView : YxPageListWindow
    {
        [Tooltip("key茶馆口令")]
        public string KeyGameKey = "game_key";
        [Tooltip("key茶馆口令")]
        public string KeyId = "id";
        [Tooltip("key茶馆排行榜时间口令")]
        public string KeyDay = "type";
        [Tooltip("排行榜时间 0是今天 1 是昨天")]
        public string Day;

        /// <summary>
        /// 当前数据
        /// </summary>
        private UserUnifyRankPageData _curData;
        /// <summary>
        /// 当前gamekey
        /// </summary>
        private string _curGameKey;

        protected override void DealTabsData()
        {
            base.DealTabsData();

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

        protected override void TabSelectAction(YxTabItem tableView)
        {
            _curGameKey = tableView.GetData<TabData>().Data.ToString();
            FirstRequest();
        }

        protected override Type GetItemType()
        {
            return typeof(TeaUserUnifyRankItemData);
        }

        protected override void OnActionCallBackDic()
        {
            _curData = new UserUnifyRankPageData(Data, GetItemType());
            DealPageData(_curData);
        }

        protected override void SetActionDic()
        {
            base.SetActionDic();
            int teaId = int.Parse(TeaUtil.CurTeaId);
            if (teaId == 0)
            {
                teaId = TeaMainPanel.CurTeaId;
            }
            ActionParam[KeyId] = teaId;
            ActionParam[KeyGameKey] = _curGameKey;
            if (!string.IsNullOrEmpty(Day))
            {
                ActionParam[KeyDay] = Day;
            }
        }
    }

    public class UserUnifyRankPageData : PageRequestData
    {
        public UserUnifyRankPageData(object data, Type type) : base(data, type)
        {
        }

        protected override void ParseData(Dictionary<string, object> dic)
        {
            base.ParseData(dic);
        }
    }
}