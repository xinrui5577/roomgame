using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.DataBundles;
namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.UserInfo
{
    public class SocialUserInfoView : BaseSocialView
    {
        [Tooltip("请求")]
        public string Action = "im.userData";
        [Tooltip("头像")]
        public YxBaseTextureAdapter HeadTex;
        [Tooltip("玩家UserID")]
        public YxBaseLabelAdapter UserId;
        [Tooltip("昵称")]
        public YxBaseLabelAdapter UserNick;
        [Tooltip("来源名称")]
        public YxBaseLabelAdapter SourceName;
        [Tooltip("金币数量")]
        public YxBaseLabelAdapter GoldNum;
        [Tooltip("房卡数量")]
        public YxBaseLabelAdapter CardNum;
        [Tooltip("亲友数量")]
        public YxBaseLabelAdapter FriendNum;
        [Tooltip("礼物数量")]
        public YxBaseLabelAdapter GiftNum;
        [Tooltip("点赞数量")]
        public YxBaseLabelAdapter AgreeNum;
        [Tooltip("评论数量")]
        public YxBaseLabelAdapter CommentNum;
        [Tooltip("黑名单数量")]
        public YxBaseLabelAdapter BlackNum;
        [Tooltip("回调列表")]
        public List<EventDelegate> CallBackList=new List<EventDelegate>();
        /// <summary>
        /// im id
        /// </summary>
        public string UserImId
        {
            get;private set;
        }

        /// <summary>
        /// 来源名称数据
        /// </summary>
        private string _sourceName;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            ReFreshView();
        }

        private void RequestUserData(Dictionary<string,object> requestData)
        {
            CurTwManager.SendAction(Action, requestData, delegate(object result)
            {
                Dictionary<string, object> parseDic = (Dictionary<string, object>) result;
                if (parseDic!=null)
                {
                    ShowInfos(parseDic);
                }
            },true,null,false);
        }

        private void ShowInfos(Dictionary<string, object> data)
        {
            var headData = new SocialHeadData(data);
            if (HeadTex)
            {
                PortraitDb.SetPortrait(headData.HeadUrl, HeadTex,1);//默认性别男(数据中没有性别相关,因此使用默认值来处理!)
            }
            UserId.TrySetComponentValue(headData.UserId);
            UserNick.TrySetComponentValue(headData.UserName);
            SourceName.TrySetComponentValue(string.IsNullOrEmpty(_sourceName) ? headData.SourceName : _sourceName);
            GoldNum.TrySetComponentValue(Convert.ToInt64(headData.CoinNum),"1");
            CardNum.TrySetComponentValue(headData.CashNum);
            FriendNum.TrySetComponentValue(headData.FriendNum);
            GiftNum.TrySetComponentValue(headData.GiftNum);
            AgreeNum.TrySetComponentValue(headData.LikeNum);
            CommentNum.TrySetComponentValue(headData.CommentNum);
            BlackNum.TrySetComponentValue(headData.BlackNum);
        }
        /// <summary>
        /// 刷新界面
        /// </summary>
        public void ReFreshView()
        {
            if (Data != null && Data is IDictionary)
            {
                var dic = (Dictionary<string, object>)Data;
                if (dic != null)
                {
                    UserImId = dic[SocialTools.KeyImId].ToString();
                    dic.TryGetValueWitheKey(out _sourceName, SocialTools.KeySourceName);
                    RequestUserData(dic);
                }
            }
        }
    }
}
