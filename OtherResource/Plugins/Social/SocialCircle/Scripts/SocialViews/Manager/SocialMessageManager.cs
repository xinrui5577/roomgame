using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using BestHTTP;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.Manager
{
    public class SocialMessageManager : BaseMono
    {
        /// <summary>
        /// 玩家ImId
        /// </summary>
        public string UserImId { get; private set; }
        /// <summary>
        /// 玩家群id
        /// </summary>
        public string UserGroupId { get; private set; }

        /// <summary>
        /// 进入亲友圈次数（标记是否首次进入亲友圈）
        /// </summary>
        public int EntryNum;

        /// <summary>
        /// 聊天对象类型
        /// </summary>
        public TalkTargetType TargetType= TalkTargetType.@group;
        /// <summary>
        /// 礼物id集合
        /// </summary>
        public List<string> GiftIds=new List<string>();
        /// <summary>
        ///  礼物数据字典
        /// </summary>
        public Dictionary<string,Dictionary<string,object>> GiftDataDic = new Dictionary<string, Dictionary<string, object>>();

        /// <summary>
        /// 礼物点数量
        /// </summary>
        public int GiftNum
        {
            get { return UserInfoModel.Instance.BackPack.GetItem(SocialTools.KeyGiftPoint); }
        }

        /// <summary>
        /// 消息中心
        /// </summary>
        private YxEventDispatchManager _eventCenter = new YxEventDispatchManager();

        private WebSocketManager _socketManager
        {
            get
            {
                if (Facade.GetInstance==null)
                {
                    return null;
                }
                return Facade.Instance<WebSocketManager>();
            }
        }
        /// <summary>
        /// 聊天中心
        /// </summary>
        private SocialTalkCenter _talkManager
        {
            get { return Facade.Instance<SocialTalkCenter>().InitCenter(); }
        }
        /// <summary>
        /// 喇叭中心
        /// </summary>
        private SocialHornCenter _hornManager
        {
            get { return Facade.Instance<SocialHornCenter>().InitCenter(); }
        }
        /// <summary>
        /// 房间中心
        /// </summary>
        private SocialRoomCenter _roomManager
        {
            get { return Facade.Instance<SocialRoomCenter>().InitCenter(); }
        }

        private bool _hasInit;
        public SocialMessageManager InitManager()
        {
            if (!_hasInit)
            {
                _hasInit = true;
                InitListener();
                _waitMessageQueue=new Queue<Dictionary<string, object>>();
            }
            return this;
        }

        public void CloseSocket()
        {
            if (_socketManager!=null)
            {
                _socketManager.CloseSocekt();
                _talkManager.ResetData();
                _hornManager.ResetData();
                _roomManager.ResetData();
                if (HTTPUpdateDelegator.Instance)
                {
                    HTTPUpdateDelegator.Instance.enabled = false;
                }
            }
        }

        public void ConnectSocket(bool fromLogin=false)
        {
            if (LoginInfo.Instance.Chesslm>0)
            {

                if (!_socketManager.IsOpen)
                {
                    bool needConnect = fromLogin;
                    if (!needConnect)
                    {
                        needConnect = App.History.CurrentHistory() > YxEHistoryPathType.Register;
                    }
                    if (needConnect)
                    {
                        CloseSocket();
                        _socketManager.ConnectSocket();
                    }
                }
            }
        }

        public void SetDebug()
        {
            _eventCenter.SetDebug();
        }

        private Queue<Dictionary<string, object>>   _waitMessageQueue;
        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="action"></param>
        /// <param name="param"></param>
        public void SendRequest(string action,Dictionary<string,object> param=null)
        {
            while (_waitMessageQueue.Count > 0)
            {
                var cacheMessage = _waitMessageQueue.Peek();
                if (cacheMessage!=null)
                {
                    string trySendAction;
                    Dictionary<string, object> trySendParam;
                    cacheMessage.TryGetValueWitheKey(out trySendAction,SocialTools.KeyAction);
                    cacheMessage.TryGetValueWitheKey(out trySendParam, SocialTools.KeyData);
                    if (!string.IsNullOrEmpty(trySendAction))
                    {
                        if (_socketManager.SendSendRequest(trySendAction, trySendParam))
                        {
                            _waitMessageQueue.Dequeue();
                            continue;
                        }
                    }
                }
                break;
            }
            if (!_socketManager.SendSendRequest(action, param))
            {
                _waitMessageQueue.Enqueue(new Dictionary<string, object>()
                {
                    {SocialTools.KeyAction,action}, {SocialTools.KeyData,param}
                });
            }
        }

        private float _cacheTime;
        private float _oneceTime = 3;
        void Update()
        {
            if (LoginInfo.Instance.Chesslm > 0)
            {
                if (Facade.HasInstance<WebSocketManager>() &&App.History.CurrentHistory() > YxEHistoryPathType.Register)
                {
                    _cacheTime += Time.deltaTime;
                    if (_cacheTime >= _oneceTime)
                    {
                        _cacheTime = 0f;
                        switch (Application.internetReachability)
                        {
                            case NetworkReachability.NotReachable:
                                break;
                            case NetworkReachability.ReachableViaLocalAreaNetwork:
                            case NetworkReachability.ReachableViaCarrierDataNetwork:
                                ConnectSocket();
                                break;
                        }
                    }
                }
            }
        }




        private void InitListener()
        {
            AddEventListeners<Dictionary<string,object>>(SocialTools.KeyActionLogin, OnGetLoginInfo);
        }
        /// <summary>
        /// 添加监听事件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fun"></param>
        /// <returns></returns>
        public bool AddEventListeners<T>(string key, Action<T> fun)
        {
            return _eventCenter.AddEventListeners(key, fun); 
        }
        /// <summary>
        /// 删除监听事件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fun"></param>
        /// <returns></returns>
        public void RemoveEventListeners<T>(string key, Action<T> fun)
        {
            _eventCenter.RemoveEventListener(key, fun);
        }
        /// <summary>
        /// 添加本地监听事件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fun"></param>
        /// <returns></returns>
        public bool AddLocalEventListeners<T>(string key, Action<T> fun)
        {
            return AddEventListeners(SocialTools.GetLocalAction(key), fun);
        }
        /// <summary>
        /// 删除事件监听
        /// </summary>  
        /// <param name="key"></param>
        /// <param name="fun"></param>
        public void RemoveLocalEventListener<T>(string key, Action<T> fun)
        {
            RemoveEventListeners(SocialTools.GetLocalAction(key), fun);
        }

        public void DispatchLocalEvent<T>(string key, T data)
        {
            var localKey = SocialTools.GetLocalAction(key);
            _eventCenter.DispatchEvent(localKey, data);
        }
        /// <summary>
        /// 消息拦截（非拦截消息发送到本地）
        /// </summary>
        readonly List<string> _interceptLocalList=new List<string>()
        {
            SocialTools.KeyActionLogin,                       //登录
            SocialTools.KeyActionGetGroupMemberInfo,          //群成员集合信息
            SocialTools.KeyActionGetGroupList,                //群排序列表
            SocialTools.KeyActionGetGroupInfo,                //群信息
            SocialTools.KeyTalkMessage,                       //聊天消息(本地推送信息)
            SocialTools.KeyActionSendMessage,                 //聊天  
            SocialTools.KeyActionSetBlack,                    //添加黑名单成员
            SocialTools.KeyActionBlackList,                   //黑名单列表 
            SocialTools.KeyActionRestoreBlack,                //还原黑名单成员 
            SocialTools.KeyActionFriendRequestList,           //好友申请列表
            SocialTools.KeyActionFriendRequestInfos,          //好友申请信息
            SocialTools.KeyActionRoomList,                    //房间列表ids
            SocialTools.KeyActionRoomListUpdate,              //房间列表刷新推送
            SocialTools.KeyActionRoomInfos,                   //房间列表信息回调
            SocialTools.KeyActionRoomUpdate,                  //房间信息变动推送
            SocialTools.KeyActionOutLineHornList,             //离线喇叭消息
            SocialTools.KeyActionHornUpdate,                  //喇叭消息推送
            SocialTools.KeyActionGroupListUpdate,             //群成员状态变化
            SocialTools.KeyActionFriendRequestUpdate,         //好友请求推送
            SocialTools.KeyActionBlackPush,                   //被拉黑消息推送
            SocialTools.KeyActionBeSendGift,                  //被赠送礼物推送
            SocialTools.KeyActionAddFriend,                   //添加好友请求回调
        };
        /// <summary>
        /// 发送消息变化事件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void DispatchEvent<T>(string key, T data)
        {
            if (_interceptLocalList.Contains(key))
            {
                _eventCenter.DispatchEvent(key, data);
            }
            else
            {
                DispatchLocalEvent(key, data);
            }
        }
        /// <summary>
        /// Socket登录信息
        /// </summary>
        /// <param name="info"></param>
        private void OnGetLoginInfo(Dictionary<string, object> info)
        {
            string gid;
            string id;
            info.TryGetValueWitheKey(out gid, SocialTools.KeyGroupId);
            info.TryGetValueWitheKey(out id, SocialTools.KeyImId);
            info.TryGetValueWitheKey(out EntryNum, SocialTools.KeyEntryNum);
            UserImId = id;
            UserGroupId = gid;
            CurTwManager.SendAction(SocialTools.KeyActionGiftSetting,null, delegate(object data)
            {
                var dic = data as Dictionary<string,object>;
                if (dic!=null)
                {
                    GiftIds.Clear();
                    GiftDataDic.Clear();
                    Dictionary<string,object> configDic;
                    dic.TryGetValueWitheKey(out configDic, SocialTools.KeyConfig);
                    foreach (var giftItem in configDic)
                    {
                        string giftId;
                        var itemDic = giftItem.Value as Dictionary<string, object>;
                        if (itemDic!=null)
                        {
                            itemDic.TryGetValueWitheKey(out giftId, SocialTools.KeyId);
                            if (!string.IsNullOrEmpty(giftId) && !GiftIds.Contains(giftId))
                            {
                                GiftIds.Add(giftId);
                                GiftDataDic.Add(giftId, itemDic);
                            }
                        }
                    }
                }
            },true,null,false);
            _talkManager.InitList(info);
            _hornManager.GetOutLineList();
            _roomManager.InitList();
        }

        /// <summary>
        /// 重置游戏监听
        /// </summary>
        public void ResetGameListeners()
        {
            if(Facade.HasInstance<SocialTalkCenter>())
            {
                _talkManager.ReCheckGameListener();
            }
        }


        public override void OnDestroy()
        {
            if (_hasInit)
            {
                _eventCenter.RemoveAllEventListener();
                CloseSocket();
            }
            base.OnDestroy();
        }

        void OnApplicationFocus(bool state)
        {
            if (state)
            {
                ConnectSocket();
            }
        }
        void OnApplicationQuit()
        {
            CloseSocket();
        }
    }

    /// <summary>
    /// 目标类型（当前命名与服务器一致，直接使用toString作为请求参数）
    /// </summary>
    public enum TalkTargetType
    {
        friend=0,
        group=1
    }

    /// <summary>
    /// 目标类型（当前命名与服务器一致，直接使用toString作为请求参数）
    /// </summary>
    public enum TalkContentType
    {
        Text = 0,
        Phiz = 1,
        Image=2,
        Voice=3
    }
}
