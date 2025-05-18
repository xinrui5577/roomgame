/** 
 *文件名称:     ExtractItem.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2018-04-19 
 *描述:         兑换记录item
 *历史记录: 
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Hall.View.PageListWindow;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Tool;

namespace Assets.Scripts.Hall.View.BankWindows
{
    public class BankExtractRecordItem :YxPageItem
    {
        #region UI Param
        [Tooltip("兑换平台")]
        public UILabel Platform;
        [Tooltip("消耗数量")]
        public YxBaseLabelAdapter CostNumber;
        [Tooltip("兑换比例")]
        public UILabel ExtractRate;
        [Tooltip("获得金额")]
        public UILabel GetNumber;
        [Tooltip("时间")]
        public UILabel Time;
        [Tooltip("提现状态")]
        public GameObject[] StatusObjs;
        [Tooltip("失败信息内容")]
        public UILabel FailText;
        #endregion

        #region Data Param
        /// <summary>
        /// 消耗金币格式
        /// </summary>
        public string CostFormat= "x{0}";
        /// <summary>
        /// 显示现金格式
        /// </summary>
        public string GetFormat = "{0}元";
        /// <summary>
        /// 比例格式
        /// </summary>
        public string RateFormat = "{0}:1";
        #endregion

        #region Local Data

        public override Type GetDataType()
        {
            return typeof (ExtractRecordItemData);
        }

        protected override void OnItemFresh()
        {
            var itemData = Data as ExtractRecordItemData;
            Platform.TrySetComponentValue(itemData.PlatformName);
            CostNumber.TrySetComponentValue(itemData.ExtractValue,"1",CostFormat);
            ExtractRate.TrySetComponentValue(string.Format(RateFormat, YxUtiles.ReduceNumber(itemData.PlatformRate)));
            GetNumber.TrySetComponentValue(string.Format(GetFormat,GlobalUtile.ReduceNumber(itemData.GetValue)));
            Time.TrySetComponentValue(itemData.ExtractTime);
            FailText.TrySetComponentValue(itemData.FailDesc);
            var count = StatusObjs.Length;
            var status = itemData.Status;
            for (int i=0;i< count;i++)
            {
                StatusObjs[i].TrySetComponentValue(i==status);
            }
        }

        #endregion

        #region Life Cycle

        #endregion

        #region Function

        #endregion

    }

    /// <summary>
    /// 兑换记录数据
    /// </summary>
    public class ExtractRecordItemData : YxData
    {
        /// <summary>
        /// Key平台名称
        /// </summary>
        private const string KeyPlatformName = "platformname";
        /// <summary>
        /// Key倍率
        /// </summary>
        private const string KeyRate = "rate";
        /// <summary>
        /// Key兑换值
        /// </summary>
        private const string KeyExtractValue = "extractvalue";
        /// <summary>
        /// Key兑换现金
        /// </summary>
        private const string KeyGetValue = "value";
        /// <summary>
        /// Key时间
        /// </summary>
        private const string KeyExtractTime = "create_dt";
        /// <summary>
        /// Key状态
        /// </summary>
        private const string KeyStatus = "status_i";
        /// <summary>
        /// Key失败状态信息描述
        /// </summary>
        private const string KeyFailDesc = "desc_x";
        /// <summary>
        /// 平台名称
        /// </summary>
        private string _platformName;
        /// <summary>
        /// 兑换比例
        /// </summary>
        private long _rate;
        /// <summary>
        ///兑换数量（消耗）
        /// </summary>
        private long _cost;
        /// <summary>
        /// 兑换金额
        /// </summary>
        private float _get;
        /// <summary>
        /// 兑换时间
        /// </summary>
        private string _time;
        /// <summary>
        /// 提现状态：0申请中 1申请成功 2申请失败
        /// </summary>
        private int _status;
        /// <summary>
        /// 失败文本提示
        /// </summary>
        private string _failDesc;

        public string PlatformName
        {
            get { return _platformName; }
        }

        public long PlatformRate
        {
            get { return _rate; }
        }
        public long ExtractValue
        {
            get { return _cost; }
        }
        public float GetValue
        {
            get { return _get; }
        }

        public string ExtractTime
        {
            get { return _time; }
        }

        public int Status
        {
            get { return _status; }
        }

        public string FailDesc
        {
            get { return _failDesc;}
        }

        public ExtractRecordItemData(object data) : base(data)
        {
        }

        public ExtractRecordItemData(object data, Type type) : base(data, type)
        {
        }

        protected override void ParseData(Dictionary<string, object> dic)
        {
            dic.TryGetValueWitheKey(out _platformName, KeyPlatformName);
            dic.TryGetValueWitheKey(out _rate, KeyRate);
            dic.TryGetValueWitheKey(out _cost, KeyExtractValue);
            dic.TryGetValueWitheKey(out _get, KeyGetValue);
            dic.TryGetValueWitheKey(out _time, KeyExtractTime);
            dic.TryGetValueWitheKey(out _status, KeyStatus);
            dic.TryGetValueWitheKey(out _failDesc, KeyFailDesc);
        }
    }
}
