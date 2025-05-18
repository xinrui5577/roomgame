using System.Collections.Generic;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.View;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.Comments
{
    /// <summary>
    /// 亲友圈评论
    /// </summary>
    public class SocialCommentView :BaseSocialView
    {
        [Tooltip("评论内容")]
        public YxBaseLabelAdapter Content;
        [Tooltip("点赞状态")]
        [HideInInspector]
        public bool IsAgree;
        [Tooltip("点赞事件")]
        public string AgreeAction = "index/indexapi/like";

        protected override void AddListeners()
        {
            base.AddListeners();
            AddEventListener<Dictionary<string, object>>(AgreeAction, OnAgreeCall);
        }

        protected override void RemoveListeners()
        {
            RemoveEventListener<Dictionary<string, object>>(AgreeAction, OnAgreeCall);
            base.RemoveListeners();
        }

        public void ClickAgreeBtn()
        {
            if (Data is Dictionary<string, object>)
            {
                var dic = Data as Dictionary<string, object>;
                Manager.SendRequest(AgreeAction, dic);
            }
        }

        public void SendComment(string value,string defValue)
        {
            if (value.Equals(defValue)||string.IsNullOrEmpty(value))
            {
                YxMessageBox.Show(SocialTools.KeyNoticeCommitInputError);
                return;
            }
            if (Data is Dictionary<string, object>)
            {
                var dic = Data as Dictionary<string, object>;
                dic[SocialTools.KeyMessageContent] = value;
                dic[SocialTools.KeyAgree] = IsAgree?1:0;
                Manager.SendRequest(InitAction,dic);
            }
        }


        protected override void OnInitDataValid()
        {
            base.OnInitDataValid();
            YxMessageBox.Show(SocialTools.KeyNoticeSendCommentSuccess);
        }

        private void OnAgreeCall(Dictionary<string,object> data)
        {
            YxMessageBox.Show(SocialTools.KeyNoticeSendAgreeSuccess);
        }
    }
}
