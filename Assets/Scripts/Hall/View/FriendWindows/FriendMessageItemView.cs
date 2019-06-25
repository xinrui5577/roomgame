using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.FriendWindows
{
    public class FriendMessageItemView : YxView
    {
        public UILabel MessageLabel;
        public GameObject Btns;
        private FriendMessageView _parentView;
        
        protected override void OnFreshView()
        {
            base.OnFreshView();
            var info = (UserInfo)Data;
            if (info == null) return;
            name = info.Id.ToString();
            string msg;
            switch (info.FriendMsgStatusI)
            {
                case 1:
                    msg = string.Format("玩家【{0}】请求成为您的好友", info.NickM);
                    Btns.transform.localScale = Vector3.one;
                    Btns.SetActive(true);
                    break;
                case 2:
                    msg = string.Format("玩家【{0}】同意了您的好友申请", info.NickM);
                    Btns.transform.localScale = Vector3.zero;
                    Btns.SetActive(false);
                    break;
                case 3:
                    msg = string.Format("玩家【{0}】拒绝了您的好友申请", info.NickM);
                    Btns.transform.localScale = Vector3.zero;
                    Btns.SetActive(false);
                    break;
                default:
                    if (MessageLabel != null) MessageLabel.text = "无效的消息";
                    Btns.transform.localScale = Vector3.zero;
                    Btns.SetActive(false);
                    return;
            }
            if (MessageLabel != null) MessageLabel.text = msg;
        }

        public void SetParentView(FriendMessageView pview)
        {
            _parentView = pview;
        }

        /// <summary>
        /// 同意
        /// </summary>
        public void OnAgreeClick()
        {
            var info = (UserInfo)Data;
            if (info == null) return;
            FriendController.Instance.SendApplyUser(info.Id, "2", msg =>
                { 
                    if (_parentView != null)_parentView.DeleteItem(this);
                });
        }

        /// <summary>
        /// 拒绝
        /// </summary>
        public void OnRefuseClick()
        {
            var info = (UserInfo)Data;
            if (info == null) return;
            FriendController.Instance.SendApplyUser(info.Id, "3", msg =>
                {
                    if (_parentView != null) _parentView.DeleteItem(this);
                });
        }
    }
}
