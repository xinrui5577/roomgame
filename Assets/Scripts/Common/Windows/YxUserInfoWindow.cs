using System.Collections.Generic;
using Assets.Scripts.Common.Windows.ChatViews;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Common.Windows
{
    public class YxUserInfoWindow : YxWindow
    {
        /// <summary>
        /// 玩家信息
        /// </summary>
        public YxBasePlayer PlayerInfo;
        /// <summary>
        /// 头像表情
        /// </summary>
        public ExpressionWindow ExpressionWin;
        /// <summary>
        /// 推广码按钮
        /// </summary>
        public GameObject SpreadBtn;
        /// <summary>
        /// 修改密码按钮
        /// </summary>
        public GameObject ChangePwdBtn;

        protected override void OnAwake()
        {
            base.OnAwake();
            CheckIsStart = true;
            AddListeners(UserInfoModel.Instance.GetType().Name + "_OnChange", UpdateView);
        }

        protected override void OnFreshView()
        {
            var userInfo = GetData<YxBaseUserInfo>();
            if (userInfo == null) { return; }
            FreshUserInfoView(userInfo);
            FreshExpressionView(userInfo.Seat);
            if (SpreadBtn != null)
            {
                var needShow = userInfo.SpreadCode;
                SpreadBtn.SetActive(needShow == 0);
            }
            if (ChangePwdBtn != null)
            {
                ChangePwdBtn.SetActive(!string.IsNullOrEmpty(userInfo.LoginName));
            }
        }

        protected void FreshUserInfoView(YxBaseUserInfo userInfo)
        {
            PlayerInfo.Info = userInfo;
            var gdata = App.GameData;
            if (gdata == null || userInfo.Seat == gdata.SelfSeat) { return;}
            var parm = new Dictionary<string, object>();
            parm["userId"] = userInfo.UserId;
            parm["gameKey"] = App.GameKey;
            Facade.Instance<TwManager>().SendAction("gamePartnerInfo", parm, obj =>
            {
                if (PlayerInfo == null) { return; }
                var dict = obj as Dictionary<string, object>;
                if (dict == null) { return;}
                userInfo = GetData<YxBaseUserInfo>();
                if (userInfo == null) { return; }
                userInfo.Parse(dict);
                PlayerInfo.Info = userInfo;
            },false,null,false);
        }

        protected virtual void FreshExpressionView(int serverSeat)
        {
            if (ExpressionWin != null)
            {
                if (App.GameData.SelfSeat == serverSeat)
                {
                    ExpressionWin.gameObject.SetActive(false);
                    ExpressionWin.transform.localScale = Vector3.zero;
                }
                else
                {
                    ExpressionWin.transform.localScale = Vector3.one;
                    ExpressionWin.gameObject.SetActive(true);
                    ExpressionWin.AttackIndex = serverSeat;
                }

            }
        }

        public override YxEUIType UIType
        {
            get { return YxEUIType.Default; }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        public void OnChangePwdClick()
        {
            var userInfo = GetData<YxBaseUserInfo>();
            if (userInfo == null) { return; }
            if (string.IsNullOrEmpty(userInfo.LoginName))
            {
                YxMessageBox.Show("您好，您现在的身份为游客，无需设置密码！！！", 3);
                return;
            }
            YxWindowManager.OpenWindow("ChangePwdWindow");
        }

        /// <summary>
        /// 添加元宝
        /// </summary>
        public void OnAddGoldClick()
        {
            var cfg = App.Config as SysConfig;
            if (cfg == null) return;
            var info = LoginInfo.Instance;
            Application.OpenURL(cfg.GetRecharge(info.user_id, info.token));
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
