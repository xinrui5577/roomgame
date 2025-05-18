using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Gift;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.DataBundles;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.UserInfo
{
    /// <summary>
    /// 亲友圈玩家信息面板
    /// </summary>
    public class SocialGameUserInfoView : BaseSocialView
    {
        [Tooltip("头像")]
        public YxBaseTextureAdapter HeadAdapter;
        [Tooltip("昵称")]
        public YxBaseLabelAdapter NickAdapter;
        [Tooltip("ID")]
        public YxBaseLabelAdapter IdAdapter;
        [Tooltip("金币")]
        public YxBaseLabelAdapter CoinAdapter;
        [Tooltip("元宝")]
        public YxBaseLabelAdapter CashAdapter;
        [Tooltip("礼品点")]
        public YxBaseLabelAdapter GiftPointAdapter;
        [Tooltip("表情窗口")]
        public SocialExpressionWindow ExpressionWindow;
        [Tooltip("商城窗口名称")]
        public string ShopWindowName= "ShopWindow";
        /// <summary>
        /// 请求列表
        /// </summary>
        private List<string> _list=new List<string>();
        #region keys
        /// <summary>
        /// 玩家ID
        /// </summary>
        private const string KeyUserId = "userId";
        /// <summary>
        /// 游戏game key
        /// </summary>
        private const string KeyGameKey = "gameKey";
        /// <summary>
        /// 获取其他人信息接口
        /// </summary>
        private const string KeyGetPartnerInfoAction = "gamePartnerInfo";
        /// <summary>
        /// 元宝数量
        /// </summary>
        private const string KeyCashA = "cash_a";
        #endregion

        public bool IsSelf { private set; get;}
        public bool IsCreateRoom { private set; get; }

        private string _imId;

        protected override void AddListeners()
        {
            base.AddListeners();
            AddEventListener<Dictionary<string, object>>(SocialTools.KeyActionGetUserInfo, OnGetUserImInfo);
            Facade.EventCenter.AddEventListeners<YxESysEventType, Dictionary<string, object>>(YxESysEventType.SysSimpleFresh, obj =>
            {
                GiftPointAdapter.TrySetComponentValue(UserInfoModel.Instance.BackPack.GetItem(SocialTools.KeyGiftPoint));
            });
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
            RemoveEventListener<Dictionary<string, object>>(SocialTools.KeyActionGetUserInfo, OnGetUserImInfo);
            Facade.EventCenter.RemoveEventListener<YxESysEventType, Dictionary<string, object>>(YxESysEventType.SysSimpleFresh, obj =>
            {
                GiftPointAdapter.TrySetComponentValue(UserInfoModel.Instance.BackPack.GetItem(SocialTools.KeyGiftPoint));
            });
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var userInfo = GetData<YxBaseUserInfo>();
            var parm = new Dictionary<string, object>();
            parm[KeyUserId] = userInfo.UserId;
            parm[KeyGameKey] =App.GameKey;
            if (ExpressionWindow)
            {
                ExpressionWindow.AttackIndex = userInfo.Seat;
            }
            IsSelf = UserInfoModel.Instance.UserInfo.UserId.Equals(userInfo.UserId);
            _list.Add(userInfo.UserId);
            if (!IsSelf)
            {
                SendGift();
            }
            Facade.Instance<TwManager>().SendAction(KeyGetPartnerInfoAction, parm, obj =>
            {
                var patchDic = obj as Dictionary<string, object>;
                if (patchDic!=null)
                {
                    if (UserInfoModel.Instance.BackPack.GetKeys().Length == 0)
                    {
                        UserController.Instance.SendSimpleUserData(delegate
                        {
                            InitUi(userInfo, patchDic);
                        },false);
                    }
                    else
                    {
                        InitUi(userInfo, patchDic);
                    }
                }
            }, false, null, false);
        }

        private void InitUi(YxBaseUserInfo userInfo,Dictionary<string,object> patchData)
        {
            NickAdapter.TrySetComponentValue(userInfo.NickM);
            IdAdapter.TrySetComponentValue(userInfo.UserId);
            CoinAdapter.TrySetComponentValue(userInfo.CoinA, "1");
            string casha;
            patchData.TryGetValueWitheKey(out casha, KeyCashA);
            CashAdapter.TrySetComponentValue(casha);
            GiftPointAdapter.TrySetComponentValue(UserInfoModel.Instance.BackPack.GetItem(SocialTools.KeyGiftPoint));
            if (HeadAdapter)
            {
                PortraitDb.SetPortrait(userInfo.AvatarX, HeadAdapter, userInfo.SexI);
            }
        }

        private void GetUserImInfoRequest(string type,string spriteName)
        {
            var sendDic = new Dictionary<string, object>
            {
                {SocialTools.KeyUserIds,_list}
            }.SetDefKeyDic(new Dictionary<string, object>()
            {
                { SocialTools.KeyFromAction,name},{ SocialTools.KeyName,spriteName},{ SocialTools.KeyType,type}
            });
            Manager.SendRequest(SocialTools.KeyActionGetUserInfo, sendDic);
        }


        private void OnGetUserImInfo(Dictionary<string,object> userImInfo)
        {
            var defDic = userImInfo.ParseDefKeyDic();
            string sendType;
            string sendName;
            List<object> list;
            defDic.TryGetValueWitheKey(out sendName,SocialTools.KeyName);
            defDic.TryGetValueWitheKey(out sendType, SocialTools.KeyType);
            userImInfo.TryGetValueWitheKey(out list, SocialTools.KeyIds);
            if (list.Count>0&&!string.IsNullOrEmpty(sendName))
            {
                var itemDic = list[0] as Dictionary<string, object>;
                if (itemDic!=null)
                {
                    itemDic.TryGetValueWitheKey(out _imId, SocialTools.KeyImId);
                    if (!string.IsNullOrEmpty(_imId))
                    {
                        SendGift(sendType, sendName);
                    }
                }
            }
        }

        public void OnClickItem(SocialGameGiftItemView item)
        {
            if (IsSelf)
            {
                YxMessageBox.Show(SocialTools.KeyNoticeCouldNotSendToSelf);
            }
            else
            {
                SendGift(item.SpriteType, item.SpriteName);
            }
        }

        private void SendGift(string spriteType="",string spriteName="")
        {
            if (string.IsNullOrEmpty(_imId))
            {
                GetUserImInfoRequest(spriteType, spriteName);
            }
            else
            {
                if (UserInfoModel.Instance.BackPack.GetItem(SocialTools.KeyGiftPoint)>0|| UserInfoModel.Instance.BackPack.GetItem(spriteType) > 0)
                {
                    var dic = new Dictionary<string, object>();
                    dic[SocialTools.KeyNum] = 1;
                    dic[SocialTools.KeyImId] = _imId;
                    dic[SocialTools.KeyData] = new Dictionary<string, object>()
                    {
                        { spriteType,new Dictionary<string,object>()
                        {
                            { SocialTools.KeyType,spriteType},  { SocialTools.KeyNum,1}
                        } }
                    };
                    dic[SocialTools.KeyFromAction] = SocialTools.KeyGameInGaming;
                    Manager.SendRequest(InitAction, dic.SetDefKeyDic(new Dictionary<string, object>()
                    {
                        { SocialTools.KeyName,spriteName}
                    }));
                }
                else
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
                                    MainYxView.OpenWindowWithData(ShopWindowName, null);
                                    return;
                            }
                        }
                    };
                    YxMessageBox.Show(messageData);
                }
            }
        }
    }
}
