using Assets.Scripts.Game.jlgame.EventII;
using UnityEngine;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jlgame.ui
{
    public class JlGameSettingView : MonoBehaviour
    {
        public EventObject EventObj;

        public void OnRecieve(EventData data)
        {
            switch (data.Name)
            {
                case "Open":
                    OpenSetting(data.Data);
                    break;
            }
        }

        public void OpenSetting(object data)
        {
            YxWindowManager.OpenWindow("SettingWindow");
        }
    }
}
