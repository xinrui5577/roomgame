using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Hall.View.RecordWindows;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using Assets.Scripts.Tea.House;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.TableList
{
    /// <summary>
    /// 亲友圈确认房间牌桌信息
    /// </summary>
    public class SocialTableItem : BaseSocialWrapItem
    {
        [Tooltip("头像prefab")]
        public YxView Prefab;
        /// <summary>
        /// 牌桌名称
        /// </summary>
        public const string KeyTableName = "name";
        /// <summary>
        /// 人数上限
        /// </summary>
        public const string KeyPlayerCount = "capacity";
        /// <summary>
        /// 人物昵称集合
        /// </summary>
        public const string KeyUsers = "users";
        /// <summary>
        /// 人物昵称集合
        /// </summary>
        public const string KeyArgs = "args";
        /// <summary>
        /// gamekey
        /// </summary>
        public const string KeyGameKey = "gamekey";
        /// <summary>
        /// 人物头像信息集合
        /// </summary>
        public const string KeyAvatars = "avatars";

        [Tooltip("牌桌信息")]
        public YxBaseLabelAdapter TableName;
        [Tooltip("多人显示布局")]
        public TeaTableLayout Layout;

        private Transform _parenTrans;

        [HideInInspector]
        public string RoomId;
        [HideInInspector]
        public string Args;

        [HideInInspector]
        public string GameKey;
        protected override void OnStart()
        {
            base.OnStart();
            if (Layout)
            {
                _parenTrans = Layout.transform;
            }
        }

        protected override void DealFreshData()
        {
            base.DealFreshData();
            string tableName;
            ParseDicData.TryGetValueWitheKey(out tableName,KeyTableName);
            TableName.TrySetComponentValue(tableName);
            int playerNum;
            ParseDicData.TryGetValueWitheKey(out playerNum, KeyPlayerCount);
            ParseDicData.TryGetValueWitheKey(out RoomId, SocialTools.KeyRoomId);
            ParseDicData.TryGetValueWitheKey(out Args, KeyArgs);
            ParseDicData.TryGetValueWitheKey(out GameKey, KeyGameKey);
            List<string> nickList =ParseDicData[KeyUsers].Copy<List<string>>();
            List<string> avatarsList = ParseDicData[KeyAvatars].Copy<List<string>>();
            List<double> imIdList = ParseDicData[SocialTools.KeyImId].Copy<List<double>>();
            List<Dictionary<string, object>> playerData = new List<Dictionary<string, object>>();
            var nickListLength=nickList.Count;
            for (int i = 0; i < nickListLength; i++)
            {
                playerData.Add(new Dictionary<string, object>()
                {
                    { HeadData.KeyName,nickList[i]},{ HeadData.KeyAvatar,avatarsList[i]},
                    { SocialTools.KeyId,imIdList[i]},{ SocialTools.KeyImId,imIdList[i]},
                });
            }
            ShowHeadItem(playerNum, playerData);
        }

        private void ShowHeadItem(int length, List<Dictionary<string, object>> userInfo)
        {
            if (Layout)
            {
                var dataLength = userInfo.Count;
                for (int i = 0; i < dataLength; i++)
                {
                    var childView=_parenTrans.GetChildView(i, Prefab);
                    if (childView)
                    {
                        childView.UpdateView(userInfo[i]);
                    }
                }

                for (int i= dataLength,childCount=transform.childCount; i< childCount;i++)
                {
                    _parenTrans.GetChildView(i, Prefab).gameObject.TrySetComponentValue(false);
                }
                Layout.SetLayoutByNum(length);
            }
        }
    }
}
