using System;
using Assets.Scripts.Hall.View.PageListWindow;
using System.Collections.Generic;
using Assets.Scripts.Hall.View.RecordWindows;
using Assets.Scripts.Common.Utils;
using YxFramwork.Common.Adapters;
using UnityEngine;

namespace Assets.Scripts.Tea.Page
{
    public class UserUnifyRankItem : YxPageItem
    {
        [Tooltip("时间")]
        public YxBaseLabelAdapter TimeLabel;
        [Tooltip("大赢家次数")]
        public YxBaseLabelAdapter BigWinerCountLabel;
        [Tooltip("积分总和")]
        public YxBaseLabelAdapter ScoreLabel;
        [Tooltip("排名")]
        public YxBaseLabelAdapter IndexLabel;
        [Tooltip("头像相关信息")]
        public HeadItem HeadInfo;

        public override Type GetDataType()
        {
            return typeof(TeaUserUnifyRankItemData);
        }

        protected override void OnItemFresh()
        {
            base.OnItemFresh();
            var itemData = GetData<TeaUserUnifyRankItemData>();
            if (itemData != null)
            {
                if (BigWinerCountLabel != null)
                {
                    BigWinerCountLabel.TrySetComponentValue(itemData.WinerCount);
                }
                if (ScoreLabel != null)
                {
                    ScoreLabel.TrySetComponentValue(itemData.Score);
                }
                if (IndexLabel != null)
                {
                    IndexLabel.TrySetComponentValue(Id);
                }
                TimeLabel.TrySetComponentValue(itemData.Time);
                HeadInfo.UpdateView(itemData.HeadDatas);
            }
        }
    }

    /// <summary>
    /// 茶馆玩家积分，大赢家次数排行棒列表数据
    /// </summary>
    public class TeaUserUnifyRankItemData : YxData
    {
        /// <summary>
        /// Key 时间
        /// </summary>
        private const string KeyTime = "date_created";
        /// <summary>
        /// Key 大赢家次数
        /// </summary>
        private const string KeyWinerCount = "count";
        /// <summary>
        /// Key 总积分
        /// </summary>
        private const string KeyScore = "coin_q";

        /// <summary>
        /// 时间
        /// </summary>
        private string _Time;
        /// <summary>
        /// 大赢家次数
        /// </summary>
        private string _WinerCount;
        /// <summary>
        /// 总积分
        /// </summary>
        private string _Score;

        /// <summary>
        /// 玩家信息
        /// </summary>
        private HeadData _headData;

        public string Time { get { return _Time; } }
        public string Score { get { return _Score; } }
        public string WinerCount { get { return _WinerCount; } }

        public HeadData HeadDatas { get { return _headData; } }

        public TeaUserUnifyRankItemData(object data) : base(data)
        {
        }

        public TeaUserUnifyRankItemData(object data, Type type) : base(data, type)
        {
        }

        protected override void ParseData(Dictionary<string, object> dic)
        {
            dic.TryGetValueWitheKey(out _Time, KeyTime);
            dic.TryGetValueWitheKey(out _Score, KeyScore);
            dic.TryGetValueWitheKey(out _WinerCount, KeyWinerCount);

            _headData = new HeadData(dic);
        }
    }
}