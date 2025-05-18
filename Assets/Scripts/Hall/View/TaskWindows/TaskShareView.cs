using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Tea;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.TaskWindows
{
    /// <summary>
    /// 分享界面（通用分享与茶馆内分享公用）
    /// </summary>
    public class TaskShareView : TaskBasseView
    {   
        [Tooltip("二维码")]
        public UITexture QrCodeTexture;
        [Tooltip("分享提示信息")]
        public UILabel ShareNoticeInfo;
        [Tooltip("获取奖励按钮")]
        public UIButton GetRewordBtn;
        [Tooltip("分享时是否带茶馆信息（茶馆分享使用）")]
        public bool ShareWithTeaInfo;
        [Tooltip("分享提示格式")]
        public string ShareNoticeFormat = "分享到朋友圈累计{0}次(每天一次)可获得{2}({1}/{0})";
        /// <summary>
        /// Key 有效分享次数
        /// </summary>
        private const string KeyValuedShare = "validShare";
        /// <summary>
        /// Key 领取奖励所需次数
        /// </summary>
        private const string KeyAwardNeedTime = "awardTime";
        /// <summary>
        /// Key 是否允许领取奖励
        /// </summary>
        private const string KeyEnableAward = "enableAward";
        /// <summary>
        /// key 奖励信息
        /// </summary>
        private const string KeyAwardInfo = "infoClient";
        /// <summary>
        /// Key分享选项 1.分享信息查询 2.分享成功 3.领取分享奖励
        /// </summary>
        private const string KeyShareOption = "option";
        /// <summary>
        /// Key 分享平台
        /// </summary>
        private const string KeySharePlatform = "share_plat";
        /// <summary>
        /// Key 分享bundle id 
        /// </summary>
        private const string KeyShareBundleId = "bundle_id";
        /// <summary>
        /// Key获得奖励信息
        /// </summary>
        private const string KeyGetAwardInfo = "awardInfo";
        /// <summary>
        /// Key 分享请求名称
        /// </summary>
        private const string KeyShareAwardAction = "shareAwards";

        #region 微信分享
        /// <summary>
        /// 微信分享
        /// </summary>
        public void OnShareWeChat()
        {
            UserController.Instance.GetShareInfo(
                GetDicData(SharePlat.WxSenceSession),
                SendWechatShare,
                ShareType.Website,
                SharePlat.WxSenceSession,
                null,
                App.GameKey
                );
        }


        #region Life Cricle

        private Dictionary<string,object> GetDicData(SharePlat plat)
        {
            var dic = new Dictionary<string, object>();
            dic.Add(YxTools.KeySharePlat, plat);
            if (ShareWithTeaInfo)
            {
                dic.Add(YxTools.KeyTeaId, TeaUtil.CurTeaId);
            }
            return dic;
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var data = Data as Dictionary<string, object>;
            if (data == null) return;
            int validShare;
            int awardTime;
            bool enableAward;
            string infoClient;
            data.TryGetValueWitheKey(out validShare, KeyValuedShare);
            data.TryGetValueWitheKey(out awardTime, KeyAwardNeedTime);
            data.TryGetValueWitheKey(out enableAward, KeyEnableAward);
            data.TryGetValueWitheKey(out infoClient, KeyAwardInfo);
            SetSharedInfo(validShare, awardTime, enableAward, infoClient);
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            //打开分享前查询分享次数
            var parm = new Dictionary<string, object>()
                        {
                            {KeyShareOption,1},
                            {KeyShareBundleId,Application.bundleIdentifier},
                            {KeySharePlatform,SharePlat.WxSenceTimeLine.ToString() },
                        };
            Facade.Instance<TwManager>().SendAction(KeyShareAwardAction, parm, UpdateView);
        }

        protected override void OnEnable()
        {
            OnShow();
        }

        protected override void OnShow()
        {
            base.OnShow();
            if (ShareNoticeInfo != null)
            {
                ShareNoticeInfo.gameObject.SetActive(App.AppStyle.Equals(YxEAppStyle.Normal));
                ShareNoticeInfo.text = "";
            }
            if (GetRewordBtn != null)
            {
                GetRewordBtn.gameObject.SetActive(App.AppStyle.Equals(YxEAppStyle.Normal));
                GetRewordBtn.isEnabled = false;
            }
        }
        #endregion

        /// <summary>
        /// 微信分享信息
        /// </summary>
        private ShareInfo _curShareInfo;

        /// <summary>
        /// 获取微信分享数据
        /// </summary>
        /// <param name="info"></param>
        private void  SendWechatShare(ShareInfo info)
        { 
            var wechatApi = Facade.Instance<WeChatApi>();
            wechatApi.InitWechat();
            if (!wechatApi.CheckWechatValidity()) return;
            _curShareInfo = info;
            wechatApi.ShareContent(info, OnShareSuccess, null, OnShareFailed);
        }
        /// <summary>
        /// 显示分享的相关文本
        /// </summary>
        /// <param name="cur"></param>
        /// <param name="target"></param>
        /// <param name="receive"></param>
        /// <param name="award"></param>
        public void SetSharedInfo(int cur, int target, bool receive, string award)
        {
            ShareNoticeInfo.TrySetComponentValue(string.Format(ShareNoticeFormat, target, cur, award));
            if (GetRewordBtn!=null)
            {
                GetRewordBtn.isEnabled = receive;
            }
        }
        /// <summary>
        /// 领取分享奖励（分享按钮点击）
        /// </summary>
        public void GetReward()
        {
            var parm = new Dictionary<string, object>()
                        {
                            {KeyShareOption,3},
                            {KeyShareBundleId,Application.bundleIdentifier},
                            {KeySharePlatform,SharePlat.WxSenceTimeLine.ToString() },
                        };
            Facade.Instance<TwManager>().SendAction(KeyShareAwardAction, parm, str =>
            {
                UpdateView(str);
                var data = (Dictionary<string, object>)str;
                ShowInfos(str,data[KeyGetAwardInfo].ToString());
                UserController.Instance.GetBackPack(callBack => Facade.EventCenter.DispatchEvent<string,object>(string.Format("{0}_OnChange", typeof(UserInfoModel).Name)));
            });
        }
        private void OnShareFailed(string info)
        {
            YxWindowManager.ShowMessageWindow(info);
        }

        protected virtual void OnShareSuccess(object msg)
        {
            if (!ShareWithTeaInfo) //茶馆内部分享无奖励领取（目前需求）
            {
                var parm = new Dictionary<string, object>()
                        {
                            {KeyShareOption,2},
                            {KeyShareBundleId,Application.bundleIdentifier},
                            {KeySharePlatform,_curShareInfo.Plat.ToString() },
                        };
                Facade.Instance<TwManager>().SendAction(KeyShareAwardAction, parm, UpdateView);
            }
        }

        #endregion

        #region 朋友圈分享
        /// <summary>
        /// 朋友圈分享
        /// </summary>
        public void OnSharefriends()
        {
            UserController.Instance.GetShareInfo
                (
                   GetDicData(SharePlat.WxSenceTimeLine),
                   SendWechatShare,
                   ShareType.Website,
                   SharePlat.WxSenceTimeLine,
                   null,
                   App.GameKey
                );
        }
        #endregion

        #region QQ分享
        /// <summary>
        /// qq分享
        /// </summary>
        public void OnShareQq()
        {
        }
        #endregion

        #region 新浪分享
        /// <summary>
        /// 新浪分享
        /// </summary>
        public void OnShareSina()
        {
        }
        #endregion

    }
}
