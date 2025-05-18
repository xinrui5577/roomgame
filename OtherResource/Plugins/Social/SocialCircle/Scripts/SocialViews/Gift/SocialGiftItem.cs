using Assets.Scripts.Common.Utils;
using Assets.Scripts.Hall.View.RecordWindows;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using UnityEngine;
using YxFramwork.Common.Adapters;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.Gift
{
    /// <summary>
    /// 礼物Item
    /// </summary>
    public class SocialGiftItem : BaseSocialSelectWrapItem
    {
        [Tooltip("礼物img")]
        public YxBaseSpriteAdapter GiftImg;
        [Tooltip("礼物名称")]
        public YxBaseLabelAdapter GiftName;
        [Tooltip("礼物名称")]
        public YxBaseLabelAdapter GiftNum;
        protected override void DealFreshData()
        {
            base.DealFreshData();
            string nickName;
            ParseDicData.TryGetValueWitheKey(out nickName,HeadData.KeyName);
            GiftName.TrySetComponentValue(nickName);
            GiftImg.TrySetComponentValue(OnlyId);
            if(SelectStatus)
            {
                int num;
                ParseDicData.TryGetValueWitheKey(out num, SocialTools.KeyNum);
                GiftNum.TrySetComponentValue(num);
            }
        }
    }
}
