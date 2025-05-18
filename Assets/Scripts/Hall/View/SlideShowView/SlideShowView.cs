/*===================================================
 *文件名称:     SlideShowView.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-10-23
 *描述:        	新版轮播图（samereturn结构）
 *历史记录: 
=====================================================*/

using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Hall.View.PageListWindow;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.SlideShowView
{
    public class SlideShowView : YxSameReturnView
    {
        #region UI Param
        [Tooltip("必要参数：用于显示轮播的数据")]
        public YxView PrefabItem;
        [Tooltip("必要参数：Item容器")]
        public UIWrapContent ItemContainer;
        [Tooltip("必要参数,显示ScrollView")]
        public UIScrollView ScrollView;
        #endregion

        #region Data Param
        [Tooltip("点击Url连接是否跳转到浏览器")]
        public bool LinkToBrowser = true;

        #endregion

        #region Local Data
        /// <summary>
        /// 移动状态
        /// </summary>
        private bool _moveState;
        /// <summary>
        /// 等待时间
        /// </summary>
        private float _stayTime;
        /// <summary>
        /// 移动速度
        /// </summary>
        private float _moveSpeed;
        /// <summary>
        /// 移动方向（1:正向 -1:反方向）
        /// </summary>
        private int _moveDirection;
        /// <summary>
        /// 当前等待时间
        /// </summary>
        private float _curStatTime;
        #endregion

        #region Life Cycle

        protected override void OnStart()
        {
            base.OnStart();
            if (ScrollView==null)
            {
                ScrollView = ItemContainer.GetComponent<UIScrollView>();
            }
        }
        /// <summary>
        /// 等待协程
        /// </summary>
        private Coroutine _coroutine;
        protected override void DealShowData()
        {
            if(Data is Dictionary<string,object>&&ItemContainer && PrefabItem)
            {
                var dic = Data as Dictionary<string, object>;
                var showDatas=new SlideShowData(dic, GetItemType());
                var count = showDatas.DataItems.Count;
                _stayTime = showDatas.StayTime;
                _moveSpeed = Math.Abs(showDatas.MoveSpeed);
                _moveDirection = showDatas.MoveSpeed > 0 ? 1 : -1;
                _curStatTime = 0;
                for (int i = 0; i < count; i++)
                {
                    var view = ItemContainer.transform.GetChildView(i,PrefabItem);
                    if (view)
                    {
                        view.UpdateView(showDatas.DataItems[i]);
                    }
                }
                ItemContainer.SortBasedOnScrollMovement();
                if (count<= 1)
                {
                    return;
                }
                if (gameObject.activeInHierarchy)
                {
                    _coroutine = StartCoroutine(SlideWait());
                }
            }
        }

        #endregion

        #region Function

        /// <summary>
        /// 获得 Item 数据
        /// </summary>
        /// <returns></returns>
        private Type GetItemType()
        {
            return typeof (SlideItemData);
        }
        /// <summary>
        /// 点击跳转Url(两种模式：1，跳转外置浏览器)
        /// </summary>
        /// <param name="linkUrl"></param>
        /// <param name="windowName"></param>
        public void OnClickLinkUrl(string linkUrl,string windowName)
        {
            if (string.IsNullOrEmpty(linkUrl))
            {
                return;
            }
            if (LinkToBrowser)
            {
                OnOpenUrl(linkUrl);
            }
            else
            {
                MainYxView.OpenWindowWithData(windowName,linkUrl);
            }
        }

        /// <summary>
        /// 轮播图滚动
        /// </summary>
        private void SlideMove()
        {
            if (!_moveState)
            {
                var showView = ScrollView.panel.GetViewSize();
                var moveVec = new Vector3(showView.x * _moveDirection, 0);
                _moveState = true;
                SpringPanel.Begin(ScrollView.panel.cachedGameObject,
                    ScrollView.transform.localPosition - moveVec,_moveSpeed).onFinished += delegate
                    {
                        if (gameObject.activeInHierarchy)
                        {
                            _coroutine = StartCoroutine(SlideWait());
                        }
                    };
            }
        }

        /// <summary>
        /// 轮播图等待
        /// </summary>
        /// <returns></returns>
        IEnumerator SlideWait()
        {
            _curStatTime = _stayTime;
            while (_curStatTime>0)
            {
                _curStatTime -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            if (_coroutine != null)
            {
                if (gameObject.activeInHierarchy)
                {
                    StopCoroutine(SlideWait());
                    _moveState = false;
                    SlideMove();
                }
            }
        }

        #endregion
    }

    public class SlideShowData : YxData
    {
        /// <summary>
        /// Key 移动速度
        /// </summary>
        private const string KeyMoveSpeed= "MoveSpeed";

        /// <summary>
        /// Key 停留时间
        /// </summary>
        private const string KeyStayTime = "StayTime";


        /// <summary>
        /// 移动时间
        /// </summary>
        public float MoveSpeed { private set; get; }

        /// <summary>
        /// 停留时间
        /// </summary>
        public float StayTime { private set; get; }


        public SlideShowData(object data) : base(data)
        {
        }

        public SlideShowData(object data, Type type) : base(data, type)
        {
        }

        protected override void ParseData(Dictionary<string, object> dic)
        {
            base.ParseData(dic);
            float moveSpeed;
            float stayTime;
            dic.TryGetValueWitheKey(out moveSpeed, KeyMoveSpeed);
            dic.TryGetValueWitheKey(out stayTime, KeyStayTime);
            MoveSpeed = moveSpeed;
            StayTime = stayTime;
        }
    }
}
