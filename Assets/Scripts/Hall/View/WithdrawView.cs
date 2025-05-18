using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View
{
    public class WithdrawView : YxView
    {
        [Tooltip("绑定信息请求")]
        public string BindInfoAction = "depositOneBandInfo";
        [Tooltip("绑定请求")]
        public string BindAction = "depositBand";
        [Tooltip("绑定类型")]
        public string BindType;
        [Tooltip("当前金额")]
        public UILabel CurrentCoin;
        [Tooltip("兑换提示")]
        public UILabel WithdrawTip;
        [Tooltip("兑换数量")]
        public UIInput ExchangeInput;
        [Tooltip("兑换的滑动条")]
        public UISlider ExchangeSlider;
        [Tooltip("收款账号")]
        public UILabel AccountLabel;
        [Tooltip("绑定按钮")]
        public GameObject BindBtn;
        [Tooltip("参数Key")]
        public List<string> ParamKeys = new List<string>();
        [Tooltip("参数Value")]
        public List<UILabel> ParamValues = new List<UILabel>();
        private long _bankCoin;

        private long _depositCoin;

        protected override void OnEnable()
        {
            base.OnEnable();
            var dic = new Dictionary<string, object>();
            dic["type"] = BindType;
            Facade.Instance<TwManager>().SendAction(BindInfoAction, dic, UpdateView);
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var depositData = Data as Dictionary<string, object>;
            if (depositData == null) return;
            _bankCoin = long.Parse(depositData["bankcoin"].ToString());
            CurrentCoin.TrySetComponentValue(YxUtiles.GetShowNumberForm(_bankCoin, 0, "N0"));
            WithdrawTip.TrySetComponentValue(depositData["noticeAbout"].ToString());
            var couldChangeBind = bool.Parse(depositData["couldChangeBind"].ToString());
            if (depositData["depositAccount"] != null)
            {
                AccountLabel.TrySetComponentValue(depositData["depositAccount"].ToString());
                BindBtn.SetActive(couldChangeBind);
            }
            else
            {
                BindBtn.SetActive(true);
            }
        }


        public void CurrentExchange()
        {
            var coin = Math.Floor(ExchangeSlider.value * _bankCoin);
            _depositCoin = long.Parse(coin.ToString());
            ExchangeInput.value = YxUtiles.GetShowNumberForm(_depositCoin, 0, "N0"); ;
        }

        public void OnClearExchange()
        {
            ExchangeInput.value = "输入兑换金额";
            ExchangeSlider.value = 0;
        }

        public void OnMaxValue()
        {
            ExchangeSlider.value = 1;
        }

        public void OnSureBtn()
        {
            var dic = GetParams();
            if (!dic.Count.Equals(ParamKeys.Count))
            {
                YxMessageBox.Show("绑定信息不完整，请完善");
                return;
            }
            Facade.Instance<TwManager>().SendAction(BindAction, dic, OnSuccessBind);

        }
        private Dictionary<string, object> GetParams()
        {
            var dic = new Dictionary<string, object>();
            var count = ParamKeys.Count;
            if (count == ParamValues.Count)
            {
                for (var i = 0; i < count; i++)
                {
                    var key = ParamKeys[i];
                    var value = ParamValues[i].text;
                    if (key.Equals("depositValue"))
                    {
                        value = YxUtiles.RecoverShowNumber(double.Parse(value)).ToString();
                    }
                    if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                    {
                        continue;
                    }
                    dic.Add(key, value);
                }
            }
            else
            {
                YxDebug.LogError("ParamKeys count is not equal with ");
            }
            return dic;
        }

        private void OnSuccessBind(object obj)
        {
            var data = obj as Dictionary<string, object>;
            if (data != null)
            {
                CallBack(data);
            }
        }
    }
}
