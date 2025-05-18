using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.Model;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.View;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.Friends
{
    public class SocialFriendView : BaseSocialSelectWrapListView
    {
        [Tooltip("好友数量")]
        public YxBaseLabelAdapter FriendNum;
        [Tooltip("监听房间创建事件")]
        public bool ListenCreateAction=false;
        [Tooltip("邀请好友玩牌接口")]
        public string InviteAction = "mahjongwm.inviteWmFriendsComplex";
        [Tooltip("是否默认选中当前玩家Item")]
        public bool SelfItemSelect = false;
        [Tooltip("是否隐藏当前玩家Item")]
        public bool HideSelfItem = false;
        /// <summary>
        /// 邀请Id（数组）
        /// </summary>
        public const string KeyInvitedIds= "inviteId";
        /// <summary>
        /// 房间号
        /// </summary>
        public const string KeyRoomId = "roomId";

        protected override void AddListeners()
        {
            AddEventListener<Dictionary<string, object>>(InitAction, OnInitReceive);
            AddEventListener<bool>(SocialTools.KeyActionGroupListUpdate, OnUpdateGroupList);
            if (ListenCreateAction)
            {
                Facade.EventCenter.AddEventListeners<YxESysEventType, CreateRoomData>(YxESysEventType.SysCreateRoom, OnCreateRoomFinish);
            }
        }

        protected override void RemoveListeners()
        {
            RemoveEventListener<Dictionary<string, object>>(InitAction, OnInitReceive);
            RemoveEventListener<bool>(SocialTools.KeyActionGroupListUpdate, OnUpdateGroupList);
            if (ListenCreateAction)
            {
                Facade.EventCenter.RemoveEventListener<YxESysEventType, CreateRoomData>(YxESysEventType.SysCreateRoom, OnCreateRoomFinish);
            }
        }

        protected override void OnInitDataValid()
        {
            List<string> getList;
            Dictionary<string, Dictionary<string, object>> getDic;
            InitGetData.TryGetValueWitheKey(out getList, SocialTools.KeyIds);
            InitGetData.TryGetValueWitheKey(out getDic, SocialTools.KeyData);
            if (HideSelfItem)
            {
                if (getList.Contains(Manager.UserGroupId))
                {
                    getList.Remove(Manager.UserGroupId);
                    if (getDic.ContainsKey(Manager.UserGroupId))
                    {
                        getDic.Remove(Manager.UserGroupId);
                    }
                }
            }
            if (getList.Count==0)
            {
                PageIds = getList.ToList();
                FreshWrapList(new Dictionary<string, Dictionary<string, object>>());
            }
            else if (!getList.ValueEqual(PageIds) || !getDic.ValueEqual(IdsDataDic))
            {
                PageIds = getList.ToList();
                FreshWrapList(getDic);
                if (!HideSelfItem&&SelfItemSelect)
                {
                    SelectFirstItem();
                }
            }
            FriendNum.TrySetComponentValue(Math.Max(TalkCenter.SortGroupList.Count - 1, 0));
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            _isFirst = true;
        }
        /// <summary>
        /// 界面打开标识
        /// </summary>
        private bool _isFirst;

        private void SelectFirstItem()
        {
            if (_isFirst)
            {
                if (PageIds.Count>0)
                {
                    if (IdsDataDic.ContainsKey(Manager.UserGroupId)&&SelectState== SelectItemState.SingleSelect)
                    {
                        OnItemSelectChange(Manager.UserGroupId, true);
                        _isFirst = false;
                    }

                }
            }
        }

        /// <summary>
        /// 界面收到排序变化事件
        /// </summary>
        /// <param name="needRequest"></param>
        private void OnUpdateGroupList(bool needRequest)
        {
            TalkCenter.GetSortList(needRequest,true);
        }

        protected override void OnVisible()
        {
            TalkCenter.GetSortList(false,true);
        }

        public void SendInvited()
        {
            var dic = Data as Dictionary<string, object>;
            if (dic!=null)
            {
                int roomId;
                dic.TryGetValueWitheKey(out roomId, SocialTools.KeyRoomId);
                if (roomId>0)
                {
                    SendInviteAction(roomId,true);
                }
            }
 
        }

        private void OnCreateRoomFinish(CreateRoomData roomInfo)
        {
            if (roomInfo.Rndld > 0)
            {
                SendInviteAction(roomInfo.Rndld);
            }
        }

        private void SendInviteAction(int roomId,bool showMessage=false)
        {
            var ids = GetSelectItems(SocialTools.KeyOtherId);
            if (ids.Count==0)
            {
                return;
            }
            CurTwManager.SendAction(InviteAction, new Dictionary<string, object>()
            {
                {KeyInvitedIds,ids}, {KeyRoomId, roomId}
            }, null, showMessage);
        }

        public override void OnClickHead(string userId, string windowName = "SocialOtherUserInfoWindow", string socialSourceName = "", List<EventDelegate> callBack = null)
        {
            if (TalkCenter.BlackBlockedList.Contains(userId))
            {
                YxMessageBox.Show(SocialTools.KeyNoticeBlockedInfo);
                return;
            }
            base.OnClickHead(userId, windowName, socialSourceName, callBack);
        }

        [ContextMenu("测试牌局中其它玩家相册")]
        public void OnOpenSocialSelectPhotoWindow()
        {
            var dic=new Dictionary<string,object>();
            List<string> otherUserId=new List<string>()
            {
                "200208","200484","200204"
            };
            dic[SocialTools.KeyData] = otherUserId;
            MainYxView.OpenWindowWithData("SocialPhotoSelectWindow", dic);
        }
    }

}