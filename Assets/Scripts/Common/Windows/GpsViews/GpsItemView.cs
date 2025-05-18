using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.DataBundles;
using YxFramwork.Common.Model;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Common.Windows.GpsViews
{
    public class GpsItemView : YxView {
        /// <summary>
        /// 头像
        /// </summary>
        public YxBaseTextureAdapter HeadPortrait;
        /// <summary>
        /// 地址label
        /// </summary>
        public YxBaseLabelAdapter AddressLabel;
        /// <summary>
        /// 昵称label
        /// </summary>
        public YxBaseLabelAdapter NickNameLabel;

        /// <summary>
        /// 地址便宜索引
        /// </summary>
        public int DisOffId = -1;

        private string _curAatar;
        protected override void OnFreshView()
        {
            var userInfo = GetData<YxBaseUserInfo>();
            if (userInfo == null)
            {
                ShowAddres(false);
                SetNoneAvatar();
                SetNickName("");
                return;
            }
          
            SetAvatar(userInfo.AvatarX, userInfo.SexI);
            SetAddres(userInfo.Address);
            SetNickName(userInfo.NickM);
        }

        protected void SetNickName(string nick)
        {
            if (NickNameLabel != null) NickNameLabel.Text(nick);
        }
        protected void SetAddres(string address)
        {
            if (AddressLabel == null) return;

            var gpsMgr = Facade.GetInterimManager<YxGPSManager>();
            if (gpsMgr != null)
            {
                AddressLabel.gameObject.SetActive(!gpsMgr.NeedHideAddress);
            }

            address = string.IsNullOrEmpty(address) ? "没有地址信息" : address;
            AddressLabel.Text(address);
        }

        protected void ShowAddres(bool needShow)
        {
            if (AddressLabel == null) return;
            AddressLabel.gameObject.SetActive(needShow);
        }

        protected void SetAvatar(string userAvatar,int sex)
        {
            if (HeadPortrait == null) return;
            if (userAvatar == _curAatar) return;
            _curAatar = userAvatar;
            PortraitDb.SetPortrait(userAvatar, HeadPortrait, sex);
        }

        protected void SetNoneAvatar()
        {
            _curAatar = null;
            if (HeadPortrait == null) return;
            HeadPortrait.SetTexture(null);
        }
    }
}
