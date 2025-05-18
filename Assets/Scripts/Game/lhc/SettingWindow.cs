using Assets.Scripts.Common.Windows;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.lhc
{
    public class SettingWindow : YxNguiWindow
    {

        void Start()
        {
            MusicSlider.value = Facade.Instance<MusicManager>().MusicVolume;
            SoundSlider.value = Facade.Instance<MusicManager>().EffectVolume;
        }

        public UISlider MusicSlider;
        public UISlider SoundSlider;

        public void OnChangeMusic(float music)
        {
            Facade.Instance<MusicManager>().MusicVolume = music;
        }

        public void OnChangeSound(float sound)
        {
            Facade.Instance<MusicManager>().EffectVolume = sound;
        }
    }
}
