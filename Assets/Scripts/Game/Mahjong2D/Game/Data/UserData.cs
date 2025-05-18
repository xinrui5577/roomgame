using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Request;
using Assets.Scripts.Game.Mahjong2D.Game.GameCtrl;
using Sfs2X.Entities.Data;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Mahjong2D.Game.Data
{
    public class UserData : UserInfo
    {
        public UserData(ISFSObject data)
        {
            IsStayInRoom = true;
            Parse(data);
        }

        /// <summary>
        /// 个性签名
        /// </summary>
        public string SignText;

        /// <summary>
        /// 胜利次数
        /// </summary>
        public int WinTimes;

        /// <summary>
        /// 失败次数
        /// </summary>
        public int LostTimes;

        /// <summary>
        /// 是否在游戏中
        /// </summary>
        public bool IsStayInRoom;
        /// <summary>
        /// 显示的索引（这个索引是对应玩家在当前玩家中显示的方向的索引，为了处理多人模式时方便）
        /// </summary>
        public int ShowDirectionIndex;
        /// <summary>
        /// 总次数
        /// </summary>
        public int TotalTimes;
        /// <summary>
        /// 加刚状态：-1：未加刚 1加刚 2一刚到底
        /// </summary>
        public int Piao;
        /// <summary>
        /// 现金（元宝）
        /// </summary>
        private int _cash = 0;
        /// <summary>
        /// 是否为房主
        /// </summary>
        public bool IsOwner;
        /// <summary>
        /// 现金
        /// </summary>
        public int Cash
        {
            get { return (int)decript(_cash); }
            set { _cash = (int)encript(value); }
        }

        /// <summary>
        /// 是否得到gps信息
        /// </summary>
        public bool IsHasGpsInfo { get; private set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public float GpsX { get; private set; }
        /// <summary>
        /// 经度
        /// </summary>
        public float GpsY { get; private set; }

        /// <summary>
        /// 是不是当前玩家
        /// </summary>
        public bool IsOther
        {
            get
            {
               return !Seat.Equals(App.GetGameManager<Mahjong2DGameManager>().SelfPlayer.UserSeat);
            }
        }
        /// <summary>
        /// 是否在线
        /// </summary>
        public bool IsOnLine;
        /// <summary>
        /// 是否有听状态
        /// </summary>
        public bool HasTing;
        /// <summary>
        /// 是否为托管状态
        /// </summary>
        public bool IsAuto;
        private void Parse(ISFSObject userData)
        {
            long ttGold;
            long gold;
            GameTools.TryGetValueWitheKey(userData, out isReady, RequestKey.KeyState);
            GameTools.TryGetValueWitheKey(userData, out Piao, RequestKey.KeyPiao);
            GameTools.TryGetValueWitheKey(userData, out WinTimes, RequestKey.KeyWinNum);
            GameTools.TryGetValueWitheKey(userData, out ttGold, RequestKey.KeyTotalGold);
            GameTools.TryGetValueWitheKey(userData, out gold, RequestKey.KeyGold);
            GameTools.TryGetValueWitheKey(userData, out SignText, RequestKey.KeySign);
            GameTools.TryGetValueWitheKey(userData, out TotalTimes, RequestKey.KeyTotal);
            GameTools.TryGetValueWitheKey(userData, out HeadImage, RequestKey.KeyAvatar);
            GameTools.TryGetValueWitheKey(userData,out name, RequestKey.KeyName);
            GameTools.TryGetValueWitheKey(userData, out id, RequestKey.KeyId);
            GameTools.TryGetValueWitheKey(userData, out IsOnLine, RequestKey.KeyIsOnLine);
            GameTools.TryGetValueWitheKey(userData, out level, RequestKey.KeyLevel);
            GameTools.TryGetValueWitheKey(userData, out Seat, RequestKey.KeySeat);
            GameTools.TryGetValueWitheKey(userData, out LostTimes, RequestKey.KeyLostNum);
            GameTools.TryGetValueWitheKey(userData, out Sex, RequestKey.KeySex);
            GameTools.TryGetValueWitheKey(userData, out ip, RequestKey.KeyIp);
            GameTools.TryGetValueWitheKey(userData, out IsAuto, RequestKey.KeyAuto);
            SetGpsData(userData);
            Gold = ttGold+gold;
            if (Sex!=ConstantData.SexMan)
            {
                Sex =ConstantData.SexWoMen;
            }
        }


        /// <summary>
        /// 设置gps信息
        /// </summary>
        /// <param name="userData"></param>
        public void SetGpsData(ISFSObject userData)
        {
            //获取gpsx; gpsy
            if ((userData.ContainsKey(RequestKey.KeyGpsX)&& userData.ContainsKey(RequestKey.KeyGpsY)) || (userData.ContainsKey("x") && userData.ContainsKey("y")))
            {
                GpsX = userData.ContainsKey(RequestKey.KeyGpsX) ? userData.GetFloat(RequestKey.KeyGpsX) : userData.GetFloat("x");
                GpsY = userData.ContainsKey(RequestKey.KeyGpsY) ? userData.GetFloat(RequestKey.KeyGpsY) : userData.GetFloat("y");
                IsHasGpsInfo = true;
            }
            else
            {
                GpsX = -1f;
                GpsY = -1f;
                IsHasGpsInfo = false;
            }
            GameTools.TryGetValueWitheKey(userData, out Country, RequestKey.Country);        
        }

        public string WinRate
        {
            get
            {
                int total = TotalTimes;
                int m = 0;
                if (total > 0)
                {
                    m = WinTimes * 10000 / total;
                }
                return (m / 100) + "%";
            }
        }
    }
}
