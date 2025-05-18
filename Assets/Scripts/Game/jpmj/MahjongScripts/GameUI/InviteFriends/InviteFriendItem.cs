using System.Collections.Generic;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI.InviteFriends
{
    public class InviteFriendItem : MonoBehaviour
    {
        public RawImage Icon;
        public Text Name;
        public Text Id;

        private string mRoomID;
      
        string setName
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                    Name.text = value;
            }
        }

        string setId
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                    Id.text = value;
            }
        }

        public void Init(FriendItemData data, string roomID)
        {
            setName = data.Nick;
            setId = data.UserID;
            SetHead(data.Avater);
            mRoomID = roomID;
        }

        public void SetHead(string url)
        {       
            AsyncImage.GetInstance().SetTextureWithAsyncImage(url, Icon, Icon.texture);
        }
        public void OnInviteFriendClick()
        {
            var apiInfo = new Dictionary<string, object>()
            {
                { "bundleID", Application.bundleIdentifier },
                { "inviteId", Id.text },
                { "roomId", mRoomID }
            };

            Facade.Instance<TwManager>().SendAction("mahjongwm.inviteWmFriends", apiInfo, data =>
            { });

            //EventDispatch.Dispatch((int)UIEventId.HideInviteFriendPnl);
        }

    }
}