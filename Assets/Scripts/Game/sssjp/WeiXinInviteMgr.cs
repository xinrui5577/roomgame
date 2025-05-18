using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.sssjp.ImgPress.Main;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.sssjp
{
    /// <summary>
    /// 聊天管理类 
    /// </summary>
    public class WeiXinInviteMgr : MonoBehaviour
    {

        public GameObject ChatInviteBtn;

        /// <summary>
        /// 点击微信邀请按钮
        /// </summary>
        public void OnClickChatInvite()
        {
            var roomInfo = App.GetGameManager<SssjpGameManager>().RoomInfo;
            YxTools.ShareFriend(roomInfo.RoomID.ToString(), roomInfo.RuleInfo);
        }
       
    }
}