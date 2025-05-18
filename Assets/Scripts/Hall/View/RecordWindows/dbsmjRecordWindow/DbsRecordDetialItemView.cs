using Assets.Scripts.Common.Windows.TabPages;
using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using UnityEngine;
using Assets.Scripts.Hall.View.NnRecordWindow;
using YxFramwork.Common;
using Assets.Scripts.Common.Utils;
using System;

namespace Assets.Scripts.Hall.View.RecordWindows
{
    public class DbsRecordDetialItemView : YxTabItem
    {
        [Tooltip("战绩详情Grid")]
        public UIGrid DetialGrid;
        [Tooltip("战绩详情预设")]
        public DbsRecordDetialItem DetialItem;
        [Tooltip("背景图")]
        public UISprite BgSprite;
        [Tooltip("对战时间")]
        public UILabel Time;
        [Tooltip("记录ID")]
        public UILabel RoundNum;
        [Tooltip("回放开关按钮")]
        public GameObject ReplayBtnParent;
        [Tooltip("行区分标志")]
        public GameObject InterlacedBgSprite;

        public string RoundFormat = "第{0}局";
        /// <summary>
        /// 回放开关
        /// </summary>
        public static bool PlayBack;
        /// <summary>
        /// host，detailItem公用
        /// </summary>
        public static string Host;

        private UIGrid _detialGrid;
        private readonly List<UserData> _userDatas = new List<UserData>();
        #region Local Data
        /// <summary>
        /// 本条数据
        /// </summary>
        private RecordDetialItemData _curData;
        #endregion

        protected override void OnFreshView()
        {
            CheckItemData();
        }

        protected void CheckItemData()
        {
            var data = Data as RecordDetialItemData;
            if (data == null) return;
            _curData = data;
            ShowItem(Id, App.UserId);
        }

        public string ReplayUrl
        {
            get { return string.Format("{0}{1}", Host, _curData.Url); }
        }

        public void ShowItem(string roundNum, string selfId)
        {
            _curData.ShowRoundNum = roundNum;
            SetItemInfo(_curData);
        }

        /// <summary>
        /// 设置时间，局数相关信息
        /// </summary>
        /// <param name="data"></param>
        private void SetItemInfo(RecordDetialItemData data)
        {
            Time.TrySetComponentValue(data.Time);
            var roundNumString = data.ShowRoundNum;
            RoundNum.TrySetComponentValue(string.Format(RoundFormat, roundNumString));
            ReplayBtnParent.TrySetComponentValue(PlayBack);
            ReplayBtnParent.TrySetComponentValue(PlayBack);
            var heads = data.HeadDatas;
            int index = 0;
            foreach (var item in heads)
            {
                var view = DetialGrid.transform.GetChildView(index++, DetialItem);
                view.UpdateView(item.Value);
            }
            DetialGrid.repositionNow = true;
            //交错背景的显示
            SetInterlacedBg(roundNumString);
        }

        /// <summary>
        /// 设置交错背景的显示
        /// </summary>
        /// <param name="roundNumStr"></param>
        public void SetInterlacedBg(string roundNumStr)
        {
            if (InterlacedBgSprite == null) return;
            int roundNum;
            if (!int.TryParse(roundNumStr, out roundNum)) return;
            InterlacedBgSprite.SetActive(roundNum % 2 != 0);
        }

        protected Type GetItemType()
        {
            return typeof(RecordDetialItemData);
        }
        protected override void OnHide()
        {
            var list = DetialGrid.GetChildList();
            foreach (var item in list)
            {
                item.gameObject.SetActive(false);
            }
        }

        public override void Hide()
        {
            OnHide();
            gameObject.SetActive(false);
        }
    }
}