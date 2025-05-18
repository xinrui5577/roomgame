using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.View;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.Gift
{
    /// <summary>
    /// 亲友圈赠送礼物
    /// </summary>
    public class SocialSendGiftView :BaseSocialSelectWrapListView
    {
        [Tooltip("礼物数量")]
        public YxBaseLabelAdapter GiftNum;
        [Tooltip("礼物图集")]
        public UIAtlas Atlas;
        /// <summary>
        /// 礼物名称格式化
        /// </summary>
        public string GiftNameFormat = "礼物{0}";

        protected override void AddListeners()
        {
            Facade.EventCenter.AddEventListeners<YxESysEventType,Dictionary<string,object>>(YxESysEventType.SysSimpleFresh, OnUserDataChange);
            base.AddListeners();
        }

        protected override void RemoveListeners()
        {
            Facade.EventCenter.RemoveEventListener<YxESysEventType,Dictionary<string, object>>(YxESysEventType.SysSimpleFresh, OnUserDataChange);
            base.RemoveListeners();
        }

        private void OnUserDataChange(Dictionary<string, object> data)
        {
            GiftNum.TrySetComponentValue(Manager.GiftNum);
        }

        protected override void OnVisible()
        {
            base.OnVisible();
            if (UserInfoModel.Instance.BackPack.GetKeys().Length == 0)
            {
                UserController.Instance.SendSimpleUserData(delegate
                {
                    GiftNum.TrySetComponentValue(Manager.GiftNum);
                }, false);
            }
            else
            {
                GiftNum.TrySetComponentValue(Manager.GiftNum);
            }
            InitList();
        }

        private void InitList()
        {
            PageIds.Clear();
            IdsDataDic.Clear();
            var giftCount = Manager.GiftIds.Count;
            if (Atlas&& giftCount> 0)
            {
                var list = Atlas.spriteList;
                var count = list.Count;
                if (count > 0)
                {
                    for (int i = 0; i < giftCount; i++)
                    {
                        var itemKey = Manager.GiftIds[i];
                        if(Manager.GiftDataDic.ContainsKey(itemKey))
                        {
                            var infoItem = Manager.GiftDataDic[itemKey];
                            PageIds.Add(itemKey);
                            IdsDataDic.Add(itemKey,infoItem);
                        }
                    }
                    FreshWrapList(IdsDataDic);
                }
            }
        }

        public void OnClickSureBtn(string windowName)
        {
            var items=GetSelectItems();
            if (items.Count==0)
            {
                YxMessageBox.Show(SocialTools.KeyNoticeSendGiftEmpty);
                return;
            }
            if (items.Count > Manager.GiftNum)
            {
                ShowGiftNotEnough(windowName);
                return;
            }
            if (Data is Dictionary<string, object>)
            {
                var dic = Data as Dictionary<string, object>;
                var sendCount = 0;
                Dictionary<string,Dictionary<string,object>> selectDic=new Dictionary<string, Dictionary<string, object>>(); 
                foreach (var checkItem in items)
                {
                    if (IdsDataDic.ContainsKey(checkItem))
                    {
                        var infoItem = IdsDataDic[checkItem];
                        if (infoItem!=null)
                        {
                            int itemNum;
                            infoItem.TryGetValueWitheKey(out itemNum, SocialTools.KeyNum);
                            string sendType;
                            infoItem.TryGetValueWitheKey(out sendType, SocialTools.KeyType);
                            if (itemNum>0)
                            {
                                sendCount += itemNum;
                                selectDic.Add(sendType, infoItem);
                            }
                        }
                    }
                }
                if (sendCount == 0)
                {
                    YxMessageBox.Show(SocialTools.KeyNoticeSendGiftEmpty);
                    return;
                }

                if (sendCount > Manager.GiftNum)
                {
                    ShowGiftNotEnough(windowName);
                    return;
                }
                dic[SocialTools.KeyNum] = sendCount;
                dic[SocialTools.KeyData] = selectDic;
                dic[SocialTools.KeyFromAction] = SocialTools.KeyGameInHall;
                Manager.SendRequest(InitAction, dic);
            }
        }

        private void ShowGiftNotEnough(string windowName)
        {
            var messageData = new YxMessageBoxData()
            {
                Msg = SocialTools.KeyNoticeGiftNotEnough,
                BeRelatedCloseBtn = false,
                Listener = delegate (YxMessageBox boxMsg, string btnName)
                {
                    switch (btnName)
                    {
                        case YxMessageBox.BtnMiddle:
                            MainYxView.OpenWindowWithData(windowName, null);
                            return;
                    }
                }
            };
            YxMessageBox.Show(messageData);
        }

        protected override void OnInitDataValid()
        {
            YxMessageBox.Show(SocialTools.KeyNoticeGiftSendSuccess);
            UserController.Instance.SendSimpleUserData(delegate
            {
                GiftNum.TrySetComponentValue(Manager.GiftNum);
            },false);
        }

        /// <summary>
        /// 单独物品数量变化
        /// </summary>
        /// <param name="giftItem"></param>
        /// <param name="add"></param>
        public void OnChangeItemData(SocialGiftItem giftItem,bool add)
        {
            if (giftItem)
            {
               var dic= giftItem.GetData<Dictionary<string, object>>();
                if (dic!=null)
                {
                    int dataNum;
                    dic.TryGetValueWitheKey(out dataNum, SocialTools.KeyNum);
                    dataNum += add ? 1 : -1;
                    if (dataNum<0)
                    {
                        YxMessageBox.Show(SocialTools.KeyNoticeGiftNumLessThanZero);
                        return;
                    }
                    dic[SocialTools.KeyNum] = dataNum;
                    IdsDataDic[giftItem.OnlyId] = dic;
                    giftItem.UpdateView(dic);
                }
            }
        }
    }
}
