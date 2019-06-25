using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.View
{
    /// <summary>
    /// 销售
    /// </summary>
    public class SalesinfoWindow : YxNguiWindow
    {
        public UILabel SaleInfoLabel;
        public char InfosBreakSign = ';';

        protected override void OnAwake()
        {
            Facade.Instance<TwManger>().SendAction("saleInfo",new Dictionary<string,object>(),UpdateView);
        }

        protected override void OnFreshView()
        {
            if (Data == null) return;
            var dict = Data as Dictionary<string, object>;
            if (dict == null) return;
            if (!dict.ContainsKey("saleInfo")) return;
            var saleInfo = dict["saleInfo"];
            if (saleInfo == null) return;
            var sale = saleInfo.ToString();
            var infos = sale.Split(InfosBreakSign);
            var len = infos.Length;
            var content = "";
            var newline = "";
            for (var i=0; i < len; i++)
            {
                var info = infos[i];
                content = string.Format("{0}{1}{2}", content, newline, info.Trim());
                newline = "\n";
            }
            SaleInfoLabel.text = content;
            SaleInfoLabel.gameObject.SetActive(false);
            StartCoroutine(UpdateLabel());
        }

        private IEnumerator UpdateLabel()
        {
            yield return null;
            SaleInfoLabel.gameObject.SetActive(true);
        }

// {"saleInfo":{"saleInfo":"\u8d2d\u4e70\u623f\u5361\uff1a 111666888 \uff08\u5fae\u4fe1\uff09; \u6295\u8bc9\u4e3e\u62a5\uff1a111666888 \uff08QQ\uff09"},"success":true


    }
}
