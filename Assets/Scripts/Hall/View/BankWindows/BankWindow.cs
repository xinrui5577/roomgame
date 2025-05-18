using System.Collections;
using System.Globalization;
using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Controller;
using com.yxixia.utile.YxDebug;
using YxFramwork.Common.Model;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.BankWindows
{
    /// <summary>
    /// 银行窗口
    /// </summary>
    public class BankWindow : YxNguiWindow
    {
        [Tooltip("要存入银行的金币UIInput")]
        public UIInput SaveLabel;
        [Tooltip("要取出银行的金币UIInput")]
        public UIInput DrawMoneyLabel;
        [Tooltip("密码UIInput")]
        public UIInput PassWordLabel;
        [Tooltip("旧密码UIInput")]
        public UIInput OldPwdLabel;
        [Tooltip("新密码UIInput")]
        public UIInput NewPwdLabel;
        [Tooltip("重复密码UIInput")]
        public UIInput AgainPwdLabel;
        private bool _isClick;
        [Tooltip("是否需要清除密码value")]
        public bool NeelClearPwd=true;
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
                YxWindowManager.ShowMessageWindow("旧密码不能为空");
                return;
            }
            if (repPwdStr != newPwdStr)
            {
                YxWindowManager.ShowMessageWindow("两次输入不一致");
                return;
            }
            UserController.Instance.ChangeBankPwd(oldPwdStr, newPwdStr);
        }

        public UIInput TheOtherAccount;
        public UIInput GoldLine;
        public UIInput AccountKey;
        [Tooltip("确认赠送ID")]
        public UIInput SureId;
        [Tooltip("是否需要校验发送信息")]
        public bool CheckSendInfo;
        [Tooltip("检测信息格式配合CheckSendInfo使用")]
        public string CheckInfoFormat = "接收人昵称:{0};\n接收人ID:{1};\n赠送数量:{2};";
        /// <summary>
        /// 赠送金币
        /// </summary>
        public void OnGiveMoneyClick()
        {
            var theOtherAccount = TheOtherAccount.value;
            float returnValue;
            float.TryParse(GoldLine.value, out returnValue);
            var realValue = YxUtiles.RecoverShowNumber(returnValue);
            var accountKey = AccountKey.value;

            if (string.IsNullOrEmpty(theOtherAccount))
            {
                YxWindowManager.ShowMessageWindow("账号不能为空");
                return;
            }
            if (realValue < 1)
            {
                YxWindowManager.ShowMessageWindow("输入金额过小，请重新输入!!!");
                return;
            }
            if (string.IsNullOrEmpty(accountKey))
            {
                YxWindowManager.ShowMessageWindow("密码不能为空");
                return;
            }
            if (SureId!=null)
            {
                if (!SureId.value.Equals(theOtherAccount))
                {
                    YxWindowManager.ShowMessageWindow("两次输入ID不一致");
                    return;
                }
            }
            if (CheckSendInfo)
            {
                FriendController.Instance.SendFindUser(theOtherAccount, msg =>
                {
                    var friendInfo = new UserInfo();
                    friendInfo.Parse((IDictionary)msg);
                    YxMessageBoxData messageData=new YxMessageBoxData();
                    messageData.Msg = string.Format(CheckInfoFormat, friendInfo.NickM, friendInfo.UserId, GoldLine.value);
                    YxMessageBox.Show(messageData.Msg,null, (box, btnName) =>
                    {
                        if (btnName.Equals(YxMessageBox.BtnLeft))
                        {
                            SendBankCoin(theOtherAccount, (int)realValue, accountKey);
                        }
                    },false, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
                });
            }
            else
            {
                SendBankCoin(theOtherAccount, (int)realValue, accountKey);
            }

        }

        /// <summary>
        /// 赠送金币
        /// </summary>
        /// <param name="account">赠送帐号</param>
        /// <param name="value">赠送值</param>
        /// <param name="password">密码</param>
        private void SendBankCoin(string account,int value,string password)
        {
            if (_isClick) return;
            _isClick = true;
            StartCoroutine(RetClickState());
            UserController.Instance.PresentBankCoin(account,value, password);
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
            YxDebug.LogError("存钱！！！！", "BankWin");
            var count = SaveLabel.value;
            if (string.IsNullOrEmpty(count))
            {
                YxWindowManager.ShowMessageWindow("请输入正确金额!!!");
                return;
            }
            var realValue = YxUtiles.RecoverShowNumber(double.Parse(count));
            
            if (realValue < 1)
            {
                YxWindowManager.ShowMessageWindow("输入金额不在有效范围内，请重新输入!!!");
                return;
            }
            UserController.Instance.SaveCoin("1", realValue.ToString());
            SaveLabel.value = "";
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
                YxWindowManager.ShowMessageWindow("请输入正确金额!!!");
                return;
            }
            if (string.IsNullOrEmpty(pwd))
            {
                YxWindowManager.ShowMessageWindow("密码不能为空!!!");
                return;
            }
            var realValue = YxUtiles.RecoverShowNumber(double.Parse(count));
            if (realValue < 1)
            {
                YxWindowManager.ShowMessageWindow("输入金额不在有效范围内，请重新输入!!!");
                return;
            }
            YxDebug.Log("Real value is:" + realValue);
            UserController.Instance.SaveCoin("2", realValue.ToString(), pwd);
            DrawMoneyLabel.value = "";
            if (NeelClearPwd)
            {
                PassWordLabel.value = "";
            }
        }

        public void GetAllCoin()
        {
            var allCoin = UserInfoModel.Instance.UserInfo.CoinA;
            SaveLabel.value = YxUtiles.GetShowNumberForm(allCoin,0,"N0").ToString(CultureInfo.InvariantCulture);
        }

        public void GetAllBankCoin()
        {
            var allBcoin = UserInfoModel.Instance.UserInfo.BankCoin;
            DrawMoneyLabel.value =YxUtiles.GetShowNumberForm(allBcoin,0,"N0").ToString(CultureInfo.InvariantCulture);
        }

        public void SetSaveCoin(float process)
        {
            var allCoin = UserInfoModel.Instance.UserInfo.CoinA * process;
            SaveLabel.value = YxUtiles.GetShowNumberForm((long)allCoin,0,"N0").ToString(CultureInfo.InvariantCulture);
        }

        public void SetGetCoin(float process)
        {
            var allBcoin = UserInfoModel.Instance.UserInfo.BankCoin * process;
            DrawMoneyLabel.value = YxUtiles.GetShowNumberForm((long)allBcoin,0,"N0").ToString(CultureInfo.InvariantCulture);
        }
    }
}