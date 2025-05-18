using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common.DataBundles;
using YxFramwork.Common.Model;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketFriendDetaillWindow : YxNguiRedPacketWindow
    {
        public UILabel UserId;
        public UILabel NickName;
        public UILabel OtherNiackName;
        public NguiTextureAdapter Head;
        public UIInput GiveMoney;
        public UIInput ChangeOtherNiackName;

        public RedPacketFriendChatLogItem OtherRedPacketFriendChatLogItem;
        public RedPacketFriendChatLogItem SelfRedPacketFriendChatLogItem;
        public UITable RedPacketFriendChatLogGrid;

        public UIScrollView ScrollView;

        public string WindowName;
        public bool NeedSendFriendChatLog;

        private RedPacketFriendData _redPacketFriendData;
        private bool _request;
        private int _curPageNum = 1;
        private int _totalCount;
        private bool _fresh;

        protected override void OnStart()
        {
            base.OnStart();
            if (NeedSendFriendChatLog)
            {
                if (ScrollView != null)
                {
                    ScrollView.onMomentumMove = OnDragFinished;
                }
            }
        }

        private void GetTableList()
        {
            var dic = new Dictionary<string, object>();
            dic["other_id"] = _redPacketFriendData.UserId;
            dic["p"] = _curPageNum++;
            Facade.Instance<TwManager>().SendAction("RedEnvelope.friendChatLog", dic, FreshFriendChatLog, true, null, false);
        }

        protected override void OnShow()
        {
            base.OnShow();
            if (NeedSendFriendChatLog && _redPacketFriendData != null&& !_fresh)
            {
                GetTableList();
            }
            _fresh = true;
        }

        protected override void OnHide()
        {
            base.OnHide();
            Clear();
            _fresh = false;
            _curPageNum = 1;
        }

        private void Clear()
        {
            if (RedPacketFriendChatLogGrid)
            {
                while (RedPacketFriendChatLogGrid.transform.childCount > 0)
                {
                    DestroyImmediate(RedPacketFriendChatLogGrid.transform.GetChild(0).gameObject);
                }
            }
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (ChangeOtherNiackName)
            {

                UIEventListener.Get(ChangeOtherNiackName.gameObject).onSelect = OnSubMit;
            }
            _redPacketFriendData = Data as RedPacketFriendData;
            if (_redPacketFriendData == null) return;
            UserId.TrySetComponentValue(_redPacketFriendData.UserId.ToString());
            NickName.TrySetComponentValue(_redPacketFriendData.NickName);
            if (Head)
            {
                PortraitDb.SetPortrait(_redPacketFriendData.Avatar, Head, 1);
            }
            OtherNiackName.TrySetComponentValue(_redPacketFriendData.OtherNickName);
            if (NeedSendFriendChatLog )
            {
                GetTableList();
            }
        }


        private void FreshFriendChatLog(object obj)
        {
            var info = obj as Dictionary<string, object>;
            if (info == null) return;
            if (info.ContainsKey("totalCount"))
            {
                _totalCount = int.Parse(info["totalCount"].ToString());
            }

            if (info.ContainsKey("data"))
            {
                var datas = info["data"] as List<object>;
                if (datas != null && datas.Count != 0)
                {
                    var userInfo = UserInfoModel.Instance.UserInfo;

                    foreach (var data in datas)
                    {
                        var redPacketFriendChatLog = new RedPacketFriendChatLog(data);

                        RedPacketFriendChatLogItem item = null;
                        if (redPacketFriendChatLog.UserId == int.Parse(userInfo.UserId))
                        {
                            item = YxWindowUtils.CreateItem(SelfRedPacketFriendChatLogItem, RedPacketFriendChatLogGrid.transform);
                        }
                        else
                        {
                            item = YxWindowUtils.CreateItem(OtherRedPacketFriendChatLogItem, RedPacketFriendChatLogGrid.transform);
                        }

                        item.UpdateView(redPacketFriendChatLog);
                    }

                    RedPacketFriendChatLogGrid.repositionNow = true;
                }
            }
        }

        public void OnSendMessage()
        {
            var win = YxWindowManager.OpenWindow(WindowName);
            win.UpdateView(_redPacketFriendData);
        }

        public void OnCreatChildWindow()
        {
            var win = CreateChildWindow(WindowName);
            win.UpdateView(_redPacketFriendData);
        }

        public void OnGiveMoney()
        {
            if (MainYxView != null)
            {
                var dic = new Dictionary<string, object>();
                dic["other_id"] = _redPacketFriendData.UserId;
                if (string.IsNullOrEmpty(GiveMoney.value))
                {
                    YxMessageTip.Show("请设置转账金额");
                    return;
                }
                dic["money"] = YxUtiles.RecoverShowNumber(Double.Parse(GiveMoney.value));
                var win = CreateChildWindow(WindowName);
                win.UpdateView(dic);
            }
        }

        public void OnSubMit(GameObject obj, bool isSelect)
        {
            if (!isSelect)
            {
                var dic = new Dictionary<string, object>();
                if (string.IsNullOrEmpty(ChangeOtherNiackName.value))
                {
                    YxMessageTip.Show("修改的昵称不能为空");
                    return;
                }
                dic["other_id"] = _redPacketFriendData.UserId;
                dic["other_nick_m"] = ChangeOtherNiackName.value;
                Facade.Instance<TwManager>().SendAction("RedEnvelope.updateFriendNick", dic, FreshOtherNickName, true, null, false);

            }
        }

        private void FreshOtherNickName(object obj)
        {
            OtherNiackName.TrySetComponentValue(ChangeOtherNiackName.value);
        }

        public void OnDeleteFriend()
        {
            var dic = new Dictionary<string, object>();
            dic["option"] = 3;
            dic["other_id"] = _redPacketFriendData.UserId;
            Facade.Instance<TwManager>().SendAction("RedEnvelope.opFriend", dic, FreshFriendsView, true, null, false);
        }

        private void FreshFriendsView(object obj)
        {
            Close();
        }
        private void OnDragFinished()
        {
            ScrollView.UpdateScrollbars(true);
            var constraint = ScrollView.panel.CalculateConstrainOffset(ScrollView.bounds.min, ScrollView.bounds.min);
            if (constraint.y <= 1f)
            {
                if (!_request)
                {
                    var currentCount = RedPacketFriendChatLogGrid.transform.childCount;
                    if (_totalCount == currentCount)
                    {
                        return;
                    }
                    GetTableList();
                    _request = true;
                }
            }
        }
    }

    public class RedPacketFriendChatLog
    {
        public string Nickm;
        public string Avatarx;
        public int CoinNum;
        public int UserId;
        public string CreateDt;

        public RedPacketFriendChatLog(object data)
        {
            var dic = data as Dictionary<string, object>;
            if (dic == null) return;
            if (dic.ContainsKey("nick_m"))
            {
                Nickm = dic["nick_m"].ToString();
            }

            if (dic.ContainsKey("avatar_x"))
            {
                Avatarx = dic["avatar_x"].ToString();
            }

            if (dic.ContainsKey("create_dt"))
            {
                CreateDt = dic["create_dt"].ToString();
            }

            if (dic.ContainsKey("coin_num_a"))
            {
                CoinNum = int.Parse(dic["coin_num_a"].ToString());
            }

            if (dic.ContainsKey("user_id"))
            {
                UserId = int.Parse(dic["user_id"].ToString()); 
            }
        }
    }
}
