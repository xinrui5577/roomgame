using System.Collections;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using YxFramwork.View;
using com.yxixia.utile.YxDebug;
using YxFramwork.Tool;

namespace Assets.Scripts.Hall.View.BankWindows
{
    /// <summary>
    /// 银行窗口
    /// </summary>
    public class BankWindow : YxNguiWindow
    { 
        [Tooltip("银行金币")]
        public UILabel BankCoin;
        [Tooltip("银行金币Adapter")]
        public NguiLabelAdapter BankCoinAdapter;
        [Tooltip("用户金币")]
        public UILabel UserCoin;
        [Tooltip("用户金币Adapter")]
        public NguiLabelAdapter UserCoinAdapter;
        [Tooltip("要存入银行的金币UIInput")]
        public UIInput SaveLabel;
        [Tooltip("要去除银行的金币UIInput")]
        public UIInput DrawMoneyLabel;
        [Tooltip("密码UIInput")]
        public UIInput PassWordLabel;
        [Tooltip("旧密码UIInput")]
        public UIInput OldPwdLabel;
        [Tooltip("新密码UIInput")]
        public UIInput NewPwdLabel;
        [Tooltip("重复密码UIInput")]
        public UIInput AgainPwdLabel;
        [Tooltip("单位：10的unit次幂")]
        public int CoinUnit = 4;

        
        private bool _isClick;

        protected void Start()
        {
            YxMsgCenterHandler.GetIntance().AddListener(RequestCmd.Sync, OnUpCoin);
            OnUpCoin();
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        public void OnChangePwdClick()
        {
            var oldPwdStr = OldPwdLabel.value;
            var repPwdStr = AgainPwdLabel.value;
            var newPwdStr = NewPwdLabel.value;

            if (string.IsNullOrEmpty(oldPwdStr))
            {
                YxMessageBox.Show("旧密码不能为空");
                return;
            }

            if (repPwdStr != newPwdStr)
            {
                YxMessageBox.Show("两次输入不一致");
                return;
            } 
            UserController.Instance.ChangeBankPwd(oldPwdStr, newPwdStr);
        }
        public UIInput TheOtherAccount;
        public UIInput GoldLine;
        public UIInput AccountKey;
        /// <summary>
        /// 赠送金币
        /// </summary>
        public void OnGiveMoneyClick()
        {
            var theOtherAccount = TheOtherAccount.value;
            float returnValue;
            float.TryParse(GoldLine.value, out returnValue);
            long realValue = YxUtiles.RecoverShowNumber(returnValue);
            var accountKey = AccountKey.value;

            if (string.IsNullOrEmpty(theOtherAccount))
            {
                YxMessageBox.Show("账号不能为空");
                return;
            }
            if (realValue < 1)
            {
                YxMessageBox.Show("输入金额过小，请重新输入!!!");
                return;
            }
            if (string.IsNullOrEmpty(accountKey))
            {
                YxMessageBox.Show("密码不能为空");
                return;
            }

            if (_isClick) return;
            _isClick = true;
            StartCoroutine(RetClickState());
            UserController.Instance.PresentBankCoin(theOtherAccount, (int)realValue, accountKey, OnUpCoin,CoinUnit);
        }

        private IEnumerator RetClickState()
        {
            yield return new WaitForSeconds(2);
            _isClick = false;
        }

        /// <summary>
        /// 保存金币
        /// </summary>
        public void OnSaveMoneyClick()
        { 
            YxDebug.LogError("存钱！！！！","BankWin");
            var count = SaveLabel.value;
            if (string.IsNullOrEmpty(count))
            { 
                YxMessageBox.Show("请输入正确金额!!!");
                return;
            }
            long realValue = YxUtiles.RecoverShowNumber(double.Parse(count));
            if (realValue < 1)
            {
                YxMessageBox.Show("输入金额过小，请重新输入!!!");
                return;
            }
            UserController.Instance.SaveCoin("1", realValue.ToString(), null, OnUpCoin, CoinUnit);
        }

        /// <summary>
        /// 取钱
        /// </summary>
        public void OnGetMoneyClick()
        {
            var count = DrawMoneyLabel.value;
            var pwd = PassWordLabel.value;
            if (string.IsNullOrEmpty(count))
            {
                YxMessageBox.Show("请输入正确金额!!!");
                return;
            }
            if (string.IsNullOrEmpty(pwd))
            {
                YxMessageBox.Show("密码不能为空!!!");
                return;
            }
            long realValue = YxUtiles.RecoverShowNumber(double.Parse(count));
            if (realValue < 1)
            {
                YxMessageBox.Show("输入金额过小，请重新输入!!!");
                return;
            }        
            YxDebug.Log("Real value is:"+ realValue);
            UserController.Instance.SaveCoin("2", realValue.ToString(), pwd, OnUpCoin,CoinUnit);
        }

        protected void OnBindDate()
        { 
            OnUpCoin();
        }

        protected void OnUpCoin(object msg=null)
        {
            var userinfo = UserInfoModel.Instance.UserInfo;
            YxTools.TrySetComponentValue(BankCoin, userinfo.BankCoin.ToString());
            YxTools.TrySetComponentValue(UserCoin, userinfo.CoinA.ToString());
            YxTools.TrySetComponentValue(BankCoinAdapter, userinfo.BankCoin, "1");
            YxTools.TrySetComponentValue(UserCoinAdapter, userinfo.CoinA, "1");
        }


        public virtual  void OnFreshCoin()
        {
            OnUpCoin();
        }
    } 
}
