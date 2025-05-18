using System.Globalization;
using Assets.Scripts.Common.components;
using Assets.Scripts.Common.Models;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.Utils;
using YxFramwork.Framework;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.ShopWindows
{
    /// <summary>
    /// 确认购买物品弹框
    /// </summary>
    public class YxAffirmGoodsMessageBox : YxMessageBox
    {
        /// <summary>
        /// 物品名
        /// </summary>
        [Tooltip("物品名")]
        public YxBaseLabelAdapter GoodsNameLabel;
        /// <summary>
        /// 物品描述
        /// </summary>
        [Tooltip("物品描述")]
        public YxBaseLabelAdapter GoodsdescLabel;
        /// <summary>
        /// 物品图标
        /// </summary>
        [Tooltip("物品图标")]
        public YxBaseTextureAdapter GoodsIcon;
        /// <summary>
        /// 热销标记
        /// </summary>
        [Tooltip("热销标记")]
        public GameObject HotSign;
        /// <summary>
        /// 当前价格
        /// </summary>
        [Tooltip("当前价格")]
        public YxKeyValueView OldPriceView;
        /// <summary>
        /// 原价
        /// </summary>
        [Tooltip("原价")]
        public YxKeyValueView CurrentPriceView;
        /// <summary>
        /// 消耗
        /// </summary>
        [Tooltip("消耗")]
        public YxKeyValueView CostPriceView;

        protected override void OnAwake()
        {
            base.OnAwake();
            InitStateTotal = 2;
        }

        protected override void OnFreshView()
        {
            var data = GetData<YxGoods>();
            if (data == null) { return;}
            SetName(data.Name);
            SetDescription(data.Description);
            SetIcon(data.IconUrl);
            SetPrice(data);
            SetHotSign(data.IsHot);
        }

        private void SetDescription(string dataDescription)
        {
            if (GoodsdescLabel == null) { return;}
            GoodsdescLabel.Text(dataDescription);
        }

        /// <summary>
        /// 设置名称
        /// </summary>
        /// <param name="dataName"></param>
        private void SetName(string dataName)
        {
            if (GoodsNameLabel == null) return;
            GoodsNameLabel.Text(dataName);
        }

        /// <summary>
        /// 设置图标
        /// </summary>
        /// <param name="dataIconUrl"></param>
        private void SetIcon(string dataIconUrl)
        {
            YxAdapterUtile.SetTexture(GoodsIcon, dataIconUrl);
        }

        /// <summary>
        /// 设置消耗
        /// </summary>
        /// <param name="goods">消耗类型</param>
        private void SetPrice(YxGoods goods)
        {
            var consumeId = goods.ConsumeId;
            var newPrice = goods.GetConsumeNumText();
            var oldPrice = goods.GetOldConsumeNumText();
            SetPrice(CurrentPriceView, consumeId,newPrice);
            SetPrice(CostPriceView, consumeId,newPrice);
            if (goods.OldConsumeNum <= goods.ConsumeNum)
            {
                if (OldPriceView != null)
                {
                    var ts = OldPriceView.transform;
                    var scale = ts.localScale;
                    scale.y = 0;
                    ts.localScale = scale;
                    OldPriceView.Hide();
                }
            } 
            SetPrice(OldPriceView, consumeId,oldPrice);
        }
         
        private void SetPrice(YxView view, string consumeId, string price)
        {
            if (view == null) { return; }
            var data = new YxKeyValueData
            {
                IconUrl = consumeId,
                Value = price
            };
            view.UpdateView(data);
        }

        private void SetHotSign(bool dataIsHot)
        {
            if (HotSign == null) { return; }
            HotSign.SetActive(dataIsHot);
        }

    }
}
