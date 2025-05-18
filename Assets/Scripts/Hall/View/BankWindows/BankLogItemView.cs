using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Hall.View.PageListWindow;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.BankWindows
{
    public class BankLogItemView : YxPageItem
    {
        public UILabel LogLabel;
        public YxBaseLabelAdapter CoinLabel;
        public UILabel DateLabel;
        [Tooltip("流水号")]
        public UILabel SerialNumberLabel;
        [Tooltip("其他玩家的名字")]
        public UILabel UserNickNameLabel;
        [Tooltip("其他玩家的Id")]
        public UILabel OtherUserIdLabel;
        [Tooltip("描述不为空的触发事件")]
        public List<EventDelegate> DescDelegates;
        /// <summary>
        /// key赠送类型
        /// </summary>
        private const string KeyType = "type_i";
        /// <summary>
        /// 当前信息缓存数据
        /// </summary>
        private BankLogItemData _cacheData;
        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (!(Data is Dictionary<string, object>)) return;
            var data = (Dictionary<string, object>) Data;
            int type;
            data.TryGetValueWitheKey(out type, KeyType);
            long coin;
            data.TryGetValueWitheKey(out coin, "coin_num_a");
            var tempFormat = type > 0 ? "玩家【{0}】赠送给您" : "您赠送给玩家【{0}】";
            LogLabel.text = string.Format(tempFormat, data["nick_m"]);
            DateLabel.text = data["create_dt"].ToString();
            CoinLabel.TrySetComponentValue(coin, "1", "x{0}金币", YxBaseLabelAdapter.YxELabelType.NumberWithUnit);
        }
        public override Type GetDataType()
        {
            return typeof(BankLogItemData);
        }

        protected override void OnItemFresh()
        {
            _cacheData = Data as BankLogItemData;
            if (_cacheData == null) return;
            if (!string.IsNullOrEmpty(_cacheData.Desc))
            {
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(DescDelegates.WaitExcuteCalls());
                }
            } 
            var tempFormat = _cacheData.SendType > 0 ? "玩家【{0}】赠送给您" : "您赠送给玩家【{0}】";
            LogLabel.TrySetComponentValue(string.Format(tempFormat, _cacheData.Nick));
            DateLabel.TrySetComponentValue(_cacheData.Time);
            CoinLabel.TrySetComponentValue(_cacheData.Value, "1", "x{0}金币", YxBaseLabelAdapter.YxELabelType.NumberWithUnit);
            SerialNumberLabel.TrySetComponentValue(_cacheData.SerialNumberId);
            UserNickNameLabel.TrySetComponentValue(_cacheData.Nick);
            OtherUserIdLabel.TrySetComponentValue(_cacheData.UserId);
        }

        public string MessageInfo
        {
            get { return _cacheData.Desc; }
        }

        /// <summary>
        /// item 点击事件
        /// </summary>
        public void OnClickItem(string windowName)
        {
            MainYxView.OpenWindowWithData(windowName, _cacheData);
        }

        /// <summary>
        /// item 点击之后显示赠送描述
        /// </summary>
        /// <param name="messageInfo"></param>
        /// <param name="windowName"></param>
        public void ShowMessage(string messageInfo,string windowName=null)
        {
            if (!string.IsNullOrEmpty(messageInfo))
            {
                YxMessageBox.Show(null, windowName, messageInfo);
            }
        }
        /// <summary>
        /// 有描述金币label颜色特殊显示
        /// </summary>
        /// <param name="color"></param>
        public void DescColorChange(Color color)
        {
            CoinLabel.Color = color;
        }
    }

    public class BankLogItemData:YxData
    {
        /// <summary>
        /// key赠送类型
        /// </summary>
        private const string KeyType = "type_i";
        /// <summary>
        /// key金币数量
        /// </summary>
        private const string KeyCoinNum = "coin_num_a";
        /// <summary>
        /// key昵称
        /// </summary>
        private const string KeyNick = "nick_m";
        /// <summary>
        /// key创建时间
        /// </summary>
        private const string KeyTime = "create_dt";
        /// <summary>
        /// key 玩家ID
        /// </summary>
        private const string KeyUserId = "user_id";
        /// <summary>
        /// key 流水号ID
        /// </summary>
        public const string KeySerialNumber = "id";
        /// <summary>
        /// key 赠送的描述
        /// </summary>
        public const string KeyDesc = "desc_x";
        /// <summary>
        /// 类型 大于0:接收 小于等于0赠送
        /// </summary>
        private int _type;
        /// <summary>
        /// 值
        /// </summary>
        private long _value;
        /// <summary>
        /// 时间
        /// </summary>
        private string _nick;
        /// <summary>
        /// 时间
        /// </summary>
        private string _time;
        /// <summary>
        /// UserID
        /// </summary>
        private string _userId;
        /// <summary>
        /// 流水号ID
        /// </summary>
        private string _serialNumberId;
        /// <summary>
        /// 赠送的描述
        /// </summary>
        private string _desc;

        public int SendType
        {
            get
            {
                return _type;
            }
        }

        public long Value
        {
            get
            {
                return _value;
            }
        }


        public string Time
        {
            get
            {
                return _time;
            }
        }

        public string Nick
        {
            get
            {
                return _nick;
            }
        }

        public string UserId
        {
            get { return _userId; }
        }

        public string SerialNumberId
        {
            get { return _serialNumberId; }
        }

        public string Desc
        {
            get { return _desc; }
        }

        public BankLogItemData(object data) : base(data)
        {
        }

        public BankLogItemData(object data, Type type) : base(data, type)
        {
        }
        protected override void ParseData(Dictionary<string, object> dic)
        {
            dic.TryGetValueWitheKey(out _type, KeyType);
            dic.TryGetValueWitheKey(out _value, KeyCoinNum);
            dic.TryGetValueWitheKey(out _time, KeyTime);
            dic.TryGetValueWitheKey(out _nick, KeyNick);
            dic.TryGetValueWitheKey(out _userId, KeyUserId);
            dic.TryGetValueWitheKey(out _serialNumberId, KeySerialNumber);
            dic.TryGetValueWitheKey(out _desc, KeyDesc);
        }
    }
}
