using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Common.Utils;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Hall.Windows.PromoteWindows
{
    /// <summary>
    /// 推广
    /// </summary>
    public class PromoteWindow : YxWindow
    {
        [Tooltip("请求名字")]
        public string ActionName = "allSpread";
        [Tooltip("领取请求")]
        public string GainAwardActionName = "getSpreadAward";
        [Tooltip("昨日总奖励")]
        public YxBaseLabelAdapter YesterdayAwardLabel;
        [Tooltip("已推广人数")]
        public YxBaseLabelAdapter HasPromoteCountLabel;
        [Tooltip("昨日直属返利")]
        public YxBaseLabelAdapter YestdayDirectlyRebateLabel;
        [Tooltip("历史总返利")]
        public YxBaseLabelAdapter YestdayHistorylyRebateLabel;
        [Tooltip("可领取的奖励")]
        public YxBaseLabelAdapter GainAwardLabel;
        [Tooltip("可领取的额外奖励")]
        public YxBaseLabelAdapter GainExtraAwardLabel;
        [Tooltip("领取按钮")]
        public YxBaseButtonAdapter BtnAward;
        
        protected override void OnAwake()
        {
            base.OnAwake();
            InitStateTotal++;
            Facade.Instance<TwManager>().SendAction(ActionName,null,UpdateView);
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var dict = GetData<Dictionary<string, object>>();
            if (dict == null) { return;}
            //昨日总奖励
            var yesterdayAward = 0f;
            dict.Parse("yesterdayAward",ref yesterdayAward);
            //已推广人数
            var allAffNum = 0;
            dict.Parse("allAffNum",ref allAffNum);
            //昨日直属返利
            var yesterdDirRevenue = 0f;
            dict.Parse("yesterdDirRevenue",ref yesterdDirRevenue);
            //历史总返利
            var allAffRevenue = 0f;
            dict.Parse("allAffRevenue", ref allAffRevenue);
            //可领取的奖励
            var allAffAward = 0f;
            dict.Parse("allAffAward", ref allAffAward);
            //可领取的额外奖励
            var allAffAwardExt = 0f;
            dict.Parse("allAffAwardExt", ref allAffAwardExt);
            dict.Parse("openSpreadUrl", ref _richesUrl);

            YesterdayAwardLabel.TrySetComponentValue(yesterdayAward.ToString(CultureInfo.InvariantCulture));
            HasPromoteCountLabel.TrySetComponentValue(allAffNum.ToString(CultureInfo.InvariantCulture));
            YestdayDirectlyRebateLabel.TrySetComponentValue(yesterdDirRevenue.ToString(CultureInfo.InvariantCulture));
            YestdayHistorylyRebateLabel.TrySetComponentValue(allAffRevenue.ToString(CultureInfo.InvariantCulture));
            GainAwardLabel.TrySetComponentValue(allAffAward.ToString(CultureInfo.InvariantCulture));
            GainExtraAwardLabel.TrySetComponentValue(allAffAwardExt.ToString(CultureInfo.InvariantCulture));
            if (BtnAward != null)
            {
                BtnAward.SetActive(allAffAward > 0 || allAffAwardExt > 0);
            }
        }

        /// <summary>
        /// 获取奖励
        /// </summary>
        public void OnAwardClick()
        {
            if (!YxBaseButtonAdapter.EnableClick(GainAwardActionName, 5000))
            {
                YxMessageBox.Show("您的操作太频繁，请稍后再试！", 3);
                return;
            }
            CurTwManager.SendAction(GainAwardActionName,null, obj =>
            {
                
            });
        }

        private string _richesUrl;
        public void OnOpenRichesClick()
        {
            if (string.IsNullOrEmpty(_richesUrl)) return;
            Application.OpenURL(_richesUrl);
        }

        public override YxEUIType UIType
        {
            get { return GetComponent<YxBaseAdapter>().UIType; }
        }
    }
}
