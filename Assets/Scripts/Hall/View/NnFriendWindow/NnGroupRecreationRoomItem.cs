using UnityEngine;

namespace Assets.Scripts.Hall.View.NnFriendWindow
{
    public class NnGroupRecreationRoomItem : MonoBehaviour
    {
        public UILabel GroupRoomNameLabel;
        public UILabel MaxJoinLabel;
        public UILabel MinJoinLabel;

        public void InitRoomData(string roomName, int maxValue, int minValue)
        {
            if (GroupRoomNameLabel != null) GroupRoomNameLabel.text = roomName;
            if (MaxJoinLabel != null) MaxJoinLabel.text = maxValue.ToString();
            if (MinJoinLabel != null) MinJoinLabel.text = minValue.ToString();
        }
    }
}
