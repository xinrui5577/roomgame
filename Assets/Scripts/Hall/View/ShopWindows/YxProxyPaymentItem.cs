using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.View.ShopWindows
{
    /// <summary>
    /// 代理充值Item
    /// </summary>
    public class YxProxyPaymentItem :  YxView
    {
        /// <summary>
        /// 代理昵称
        /// </summary>
        [Tooltip("代理昵称")]
        public YxBaseLabelAdapter LabelName;
        /// <summary>
        /// 代理微信号
        /// </summary>
        [Tooltip("代理微信号")]
        public YxBaseLabelAdapter LabelId;

        private YxProxyData _proxyData = new YxProxyData();

        protected override void OnFreshView()
        {
            _proxyData.Parse(Data as Dictionary<string, object>);
            SetWxId(_proxyData.WxId);
            SetName(_proxyData.WxName);
        }

        private void SetWxId(string id)
        {
            if (LabelId == null) { return;}
            LabelId.Text(id);
        }

        private void SetName(string nike)
        {
            if (LabelName == null) { return;}
            LabelName.Text(nike);
        }

        public void OnProxyClick(string windowName = "AgencyDetailWindow")
        {
            var window = YxWindowManager.OpenWindow(windowName);
            window.UpdateView(new Dictionary<string, object>()
                {
                    {"key",_proxyData.WxName }, { "value",_proxyData.WxId}
                });
        }
        public void OnChildProxyClick(string windowName = "AgencyDetailWindow")
        {
            var mainWin = MainYxView as YxWindow;
            if (mainWin == null) return;
            var window = mainWin.CreateChildWindow(windowName);
            window.UpdateView(new Dictionary<string, object>()
                {
                    {"key",_proxyData.WxName }, { "value",_proxyData.WxId}
                });
        }
    }

    public struct YxProxyData
    {
        public string Id;
        public string Type;
        public string WxId;
        public string WxName;
        public void Parse(object obj)
        {
            var dict = obj as Dictionary<string, object>;
            if (dict == null) return;
            DictionaryHelper.Parse(dict, "id", ref Id);
            DictionaryHelper.Parse(dict, "wxid", ref WxId);
            DictionaryHelper.Parse(dict, "wxnick", ref WxName);
        }
    }
}
