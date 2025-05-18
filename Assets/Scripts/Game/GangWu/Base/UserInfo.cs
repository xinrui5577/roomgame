namespace Assets.Scripts.Game.GangWu.Base
{
    [System.Serializable]
    public class UserInfo :BaseObj
    {
        public string Name;
        public int Id = -1;
        public string Ip = string.Empty;
        public int Level;
        public string HeadImage = "HS_9";
        public int Sex;
        public int Seat;
        /// <summary>
        /// 当前房间的金额
        /// </summary>
        public int RoomGold;
        private long _coin;

        /// <summary>
        /// 玩家载入游戏的手中的金额
        /// </summary>
        public long Gold
        {
            get
            {
                return decript(_coin);
            }
            set
            {
                _coin = encript(value);
            }
        }
        public bool IsReady;
        private string _country;

        public string Country
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                    _country = "未能获得地址信息!";
                else
                    _country = value;
            }
            get { return _country; }
        }
    }
}
