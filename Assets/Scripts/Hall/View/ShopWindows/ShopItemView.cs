using System.Globalization;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.ShopWindows
{
    /// <summary>
    /// 商店item
    /// </summary>
    public class ShopItemView : YxView
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
        public UISprite CostType;
        /// <summary>
        /// 商品图片
        /// </summary>
        [Tooltip("商品图片")]
        public UITexture GoodsIcon;
        /// <summary>
        /// 商品图片背景 用来让图片按这个比例所缩放
        /// </summary>
        [Tooltip("商品图片背景，用来让图片按这个比例所缩放")]
        public UIWidget GoodsIconWidget;
        [Tooltip("消耗格式")]
        public string CostFormat = "x{0}";

        protected override void OnFreshView()
        {
            var data = Data as ShopModelUnit;
            if (data == null) return;
            if (LabelDesc != null) LabelDesc.Text(data.Description);
            LabelName.Text(data.Name);
            //YxDebug.LogError("type is{0},currency is {1},CurrencyType is{2}", "ShopItemView", null, data.Type, data.Currency, data.CurrencyType);

            if (data.Currency >= 0)
            {
                var currency = data.CurrencyType == "5" ? data.Currency.ToString(CultureInfo.InvariantCulture) : string.Format(CostFormat, data.Currency);
                if (data.CurrencyType == "1" || data.CurrencyType == "coin_a")
                {
                    currency = string.Format(CostFormat, YxUtiles.ReduceNumber((long)data.Currency));
                }
                LabelCost.Text(currency);
            }
            if (CostType != null)
            {
                CostType.spriteName = GetCostTypeName(data.CurrencyType);
                CostType.MakePixelPerfect();
                var offh = (float)40 / CostType.height;
                if (offh < 1)
                {
                    var ts = CostType.transform;
                    ts.localScale = new Vector3(offh, offh, offh);
                }
            }
            var url = data.IconUrl;
            if (string.IsNullOrEmpty(url)) return;
            Facade.Instance<AsyncImage>().GetAsyncImage(url, FreshIcon);
        }

        public static string GetCostTypeName(string type)
        {
            switch (type)
            {
                case "":
                    return "";
                case "1":
                    return "coin_a";
                case "2":
                    return "cash_a";
                case "3":
                    return "coupon_a";
                case "4":
                    return "item2_q";
                case "5":
                    return "rmb_p";
                default:
                    return type;
            }
        }

        private void FreshIcon(Texture2D obj, int hashCode)
        {
            GoodsIcon.mainTexture = obj;
            if (GoodsIconWidget != null)
            {
                GoodsIcon.MakePixelPerfect();
                var w = GoodsIconWidget.width;
                var h = GoodsIconWidget.height;
                if (w > h)
                {
                    GoodsIcon.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnHeight;
                    GoodsIcon.height = h;
                }
                else
                {
                    GoodsIcon.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnWidth;
                    GoodsIcon.width = w;
                }
            }
        }

        public void OnBuyGoods(GameObject obj)
        {
            var data = Data as ShopModelUnit;
            if (data == null) return;
            var clickUrl = data.ClickUrl;

            if (string.IsNullOrEmpty(clickUrl))
            {
                var box = YxMessageBox.Show(null, "GoodsItemMsgBox", "", null, (mesBox, btnName) =>
                {
                    if (btnName == YxMessageBox.BtnLeft)
                    {
                        UserController.Instance.Buy(obj.name);
                    }
                }, true, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
                box.UpdateView(Data);
                return;
            }
            switch (clickUrl)
            {
                case "unknow": //未开放
                    YxMessageBox.Show("非常抱歉，此物品暂时还未开放！！");
                    return;
                case "pay"://支付
                    OnPay(data);
                    return;
                default://网页跳转
                    {
                        _hasFresh = true;
                        var sendMsg = string.Format("{0}?userId={1}&ctoken={2}&AppVer={3}&bundleID={4}&goodsId={5}",
                                                    data.ClickUrl,
                                                    LoginInfo.Instance.user_id,
                                                    LoginInfo.Instance.ctoken,
                                                    Application.version,
                                                    Application.bundleIdentifier,
                                                    data.Id);
                        Application.OpenURL(sendMsg);
                    }
                    return;
            }
        }

        private void OnPay(ShopModelUnit data)
        {
            var payInfo = new PayInfo
            {
                Id = data.Id,
                GoldsName = data.Name,
                CostNum = data.Currency,
                Describe = data.Description
            };
            MainYxView.OpenWindowWithData("PayChangeWindow", payInfo);
        }

        private bool _hasFresh;

        private void OnApplicationFocus()
        {
            if (!_hasFresh) return;
            _hasFresh = false;
            UserController.Instance.SendSimpleUserData();
        }
    }
}
