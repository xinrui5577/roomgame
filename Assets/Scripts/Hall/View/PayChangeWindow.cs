using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Controller;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View
{
    /// <summary>
    /// 支付界面
    /// </summary>
    public class PayChangeWindow : YxNguiWindow
    {
        [Tooltip("金币")]
        public UILabel GoldNameLabel;
        [Tooltip("元宝")]
        public UILabel CostNumLabel;
        [Tooltip("支付类型")]
        public UIButton PrefabPayItem;
        [Tooltip("grid")]
        public UIGrid PayItemGrid;
        private UIButton _payItem;

        protected override void OnFreshView()
        {
            if (Data == null) return;
            var payInfo = Data as PayInfo;
            if (payInfo == null) return;
            if (GoldNameLabel != null) GoldNameLabel.text = payInfo.GoldsName;
            if (CostNumLabel != null)
            {
                CostNumLabel.text = string.Format("¥ {0}", payInfo.CostNum);
            }
            var parm = new Dictionary<string, object>();
            Facade.Instance<TwManger>().SendAction("payChannelList", parm, UpdateListView);
        }

        private void UpdateListView(object msg)
        {
            var list = msg as List<object>;
            if (list == null) return;
            var parentTs = PayItemGrid.transform;
            var payType = "";
            foreach (var o in list)
            {
                payType = o.ToString();
                _payItem = Instantiate(PrefabPayItem);
                _payItem.gameObject.SetActive(true);
                _payItem.name = payType;
                var overSprite = string.Format("{0}_1", o);
                _payItem.normalSprite = string.Format("{0}_0",o);
                _payItem.hoverSprite = overSprite;
                _payItem.pressedSprite = overSprite; 
                var ts = _payItem.transform;
                ts.parent = parentTs;
                ts.localScale = Vector3.one;
            }
            PayItemGrid.repositionNow = true;
            PayItemGrid.Reposition();
            if (list.Count != 1) return;
            if (Data == null) return;
            var payInfo = Data as PayInfo;
            if (payInfo == null) return;
            OnPayClick(payType);
        }

        public void OnPayClick(string channel)
        {
            if (Data == null) return;
            var payInfo = Data as PayInfo;
            if (payInfo == null) return;
            var userCtr = PayController.Instance;
            var eChannel = (PayPlatForm)Enum.Parse(typeof(PayPlatForm), channel);
            userCtr.GetPayInfo(payInfo.GetCents(), payInfo.Describe, payInfo.Id, eChannel,
                OnPaySuccess,
                OnPayCancel,
                OnPayFaile, 
                urlPayObj =>
                    {
                        _orderid = "";
                        var result = (IDictionary)urlPayObj;
                        if (result.Contains("orderid"))
                        {
                            var orderObj = result["orderid"];
                            if (orderObj != null)
                            {
                                _orderid = orderObj.ToString();
                            }
                        }
                        _hasFresh = true;
                    });
        }

        private string _orderid;
        private bool _hasFresh;
        private void OnApplicationFocus()
        {
            if (!_hasFresh) return;
            _hasFresh = false;
            var payCtrl = PayController.Instance;
            StartCoroutine(payCtrl.UrlPay(_orderid, OnPaySuccess, OnPayFaile, OnPayWaitting));
        }
         
        private void OnPaySuccess(object obj)
        {
            if (_waitBox != null) _waitBox.Close();
            var result = obj as Dictionary<string, object>;
            var info = result != null && result.ContainsKey("info") ? result["info"].ToString() : "支付成功！！！";
            YxMessageBox.Show(info);
            UserController.Instance.SendSimpleUserData();
            Close();
        }

        private void OnPayFaile(object obj)
        {
            if (_waitBox != null) _waitBox.Close();
            var result = obj as Dictionary<string, object>;
            var info = result != null && result.ContainsKey("info") ? result["info"].ToString() : "支付失败！！！" ;
            YxMessageBox.Show(info);
        }

        private void OnPayCancel(object obj)
        {
            if(_waitBox!=null) _waitBox.Close();
             var result = obj as Dictionary<string, object>;
            var info = result != null && result.ContainsKey("info") ? result["info"].ToString() : "支付取消！！！";
            YxMessageBox.Show(info);
        }

        private YxWindow _waitBox;
        private void OnPayWaitting(object obj)
        {
            if (_waitBox != null) _waitBox.Close();
            var result = obj as Dictionary<string, object>;
            var info = result != null && result.ContainsKey("info") ? result["info"].ToString() : "支付中，请稍后！！！";
            var boxData = new YxMessageBoxData
                {
                    Msg = info,
                    DelayedShowBtn = 3
                };
            _waitBox = YxMessageBox.Show(boxData);
        }
    }

    public class PayInfo
    {
        public string Id;
        public string GoldsName;
        public float CostNum;
        public string Describe;
        public int GetCents()
        {
            return (int)(CostNum * 100);
        }
    }
}
