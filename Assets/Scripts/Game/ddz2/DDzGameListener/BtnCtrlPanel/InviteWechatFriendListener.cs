using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.BtnCtrlPanel
{
    public class InviteWechatFriendListener : ServEvtListener
    {

        [SerializeField]
        protected GameObject IvtWechatFriendBtn;

      


        /// <summary>
        /// 房间号
        /// </summary>
        private string _roomId="";
 

        /// <summary>
        /// 服务器发送过来的牌局信息
        /// </summary>
        private string _ruleInfo;


        protected override void OnAwake()
        {
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyGetGameInfo, OnGetGameInfoData);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyOnRejoin, OnGetGameInfoData);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeAllocate, OnAlloCateCds);
        }


        private void OnGetGameInfoData(DdzbaseEventArgs args)
        {
            SetAllBtnsActive(false);
            var data = args.IsfObjData;

            if(data.ContainsKey(NewRequestKey.KeyRoomId))_roomId = data.GetInt(NewRequestKey.KeyRoomId).ToString(CultureInfo.InvariantCulture);

            if (data.ContainsKey(NewRequestKey.KeyGameStatus)
                && data.GetInt(NewRequestKey.KeyGameStatus) == GlobalConstKey.StatusIdle&&App.GetGameData<DdzGameData>().IsRoomGame)
            {
                if (data.ContainsKey(NewRequestKey.KeyCurRound) && data.GetInt(NewRequestKey.KeyCurRound) <= 1) SetAllBtnsActive(true);
            }

            if(data.ContainsKey("rule"))
            {
                _ruleInfo=data.GetUtfString("rule");
            }
        }

        /// <summary>
        /// 已经开始发牌了说明人齐了，开始游戏了，可以隐藏微信好友按钮了
        /// </summary>
        /// <param name="args"></param>
        private void OnAlloCateCds(DdzbaseEventArgs args)
        {
            SetAllBtnsActive(false);
        }

        /// <summary>
        /// 当点击分享邀请好友
        /// </summary>
        public void OnClickInviteFriend()
        {
            WeixinShare();
        }


        void WeixinShare()
        {
            var gdata = App.GetGameData<DdzGameData>();
            var dic = new Dictionary<string, object>();
            dic.Add("type", 0);
            dic.Add("sharePlat", 0);
            dic.Add("event", "findroom");
            dic.Add("roomid", _roomId);
            dic.Add("roomRule", _ruleInfo);
            Facade.Instance<WeChatApi>().InitWechat(AppInfo.WxAppId);
            UserController.Instance.GetShareInfo(dic, (info) =>
            {
                info.ShareData["title"] = gdata.GetPlayerInfo().NickM + "-" + info.ShareData["title"];
                info.ShareData["content"] = "[斗地主]房间号:[" + _roomId + "] ;";
                info.ShareData["content"] += _ruleInfo + "。速来玩吧! (仅供娱乐，禁止赌博)";
                Facade.Instance<WeChatApi>().ShareContent(info);
            });

        }


        private void SetAllBtnsActive(bool isActive)
        {
            IvtWechatFriendBtn.SetActive(isActive);
        }

        public override void RefreshUiInfo()
        {
            
        }
    }
}
