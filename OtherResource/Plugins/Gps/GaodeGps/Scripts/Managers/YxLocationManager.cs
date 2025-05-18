using UnityEngine;
using YxPlugins.Managers;

namespace Assets.Scripts.Common.YxPlugins.Gps.Managers
{
    /// <summary>
    /// 
    /// </summary>
    public class YxLocationManager : YxBaseLocationManager
    {
        public YxLocationManager()
        {
            Debug.Log("高德Gps");
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    GpsInstance = new GaodeLocationAndroidManager { Loop = false };
                    return;
                case RuntimePlatform.IPhonePlayer:
                    GpsInstance = new GodeLocationIosManager { Loop = false };
                    return;
                default:
                    GpsInstance = new DefaultLocationManager { Loop = false };
                    return;
            }
        }
    }
}
