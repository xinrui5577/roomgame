using Assets.Scripts.Game.sanpian.DataStore;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.sanpian.CommonCode.ChessCommon {
    public class SoundSetting :MonoBehaviour
    {
        public const string BGMusicVolume = "chess_bgmusicVolume";
        public const string AudioVolume = "chess_audioVolume";
        public UISlider BgMusicBar;
        public UISlider AudioBar;

        private float _bgMusicValue;
        private float _soundValue;

        public void Start() {
            _bgMusicValue = PlayerPrefs.GetFloat(BGMusicVolume, 0.5f);
            if (BgMusicBar!=null)
            {
                BgMusicBar.value = _bgMusicValue;
            }
            if (BgMusicToggle != null) {
                BgMusicToggle.value = _bgMusicValue > 0.5;
            }
            _soundValue = PlayerPrefs.GetFloat(AudioVolume, 0.1f);
            if (AudioBar != null) {
                AudioBar.value = _soundValue;
            }
            if (SoundToggle != null) {
                SoundToggle.value = _soundValue > 0.1;
            }
        }

        public void OnBgMusicVolumeChange() {
            _bgMusicValue = BgMusicBar.value;
            var gmsg = App.GetGameData<SanPianGameData>().GMessage;
            if (gmsg.OnBgMusicVolumeChange != null)
            {
                gmsg.OnBgMusicVolumeChange(BgMusicBar.value);
            }
        }

        public void OnAudioVolumeChange() {
            _soundValue = AudioBar.value;
            var gmsg = App.GetGameData<SanPianGameData>().GMessage;
            if (gmsg.OnAudioVolumeChange != null)
            {
                gmsg.OnAudioVolumeChange(AudioBar.value);
            }
        }

        public void Close() {
            PlayerPrefs.SetFloat(AudioVolume, _soundValue);
            PlayerPrefs.SetFloat(BGMusicVolume, _bgMusicValue);
            PlayerPrefs.Save();
            gameObject.SetActive(false);
        }

        public UIToggle BgMusicToggle;
        public UIToggle SoundToggle;
        public void ToggleMusic() {
            _bgMusicValue = BgMusicToggle.value ? 1 : 0;
            var gmsg = App.GetGameData<SanPianGameData>().GMessage;
            if (gmsg.OnBgMusicVolumeChange != null)
            {
                Debug.Log("send mesg");
                gmsg.OnBgMusicVolumeChange(_bgMusicValue);
            }
        }

        public void ToggleSound() {
            _soundValue = SoundToggle.value ? 1 : 0;
            var gmsg = App.GetGameData<SanPianGameData>().GMessage;
            if (gmsg.OnAudioVolumeChange != null)
            {
                gmsg.OnAudioVolumeChange(_soundValue);
            }
        }
    }
}
