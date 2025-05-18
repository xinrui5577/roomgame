using Assets.Scripts.Common.Windows;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.Salvo.Windows
{
    public class HelpWindow : YxNguiWindow
    {
        protected override void OnShow()
        {
            Facade.Instance<MusicManager>().Play("btn");
        }

        protected override void OnHide()
        {
            Facade.Instance<MusicManager>().Play("btn");
        }
    }
}
