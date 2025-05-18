using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Manager;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxPlugins.Interfaces;

namespace Assets.Scripts.Common.YxPlugins.Social.Managers
{
    public class ZySocialManager : IBasePlugins
    {
        public void Init()
        {
            _manager=Facade.Instance<SocialMessageManager>().InitManager(); 
        }

        private static SocialMessageManager _manager;
        private static SocialMessageManager Manager
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

        /// <summary>
        /// 登录事件
        /// </summary>
        /// <param name="obj"></param>
        private static void OnSuccess(LoginInfo obj)
        {
            YxDebug.LogError("大厅登录成功事件");
            Manager.ConnectSocket(true);
        }
        /// <summary>
        /// 重连时间
        /// </summary>
        private static void OnGameing(object data)
        {
            YxDebug.LogError("游戏中");
            Manager.ConnectSocket();
        }

        /// <summary>
        /// 进入大厅还是登录界面
        /// </summary>
        /// <param name="hasLogin"></param>
        private static void OnHallState(bool hasLogin)
        {
            if (!hasLogin)
            {
                Manager.CloseSocket();
            }
        }

        public void Destroy()
        {

        }

        public void Reset()
        {
            var center = Facade.EventCenter;
            center.RemoveEventListener<YxESysEventType, bool>(YxESysEventType.SysHallState, OnHallState);
            center.RemoveEventListener<YxESysEventType, LoginInfo>(YxESysEventType.SysLoginSuccess, OnSuccess);
            center.RemoveEventListener<YxESysEventType, object>(YxESysEventType.SysInGame, OnGameing);
            center.AddEventListeners<YxESysEventType, bool>(YxESysEventType.SysHallState, OnHallState);
            center.AddEventListeners<YxESysEventType, LoginInfo>(YxESysEventType.SysLoginSuccess, OnSuccess);
            center.AddEventListeners<YxESysEventType, object>(YxESysEventType.SysInGame, OnGameing);
            if (Facade.HasInstance<SocialMessageManager>())
            {
                Manager.ResetGameListeners();
            }
        }
    }
}
