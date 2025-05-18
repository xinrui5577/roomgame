using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Sfs2X.Entities.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI.GPS
{
    public class GpsUser : MonoBehaviour
    {
        public bool AllowActive
        {
            get { return isHasGpsInfo && IsShowHead; }
        }

        /// <summary>
        /// 是否得到gps信息
        /// </summary>
        public bool isHasGpsInfo { get; set; }

        //头像处于激活状态
        public bool IsShowHead { get; set; }

        public string Country { get; set; }

        public string Ip { get; set; }

        public Vector2 Position { get; set; }

        public Transform Address;

        public Transform Line;

        public void Init(int localSeat, int nextSeat)
        {
            LocalSeat = localSeat;
            NextSeat = nextSeat;
        }

        /// <summary>
        /// 玩家当前的位置seat
        /// </summary>
        public int LocalSeat { get; set; }
        /// <summary>
        /// 下一个玩家的座位
        /// </summary>
        public int NextSeat { get; set; }

        /// <summary>
        /// 设置gps信息
        /// </summary>
        /// <param name="userData"></param>
        public void SetGpsData(ISFSObject userData, string ip)
        {
            //获取gpsx; gpsy
            if ((userData.ContainsKey("x") && userData.ContainsKey("y")))
            {
                float GpsX = userData.GetFloat("x");
                float GpsY = userData.GetFloat("y");
                Position = new Vector2(GpsX, GpsY);
                isHasGpsInfo = true;
            }
            else
            {
                Position = new Vector2(0, 0);
                isHasGpsInfo = false;
            }

            if (userData.ContainsKey(RequestKeyOther.Country))
                Country = userData.GetUtfString(RequestKeyOther.Country);

            if (!string.IsNullOrEmpty(ip))
                Ip = ip;
        }

        public void Hide()
        {
            if (Address != null)
                Address.gameObject.SetActive(false);

            if (Line != null)
                Line.gameObject.SetActive(false);
        }

        public void ShowAddressInfo()
        {
            if (Address == null) return;

            Address.gameObject.SetActive(true);
            Text t = Address.FindChild("address").GetComponent<Text>();

            if (t != null)
            {
                t.text = string.Format("IP:{0}\n所在地:{1}", !string.IsNullOrEmpty(Ip) ? Ip : "未知", isHasGpsInfo ? Country : "未提供位置信息\n请开启位置服务,并给予应用相应权限");

                RectTransform tRect = t.transform as RectTransform;
                //消息框自适应
                if (tRect.sizeDelta.y > 125)
                {
                    RectTransform aRect = Address as RectTransform;
                    aRect.sizeDelta = new Vector2(aRect.sizeDelta.x, tRect.sizeDelta.y + 20f);
                }
            }
        }

        public void ShowLine(string str)
        {
            if (Line == null) return;

            Text t = Line.FindChild("Text").GetComponent<Text>();
            t.text = str;
        }

    }
}