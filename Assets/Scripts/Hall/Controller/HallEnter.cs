using Assets.Scripts.Common;
using Assets.Scripts.Common.Interface;
using Assets.Scripts.Common.YxPlugins;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Interfaces;
using YxFramwork.Tool;
using YxPlugins.Interfaces;

namespace Assets.Scripts.Hall.Controller
{
    internal class HallEnter : YxBaseHallEnter
    {
        protected override ISysCfg GetSystemCfg()
        {
            return new SysConfig(IsDeve);
        }

        protected override IUI GetUIImpl(Vector2 screenSize)
        {
            return new UIImpl(screenSize);
        }

        protected override IEventHandle GetTwMessageEventHandle()
        {
            return new TwMessageEventHandle();
        }

        protected override IBasePluginsFactory GetPluginsFactory()
        {
            return new YxPluginsFactory();
        }
         
        protected override void OnAwake()
        {
            base.OnAwake();
            var root = GetComponent<UIRoot>();
            root.manualWidth = (int)ScreenSize.x;
            root.manualHeight = (int)ScreenSize.y;
        }
    }
}
