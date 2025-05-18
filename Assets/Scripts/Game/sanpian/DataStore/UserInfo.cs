using System;
using Assets.Scripts.Game.sanpian.CommonCode.SrcGameBase.DataObj;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.sanpian.DataStore {
    /// <summary>
    /// 用户信息
    /// </summary>
    [Serializable]
    public class UserInfo :BaseObj
    {
        public string Name;
        public int Id;
        /// <summary>
        /// 等级
        /// </summary>
        public int Level;
        /// <summary>
        /// 头像
        /// </summary>
        public string HeadImage = "HS_9";
        /// <summary>
        /// 性别
        /// </summary>
        public short Sex;
        /// <summary>
        /// 座位
        /// </summary>
        public int Seat;
        /// <summary>
        /// 钱币
        /// </summary>
        private long _coin;

        /// <summary>
        /// 纸牌数----断线重连
        /// </summary>
        public int cardLen;

        /// <summary>
        /// 本家手牌
        /// </summary>
        public int[] cards;

        /// <summary>
        /// 组ID
        /// </summary>
        public int teamId;

        /// <summary>
        /// 已出的三片数量
        /// </summary>
        public int pianNum;


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
        /// <summary>
        /// 片分数
        /// </summary>
        public int PianScore { get; set; }
        public bool IsReady;
        /// <summary>
        /// 地里位置
        /// </summary>
        public string Country;
        /// <summary>
        /// Ip地址
        /// </summary>
        public string Ip;
        /// <summary>
        /// 纬度
        /// </summary>
        public float GpsX { get; private set; }
        /// <summary>
        /// 经度
        /// </summary>
        public float GpsY { get; private set; }
        /// <summary>
        /// 是否得到gps信息
        /// </summary>
        public bool IsHasGpsInfo { get; private set; }
        /// <summary>
        /// 是否在线
        /// </summary>
        public bool IsOnline;


        /// <summary>
        /// 队友手牌
        /// </summary>
        public int[] mateCards;

        /// <summary>
        /// 设置gps信息
        /// </summary>
        /// <param name="userData"></param>
        public void SetGpsData(ISFSObject userData)
        {
            if (userData.ContainsKey("ip"))
                Ip = userData.GetUtfString("ip");

            if (userData.ContainsKey("country"))
                Country = userData.GetUtfString("country");

            //获取gpsx; gpsy
            if ((userData.ContainsKey("gpsx") && userData.ContainsKey("gpsy")) || (userData.ContainsKey("x") && userData.ContainsKey("y")))
            {
                GpsX = userData.ContainsKey("gpsx") ? userData.GetFloat("gpsx") : userData.GetFloat("x");
                GpsY = userData.ContainsKey("gpsy") ? userData.GetFloat("gpsy") : userData.GetFloat("y");
                IsHasGpsInfo = true;
            }
            else
            {
                GpsX = -1f;
                GpsY = -1f;
                IsHasGpsInfo = false;
            }
        }
    }
}
