using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Common.Utils;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.Windows.PromoteWindows
{
    public class PromoteDetailItem : YxView
    {
        [Tooltip("日期label")]
        public YxBaseLabelAdapter DateTimeLabel;
        [Tooltip("昵称label")]
        public YxBaseLabelAdapter NickLabel;
        [Tooltip("用户Idlabel")]
        public YxBaseLabelAdapter UserIdLabel;
        [Tooltip("推广人数label")]
        public YxBaseLabelAdapter PromoteCountLabel;
        [Tooltip("当日返佣label")]
        public YxBaseLabelAdapter CurRebateLabel;
        [Tooltip("历史返佣label")]
        public YxBaseLabelAdapter HistoryRebateLabel;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var data = GetData<PromoteDetailInfo>();
            if (data == null) return;
            DateTimeLabel.TrySetComponentValue(data.DateTime);
            NickLabel.TrySetComponentValue(data.Nick);
            UserIdLabel.TrySetComponentValue(data.UserId);
            PromoteCountLabel.TrySetComponentValue(data.PromoteCount);
            CurRebateLabel.TrySetComponentValue(data.CurRebate.ToString(CultureInfo.InvariantCulture));
            HistoryRebateLabel.TrySetComponentValue(data.HistoryRebate.ToString(CultureInfo.InvariantCulture));
        }
    }

    public class PromoteDetailInfo
    {
        public string DateTime;
        public string Nick;
        public string UserId;
        public int PromoteCount;
        public float CurRebate;
        public float HistoryRebate;

        public void Parse(object infoData)
        {
            var data = infoData as Dictionary<string, object>;
            if (data != null)
            {
                Parse(data);
            }
        }

        protected void Parse(Dictionary<string,object> infoData)
        {
            infoData.Parse("time", ref DateTime);
            infoData.Parse("nick_m", ref Nick);
            infoData.Parse("ID", ref UserId);
            infoData.Parse("allAffNum", ref PromoteCount);
            infoData.Parse("revenue", ref CurRebate);
            infoData.Parse("allRenvenue", ref HistoryRebate);
        }
    }
}
