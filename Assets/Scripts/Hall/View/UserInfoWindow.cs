using System.Collections.Generic;
using Assets.Scripts.Common;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.components;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.DataBundles;
using YxFramwork.Common.Model;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
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
        public NguiTextureAdapter Portrait;
        /// <summary>
        /// 签名
        /// </summary>
        public UILabel Sign;
        /// <summary>
        /// 电话
        /// </summary>
        public UILabel PhoneNumber;
        /// <summary>
        /// 手机绑定按钮
        /// </summary>
        public YxGameObjectSelector BindPhoneSelector;
        public string IdForm = "ID:";
        /// <summary>
        /// 推广码按钮
        /// </summary>
        public GameObject SpreadBtn;
        /// <summary>
        /// 推广码可操作状态
        /// </summary>
        public UIButton SpreadStateBtn;
        /// <summary>
        /// 推广码显示
        /// </summary>
        public UILabel Spread;
        /// <summary>
        /// 代理码描述
        /// </summary>
        public string PromoterMsg = "无代理码";
        /// <summary>
        /// 是否自己 TODO 临时方案
        /// </summary>
        public bool IsSelf = true;
        /// <summary>
        /// 游客状态额外执行事件
        /// </summary>
        public List<EventDelegate> VisitorAction=new List<EventDelegate>();

        protected override void OnAwake()
        {
            base.OnAwake();
            if (!IsSelf) return;
            AddListeners(string.Format("{0}_OnChange", UserInfoModel.Instance.GetType().Name),
                                           delegate
                                               {
                                                   var userInfo = UserInfoModel.Instance.UserInfo;
                PortraitDb.SetPortrait(userInfo.AvatarX, Portrait, userInfo.SexI);
                                                   UpdateView();
                                               });
        }

        protected override void OnStart()
        {
            if (!IsSelf) return;
            OnBindDate(UserInfoModel.Instance.UserInfo);
            AddListeners(RequestCmd.Sync, UpdateView);
            if (Sign!=null) UIEventListener.Get(Sign.gameObject).onSelect = OnChangeSign;  
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        public void OnChangePwd()
        {
            var userInfo = UserInfoModel.Instance.UserInfo;
            if (string.IsNullOrEmpty(userInfo.LoginName))
            {
                YxMessageBox.Show("您好，您现在的身份为游客，无需设置密码！！！",3);
                return;
            }
            YxWindowManager.OpenWindow("ChangePwdWindow");
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
            UserInfo userInfo;
            if (IsSelf)
            {
                userInfo = UserInfoModel.Instance.UserInfo;
            }
            else
            {
                userInfo = Data as UserInfo;
            }
            OnBindDate(userInfo);
        }

        protected virtual void OnBindDate(UserInfo userInfo)
        {
            var loginName = userInfo.LoginName;
            if (UserId != null) UserId.text = string.Format("{0}{1}", IdForm, userInfo.UserId);
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

            if (UserCoin != null) UserCoin.text = YxUtiles.ReduceNumber(userInfo.CoinA);
            if (UserGold!=null) UserGold.text = userInfo.CashA.ToString();
            UserCoinAdapter.TrySetComponentValue( userInfo.CoinA, "1");
            if (ClienIp != null) ClienIp.text = userInfo.ClientIP;
            if (NikeName != null) NikeName.text = userInfo.NickM;
            if (PhoneNumber != null) PhoneNumber.text = userInfo.PhoneNumber;
            if (Sign != null)
            {
                var uiinput = Sign.GetComponent<UIInput>();
                if (uiinput != null)
                {
                    uiinput.value = userInfo.Signature;
                }
                else
                {
                    Sign.text = userInfo.Signature;
                }
            }
            if (SpreadBtn != null)
            {
                var needShow = userInfo.Promoter != null && userInfo.Promoter == false;
                SpreadBtn.SetActive(needShow);
            }
            if (SpreadStateBtn != null)
            {
                if (string.IsNullOrEmpty(loginName))
                {
                    SpreadStateBtn.transform.parent.gameObject.SetActive(false);
                    if (gameObject.activeInHierarchy)
                    {
                        StartCoroutine(VisitorAction.WaitExcuteCalls());
                    }
                }
                Spread.text = userInfo.PromoterId ?? PromoterMsg;
                SpreadStateBtn.gameObject.SetActive(string.IsNullOrEmpty(userInfo.PromoterId));
            }
            if (BindPhoneSelector!=null)
            {
                BindPhoneSelector.Change(string.IsNullOrEmpty(userInfo.PhoneNumber) ? 0 : 1);
            }

            PortraitDb.SetPortrait(userInfo.AvatarX, Portrait, userInfo.SexI);
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
        /// <summary>
        /// 修改个性签名
        /// </summary>
        public void OnChangeSign(GameObject obj, bool isSelect)
        {
            if (!isSelect)
            {
                var content = obj.GetComponent<UIInput>().value;
                if (UserInfoModel.Instance.UserInfo.Signature.Equals(content))
                {
                    return;
                }
                var dic = new Dictionary<string, object>();
                dic["newSignature"] = content;
                Facade.Instance<TwManager>()
                    .SendAction("changeUserOptions", dic, data =>
                    {
                        UserController.Instance.GetUserDate();
                    });
            }
        }
    }
}
