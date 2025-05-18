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
            Debug.Log("默认Gps");
            GpsInstance = new DefaultLocationManager { Loop = false };
        }
    }
}
