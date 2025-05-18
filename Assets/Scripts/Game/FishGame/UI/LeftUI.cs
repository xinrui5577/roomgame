using Assets.Scripts.Game.FishGame.Common.Managers;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.FishGame.UI
{
    public class LeftUI : UIView
    {

        public void OnSetting()
        {
            YxWindowManager.OpenWindow("SettingWindow");
        }

        public void OnReturn()
        {
            GameMain.OnQuitGame();
        }

        public void OnHelp()
        {
            YxWindowManager.OpenWindow("HelpWindow");
        }
    }
}
