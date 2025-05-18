using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using UnityEngine;
using YxFramwork.Common.Adapters;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.Comments
{
    /// <summary>
    /// 评论item
    /// </summary>
    public class SocialCommentsItem : BaseSocialWrapHeadItem
    {
        [Tooltip("描述文本")]
        public YxBaseLabelAdapter DescText;
        [Tooltip("点赞数量")]
        public YxBaseLabelAdapter AgreeNumText;
        [Tooltip("点赞状态")]
        public List<EventDelegate> AgreeStatusAction=new List<EventDelegate>();

        public bool AgreeStatus { private set; get;}

        public int AgreeNum { private set; get; }

        protected override void DealFreshData()
        {
            base.DealFreshData();
            string desc;
            ParseDicData.TryGetValueWitheKey(out desc, SocialTools.KeyMessageContent);
            bool status;
            ParseDicData.TryGetValueWitheKey(out status, SocialTools.KeyAgreeStatus);
            int agreeNum;
            ParseDicData.TryGetValueWitheKey(out agreeNum, SocialTools.KeyAgreeNum);
            DescText.TrySetComponentValue(desc);
            AgreeNum = agreeNum;
            AgreeNumText.TrySetComponentValue(agreeNum);
            AgreeStatus = status;
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(AgreeStatusAction.WaitExcuteCalls());
            }
        }
    }
}
