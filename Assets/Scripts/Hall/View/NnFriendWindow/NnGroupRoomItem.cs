using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.DataBundles;

namespace Assets.Scripts.Hall.View.NnFriendWindow
{
    public class NnGroupRoomItem : MonoBehaviour
    {
        public YxBaseTextureAdapter UserHead;
        public UILabel UserName;
        public UILabel UserId;
        public UILabel GameName;
        public UILabel RoomId;
        public UILabel Ante;
        public UILabel Round;
        public UILabel PayWay;
        public UILabel Rule;

        public void InitData(string userHead,string userName,string userId,string gameName,string roomId,string ante,string round,string payWay,string rule)
        {
            PortraitDb.SetPortrait(userHead, UserHead, 1);
            UserName.text = userName;
            UserId.text =string.Format("{0}ID:{1}{2}","(",userId,")");
            GameName.text = gameName;
            RoomId.text ="房号"+roomId;
            Ante.text = ante;
            Round.text = round;
            PayWay.text = payWay;
            Rule.text = rule;
        }
    }
}
