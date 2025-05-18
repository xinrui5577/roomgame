using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.DataBundles;

namespace Assets.Scripts.Hall.View.NnFriendWindow
{
    public class NnFriendGroupItem : MonoBehaviour
    {
        [HideInInspector]
        public string CurrentGroupName;
        [HideInInspector]
        public string CurrentGroupSign;
        [HideInInspector] 
        public int CurrentGroupId;
        [HideInInspector] 
        public string CurrentGroupOwnerId;

        public UILabel GroupNameUp;
        public UILabel CreatRoomCountUp;
        public UILabel GroupNameDown;
        public UILabel CreatRoomCountDown;
        public YxBaseTextureAdapter UserOwnerHead;

        private int _userCount;

        public void InitData(string groupName,int roomCount ,int userNum, string avatarData,int groupId,string userId,string groupSign)
        {
            CurrentGroupOwnerId = userId;
            CurrentGroupName = groupName;
            CurrentGroupSign = groupSign;
            CurrentGroupId = groupId;
            _userCount = userNum;
            gameObject.SetActive(true);
            GroupNameUp.text = string.Format("{0}({1})", groupName, userNum);
            CreatRoomCountUp.text = string.Format("已开房间({0})", roomCount);
            GroupNameDown.text = string.Format("{0}({1})", groupName, userNum);
            CreatRoomCountDown.text = string.Format("已开房间({0})", roomCount);
            PortraitDb.SetPortrait(avatarData, UserOwnerHead, 1);
            UserOwnerHead.gameObject.SetActive(true);
        }

        public void OnFreshRoomCount(int roomCount)
        {
            CreatRoomCountUp.text = string.Format("已开房间({0})", roomCount);
            CreatRoomCountDown.text = string.Format("已开房间({0})", roomCount);
        }

        public void OnAgreePeopleCount()
        {
            GroupNameUp.text = string.Format("{0}({1})", CurrentGroupName, ++_userCount);
            GroupNameDown.text = string.Format("{0}({1})", CurrentGroupName, _userCount);
        }

        public void OnDeletePeopleCount()
        {
            GroupNameUp.text = string.Format("{0}({1})", CurrentGroupName, --_userCount);
            GroupNameDown.text = string.Format("{0}({1})", CurrentGroupName, _userCount);
        }
    }
}
