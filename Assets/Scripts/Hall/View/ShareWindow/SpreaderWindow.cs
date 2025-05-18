using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Hall.View.ShareWindow
{
    public class SpreaderWindow : YxNguiWindow
    {
        public UITexture QrCode;
        public UILabel AffiliateId;
        public UILabel UnGetAwardNum;
        public UILabel AfiliateNum;
        public UILabel AffilateShow;
        public GameObject QrCodeBg;
        public UITexture QrCodeBig;
        [Tooltip("推广二维码")]
        public YxBaseTextureAdapter PromotionTex;
        [Tooltip("推广总收益")]
        public YxBaseLabelAdapter AllAffReward;
        [Tooltip("推广未领取收益")]
        public YxBaseLabelAdapter UnEffectAffReward;
        [Tooltip("跳转格式")]
        public string LinkFormat= "{0}userId={1}&ctoken={2}";

        public string AffDetailUrl
        {
            private set; get;
        }

        /// <summary>
        /// Key推广数据
        /// </summary>
        private const string KeyPromotion = "daXiaoAffilateData";
        /// <summary>
        /// Key推广二维码
        /// </summary>
        private const string KeyPromotionQrCode = "eQRCode";
        /// <summary>
        /// Key推广总收益
        /// </summary>
        private const string KeyPromotionAllAffReward = "allAffReward";
        /// <summary>
        /// Key推广未领取收益
        /// </summary>
        private const string KeyPromotionUnEffectAffReward = "unEffectAffReward";
        /// <summary>
        /// Key推广详情地址
        /// </summary>
        private const string KeyPromotionAffDetailUrl = "affDetailUrl";

        protected void Start()
        {
            Facade.Instance<TwManager>().SendAction("affiliateFriend", new Dictionary<string, object>(), (data) =>
                {
                    if (data == null) return;
                    if (!(data is Dictionary<string, object>)) return;
                    var dataInfo = data as Dictionary<string, object>;
                    var qRCode = dataInfo.ContainsKey("eQRCode") ? dataInfo["eQRCode"].ToString() : "";//二维码
                    var affiliateId = dataInfo.ContainsKey("affiliateId") ? dataInfo["affiliateId"].ToString() : "";//推广ID
                    var unAwardNum = dataInfo.ContainsKey("unAwardNum") ? dataInfo["unAwardNum"].ToString() : "";
                    var afiliateNum = dataInfo.ContainsKey("afiliateNum") ? dataInfo["afiliateNum"].ToString() : "";
                    var affilateAward = dataInfo.ContainsKey("affilateAward") ? dataInfo["affilateAward"].ToString() : "";
                    var affilateAwardType = dataInfo.ContainsKey("affilateAwardType") ? dataInfo["affilateAwardType"].ToString() : "";

                    AsyncImage.Instance.GetAsyncImage(qRCode, (texture, s) =>
                        {
                            if (QrCode)
                            {
                                QrCode.mainTexture = texture;
                            }
                            if (QrCodeBig)
                            {
                                QrCodeBig.mainTexture = texture;
                            }
                        });
                    AffiliateId.TrySetComponentValue(affiliateId) ;
                    UnGetAwardNum.TrySetComponentValue(unAwardNum);
                    AfiliateNum.TrySetComponentValue(afiliateNum);
                    AffilateShow.TrySetComponentValue(string.Format("{0}{1}!", affilateAward, affilateAwardType));
                    if (dataInfo.ContainsKey(KeyPromotion))
                    {
                        var dic=new Dictionary<string,object>();
                        dataInfo.TryGetValueWitheKey(out dic,KeyPromotion);
                        GetPromotionData(dic);
                    }
                });
        }

        public void OnGetAward()
        {
            Facade.Instance<TwManager>().SendAction("receiveAffiliateAward", new Dictionary<string, object>(), data => { });
        }

        public void OnQrCode()
        {
            QrCodeBg.SetActive(!QrCodeBg.activeSelf);
        }

        /// <summary>
        /// 分享好友
        /// </summary>
        public void OnClickShare()
        {
            Facade.Instance<WeChatApi>().InitWechat(App.Config.WxAppId);
            var dic = new Dictionary<string, object>();
            dic["type"] = 1;
            dic["image"] = App.UI.CaptureScreenshot();
            dic["shareType"] = 1;
            dic["sharePlat"] = 0;
            UserController.Instance.GetShareInfo(dic, info => Facade.Instance<WeChatApi>().ShareContent(info, str =>
              {
                  var dict = new Dictionary<string, object>();
                  dict["option"] = 2;
                  dict["sharePlat"] = SharePlat.WxSenceSession.ToString();
                  Facade.Instance<TwManager>().SendAction("shareGameResultRequest", dict, null);
              }));
        }

        /// <summary>
        /// 分享朋友圈
        /// </summary>
        public void OnClickShareFriend()
        {
            Facade.Instance<WeChatApi>().InitWechat(App.Config.WxAppId);
            var dic = new Dictionary<string, object>();
            dic["type"] = 1;
            dic["image"] = App.UI.CaptureScreenshot();
            dic["shareType"] = (int)ShareType.Website;
            dic["sharePlat"] = (int)SharePlat.WxSenceTimeLine;
            UserController.Instance.GetShareInfo(dic, info => Facade.Instance<WeChatApi>().ShareContent(info, str =>
            {
                var dict = new Dictionary<string, object>();
                dict["option"] = 2;
                dict["sharePlat"] = SharePlat.WxSenceTimeLine.ToString();
                Facade.Instance<TwManager>().SendAction("shareGameResultRequest", dict, null);
            }), ShareType.Website, SharePlat.WxSenceTimeLine);
        }

        /// <summary>
        /// 解析推广数据并处理相关UI
        /// </summary>
        /// <param name="data"></param>
        private void GetPromotionData(Dictionary<string,object> data)
        {
            var codeUrl = "";
            var allGet = 0;
            var unGet = 0;
            var linkUrl = "";
            data.TryGetValueWitheKey(out codeUrl, KeyPromotionQrCode);
            data.TryGetValueWitheKey(out allGet, KeyPromotionAllAffReward);
            data.TryGetValueWitheKey(out unGet, KeyPromotionUnEffectAffReward);
            data.TryGetValueWitheKey(out linkUrl, KeyPromotionAffDetailUrl);
            AsyncImage.Instance.GetAsyncImage(codeUrl, (texture, s) =>
            {
                PromotionTex.TrySetComponentValue(texture);
            });
            AllAffReward.TrySetComponentValue(allGet);
            UnEffectAffReward.TrySetComponentValue(unGet);
            AffDetailUrl = string.Format(LinkFormat, linkUrl, Singleton<LoginInfo>.Instance.user_id, Singleton<LoginInfo>.Instance.ctoken);
        }

        
    }
}
