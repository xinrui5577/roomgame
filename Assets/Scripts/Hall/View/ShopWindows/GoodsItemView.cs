using System;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.ShopWindows
{
    public class GoodsItemView : YxMessageBox
    {
        /// <summary>
        /// 物品名
        /// </summary>
        public UILabel GoodsNameLabel;
        /// <summary>
        /// 物品描述
        /// </summary>
        public UILabel GoodsdescLabel;
        /// <summary>
        /// 物品图标
        /// </summary>
        public UITexture GoodsIcon;

        public PriceData CurCostData;
        public PriceData NewCostData;
        public PriceData CostData; 
        protected override void OnAwake()
        {
            base.OnAwake();
            InitStateTotal = 2;
        }

        protected override void OnFreshView()
        {
            var data = Data as ShopModelUnit;
            if (data == null) return;
            var iconName = ShopItemView.GetCostTypeName(data.CurrencyType);
            GoodsNameLabel.text = data.Name;
            GoodsdescLabel.text = data.Description;
            if (data.SaleDiscount > 0)
            {
                var old = data.Currency / ((float)data.SaleDiscount/100);
                SetCostData(iconName, old.ToString("F2"), CurCostData, 30);
            }
            else if (data.SaleDiscount < 0)
            {
                var old = data.Currency - data.SaleDiscount;
                SetCostData(iconName, old.ToString("F2"), CurCostData, 30);
            }
            else
            {
                CurCostData.Action(false);
            }
            var currency = data.Currency.ToString("F2");
            SetCostData(iconName, currency, NewCostData, 30);
            SetCostData(iconName, currency, CostData, 40);
            var url = data.IconUrl;
            if (!string.IsNullOrEmpty(url)) Facade.Instance<AsyncImage>().GetAsyncImage(url, FreshIcon);
        }

        protected void SetCostData(string type,string cost,PriceData data,int maxHeight)
        {
            data.Action(true);
            data.CostLabel.text = string.Format("x{0}",cost);
            var sp = data.CostTypeSprite;
            sp.spriteName = type;
            sp.MakePixelPerfect();
            var offh = (float)maxHeight / sp.height;
            if (!(offh < 1)) return;
            var ts = sp.transform;
            ts.localScale = new Vector3(offh, offh, offh);
        }

        private void FreshIcon(Texture2D obj)
        {
            GoodsIcon.mainTexture = obj;
        }

    }

    [Serializable]
    public class PriceData
    {
        public UISprite CostTypeSprite;
        public UILabel CostLabel;

        public void Action(bool isAction)
        {
            if (CostTypeSprite != null)  CostTypeSprite.transform.parent.gameObject.SetActive(isAction);
        }
    }
}
