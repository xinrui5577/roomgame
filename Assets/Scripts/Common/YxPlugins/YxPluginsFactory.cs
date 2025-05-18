using System.Collections.Generic;
using Assets.Scripts.Common.YxPlugins.Gps.Managers;
using Assets.Scripts.Common.YxPlugins.Social.Managers;
using UnityEngine;
using YxPlugins.Interfaces;
using YxPlugins.Managers;

namespace Assets.Scripts.Common.YxPlugins
{
    public class YxPluginsFactory : IBasePluginsFactory
    {
        private readonly Dictionary<string,IBasePlugins> _pluginMgrs = new Dictionary<string, IBasePlugins>();
        public YxPluginsFactory()
        {
            InitPlugins();
        }

        private void InitPlugins()
        {
            AddPlugin<ZySocialManager>("ZySocialManager");
        }

        private void AddPlugin<T>(string mgrName) where T : IBasePlugins, new()
        {
            var mgr = new T();
            _pluginMgrs[mgrName] = mgr;
            mgr.Init();
            mgr.Reset();
        }

        /// <summary>
        /// 获取gps定位插件
        /// </summary>
        /// <returns></returns>
        public YxBaseLocationManager CreateGps()
        {
            return new YxLocationManager();
        }

        public void Rest()
        {
            foreach (var kv in _pluginMgrs)
            {
                var mgr = kv.Value;
                if (mgr != null)
                {
                    mgr.Reset();
                }
            }
        }
    }
}
