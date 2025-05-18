using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.Rank
{
    /// <summary>
    /// 热度排行列表
    /// </summary>
    public class SocialRankListView : BaseSocialSelectWrapListView
    {
        protected override void OnVisible()
        {
            if (Manager.EntryNum == 0)//亲友圈首次进入
            {
                Manager.SendRequest(SocialTools.KeyActionEntryAdd);
            }
            base.OnVisible();
        }

        protected override void AddListeners()
        {
            AddEventListener<Dictionary<string, object>>(InitAction, OnInitReceive);
            AddEventListener<Dictionary<string, object>>(PartAction, OnPart);
            AddEventListener<Dictionary<string, object>>(SocialTools.KeyActionEntryAdd, OnEntryNumChange); 
            AddEventListener<Dictionary<string, object>>(SocialTools.KeyActionGetGroupList, OnGroupInfoChange); 
        }

        protected override void RemoveListeners()
        {
            RemoveEventListener<Dictionary<string, object>>(InitAction, OnInitReceive);
            RemoveEventListener<Dictionary<string, object>>(PartAction, OnPart);
            RemoveEventListener<Dictionary<string, object>>(SocialTools.KeyActionEntryAdd, OnEntryNumChange);
            RemoveEventListener<Dictionary<string, object>>(SocialTools.KeyActionGetGroupList, OnGroupInfoChange);
        }

        protected override void OnInitDataValid()
        {
            InitGetData.TryGetStringListWithKey(out PageIds,SocialTools.KeyIds);
            TalkCenter.GetRankList(PageIds,PartAction);
        }

        private void OnPart(Dictionary<string, object> data)
        {
            Dictionary<string, Dictionary<string, object>> getDic;
            data.TryGetValueWitheKey(out getDic, SocialTools.KeyData);
            FreshWrapList(getDic);
        }
        protected void OnEntryNumChange(Dictionary<string, object> entryTime)
        {
            Manager.EntryNum++;
        }

        private void OnGroupInfoChange(Dictionary<string, object> info)
        {
            OnVisible();
        }
    }
}