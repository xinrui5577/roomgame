using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Hall.View.PageListWindow;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Tool;

/*===================================================
 *文件名称:     TeaMemberInfoItem.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-10-12
 *描述:        	茶馆成员信息Item
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Tea.Page
{
    public class TeaMemberInfoItem : YxPageItem 
    {
        #region UI Param
        [Tooltip("玩家Id")]
        public YxBaseLabelAdapter UserIdLabel;
        [Tooltip("玩家姓名")]
        public YxBaseLabelAdapter UserNameLabel;
        [Tooltip("加入茶馆时间")]
        public YxBaseLabelAdapter JoinTimeLabel;
        [Tooltip("申请加入茶馆时间")]
        public YxBaseLabelAdapter ApplyTimeLabel;
        [Tooltip("玩家金币")]
        public YxBaseLabelAdapter UserCoinLabel;
        [Tooltip("玩家在线状态")]
        public YxBaseLabelAdapter OnLineStatusLabel;

        #endregion

        #region Data Param
        [Tooltip("玩家货币格式")]
        public string UserCoinFormat = "{0}";
        /// <summary>
        /// 加入状态
        /// </summary>
        public bool JoinState { get; private set; }
        /// <summary>
        /// 授权开房状态
        /// </summary>
        public bool PersonCtrlStatus
        {
            get {return  TeaUtil.CurPersonCtrlStatus == 1; }
        }

        /// <summary>
        /// 创建房间状态
        /// </summary>
        public bool CreateStatus { get; private set; }

        #endregion

        #region Local Data

        /// <summary>
        /// 当前成员加入茶馆状态
        /// </summary>
        private bool _joinState;

        #endregion

        #region Life Cycle


        public override Type GetDataType()
        {
            return typeof(TeaMemberInfoItemData);
        }

        protected override void OnItemFresh()
        {
            base.OnItemFresh();
            var itemData = GetData<TeaMemberInfoItemData>();
            if(itemData!=null)
            {
                CreateStatus = itemData.AllowCreateRoom == 1;
                UserIdLabel.TrySetComponentValue(itemData.UserId);
                UserNameLabel.TrySetComponentValue(itemData.UserName);
                JoinTimeLabel.TrySetComponentValue(itemData.JoinTime);
                ApplyTimeLabel.TrySetComponentValue(itemData.ApplyTime);
                UserCoinLabel.TrySetComponentValue(string.Format(UserCoinFormat, YxUtiles.ReduceNumber(itemData.CoinValue)));
                OnLineStatusLabel.TrySetComponentValue(itemData.OnLineStatus > 0 ? "在线" : "离线");
            }
        }

        #endregion

        #region Function

        #endregion
    }

    /// <summary>
    /// 茶馆成员item数据
    /// </summary>
    public class TeaMemberInfoItemData : YxData
    {
        /// <summary>
        /// Key 昵称
        /// </summary>
        private const string KeyNickName= "nickname";
        /// <summary>
        /// Key 玩家Id
        /// </summary>
        private const string KeyUserId = "user_id";
        /// <summary>
        /// Key 茶馆Id
        /// </summary>
        private const string KeyTeaId = "tea_id";
        /// <summary>
        /// Key 成员加入茶馆时间
        /// </summary>
        private const string KeyLastUpdateTime = "last_update_dt";
        /// <summary>
        /// Key 成员申请时间
        /// </summary>
        private const string KeyApplyTime = "create_dt";
        /// <summary>
        /// Key 消耗货币数量
        /// </summary>
        private const string KeyCoinValue = "coin";
        /// <summary>
        /// Key 是否允许创建房间（teaGetIn的per_ctrl为1时才会有此字段）
        /// </summary>
        private const string KeyAllowCreate = "is_create";
        /// <summary>
        /// Key 成员在线状态
        /// </summary>
        private const string KeyOnLineStatus = "status_n";
        /// <summary>
        /// 玩家昵称
        /// </summary>
        private string _userName;
        /// <summary>
        /// 玩家Id
        /// </summary>
        private string _userId;
        /// <summary>
        /// 加入时间
        /// </summary>
        private string _joinTime;
        /// <summary>
        /// 申请加入时间
        /// </summary>
        private string _applyTime;
        /// <summary>
        /// 茶馆Id
        /// </summary>
        private string _teaId;
        /// <summary>
        /// Coin 值（根据茶馆实际消耗类型决定）
        /// </summary>
        private long _coinValue;
        /// <summary>
        /// 是否允许创建房间 0.不允许 1.允许
        /// </summary>
        private int _allowCreate;
        /// <summary>
        /// 在线状态（0不在线 1在线）
        /// </summary>
        private int _onLineStatus;

        public string UserName
        {
            get { return _userName;}
        }

        public string UserId
        {
            get { return _userId;}
        }

        public string JoinTime
        {
            get { return _joinTime; }
        }

        public string ApplyTime
        {
            get { return _applyTime;}
        }

        public string TeaId
        {
            get { return _teaId; }
        }

        public long CoinValue
        {
            get { return _coinValue;}
        }

        public int AllowCreateRoom
        {
            get { return _allowCreate;}
        }

        public int OnLineStatus
        {
            get{ return _onLineStatus;}
        }

        public TeaMemberInfoItemData(object data) : base(data)
        {
        }

        public TeaMemberInfoItemData(object data, Type type) : base(data, type)
        {
        }

        protected override void ParseData(Dictionary<string, object> dic)
        {
            dic.TryGetValueWitheKey(out _userName, KeyNickName);
            dic.TryGetValueWitheKey(out _userId, KeyUserId);
            dic.TryGetValueWitheKey(out _joinTime, KeyLastUpdateTime);
            dic.TryGetValueWitheKey(out _applyTime, KeyApplyTime);
            dic.TryGetValueWitheKey(out _teaId, KeyTeaId);
            dic.TryGetValueWitheKey(out _coinValue, KeyCoinValue);
            dic.TryGetValueWitheKey(out _allowCreate, KeyAllowCreate);
            dic.TryGetValueWitheKey(out _onLineStatus, KeyOnLineStatus);
        }

        /// <summary>
        /// 更改创建状态
        /// </summary>
        /// <param name="status"></param>
        public void ChangeCreateStatus(int status)
        {
            _allowCreate = status;
        }
    }
}
