using System.Collections.Generic;
using Assets.Scripts.Common;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.SpinningWindows
{
    /// <summary>
    /// 奖励item
    /// </summary>
    public class RewardItemView : YxView
    {
        [Tooltip("奖品名称labe")] 
        public UILabel RewardName;
        [Tooltip("奖品个数labe")] 
        public UILabel RewardCount;
        [Tooltip("奖品个数格式")] 
        public string RewardCountFormat = "{0}台";
        [Tooltip("分享兑换")] 
        public GameObject State_Share;
        [Tooltip("兑换")] 
        public GameObject State_Convert;
        [Tooltip("已兑换")] 
        public GameObject State_HasConvert;
         [Tooltip("分享类型")] 
        public ShareType EShareType = ShareType.Website;
         [Tooltip("分享类型")] 
        public SharePlat ESharePlat = SharePlat.WxSenceTimeLine;
        protected override void OnFreshView()
        {
            if (Data == null) return;
            var rData = Data as RewardItemData;
            if (rData == null) return;
            RewardName.text = rData.Name;
            RewardCount.text = string.Format(RewardCountFormat, rData.Count);
            ChangeBtnState(rData.Status);
        }

        private void ChangeBtnState(int state)
        {
            if (State_Share != null) State_Share.SetActive(state == 0);
            if (State_Convert != null) State_Convert.SetActive(state == 1);
            if (State_HasConvert != null) State_HasConvert.SetActive(state >= 2);
            var data = GetData<RewardItemData>();
            if (data != null)
            {
                data.Status = state;
            } 
        }

        /// <summary>
        /// 分享兑换
        /// </summary>
        public void OnRewardExchange()
        {
            var wechatApi = Facade.Instance<WeChatApi>();
            if (!wechatApi.CheckWechatValidity()) return;
            UserController.Instance.GetShareInfo(SendWechatShare, EShareType, ESharePlat);
        }

        private ShareInfo _curShareInfo;
        private void SendWechatShare(ShareInfo info)
        {
            var wechatApi = Facade.Instance<WeChatApi>();
            var appId = App.Config.WxAppId;
            if (string.IsNullOrEmpty(appId)) return;
            _curShareInfo = info;
            wechatApi.InitWechat(appId);
            var ritem = GetData<RewardItemData>();
            var rdName = ritem == null ? "" : ritem.Name;
            info.Title = string.Format(info.Content, rdName);
            wechatApi.ShareContent(info, OnShareSuccess, null, OnShareFailed);
        }

        protected virtual void OnShareSuccess(object msg)
        {
            if (Data == null) return;
            var rData = Data as RewardItemData;
            if (rData == null) return;
            var parm = new Dictionary<string, object>();
            parm["id"] = rData.Id;
            Facade.Instance<TwManger>().SendAction("shareInfo_yr", parm, obj => ChangeBtnState(1));
        }

        private void OnShareFailed(string obj)
        {
            YxMessageBox.Show("非常抱歉，分享失败了！");
        }

        public void UpdateBtns(object obj)
        {
            ChangeBtnState(2); 
            if (CallBack != null) CallBack(obj);
        }
    }

     /* {
      "name": "\u8868\u60c5\u798f\u888b",
      "id": "41",
      "user_id": "200241",
      "award_id": "7",
      "status": "0",
      "phone": null,
      "address": null,
      "create_dt": "2017-07-05 11:40:12",
      "last_update_dt": null
    }*/
    public class RewardItemData : SpinningItemData
    {
        public int Status;
        public string UserId;
        public string Phone;
        public string Address;
        public string CreateDt;
        public string LastUpdateDt;
        public int Type;//类型0实物1虚拟物品2卡密
        /// <summary>
        ///兑换物品类型
        /// </summary>
        private string _keyType="type_s";

        public RewardItemData(IDictionary<string, object> dict)  : base(dict)
        {
            if (dict == null) return; 
            if (dict.ContainsKey("status"))
            {
                if (!int.TryParse(dict["status"].ToString(), out Status))
                {
                    Status = -1;
                }
            } 
            if (dict.ContainsKey("user_id"))
            {
                var temp = dict["user_id"];
                if (temp != null) UserId = temp.ToString();
            }
            if (dict.ContainsKey("phone"))
            {
                var temp = dict["phone"];
                if (temp != null) Phone = temp.ToString();
            }
            if (dict.ContainsKey("address"))
            {
                var temp = dict["address"];
                if (temp != null) Address = temp.ToString();
            }
            if (dict.ContainsKey("create_dt"))
            {
                var temp = dict["create_dt"];
                if (temp != null) CreateDt = temp.ToString();
            }
            if (dict.ContainsKey("last_update_dt"))
            {
                var temp = dict["last_update_dt"];
                if (temp != null) LastUpdateDt = temp.ToString();
            }
            if (dict.ContainsKey(_keyType))
            {
                Type = int.Parse(dict[_keyType].ToString());
            }
        }
    }
}
