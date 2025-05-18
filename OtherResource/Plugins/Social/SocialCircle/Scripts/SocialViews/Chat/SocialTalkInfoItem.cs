using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.Chat
{
    public class SocialTalkInfoItem : YxView
    {
        [Tooltip("聊天对象是否为当前玩家")]
        public List<EventDelegate> IsSelfAction=new List<EventDelegate>();
        [HideInInspector]
        [Tooltip("是否为当前玩家")]
        public bool IsSelf;
        [Tooltip("是否需要显示时间")]
        public bool ShowTime;
        [Tooltip("消息时间文本")]
        public YxBaseLabelAdapter MessageTimeLabel;

        private Dictionary<string, object> _contentData;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if(Data is IDictionary)
            {
                _contentData = Data as Dictionary<string, object>;
                if (_contentData != null)
                {
                    _contentData.TryGetValueWitheKey(out IsSelf, SocialTools.KeyIsSelf);
                    string messageTime;
                    _contentData.TryGetValueWitheKey(out messageTime, SocialTools.KeyShowMessageTime);
                    ShowTime =!string.IsNullOrEmpty(messageTime);
                    if (ShowTime)
                    {
                        MessageTimeLabel.TrySetComponentValue(messageTime);
                    }
                    if (gameObject.activeInHierarchy)
                    {
                        StartCoroutine(IsSelfAction.WaitExcuteCalls());
                    }
                }
            }
        }

        public void FreshContent(SocialContentItem selectContent)
        {
            if (selectContent.gameObject.activeInHierarchy)
            {
                selectContent.UpdateView(_contentData);
            }
        }
    }
}
