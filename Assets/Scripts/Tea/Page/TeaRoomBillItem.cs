using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Hall.View.PageListWindow;
using Assets.Scripts.Hall.View.RecordWindows;
using UnityEngine;
using YxFramwork.Common.Adapters;

/*===================================================
 *文件名称:     TeaRoomBillItem.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-10-11
 *描述:        	茶馆账单信息Item
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Tea.Page
{
    public class TeaRoomBillItem : YxPageItem 
    {
        #region UI Param
        [Tooltip("房间ID文本")]
        public YxBaseLabelAdapter RoomIdLabel;
        [Tooltip("游戏名称文本")]
        public YxBaseLabelAdapter GameNameLabel;
        [Tooltip("房间信息文本")]
        public YxBaseLabelAdapter GameInfoLabel;
        [Tooltip("创建时间文本")]
        public YxBaseLabelAdapter CreateTimeLabel;
        [Tooltip("结束时间文本")]
        public YxBaseLabelAdapter EndTimeLabel;
        [Tooltip("索引文本")]
        public YxBaseLabelAdapter IndexLabel;
        [Tooltip("头像相关信息")]
        public HeadItem[] Heads;
        #endregion

        #region Data Param
        [Tooltip("账单房间号长度，默认6")]
        public int ShowRoomIdLenth=6;
        [Tooltip("结束时间格式")]
        public string EndTimeFormat = "结束时间:{0}";
        #endregion

        #region Local Data

        #endregion

        #region Life Cycle

        public override Type GetDataType()
        {
            return typeof(TeaRoomBillItemData);
        }

        /// <summary>
        /// 房间账单是否结算
        /// </summary>
        public bool RoomBillFinished
        {
            private set; get;
        }

        protected override void OnItemFresh()
        {
            base.OnItemFresh();
            var itemData = GetData<TeaRoomBillItemData>() ;
            if(itemData!=null)
            {
                RoomIdLabel.TrySetComponentValue(itemData.RoomId.Substring(itemData.RoomId.Length- ShowRoomIdLenth));
                GameNameLabel.TrySetComponentValue(itemData.GameName);
                GameInfoLabel.TrySetComponentValue(itemData.InfoStr);
                CreateTimeLabel.TrySetComponentValue(itemData.CreateTime);
                var endTime = itemData.EndTime;
                if (!string.IsNullOrEmpty(endTime))
                {
                    EndTimeLabel.TrySetComponentValue(string.Format(EndTimeFormat, endTime));
                }
                
                IndexLabel.TrySetComponentValue(Id);
                var dataCount = itemData.HeadDatas.Count;
                for (int i = 0; i < Heads.Length; i++)
                {
                    var headState = dataCount >i;
                    var headItem = Heads[i];
                    headItem.gameObject.TrySetComponentValue(headState);
                    if (headState)
                    {
                        var headData = itemData.HeadDatas[i];
                        if (headItem)
                        {
                            headItem.UpdateView(headData);
                        }
                    }
                }
                RoomBillFinished = itemData.RooedmFihished;
            }
        }

        

        #endregion

        #region Function
        /// <summary>
        /// 点击查看
        /// </summary>
        public void OnCheckClick(string windowName)
        {
            MainYxView.OpenWindowWithData(windowName, Data);
        }
        #endregion
    }

    /// <summary>
    /// 茶馆账单数据
    /// </summary>
    public class TeaRoomBillItemData : YxData
    {
        /// <summary>
        /// KeyGameKey
        /// </summary>
        private const string KeyGameKey = "game_key_c";
        /// <summary>
        /// Key 游戏名称
        /// </summary>
        private const string KeyGameName = "game_name";
        /// <summary>
        /// Key 分组id
        /// </summary>
        private const string KeyGroupId = "group_id";
        /// <summary>
        /// Key 房间信息
        /// </summary>
        private const string KeyInfoStr = "info_str";
        /// <summary>
        /// Key 房间ID
        /// </summary>
        private const string KeyRoomId = "room_id";
        /// <summary>
        /// Key 局数
        /// </summary>
        private const string KeyRoundNum = "round_num";
        /// <summary>
        /// Key 账单结算类型
        /// </summary>
        private const string KeyTypeI = "type_i";
        /// <summary>
        /// Key 消耗品数量
        /// </summary>
        private const string KeyUseNum= "use_num";
        /// <summary>
        /// Key 玩家数量
        /// </summary>
        private const string KeyUserNum = "user_num";
        /// <summary>
        /// Key 创建时间
        /// </summary>
        private const string KeyCreateTime = "create_dt";
        /// <summary>
        /// Key 结束时间
        /// </summary>
        private const string KeyEndTime = "update_dt";
        /// <summary>
        /// Key 玩家基本信息
        /// </summary>
        private const string KeyUserInfo = "userinfo";
        /// <summary>
        /// GameKey
        /// </summary>
        private string _gameKey;
        /// <summary>
        /// 游戏名称
        /// </summary>
        private string _gameName;
        /// <summary>
        /// 茶馆ID
        /// </summary>
        private string _groupId;
        /// <summary>
        /// 房间信息
        /// </summary>
        private string _infoStr;
        /// <summary>
        /// 房间号
        /// </summary>
        private string _roomId;
        /// <summary>
        /// 局数或圈数（根据use_num判断）
        /// </summary>
        private string _roundNum;
        /// <summary>
        /// 账单完成类型：0,未结算 1.已结算
        /// </summary>
        private int _typeI;
        /// <summary>
        /// 消耗数量（创建房间消耗物品）
        /// </summary>
        private string _useNum;
        /// <summary>
        /// 玩家数量
        /// </summary>
        private string _userNum;
        /// <summary>
        /// 创建时间
        /// </summary>
        private string _createTime;
        /// <summary>
        /// 结束时间
        /// </summary>
        private string _endTime;
        /// <summary>
        /// 玩家信息
        /// </summary>
        private List<HeadData> _headDatas=new List<HeadData>();
        public string GameKey {get { return _gameKey;}}
        public string GameName { get { return _gameName; } }
        public string GroupId { get { return _groupId; } }
        public string InfoStr { get { return _infoStr; } }
        public string RoomId { get { return _roomId; } }
        public string RoomNum { get { return _roundNum; } }
        public bool RooedmFihished { get { return _typeI==1; } }
        public string UseNum { get { return _useNum; } }
        public string UserNum { get { return _userNum; } }
        public string CreateTime { get { return _createTime; } }

        public string EndTime { get { return _endTime; } }

        public List<HeadData> HeadDatas { get { return _headDatas; } }


        public TeaRoomBillItemData(object data) : base(data)
        {
        }

        public TeaRoomBillItemData(object data, Type type) : base(data, type)
        {
        }

        protected override void ParseData(Dictionary<string, object> dic)
        {
            dic.TryGetValueWitheKey( out _gameKey, KeyGameKey);
            dic.TryGetValueWitheKey( out _gameName, KeyGameName);
            dic.TryGetValueWitheKey( out _groupId, KeyGroupId);
            dic.TryGetValueWitheKey( out _infoStr, KeyInfoStr);
            dic.TryGetValueWitheKey( out _roomId, KeyRoomId);
            dic.TryGetValueWitheKey( out _roundNum, KeyRoundNum);
            dic.TryGetValueWitheKey( out _useNum, KeyUseNum);
            dic.TryGetValueWitheKey( out _userNum, KeyUserNum);
            dic.TryGetValueWitheKey(out _createTime, KeyCreateTime);
            dic.TryGetValueWitheKey(out _endTime, KeyEndTime);
            dic.TryGetValueWitheKey(out _typeI, KeyTypeI);
            Dictionary<string, object> userInfoDic;
            dic.TryGetValueWitheKey(out userInfoDic, KeyUserInfo);
            ParseHeadDatas(userInfoDic);
        }

        void ParseHeadDatas(Dictionary<string,object> headDatas)
        {
            _headDatas.Clear();
            foreach (var valuePair in headDatas)
            {
                var headData=new HeadData(valuePair.Key,valuePair.Value);
                _headDatas.Add(headData);
            }
        }
    }
}
