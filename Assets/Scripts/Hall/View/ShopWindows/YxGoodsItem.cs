using System.Collections.Generic;
using Assets.Scripts.Common.Models;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.Model;
using YxFramwork.Common.Utils;
using YxFramwork.Controller;
using YxFramwork.Framework;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.ShopWindows
{
    /// <summary>
    /// 商品item
    /// </summary>
    public class YxGoodsItem : YxView
    { 
        /// <summary>
        /// 商品名称
        /// </summary>
        [Tooltip("商品名称")]
        public YxBaseLabelAdapter LabelName;
        /// <summary>
        /// 花销
        /// </summary>
        [Tooltip("花销")]
        public YxBaseLabelAdapter LabelCost;
        /// <summary>
        /// 描述
        /// </summary>
        [Tooltip("描述")]
        public YxBaseLabelAdapter LabelDesc;
        /// <summary>
        /// 花销类别
        /// </summary>
        [Tooltip("花销类别")]
        public YxBaseSpriteAdapter CostType;
        /// <summary>
        /// 商品图片
        /// </summary>
        [Tooltip("商品图片")]
        public YxBaseTextureAdapter GoodsIcon; 
        /// <summary>
        /// 热销标记
        /// </summary>
        [Tooltip("热销标记")]
        public GameObject HotSign;
        [Tooltip("消耗格式")]
        public string CostFormat = "x{0}";

        private YxGoods _curData;
        protected override void OnFreshView()
        {
            _curData = Parse(Data);
            if (_curData == null) { return;}
            SetGoodsName(_curData.Name);
            SetPrice(_curData.ConsumeId, _curData.GetConsumeNumText());
            SetIcon(_curData.IconUrl);
            SetDescription(_curData.Description);
            SetHotSign(_curData.IsHot);
        }
         
        private void SetGoodsName(string dataName)
        {
            if (LabelName == null) { return;}
            LabelName.Text(dataName);
        }

        private void SetPrice(string dataConsumeId, string dataConsumeNum)
        {
            if (LabelCost != null)
            {
                LabelCost.Text(dataConsumeNum);
            }
            if (CostType != null)
            {
                CostType.SetSpriteName(dataConsumeId);
            }
        }

        private void SetIcon(string dataIconUrl)
        {
            YxAdapterUtile.SetTexture(GoodsIcon, dataIconUrl);
        }
 
        private void SetDescription(string dataDescription)
        {
            if (LabelDesc == null) { return; }
            LabelDesc.Text(dataDescription);
        }

        /// <summary>
        /// 是否为热销产品
        /// </summary>
        /// <param name="dataIsHot"></param>
        private void SetHotSign(bool dataIsHot)
        {
            if (HotSign == null) { return; }
            HotSign.SetActive(dataIsHot);
        }


        public void OnBuyGoods()
        {
            if (_curData == null) return;
            switch (_curData.BuyAction)
            {
                case YxEBuyAction.payment:
                    OnBuyWithPay(_curData);
                    break;
                case YxEBuyAction.trade:
                    OnBuyWithTrade(_curData);
                    break;
                case YxEBuyAction.url:
                    OnBuyWithUrl(_curData);
                    break; 
                case YxEBuyAction.outpay:
                    OnBuyWithUrl(_curData);
                    break; 
                default:
                    YxMessageBox.Show("非常抱歉，此物品暂时还未开放！！");
                    break; 
            }
        }

        /// <summary>
        /// 网页购买
        /// </summary>
        /// <param name="data"></param>
        private void OnBuyWithUrl(YxGoods data)
        {
            _hasFresh = true;
            var sendMsg = string.Format("{0}?userId={1}&ctoken={2}&AppVer={3}&bundleID={4}&goodsId={5}",
                data.PayUrl,
                LoginInfo.Instance.user_id,
                LoginInfo.Instance.ctoken,
                Application.version,
                Application.bundleIdentifier,
                data.Id);
            Application.OpenURL(sendMsg);
        }

        /// <summary>
        /// 兑换
        /// </summary>
        /// <param name="data"></param>
        private void OnBuyWithTrade(YxGoods data)
        {
            var box = YxMessageBox.Show(null, "AffirmGoodsMessageBox", "", null, (mesBox, btnName) =>
            {
                if (btnName == YxMessageBox.BtnLeft)
                {
                    UserController.Instance.BuyGoods(Id, data.Id); 
                }
            }, true, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
            box.UpdateView(_curData);
        }

        private void OnBuyWithPay(YxGoods data)
        {
            MainYxView.OpenWindowWithData("PayChangeWindow", data);
        }
        private bool _hasFresh;

        protected void OnApplicationFocus()
        {
            if (!_hasFresh) return;
            _hasFresh = false;
            UserController.Instance.SendSimpleUserData();
        }

        public virtual YxGoods Parse(object obj)
        {
            var dict = obj as Dictionary<string, object>;
            var data = new YxGoods();
            if (dict != null)
            {
                data.Parse(dict);
                data.Type = Id;
            }
            return data;
        }
    }
}
