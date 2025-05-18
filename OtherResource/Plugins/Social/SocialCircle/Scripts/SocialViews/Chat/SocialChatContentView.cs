using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.Chat
{
    public class SocialChatContentView:BaseSocialView
    {
        [Tooltip("Item Prefab")]
        public SocialTalkInfoItem PrefabItem;
        [Tooltip("目标名称格式")]
        public string TargetNameFormat = "【{0}】的亲友圈";
        [Tooltip("聊天对象名称（目前为聊天群名称）")]
        public YxBaseLabelAdapter TalkTargetName;
        [Tooltip("在线群成员数量")]
        public YxBaseLabelAdapter OnLineMemberCount;
        [Tooltip("Table")]
        public UITable Table;
        [Tooltip("Panel")]
        public UIPanel ShowPanel;
        [Tooltip("当前玩家发送聊天消息后移动偏移量")]
        public float SelfTalkMoveOffset = 10;
        [Tooltip("移动强度")]
        public float MoVeStrength = 1000;
        [Tooltip("初始化缓存区")]
        public UITable CacheAreaTable;
        [Tooltip("其它人未读按钮事件")]
        public List<EventDelegate> OtherUnReadBtnAction=new List<EventDelegate>();
        [Tooltip("消息间隔时间(超过对应时间后显示消息具体时间，连续未超过的时间不显示)")]
        public long MessageDelaTime=3600;
        private string _targetId;
        /// <summary>
        /// 等待刷新时间
        /// </summary>
        private float WaitFreshTime=0.2f;

        public bool ShowUnReadFlag { get; private set; }

        /// <summary>
        /// 聊天内容
        /// </summary>
        List<Dictionary<string, object>> _talkContent=new List<Dictionary<string, object>>();

        protected override void OnAwake()
        {
            ShowPanel.onClipMove = delegate
            {
                if (ShowUnReadFlag)
                {
                    FreshUnReadFlagState();
                }
            };
            base.OnAwake();
        }

        protected override void AddListeners()
        {
            base.AddListeners();
            AddEventListener<Dictionary<string, object>>(SocialTools.KeyTalkMessage, OnReceiveNewMessage);
            AddEventListener<Dictionary<string, object>>(SocialTools.KeyActionBlackPush, OnBlockedInfoChange);
            AddEventListener<Dictionary<string, object>>(SocialTools.KeyActionGetGroupMemberInfo, OnReciveMemberInfos);
            AddEventListener<Dictionary<string, object>>(SocialTools.KeyActionObserverIn, OnOnlineNumberListener);
            AddEventListener<Dictionary<string, object>>(SocialTools.KeyActionMemberNumChange, OnOnlineNumberListener);
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
            RemoveEventListener<Dictionary<string, object>>(SocialTools.KeyTalkMessage, OnReceiveNewMessage);
            RemoveEventListener<Dictionary<string, object>>(SocialTools.KeyActionBlackPush, OnBlockedInfoChange);
            RemoveEventListener<Dictionary<string, object>>(SocialTools.KeyActionGetGroupMemberInfo, OnReciveMemberInfos);
            RemoveEventListener<Dictionary<string, object>>(SocialTools.KeyActionObserverIn, OnOnlineNumberListener);
            RemoveEventListener<Dictionary<string, object>>(SocialTools.KeyActionMemberNumChange, OnOnlineNumberListener);
        }
        /// <summary>
        /// 消息队列
        /// </summary>
        private Queue<Dictionary<string,object>> _messageQueue=new Queue<Dictionary<string, object>>();

        protected override void OnInitDataValid()
        {
            if (gameObject.activeInHierarchy)
            {
                Reset();
                string targetName;
                InitGetData.TryGetValueWitheKey(out _targetId, SocialTools.KeyId);
                if (!string.IsNullOrEmpty(_targetId))
                {
                    Manager.SendRequest(SocialTools.KeyActionObserverIn, new Dictionary<string, object>()
                    {
                        {SocialTools.KeyGroupId,_targetId}
                    });
                    _messageQueue.Clear();
                    InitGetData.TryGetValueWitheKey(out targetName, SocialTools.KeyGroupName);
                    TalkTargetName.TrySetComponentValue(string.Format(TargetNameFormat, targetName));
                    TalkCenter.FriendListRedPointChange(_targetId, false);
                    List<Dictionary<string, object>> getList;
                    InitGetData.TryGetValueWitheKey(out getList, SocialTools.KeyData);
                    for (int i = 0, length = getList.Count; i < length; i++)
                    {
                        var infoItem = getList[i];
                        _messageQueue.Enqueue(infoItem);
                    }
                    TalkCenter.GetGroupMembers(_targetId,false);
                }
            }
        }

        protected void OnReciveMemberInfos(Dictionary<string,object> memberInfos)
        {
            var defDic = memberInfos.ParseDefKeyDic();
            bool isSingle;
            defDic.TryGetValueWitheKey(out isSingle, SocialTools.KeySingleItem);
            if (isSingle)
            {
                ShowItem(true);
            }
            else
            {
                ShowItem(false);
            }
        }

        protected void OnBlockedInfoChange(Dictionary<string,object> info)
        {
            int opType;
            info.TryGetValueWitheKey(out opType, SocialTools.KeyUpdateType);
            if (opType==1)
            {
                string blackImId;
                info.TryGetValueWitheKey(out blackImId, SocialTools.KeyImId);
                var blackGroupId = TalkCenter.GetBlockGroupId(blackImId);
                if (blackGroupId.Equals(_targetId))
                {
                    YxMessageBox.Show(SocialTools.KeyNoticeBlockedInfo);
                    TalkCenter.ResetTalkTarget();
                    ChangeTalkTarget(Manager.UserGroupId,this);
                }
            }
        }

        private void OnOnlineNumberListener(Dictionary<string,object> data)
        {
            string playerNum;
            data.TryGetValueWitheKey(out playerNum, SocialTools.KeyNum);
            OnLineMemberCount.TrySetComponentValue(playerNum);
        }


        private string _cacheSendId;
        private void ShowItem(bool isSingle)
        {
            if (_messageQueue.Count>0)
            {
                var newMessage = _messageQueue.Peek();
                string sendId;
                newMessage.TryGetValueWitheKey(out sendId,SocialTools.KeyMessageSendId);
                if (TalkCenter.MemberDic.ContainsKey(sendId))
                {
                    newMessage[SocialTools.KeyMemberInfo] = TalkCenter.MemberDic[sendId];
                     AddItemBySingle(isSingle, newMessage);
               
                }
                else
                {
                    if (_cacheSendId== sendId)
                    {
                        AddItemBySingle(isSingle, newMessage);
                        return;
                    }
                    _cacheSendId = sendId;
                    TalkCenter.GetGroupMembers(_targetId,isSingle);
                }
            }
            else
            {
                if (!isSingle)
                {
                    if(gameObject.activeInHierarchy)
                    StartCoroutine(WaitFreshTable());
                }
            }
        }

        private long _lastMessageTime;
        private void AddItemBySingle(bool isSingle,Dictionary<string,object> newMessage)
        {
            _messageQueue.Dequeue();
            long time;
            newMessage.TryGetValueWitheKey(out time,SocialTools.KeyTalkItemTime);
            newMessage[SocialTools.KeyShowMessageTime] = SocialTools.GetMessageTimeShow(_lastMessageTime,time,MessageDelaTime);
            if (_lastMessageTime==0)
            {
                _lastMessageTime = time;
            }
            if (isSingle)
            {
                if (_talkContent.Count == 0)
                {
                    SpringPanel.Begin(ShowPanel.gameObject, Vector3.zero, MoVeStrength);
                }
                AddItem(newMessage, true, false, delegate
                {
                    if (!ShowUnReadFlag)
                        FreshUnReadFlagState();
                });
            }
            else
            {
                AddItem(newMessage, false, true);
            }
            ShowItem(isSingle);
        }

        private void Reset()
        {
            _lastMessageTime = 0;
            _cacheSendId = string.Empty;
            FreshUnReadFlagState();
            if (CacheAreaTable)
            {
                var list = Table.GetChildList();
                for (int i = 0, length = list.Count; i < length; i++)
                {
                    var itemObj = list[i].gameObject;
                    if (itemObj)
                    {
                        CacheAreaTable.gameObject.AddChildToParent(itemObj);
                        itemObj.TrySetComponentValue(false);
                    }
                }
            }
            _talkContent.Clear();
            SpringPanel.Begin(ShowPanel.gameObject, Vector3.zero,MoVeStrength);
        }

        private void OnReceiveNewMessage(Dictionary<string,object> newMessage)
        {
             _messageQueue.Enqueue(newMessage);
             ShowItem(true);
        }


        private void AddItem(Dictionary<string, object> newMessage,bool resetItemPos=false, bool fromCache=false,TwCallBack finishCall=null)
        {
            var isSelf = IsSelfMessage(newMessage);
            newMessage[SocialTools.KeyIsSelf] = isSelf;
            if (CacheAreaTable==null|| Table==null)
            {
                return;
            }
            var viewParent = fromCache ? CacheAreaTable.transform : Table.transform;
            viewParent.GetChildView(_talkContent.Count, PrefabItem).UpdateView(newMessage);
            _talkContent.Add(newMessage);
            if (resetItemPos)
            {
                TableResetPosition(isSelf,false,finishCall);
            }
        }

        IEnumerator WaitFreshTable()
        {
            yield return new WaitForSeconds(WaitFreshTime);
            TableResetPosition(true,true);
        }

        public void TableResetPosition(bool isSelf, bool cacheMove = false, TwCallBack finishCall = null)
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(GetBounds(isSelf, cacheMove, finishCall));
            }
        }

        IEnumerator GetBounds(bool isSelf,bool cacheMove=false,TwCallBack rePositionFinish=null)
        {
            yield return new WaitForEndOfFrame();
            if (CacheAreaTable == null || Table == null)
            {
                yield break;
            }
            if (cacheMove)
            {
                CacheAreaTable.Reposition();
            }
            else
            {
                Table.Reposition();
            }

            if (rePositionFinish != null)
            {
                rePositionFinish(null);
            }

            if (isSelf)
            {
                StartCoroutine(ToBottom(cacheMove));
            }
        }

        IEnumerator ToBottom(bool cacheMove = false)
        {
            yield return new WaitForEndOfFrame();
            var cacheParent = cacheMove ? CacheAreaTable.transform : Table.transform;
            var bounds = NGUIMath.CalculateRelativeWidgetBounds(cacheParent);
            var showHeight = ShowPanel.baseClipRegion.w;
            var areaHeight = bounds.size.y;
            if (cacheMove)
            {
                var cacheList = CacheAreaTable.GetChildList();
                for (int i = 0,cacheChildCount= cacheList.Count; i < cacheChildCount; i++)
                {
                    Table.gameObject.AddChildToParent(cacheList[i].gameObject,true);
                }
                TableResetPosition(true);
            }
            if (areaHeight> showHeight)
            {
                SpringPanel.Begin(ShowPanel.gameObject, Vector3.up * (areaHeight - showHeight+ SelfTalkMoveOffset), MoVeStrength);
            }
        }
        /// <summary>
        /// 显示最新消息
        /// </summary>
        public void ShoLastMessage()
        {
            TableResetPosition(true,false, delegate
            {
                FreshUnReadFlagState();
            });
        }

        /// <summary>
        /// 刷新未读标记提示状态
        /// </summary>
        private void FreshUnReadFlagState()
        {
            ShowUnReadFlag = NeedShowUnReadFlag();
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(OtherUnReadBtnAction.WaitExcuteCalls());
            }
        }

        private bool NeedShowUnReadFlag()
        {
            var childList = Table.GetChildList();
            if (childList.Count > 0)
            {
                if (_talkContent.Count == 0)
                {
                    return false;
                }
                var lastContent = _talkContent.Last();
                if (IsSelfMessage(lastContent))
                    return false;
                var parentTrans = Table.transform;
                var showBottom = parentTrans.InverseTransformPoint(ShowPanel.worldCorners[0]);
                var lastItemPos = childList.Last().localPosition;
                return lastItemPos.y- SelfTalkMoveOffset < showBottom.y;
            }
            return false;
        }

        private bool IsSelfMessage(Dictionary<string,object> messageData)
        {
            string sendId;
            messageData.TryGetValueWitheKey(out sendId, SocialTools.KeyMessageSendId);
            return Manager.UserImId == sendId;
        }

        public override void OnDestroy()
        {
            TalkCenter.ResetTalkTarget();
            base.OnDestroy();
        }
    }
}
