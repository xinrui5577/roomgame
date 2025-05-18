using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using YxFramwork.View;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.AddFriend
{
    /// <summary>
    /// 选择要添加的亲友
    /// </summary>
    public class SocialSelectAddFriendView : SocialAddFriendView
    {
        protected override void OnVisible()
        {

        }

        protected override string GetIdsKey()
        {
            return SocialTools.KeyStrangef;
        }

        protected override void CheckVaild()
        {
            base.CheckVaild();
            GetVaildList(TalkCenter.GetBlackListByImId());
            GetVaildList(TalkCenter.GetGroupListByImId());
        }

        /// <summary>
        /// 获取有效列表
        /// </summary>
        /// <param name="checkList"></param>
        private void GetVaildList(List<string> checkList)
        {
            for (int i = 0,length= checkList.Count; i < length; i++)
            {
                var checkKey = checkList[i];
                if (PageIds.Contains(checkKey))
                {
                    PageIds.Remove(checkKey);
                }
            }
        }

        public void SendAddRequest()
        {
            var ids = GetSelectItems();
            if (ids.Count==0)
            {
                YxMessageBox.Show(SocialTools.KeyNoticeSelectEmpty);
                return;
            }
            if (Data is IDictionary)
            {
                var dic = Data as Dictionary<string, object>;
                if (dic != null)
                {
                    dic[SocialTools.KeyIds] = ids;
                    dic[SocialTools.KeySourceId] = dic[SocialTools.KeyImId];
                    Manager.SendRequest(SocialTools.KeyActionAddFriend, dic);
                }
            }
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (Data is IDictionary)
            {
                var dic = Data as Dictionary<string, object>;
                if (dic!=null)
                {
                    RequestWithSortType(dic);
                }
            }
        }

        public override void OnClickSearch()
        {
            RequestWithSortType(Data as Dictionary<string, object>);
        }
    }
}
