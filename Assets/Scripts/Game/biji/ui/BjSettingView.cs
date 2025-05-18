using Assets.Scripts.Game.biji.EventII;
using UnityEngine;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.biji.ui
{
    public class BjSettingView : MonoBehaviour
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
