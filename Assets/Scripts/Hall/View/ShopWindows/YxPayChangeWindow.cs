using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common.Models;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Controller;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.ShopWindows
{
    public class YxPayChangeWindow : YxWindow
    {
        [Tooltip("商品名称")]
        public YxBaseLabelAdapter GoodsNameLabel;
        [Tooltip("消耗数量")]
        public YxBaseLabelAdapter ConsumeNumLabel;
        [Tooltip("描述")]
        public YxBaseLabelAdapter DescribeLabel;
        [Tooltip("物品的图标")]
        public YxBaseTextureAdapter GoodsIcon;
        public string ConsumeFormat = "¥ {0}";
        [Tooltip("支付类型预制体")]
        public YxPayChangeItem PrefabPayItem;
        [Tooltip("grid预制体")]
        public YxBaseGridAdapter PrefabPayItemGrid;
        [Tooltip("1个的时候需要直接支付")]
        public bool NeedDirectPay = true;
        private YxBaseGridAdapter _payItemGrid;
        protected List<object> PayTypeList;

        protected override void OnAwake()
        {
            InitStateTotal = 3;//打开窗口传入数据：1次， 获取支付类型：2次
            CheckIsStart = true;
            CurTwManager.SendAction("store.paytype", null, FreshPayType);
        }

        private void FreshPayType(object obj)
        {
            var dict = obj as Dictionary<string, object>;
            if (dict != null)
            {
                DictionaryHelper.ParseList(dict, "payType", ref PayTypeList);
                if (PayTypeList != null)
                {
                    FreshView(false);
                    return;
                }
            }
            Close();
        }
         
        protected override void OnFreshView()
        {
            var goods = GetData<YxGoods>();
            if (goods == null) return;
            UpdatePayTypes();
            SetGoodsName(goods.Name);
            SetGoodsIcon(goods.IconUrl);
            SetConsumeNum(goods.GetConsumeNumText());
            SetDescribe(goods.Description);
            
        }

        private void SetGoodsIcon(string goodsIconUrl)
        {
            if (GoodsIcon == null) { return;}
            if (string.IsNullOrEmpty(goodsIconUrl)) { return;}
            Facade.Instance<AsyncImage>().GetAsyncImage(goodsIconUrl, FreshIcon);
        }

        private void FreshIcon(Texture2D obj, int hashCode)
        {
            GoodsIcon.SetTexture(obj);
        }

        private void UpdatePayTypes()
        {
            YxWindowUtils.CreateMonoParent(PrefabPayItemGrid,ref _payItemGrid);
            if (PayTypeList == null)
            {
                Close();
                return;
            }
            var len = PayTypeList.Count;
            
            if (len == 1 && NeedDirectPay)
            {
                var payInfo = new YxPayInfo();
                if (payInfo.Parse(PayTypeList[0]))
                {
                    OnPayClick(GetData<YxGoods>(),payInfo);
                }
            }
            var gridTs = _payItemGrid.transform;
            for (var i = 0; i < len; i++)
            {
                var dict = PayTypeList[i] as Dictionary<string,object>;
                if (dict == null) { continue;}
                var payInfo = new YxPayInfo();
                if (!payInfo.Parse(PayTypeList[i])){continue;}
                var item = YxWindowUtils.CreateItem(PrefabPayItem, gridTs);
                item.UpdateView(payInfo);
            }
            _payItemGrid.Reposition();
        }

        private YxWindow _waitBox;

        public void OnPayItemClick(YxPayChangeItem item)
        {
            var goodsInfo = GetData<YxGoods>();
            var payInfo = item.GetData<YxPayInfo>();
            OnPayClick(goodsInfo, payInfo);
        }

        /// <summary>
        /// 支付状态  0：无   1：等待   2：成功   3：失败    4：取消
        /// </summary>
        private int _payState;
        protected void OnPayClick(YxGoods goodsInfo, YxPayInfo payInfo)
        {
            if (goodsInfo == null) { return; }
            if (payInfo == null) { return; }
            var payCtrl = PayController.Instance;
            _payState = 1;
            payCtrl.GetPayment(payInfo.Id, goodsInfo.BuyNum, goodsInfo.Description, goodsInfo.Id, payInfo.PayType,
                OnPaySuccess,
                OnPayCancel,
                OnPayFaile,
                OnCheckUrlPayStatus);
        }

        private string _orderId = null;
        private void OnApplicationFocus()
        {
            if (string.IsNullOrEmpty(_orderId)) { return;}
            StopAllCoroutines();
            var payCtrl = PayController.Instance;
            StartCoroutine(payCtrl.Checkorder(_orderId, OnPaySuccess, OnPayFaile, OnPayWaitting));
        }

        /// <summary>
        /// 检查url支付状态
        /// </summary>
        /// <param name="obj"></param>
        private void OnCheckUrlPayStatus(object obj)
        {
            var result = obj as Dictionary<string, object>;
            if (result == null) return; 
            if (DictionaryHelper.Parse(result, "order_id", ref _orderId))
            {
                OnApplicationFocus();
            }
        }

        /// <summary>
        /// 支付成功
        /// </summary>
        /// <param name="obj"></param>
        private void OnPaySuccess(object obj)
        {
            if (_payState != 1) { return;}
            _payState = 2;
            if (_waitBox != null) _waitBox.Close();
            var result = obj as Dictionary<string, object>;
            var info = result != null && result.ContainsKey("info") ? result["info"].ToString() : "支付成功！！！";
            _orderId = null;
            UserController.Instance.SendSimpleUserData(); 
            YxMessageBox.Show(info,"", (box,btnName) =>
            {
                Close();
            });
        }

        /// <summary>
        /// 支付失败
        /// </summary>
        /// <param name="obj"></param>
        private void OnPayFaile(object obj)
        {
            if (_payState != 1) { return; }
            _payState = 3;
            if (_waitBox != null) _waitBox.Close();
            var result = obj as Dictionary<string, object>;
            var info = result != null && result.ContainsKey("info") ? result["info"].ToString() : "支付失败！！！";
            _orderId = null;
            YxMessageBox.Show(info);
            StopAllCoroutines();
        }

        /// <summary>
        /// 支付取消
        /// </summary>
        /// <param name="obj"></param>
        private void OnPayCancel(object obj)
        {
            if (_payState != 1) { return; }
            _payState = 4;
            if (_waitBox != null) _waitBox.Close();
            var result = obj as Dictionary<string, object>;
            var info = result != null && result.ContainsKey("info") ? result["info"].ToString() : "支付取消！！！";
            _orderId = null;
            YxMessageBox.Show(info);
            StopAllCoroutines();
        }

        private void OnPayWaitting()
        {
            if (_waitBox != null) _waitBox.Close();
            var boxData = new YxMessageBoxData
            {
                Msg = "支付中，请稍后！！！",
                DelayedShowBtn = 3
            };
            _waitBox = YxMessageBox.Show(boxData);
        }

        /// <summary>
        /// 设置描述
        /// </summary>
        /// <param name="describe"></param>
        private void SetDescribe(string describe)
        {
            if (DescribeLabel == null) { return;}
            DescribeLabel.Text(describe);
        }

        /// <summary>
        /// 设置额度
        /// </summary>
        /// <param name="consumeNum"></param>
        private void SetConsumeNum(string consumeNum)
        {
            if (ConsumeNumLabel == null) { return; }
            ConsumeNumLabel.Text(string.Format(ConsumeFormat,consumeNum));
        }

        /// <summary>
        /// 设置名称
        /// </summary>
        /// <param name="goodsName"></param>
        private void SetGoodsName(string goodsName)
        {
            if (GoodsNameLabel == null) { return; }
            GoodsNameLabel.Text(goodsName);
        }

        public override YxEUIType UIType
        {
            get { return YxEUIType.Default; }
        }
    }

    /// <summary>
    /// 支付信息
    /// </summary>
    public class YxPayInfo
    {
        /// <summary>
        /// id
        /// </summary>
        public string Id;
        /// <summary>
        /// 支付类型
        /// </summary>
        public YxEPaymentPlatForm PayType;
        /// <summary>
        /// 支付名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId;

        public bool Parse(object obj)
        {
            var dict = obj as Dictionary<string, object>;
            if (dict == null) return false;
            DictionaryHelper.Parse(dict, "id", ref Id);
            DictionaryHelper.ParseEnum(dict, "payment_class", ref PayType);
            DictionaryHelper.Parse(dict, "name", ref Name);
            DictionaryHelper.Parse(dict, "order_id", ref OrderId);
            return true;
        }
    }
}
