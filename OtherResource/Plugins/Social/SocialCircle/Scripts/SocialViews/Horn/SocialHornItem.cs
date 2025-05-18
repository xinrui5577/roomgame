using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using UnityEngine;
using YxFramwork.Common.Adapters;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.Horn
{
    public class SocialHornItem : BaseSocialWrapItem
    {
        [Tooltip("ÄÚÈÝ")]
        public YxBaseLabelAdapter Content;
        protected override void DealFreshData()
        {
            string desc;
            ParseDicData.TryGetValueWitheKey(out desc,SocialTools.KeyMessageContent);
            Content.TrySetComponentValue(desc);
            base.DealFreshData();
        }
    }
}
