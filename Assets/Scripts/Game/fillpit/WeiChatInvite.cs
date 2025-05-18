using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.fillpit
{
    public class WeiChatInvite : MonoBehaviour
    {
        /// <summary>
        /// 微信邀请按钮
        /// </summary>
        public GameObject WeiChatInviteBtn;

        public void OnGameBegin()
        {
            if (WeiChatInviteBtn == null) return;
            WeiChatInviteBtn.SetActive(false);
        }

        /// <summary>
        /// 点击事件,外挂
        /// </summary>
        public void OnClickWeiChatBtn()
        {
            var roomInfo = App.GetGameManager<FillpitGameManager>().RoomInfo;
            ShareFriend(roomInfo.RoomId.ToString(), roomInfo.Rule);
        }

        /// <summary>
        /// Key分享类型
        /// </summary>
        private const string KeyShareType = "type";
        /// <summary>
        /// Key房间ID
        /// </summary>
        private const string KeyRoomId = "roomid";
        /// <summary>
        /// Key 微信分享事件
        /// </summary>
        private const string KeyShareEvent = "event";
        /// <summary>
        /// Key 房间玩法
        /// </summary>
        private const string KeyRoomRule = "roomRule";
        /// <summary>
        /// Key 分享平台
        /// </summary>
        public const string KeySharePlat = "sharePlat";
        /// <summary>
        /// Key 茶馆ID
        /// </summary>
        public const string KeyTeaId = "TeaId";
        /// <summary>
        /// Value微信邀请好友type值
        /// </summary>
        private const int ValueShareType = 0;
        /// <summary>
        /// Value微信分享事件
        /// </summary>
        private const string ValueShareEvent = "findroom";
        /// <summary>
        /// Value微信邀请好友type值
        /// </summary>
        private const int ValueSharePlatType = 0;

        /// <summary>
        /// 微信邀请好友(分享游戏数据)
        /// </summary>
        /// <param name="roomId">房间号</param>
        /// <param name="roomInfo">房间信息</param>
        /// <param name="gameKey">gamekey</param>
        /// <param name="teaId">茶馆ID</param>
        public static void ShareFriend(string roomId, string roomInfo, string gameKey = "", string teaId = "")
        {
            if (CheckWeChat())
            {
                if (string.IsNullOrEmpty(gameKey))
                {
                    gameKey = App.GameKey;
                }
                var dic = new Dictionary<string, object>
                {
                    {KeyShareType, ValueShareType},
                    {KeyRoomId, roomId},
                    {KeyShareEvent, ValueShareEvent},
                    {KeyRoomRule, roomInfo},
                    {KeySharePlat,ValueSharePlatType}
                };
                if (!string.IsNullOrEmpty(KeyTeaId))
                {
                    dic.Add(KeyTeaId, teaId);
                }

                var gdata = App.GetGameData<FillpitGameData>();

                UserController.Instance.GetShareInfo(dic, info =>
                {
                    info.ShareData["title"] = gdata.GetPlayerInfo().NickM + "-" + info.ShareData["title"];
                    info.ShareData["content"] = "[填大坑]房间号:[" + roomId + "] ;";
                    info.ShareData["content"] += roomInfo + "。速来玩吧! (仅供娱乐，禁止赌博)";
                    Facade.Instance<WeChatApi>().ShareContent(info);
                }, ShareType.Website, SharePlat.WxSenceSession, null, gameKey);
            }
        }

        /// <summary>
        /// 检测微信是否可用
        /// </summary>
        /// <returns></returns>
        public static bool CheckWeChat()
        {
            var api = Facade.Instance<WeChatApi>();
            return api.InitWechat() && api.CheckWechatValidity();
        }

        public void SetWeiChatBtnActive(bool active)
        {
            if (WeiChatInviteBtn == null)
            {
                Debug.LogError("The WeiChat button is null!!");
                return;
            }
            WeiChatInviteBtn.SetActive(active);
        }
    }
}
