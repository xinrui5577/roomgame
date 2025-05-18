using UnityEngine;
using System.Collections;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;
using Assets.Scripts.Common.UI;

namespace Assets.Scripts.Game.SportsCarClub
{
    public class GameSettings : SettingWindow
    {
        public GameObject slider_BGM;
        public GameObject slider_SE;
        public GameObject settingsView;

        // Use this for initialization
        void Start()
        {
            InitSilerValue();
        }

        // Update is called once per frame
        void Update()
        {
            OnUpdateVolume();
        }

        public void OnClickClose()
        {
            settingsView.SetActive(false);
        }

        public void OnClickOpen()
        {
            settingsView.SetActive(true);
        }

        public void InitSilerValue()
        {
            var bgmValue = Facade.Instance<MusicManager>().MusicVolume;
            var seValue = Facade.Instance<MusicManager>().EffectVolume;

            slider_BGM.GetComponentInChildren<UISlider>().value = bgmValue;
            slider_SE.GetComponentInChildren<UISlider>().value = seValue;
        }

        public void OnUpdateVolume()
        {
            var bgmValue = slider_BGM.GetComponentInChildren<UISlider>().value;
            var seValue = slider_SE.GetComponentInChildren<UISlider>().value;
            base.OnUpdateMusicVolume(bgmValue);
            base.OnUpdateSoundVolume(seValue);
        }

    }
}