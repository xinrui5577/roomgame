using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Common.Views
{
    /// <summary>
    /// 二维码试图
    /// </summary>
    public class YxQrCodeView : YxView
    {
        /// <summary>
        /// 二维码显示图片
        /// </summary>
        public YxBaseTextureAdapter QrCodeTexture;
        /// <summary>
        /// 
        /// </summary>
        public string ActionName = "getShareQrcode";
        public YxEQrCodeType ActionParmaType = YxEQrCodeType.Share;
        /// <summary>
        /// 扩展用
        /// </summary>
        public string QrCodeExType;
        protected override void OnAwake()
        {
            base.OnAwake();
            InitStateTotal = 2;
            CheckIsStart = true;
            var parma = new Dictionary<string, object> { { "type", ActionParmaType == YxEQrCodeType.Custom? QrCodeExType : ActionParmaType.ToString()} };
            CurTwManager.SendAction(ActionName, parma, UpdateView);
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var dict = GetData<Dictionary<string, object>>();
            if (dict == null)
            {
                QrCodeTexture.SetTexture(null);
                return;
            }
            var url = "";
            dict.Parse("qrcodeUrl", ref url);
            AsyncImage.Instance.GetAsyncImage(url, (texture, code) =>
            {
                if (!code.Equals(url.GetHashCode())) { return; }
                QrCodeTexture.SetTexture(texture);
            });
        }

        public void OnOqCopy()
        {
            var dict = GetData<Dictionary<string, object>>();
            if (dict == null || !dict.ContainsKey("qrcodeStr")) return;
            var qrcodeStr = dict["qrcodeStr"].ToString();
            if (string.IsNullOrEmpty(qrcodeStr)) { return;}
            Facade.Instance<YxGameTools>().PasteBoard = qrcodeStr;
            YxMessageTip.Show("复制成功！",3);
        }
    }

    public enum YxEQrCodeType
    {
        /// <summary>
        /// 分享
        /// </summary>
        Share,
        Custom
    }
}
