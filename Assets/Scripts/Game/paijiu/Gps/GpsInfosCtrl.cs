using System;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using System.Collections.Generic;
using YxFramwork.Common;

namespace Assets.Scripts.Game.paijiu.Gps
{
    public class GpsInfosCtrl : MonoBehaviour
    {
        protected void Awake()
        {
            _isShow = false;
        }

        private bool _isShow;
        public bool IsShow
        {
            get
            {
                return _isShow;
            }
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
                    string des = string.Empty;
                    if (users[i].IsGpsInfo && users[nextSeat].IsGpsInfo)
                    {
                        int distance = (int)GetDistince(users[i].GpsX, users[i].GpsY, users[nextSeat].GpsX, users[nextSeat].GpsY);
                        if (distance < 1000)
                        {
                            des = distance < 100 ? "<=100m" : string.Format("距离：{0}M", distance);
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



        public List<GameObject> LineList = new List<GameObject>();
        public List<GameObject> PointList = new List<GameObject>();
        private readonly List<UILabel> _desLabelList = new List<UILabel>();


        /// <summary>
        /// 人数大于3个人时,通过玩家名字进行处理
        /// </summary>
        public void ShowLine(PaiJiuPlayer panel)
        {
            if (!panel.HasGpsInfo)
                return;
            int index = 0;
            List<int> indexList = new List<int>();

            var gdata = App.GetGameData<PaiJiuGameData>();
            var panels = gdata.PlayerList;
            //显示红点
            for (int i = 0; i < panels.Length; i++)
            {
                if (panels[i] == null)
                    continue;
                var player = (PaiJiuPlayer)panels[i];
                if (panels[i].Equals(panel))
                {
                    index = i;
                    string format = string.Format("Point{0}", index);

                    PointList.Find(obj => obj.name.Equals(format)).SetActive(true);
                }
                else if (!panels[i].Equals(gdata.GetPlayer<PaiJiuPlayerSelf>()) && panels[i].gameObject.activeSelf && player.HasGpsInfo)
                {
                    if (i == 0)
                        continue;
                    indexList.Add(i);
                    string format = string.Format("Point{0}", i);

                    PointList.Find(obj => obj.name.Equals(format)).SetActive(true);
                }
            }

            if (indexList.Count > 0)
            {
                foreach (int ind in indexList)
                {
                    foreach (GameObject line in LineList)
                    {
                        bool active = line.activeSelf || (line.name.Contains(index.ToString()) && line.name.Contains(ind.ToString()));
                        line.SetActive(active);
                        if (active)
                        {
                            UILabel desLabel = line.transform.GetChild(1).GetComponent<UILabel>() ??
                                               line.transform.Find("Label").GetComponent<UILabel>();
                            if (desLabel == null || desLabel.gameObject.activeSelf)
                            {
                            }
                            else
                            {
                                var p1 = (PaiJiuPlayer)panels[index];
                                var p2 = (PaiJiuPlayer)panels[ind];

                                if (p1.HasGpsInfo && p2.HasGpsInfo)
                                {
                                    int distance = (int)GetDistince(p1.GpsX, p1.GpsY, p2.GpsX, p2.GpsY);
                                    string des;
                                    if (distance < 1000)
                                    {
                                        des = distance < 100 ? "间距小于100m" : string.Format("间距 : {0}m", distance);
                                    }
                                    else
                                    {
                                        des = string.Format("间距 : {0}Km", distance / 1000);
                                    }
                                    desLabel.text = des;
                                    desLabel.gameObject.SetActive(true);
                                    _desLabelList.Add(desLabel);
                                }
                            }
                        }
                    }
                }
            }

            indexList.Clear();
        }

        public void HideLineAndPoint()
        {
            //隐藏玩家间的连线
            foreach (var line in LineList)
            {
                line.SetActive(false);
            }
            //隐藏玩家上显示的圆点图片
            foreach (var point in PointList)
            {
                point.SetActive(false);
            }
            //隐藏显示距离的数据
            foreach (UILabel desLabel in _desLabelList)
            {
                desLabel.gameObject.SetActive(false);
            }
        }

        public void Hide()
        {
            //bg.SetActive(false);
            _isShow = false;
        }

        public void Show()
        {
            //bg.SetActive(true);
            _isShow = true;
        }

        /// <summary>
        /// 根据Gps信息获得2个点之间距离
        /// </summary>
        /// <param name="a1">n</param>
        /// <param name="a2">e</param>
        /// <param name="b1">n</param>
        /// <param name="b2">e</param>
        /// <returns></returns>
        public static double GetDistince(float a1, float a2, float b1, float b2)
        {
            const double r = 6371004;
            const double piRm = 180 / Math.PI;
            var c = 1 - (Math.Pow((Math.Sin((90 - a2) / piRm) * Math.Cos(a1 / piRm) - Math.Sin((90 - b2) / piRm) * Math.Cos(b1 / piRm)), 2)
                + Math.Pow((Math.Sin((90 - a2) / piRm) * Math.Sin(a1 / piRm) - Math.Sin((90 - b2) / piRm) * Math.Sin(b1 / piRm)), 2)
                + Math.Pow((Math.Cos((90 - a2) / piRm) - Math.Cos((90 - b2) / piRm)), 2)) / 2;
            return r * Math.Acos(c);
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
