using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.DataBundles;
using YxFramwork.Common.Model;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.Horn
{
    /// <summary>
    /// 亲友圈小工具(喇叭+热度名次+请求名次)
    /// </summary>
    public class SocialHornView : BaseSocialView
    {
        [Tooltip("附加长度")]
        private float _patchLength = 150;
        [Tooltip("喇叭消息数量")]
        public YxBaseLabelAdapter MessageNum;
        [Tooltip("最新请求头像")]
        public YxBaseTextureAdapter NewRequestTex;
        [Tooltip("请求消息数量")]
        public YxBaseLabelAdapter RequestNum;
        [Tooltip("排行名次数量")]
        public YxBaseLabelAdapter RankNum;
        [Tooltip("喇叭显示内容")]
        public YxBaseLabelAdapter HornContentInfo;
        [Tooltip("喇叭消息数量变化事件")]
        public List<EventDelegate> HornNumAction = new List<EventDelegate>();
        [Tooltip("好友申请消息数量变化事件")]
        public List<EventDelegate> RequestNumAction = new List<EventDelegate>();
        [Tooltip("显示面板")]
        public UIPanel ShowPanel;
        /// <summary>
        /// 显示喇叭数量相关
        /// </summary>
        public bool ShowHornNum { get; private set; }
        /// <summary>
        /// 显示请求数量相关
        /// </summary>
        public bool ShowRequestNum { get; private set; }
        private float _deltaTime = 0.01f;
        private int _moveSpeed =4;
        private string _currentMessageId;
        private string _currentContent;
        private Vector3 _contentBaseVec;
        private bool _needMove;
        private float _panelShowWidth;
        /// <summary>
        /// 喇叭中心
        /// </summary>
        private SocialHornCenter _hornManager
        {
            get { return Facade.Instance<SocialHornCenter>().InitCenter(); }
        }

        protected override void AddListeners()
        {
            AddEventListener<Dictionary<string,object>>(SocialTools.KeyActionHornList, OnInitReceive);
            AddEventListener<Dictionary<string,object>>(SocialTools.KeyRankNumChange,OnRankNumChange);
            AddEventListener<Dictionary<string, object>>(SocialTools.KeyActionFriendRequestList, OnRequestNumChange);
        }

        protected override void RemoveListeners()
        {
            RemoveEventListener<Dictionary<string, object>>(SocialTools.KeyActionHornList, OnInitReceive);
            RemoveEventListener<Dictionary<string, object>>(SocialTools.KeyRankNumChange, OnRankNumChange);
            RemoveEventListener<Dictionary<string, object>>(SocialTools.KeyActionFriendRequestList, OnRequestNumChange);
        }

        protected override void OnAwake()
        {
            if (HornContentInfo)
            {
                _contentBaseVec = HornContentInfo.transform.localPosition;
            }

            if (ShowPanel)
            {
                var con = ShowPanel.worldCorners;
                _panelShowWidth=(transform.InverseTransformPoint(con[2]) - transform.InverseTransformPoint(con[0])).x;
            }
            base.OnAwake();
        }

        protected override void OnVisible()
        {
            TalkCenter.GetSortList();
            _hornManager.GetShowList();
            OnRankNumChange();
            OnRequestNumChange();
        }

        private void OnRankNumChange(Dictionary<string,object> data=null)
        {
            RankNum.TrySetComponentValue(TalkCenter.RankInfo);
        }

        private void OnRequestNumChange(Dictionary<string, object> data = null)
        {
            ShowRequestNum = TalkCenter.FriendRequestNum > 0;
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(RequestNumAction.WaitExcuteCalls());
            }
            RequestNum.TrySetComponentValue(TalkCenter.FriendRequestNum);
            var lastHead = TalkCenter.LastItemHeadUrl;
            if (NewRequestTex&&!string.IsNullOrEmpty(lastHead))
            {
                PortraitDb.SetPortrait(lastHead, NewRequestTex,1);
            }
        }

        private int _showIndex;

        protected override void OnInitDataValid()
        {
            ShowHornNum = InitGetData.Count>0;
            if (HornContentInfo&& ShowHornNum)
            {
                if (string.IsNullOrEmpty(_currentContent))
                {
                    InitGetData.TryGetValueWitheKey(out _currentContent, SocialTools.KeyMessageContent);
                    InitGetData.TryGetValueWitheKey(out _currentMessageId, SocialTools.KeyId);
                    HornContentInfo.TrySetComponentValue(_currentContent);
                    _needMove = true;
                    MoveContent();
                }
            }
            MessageNum.TrySetComponentValue(_hornManager.GetHornUnReadCount);
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(HornNumAction.WaitExcuteCalls());
            }
        }

        private Coroutine _moveCor;
        private void MoveContent()
        {
            if (HornContentInfo)
            {
                if (_needMove)
                {
                    var targetVecX = _contentBaseVec.x + (HornContentInfo.Width + _panelShowWidth + _patchLength);
                    var times = (int)targetVecX / _moveSpeed;
                    if (_moveCor!=null)
                    {
                        StopCoroutine(_moveCor);
                    }
                    _moveCor=StartCoroutine(Move(Math.Abs(times)));
                }
            }
        }

        IEnumerator Move(int times)
        {
            if (_needMove)
            {
                var dealTime = times;
                while (true)
                {
                    yield return new WaitForSeconds(_deltaTime);
                    HornContentInfo.transform.localPosition += Vector3.left * _moveSpeed;
                    dealTime--;
                    if (dealTime<= 0)
                    {
                        dealTime = times;
                        HornContentInfo.transform.localPosition = _contentBaseVec;
                    }
                }
            }
            else
            {
                yield break;
            }
        }

        public void ShowNextMessage()
        {
            if (HornContentInfo)
            {
                if (!string.IsNullOrEmpty(_currentMessageId) && !string.IsNullOrEmpty(_currentContent))
                {
                    _needMove = false;
                    _currentContent = string.Empty;
                    HornContentInfo.TrySetComponentValue(_currentContent);
                    HornContentInfo.transform.localPosition = _contentBaseVec;
                    _hornManager.SetHornListReadFinish(_currentMessageId);
                }
            }
        }
        #region
        //test1 模拟游戏内打开亲友面板
        public void OnOpenFriendWindow()
        {
            var dic = new Dictionary<string, object>()
            {
                {"rndId",645112}
            };
            var window=YxWindowManager.OpenWindow("SocialFriendListWindow");
            if (window)
            {
                window.UpdateView(dic);
            }
        }
        //test2 模拟分享游戏内截屏到亲友圈
        [ContextMenu("测试发送图片")]
        public void OnSendToScoialGroup()
        {
            string url= "Assets/Skins/DefaultSkins/skin_0021/loading/LOGO 1.png";
            url = Path.GetFullPath(url);
            var dic = new Dictionary<string, object>()
            {
                {SocialTools.KeyId,App.UserId}//玩家id
                ,{SocialTools.KeyData,url}//截图路径
            };
            Facade.EventCenter.DispatchEvent(SocialTools.KeyShareToSocial, dic);
        }
        public new void OnOpenUserInfoWindow()
        {
            Debug.LogError("点击玩家头像!");
            if (App.AppStyle == YxEAppStyle.Concise)
                return;
            YxWindow yxWindow = YxWindowManager.OpenWindow("SocialUserGameInfoWindow");
            if (yxWindow == null)
                return;
            yxWindow.UpdateView(UserInfoModel.Instance.UserInfo);
        }
        #endregion
    }
}
