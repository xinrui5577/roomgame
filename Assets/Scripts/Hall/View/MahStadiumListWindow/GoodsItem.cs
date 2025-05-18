using UnityEngine;

namespace Assets.Scripts.Hall.View.MahStadiumListWindow
{
    /// <summary>
    /// 推荐或者好评帮的信息
    /// </summary>
    public class GoodsItem : MonoBehaviour
    {
        public UILabel SeriaNumber;
        public UILabel RoomName;
        public UILabel MahStadimId;

        public void InitInfo(int num, string roomName, string commend)
        {
            SeriaNumber.text = string.Format("{0}{1}", num, ".");
            RoomName.text = roomName;
            MahStadimId.text = commend;
        }
    }
}
	
