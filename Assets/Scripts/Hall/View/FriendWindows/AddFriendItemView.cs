using Assets.Scripts.Common.components;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Framework;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.FriendWindows
{
    public class AddFriendItemView : YxView
    {
        public UITexture Protrail;
        public UILabel NickLabel;
        public UILabel StatusLabel;
        public UILabel UserIdLabel;
        public UIButton AddBtn;
        protected override void OnFreshView()
        {
            base.OnFreshView();
            var info = (UserInfo)Data;
            if (info == null) return;
            if (NickLabel != null) NickLabel.text = info.NickM;
            if (StatusLabel != null) StatusLabel.text = info.IsOnLine ? "在线" : "离线";
            name = info.UserId.ToString();
            if (UserIdLabel != null) UserIdLabel.text = string.Format("ID：{0}", info.UserId);
            if (Protrail == null) return;
            var avatar = info.AvatarX;
            int sex;
            int.TryParse(info.SexI.ToString(), out sex);
            PortraitRes.SetPortrait(avatar, Protrail, sex);
            if (App.UserId != info.UserId && info.FriendMsgStatusI != 2)
            {
                AddBtn.gameObject.SetActive(true);
                AddBtn.isEnabled = info.FriendMsgStatusI != 1;
            }
            else
            {
                AddBtn.gameObject.SetActive(false);
            }
            
        }

        public void OnAddFriend(AddFriendItemView item)
        { 
            FriendController.Instance.SendAddFriend(item.name, msg => AddBtn.isEnabled = false);
        }
    }
}
