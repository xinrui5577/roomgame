using System;
using System.Collections;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.TaskWindows
{ 
    public class TaskOnlineRewardView : TaskBasseView
    {
        /// <summary>
        /// 倒计时
        /// </summary>
        public UILabel TimeLabel;
        /// <summary>
        /// 奖励
        /// </summary>
        public UILabel RewardLabel;
        /// <summary>
        /// 结束标语
        /// </summary>
        public GameObject OverLabel;

        public UIButton RewardBtn;

        private static int _curIndex;
        private int _curNeedTime;
        protected override void OnStart()
        {
            base.OnStart();
            var parm = new SFSObject();
            parm.PutInt("idx", -1);
            HallMainController.Instance.SendFrameRequest("",RequestCmd.Gift, parm, OnInitRwared);
        }

        private bool _isStartTime;
        private void OnInitRwared(ISFSObject msg)
        { 
            var flag = CheckReward(msg);
            _isStartTime = flag;
            ChangeState(!flag);
        }

        /// <summary>
        /// 发送领奖
        /// </summary>
        public void OnSendGetReward()
        {
            if (!RewardBtn.isEnabled) return;
            RewardBtn.isEnabled = false;
            var parm = new SFSObject();
            parm.PutInt("idx", _curIndex);
            HallMainController.Instance.SendFrameRequest("",RequestCmd.Gift, parm, OnRewardSuccess);
        }
        
        private void Update()
        {
            if (!_isStartTime) return;
            var curTime = (DateTime.Now-App.LoginTime).TotalMilliseconds;
            if (curTime > _curNeedTime)
            {
                ChangeState(true);
                _isStartTime = false;
                RewardBtn.isEnabled = true;
                return;
            }
//            var d = _curNeedTime - (int)curTime;
//            var ts = new TimeSpan(0, 0, d);
            var d = _curNeedTime - curTime;
            var time = new DateTime(1970, 1, 1);
            var timeStr = time.AddMilliseconds(d).ToString("HH:mm:ss");
            TimeLabel.text = string.Format("领取奖励倒计时：{0}", timeStr);
        }

        private void OnRewardSuccess(ISFSObject msg)
        {
            _curIndex++;
            var hasReward = CheckReward(msg);
            if (!msg.ContainsKey("coin")) return;
            var coin = (int)msg.GetLong("coin");
            var gold = msg.ContainsKey("gold") ? msg.GetInt("gold") : 0;
            YxMessageBox.Show(string.Format("恭喜获得在线奖励，{0}金币！！！", gold), "", (box, btnName) =>
                {
                    _isStartTime = hasReward;
                    ChangeState(!hasReward);
                });
//            UserInfoModel.Instance.UserInfo.CoinA = coin;
//            UserInfoModel.Instance.Save();
        }

        private bool CheckReward(ISFSObject msg)
        {
            if (msg == null) return false;
            var cfg = msg.GetUtfString("config");
            var cfgInfo = cfg.Split(',');
            var startIndex = _curIndex * 2;
            var hasReward = startIndex < cfgInfo.Length;
            if (hasReward)
            {
                var count = startIndex + 2;
                for (var i = startIndex; i < count; i++)
                {
                    int.TryParse(cfgInfo[i], out _curNeedTime);
                    _curNeedTime *= 1000;
                    RewardLabel.text = cfgInfo[++i];
                }
            }
            else
            {
                RewardBtn.gameObject.SetActive(false);
                OverLabel.SetActive(true);
            }
            return hasReward;
        }
    }
}
