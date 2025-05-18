using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Manager;
using UnityEngine;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;


namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base
{
    /// <summary>
    /// 两种初始化方式：
    /// 1.在预设上面配置外置参数，页面打开时会自动请求
    /// 2.预设参数无数据，请求数据由上级界面传入
    /// </summary>
    public class BaseSocialView : YxView
    {
        [Tooltip("初始化Url")]
        public string InitAction = "";
        [Tooltip("显示时发送请求")]
        public bool EnableWithGetData = true;
        #region Life cycle

        private SocialTalkCenter _center;
        protected SocialTalkCenter TalkCenter
        {
            get
            {
                if (_center == null)
                {
                    _center = Facade.Instance<SocialTalkCenter>().InitCenter();
                }
                return _center;
            }
        }


        public string CurrentImId
        {
            get
            {
                return Manager.UserImId; 
            }
        }

        public string CurrentGroupId
        {
            get
            {
                return Manager.UserGroupId;
            }
        }
        public string TargetId
        {
            get
            {
                return TalkCenter.TalkId;
            }
        }
        private SocialMessageManager _manager;
        protected SocialMessageManager Manager
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

        /// <summary>
        ///  post 参数
        /// </summary>
        /// <returns></returns>
        protected virtual Dictionary<string, object> GetPostParam()
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            return param;
        }

        /// <summary>
        /// 收到服务器返回的页面初始化数据
        /// </summary>
        /// <param name="data"></param>
        protected void OnInitReceive(Dictionary<string, object> data)
        {
            InitGetData = data;
            if (InitGetData != null)
            {
                OnInitDataValid();
            }
        }
        /// <summary>
        /// 页面初始化数据(成功数据主体data)
        /// </summary>
        protected Dictionary<string, object> InitGetData=new Dictionary<string, object>();

        /// <summary>
        /// 初始化数据
        /// </summary>
        protected virtual void OnInitDataValid()
        {

        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="action"></param>
        /// <param name="param"></param>
        protected void SendSocialMessage(string action,Dictionary<string,object> param=null)
        {
            if (string.IsNullOrEmpty(action))
            {
                return;
            }
            Manager.SendRequest(action,param);
        }
        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fun"></param>
        /// <returns></returns>
        protected bool AddEventListener<T>(string key, Action<T> fun)
        {
            return Manager.AddLocalEventListeners(key, fun);
        }

        /// <summary>
        /// 删除事件监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fun"></param>
        protected void RemoveEventListener<T>(string key, Action<T> fun)
        {
             Manager.RemoveLocalEventListener(key, fun);
        }
        #endregion
        #region View life cicle

        protected override void OnAwake()
        {
            AddListeners();
            base.OnAwake();
        }

        public override void OnDestroy()
        {
            if (Facade.HasInstance<SocialMessageManager>())
            {
                RemoveListeners();
                if (Data is Dictionary<string, object>)
                {
                    var dic = Data as Dictionary<string, object>;
                    if (dic.ContainsKey(SocialTools.KeyCallBack))
                    {
                        var callList = dic[SocialTools.KeyCallBack] as List<EventDelegate>;
                        if (callList != null)
                        {
                            EventDelegate.Execute(callList);
                        }
                    }
                }
            }
            base.OnDestroy();
        }

        protected override void OnVisible()
        {
            base.OnVisible();
            _center = TalkCenter;
            if (EnableWithGetData)
            {
                SendSocialMessage(InitAction);
            }
        }

        protected virtual void AddListeners()
        {
            AddEventListener<Dictionary<string,object>>(InitAction, OnInitReceive);
        }

        protected virtual void RemoveListeners()
        {
            RemoveEventListener<Dictionary<string, object>>(InitAction, OnInitReceive);
        }

        protected void ChangeTalkTarget(string groupId,YxView showView=null)
        {
            Dictionary<string, object> _targetGroupInfo;
            if (TalkCenter.SetTalkTarget(groupId, out _targetGroupInfo))
            {
                if (showView == null)
                {
                    MainYxView.OpenWindowWithData("SocialChatContentWindow", null);
                }
                Manager.DispatchLocalEvent(SocialTools.KeyTalkTargetChange, _targetGroupInfo);
            }
        }

        /// <summary>
        /// 点击头像后打开相应面板(子窗口)，请求php并展示相应的信息
        /// </summary>
        /// <param name="userId">查找Id</param>
        /// <param name="windowName">窗口名称</param>
        /// <param name="socialSourceName">亲友圈来源名称</param>
        /// <param name="callBack"></param>
        public virtual void OnClickHead(string userId,string windowName="SocialOtherUserInfoWindow",string socialSourceName="", List<EventDelegate> callBack = null)
        {
            var dic = new Dictionary<string, object>()
            {
                {SocialTools.KeyImId, userId}
            };
            if (!string.IsNullOrEmpty(socialSourceName))
            {
                dic[SocialTools.KeySourceName] = socialSourceName;
            }

            if (callBack!=null)
            {
                dic[SocialTools.KeyCallBack] = callBack;
            }
            MainYxView.OpenWindowWithData(windowName,dic);
        }

    }
    #endregion
}