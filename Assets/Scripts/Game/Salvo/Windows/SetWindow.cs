using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.Salvo.Windows
{
    public class SetWindow : YxNguiWindow
    {
        public UIToggle SoundToggle;
        public UIToggle MusicToggle;

        protected override void OnShow()
        {
            var musicMgr = Facade.Instance<MusicManager>();
            musicMgr.Play("btn");
            SoundToggle.value = musicMgr.EffectVolume >0;
            MusicToggle.value = musicMgr.MusicVolume > 0;
        }

        protected override void OnHide()
        {
            Facade.Instance<MusicManager>().Play("btn");
        }

        public void OnSetMusic(GameObject toggleObj)
        {
            Facade.Instance<MusicManager>().Play("btn");
            var toggle = toggleObj.GetComponent<UIToggle>();
            var volume = toggle.value?1:0;
            Facade.Instance<MusicManager>().MusicVolume = volume;
        }

        public void OnSetSound(GameObject toggleObj)
        {
            Facade.Instance<MusicManager>().Play("btn");
            var toggle = toggleObj.GetComponent<UIToggle>();
            var volume = toggle.value ? 1 : 0;
            Facade.Instance<MusicManager>().EffectVolume = volume;
        } 
    }
}
