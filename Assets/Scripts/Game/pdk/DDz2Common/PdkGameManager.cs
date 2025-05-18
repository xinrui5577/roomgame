using System;
using System.Collections.Generic;
using Assets.Scripts.Game.pdk.DdzEventArgs;
using Assets.Scripts.Game.pdk.InheritCommon;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.ConstDefine;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.pdk.DDz2Common
{
    public class PdkGameManager : YxGameManager
    {
        /// <summary>
        /// 场景销毁后，重置静态变量
        /// </summary>
        public override void OnDestroy()
        {
            _onUserOutEvt = null;
            _onUserIdle = null;
            _onGetGameInfoEvt = GlobalData.OnGetGameInfo;
            _onGetRejoinDataEvt = GlobalData.OnGetRejoinData;
            OnServResponseEvtDic.Clear();
        }
        private static EventHandler<DdzbaseEventArgs> _onUserOutEvt;
        /// <summary>
        /// 添加服务器OnUserOut事件
        /// </summary>
        /// <param name="eventHandler"></param>
        public static void AddOnUserOutEvt(EventHandler<DdzbaseEventArgs> eventHandler)
        {
            _onUserOutEvt += eventHandler;
        }

        /// <summary>
        /// 玩家空闲时事件
        /// </summary>
        private static EventHandler<DdzbaseEventArgs> _onUserIdle;
        /// <summary>
        /// 获得gps信息
        /// </summary>
        /// <param name="eventHandler"></param>
        public static void AddOnUserIdle(EventHandler<DdzbaseEventArgs> eventHandler)
        {
            _onUserIdle += eventHandler;
        }

        private static EventHandler<DdzbaseEventArgs> _onGetGameInfoEvt = GlobalData.OnGetGameInfo;
        /// <summary>
        /// 添加服务器OnGameInfo事件
        /// </summary>
        /// <param name="eventHandler"></param>
        public static void AddOnGameInfoEvt(EventHandler<DdzbaseEventArgs> eventHandler)
        {
            _onGetGameInfoEvt += eventHandler;
        }

        private static EventHandler<DdzbaseEventArgs> _onGetRejoinDataEvt = GlobalData.OnGetRejoinData;
        /// <summary>
        /// 服务器OnGetRejoinData事件
        /// </summary>
        /// <param name="eventHandler"></param>
        public static void AddOnGetRejoinDataEvt(EventHandler<DdzbaseEventArgs> eventHandler)
        {
            _onGetRejoinDataEvt += eventHandler;
        }

        private static readonly Dictionary<int, EventHandler<DdzbaseEventArgs>> OnServResponseEvtDic = new Dictionary<int, EventHandler<DdzbaseEventArgs>>();
        /// <summary>
        /// 添加服务器OnServResponse事件相关集合
        /// </summary>
        /// <param name="keyType"></param>
        /// <param name="eventHandler"></param>
        public static void AddOnServResponseEvtDic(int keyType, EventHandler<DdzbaseEventArgs> eventHandler)
        {
            if (!OnServResponseEvtDic.ContainsKey(keyType))
            {
                OnServResponseEvtDic[keyType] = eventHandler;
                return;
            }
            OnServResponseEvtDic[keyType] += eventHandler;
        }



        public override void UserOut(int localSeat, ISFSObject responseData)
        {
            if (_onUserOutEvt == null) return;

            ISFSObject data = responseData;
            data.PutInt(RequestKey.KeySeat, data.GetInt(RequestKey.KeySeat));
            var eventArgs = new DdzbaseEventArgs(data);

            _onUserOutEvt(this, eventArgs);
        }

        public override void UserIdle(int localSeat, ISFSObject responseData)
        {
            if (_onUserIdle == null) return;

            ISFSObject data = responseData;
            data.PutInt(RequestKey.KeySeat, data.GetInt(RequestKey.KeySeat));
            data.PutBool(NewRequestKey.KeyUserIdle, true);
            var eventArgs = new DdzbaseEventArgs(data);

            _onUserIdle(this, eventArgs);
        }

        public override void UserOnLine(int localSeat, ISFSObject responseData)
        {
            if (_onUserIdle == null) return;

            ISFSObject data = responseData;
            data.PutInt(RequestKey.KeySeat, data.GetInt(RequestKey.KeySeat));
            data.PutBool(NewRequestKey.KeyUserIdle, false);
            var eventArgs = new DdzbaseEventArgs(data);

            _onUserIdle(this, eventArgs);
        }

        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            if (_onGetGameInfoEvt == null) return;
            var eventArgs = new DdzbaseEventArgs(gameInfo);
            _onGetGameInfoEvt(this, eventArgs);
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
            if (_onGetRejoinDataEvt == null) return;
            var eventArgs = new DdzbaseEventArgs(gameInfo);
            _onGetRejoinDataEvt(this, eventArgs);
        }

        public override void GameStatus(int status, ISFSObject info)
        {
        
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            var keyType = response.GetInt(RequestKey.KeyType);

            if (!OnServResponseEvtDic.ContainsKey(keyType)) return;

            var eventArgs = new DdzbaseEventArgs(response);
            OnServResponseEvtDic[keyType](this, eventArgs);
        }
        public override int OnChangeRoom()
        {
            return base.OnChangeRoom();
        }
        public override void OnChangeRoomClick()
        {
            base.OnChangeRoomClick();
        }
    }
}
