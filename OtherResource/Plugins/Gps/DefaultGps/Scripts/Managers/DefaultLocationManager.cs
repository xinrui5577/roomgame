using System;
using UnityEngine;
using YxPlugins.Enums;
using YxPlugins.Interfaces;
using YxPlugins.Structs;

namespace Assets.Scripts.Common.YxPlugins.Gps.Managers
{
    /// <summary>
    /// 默认位置管理者
    /// </summary>
    internal class DefaultLocationManager : IBaseGps
    {
        /// <inheritdoc />
        public YxELocationStatus Status {
            get
            {
                var status = Input.location.status;
                switch (status)
                {
                    case LocationServiceStatus.Initializing:
                        return YxELocationStatus.Initializing;
                    case LocationServiceStatus.Stopped:
                        return YxELocationStatus.Stopped;
                    case LocationServiceStatus.Running:
                        return YxELocationStatus.Succeed;
                    case LocationServiceStatus.Failed:
                        return YxELocationStatus.Failed;
                }
                return YxELocationStatus.Waitting;
            }
        }

        /// <inheritdoc />
        public bool Loop { get; set; }
         
        /// <inheritdoc />
        public Action<int> LocationChangedAction { get; set; }

        /// <inheritdoc />
        public void Init()
        {
            Input.location.Start();
        }

        /// <inheritdoc />
        public void Start()
        {
        }

        /// <inheritdoc />
        public void Stop()
        {
            Input.location.Stop();
        }

        private YxLocationInfo _locationInfo;
        public YxLocationInfo LocationInfo {
            get
            {
                var location = Input.location;
                var lastData = location.lastData;
                var info = _locationInfo;
                info.Latitude = lastData.latitude;
                info.Longitude = lastData.longitude;
                info.Address = "";
                _locationInfo = info;
                return info;
            }
            set { _locationInfo = value; }
        }
    }
}
