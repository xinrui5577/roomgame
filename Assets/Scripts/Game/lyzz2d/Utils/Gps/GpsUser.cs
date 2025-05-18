using UnityEngine;

namespace Assets.Scripts.Game.lyzz2d.Utils.Gps
{
    public class GpsUser
    {
        /// <summary>
        ///  距离
        /// </summary>
        public UILabel DistanceLabel;

        /// <summary>
        ///  x轴
        /// </summary>
        public float GpsX;

        /// <summary>
        ///  y轴
        /// </summary>
        public float GpsY;

        /// <summary>
        ///  是否开启GPS
        /// </summary>
        public bool IsGpsInfo;

        /// <summary>
        ///  线
        /// </summary>
        public GameObject Line;

        /// <summary>
        ///  座位号
        /// </summary>
        public int LocalSeat;

        /// <summary>
        /// 下个位置座位
        /// </summary>
        public int NextSeat;

        public GpsUser(bool isGps, float gpsx, float gpsy, UILabel disLabel, GameObject line, int localSeat,
            int nextSeat)
        {
            IsGpsInfo = isGps;
            GpsX = gpsx;
            GpsY = gpsy;
            DistanceLabel = disLabel;
            Line = line;
            LocalSeat = localSeat;
            NextSeat = nextSeat;
        }
    }
}