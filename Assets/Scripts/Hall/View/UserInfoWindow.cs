using System.Collections;
using Assets.Scripts.Common;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Windows;
using Assets.Scripts.Common.components;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View
{
    public class UserInfoWindow:YxNguiWindow
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public UILabel UserId;
        /// <summary>
        /// 用户名
        /// </summary>
        public UILabel UserName;
        /// <summary>
        /// 昵称
        /// </summary>
        public UILabel NikeName;
        /// <summary>
        /// 性别
        /// </summary>
        public UILabel Sex;
        /// <summary>
        /// 金币
        /// </summary>
        public UILabel UserCoin;
        [Tooltip("金币adapter")]
        public NguiLabelAdapter UserCoinAdapter;
        /// <summary>
        /// 元宝
        /// </summary>
        public UILabel UserGold;
        /// <summary>
        /// ip
        /// </summary>
        public UILabel ClienIp; 
        /// <summary>
        /// 头像
        /// </summary>
        public UITexture Portrait;
        /// <summary>
        /// 签名
        /// </summary>
        public UILabel Sign;
        /// <summary>
        /// 电话
        /// </summary>
        public UILabel PhoneNumber;
        public string IdForm = "ID:";
        /// <summary>
        /// 推广码按钮
        /// </summary>
        public GameObject SpreadBtn;

        protected override void OnAwake()
        {
            base.OnAwake();
            YxMsgCenterHandler.GetIntance().AddListener(string.Format("{0}_OnChange", UserInfoModel.Instance.GetType().Name), delegate
            {
                var userInfo = UserInfoModel.Instance.UserInfo;
                PortraitRes.SetPortrait(userInfo.AvatarX, Portrait, userInfo.SexI);
            });
        } 

        protected override void OnStart()
        { 
            OnBindDate();
            YxMsgCenterHandler.GetIntance().AddListener(RequestCmd.Sync, UpdateView);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        public void OnChangePwd()
        {
            var userInfo = UserInfoModel.Instance.UserInfo;
            if (string.IsNullOrEmpty(userInfo.LoginM))
            {
                YxMessageBox.Show("您好，您现在的身份为游客，无需设置密码！！！",3);
                return;
            }
            YxWindowManager.OpenWindow("ChangePwdWindow",true);
        }

        /// <summary>
        /// 添加元宝
        /// </summary>
        public void OnAddGold()
        {
            var cfg = App.Config as SysConfig;
            if (cfg == null) return;
            var info = LoginInfo.Instance;
            Application.OpenURL(cfg.GetRecharge(info.user_id, info.token));
        }

        protected override void OnFreshView()
        {
            OnBindDate();
        }

        protected virtual void OnBindDate()
        {
            var userInfo = UserInfoModel.Instance.UserInfo;
            var loginName = userInfo.LoginM;
            if (UserId != null) UserId.text = string.Format("{0}{1}", IdForm,App.UserId);
            if (UserName != null) UserName.text = string.IsNullOrEmpty(loginName) ? "游客" : loginName;
            if (Sex != null)
            {
                switch (userInfo.SexI)
                {
                    case 0:
                        Sex.text = "女";
                        break;
                    case 1:
                        Sex.text = "男";
                        break;
                    default:
                        Sex.text = "保密";
                        break;
                } 
            }
            YxTools.TrySetComponentValue(UserCoin, userInfo.CoinA.ToString());
            YxTools.TrySetComponentValue(UserGold, userInfo.CashA.ToString());
            YxTools.TrySetComponentValue(UserCoinAdapter, userInfo.CoinA, "1");
            if (ClienIp != null) ClienIp.text = userInfo.ClientIP;
            if (NikeName != null) NikeName.text = userInfo.NickM;
            if (PhoneNumber != null) PhoneNumber.text = userInfo.MobileN;
            if (SpreadBtn != null)
            {
                var needShow = userInfo.Promoter != null && userInfo.Promoter == false;
                SpreadBtn.SetActive(needShow);
            }
            PortraitRes.SetPortrait(userInfo.AvatarX, Portrait, userInfo.SexI);
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        public void OnQuitGame()
        {
            App.QuitGameWithMsgBox();
        }

        /// <summary>
        /// 切换账号
        /// </summary>
        public void OnChangeAccount()
        {
            HallMainController.Instance.ChangeAccount();
            Close();
        }
    }
}
