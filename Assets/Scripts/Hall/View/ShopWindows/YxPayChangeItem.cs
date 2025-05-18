using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.ShopWindows
{
    /// <summary>
    /// 支付item
    /// </summary>
    public class YxPayChangeItem : YxView
    {
        [Tooltip("支付类型预制体")]
        public YxBaseButtonAdapter ButtonItem;

        protected override void OnAwake()
        {
            base.OnAwake();
            CheckIsStart = true;
        }

        protected override void OnFreshView()
        {
            var payInfo = GetData<YxPayInfo>();
            if (payInfo == null) { return;}
            var payType = payInfo.PayType.ToString();
            name = payType;
            ButtonItem.SetSkinName(payType);
            ButtonItem.SetLabel(payInfo.Name);
        }
    }
}
