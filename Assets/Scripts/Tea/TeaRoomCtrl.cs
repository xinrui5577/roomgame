using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Tea
{
    /// <summary>
    /// 茶馆创建房间背景图控制
    /// </summary>
    public class TeaRoomCtrl : MonoBehaviour
    {
        public List<int> UserSeats;
        public UISprite BackGround;
        public string BgPrefix;

        public void ChangeTableItemBg(RoomInfoData roomData, UISprite[] emptySeat)
        {
            if (BackGround)
            {
                if (roomData.UserNum <= UserSeats[0])
                {
                    BackGround.spriteName = BgPrefix + UserSeats[0];
                }

                for (int i = 0; i < UserSeats.Count; i++)
                {
                    if (UserSeats[i] < roomData.UserNum && roomData.UserNum <= UserSeats[i + 1])
                    {
                        BackGround.spriteName = BgPrefix + UserSeats[i + 1];
                    }
                }

                for (int i = 0; i < emptySeat.Length; i++)
                {
                    emptySeat[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
