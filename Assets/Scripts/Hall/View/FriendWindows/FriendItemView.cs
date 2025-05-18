using YxFramwork.Common.Adapters;
using YxFramwork.Common.DataBundles;
using YxFramwork.Common.Model;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.FriendWindows
{
    public class FriendItemView : YxView
    {
        public YxBaseTextureAdapter Protrail;
        public UILabel NickLabel;
        public UILabel StatusLabel;
        public UILabel UserIdLabel;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var info = (UserInfo)Data;
            if (info == null) return;
            if (NickLabel != null) NickLabel.text = info.NickM;
            if (StatusLabel!=null) StatusLabel.text = info.IsOnLine? "在线" : "离线";
            name = info.Id.ToString();
            if (UserIdLabel != null) UserIdLabel.text = string.Format("ID：{0}",info.UserId);
            if (Protrail == null) return;
            var avatar = info.AvatarX;
            int sex;
            int.TryParse(info.SexI.ToString(), out sex);
            PortraitDb.SetPortrait(avatar, Protrail, sex);
        }

        public void OnChatClick(string nickName)
        {

        } 
    }
}
