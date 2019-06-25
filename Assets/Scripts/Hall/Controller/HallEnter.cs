using Assets.Scripts.Common;
using Assets.Scripts.Common.Interface;
using YxFramwork.Common;
using YxFramwork.Common.Interface;

namespace Assets.Scripts.Hall.Controller
{
    internal class HallEnter : EnterHall
    {
        protected override ISysCfg GetSystemCfg()
        {
            return new SysConfig();
        }

        protected override IUI GetUIImpl()
        {
            return new NguiImpl();
        } 
    }
}
