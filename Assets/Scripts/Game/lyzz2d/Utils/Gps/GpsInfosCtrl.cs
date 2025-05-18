using System;
using Assets.Scripts.Game.lyzz2d.Utils.Single;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.lyzz2d.Utils.Gps
{
    public class GpsInfosCtrl : MonoSingleton<GpsInfosCtrl>
    {
        private GameObject bg;

        public bool IsShow { get; private set; }

        public override void Awake()
        {
            base.Awake();
            bg = transform.GetChild(0).gameObject;
            bg.SetActive(false);
            IsShow = false;
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
            for (var i = 0; i < users.Length; i++)
            {
                var nextSeat = (i + 1)%users.Length;

                if (users[i].NextSeat.Equals(users[nextSeat].LocalSeat))
                {
                    var des = "";
                    if (users[i].IsGpsInfo && users[nextSeat].IsGpsInfo)
                    {
                        var distance =
                            (int) Distince(users[i].GpsX, users[i].GpsY, users[nextSeat].GpsX, users[nextSeat].GpsY);
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
                            des = string.Format("距离：{0} KM", distance/1000f);
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
            bg.SetActive(false);
            IsShow = false;
        }

        public void Show()
        {
            bg.SetActive(true);
            IsShow = true;
        }

        /// <summary>
        ///     根据Gps信息获得2个点之间距离
        /// </summary>
        /// <param name="a1">n</param>
        /// <param name="a2">e</param>
        /// <param name="b1">n</param>
        /// <param name="b2">e</param>
        /// <returns></returns>
        public static double Distince(float a1, float a2, float b1, float b2)
        {
            double R = 6371004;
            var PI_RM = 180/Math.PI;
            var C = 1 -
                    (Math.Pow(
                        Math.Sin((90 - a2)/PI_RM)*Math.Cos(a1/PI_RM) - Math.Sin((90 - b2)/PI_RM)*Math.Cos(b1/PI_RM), 2) +
                     Math.Pow(
                         Math.Sin((90 - a2)/PI_RM)*Math.Sin(a1/PI_RM) - Math.Sin((90 - b2)/PI_RM)*Math.Sin(b1/PI_RM), 2) +
                     Math.Pow(Math.Cos((90 - a2)/PI_RM) - Math.Cos((90 - b2)/PI_RM), 2))/2;
            return R*Math.Acos(C);
        }
    }
}