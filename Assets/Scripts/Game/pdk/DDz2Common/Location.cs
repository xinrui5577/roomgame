using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.pdk.InheritCommon;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.pdk.DDz2Common
{
    public class Location : MonoBehaviour
    {

        public static void ClearSelf()
        {

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

            var apiInfo = new Dictionary<string, object>() { { "bundleID", Application.bundleIdentifier } };
            Facade.Instance<TwManager>().SendAction("gpsOpen", apiInfo, rs =>
            {
                var sharedata = (Dictionary<string, object>)rs;

                if (sharedata.ContainsKey("gpsOpen") && bool.Parse(sharedata["gpsOpen"].ToString()) )
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
                YxDebug.Log("Location status : " + Input.location.status);
                //成功
                if (Input.location.status != LocationServiceStatus.Failed)
                {
                   
                    ISFSObject iobj = new SFSObject();
                    iobj.PutFloat("x", Input.location.lastData.latitude);
                    iobj.PutFloat("y", Input.location.lastData.longitude);
                    IRequest request = new ExtensionRequest("locat", iobj);
                    GlobalData.ServInstance.SendRequest(request);
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

