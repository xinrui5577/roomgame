using UnityEngine;


namespace Assets.Scripts.Game.LXGameScripts
{
    public class GameSettingWindow : PopWindowBase
    {
        private UISlider Sound;

        private UISlider Effect;
        private GameObject Close;

        private string path = "SettingBG/Set_bg/";
        void Awake()
        {
            Sound = transform.FindChild(path + "Music/Change_Music").GetComponent<UISlider>();
            Effect = transform.FindChild(path + "Effect/Change_Effect").GetComponent<UISlider>();
            Close = transform.FindChild("SettingBG/Close").gameObject;
        }

        void Start()
        {
            UIEventListener.Get(Close).onClick = OnHideClick;
            float soundVolum = PlayerPrefs.GetFloat("SoundVolume", 1);
            float effectVolum = PlayerPrefs.GetFloat("EffectVolume", 1);
            Sound.value = soundVolum;
            Effect.value = effectVolum;
        }

        public void OnSoundChange()
        {
            SoundManager.Instance.SoundVolume = Sound.value;
        }

        public void OnEffectChange()
        {
            SoundManager.Instance.EffectVolume = Effect.value;
        }

        private void OnHideClick(GameObject go)
        {
            Hide();
        }

        public void OnExistGameTotHall()
        {
            Hide();
            EventDispatch.Dispatch((int)EventID.GameEventId.OnCloseBtnClick);
        }
    }
}