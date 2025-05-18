using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using UnityEngine;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base
{
    /// <summary>
    /// 带头像信息相关Item数据
    /// </summary>
    public class BaseSocialWrapHeadItem : BaseSocialWrapItem
    {
        [Tooltip("头像点击事件")]
        public List<EventDelegate> HeadClickAction = new List<EventDelegate>();
        [Tooltip("头像")]
        public BaseSocialHeadItem Head;

        public string UserImId{ protected set; get;}

        protected override void DealFreshData()
        {
            base.DealFreshData();
            if (Head)
            {
                var headData = ParseDicData.Copy<Dictionary<string,object>>();
                var socialData = new SocialHeadData(headData);
                var oldHeadData = Head.GetData<SocialHeadData>();
                if (oldHeadData.ValueEqual(socialData))
                {
                    return;
                }
                Head.UpdateView(socialData);
                UserImId = socialData.ImId;
            }
        }

        public virtual void OnClick()
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(HeadClickAction.WaitExcuteCalls());
            }
        }

    }
}
