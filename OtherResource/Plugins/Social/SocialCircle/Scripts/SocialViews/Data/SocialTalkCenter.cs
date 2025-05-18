using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Manager;
using Assets.Scripts.Hall.View.RecordWindows;
using com.yxixia.utile.Utiles;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.View;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data
{
    /// <summary>
    /// 亲友圈聊天数据中心（依赖于群组结构的模型）
    /// </summary>
    public class SocialTalkCenter:BaseMono
    {
        /// <summary>
        /// 排序顺序列表（排序列表）(群Id)
        /// </summary>
        public List<string> SortGroupList=new List<string>();

        /// <summary>
        /// 好友请求列表
        /// </summary>
        public List<string> FriendRequestList = new List<string>();

        /// <summary>
        /// 添加好友排序列表(群Id)
        /// </summary>
        public List<string> AddFriendSortGroupList = new List<string>();
        /// <summary>
        /// 黑名单列表(群Id)
        /// </summary>
        public List<string> BlackGroupList = new List<string>();
        /// <summary>
        /// 被拉黑列表请求
        /// </summary>
        public List<string> BlackBlockedList = new List<string>();
        /// <summary>
        /// 排行列表
        /// </summary>
        private List<string> _rankList = new List<string>();
        /// <summary>
        /// 群信息集合
        /// 主键：聊天对象ID
        /// 数据：玩家信息，聊天对象类型（群或者单独玩家），未读消息
        /// </summary>
        private Dictionary<string,Dictionary<string,object>>_groupInfoDic=new Dictionary<string, Dictionary<string, object>>();
        /// <summary>
        /// 群信息集合
        /// 主键：imId(owner_id)
        /// 数据：玩家信息，聊天对象类型（群或者单独玩家），未读消息
        /// </summary>
        private Dictionary<string,Dictionary<string,object>>_groupInfoDicByImId=new Dictionary<string, Dictionary<string, object>>();
        /// <summary>
        /// 聊天消息ID集合
        /// 主键：聊天对象ID
        /// 数据：聊天对象消息ID集合（排序顺序）
        /// </summary>
        private Dictionary<string, List<Dictionary<string,object>>> _talkListDic = new Dictionary<string, List<Dictionary<string,object>>>();
        /// <summary>
        /// 成员字典信息(会记录所有成员的基本信息)
        /// 主键：成员ID
        /// 数据：成员信息
        /// </summary>
        private Dictionary<string, Dictionary<string, object>> _memberDic =new Dictionary<string, Dictionary<string, object>>();

        public Dictionary<string, Dictionary<string, object>> MemberDic
        {
            get { return _memberDic; }
        }

        /// <summary>
        /// 好友请求信息字典
        /// 主键：请求Id
        /// 数据：好友请求信息
        /// </summary>
        private Dictionary<string, Dictionary<string, object>> _friendRequestInfoDic = new Dictionary<string, Dictionary<string, object>>();

        private SocialMessageManager _manager;

        private SocialMessageManager Manager
        {
            get
            {
                if (_manager==null)
                {
                    _manager = Facade.Instance<SocialMessageManager>().InitManager();
                }
                return _manager;
            }
        }

        private string _currentTalkId;

        private TalkTargetType _currentTalkType;

        /// <summary>
        /// 当前热度排名
        /// </summary>
        public int RankNum;

        public string RankInfo
        {
            get
            {
                return (RankNum+1)< 100 ? (RankNum+1).ToString() : SocialTools.KeyNoticeRankOutOfRange;
            }
        }

        /// <summary>social
        /// 单独数据变化Key
        /// </summary>
        public string TalkId
        {
            get { return _currentTalkId; }
        }
        /// <summary>
        /// 好友申请数量
        /// </summary>
        public int FriendRequestNum
        {
            get
            {
                if (FriendRequestList.Count>_friendRequestInfoDic.Count)
                {
                    return 0;
                }
                else
                {
                    return FriendRequestList.Count;
                }
            }
        }

        public string LastItemHeadUrl
        {
            get
            {
                var returnUrl = string.Empty;
                if (FriendRequestNum>0)
                {
                    var lastItem = FriendRequestList.First();//第一条消息是最新的消息
                    if (_friendRequestInfoDic.ContainsKey(lastItem))
                    {
                        var dic = _friendRequestInfoDic[lastItem];
                        if (dic!= null)
                        {
                            dic.TryGetValueWitheKey(out returnUrl, HeadData.KeyAvatar);
                        }
                    }
                }
                return returnUrl;
            }
        }

        /// <summary>
        /// 是否初始化
        /// </summary>
        private bool _hasInit;
        public SocialTalkCenter InitCenter()
        {
            if (!_hasInit)
            {
                InitListeners();
                _hasInit = true;
            }
            return this;
        }

        public override void OnDestroy()
        {
            RemoveListeners();
            base.OnDestroy();
        }

        void InitListeners()
        {
            //群成员信息集合
            Manager.AddEventListeners<Dictionary<string, object>>(SocialTools.KeyActionGetGroupMemberInfo, OnReceiveGroupMemberInfos);
            //黑名单列表
            Manager.AddEventListeners<Dictionary<string, object>>(SocialTools.KeyActionBlackList, OnReceiveBlackList);
            //被拉黑推送
            Manager.AddEventListeners<Dictionary<string, object>>(SocialTools.KeyActionBlackPush, OnBlockedPush);
            //好友申请列表
            Manager.AddEventListeners<Dictionary<string, object>>(SocialTools.KeyActionFriendRequestList,OnReceiveFriendRequestList);
            //好友申请信息列表
            Manager.AddEventListeners<Dictionary<string, object>>(SocialTools.KeyActionFriendRequestInfos,OnReceiveFriendRequestInfos);
            //好友申请信息推送
            Manager.AddEventListeners<Dictionary<string, object>>(SocialTools.KeyActionFriendRequestUpdate,OnFriendRequestUpdate);
            //拉黑好友
            Manager.AddEventListeners<Dictionary<string, object>>(SocialTools.KeyActionSetBlack, SetFriendToBlackList);
            //还原好友
            Manager.AddEventListeners<Dictionary<string, object>>(SocialTools.KeyActionRestoreBlack, RestoreFriendBlackList);
            //群成员排序列表
            Manager.AddEventListeners<Dictionary<string, object>>(SocialTools.KeyActionGetGroupList,OnReceiveSortListInfos);
            //聊天对象列表（理论上应该是群和人的集合，目前只支持群）
            Manager.AddEventListeners<Dictionary<string, object>>(SocialTools.KeyActionGetGroupInfo,OnReceiveGroupInfos);
            //接收到新的聊天信息
            Manager.AddEventListeners<Dictionary<string, object>>(SocialTools.KeyTalkMessage,OnReciveNewTalkData);
            //接收到自己的信息成功回调
            Manager.AddEventListeners<Dictionary<string, object>>(SocialTools.KeyActionSendMessage,OnReceiveSelfSendMessage);
            //群成员信息变化推送
            Manager.AddEventListeners<Dictionary<string, object>>(SocialTools.KeyActionGroupListUpdate,OnGroupInfoUpdate);
            //接收到别人赠送的礼物
            Manager.AddEventListeners<Dictionary<string, object>>(SocialTools.KeyActionBeSendGift, OnBeSendGift);
            //添加好友成功回调
            Manager.AddEventListeners<Dictionary<string, object>>(SocialTools.KeyActionAddFriend, AddFriendRequestResult);
            //游戏内分享图片到亲友圈
            Facade.EventCenter.AddEventListeners<string, Dictionary<string, object>>(SocialTools.KeyShareToSocial, OnReviveShareInfo);
        }

        void RemoveListeners()
        {
            //群成员信息集合
            Manager.RemoveEventListeners<Dictionary<string, object>>(SocialTools.KeyActionGetGroupMemberInfo,
                OnReceiveGroupMemberInfos);
            //黑名单列表
            Manager.RemoveEventListeners<Dictionary<string, object>>(SocialTools.KeyActionBlackList,
                OnReceiveBlackList);
            //被拉黑推送
            Manager.RemoveEventListeners<Dictionary<string, object>>(SocialTools.KeyActionBlackPush, OnBlockedPush);
            //好友申请列表
            Manager.RemoveEventListeners<Dictionary<string, object>>(SocialTools.KeyActionFriendRequestList,
                OnReceiveFriendRequestList);
            //好友申请信息列表
            Manager.RemoveEventListeners<Dictionary<string, object>>(SocialTools.KeyActionFriendRequestInfos,
                OnReceiveFriendRequestInfos);
            //好友申请信息推送
            Manager.RemoveEventListeners<Dictionary<string, object>>(SocialTools.KeyActionFriendRequestUpdate,
                OnFriendRequestUpdate);
            //拉黑好友
            Manager.RemoveEventListeners<Dictionary<string, object>>(SocialTools.KeyActionSetBlack,
                SetFriendToBlackList);
            //还原好友
            Manager.RemoveEventListeners<Dictionary<string, object>>(SocialTools.KeyActionRestoreBlack,
                RestoreFriendBlackList);
            //群成员排序列表
            Manager.RemoveEventListeners<Dictionary<string, object>>(SocialTools.KeyActionGetGroupList,
                OnReceiveSortListInfos);
            //聊天对象列表（理论上应该是群和人的集合，目前只支持群）
            Manager.RemoveEventListeners<Dictionary<string, object>>(SocialTools.KeyActionGetGroupInfo,OnReceiveGroupInfos);
            //接收到新的聊天信息
            Manager.RemoveEventListeners<Dictionary<string, object>>(SocialTools.KeyTalkMessage, OnReciveNewTalkData);
            //接收到自己的信息成功回调
            Manager.RemoveEventListeners<Dictionary<string, object>>(SocialTools.KeyActionSendMessage,
                OnReceiveSelfSendMessage);
            //群成员信息变化推送
            Manager.RemoveEventListeners<Dictionary<string, object>>(SocialTools.KeyActionGroupListUpdate,
                OnGroupInfoUpdate);
            //接收到别人赠送的礼物
            Manager.RemoveEventListeners<Dictionary<string, object>>(SocialTools.KeyActionBeSendGift, OnBeSendGift);
            //添加好友成功回调
            Manager.RemoveEventListeners<Dictionary<string, object>>(SocialTools.KeyActionAddFriend, AddFriendRequestResult);
            //游戏内分享图片到亲友圈
            Facade.EventCenter.RemoveEventListener<string, Dictionary<string, object>>(SocialTools.KeyShareToSocial,
                OnReviveShareInfo);
        }

        /// <summary>
        /// 重新检测游戏内事件监听
        /// </summary>
        public void ReCheckGameListener()
        {
            Facade.EventCenter.RemoveEventListener<string, Dictionary<string, object>>(SocialTools.KeyShareToSocial, OnReviveShareInfo);
            Facade.EventCenter.AddEventListeners<string, Dictionary<string, object>>(SocialTools.KeyShareToSocial, OnReviveShareInfo);
        }


        private void OnReviveShareInfo(Dictionary<string,object> shareInfo)
        {
            string userId;
            string url;
            shareInfo.TryGetValueWitheKey(out userId,SocialTools.KeyId);
            shareInfo.TryGetValueWitheKey(out url, SocialTools.KeyData);
            var getInfoImId= GetTypeDataInDic(userId,SocialTools.KeyOtherId, SocialTools.KeyOwnerId);
            if(BlackBlockedList.Contains(getInfoImId))
            {
                YxMessageBox.Show(SocialTools.KeyNoticeBlockedInfo);
                return;
            }
            string groupId;
            if (CheckUserInList(BlackGroupList, userId, out groupId, SocialTools.KeyOtherId))
            {
                YxMessageBox.Show(SocialTools.KeyNoticeShareUserInBlackList);
                return;
            }
            if (CheckUserInList(SortGroupList, userId,out groupId, SocialTools.KeyOtherId))
            {
                if (!string.IsNullOrEmpty(url))
                {
                    LocalImageUpLoad(
                        delegate(string uploadUrl)
                        {
                            SendTalkInfo(uploadUrl, TalkContentType.Image, groupId, SocialTools.KeyAction);
                        }, url);
                }
            }
        }

        /// <summary>
        /// 设置聊天对象
        /// </summary>
        /// <param name="targetId">目标ID</param>
        /// <param name="targetInfo">目标信息</param>
        /// <param name="targetType">目标类型</param>
        public bool SetTalkTarget(string targetId, out Dictionary<string, object> targetInfo, TalkTargetType targetType = TalkTargetType.@group)
        {
            targetInfo = null;
            if (_groupInfoDic.ContainsKey(targetId))
            {
                string tarGetInfo;
                if (CheckUserInList(SortGroupList, targetId, out tarGetInfo, SocialTools.KeyId, SocialTools.KeyOwnerId))
                {
                    if (BlackBlockedList.Contains(tarGetInfo))
                    {
                        YxMessageBox.Show(SocialTools.KeyNoticeBlockedInfo);
                        return false;
                    }
                    _currentTalkId = targetId;
                    _currentTalkType = targetType;
                    targetInfo = GetTargetGroupInfo(targetId);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 清空聊天对象
        /// </summary>
        public void ResetTalkTarget()
        {
            if (!string.IsNullOrEmpty(_currentTalkId))
            {
                Manager.SendRequest(SocialTools.KeyActionObserverOut);
                _currentTalkId = string.Empty;
            }
        }

        private Dictionary<string,object> GetTargetGroupInfo(string targetId)
        {
            if (!_groupInfoDic.ContainsKey(targetId))
            {
                if (Manager.UserGroupId!=null)
                {
                    targetId = Manager.UserGroupId;
                }
                else
                {
                    Debug.LogError("当前玩家的群id是空的，请确认异常!");
                    return null;
                }
            }
            var targetInfo = _groupInfoDic[targetId];
            if (_talkListDic.ContainsKey(targetId))
            {
                var talkList = _talkListDic[targetId];
                if (talkList.Count > 0)
                {
                    targetInfo[SocialTools.KeyData] = _talkListDic[targetId];
                }
            }
            return targetInfo;
        }
        /// <summary>
        /// 拉黑请求
        /// </summary>
        /// <param name="imId"></param>
        public void DeFriend(string imId)
        {
            Manager.SendRequest(SocialTools.KeyActionSetBlack,new Dictionary<string, object>()
                {
                    {SocialTools.KeyImId,imId}
                });
        }

        /// <summary>
        /// 还原请求
        /// </summary>
        /// <param name="imId"></param>
        public void ReFriend(string imId)
        {
            Manager.SendRequest(SocialTools.KeyActionRestoreBlack, new Dictionary<string, object>()
            {
                {SocialTools.KeyImId,imId}
            });
        }

        /// <summary>
        /// 初始化列表数据
        /// </summary>
        public void InitList(Dictionary<string, object> initInfo)
        {
            List<string> socialGroupList;
            _needRequestSortList = true;
            initInfo.TryGetStringListWithKey(out socialGroupList, SocialTools.KeyGroupList);
            List<string> socialBlackList;
            initInfo.TryGetStringListWithKey(out socialBlackList, SocialTools.KeyBlackList);
            int getRankNum;
            initInfo.TryGetValueWitheKey(out getRankNum, SocialTools.KeyRankNum);
            ChangRankNum(getRankNum);
            SetListFomAction(socialGroupList,ref SortGroupList);
            SetListFomAction(socialBlackList, ref BlackGroupList,false,false,SocialTools.KeyActionBlackList);
            GetSortList();
            GetBlackSortList();
            GetFriendRequestList();
            GetBlockedList();
        }

        public void ResetData()
        {
            SortGroupList.Clear();
            FriendRequestList.Clear();
            AddFriendSortGroupList.Clear();
            BlackGroupList.Clear();
            BlackBlockedList.Clear();
            _rankList.Clear();
            _groupInfoDic.Clear();
            _groupInfoDicByImId.Clear();
            _talkListDic.Clear();
            _memberDic.Clear();
            _friendRequestInfoDic.Clear();
            _needRequestSortList = true;
        }

        #region 获取数据接口
        /// <summary>
        /// 获取群排序列表(获取当前玩家的聊天对象列表)
        /// </summary>
        public void GetSortList(bool directFresh=false, bool noCheckSend = false,string fromAction=SocialTools.KeyActionGetGroupList)
        {
            bool needSort = fromAction== SocialTools.KeyActionGetGroupList && _needRequestSortList;
            if (needSort)
            {
                _needRequestSortList = false;
            }
            if (SortGroupList.Count>0&&!directFresh&&!needSort)
            {
                SendFriendListToLocal(fromAction);
            }
            else
            {
                Manager.SendRequest(SocialTools.KeyActionGetGroupList,SocialTools.SetDefKeyDic(null,new Dictionary<string, object>()
                {
                    {SocialTools.KeyNoCheckSend,noCheckSend}, {SocialTools.KeyFromAction,fromAction}
                }));
            }
        }

        /// <summary>
        /// 获取群排序列表(获取当前玩家的聊天对象列表)
        /// </summary>
        public void GetFriendRequestList(bool directFresh = false, bool noCheckSend = false)
        {
            if (FriendRequestList.Count > 0 && !directFresh)
            {
                SendFriendRequestListToLocal();
            }
            else
            {
                Manager.SendRequest(SocialTools.KeyActionFriendRequestList,SocialTools.SetDefKeyDic(null,new Dictionary<string, object>()
                {
                    {SocialTools.KeyNoCheckSend,noCheckSend}
                }));
            }
        }
        /// <summary>
        /// 更改好友请求数据
        /// </summary>
        /// <param name="boxId">boxId</param>
        /// <param name="handle">更改后的状态</param>
        public void ChangeFriendRequestData(string boxId,string handle)
        {
            if (_friendRequestInfoDic.ContainsKey(boxId))
            {
                _friendRequestInfoDic[boxId][SocialTools.KeyHandles] = handle;
            }
        }

        /// <summary>
        /// 获取黑名单列表
        /// </summary>
        public void GetBlackSortList(bool directFresh = false, bool noCheckSend = false)
        {
            if (BlackGroupList.Count > 0 && !directFresh)
            {
                SendBlackListToLocal();
            }
            else
            {
                var dic = new Dictionary<string, object>()
                {
                    { SocialTools.KeyBlackListType,0}
                }.SetDefKeyDic(new Dictionary<string, object>()
                {
                    { SocialTools.KeyNoCheckSend, true}, { SocialTools.KeyBlackListType,0}
                });
                Manager.SendRequest(SocialTools.KeyActionBlackList, dic);
            }
        }

        public void GetBlockedList()
        {
            var dic = new Dictionary<string, object>()
            {
                { SocialTools.KeyBlackListType,1}
            }.SetDefKeyDic(new Dictionary<string, object>()
            {
                { SocialTools.KeyNoCheckSend, true}, { SocialTools.KeyBlackListType,1}
            });
            Manager.SendRequest(SocialTools.KeyActionBlackList, dic);
        }

        /// <summary>
        /// 获取排行列表
        /// </summary>
        /// <param name="rankList"></param>
        /// <param name="action"></param>
        public void GetRankList(List<string> rankList,string action)
        {
            if (rankList.Contains(Manager.UserGroupId))
            {
                var index = rankList.IndexOf(Manager.UserGroupId);
                if (index>-1)
                {
                    ChangRankNum(index);
                }
            }
            SetListFomAction(rankList,ref _rankList,true,true, action);
        }

        /// <summary>
        /// 获取群成员信息
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="isSingle">是否为单条消息</param>
        public void GetGroupMembers(string groupId,bool isSingle)
        {
            Manager.SendRequest(SocialTools.KeyActionGetGroupMemberInfo, new Dictionary<string, object>()
            {
                { SocialTools.KeyGroupId,groupId},
            }.SetDefKeyDic(new Dictionary<string, object>()
            {
                { SocialTools.KeySingleItem,isSingle}
            }));
        }

        /// <summary>
        /// 获取群排序信息
        /// </summary>
        public void GetGroupInfos(List<string> ids,bool isLast = false,string fromAction=SocialTools.KeyActionGetGroupList,string partAction= SocialTools.KeyActionGetGroupInfo)
        {
            Manager.SendRequest(partAction,new Dictionary<string, object>()
            {
                { SocialTools.KeyIds,ids}, {SocialTools.KeyDefKey,new Dictionary<string,object>()
                {
                    { SocialTools.KeyIsLastRequest,isLast},{ SocialTools.KeyFromAction,fromAction}
                }} 
            });
        }

        /// <summary>
        /// 发送聊天消息
        /// </summary>
        /// <param name="content"></param>
        /// <param name="contentType"></param>
        /// <param name="targetId"></param>
        /// <param name="fromAction"></param>
        public void SendTalkInfo(string content, TalkContentType contentType,string targetId="",string fromAction="")
        {
            var dic = new Dictionary<string, object>();
            dic[SocialTools.KeyMessageReceiveId] = string.IsNullOrEmpty(targetId)?string.IsNullOrEmpty(_currentTalkId)? Manager.UserGroupId: _currentTalkId : targetId;
            dic[SocialTools.KeyMessageContent] = content;
            dic[SocialTools.KeyMessageType] = string.IsNullOrEmpty(targetId) ? _currentTalkType.ToString(): TalkTargetType.@group.ToString();
            dic[SocialTools.KeyMessageContentType] = contentType;
            if (!string.IsNullOrEmpty(fromAction))
            {
                dic.SetDefKeyDic(new Dictionary<string, object>{{SocialTools.KeyFromAction,fromAction}});
            }
            Manager.SendRequest(SocialTools.KeyActionSendMessage,dic);
        }

        #endregion

        #region 数据接收
        /// <summary>
        /// 查询群信息的最大长度（一次最多返回数据数量）
        /// </summary>
        private int _maxQueryGroupInfoCount = 6;
        /// <summary>
        /// 接收群排序列表
        /// </summary>
        private void OnReceiveSortListInfos(Dictionary<string, object> sortData)
        {
            List<string> newIdList;
            sortData.TryGetStringListWithKey(out newIdList, SocialTools.KeyIds);
            var defDic = sortData.ParseDefKeyDic();
            string fromAction;
            bool noCheckSend;
            defDic.TryGetValueWitheKey(out fromAction, SocialTools.KeyFromAction);
            defDic.TryGetValueWitheKey(out noCheckSend, SocialTools.KeyNoCheckSend,true);
            switch (fromAction)
            {
                case SocialTools.KeyActionAddList:
                    SetListFomAction(newIdList, ref AddFriendSortGroupList, true, noCheckSend,fromAction);
                    break;
                case SocialTools.KeyActionGetGroupList:
                    SetListFomAction(newIdList, ref SortGroupList,true,noCheckSend,fromAction);
                    break;
            }
        }

        /// <summary>
        /// 接收群基本信息
        /// </summary>
        private void OnReceiveGroupInfos(Dictionary<string, object> sortData)
        {
            List<object> infos;
            sortData.TryGetValueWitheKey(out infos, SocialTools.KeyIds);
            var length = infos.Count;
            for (int i = 0; i < length; i++)
            {
                var item = infos[i] as Dictionary<string, object>;
                if (item != null)
                {
                    string groupId;
                    item.TryGetValueWitheKey(out groupId, SocialTools.KeyId);
                    string imId;
                    item.TryGetValueWitheKey(out imId, SocialTools.KeyOwnerId);
                    if (imId==Manager.UserImId)
                    {
                        item[SocialTools.KeyGroupName] = SocialTools.KeySelfNickShow;
                    }
                    if (!string.IsNullOrEmpty(groupId))
                    {
                        _groupInfoDic[groupId] = item;
                    }
                    if (!string.IsNullOrEmpty(imId))
                    {
                        _groupInfoDicByImId[imId] = item;
                    }
                }
            }
            Dictionary<string, object> defaultData;
            sortData.TryGetValueWitheKey(out defaultData, SocialTools.KeyDefKey);
            if ((bool)defaultData[SocialTools.KeyIsLastRequest])
            {
                string fromAction;
                defaultData.TryGetValueWitheKey(out fromAction, SocialTools.KeyFromAction);
                SendListDataByAction(fromAction);
            }
        }
        /// <summary>
        /// 接收群成员信息（批量接收）
        /// </summary>
        /// <param name="memberInfos"></param>
        private void OnReceiveGroupMemberInfos(Dictionary<string,object> memberInfos)
        {
            List<object> members;
            memberInfos.TryGetValueWitheKey(out members, SocialTools.KeyMembers);
            var length = members.Count;
            for (int i = 0; i < length; i++)
            {
                var memberItem = members[i] as Dictionary<string, object>;
                if (memberItem != null)
                {
                    string imId;
                    memberItem.TryGetValueWitheKey(out imId, SocialTools.KeyImId);
                    if (!string.IsNullOrEmpty(imId))
                    {
                        _memberDic[imId] = memberItem;
                    }
                }
            }
            Manager.DispatchLocalEvent(SocialTools.KeyActionGetGroupMemberInfo, memberInfos);
        }

        /// <summary>
        /// 下次界面请求排序列表时是否需要重新请求
        /// </summary>
        private bool _needRequestSortList;
        /// <summary>
        /// 群信息变化
        /// </summary>
        /// <param name="updateInfo"></param>
        private void OnGroupInfoUpdate(Dictionary<string, object> updateInfo)
        {
            int updateType;
            updateInfo.TryGetValueWitheKey(out updateType, SocialTools.KeyUpdateType);
            string groupId;
            updateInfo.TryGetValueWitheKey(out groupId, SocialTools.KeyId);
            switch (updateType)
            {
                case -1:
                    TryRemoveGroupData(groupId);
                    Manager.DispatchLocalEvent(SocialTools.KeyActionGroupListUpdate,false);
                    if (!App.GameKey.Equals(SocialTools.KeyGameInHall))
                    {
                        GetSortList(false, true);
                    }
                    break;
                case 0:
                case 1:
                    if (!SortGroupList.Contains(groupId))
                    {
                        SortGroupList.Add(groupId);
                    }
                    GetGroupInfos(new List<string>() { groupId });
                    _needRequestSortList = true;
                    Manager.DispatchLocalEvent(SocialTools.KeyActionGroupListUpdate, true);
                    if (!App.GameKey.Equals(SocialTools.KeyGameInHall))
                    {
                        GetSortList(true, true);
                    }
                    break;
                default:
                    YxDebug.LogEvent(string.Format(SocialTools.KeyNoticePushDataError, SocialTools.KeyActionGroupListUpdate, updateType));
                    break;
            }
        }

        /// <summary>
        /// 接收礼物回调
        /// </summary>
        /// <param name="giftInfo"></param>
        private void OnBeSendGift(Dictionary<string,object> giftInfo)
        {
            UserController.Instance.SendSimpleUserData(null,false);
        }

        /// <summary>
        /// 添加好友请求结果回调
        /// </summary>
        /// <param name="partInfos"></param>
        private void AddFriendRequestResult(Dictionary<string, object> partInfos)
        {
            YxMessageBox.Show(SocialTools.KeyNoticeRequestSendSuccess);
        }

        /// <summary>
        /// 尝试删除本地群数据
        /// </summary>
        /// <param name="groupId"></param>
        private void TryRemoveGroupData(string groupId)
        {
            if (SortGroupList.Contains(groupId))
            {
                SortGroupList.Remove(groupId);
                if (_groupInfoDic.ContainsKey(groupId))
                {
                    var imId = _groupInfoDic[groupId][SocialTools.KeyOwnerId];
                    _groupInfoDic.Remove(groupId);
                    if (_groupInfoDicByImId.ContainsKey(imId.ToString()))
                    {
                        _groupInfoDicByImId.Remove(imId.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 拉黑好友成功回调
        /// </summary>
        /// <param name="setResult"></param>
        private void SetFriendToBlackList(Dictionary<string, object> setResult)
        {
            GetSortList(true,true);
            GetBlackSortList(true,true);
            Manager.DispatchLocalEvent(SocialTools.KeyActionSetBlack, setResult);
        }

        /// <summary>
        /// 好友还原回调
        /// </summary>
        /// <param name="setResult"></param>
        private void RestoreFriendBlackList(Dictionary<string, object> setResult)
        {
            GetSortList(true);
            GetBlackSortList(true);
        }
        /// <summary>
        /// 接收到聊天消息（所有人）
        /// </summary>
        /// <param name="talkData"></param>
        public void OnReciveNewTalkData(Dictionary<string, object> talkData)
        {
            string reciveId;
            talkData.TryGetValueWitheKey(out reciveId, SocialTools.KeyMessageReceiveId);
            string targetType;
            talkData.TryGetValueWitheKey(out targetType, SocialTools.KeyMessageType);
            string messageId;
            talkData.TryGetValueWitheKey(out messageId, SocialTools.KeyId);
            if (reciveId != null)
            {
                string sendId;
                talkData.TryGetValueWitheKey(out sendId, SocialTools.KeyMessageSendId);
                if (!_talkListDic.ContainsKey(reciveId))
                {
                    _talkListDic[reciveId] = new List<Dictionary<string, object>>();
                }
                _talkListDic[reciveId].Add(talkData);
                if (reciveId.Equals(_currentTalkId))
                {
                    _manager.DispatchLocalEvent(SocialTools.KeyTalkMessage, talkData);
                }
                else
                {
                    if (_groupInfoDic.ContainsKey(reciveId))
                    {
                        FriendListRedPointChange(reciveId, true);
                    }
                }
            }
        }

        /// <summary>
        /// 接收到自己发送消息回调
        /// </summary>
        /// <param name="talkData"></param>
        private void OnReceiveSelfSendMessage(Dictionary<string, object> talkData)
        {
            var defDic=talkData.ParseDefKeyDic();
            if (defDic.ContainsKey(SocialTools.KeyFromAction))
            {
                if (SocialTools.KeyAction.Equals(defDic[SocialTools.KeyFromAction]))
                {
                    YxMessageBox.Show(SocialTools.KeyNoticeShareSuccess);
                }
            }
            Manager.DispatchLocalEvent(SocialTools.KeyActionSendMessage, talkData);
        }   

        /// <summary>
        /// 黑名单
        /// </summary>
        /// <param name="blackListData"></param>
        private void OnReceiveBlackList(Dictionary<string, object> blackListData)
        {
            List<string> newIdList;
            blackListData.TryGetStringListWithKey(out newIdList, SocialTools.KeyIds);
            var defDic = blackListData.ParseDefKeyDic();
            int type;
            defDic.TryGetValueWitheKey(out type,SocialTools.KeyBlackListType);
            switch (type)
            {
                case 0:
                    var blackGroupList = new List<string>();
                    for (int i = 0, length = newIdList.Count; i < length; i++)
                    {
                        var key = newIdList[i];
                        if (_groupInfoDicByImId.ContainsKey(key))
                        {
                            blackGroupList.Add(_groupInfoDicByImId[key][SocialTools.KeyId].ToString());
                        }
                    }
                    SetListFomAction(blackGroupList, ref BlackGroupList, true, (bool)defDic[SocialTools.KeyNoCheckSend], SocialTools.KeyActionBlackList);
                    break;
                case 1:
                    BlackBlockedList = newIdList.ToList();
                    break;
            }
        }
        /// <summary>
        /// 被拉黑消息推送
        /// </summary>
        private void OnBlockedPush(Dictionary<string,object> blockedInfo)
        {
            int opType;
            string blockImId;
            string blackImId;
            blockedInfo.TryGetValueWitheKey(out opType,SocialTools.KeyUpdateType);
            blockedInfo.TryGetValueWitheKey(out blockImId, SocialTools.KeyBlackId);
            blockedInfo.TryGetValueWitheKey(out blackImId, SocialTools.KeyImId);
            if (blockImId.Equals(Manager.UserImId))
            {
                switch (opType)
                {
                    case -1:
                        if (BlackBlockedList.Contains(blackImId))
                        {
                            BlackBlockedList.Remove(blackImId);
                        }
                        break;
                    case 1:
                        if (!BlackBlockedList.Contains(blackImId))
                        {
                            BlackBlockedList.Add(blackImId);
                        }
                        break;
                    default:
                        Debug.LogError(string.Format(SocialTools.KeyNoticePushDataError,SocialTools.KeyActionBlackPush,opType));
                        break;
                }
                Manager.DispatchLocalEvent(SocialTools.KeyActionBlackPush,blockedInfo);
            }
        }

        /// <summary>
        /// 好友申请列表集合
        /// </summary>
        /// <param name="friendRequestData"></param>
        private void OnReceiveFriendRequestList(Dictionary<string, object> friendRequestData)
        {
            List<string> getList;
            friendRequestData.TryGetStringListWithKey(out getList, SocialTools.KeyList);
            if (!getList.ValueEqual(FriendRequestList)&&getList.Count>0)
            {
                FriendRequestList = getList.ToList();
                var newRequestList = new List<string>();
                var totalCount = FriendRequestList.Count;
                for (int i = 0; i < totalCount; i++)
                {
                    var checkId = FriendRequestList[i];
                    if (!_friendRequestInfoDic.ContainsKey(checkId))
                    {
                        newRequestList.Add(checkId);
                    }
                }
                if (newRequestList.Count==0)
                {
                    SendFriendRequestListToLocal();
                    return;
                }
                var requestCount = newRequestList.Count;
                Manager.DispatchLocalEvent(SocialTools.KeyRequestNumChange,new Dictionary < string, object >());
                var requestTime = requestCount / _maxQueryGroupInfoCount + (requestCount % _maxQueryGroupInfoCount == 0 ? 0 : 1);
                for (int i = 0; i < requestTime; i++)
                {
                    var starIndex = i * _maxQueryGroupInfoCount;
                    var leftCount = requestCount - starIndex;
                    var requestList = newRequestList.GetRange(starIndex, Math.Min(leftCount, _maxQueryGroupInfoCount));
                    GetGroupInfos(requestList, leftCount <= _maxQueryGroupInfoCount, SocialTools.KeyActionFriendRequestList, SocialTools.KeyActionFriendRequestInfos);
                }
            }
            else
            {
                FriendRequestList = getList.ToList();
                SendFriendRequestListToLocal();
            }
        }

        /// <summary>
        /// 接收到好友申请列表信息
        /// </summary>
        private void OnReceiveFriendRequestInfos(Dictionary<string, object> friendRequestData)
        {
            List<object> infos;
            friendRequestData.TryGetValueWitheKey(out infos, SocialTools.KeyIds);
            var length = infos.Count;
            for (int i = 0; i < length; i++)
            {
                var item = infos[i] as Dictionary<string, object>;
                if (item != null)
                {
                    string boxId;
                    item.TryGetValueWitheKey(out boxId, SocialTools.KeyBoxId);
                    if (!string.IsNullOrEmpty(boxId))
                    {
                        item[SocialTools.KeyId] = boxId;
                        _friendRequestInfoDic[boxId] = item;
                    }
                }
            }
            Dictionary<string, object> defaultData;
            friendRequestData.TryGetValueWitheKey(out defaultData, SocialTools.KeyDefKey);
            if ((bool)defaultData[SocialTools.KeyIsLastRequest])
            {
                string fromAction;
                defaultData.TryGetValueWitheKey(out fromAction, SocialTools.KeyAction);
                SendFriendRequestListToLocal();
            }
        }
        
        /// <summary>
        /// 好友请求推送
        /// </summary>
        /// <param name="friendRequestData"></param>
        private void OnFriendRequestUpdate(Dictionary<string, object> friendRequestData)
        {
            string boxId;
            friendRequestData.TryGetValueWitheKey(out boxId, SocialTools.KeyBoxId);
            if (!string.IsNullOrEmpty(boxId))
            {
                friendRequestData[SocialTools.KeyId] = boxId;
                _friendRequestInfoDic[boxId] = friendRequestData;
            }
            GetFriendRequestList(true, true);
        }


        #endregion

        #region 逻辑处理

        /// <summary>
        ///发送亲友数据到本地
        /// </summary>
        private void SendFriendListToLocal(string action)
        {
            var groupListData = new Dictionary<string, object>();
            List<string> dealList;
            switch(action)
            {
                case SocialTools.KeyActionAddList:
                    dealList=AddFriendSortGroupList.ToList();
                    break;
                case SocialTools.KeyActionGetGroupList:
                    dealList = SortGroupList.ToList();
                    break;
                case SocialTools.KeyActionRankInfos:
                    dealList = _rankList.ToList();
                    break;
                default:
                    dealList=new List<string>();
                    break;
            }
            groupListData[SocialTools.KeyIds] = dealList;
            groupListData[SocialTools.KeyData] = SocialTools.SelectDicDataFromList(dealList, _groupInfoDic);
            Manager.DispatchLocalEvent(action, groupListData);
        }
        /// <summary>
        /// 发送黑名单数据到本地
        /// </summary>
        private void SendBlackListToLocal()
        {
            var groupListData = new Dictionary<string, object>();
            groupListData[SocialTools.KeyIds] = BlackGroupList;
            groupListData[SocialTools.KeyData] = SocialTools.SelectDicDataFromList(BlackGroupList, _groupInfoDic);
            Manager.DispatchLocalEvent(SocialTools.KeyActionBlackList, groupListData);
        }

        public void ChangRankNum(int changeNum)
        {
            if (!RankNum.Equals(changeNum))
            {
                RankNum = changeNum;
                Manager.DispatchLocalEvent(SocialTools.KeyRankNumChange,new Dictionary<string,object>());
            }
        }

        /// <summary>
        /// 发送好友请求列表到本地
        /// </summary>
        private void SendFriendRequestListToLocal()
        {
            var groupListData = new Dictionary<string, object>();
            groupListData[SocialTools.KeyIds] = FriendRequestList;
            groupListData[SocialTools.KeyData] = SocialTools.SelectDicDataFromList(FriendRequestList, _friendRequestInfoDic);
            Manager.DispatchLocalEvent(SocialTools.KeyActionFriendRequestList, groupListData);
        }

        /// <summary>
        /// 发送本地事件
        /// </summary>
        /// <param name="action"></param>
        private void SendListDataByAction(string action)
        {
            switch (action)
            {
                case SocialTools.KeyActionGetGroupList:
                case SocialTools.KeyActionAddList:
                case SocialTools.KeyActionRankInfos:
                    SendFriendListToLocal(action);
                    break;
                case SocialTools.KeyActionBlackList:
                    SendBlackListToLocal();
                    break;
                default:
                    Debug.LogError("未知来源Action:"+ action);
                    break;
            }
        }

        /// <summary>
        /// 好友列表红点状态变化
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        public void FriendListRedPointChange(string id, bool state)
        {
            if (_groupInfoDic.ContainsKey(id))
            {
                bool unreadStatus;
                _groupInfoDic[id].TryGetValueWitheKey(out unreadStatus, SocialTools.KeyMessageUnRead);
                if (unreadStatus != state)
                {
                    _groupInfoDic[id][SocialTools.KeyMessageUnRead] = state;
                    GetSortList();
                }
            }
        }

        /// <summary>
        /// 设置列表数据
        /// </summary>
        /// <param name="sourceList">数据源</param>
        /// <param name="dealList">处理数据</param>
        /// <param name="coverList">是否覆盖处理数据</param>
        /// <param name="noCheck">是否检测本地缓存</param>
        /// <param name="fromAction">来源时间</param>
        public void SetListFomAction(List<string> sourceList,ref List<string> dealList,bool coverList=false,bool noCheck=false,string fromAction = SocialTools.KeyActionGetGroupList)
        {
            if (sourceList.ValueEqual(dealList)&&!noCheck)
            {
                return;
            }
            if (coverList)
            {
                dealList = sourceList.ToList();
            }
            var totalCount = sourceList.Count;
            var newRequestList = new List<string>();
            for (int i = 0; i < totalCount; i++)
            {
                var checkId = sourceList[i];
                if (!_groupInfoDic.ContainsKey(checkId))
                {
                    newRequestList.Add(checkId);
                }
            }
            if (newRequestList.Count == 0)
            {
                SendListDataByAction(fromAction);
            }
            else
            {
                var requestCount = newRequestList.Count;
                var requestTime = requestCount / _maxQueryGroupInfoCount +(requestCount%_maxQueryGroupInfoCount==0?0:1);
                for (int i = 0; i < requestTime; i++)
                {
                    var starIndex = i * _maxQueryGroupInfoCount;
                    var leftCount = requestCount - starIndex;
                    var requestList = newRequestList.GetRange(starIndex, Math.Min(leftCount, _maxQueryGroupInfoCount));
                    GetGroupInfos(requestList,leftCount <= _maxQueryGroupInfoCount, fromAction);
                }
            }
        }
        /// <summary>
        /// 获取黑名单列表(Im id)
        /// </summary>
        /// <returns></returns>
        public List<string> GetBlackListByImId()
        {
            return GetListByCheckType(BlackGroupList);
        }

        /// <summary>
        /// 获取好友列表(Im id)
        /// </summary>
        /// <returns></returns>
        public List<string> GetGroupListByImId()
        {
            return GetListByCheckType(SortGroupList);
        }
        /// <summary>
        /// 检测对应imId是否存在于黑名单中
        /// </summary>
        /// <param name="imId"></param>
        /// <returns></returns>
        public bool CheckUserInBlackList(string imId)
        {
            string findImId;
            return CheckUserInList(BlackGroupList, imId, out findImId);
        }

        public string GetBlockGroupId(string imId)
        {
            string findBlockGroupId;
            if (!CheckUserInList(SortGroupList, imId, out findBlockGroupId))
            {
                CheckUserInList(BlackGroupList, imId, out findBlockGroupId);
            }
            return findBlockGroupId;
        }

        public List<Dictionary<string, object>> GetUserInfoByUserIdList(List<string> userIds)
        {
            var returnData=new List<Dictionary<string,object>>();
            var count = userIds.Count;
            for (int i = 0; i < count; i++)
            {
                var checkId = userIds[i];
                string findGroupId;
                if (!CheckUserInList(SortGroupList, checkId, out findGroupId,SocialTools.KeyOtherId))
                {
                    CheckUserInList(BlackGroupList, checkId, out findGroupId,SocialTools.KeyOtherId);
                }
                if (!string.IsNullOrEmpty(findGroupId))
                {
                    if(_groupInfoDic.ContainsKey(findGroupId))
                    {
                        returnData.Add(_groupInfoDic[findGroupId]);
                    }
                }
            }
            return returnData;
        }

        /// <summary>
        /// 获取指定列表的ImId排序（黑名单列表与好友列表）
        /// </summary>
        /// <param name="list"></param>
        /// <param name="checkType"></param>
        /// <returns></returns>
        private List<string> GetListByCheckType(List<string> list,string checkType= SocialTools.KeyOwnerId)
        {
            var returnList=new List<string>();
            for (int i=0,length=list.Count;i<length;i++)
            {
                var checkKey = list[i];
                if (_groupInfoDic.ContainsKey(checkKey))
                {
                    returnList.Add(_groupInfoDic[checkKey][checkType].ToString());
                }
            }
            return returnList;
        }

        /// <summary>
        /// 检测对应列表数据中是否存在指定key的数据
        /// </summary>
        /// <param name="list">检测范围</param>
        /// <param name="checkData">检测数据</param>
        /// <param name="returnData">返回数据</param>
        /// <param name="checkType">校验字段</param>
        /// <param name="returnType">返回字段</param>
        /// <returns></returns>
        private bool CheckUserInList(List<string> list,string checkData,out string returnData,string checkType = SocialTools.KeyOwnerId,string returnType= SocialTools.KeyId)
        {
            for (int i = 0, length = list.Count; i < length; i++)
            {
                var checkKey = list[i];
                if (_groupInfoDic.ContainsKey(checkKey))
                {
                    var itemData = _groupInfoDic[checkKey];
                    if (itemData.ContainsKey(checkType))
                    {
                        if (checkData.Equals(itemData[checkType].ToString()))
                        {
                            if (itemData.ContainsKey(returnType))
                            {
                                returnData = itemData[returnType].ToString();
                                return true;
                            }
                        }
                    }
                }
            }
            returnData = string.Empty;
            return false;
        }

        /// <summary>
        /// 根据传入值，找到查找值
        /// </summary>
        /// <param name="findValue"></param>
        /// <param name="inputType"></param>
        /// <param name="findType"></param>
        /// <returns></returns>
        public string GetTypeDataInDic(string findValue,string inputType,string findType)
        {
            foreach (var item in _groupInfoDic)
            {
                var value = item.Value;
                if (value!=null)
                {
                    if (value.ContainsKey(inputType)&& value.ContainsKey(findType))
                    {
                        if (value[inputType].ToString().Equals(findValue))
                        {
                            return value[findType].ToString();
                        }
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 选择本地图片并上传
        /// </summary>
        /// <param name="finishCall"></param>
        public void SelectImageUpLoad(Action<string> finishCall)
        {
            NativeGallery.GetImageFromGallery((path) => { LocalImageUpLoad(finishCall, path); });
        }
        /// <summary>
        /// 本地指定路径文件上传
        /// </summary>
        /// <param name="finishCall"></param>
        /// <param name="path"></param>
        public void LocalImageUpLoad(Action<string> finishCall,string path)
        {
            if (path != null)
            {
                StartCoroutine(SocialTools.LoadLocalImage(path, delegate (Texture2D tex)
                {
                    if (tex != null)
                    {
                        StartCoroutine(SocialTools.Uploading(SocialSetting.UpLoadUrl.CombinePath(SocialTools.KeyImageUpLoadAction), SocialTools.GetLocalImageData(path, tex), delegate (string downUrl)
                        {
                            var uploadUrl = SocialTools.GetUpLoadUrl(downUrl);
                            if (!string.IsNullOrEmpty(uploadUrl))
                            {
                                if (finishCall != null)
                                {
                                    finishCall(uploadUrl);
                                }
                            }
                        }));
                    }
                }));
            }
        }

        void OnApplicationQuit()
        {
            ResetData();
        }
        #endregion
    }
}
