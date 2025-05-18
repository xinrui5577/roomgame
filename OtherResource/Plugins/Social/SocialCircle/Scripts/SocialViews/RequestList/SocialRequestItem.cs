using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using com.yxixia.utile.Utiles;
using UnityEngine;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.RequestList
{
    /// <summary>
    /// 亲友圈好友请求item
    /// </summary>
    public class SocialRequestItem : BaseSocialWrapHeadItem
    {
        [Tooltip("来源名称")]
        public string SourceName;
        [Tooltip("忽略事件")]
        public List<EventDelegate> IgnoreAction=new List<EventDelegate>();
        [Tooltip("确认事件")]
        public List<EventDelegate> AgreeAction = new List<EventDelegate>();
        [Tooltip("拒绝事件")]
        public List<EventDelegate> RefuseAction = new List<EventDelegate>();
        [Tooltip("显示按钮组事件")]
        public List<EventDelegate> ShowBtnsAction = new List<EventDelegate>();
        [Tooltip("是否显示忽略按钮")]
        [HideInInspector]
        public bool ShowIgnore;
        [Tooltip("是否显示同意状态提示")]
        [HideInInspector]
        public bool ShowAgree;
        [Tooltip("是否显示拒绝信息")]
        [HideInInspector]
        public bool ShowRefuseInfo;
        [Tooltip("是否显示按钮组")]
        [HideInInspector]
        public bool ShowBtns;
        protected override void DealFreshData()
        {
            base.DealFreshData();
            ParseDicData.TryGetValueWitheKey(out SourceName, SocialTools.KeySourceName);
            RequestHandleType handEnum= RequestHandleType.none;
            DictionaryHelper.ParseEnum(ParseDicData, SocialTools.KeyHandles, ref handEnum);
            ShowIgnore = handEnum != RequestHandleType.ignore;
            ShowAgree= handEnum== RequestHandleType.agree;
            ShowRefuseInfo = handEnum == RequestHandleType.refuse;
            ShowBtns =!(ShowRefuseInfo||ShowAgree);
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(ShowBtnsAction.WaitExcuteCalls(true));
                StartCoroutine(IgnoreAction.WaitExcuteCalls(true));
                StartCoroutine(AgreeAction.WaitExcuteCalls(true));
                StartCoroutine(RefuseAction.WaitExcuteCalls(true));
            }
        }

    }
    /// <summary>
    /// 请求处理类型
    /// </summary>
    public enum  RequestHandleType
    {
        none,        //未知类型
        not_operated,//未操作
        refuse,      //拒绝
        agree,       //同步
        ignore       //忽略
    }
}
