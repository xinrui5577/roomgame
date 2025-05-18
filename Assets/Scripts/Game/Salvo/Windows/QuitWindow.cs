using System;
using Assets.Scripts.Common.Windows;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.Salvo.Windows
{
    public class QuitWindow : YxNguiWindow
    {
        public Action QuitFunction;

        protected override void OnShow()
        {
            Facade.Instance<MusicManager>().Play("btn");
        }

        protected override void OnHide()
        {
            Facade.Instance<MusicManager>().Play("btn");
        }

        public void OnQuitBtn()
        {
            Facade.Instance<MusicManager>().Play("btn");
            if (QuitFunction == null) return;
            QuitFunction();
        }
    }
}
