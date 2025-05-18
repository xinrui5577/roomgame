using Assets.Scripts.Common.Interface;
using Assets.Scripts.Common.YxPlugins;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Interfaces;
using YxPlugins.Interfaces;

namespace Assets.Scripts.Common
{
    public class ReplayGameEnter : YxBaseReplayGameEnter
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
    }
}
