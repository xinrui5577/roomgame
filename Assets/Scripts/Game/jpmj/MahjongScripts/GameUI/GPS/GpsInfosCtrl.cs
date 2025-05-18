using UnityEngine;
using System.Collections;
using System;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using Sfs2X.Requests;
using YxFramwork.Common;
using System.Collections.Generic;
using UnityEngine.UI;
using YxFramwork.Manager;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI.GPS
{
    public class GpsInfosCtrl : MonoBehaviour
    {
        public GpsUser[] Users = new GpsUser[UtilDef.GamePlayerCnt];

        private GameObject bg;

        void Awake()
        {
            bg = transform.GetChild(0).gameObject;
            bg.SetActive(false);
            _isShow = false;
        }

        private bool _isShow;
        public bool IsShow { get { return _isShow; } }

        public void SetGpsUserData(ISFSObject data, string ip, int chair)
        {
            if (chair > Users.Length) return;
            Users[chair].SetGpsData(data, ip);
        }

        public virtual void SetIpAndCountry(string ip,string country, int chair)
        {
            if (chair > Users.Length) return;
            Users[chair].Ip = ip;
            Text t = Users[chair].Address.FindChild("address").GetComponent<Text>();
            if (string.IsNullOrEmpty(t.text))
            {
                t.text = country; 
            }
        }

        public void Hide()
        {
            for (int i = 0; i < Users.Length; i++) Users[i].Hide();
            bg.SetActive(false);
            _isShow = false;
        }

        public void Show()
        {
            YxDebug.LogError(" ----- --------------- Show");

            bg.SetActive(true);
            _isShow = true;
        }

        public virtual void ShowLine()
        {
            for (int i = 0; i < Users.Length; i++)
            {
             

                string des = "";
                int nextSeat = Users[i].NextSeat;

                if (Users[i].AllowActive && Users[nextSeat].AllowActive)
                {
                    int distance = (int)Distince(Users[i].Position.x, Users[i].Position.y, Users[nextSeat].Position.x, Users[nextSeat].Position.y);

                    if (distance < 1000)
                    {
                        if (distance < 100)
                            des = "距离<=100m";
                        else
                            des = string.Format("距离：{0}M", distance);
                    }
                    else
                        des = string.Format("距离：{0} KM", distance / 1000f);

                    if (Users[i].Line != null)
                    {
                        Users[i].Line.gameObject.SetActive(true);
                        Users[i].ShowLine(des);
                    }
                }
            }
        }

        /// <summary>
        /// 根据Gps信息获得2个点之间距离
        /// </summary>
        /// <param name="a1">n</param>
        /// <param name="a2">e</param>
        /// <param name="b1">n</param>
        /// <param name="b2">e</param>
        /// <returns></returns>
        public double Distince(float a1, float a2, float b1, float b2)
        {
            double R = 6371004;
            double PI_RM = 180 / Math.PI;
            double C = 1 - (Math.Pow((
                Math.Sin((90 - a2) / PI_RM) * Math.Cos(a1 / PI_RM) -
                Math.Sin((90 - b2) / PI_RM) * Math.Cos(b1 / PI_RM)), 2) +
                Math.Pow((Math.Sin((90 - a2) / PI_RM) * Math.Sin(a1 / PI_RM) -
                Math.Sin((90 - b2) / PI_RM) * Math.Sin(b1 / PI_RM)), 2) +
                Math.Pow((Math.Cos((90 - a2) / PI_RM) -
                Math.Cos((90 - b2) / PI_RM)), 2)) / 2;

            return R * Math.Acos(C);
        }

        public bool IsNeedGps { get; private set; }

        void Start()
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                CheckGpsNeed();
            }
        }

        /// <summary>
        /// 检查是否需要Gps
        /// </summary>
        public void CheckGpsNeed()
        {
            StartCoroutine(StartCheckGpsNeed());
        }

        private IEnumerator StartCheckGpsNeed()
        {
            while (Facade.Instance<TwManager>() == null)
            {
                yield return new WaitForSeconds(3f);
            }

#if UNITY_5_6
            var apiInfo = new Dictionary<string, object>() { { "bundleID", Application.identifier } };
#else
            var apiInfo = new Dictionary<string, object>() { { "bundleID", Application.bundleIdentifier } };
#endif
            Facade.Instance<TwManager>().SendAction("gpsOpen", apiInfo, rs =>
            {
                var sharedata = (Dictionary<string, object>)rs;

                if (sharedata.ContainsKey("gpsOpen") && bool.Parse(sharedata["gpsOpen"].ToString()))
                {
                    StartCoroutine(CheckSmartAndSendMsg());
                    IsNeedGps = true;
                }
                else
                {
                    IsNeedGps = false;
                }
            });
        }

        public void OnApplicationPause(bool isPause)
        {
            if (!isPause && IsNeedGps)
            {
                StartCoroutine(CheckSmartAndSendMsg());
            }
        }

        IEnumerator CheckSmartAndSendMsg()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(3f);

            while (!App.HasLogin)
            {
                YxDebug.Log("游戏没有登录!");
                yield return new WaitForSeconds(5f);
            }

            StartTestGps();
        }

        public void StartTestGps()
        {
            StopAllCoroutines();
            StartCoroutine(CornameStGps);
        }

        void StopGPS()
        {
            Input.location.Stop();
        }

        private const string CornameStGps = "StartGPS";

        IEnumerator StartGPS()
        {
            YxDebug.Log("开始检测位置信息!");
            // LocationService.Start() 启动位置服务的更新,最后一个位置坐标会被使用
            Input.location.Start(10.0f, 10.0f);

            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                // 暂停协同程序的执行(1秒)
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            //超时
            if (maxWait < 1 && Input.location.status == LocationServiceStatus.Initializing)
            {
                YxDebug.LogError("超时不能发送gps");
                StopGPS();
                yield return new WaitForSeconds(10f);
                StartCoroutine(CornameStGps);
            }
            else
            {
                YxDebug.LogError("Location status : " + Input.location.status);

                //成功
                if (Input.location.status != LocationServiceStatus.Failed)
                {
                    ISFSObject iobj = new SFSObject();
                    iobj.PutFloat("x", Input.location.lastData.latitude);
                    iobj.PutFloat("y", Input.location.lastData.longitude);
                    IRequest request = new ExtensionRequest("locat", iobj);
                    App.GetRServer<NetWorkManager>().SendRequest(request);
                    StopGPS();

                    YxDebug.LogError("成功发送gps消息");
                    yield return new WaitForSeconds(10f);
                    StartCoroutine(CornameStGps);
                }
                //失败
                else
                {
                    YxDebug.LogError("发送gps消息失败");
                    StopGPS();
                    yield return new WaitForSeconds(10f);
                    StartCoroutine(CornameStGps);
                }
            }
        }

        /// <summary>
        /// 获得gps信息
        /// </summary>
        /// <param name="param"></param>
        public void OnGetGpsInfo(ISFSObject param)
        {
            YxDebug.LogError("OnGetGpsInfo:" + param.GetDump());
            StopCoroutine(CornameStGps);
            StartCoroutine(AfterGetGpsInfo());
        }

        /// <summary>
        /// 如果成功发送并得到数据 则300秒后再发送gps请求
        /// </summary>
        /// <returns></returns>
        private IEnumerator AfterGetGpsInfo()
        {
            yield return new WaitForSeconds(300f);
            StartTestGps();
        }
    }
}