namespace Assets.Scripts.Game.Mahjong2D.CommonFile.Data
{
    public class UserInfo :BaseObj
    {
        public string name;
        public int id;
        public string ip;
        public int level;
        public string HeadImage;
        public short Sex;
        public int Seat;
        private long _coin;
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
        public bool isReady;
        public string Country;
    }
}
