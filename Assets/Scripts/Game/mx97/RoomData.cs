using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.mx97
{
    public class RoomData
    {
        public int MinGold;
        public int MaxGold;
        public int EndTime;
        public string Name;
        /// <summary>
        /// 游戏房间类型，比赛还是普通房间
        /// </summary>
        public int RoomType;
        public string RoomIcon;
        public int Ante;
        /// <summary>
        /// 房间id
        /// </summary>
        public int RoomId;

        public void ParseData(ISFSObject temp)
        {
            Ante = temp.GetInt("ante");
            MinGold = temp.GetInt("min");
            MaxGold = temp.ContainsKey("max")?temp.GetInt("max"):0;
            string icon = "room_";
            if (Ante < 1000)
            {
                icon = icon + "1";
            }
            else if (Ante < 10000)
            {
                icon = icon + "2";
            }
            else
            {
                icon = icon + "3";
            }
            RoomIcon = icon;
            RoomId = temp.GetInt("gameType");
            Name = temp.GetUtfString("name");
            RoomType = temp.ContainsKey("roomType") ? temp.GetInt("roomType") : 0;
            EndTime = temp.ContainsKey("endTime") ? temp.GetInt("endTime") : 0;
        }
    }
}

