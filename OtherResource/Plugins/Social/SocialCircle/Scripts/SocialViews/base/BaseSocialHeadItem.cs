using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Hall.View.RecordWindows;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using UnityEngine;
using YxFramwork.Common.Adapters;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base
{
    /// <summary>
    /// 继承于HeadItem 处理显示人物在线状态相关显示
    /// </summary>
    public class BaseSocialHeadItem : HeadItem
    {
        [Tooltip("在线状态事件")]
        public List<EventDelegate> OnLineStateAction=new List<EventDelegate>();
        [Tooltip("游戏中状态事件")]
        public List<EventDelegate> OnGamingStateAction = new List<EventDelegate>();
        [Tooltip("游戏信息")]
        public YxBaseLabelAdapter GamingInfoLabel;
        /// <summary>
        /// 在线状态
        /// </summary>
        public bool OnLineState { get; private set;}
        /// <summary>
        /// 游戏中状态
        /// </summary>
        public bool OnGamingState { get; private set; }

        public string GroupId { get;protected set; }

        public string ImId { get; protected set; }

        public string UserId { get; protected set; }

        private UITexture _headTexture;

        protected override void OnAwake()
        {
            if (ShowHead)
            {
                _headTexture = ShowHead.GetComponent<UITexture>();
            }
            base.OnAwake();
        }

        protected override void RefreshItem(HeadData data)
        {
            base.RefreshItem(data);
            var socialData = data as SocialHeadData;
            if (socialData != null)
            {
                if (socialData.ExistOnlineData)
                {
                    OnLineState = socialData.IsOnline;
                    if (gameObject.activeInHierarchy)
                    {
                        StartCoroutine(OnLineStateAction.WaitExcuteCalls());
                    }
                }
                if (socialData.ExistInGameData)
                {
                    OnGamingState = socialData.IsGameing;
                    if (gameObject.activeInHierarchy)
                    {
                        StartCoroutine(OnGamingStateAction.WaitExcuteCalls());
                    }
                }
                SetUserIds(socialData);
                GamingInfoLabel.TrySetComponentValue(socialData.GameingInfo);
            }
        }

        protected virtual void SetUserIds(SocialHeadData data)
        {
            ImId = data.ImId;
            UserId = data.UserId;
            GroupId = data.GroupId;
        }

        /// <summary>
        /// 玩家离线
        /// </summary>
        public void OutLine()
        {
            if (_headTexture)
            {
                _headTexture.ColorType = UIRect.EColorType.Gray;
            }
        }
        /// <summary>
        /// 玩家在线
        /// </summary>
        public void OnLine()
        {
            if (_headTexture)
            {
                _headTexture.ColorType = UIRect.EColorType.Normal;
            }
        }


        public void OnChangeNikc(bool state,int defWidth,int changeWidth,GameObject attached)
        {
            if (state)
            {
                ShowUserName.width = changeWidth;
                attached.SetActive(false);
            }
            else
            {
                ShowUserName.width = defWidth;
                attached.SetActive(true);
            }

        }
    }

    /// <summary>
    /// 包括玩家是否在线状态，玩家是否游戏中状态
    /// </summary>
    public class SocialHeadData:HeadData
    {
        /// <summary>
        /// 在线状态
        /// </summary>
        public const string KeyOnLineState = "state";
        /// <summary>
        /// 游戏状态状态
        /// </summary>
        public const string KeyGamingState = "in_game";
        /// <summary>
        /// 玩家 昵称
        /// </summary>
        public const string KeyNickM = "nick_m";
        /// <summary>
        /// 玩家id
        /// </summary>
        public const string KeyUserId = "user_id";
        /// <summary>
        /// 玩家元宝
        /// </summary>
        public const string KeyCashNum= "cash_a";
        /// <summary>
        /// 玩家金币
        /// </summary>
        public const string KeyCoinNum = "coin_a";
        /// <summary>
        /// 玩家im id
        /// </summary>
        public const string KeyImId = "im_id";
        /// <summary>
        /// 玩家group id
        /// </summary>
        public const string KeyGroupId = "group_id";
        /// <summary>
        /// 玩家亲友数量
        /// </summary>
        public const string KeyFriendNun = "friend";
        /// <summary>
        /// 玩家礼物数量
        /// </summary>
        public const string KeyGiftNum = "gift";
        /// <summary>
        /// 玩家点赞数量
        /// </summary>
        public const string KeyLikeNum = "like";
        /// <summary>
        /// 玩家评论数量
        /// </summary>
        public const string KeyCommentNum = "comment";
        /// <summary>
        /// 玩家黑名单数量
        /// </summary>
        public const string KeyBlackNum = "black";
        /// <summary>
        /// 在线
        /// </summary>
        public const string KeyStateOnLine = "online";
        /// <summary>
        /// 离线
        /// </summary>
        public const string KeyStateOffline = "offline";
        /// <summary>
        /// 是否存在在线状态数据
        /// </summary>
        public bool ExistOnlineData { private set; get; }
        /// <summary>
        /// 是否存在游戏状态数据
        /// </summary>
        public bool ExistInGameData { private set; get; }
        /// <summary>
        /// ImId
        /// </summary>
        public string ImId { private set; get; }
        /// <summary>
        /// GroupId
        /// </summary>
        public string GroupId { private set; get; }
        /// <summary>
        /// 元宝数量
        /// </summary>
        public string CashNum { private set; get; }
        /// <summary>
        /// 金币数量
        /// </summary>
        public string CoinNum { private set; get; }
        /// <summary>
        /// 亲友数量
        /// </summary>
        public string FriendNum { private set; get; }
        /// <summary>
        /// 亲友源
        /// </summary>
        public string SourceName { private set; get; }
        /// <summary>
        /// 礼物数量
        /// </summary>
        public string GiftNum { private set; get; }
        /// <summary>
        /// 黑名单数量
        /// </summary>
        public string BlackNum { private set; get; }
        /// <summary>
        /// 评论数量
        /// </summary>
        public string CommentNum { private set; get; }
        /// <summary>
        /// 点赞数量
        /// </summary>
        public string LikeNum { private set; get; }
        /// <summary>
        /// 是否在线
        /// </summary>
        public bool IsOnline { private set; get; }
        /// <summary>
        /// 是否游戏中
        /// </summary>
        public bool IsGameing {
            get { return !string.IsNullOrEmpty(GameingInfo); }
        }

        /// <summary>
        /// 游戏中状态信息
        /// </summary>
        public string GameingInfo
        {
            private set; get;
        }

        public SocialHeadData(Dictionary<string, object> dic) : base(dic)
        {

        }

        public SocialHeadData(string id, object data) : base(id, data)
        {

        }

        protected override void DealInfo(Dictionary<string, object> dic)
        {
            base.DealInfo(dic);
            if (dic.ContainsKey(KeyOnLineState))
            {
                string isOnLine;
                dic.TryGetValueWitheKey(out isOnLine, KeyOnLineState);
                IsOnline = isOnLine== KeyStateOnLine;
                ExistOnlineData = true;
            }
            else
            {
                ExistOnlineData = false;
            }
            if (dic.ContainsKey(KeyGamingState))
            {
                string gameInfo;
                dic.TryGetValueWitheKey(out gameInfo, KeyGamingState);
                GameingInfo = gameInfo;
                ExistInGameData = true;
            }
            else
            {
                ExistInGameData = false;
            }
            if (dic.ContainsKey(SocialTools.KeyNickName))
            {
                dic.TryGetValueWitheKey(out _userName, SocialTools.KeyNickName);
            }
            if (dic.ContainsKey(KeyNickM))
            {
                dic.TryGetValueWitheKey(out _userName, KeyNickM);
            }
            if (dic.ContainsKey(SocialTools.KeyGroupName))
            {
                dic.TryGetValueWitheKey(out _userName, SocialTools.KeyGroupName);
            }
            if (dic.ContainsKey(KeyImId))
            {
                dic.TryGetValueWitheKey(out _userId, KeyUserId);
                string imId;
                dic.TryGetValueWitheKey(out imId, KeyImId);
                string cashNum;
                dic.TryGetValueWitheKey(out cashNum, KeyCashNum);
                string coinNum;
                dic.TryGetValueWitheKey(out coinNum, KeyCoinNum);
                string friendNum;
                dic.TryGetValueWitheKey(out friendNum, KeyFriendNun);
                string giftNum;
                dic.TryGetValueWitheKey(out giftNum, KeyGiftNum);
                string commentNum;
                dic.TryGetValueWitheKey(out commentNum, KeyCommentNum);
                string likeNum;
                dic.TryGetValueWitheKey(out likeNum, KeyLikeNum);
                string blackNum;
                dic.TryGetValueWitheKey(out blackNum, KeyBlackNum);
                string groupId;
                dic.TryGetValueWitheKey(out groupId, KeyGroupId);
                string sourceName;
                dic.TryGetValueWitheKey(out sourceName, SocialTools.KeySourceName,"无");
                ImId = imId;
                CashNum = cashNum;
                CoinNum = coinNum;
                FriendNum = friendNum;
                GiftNum = giftNum;
                CommentNum = commentNum;
                LikeNum = likeNum;
                BlackNum = blackNum;
                GroupId = groupId;
                SourceName = sourceName;
            }
            if (dic.ContainsKey(SocialTools.KeyOwnerId))
            {
                string imId;
                dic.TryGetValueWitheKey(out imId, SocialTools.KeyOwnerId);
                ImId = imId;
            }
        }
    }
}
