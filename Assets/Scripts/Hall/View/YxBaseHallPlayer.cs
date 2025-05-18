using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.Model;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View
{
    /// <summary>
    /// 大厅玩家信息
    /// </summary>
    public class YxBaseHallPlayer : YxBasePlayer
    {
        /// <summary>
        /// 
        /// </summary>
        public string RoomCardId = "item2_q";
        /// <summary>
        /// 房卡
        /// </summary>
        [Tooltip("房卡 label")]
        public YxBaseLabelAdapter RoomCardLabel;
        [Tooltip("电话")]
        public YxBaseLabelAdapter PhoneNumberLabel;

        protected override void OnAwake()
        {
            base.OnAwake();
            CheckIsStart = true;
            AddListeners(UserInfoModel.Instance.GetType().Name + "_OnChange", UpdateView); 
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnFreshView()
        {
            Data = UserInfoModel.Instance.UserInfo;
            base.OnFreshView();
        }

        protected override void FreshUserInfo()
        {
            base.FreshUserInfo();
            SetRoomCard(RoomCard);
            var userInfo = Data as UserInfo;
            if (userInfo == null) return;
            SetPhoneNumber(userInfo.PhoneNumber);
        }

        private void SetPhoneNumber(string phoneNumber)
        {
            if (PhoneNumberLabel == null) return;
            PhoneNumberLabel.Text(phoneNumber);
        }

        public int RoomCard
        {
            get
            {
                var bp = UserInfoModel.Instance.BackPack;
                return bp.GetItem(RoomCardId);
            } 
            set
            {
                var bp = UserInfoModel.Instance.BackPack;
                bp.FreshItem(RoomCardId,value);
            }
        }

        /// <summary>
        /// 设置房卡
        /// </summary>
        /// <param name="rcard"></param>
        protected virtual void SetRoomCard(int rcard)
        {
            if (RoomCardLabel == null) return;
            if (rcard < 0)
            {
                rcard = 0;
            }
            RoomCardLabel.Text(rcard);
        }
    }
}
