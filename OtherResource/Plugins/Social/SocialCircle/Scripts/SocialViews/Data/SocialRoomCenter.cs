using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Manager;
using com.yxixia.utile.YxDebug;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data
{
    /// <summary>
    /// 房间信息中心
    /// </summary>
    public class SocialRoomCenter : BaseMono
    {
        /// <summary>
        /// 是否初始化
        /// </summary>
        private bool _hasInit;
        /// <summary>
        /// 房间列表
        /// </summary>
        private List<string> _roomList = new List<string>();
        /// <summary>
        /// 房间数据
        /// Key：SocialTools.KeyId
        /// value：房间信息
        /// </summary>
        private Dictionary<string, Dictionary<string, object>> _roomInfos = new Dictionary<string, Dictionary<string, object>>();
        public SocialRoomCenter InitCenter()
        {
            if (!_hasInit)
            {
                InitListeners();
                _hasInit = true;
            }

            return this;
        }

        private SocialMessageManager _manager;

        private SocialMessageManager Manager
        {
            get
            {
                if (_manager == null)
                {
                    _manager = Facade.Instance<SocialMessageManager>().InitManager();
                }

                return _manager;
            }
        }

        void InitListeners()
        {
            //接收到房间列表
            Manager.AddEventListeners<Dictionary<string, object>>(SocialTools.KeyActionRoomList, OnReceiveRoomList);
            //房间信息集合
            Manager.AddEventListeners<Dictionary<string, object>>(SocialTools.KeyActionRoomInfos, OnReceiveRoomInfos);
            //房间变化推送
            Manager.AddEventListeners<Dictionary<string, object>>(SocialTools.KeyActionRoomUpdate, OnRoomInfoUpdate);
            //房间列表刷新推送
            Manager.AddEventListeners<Dictionary<string, object>>(SocialTools.KeyActionRoomListUpdate, OnRoomListInfoUpdate);
        }

        void RemoveListeners()
        {
            //群成员信息集合
            Manager.RemoveEventListeners<Dictionary<string, object>>(SocialTools.KeyActionRoomList, OnReceiveRoomList);
            //房间信息集合
            Manager.RemoveEventListeners<Dictionary<string, object>>(SocialTools.KeyActionRoomInfos, OnReceiveRoomInfos);
            //房间变化推送
            Manager.RemoveEventListeners<Dictionary<string, object>>(SocialTools.KeyActionRoomUpdate, OnRoomInfoUpdate);
            //房间列表刷新推送
            Manager.RemoveEventListeners<Dictionary<string, object>>(SocialTools.KeyActionRoomListUpdate, OnRoomListInfoUpdate);
        }

        private void OnReceiveRoomList(Dictionary<string, object> roomListData)
        {
            List<string> newIdList;
            roomListData.TryGetStringListWithKey(out newIdList, SocialTools.KeyIds);
            if (roomListData.Count==0|| roomListData.ValueEqual(_roomList))
            {
                SendDatasToLocal();
                return;
            }
            _roomList = newIdList.ToList();
            var needList=new List<string>();
            for (int i = 0,length= _roomList.Count; i < length; i++)
            {
                var itemKey = _roomList[i];
                if (!_roomInfos.ContainsKey(itemKey))
                {
                    needList.Add(itemKey);
                }
            }

            if (needList.Count==0)
            {
                SendDatasToLocal();
                return;
            }
            int requestLength;
            roomListData.TryGetValueWitheKey(out requestLength, SocialTools.KeyDataNum, 6);
            var requestCount = needList.Count;
            var requestTime = requestCount / requestLength + (requestCount % requestLength == 0 ? 0 : 1);
            for (int i = 0; i < requestTime; i++)
            {
                var starIndex = i * requestLength;
                var leftCount = requestCount - starIndex;
                var requestList = needList.GetRange(starIndex, Math.Min(leftCount, requestLength));
                Manager.SendRequest(SocialTools.KeyActionRoomInfos, new Dictionary<string, object>()
                {
                    { SocialTools.KeyIds,requestList}, {SocialTools.KeyDefKey,new Dictionary<string,object>()
                    {
                        { SocialTools.KeyIsLastRequest,leftCount <= requestLength}
                    }}
                });
            }
        }

        private void OnReceiveRoomInfos(Dictionary<string, object> roomInfos)
        {
            List<object> infos;
            roomInfos.TryGetValueWitheKey(out infos, SocialTools.KeyIds);
            var length = infos.Count;
            for (int i = 0; i < length; i++)
            {
                var item = infos[i] as Dictionary<string, object>;
                if (item != null)
                {
                    string id;
                    item.TryGetValueWitheKey(out id, SocialTools.KeyId);
                    if (!string.IsNullOrEmpty(id))
                    {
                        _roomInfos[id] = item;
                    }
                }
            }
            Dictionary<string, object> defaultData;
            roomInfos.TryGetValueWitheKey(out defaultData, SocialTools.KeyDefKey);
            if ((bool)defaultData[SocialTools.KeyIsLastRequest])
            {
                SendDatasToLocal();
            }

        }
        /// <summary>
        /// 是否需要请求数据进行刷新
        /// </summary>
        private bool _needRequest;
        private void OnRoomInfoUpdate(Dictionary<string, object> roomInfos)
        {
            int updateType;
            roomInfos.TryGetValueWitheKey(out updateType, SocialTools.KeyUpdateType);
            string roomId;
            roomInfos.TryGetValueWitheKey(out roomId, SocialTools.KeyId);
            switch (updateType)
            {
                case -1:
                    if (_roomList.Contains(roomId))
                    {
                        _roomList.Remove(roomId);
                        if(_roomInfos.ContainsKey(roomId))
                        {
                            _roomInfos.Remove(roomId);
                        }
                    }
                    Manager.DispatchLocalEvent(SocialTools.KeyActionRoomUpdate, roomInfos);
                    break;
                case 0:
                case 1:
                    _needRequest = true;
                    _roomInfos[roomId] = roomInfos;
                    Manager.DispatchLocalEvent(SocialTools.KeyActionRoomUpdate, roomInfos);
                    break;
                default:
                    YxDebug.LogEvent(string.Format(SocialTools.KeyNoticePushDataError, SocialTools.KeyActionRoomUpdate,updateType));
                    break;
            }
        }


        private void OnRoomListInfoUpdate(Dictionary<string, object> roomInfos)
        {
            _needRequest = true;
            GetShowList();
        }

        /// <summary>
        /// 获取显示数据
        /// </summary>
        public void GetShowList()
        {
            if (_needRequest)
            {
                InitList();
            }
            else
            {
                SendDatasToLocal();
            }
        }

        public void ResetData()
        {
            _needRequest = true;
            _roomList.Clear();
            _roomInfos.Clear();
        }

        void OnApplicationQuit()
        {
            ResetData();
        }

        /// <summary>
        /// 发送喇叭消息数据到本地
        /// </summary>
        private void SendDatasToLocal()
        {
            var hornInfos = new Dictionary<string, object>();
            hornInfos[SocialTools.KeyIds] = _roomList;
            hornInfos[SocialTools.KeyData] = SocialTools.SelectDicDataFromList(_roomList, _roomInfos);
            Manager.DispatchLocalEvent(SocialTools.KeyActionRoomList, hornInfos);
        }

        public void InitList()
        {
            _needRequest = false;
            Manager.SendRequest(SocialTools.KeyActionRoomList);
        }

        public override void OnDestroy()
        {
            RemoveListeners();
            base.OnDestroy();
        }
    }
}