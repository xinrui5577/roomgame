using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.Windows.PromoteWindows
{
    public class PromoteExtractRecordItem : YxView
    {
        [Tooltip("")]
        public YxBaseLabelAdapter DateTimeLabel;
        [Tooltip("")]
        public YxBaseLabelAdapter RewardLabel;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var info = GetData<PromoteExtractRecordInfo>();
            if (info == null) return;
            DateTimeLabel.TrySetComponentValue(info.DateTime);
            RewardLabel.TrySetComponentValue(info.Reward.ToString());
        }
    }

    public class PromoteExtractRecordInfo
    {

        public string DateTime;
        public int Reward;

        public void Parse(object data)
        {
            var dict = data as Dictionary<string, object>;
            if (dict != null)
            {
                Parse(dict);
            }
        }

        private void Parse(Dictionary<string, object> dict)
        {
            dict.Parse("date_created", ref DateTime);
            dict.Parse("coin_q", ref Reward);
        }
    }
}
