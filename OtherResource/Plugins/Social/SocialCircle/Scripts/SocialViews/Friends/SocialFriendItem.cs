using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Manager;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.View;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.Friends
{
    public class SocialFriendItem : BaseSocialSelectWrapItem
    {
        [Tooltip("消息读取状态")]
        public List<EventDelegate> MessageReadStatus = new List<EventDelegate>();
        [Tooltip("当前玩家状态")]
        public List<EventDelegate> SelfStatusAction = new List<EventDelegate>();
        private SocialMessageManager _manager;
        private SocialMessageManager Manager
        {
            get
            {
                if (_manager == null)
                {
                    _manager = Facade.Instance<SocialMessageManager>().InitManager();
                }
                return _manager;
            }
        }

        private SocialTalkCenter _center;
        protected SocialTalkCenter TalkCenter
        {
            get
            {
                if (_center == null)
                {
                    _center = Facade.Instance<SocialTalkCenter>().InitCenter();
                }
                return _center;
            }
        }
        protected override void OnAwake()
        {
            base.OnAwake();
            Manager.AddEventListeners<Dictionary<string,object>>(SocialTools.KeyActionBlackPush,OnBlockInfoChange);
        }

        public override void OnDestroy()
        {
            Manager.RemoveEventListeners<Dictionary<string, object>>(SocialTools.KeyActionBlackPush,OnBlockInfoChange);
            base.OnDestroy();
        }

        /// <summary>
        /// 未读消息状态
        /// </summary>
        public bool HaveNewMessage
        {
            private set;
            get;
        }

        public bool IsSelfItem
        {
            get { return OnlyId == Manager.UserGroupId; }
        }

        public bool IsBlocked
        {
            get
            {
                return ParseDicData.ContainsKey(SocialTools.KeyOwnerId) && TalkCenter.BlackBlockedList.Contains(ParseDicData[SocialTools.KeyOwnerId].ToString());
            }
        }

        public override void OnClick()
        {
            if (SelectType == (int) SelectItemState.MulSelect)
            {
                if (IsSelfItem)//多选状态过滤选中自己
                {
                    return;
                }
                if (IsBlocked)//被对应玩家拉黑
                {
                    YxMessageBox.Show(SocialTools.KeyNoticeBlockedInfo);
                    return;
                }
            }
            base.OnClick();
        }

        protected override void DealFreshData()
        {
            base.DealFreshData();
            if (gameObject.activeInHierarchy)
            {
                bool readStatus;
                ParseDicData.TryGetValueWitheKey(out readStatus,SocialTools.KeyMessageUnRead);
                HaveNewMessage = readStatus;
                StartCoroutine(MessageReadStatus.WaitExcuteCalls());
                StartCoroutine(SelfStatusAction.WaitExcuteCalls());
            }
        }

        private void OnBlockInfoChange(Dictionary<string,object> data)
        {
            if (SelectStatus&& SelectType == (int)SelectItemState.MulSelect&&IsBlocked)
            {
                base.OnClick();
            }
        } 
    }
}
