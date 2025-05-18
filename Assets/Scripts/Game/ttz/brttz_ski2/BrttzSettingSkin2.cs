using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.ttz
{
    public class BrttzSettingSkin2 : MonoBehaviour
    {
        public GameObject SoundSign;
        public GameObject MusicSign;

        public void OnSoundChange()
        {
            var musicMgr = Facade.Instance<MusicManager>();
            float value = SoundSign.activeSelf ? 1 : 0;
            musicMgr.EffectVolume = value;
            SoundSign.SetActive(!SoundSign.activeSelf);
        }

        public void OnMusicChange()
        {
            var musicMgr = Facade.Instance<MusicManager>();
            float value = MusicSign.activeSelf ? 1 : 0;
            musicMgr.MusicVolume = value;
            MusicSign.SetActive(!MusicSign.activeSelf);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            var musicMgr = Facade.Instance<MusicManager>();
            if (SoundSign != null) SoundSign.SetActive(musicMgr.EffectVolume <= 0);
            if (MusicSign != null) MusicSign.SetActive(musicMgr.MusicVolume <= 0);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}