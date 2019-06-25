/** 
 *文件名称:     RecordDetailItem.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2018-03-23 
 *描述:         回放详情Item
 *历史记录: 
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Hall.View.PageListWindow;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.RecordWindows
{
    public class RecordDetailItem : YxView
    {
        #region UI Param
        [Tooltip("头像预设")]
        public HeadItem HeadItemPrefab;
        [Tooltip("头像Grid")]
        public UIGrid HeadGrid;
        [Tooltip("背景图")]
        public UISprite BgSprite;
        [Tooltip("对战时间")]
        public UILabel Time;
        [Tooltip("记录ID")]
        public UILabel RoundNum;
        [Tooltip("回放开关按钮")]
        public GameObject ReplayBtnParent;
        #endregion
        #region Data Param
        /// <summary>
        /// host，detailItem公用
        /// </summary>
        public static string Host;
        [Tooltip("默认背景高度")]
        public int BaseHeight = 220;
        [Tooltip("局数格式")]
        public string RoundFormat = "第{0}局";
        /// <summary>
        /// 回放开关
        /// </summary>
        public static bool PlayBack;
        #endregion 
        #region Local Data
        /// <summary>
        /// 本条数据
        /// </summary>
        private RecordDetialItemData _curData;
        #endregion 
        #region Life Cycle

        protected override void OnFreshView()
        {
            CheckItemData();
        }

        protected bool CheckItemData()
        {
            var data = Data as RecordDetialItemData;
            if (data != null)
            {
                _curData = data;
                ShowItem(Id, App.UserId);
                return true;
            }
            return false;
        }

        protected override void OnHide()
        {
            var list = HeadGrid.GetChildList();
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

        #endregion
        #region Function

        public string ReplayUrl
        {
            get { return string.Format("{0}{1}", Host, _curData.Url);}
        }

        public void ShowItem(string roundNum, string selfId)
        {
            _curData.ShowRoundNum = roundNum;
            SetItemInfo(_curData);
            float moveY = HeadGrid.cellHeight * ((HeadGrid.GetChildList().Count - 1) / HeadGrid.maxPerLine);
            if (BgSprite)
            {
                BgSprite.height = (int)moveY + BaseHeight;
            }
        }
        /// <summary>
        /// 设置时间，局数相关信息
        /// </summary>
        /// <param name="data"></param>
        private void SetItemInfo(RecordDetialItemData data)
        {
            YxTools.TrySetComponentValue(Time, data.Time);
            YxTools.TrySetComponentValue(RoundNum, string.Format(RoundFormat, data.ShowRoundNum));
            YxTools.TrySetComponentValue(ReplayBtnParent, PlayBack);
            var heads = data.HeadDatas;
            int index = 0;
            foreach (var item in heads)
            {
                var view = YxTools.GetChildView(index++, HeadItemPrefab, HeadGrid.transform);
                view.UpdateView(item.Value);
            }
            HeadGrid.repositionNow = true;
        }
        #endregion

    }

    /// <summary>
    /// 回放详情基本数据
    /// </summary>
    public class RecordDetialItemData : YxData
    {
        /// <summary>
        /// key 回放id
        /// </summary>
        private const string KeyReplayId = "replay_id";
        /// <summary>
        /// key 游戏内信息
        /// </summary>
        private const string KeyHeads = "head_s";
        /// <summary>
        /// key 通用信息（目前未使用，使用了Heads的数据）
        /// </summary>
        private const string KeyInfoH = "info_h";
        /// <summary>
        /// key 时间
        /// </summary>
        private const string KeyTime = "create_dt";
        /// <summary>
        /// key 回放Url
        /// </summary>
        private const string KeyUrl = "url";
        /// <summary>
        /// 回放id
        /// </summary>
        private string _replayId;
        /// <summary>
        ///  玩家信息
        /// </summary>
        private Dictionary<string,object> _userInfoHeads;
        /// <summary>
        /// key 时间
        /// </summary>
        private string _createTime;
        /// <summary>
        /// key url
        /// </summary>
        private string _url;
        /// <summary>
        /// 显示局数
        /// </summary>
        private string _showRound;

        public string Url
        {
            get { return _url; }
        }

        public string ReplayId
        {
            get { return _replayId; }
        }

        public string Time
        {
            get { return _createTime; }
        }


        public string ShowRoundNum
        {
            get
            {
                return _showRound;
            }
            set
            {
                _showRound = value;
            }
        }

        public Dictionary<string,object> HeadDatas
        {
            get
            {
                return _userInfoHeads;
            }
        }
        public RecordDetialItemData(object data) : base(data)
        {

        }

        public RecordDetialItemData(object data, Type type) : base(data, type)
        {
        }

        protected override void ParseData(Dictionary<string, object> dic)
        {
            YxTools.TryGetValueWitheKey(dic,out _replayId, KeyReplayId);
            YxTools.TryGetValueWitheKey(dic, out _userInfoHeads, KeyHeads);
            YxTools.TryGetValueWitheKey(dic, out _createTime, KeyTime);
            YxTools.TryGetValueWitheKey(dic, out _url, KeyUrl);
        }
    }
}
