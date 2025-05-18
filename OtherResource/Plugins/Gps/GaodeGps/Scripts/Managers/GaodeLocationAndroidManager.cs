using System;
using Assets.Scripts.Common.YxPlugins.Gps.Proxys;
using UnityEngine;
using YxPlugins.Enums;
using YxPlugins.Interfaces;
using YxPlugins.Structs;

namespace Assets.Scripts.Common.YxPlugins.Gps.Managers
{
    /// <summary>
    /// android高德
    /// author:defa
    /// </summary>
    public class GaodeLocationAndroidManager :IBaseGps
    {
        private AmapEvent _amap;
        private AndroidJavaClass _jcu;
        private AndroidJavaObject _jou;
        private AndroidJavaObject _mLocationClient;
        private AndroidJavaObject _mLocationOption;
        

        /// <inheritdoc />
        public YxELocationStatus Status { get; private set; }

        /// <inheritdoc />
        public bool Loop { get; set; }

        /// <summary>
        /// 事件
        /// </summary>
        public Action<int> LocationChangedAction { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GaodeLocationAndroidManager()
        {
            var info = LocationInfo;
            info.Longitude = double.NaN;
            info.Latitude = double.NaN;
            info.Address = string.Empty;
            LocationInfo = info;
        }

        /// <inheritdoc />
        public void Init()
        {
            StartLocation();
            Status = YxELocationStatus.Initializing;
        }

        /// <summary>
        /// 开始定位
        /// </summary>
        public void StartLocation()
        {
            try
            {
                _jcu = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                _jou = _jcu.GetStatic<AndroidJavaObject>("currentActivity");

                //初始化定位
                _mLocationClient = new AndroidJavaObject("com.amap.api.location.AMapLocationClient", _jou);

                //配置参数
                //初始化AMapLocationClientOption对象
                _mLocationOption = new AndroidJavaObject("com.amap.api.location.AMapLocationClientOption");
                
                // 启动定位
                //给定位客户端对象设置定位参数
                _mLocationClient.Call("setLocationOption", _mLocationOption);
                ////声明定位回调监听器
                StartLocationChanged();
                //设置定位回调监听
                _mLocationClient.Call("setLocationListener", _amap);
                //启动定位
                _mLocationClient.Call("startLocation");

            }
            catch (Exception e)
            { 
                Debug.Log(e);
                Stop();
            }
        }

        /// <summary>
        /// 返回定位信息
        /// </summary>
        private void StartLocationChanged()
        {
            //声明定位回调监听器
            _amap = new AmapEvent();
            _amap.LocationChanged += OnLocationChanged;
        }

        /// <inheritdoc />
        public void Start()
        {
            Status = YxELocationStatus.Started;
        }

        /// <inheritdoc />
        public void Stop()
        {
            if (_amap != null)
            {
                _amap.LocationChanged -= OnLocationChanged;
            }

            if (_mLocationClient != null)
            {
                _mLocationClient.Call("stopLocation");
                _mLocationClient.Call("onDestroy");
            }
            Status = YxELocationStatus.Stopped;
        }

        public YxLocationInfo LocationInfo { get; set; }

        private void OnLocationChanged(AndroidJavaObject amapLocation)
        {
            var code = -1;
            var info = LocationInfo;
            if (amapLocation != null)
            {
                code = amapLocation.Call<int>("getErrorCode");
                if (code == 0)
                {
                    try
                    {
                        //纬度
                        info.Latitude = amapLocation.Call<double>("getLatitude");
                        //经度
                        info.Longitude = amapLocation.Call<double>("getLongitude");
                        info.Address = amapLocation.Call<string>("getAddress");
                        LocationInfo = info;
                        Status = YxELocationStatus.Succeed;
                        LaunchLocationChangedAction(code);
                        return;
                    }
                    catch (Exception e)
                    {
                        Status = YxELocationStatus.Failed;
                    }
                    finally
                    {
                        if (!Loop)
                        {
                            Debug.LogError("----:" + Status);
                        }
                    }
                }
            }
            Status = YxELocationStatus.Failed;
            info.Latitude = double.NaN;
            info.Longitude = double.NaN;
            info.Address = "";
            LocationInfo = info;
            Debug.LogError("----:"+Status);
            LaunchLocationChangedAction(code);
        }

        private void LaunchLocationChangedAction(int code)
        {
            if (LocationChangedAction != null) LocationChangedAction(code);
        }

        public void Destroy()
        {
            Stop();
        }
    }
}
