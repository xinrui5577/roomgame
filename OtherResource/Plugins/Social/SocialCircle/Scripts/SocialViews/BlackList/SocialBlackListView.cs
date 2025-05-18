using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.BlackList
{
    public class SocialBlackListView : BaseSocialSelectWrapListView
    {
        protected override void OnVisible()
        {
            TalkCenter.GetBlackSortList(false,true);
        }
        public void OnClickRevert(BaseSocialWrapItem item)
        {
            var data=item.GetData<Dictionary<string, object>>();
            if (data!=null)
            {
                TalkCenter.ReFriend(data[SocialTools.KeyOwnerId].ToString());
            }
        }

        protected override void OnInitDataValid()
        {
            Dictionary<string, Dictionary<string, object>> getDic;
            InitGetData.TryGetValueWitheKey(out PageIds, SocialTools.KeyIds);
            InitGetData.TryGetValueWitheKey(out getDic, SocialTools.KeyData);
            FreshWrapList(getDic);
        }
    }
}
