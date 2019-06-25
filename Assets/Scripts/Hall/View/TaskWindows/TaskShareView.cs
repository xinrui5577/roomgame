using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common;  
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
    /// todo 待修改
    /// </summary>
    public class TaskShareView : TaskBasseView
    {
        /// <summary>
        /// 分享的信息
        /// </summary>
        [SerializeField]
        private UILabel _shareInfo;
        /// <summary>
        /// 获取奖励按钮
        /// </summary>
        [SerializeField]
        private UIButton _getRewordBtn;
        /// <summary>
        /// 当前的分享次数
        /// </summary>
        private int _curNum;
        /// <summary>
        /// 目标次数
        /// </summary>
        private int _targetNum;

        #region 微信分享
        /// <summary>
        /// 微信分享
        /// </summary>
        public void OnShareWeChat()
        {
            var wechatApi = Facade.Instance<WeChatApi>();
            if (!wechatApi.CheckWechatValidity()) return;
            UserController.Instance.GetShareInfo(SendWechatShare);
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (Data == null) return;
            var data = (Dictionary<string, object>)Data;
            SetSharedInfo(int.Parse(data["validShare"].ToString()),
                          int.Parse(data["awardTime"].ToString()),
                          (bool)data["enableAward"],
                          data["infoClient"].ToString());
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            //打开分享前查询分享次数
            YxWindowManager.ShowWaitFor();
            var parm = new Dictionary<string, object>()
                        {
                            {"option",1},
                            {"bundle_id",Application.bundleIdentifier},
                            {"share_plat",SharePlat.WxSenceTimeLine.ToString() },
                        };
            Facade.Instance<TwManger>().SendAction("shareAwards", parm, str =>
            {
                YxWindowManager.HideWaitFor();
                Data = str;
                OnFreshView();
            });//TODO 待修复
        }

        protected override void OnEnableEx()
        {
            OnShow();
        }

        protected override void OnShow()
        {
            base.OnShow();
            if (_shareInfo!=null)
            {
                _shareInfo.gameObject.SetActive(App.AppStyle.Equals(EAppStyle.Normal));
                _shareInfo.text = "";
            }
            if (_getRewordBtn!=null)
            {
                _getRewordBtn.gameObject.SetActive(App.AppStyle.Equals(EAppStyle.Normal));
                _getRewordBtn.isEnabled = false;
            }
        }

        private ShareInfo _curShareInfo;
        private void SendWechatShare(ShareInfo info)
        { 
            var wechatApi = Facade.Instance<WeChatApi>();
            var appId = App.Config.WxAppId;
            if (string.IsNullOrEmpty(appId)) return;
            _curShareInfo = info;
            wechatApi.InitWechat();
            info.Title = string.Format("{0}{1}", UserInfoModel.Instance.UserInfo.NickM, info.Title);
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
            _curNum = cur;
            _targetNum = target;
            if (_shareInfo!=null)
            {
                _shareInfo.text = string.Format("分享到朋友圈累计{0}次（每天一次）可获得{2}（{1}/{0}）", _targetNum, _curNum, award);
            }
            if (_getRewordBtn!=null)
            {
                _getRewordBtn.isEnabled = receive;
            }   
        }
        /// <summary>
        /// 领取分享奖励（分享按钮点击）
        /// </summary>
        public void GetReward()
        {
            YxWindowManager.ShowWaitFor();
            var parm = new Dictionary<string, object>()
                        {
                            {"option",3},
                            {"bundle_id",Application.bundleIdentifier},
                            {"share_plat",SharePlat.WxSenceTimeLine.ToString() },
                        };
            Facade.Instance<TwManger>().SendAction("shareAwards", parm, str =>
            {
                YxWindowManager.HideWaitFor();
                UpdateView(str);
                var data = (Dictionary<string, object>)str;
                ShowInfos(str,data["awardInfo"].ToString());
                UserController.Instance.GetBackPack(callBack => YxMsgCenterHandler.GetIntance().FireEvent(string.Format("{0}_OnChange", typeof(UserInfoModel).Name)));
            });
        }
        private void OnShareFailed(string obj)
        {
            YxMessageBox.Show("非常抱歉，分享失败了！");
        }

        protected virtual void OnShareSuccess(object msg)
        {
            var parm = new Dictionary<string, object>()
                        {
                            {"option",2},
                            {"bundle_id",Application.bundleIdentifier},
                            {"share_plat",_curShareInfo.Plat.ToString() },
                        };
            Facade.Instance<TwManger>().SendAction("shareAwards", parm, UpdateView); 
        }

        #endregion

        #region 朋友圈分享
        /// <summary>
        /// 朋友圈分享
        /// </summary>
        public void OnSharefriends()
        {
            var wechatApi = Facade.Instance<WeChatApi>();
            if (!wechatApi.CheckWechatValidity()) return;
            UserController.Instance.GetShareInfo(SendWechatShare, ShareType.Website, SharePlat.WxSenceTimeLine);
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
