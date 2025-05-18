using Assets.Scripts.Hall.View.AboutRoomWindows;
using YxFramwork.Common;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.Panels
{
    /// <summary>
    /// 创建模式
    /// </summary>
    public class CreateRoomModelPanel : YxBasePanel
    { 
        public void OnOpenCreateRoomWindow(string winName)
        {
            var win = CreateOhterWindowWithT<CreateRoomWindow>(winName);
            if (win == null) { return;}
            win.GameKey = App.LoadingGameKey;
            win.IsDesignated = true;
        }
    }
}
