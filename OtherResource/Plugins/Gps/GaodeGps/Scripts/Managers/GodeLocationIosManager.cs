using System;
using System.Runtime.InteropServices;
using System.Text;
using YxPlugins.Enums;
using YxPlugins.Interfaces;
using YxPlugins.Structs;

namespace Assets.Scripts.Common.YxPlugins.Gps.Managers
{
    /// <inheritdoc />
    /// <summary>
    /// 高德gps
    /// </summary>
    public class GodeLocationIosManager :IBaseGps
    {
        //初始化高德
        [DllImport("__Internal")]
        private static extern void _GaoDePlatformInit();

        //开始定位
        [DllImport("__Internal")]
        private static extern void _GaoDePlatformStart();

        //停止定位
        [DllImport("__Internal")]
        private static extern void _GaoDePlatformEnd();

        //纬度
        [DllImport("__Internal")]
        private static extern float _Latitude();

        //经度
        [DllImport("__Internal")]
        private static extern float _Longitude();

        //地址描述
        [DllImport("__Internal")]
        private static extern IntPtr _locationDespos(out int len);

        /// <summary>
        /// 
        /// </summary>
        public GodeLocationIosManager()
        {
            var info = LocationInfo;
            info.Longitude = double.NaN;
            info.Latitude = double.NaN;
            info.Address = string.Empty;
            LocationInfo = info;
        }

        /// <summary>
        /// 获取地址
        /// </summary>
        /// <returns></returns>
        public static string GetLocationDespos()
        {
            int strLen;
            var stringData = _locationDespos(out strLen);
            if (strLen <= 0) { return string.Empty;}
            var buffer = new byte[strLen];
            Marshal.Copy(stringData, buffer, 0, strLen);
            Marshal.FreeHGlobal(stringData);
            return Encoding.UTF8.GetString(buffer);
        }

        /// <inheritdoc />
        public YxELocationStatus Status { get; private set; }

        /// <inheritdoc />
        public bool Loop { get; set; }
         
        /// <inheritdoc />
        public Action<int> LocationChangedAction{get;set;}

        /// <inheritdoc />
        public void Init()
        {
            _GaoDePlatformInit();
            _GaoDePlatformStart();
            Status = YxELocationStatus.Initializing;
        }

        /// <inheritdoc />
        public void Start()
        {
            _GaoDePlatformStart();
            var info = LocationInfo;
            info.Longitude = _Longitude();
            info.Latitude = _Latitude();
            info.Address = GetLocationDespos();
            LocationInfo = info;
            if (!Loop)
            {
                Stop();
            }
            Status = YxELocationStatus.Succeed;
        }

        /// <inheritdoc />
        public void Stop()
        {
            _GaoDePlatformEnd();
            Status = YxELocationStatus.Stopped;
        }

        public YxLocationInfo LocationInfo { get; set; }

        public void Destroy()
        {
            Stop();
        }
    }
}
