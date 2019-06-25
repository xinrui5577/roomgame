using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.ShareWindow
{
    /// <summary>
    /// 分享界面
    /// </summary>
    public class ShareWindow : YxNguiWindow
    {
        /// <summary>
        /// 分享信息
        /// </summary>
        [SerializeField]
        public UILabel ShareInfo;
        /// <summary>
        /// 获取奖励按键
        /// </summary>
        [SerializeField]
        public UIButton GetRewardBtn;
        /// <summary>
        /// 当前次数
        /// </summary>
        [HideInInspector]
        public int CurNum;
        /// <summary>
        /// 目标次数
        /// </summary>
        [HideInInspector]
        public int TargetNum;
        /// <summary>
        /// 分享本地显示对象
        /// </summary>
        public ShareItem[] Shares;

        protected override void OnStart()
        {
            base.OnStart();
            for (int i = 0; i < Shares.Length; i++)
            {
                Shares[i].Init((SharePlat)i);
            }
        }

        protected override void OnShow()
        {
            ShareInfo.text = "";
        }

        public void SetSharedInfo(int cur,int target,bool receive,string award)
        {
            CurNum = cur;
            TargetNum = target;
            ShareInfo.text = string.Format("分享到朋友圈累计{0}次（每天一次）可获得{2}（{1}/{0}）",TargetNum,CurNum, award);
            GetRewardBtn.isEnabled = receive;
        }

        /// <summary>
        /// 获取奖励
        /// </summary>
        public void GetReward()
        {
            YxWindowManager.ShowWaitFor();
            var parm = new Dictionary<string, object>()
                        {
                            {"option",3},
                            {"bundle_id",Application.bundleIdentifier},
                            {"share_plat",((int)SharePlat.WxSenceTimeLine).ToString() },
                        };
            Facade.Instance<TwManger>().SendAction("shareAwards", parm, str =>
            {
                YxWindowManager.HideWaitFor();
                UpdateView(str);
                var data = (Dictionary<string, object>)str;
                YxMessageBox.Show(data["awardInfo"].ToString());
                UserController.Instance.GetBackPack();
            });
        }

    }
}
