using UnityEngine;

namespace Assets.Scripts.Common.YxPlugins.Gps.Proxys
{
    /// <summary>
    /// 
    /// </summary>
    class AmapEvent : AndroidJavaProxy
    {
        /// <summary>
        /// 
        /// </summary>
        public AmapEvent(): base("com.amap.api.location.AMapLocationListener")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amapLocation"></param>
        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once InconsistentNaming
        void onLocationChanged(AndroidJavaObject amapLocation)
        {
            // ReSharper disable once UseNullPropagation
            if (LocationChanged != null)
            {
                LocationChanged(amapLocation);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amap"></param>
        public delegate void DelegateOnLocationChanged(AndroidJavaObject amap);
        /// <summary>
        /// 回调
        /// </summary>
        public event DelegateOnLocationChanged LocationChanged;
    }
}
