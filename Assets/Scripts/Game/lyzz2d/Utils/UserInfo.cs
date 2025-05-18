namespace Assets.Scripts.Game.lyzz2d.Utils
{
    public class UserInfo : BaseObj
    {
        private long _coin;
        public string Country;
        public string HeadImage;
        public int id;
        public string ip;
        public bool isReady;
        public int level;
        public string name;
        public int Seat;
        public short Sex;

        public long Gold
        {
            get { return decript(_coin); }
            set { _coin = encript(value); }
        }
    }
}