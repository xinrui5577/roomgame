using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.Comments
{
    /// <summary>
    /// 评论列表
    /// </summary>
    public class SocialCommentsListView : BaseSocialWrapListView
    {
        public void OnClickAgreeBtn(SocialCommentsItem item)
        {
            Manager.SendRequest(PartAction,new Dictionary<string, object>()
            {
                { SocialTools.KeyCommentId,item.OnlyId}
            });
            var data = item.GetData() as Dictionary<string,object>;
            var changeState = !item.AgreeStatus;
            if (data != null)
            {
                data[SocialTools.KeyAgreeStatus] = changeState;
                data[SocialTools.KeyAgreeNum] = item.AgreeNum + (changeState ? 1 : -1);
                IdsDataDic[item.OnlyId] = data;
                item.UpdateView(data);
            }
        }
        protected override void OnVisible()
        {
            RequestWithPage(0);
        }

        private int _pageTotalCount;
        Dictionary<string, Dictionary<string, object>> cacheDic = new Dictionary<string, Dictionary<string, object>>();
        protected override void OnInitDataValid()
        {
            var isFirst = PageIds.Count == 0;
            if (isFirst)
            {
                cacheDic.Clear();
                _pageTotalCount = 0;
                InitGetData.TryGetValueWitheKey(out _pageTotalCount, SocialTools.KeyTotalSize);
            }
            int pageIndex;
            InitGetData.TryGetValueWitheKey(out pageIndex, SocialTools.KeyPage);
            List<object> comments;
            InitGetData.TryGetValueWitheKey(out comments, SocialTools.KeyComments);
            var receiveLenth = comments.Count;
            for (int i = 0; i < receiveLenth; i++)
            {
                var itemDic = comments[i] as Dictionary<string, object>;
                if (itemDic!=null)
                {
                    int key;
                    itemDic.TryGetValueWitheKey(out key, SocialTools.KeyId,int.MinValue);
                    if (key!=int.MinValue)
                    {
                        PageIds.Add(key.ToString());
                        cacheDic.Add(key.ToString(), itemDic);
                    }
                }
            }
            FreshWrapList(cacheDic);
            if (PageIds.Count < _pageTotalCount&& pageIndex*6< _pageTotalCount)
            {
                RequestWithPage(pageIndex + 1);
            }
        }

        private void RequestWithPage(int page)
        {
            if(Data is IDictionary)
            {
                var parDic = Data as Dictionary<string, object>;
                var sendDic=new Dictionary<string, object>();
                sendDic[SocialTools.KeyPage] = page;
                if (parDic != null) sendDic[SocialTools.KeyImId] = parDic[SocialTools.KeyImId];
                Manager.SendRequest(SocialTools.KeyActionCommentsList, sendDic);
            }
        }
    }
}
