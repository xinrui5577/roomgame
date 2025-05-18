using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base;
using UnityEngine;
using YxFramwork.Common.Adapters;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.Rank
{
    /// <summary>
    /// 热度排行item
    /// </summary>
    public class SocialRankListItem : BaseSocialSelectWrapItem
    {
        [Tooltip("排行名次文本")]
        public YxBaseLabelAdapter RankNumLabel;
        [Tooltip("排行名次图片")]
        public YxBaseSpriteAdapter RankNumSprite;
        [Tooltip("对应名次内显示特片格式的排行")]
        public int RankImageLenth = 3;
        [Tooltip("排行图片格式")]
        public string RankFormat = "Rank_{0}";
        [Tooltip("显示图片事件")]
        public List<EventDelegate> ShowImageAction=new List<EventDelegate>();
        /// <summary>
        /// 是否显示排行图片
        /// </summary>
        public bool ShowRankImage
        {
            private set;
            get;
        }

        protected override void DealFreshData()
        {
            base.DealFreshData();
            ShowRankImage = IdCode < RankImageLenth;
            if (ShowRankImage)
            {
                RankNumSprite.TrySetComponentValue(string.Format(RankFormat, IdCode + 1));
            }
            else
            {
                RankNumLabel.TrySetComponentValue(IdCode + 1);
            }

            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(ShowImageAction.WaitExcuteCalls());
            }
        }

    }
}
