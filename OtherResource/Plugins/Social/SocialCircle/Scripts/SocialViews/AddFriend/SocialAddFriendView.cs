using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using UnityEngine;
using YxFramwork.View;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.AddFriend
{
    /// <summary>
    /// 添加亲友列表
    /// </summary>
    public class SocialAddFriendView : BaseSocialSelectWrapListView
    {
        [Tooltip("选项")]
        public List<string> Options = new List<string>();
        [Tooltip("排序类型")]
        public SocialFriendSortType SortType;
        [Tooltip("监听黑名单变化")]
        public bool ListenBlackListChange = true;
        /// <summary>
        /// 缓存数据
        /// </summary>
        protected Dictionary<string, Dictionary<string, object>> CacheDic = new Dictionary<string, Dictionary<string, object>>();
        protected override void AddListeners()
        {
            AddEventListener<Dictionary<string, object>>(InitAction, OnInitReceive);
            AddEventListener<Dictionary<string, object>>(PartAction, OnPart);
            if (ListenBlackListChange)
            {
                //拉黑好友
                AddEventListener<Dictionary<string, object>>(SocialTools.KeyActionSetBlack, SetFriendToBlackList);
            }
        }

        protected override void OnVisible()
        {
            RequestWithSortType();
        }

        protected override void RemoveListeners()
        {
            RemoveEventListener<Dictionary<string, object>>(InitAction, OnInitReceive);
            RemoveEventListener<Dictionary<string, object>>(PartAction, OnPart);
            if (ListenBlackListChange)
            {
                //拉黑好友
                RemoveEventListener<Dictionary<string, object>>(SocialTools.KeyActionSetBlack, SetFriendToBlackList);
            }
        }
        protected int MaxPerLength = 6;
        /// <summary>
        /// ids 列表key
        /// </summary>
        /// <returns></returns>
        protected virtual string GetIdsKey()
        {
            return SocialTools.KeyFirends;
        }

        protected override void OnInitDataValid()
        {
            InitGetData.TryGetStringListWithKey(out PageIds, GetIdsKey());
            CacheDic.Clear();
            CheckVaild();
            var idLength = PageIds.Count;
            if (idLength == 0)
            {
                FreshWrapList(new Dictionary<string, Dictionary<string, object>>());
            }
            else
            {
                var requestTime = idLength / MaxPerLength + (idLength % MaxPerLength == 0 ? 0 : 1);
                for (int i = 0; i < requestTime; i++)
                {
                    var starIndex = i * MaxPerLength;
                    var leftCount = idLength - starIndex;
                    var requestList = PageIds.GetRange(starIndex, Math.Min(leftCount, MaxPerLength));
                    var sendDic = new Dictionary<string, object>
                    {
                        {SocialTools.KeyIds,requestList}
                    }.SetDefKeyDic(new Dictionary<string, object>()
                    {
                        { SocialTools.KeyFromAction, InitAction}
                    });
                    Manager.SendRequest(PartAction, sendDic);
                }
            }
        }

        public void SetFriendToBlackList(Dictionary<string,object> data)
        {
            RequestWithSortType();
        }
        /// <summary>
        /// 检测列表数据有效性
        /// </summary>
        protected virtual void CheckVaild()
        {
        }

        protected  virtual void OnPart(Dictionary<string,object> partInfos) 
        {
            var defDic=partInfos.ParseDefKeyDic();
            if (defDic.ContainsKey(SocialTools.KeyFromAction))
            {
                var formAction = defDic[SocialTools.KeyFromAction].ToString();
                if (formAction.Equals(InitAction))
                {
                    List<object> partData;
                    partInfos.TryGetValueWitheKey(out partData, SocialTools.KeyIds);
                    for (int i = 0, length = partData.Count; i < length; i++)
                    {
                        var itemDic = partData[i] as Dictionary<string, object>;
                        if (itemDic != null)
                        {
                            string id = itemDic[SocialTools.KeyImId].ToString();
                            itemDic[SocialTools.KeyId] = id;
                            CacheDic[id] = itemDic;
                        }
                    }
                    FreshWrapList(CacheDic);
                }
            }
        }
        /// <summary>
        /// 更换排序类型
        /// </summary>
        /// <param name="changeName"></param>
        public void ChangeSortType(string changeName)
        {
           int id=Options.FindIndex(item=>item == changeName);
            if (id>-1)
            {
                SortType = (SocialFriendSortType) id;
            }
        }
        /// <summary>
        /// 点击搜索
        /// </summary>
        public virtual void OnClickSearch()
        {
            RequestWithSortType();
        }
        /// <summary>
        /// 请求
        /// </summary>
        protected virtual void RequestWithSortType(Dictionary<string,object> dataDic=null)
        {
            var sendDic = dataDic == null ? new Dictionary<string, object>() : dataDic;
            sendDic[SocialTools.KeySortType] = (int) SortType;
            SendSocialMessage(InitAction,sendDic);
        }

        public override void Defriend(string id)
        {
            if (TalkCenter.CheckUserInBlackList(id))
            {
                YxMessageBox.Show(SocialTools.KeyNoticeAlreadyExistInBlackList);
                return;
            }
            base.Defriend(id);
        }

        /// <summary>
        /// 亲友圈好友数据排行类型（规则）
        /// </summary>
        public enum  SocialFriendSortType
        {
            OnLine,                         //在线状态
            HotRank,                        //热度排行
            FriendShip                      //亲密度
        }
    }
}
