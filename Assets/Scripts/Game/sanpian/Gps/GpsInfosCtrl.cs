using System;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.sanpian.Gps
{
    public class GpsInfosCtrl : MonoBehaviour
    {
        //private GameObject bg;
        void Awake()
        {
            //gameObject.SetActive(false);
            //bg = transform.GetChild(0).gameObject;
            //bg.SetActive(false);
        }

        /// <summary>
        /// 显示玩家之间距离信息和地址信息
        /// </summary>
        public void ShowDistince(GpsUser[] users)
        {
            YxDebug.Log("GSP User 数量是：" + users.Length);
            if (users.Length == 1)
            {
                return;
            }
            for (int i = 0; i < users.Length; i++)
            {
                int nextSeat = (i + 1) % users.Length;

                if (users[i].NextSeat.Equals(users[nextSeat].LocalSeat))
                {
                    string des = "";
                    if (users[i].IsGpsInfo && users[nextSeat].IsGpsInfo)
                    {
                        int distance = (int)Distince(users[i].GpsX, users[i].GpsY, users[nextSeat].GpsX, users[nextSeat].GpsY);
                        if (distance < 1000)
                        {
                            if (distance < 100)
                            {
                                des = "<=100m";
                            }
                            else
                            {
                                des = string.Format("距离：{0}M", distance);
                            }

                        }
                        else
                        {
                            des = string.Format("距离：{0} KM", distance / 1000f);
                        }
                        users[i].Line.SetActive(true);
                    }
                    users[i].DistanceLabel.text = des;

                }
                else
                {
                    users[i].Line.SetActive(false);
                }
            }

        }

        public void Hide()
        {
            gameObject.SetActive(false);
            //bg.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            //bg.SetActive(true);
        }

        /// <summary>
        /// 根据Gps信息获得2个点之间距离
        /// </summary>
        /// <param name="a1">n</param>
        /// <param name="a2">e</param>
        /// <param name="b1">n</param>
        /// <param name="b2">e</param>
        /// <returns></returns>
        public static double Distince(float a1, float a2, float b1, float b2)
        {
            double R = 6371004;
            double PI_RM = 180 / Math.PI;
            double C = 1 - (Math.Pow((Math.Sin((90 - a2) / PI_RM) * Math.Cos(a1 / PI_RM) - Math.Sin((90 - b2) / PI_RM) * Math.Cos(b1 / PI_RM)), 2) + Math.Pow((Math.Sin((90 - a2) / PI_RM) * Math.Sin(a1 / PI_RM) - Math.Sin((90 - b2) / PI_RM) * Math.Sin(b1 / PI_RM)), 2) + Math.Pow((Math.Cos((90 - a2) / PI_RM) - Math.Cos((90 - b2) / PI_RM)), 2)) / 2;
            return R * Math.Acos(C);
        }
    }

    public class GpsUser
    {
        /// <summary>
        /// 纬度
        /// </summary>
        public float GpsX;
        /// <summary>
        /// 经度
        /// </summary>
        public float GpsY;
        /// <summary>
        /// 距离显示
        /// </summary>
        public UILabel DistanceLabel;
        /// <summary>
        /// 线
        /// </summary>
        public GameObject Line;
        /// <summary>
        /// 是否得到了Gps信息
        /// </summary>
        public bool IsGpsInfo;
        /// <summary>
        /// 玩家当前的位置seat
        /// </summary>
        public int LocalSeat;
        /// <summary>
        /// 下一个玩家的座位
        /// </summary>
        public int NextSeat;

        public GpsUser(bool isGps, float gpsx, float gpsy, UILabel disLabel, GameObject line, int localSeat, int nextSeat)
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
