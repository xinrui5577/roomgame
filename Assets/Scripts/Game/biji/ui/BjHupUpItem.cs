using Assets.Scripts.Common.Adapters;
using UnityEngine;
using YxFramwork.Common.DataBundles;

namespace Assets.Scripts.Game.biji.ui
{
    public class BjHupUpItem : MonoBehaviour
    {
        public NguiTextureAdapter UserHead;
        public GameObject RoomOwnerIcon;
        public UILabel UserState;
        public UILabel UserName;

        public void SetItemView(string head, int sex, string userName, bool isSponsor = false)
        {
            PortraitDb.SetPortrait(head, UserHead, sex);
            UserName.text = userName;
            name = userName;
            if (isSponsor)
            {
                UserState.text = "发起者";
            }
        }

        public void ShowRoomOwner()
        {
            RoomOwnerIcon.SetActive(true);
        }

        public void ChangeStae()
        {
            UserState.text = "已同意";
        }
    }
}
