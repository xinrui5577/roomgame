using Assets.Scripts.Game.pdk.InheritCommon;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.pdk.DDz2Common
{
    public class SwitchVoiceChatListener : MonoBehaviour
    {
        /// <summary>
        /// 显示mic的那个ui
        /// </summary>
        public GameObject ShowVoiceMicUiGob;

        // Use this for initialization
        void Start ()
        {
            App.GetGameData<GlobalData>().OnPlayerVoiceChatSteChange = OnPlayerVoiceChatSteChange;

            if (ShowVoiceMicUiGob != null) ShowVoiceMicUiGob.SetActive(App.GetGameData<GlobalData>().IsPlayVoiceChat);
        }

        /// <summary>
        /// 监听是否关闭声音开关
        /// </summary>
        /// <param name="isChange"></param>
        private void OnPlayerVoiceChatSteChange(bool isChange)
        {
            ShowVoiceMicUiGob.SetActive(isChange);
        }


    }
}
