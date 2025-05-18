using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.Utils;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Common.components
{
    /// <summary>
    /// 手机验证码视图
    /// </summary>
    public class VerifyCodeView : YxView
    {
        /// <summary>
        /// 手机
        /// </summary>
        public YxBaseInputLabelAdapter Phone;
        /// <summary>
        /// 验证码
        /// </summary>
        public YxBaseInputLabelAdapter VerifyCode;
        /// <summary>
        /// 倒计时
        /// </summary>
        public YxBaseLabelAdapter CountDown;
        /// <summary>
        /// 
        /// </summary>
        public bool NeedTocken;
        /// <summary>
        /// 
        /// </summary>
        public bool NeedMessage;
        /// <summary>
        /// 
        /// </summary>
        public int TotalCountDown = 60;

        public string ActionName;

        /// <summary>
        /// 发送按钮
        /// </summary>
        public YxBaseButtonAdapter SendBtnAdapter;

        protected override void OnVisible()
        {
            if (string.IsNullOrEmpty(ActionName))
            {
                ActionName = NeedTocken ? "sendTelephoneVerify" : "index.php/client/User/sendTelVerify";
            }
            CountDown.SetActive(false);
            var key = GetPpKey();
            var flag = true;
            if (Util.HasKey(key))
            {
                var lastTime = Util.GetString(key);
                var date = DateTime.Parse(lastTime);
                var span = date - DateTime.Now;
                if (span.TotalSeconds > 0)
                {
                    Debug.LogError("发送");
                    StartSendGetVerifyCode(date);
                    flag = false;
                }
            }
            if (flag)
            {
                Reset();
            }
        }

        protected override void OnUnVisible()
        {
            Reset();
        }

        private string GetPpKey()
        {
            var key = string.Format("VerifyCode_{0}",name);
            return key;
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        public void OnSendGetVerifyCode()
        {
            var dtime = DateTime.Now.AddSeconds(TotalCountDown);
            StartSendGetVerifyCode(dtime,true);
        }

        private Coroutine _countDownCoroutine;
        /// <summary>
        /// 获取验证码
        /// </summary>
        private void StartSendGetVerifyCode(DateTime targetTime,bool needSend = false)
        {
            if (_countDownCoroutine!=null) { return; }
            if (needSend && !SendVerifyCode()) { return; }
            SendBtnAdapter.IsEnabled = false;
            //开始倒计时
            _countDownCoroutine = StartCoroutine(CountDownEvent(targetTime));
        }


        /// <summary>
        /// 倒计时
        /// </summary>
        /// <param name="targetTime"></param>
        /// <returns></returns>
        private IEnumerator CountDownEvent(DateTime targetTime)
        {
            var wait = new WaitForSeconds(1);
            CountDown.SetActive(true);
            var key = GetPpKey();
            Util.SetString(key, targetTime.ToString("yyyy-MM-dd HH:mm:ss"));
            while (DateTime.Now < targetTime)
            {
                var span = targetTime - DateTime.Now;
                CountDown.Text(((int)span.TotalSeconds).ToString());
                yield return wait;
            } 
            Reset();
            Util.RemoveData(key);
        }


        /// <summary>
        /// 发送验证码
        /// </summary>
        private bool SendVerifyCode()
        {
            var phoneNumber = Phone.Value;
            if (NeedMessage)
            {
                if (string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length != 11)
                {
                    YxMessageBox.Show("请正确输入手机号！");
                    return false;
                }
            }
            if (NeedTocken)
            {
                SendTelVerifyAction(phoneNumber);
            }
            else
            {
                SendTelVerify(phoneNumber);
            }
            return true;
        }

        /// <summary>
        /// 请求手机验证码 无token
        /// </summary>
        public void SendTelVerify(string mobileNumber, TwCallBack onFinished = null)
        {
            var url = App.Config.ServerUrl.CombinePath(ActionName);
            //                , string.Format("&mobile_n={0}", mobileNumber)
            var getParam = new Dictionary<string, object>();
            getParam["mobile_n"] = mobileNumber;
            Facade.Instance<TwManager>().SendMsg(url, getParam, null, onFinished, true, OnError);
        }
        /// <summary>
        /// 请求手机验证码
        /// </summary>
        public void SendTelVerifyAction(string mobileNumber, TwCallBack onFinished = null)
        {
            var parm = new Dictionary<string, object> { { "phone", mobileNumber } };
            Facade.Instance<TwManager>().SendAction(ActionName, parm, onFinished,true,OnError);
        }

        private void OnError(object data)
        {
            var dict = data as Dictionary<string, object>;
            if (dict == null) { return;}
            var msg = "";
            var errCode = 0;
            dict.Parse("errMsg", ref msg);
            dict.Parse("errCode", ref errCode);
            if (!string.IsNullOrEmpty(msg))
            {
                YxMessageBox.Show(msg);
            }
            if (errCode != 1)
            {
                Reset();
            }
        }

        private void Reset()
        {
            CountDown.SetActive(false);
            SendBtnAdapter.IsEnabled = true;
            if (_countDownCoroutine != null)
            {
                StopAllCoroutines();
                _countDownCoroutine = null;
            } 
        }
    }
}
