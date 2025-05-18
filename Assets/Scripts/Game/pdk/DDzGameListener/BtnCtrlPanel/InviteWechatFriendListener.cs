using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.pdk.DDz2Common;
using Assets.Scripts.Game.pdk.DdzEventArgs;
using Assets.Scripts.Game.pdk.InheritCommon;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.pdk.DDzGameListener.BtnCtrlPanel
{
    public class InviteWechatFriendListener : ServEvtListener
    {

        public GameObject IvtWechatFriendBtn;

        /// <summary>
        /// 房间号
        /// </summary>
        private string _roomId = "";
        /// <summary>
        /// 最大局数
        /// </summary>
        private string _maxRound = "";

        private int _jiabei = 0;

        private GlobalConstKey.GameType _curGameType = GlobalConstKey.GameType.CallScore;

        protected override void OnAwake()
        {
            PdkGameManager.AddOnGameInfoEvt(OnGetRejoionData);
            PdkGameManager.AddOnGetRejoinDataEvt(OnGetRejoionData);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeAllocate, OnAlloCateCds);
        }


        private void OnGetRejoionData(object obj, DdzbaseEventArgs args)
        {
            IvtWechatFriendBtn.SetActive(false);
            var data = args.IsfObjData;

            if (data.ContainsKey(NewRequestKey.KeyRoomId)) _roomId = data.GetInt(NewRequestKey.KeyRoomId).ToString(CultureInfo.InvariantCulture);

            if (data.ContainsKey(NewRequestKey.KeyMaxRound)) _maxRound = data.GetInt(NewRequestKey.KeyMaxRound).ToString(CultureInfo.InvariantCulture);

            if (data.ContainsKey(NewRequestKey.KeyJiaBei)) _jiabei = data.GetInt(NewRequestKey.KeyJiaBei);

            if (data.ContainsKey(NewRequestKey.KeyQt)) _curGameType = (GlobalConstKey.GameType)data.GetInt(NewRequestKey.KeyQt);


            if (data.ContainsKey(NewRequestKey.KeyState)
                && data.GetInt(NewRequestKey.KeyState) == GlobalConstKey.StatusIdle)
            {
                if (data.ContainsKey(NewRequestKey.KeyCurRound) && data.GetInt(NewRequestKey.KeyCurRound) <= 1) IvtWechatFriendBtn.SetActive(true);
            }
        }

        /// <summary>
        /// 已经开始发牌了说明人齐了，开始游戏了，可以隐藏微信好友按钮了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnAlloCateCds(object sender, DdzbaseEventArgs args)
        {
            IvtWechatFriendBtn.SetActive(false);
        }

        /// <summary>
        /// 当点击分享邀请好友
        /// </summary>
        public void OnClickInviteFriend()
        {
            /*            Facade.Instance<WeChatApi>().InitWechat(AppInfo.AppId);
                        UserController.Instance.GetShareInfo((info) =>
                        {
                            info.ShareData["content"] = "房间号:[" + _roomId + "]," + _maxRound + "局,";
            /*                switch (_curGameType)
                            {
                                case GlobalConstKey.GameType.CallScore:
                                case GlobalConstKey.GameType.CallScoreWithFlow:
                                    info.ShareData["content"] += "叫分玩法,";
                                    break;
                                case GlobalConstKey.GameType.Kick:
                                    info.ShareData["content"] += "踢地主玩法,";
                                    break;
                                case GlobalConstKey.GameType.Grab:
                                    info.ShareData["content"] += "抢地主玩法,";
                                    break;
                            }#1#

            /*                //是否加倍
                            if (_jiabei > 0)
                            {
                                info.ShareData["content"] += "可加倍,";
                            }#1#
                            info.ShareData["content"] += App.GetGameData<GlobalData>().RoomPlayInfo;
                            info.ShareData["content"] += "，速来玩吧!";
                            Facade.Instance<WeChatApi>().ShareContent(info);
                        });*/

            Facade.Instance<WeChatApi>().InitWechat(AppInfo.WxAppId);

            var dic = new Dictionary<string, object>
                {
                    {"type", 0},
                    {"roomid", _roomId},
                    {"event", "findroom"},
                    {"roomRule", App.GetGameData<GlobalData>().RoomPlayInfo},
                    {"sharePlat", 0}
                };

            UserController.Instance.GetShareInfo(dic, (info) =>
                {
                    info.ShareData["title"] = "王牌游戏-跑得快 房间号:[" + _roomId + "]";
                    info.ShareData["content"] = _maxRound + "局,";
                    info.ShareData["content"] += App.GetGameData<GlobalData>().RoomPlayInfo;
                    info.ShareData["content"] += "，速来玩吧!";
                    Facade.Instance<WeChatApi>().ShareContent(info);
                });
        }

        public override void RefreshUiInfo()
        {

        }
    }
}
