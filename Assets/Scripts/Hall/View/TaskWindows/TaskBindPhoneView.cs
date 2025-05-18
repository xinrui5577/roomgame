using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.TaskWindows
{
    public class TaskBindPhoneView : TaskBasseView
    {
        /// <summary>
        /// 手机号输入
        /// </summary>
        public UIInput MobileNumInput;
        /// <summary>
        /// 手机号显示
        /// </summary>
        public UILabel MobileNumLabel;
        /// <summary>
        /// 绑定验证码输入
        /// </summary>
        public UIInput BindVerificationInput;
        /// <summary>
        /// 解绑验证码输入
        /// </summary>
        public UIInput UnBindVerificationInput;
        /// <summary>
        /// 验证周期（s）
        /// </summary>
        public int VerificationCyc = 60;
        /// <summary>
        /// 验证码倒计时格式
        /// </summary>
        public string VerifTimeFormat = "({0})";
        /// <summary>
        /// 必须绑定
        /// </summary>
        public bool MustFillin;

        protected override void OnStart()
        { 
            var mobileNum = UserInfoModel.Instance.UserInfo.PhoneNumber;
            ChangeState(!string.IsNullOrEmpty(mobileNum));
            MobileNumLabel.text = mobileNum;
            MobileNumInput.value = mobileNum;
        }

        private UIButton _veriBtn; 
        private UILabel _verifyLabel; 
        /// <summary>
        /// 发送验证码请求
        /// </summary>
        public void OnSendVerification(UIButton btn,UILabel label)
        {
            if (!btn.isEnabled)
            {
                YxMessageTip.Show("验证码已发送，请稍后再试！");
                return;
            }
            _veriBtn = btn;
            _verifyLabel = label;
            var phone = MobileNumInput.value;
            if (string.IsNullOrEmpty(phone) || phone.Length<11)
            {
                YxMessageBox.Show("请输入手机号码！！！");
                return;
            }
            btn.isEnabled = false;
            var parm = new Dictionary<string, object>();
            parm["phone"] = phone;
            Facade.Instance<TwManager>().SendAction("sendTelephoneVerify", parm, OnVerificationSuccess, true, OnFaile);
            _coroutine = StartCoroutine(VerifFinishCyc(btn, label));
        }

        private Coroutine _coroutine;
        private void OnFaile(object msg)
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            if(_veriBtn!=null) _veriBtn.isEnabled = true;
            if (_verifyLabel != null) _verifyLabel.text = "";
            if (!(msg is IDictionary<string, object>)) return;
            var dict = (IDictionary<string, object>) msg;
            var emsgobj = dict["errorMessages"];
            if (!(emsgobj is Dictionary<string, object>)) return;
            var emsDict = emsgobj as Dictionary<string, object>;
            if (!emsDict.ContainsKey("sendTelephoneVerify")) return;
            var actionMsgObj = emsDict["sendTelephoneVerify"];
            var emsg = actionMsgObj == null ? "" : actionMsgObj.ToString();
            YxMessageBox.Show(emsg);
        }

        private IEnumerator VerifFinishCyc(UIButtonColor btn, UILabel label)
        {
            btn.isEnabled = false;
            if (label != null)
            {
                label.gameObject.SetActive(true);
                var waitOnce = new WaitForSeconds(1);
                var total = VerificationCyc;
                while (total>=0)
                {
                    label.text = string.Format(VerifTimeFormat, total--);
                    yield return waitOnce;
                }
                label.gameObject.SetActive(false);
            }
            else
            {
                yield return new WaitForSeconds(VerificationCyc);
            }
            btn.isEnabled = true; 
        }

        /// <summary>
        /// 验证码请求成功
        /// </summary>
        /// <param name="msg"></param>
        private void OnVerificationSuccess(object msg)
        {
            ShowInfos(msg,"验证码已发送，请查看您的手机",3);
        }

        /// <summary>
        /// 发送绑定手机
        /// </summary>
        public void OnSendBindPhone()
        {
            var mobile = MobileNumInput.value;
            if (string.IsNullOrEmpty(mobile))
            {
                YxMessageBox.Show("请输入手机号码！！！");
                return;
            }
            var verification = BindVerificationInput.value;
            if (string.IsNullOrEmpty(verification))
            {
                YxMessageBox.Show("请输入验证码！！！");
                return;
            }
            var pram = new Dictionary<string, object>();
            pram["phone"] = mobile;
            pram["verify"] = verification;
            Facade.Instance<TwManager>().SendAction("getBindPhoneAward", pram, BoundphoneSuccess);
        }

        /// <summary>
        /// 绑定手机成功
        /// </summary>
        /// <param name="msg"></param>
        private void BoundphoneSuccess(object msg)
        {
            BindVerificationInput.value = "";
            if (FinishState!=null) ChangeState(true);
            
            var mobileNum = MobileNumInput.value;
            UserInfoModel.Instance.UserInfo.PhoneNumber = mobileNum;
            MobileNumLabel.text = mobileNum;
            UserInfoModel.Instance.Save();
            var pram = (IDictionary)msg;
            if (pram.Contains("coin"))
            {
                var coin = int.Parse(pram["coin"].ToString());
                if (coin > 0)
                {
                    UserInfoModel.Instance.UserInfo.CoinA += coin;
                    ShowInfos(msg, string.Format("恭喜您，首次绑定手机成功！！！\n奖励{0}金币！！", coin));
                }
            }
            else
            {
                ShowInfos(msg, "恭喜您，绑定手机成功！！！");
            }
            if (FinishState==null)Close();
        }

        /// <summary>
        /// 发送解绑手机 todo
        /// </summary>
        public void OnSendUnBindPhone()
        {
            var pram = new Dictionary<string, object>();
            pram["verify"] = UnBindVerificationInput.value;
            Facade.Instance<TwManager>().SendAction("getUnBindPhoneAward", pram, UnBoundphoneSuccess);
        }

        /// <summary>
        /// 完成解绑操作
        /// </summary>
        /// <param name="msg"></param>
        private void UnBoundphoneSuccess(object msg)
        { 
            UnBindVerificationInput.value = "";
            ShowInfos(msg, "该账号已经解除手机绑定！！！");
            UserInfoModel.Instance.UserInfo.PhoneNumber = "";
            MobileNumLabel.text = "";
            ChangeState(false);
        }

        public YxWindow ParentWindow;
        public override void Close()
        {
            if (ParentWindow == null) return;
            if (MustFillin && string.IsNullOrEmpty(UserInfoModel.Instance.UserInfo.PhoneNumber)) return;
            ParentWindow.Close();
        }
    }
}
