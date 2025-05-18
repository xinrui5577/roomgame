using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Hall.View.PageListWindow;
using UnityEngine;
using YxFramwork.Common.Adapters;

/*===================================================
 *文件名称:     TeaHouseListItem.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-11-24
 *描述:        	茶馆列表Item
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Tea.Page
{
    public class TeaHouseListItem : YxPageItem 
    {
        #region UI Param
        [Tooltip("茶馆名称")]
        public YxBaseLabelAdapter TeaHouseNameLabel;
        [Tooltip("茶馆口令")]
        public YxBaseLabelAdapter TeaHouseIdLabel;
        [Tooltip("茶馆创建时间")]
        public YxBaseLabelAdapter TeaHouseCreateTimeLabel;
        [Tooltip("茶馆牌桌数量上限")]
        public YxBaseLabelAdapter TeaTableLimitLabel;
        [Tooltip("茶馆成员数量上限")]
        public YxBaseLabelAdapter TeaPersonLimitLabel;
        [Tooltip("状态按钮")]
        public GameObject[] StateButtons;
        #endregion

        #region Data Param
        [Tooltip("是否为馆主信息，用于区分UI")]
        public bool IsManager;

        /// <summary>
        /// 馆主创建房间开关（true:只有馆主可以创建 false:成员可以创建）
        /// </summary>
        public bool MangerCreateTable { get; private set;}
        /// <summary>
        /// 所有人都可以加入房间（true:所有人都可以，包括馆外人员 false:只有当前茶馆成员可以进入）
        /// </summary>
        public bool AnyoneJoinTable { get; private set;}
        /// <summary>
        /// 创建房间消耗个人资源（true：消耗创建者资源 false:消耗馆主资源）
        /// </summary>
        public bool CostByUser { get; private set;}

        #endregion

        #region Local Data
        /// <summary>
        /// 当前状态
        /// </summary>
        private int _curStatus = -1;
        #endregion

        #region Life Cycle

        public override Type GetDataType()
        {
            return typeof(TeaHouseListeItemData);
        }

        protected override void OnItemFresh()
        {
            base.OnItemFresh();
            var itemData = GetData<TeaHouseListeItemData>();
            if (itemData!=null)
            {
                TeaHouseNameLabel.TrySetComponentValue(itemData.TeaHouseName);
                TeaHouseIdLabel.TrySetComponentValue(itemData.TeaId);
                TeaHouseCreateTimeLabel.TrySetComponentValue(itemData.CreateTime);
                SetItemState(itemData.JoinStatus);
                if (IsManager)
                {
                    TeaTableLimitLabel.TrySetComponentValue(itemData.TableLimit);
                    TeaPersonLimitLabel.TrySetComponentValue(itemData.PersonLimit);
                    MangerCreateTable = itemData.CreateLimi==1;
                    AnyoneJoinTable = itemData.JoinLimit==1;
                    CostByUser = itemData.ConsumeLimit==1;
                }
            }
        }

        /// <summary>
        /// 设置Item状态
        /// </summary>
        /// <param name="status">变更状态</param>
        public void SetItemState(int status)
        {
            if (_curStatus!=status)
            {
                if (_curStatus!=-1)
                {
                    StateButtons[_curStatus].TrySetComponentValue(false);
                }
                _curStatus = status;
                StateButtons[_curStatus].TrySetComponentValue(true);
            }
        }

        #endregion

        #region Function

        #endregion
    }

    public class TeaHouseListeItemData:YxData
    {
        /// <summary>
        /// key 茶馆ID
        /// </summary>
        protected const string KeyTeaId= "tea_id";
        /// <summary>
        /// Key 馆主ID
        /// </summary>
        protected const string KeyOwenerId = "user_id";
        /// <summary>
        /// Key 茶馆名称
        /// </summary>
        protected const string KeyTeaHouseName = "tea_name";
        /// <summary>
        /// Key 茶馆名称(TeaGetIn接口数据兼容处理)
        /// </summary>
        protected const string KeyTeaGetInHouseName = "name";
        /// <summary>
        /// Key 创建时间
        /// </summary>
        protected const string KeyCreatTime = "create_dt";
        /// <summary>
        /// Key 申请加入状态值
        /// </summary>
        protected const string KeyStatus = "status_t";
        /// <summary>
        /// Key 创建房间上限
        /// </summary>
        protected const string KeyNumberC = "num_c";
        /// <summary>
        /// Key 创建房间上限
        /// </summary>
        protected const string KeyTeaGetInNumberC = "roomNum";
        /// <summary>
        /// Key 群人数上限
        /// </summary>
        protected const string KeyGroupN = "group_n";
        /// <summary>
        /// Key 开房权限0.所有人都可以创建 1.只有群主能创建
        /// </summary>
        protected const string KeyOnlyOwner = "only_owner";
        /// <summary>
        /// Key 是否禁止其他人进入群游戏
        /// </summary>
        protected const string KeyForbitOther = "forbit_other";
        /// <summary>
        /// Key 房卡消耗人
        /// </summary>
        protected const string KeyConsumeUser = "consume_user";
        /// <summary>
        /// 茶馆加入状态：申请完成状态
        /// </summary>
        protected const int JoinStatusApplyFinished=1;
        /// <summary>
        /// 茶馆ID（口令）
        /// </summary>
        private string _teaId;
        /// <summary>
        /// 馆主Id
        /// </summary>
        private string _ownerId;
        /// <summary>
        /// 茶馆名称
        /// </summary>
        private string _teaHouseName;
        /// <summary>
        /// 创建时间
        /// </summary>
        private string _createTime;
        /// <summary>
        /// 茶馆加入状态:  0未申请 1已申请3已加入
        /// </summary>
        private int _joinStatus;
        /// <summary>
        /// 牌桌上限数量
        /// </summary>
        private int _tableLimit;
        /// <summary>
        /// 人数上限
        /// </summary>
        private int _personLimit;
        /// <summary>
        /// 创建房间权限0.茶馆内所有人 1.馆主
        /// </summary>
        private int _createLimit;
        /// <summary>
        /// 加入房间权限
        /// </summary>
        private int _joinLimit;
        /// <summary>
        /// 卡耗相关权限
        /// </summary>
        private int _consumeLimit;

        public string TeaId
        {
            get { return _teaId; }
        }

        public string OwnerId
        {
            get { return _ownerId; }
        }

        public string TeaHouseName
        {
            get { return _teaHouseName;}
        }

        public string CreateTime
        {
            get { return _createTime;}
        }

        public int JoinStatus
        {
            get { return _joinStatus; }
        }

        public int TableLimit
        {
            get { return _tableLimit; }
        }

        public int PersonLimit
        {
            get
            {
                return _personLimit;
            }
        }

        public int CreateLimi
        {
            get { return _createLimit; }
        }

        public int JoinLimit
        {
            get { return _joinLimit; }
        }

        public int ConsumeLimit
        {
            get { return _consumeLimit; }
        }

        public TeaHouseListeItemData(object data) : base(data)
        {
        }

        public TeaHouseListeItemData(object data, Type type) : base(data, type)
        {
        }

        protected override void ParseData(Dictionary<string, object> dic)
        {
            dic.TryGetValueWitheKey(out _teaId, KeyTeaId);
            dic.TryGetValueWitheKey(out _ownerId, KeyOwenerId);
            dic.TryGetValueWitheKey(out _teaHouseName, KeyTeaHouseName);
            dic.TryGetValueWitheKey(out _teaHouseName, KeyTeaGetInHouseName, _teaHouseName);
            dic.TryGetValueWitheKey(out _createTime, KeyCreatTime);
            dic.TryGetValueWitheKey(out _joinStatus, KeyStatus,-1);
            dic.TryGetValueWitheKey(out _tableLimit, KeyNumberC);
            dic.TryGetValueWitheKey(out _tableLimit, KeyTeaGetInNumberC, _tableLimit);
            dic.TryGetValueWitheKey(out _personLimit, KeyGroupN);
            dic.TryGetValueWitheKey(out _createLimit, KeyOnlyOwner);
            dic.TryGetValueWitheKey(out _joinLimit, KeyForbitOther);
            dic.TryGetValueWitheKey(out _consumeLimit, KeyConsumeUser);
        }
        /// <summary>
        /// 申请成功
        /// </summary>
        public void OnApplySuccess()
        {
            _joinStatus = JoinStatusApplyFinished;
        }
    }
}
