using Assets.Scripts.Common.Models;
using com.yxixia.utile.YxDebug;
using YxFramwork.Common.Adapters;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.ShopWindows
{
    public class YxRechargeWindow : YxPayChangeWindow
    {
        public string GoodsId;
        public YxBaseInputLabelAdapter CountLabel;
        protected override void OnAwake()
        {
            var goods = new YxGoods
            {
                Id = GoodsId
            };
            base.OnAwake();
            UpdateView(goods);
        }

        public void OnChangeCount(YxBaseInputLabelAdapter countLabel)
        {
            var value = countLabel.Value;
            if (string.IsNullOrEmpty(value)) return;
            int count;
            if (!int.TryParse(value, out count))
            {
                return;
            }
            var info = GetData<YxGoods>();
            if (info == null) return;
            info.BuyNum = count;
        }

        private YxPayInfo _curType;
        public void OnChangePayType(YxPayChangeItem item)
        {
            _curType  = item.GetData<YxPayInfo>();
        }

        public void OnRechargeClick()
        {
            if (_curType == null) { return;}
            var goodsInfo = GetData<YxGoods>();
            if (goodsInfo.BuyNum <= 0)
            {
                YxMessageTip.Show("请输入金额", 3);
                return;
            }
            OnPayClick(goodsInfo, _curType);
        }
    }
}
