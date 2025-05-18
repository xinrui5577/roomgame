using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.RequestList
{
    public class SocialRequestListView : BaseSocialWrapListView
    {
        protected override void AddListeners()
        {
            AddEventListener<Dictionary<string, object>>(InitAction, OnInitReceive);
            AddEventListener<Dictionary<string, object>>(PartAction, OnReciveOpResult);
        }

        protected override void RemoveListeners()
        {
            RemoveEventListener<Dictionary<string, object>>(InitAction, OnInitReceive);
            RemoveEventListener<Dictionary<string, object>>(PartAction, OnReciveOpResult);
        }

        protected override void OnInitDataValid()
        {
            Dictionary<string, Dictionary<string, object>> getDic;
            InitGetData.TryGetValueWitheKey(out PageIds, SocialTools.KeyIds);
            InitGetData.TryGetValueWitheKey(out getDic, SocialTools.KeyData);
            FreshWrapList(getDic);
        }

        protected override void OnVisible()
        {
            TalkCenter.GetFriendRequestList();
        }

        private void OnReciveOpResult(Dictionary<string,object> resutlt)
        {
            var defDic = resutlt.ParseDefKeyDic();
            if (defDic != null)
            {
                TalkCenter.ChangeFriendRequestData(defDic[SocialTools.KeyBoxId].ToString(), resutlt[SocialTools.KeyHandles].ToString());
            }
            
            TalkCenter.GetFriendRequestList(true,true);
        }

        /// <summary>
        /// 点击同意按钮
        /// </summary>
        public void OnClickAgree(SocialRequestItem item)
        {
            SendRequestOp(item.OnlyId,1);
        }
        /// <summary>
        /// 点击忽略按钮
        /// </summary>
        /// <param name="item"></param>
        public void OnClickIgnore(SocialRequestItem item)
        {
            SendRequestOp(item.OnlyId,0);
        }
        /// <summary>
        /// 发送请求操作
        /// </summary>
        /// <param name="onlyId"></param>
        /// <param name="replyData"></param>
        private void SendRequestOp(string onlyId,int replyData)
        {
            var dataDIc = new Dictionary<string, object>()
            {
                {SocialTools.KeyBoxId,onlyId}, {SocialTools.KeyReply, replyData}
            };
            Manager.SendRequest(PartAction, dataDIc.SetDefKeyDic(dataDIc.Copy<Dictionary<string,object>>()));
        }
    }
}
