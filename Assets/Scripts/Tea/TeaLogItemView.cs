using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Hall.View.PageListWindow;
using UnityEngine;
using YxFramwork.Common.Adapters;

namespace Assets.Scripts.Tea
{
    public class TeaLogItemView : YxPageItem
    {
        public UILabel LogLabel;
        public YxBaseLabelAdapter CoinLabel;
        public UILabel DateLabel;

        /// <summary>
        /// 流水号
        /// </summary>
        public UILabel SerialNumber;
        /// <summary>
        /// 昵称
        /// </summary>
        public UILabel NickName;
        /// <summary>
        /// 玩家的游戏ID
        /// </summary>
        public UILabel UserId;
        /// <summary>
        /// 金币的变化数量
        /// </summary>
        public UILabel CoinChange;
        /// <summary>
        /// 创建的时间
        /// </summary>
        public UILabel CreatTime;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var data = Data as Dictionary<string, object>;
            if (data == null) return;
            var nickName = data.ContainsKey("nick_m") ? data["nick_m"].ToString() : "";
            var id = data.ContainsKey("id") ? data["id"].ToString() : "";
            var otherId = data.ContainsKey("other_id") ? data["other_id"].ToString() : "";
            var coinNum = data.ContainsKey("coin_num_a") ? data["coin_num_a"].ToString() : "";
            var createDt = data.ContainsKey("create_dt") ? data["create_dt"].ToString() : "";

            SerialNumber.text = id;
            NickName.text = nickName;
            UserId.text = otherId;
            CoinChange.text = coinNum;
            CreatTime.text = createDt;
        }

        public override Type GetDataType()
        {
            return typeof(Dictionary<string, object>);
        }

        protected override void OnItemFresh()
        {
            var itemData = new TeaLogItemData();
            itemData.ParseData(Data as Dictionary<string, object>);
            var tempFormat = itemData.Type > 0 ? "玩家【{0}】赠送给您" : "您赠送给玩家【{0}】";
            LogLabel.TrySetComponentValue(string.Format(tempFormat, itemData.Nick));
            DateLabel.TrySetComponentValue(itemData.Time);
            CoinLabel.TrySetComponentValue(itemData.Value, "1", "x{0}金币", YxBaseLabelAdapter.YxELabelType.NumberWithUnit);
        }
    }

    public class TeaLogItemData
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
        /// 类型
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

        public int Type
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

        public void ParseData(Dictionary<string, object> dic)
        {
            dic.TryGetValueWitheKey(out _type, KeyType);
            dic.TryGetValueWitheKey(out _value, KeyCoinNum);
            dic.TryGetValueWitheKey(out _time, KeyTime);
            dic.TryGetValueWitheKey(out _nick, KeyNick);
        }                               
    }
}
