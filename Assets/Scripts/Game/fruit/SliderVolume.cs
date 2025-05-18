using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;
using Assets.Scripts.Common.UI;

namespace Assets.Scripts.Game.fruit
{
    public class SliderVolume : SettingWindow
    {
        public GameObject slider_BGM;
        public GameObject slider_SE;

        // Use this for initialization
        void Start()
        {
            initSilerValue();
        }

        // Update is called once per frame
        void Update()
        {
            OnUpdateVolume();
        }

        public void onClickClose()
        {
            Facade.Instance<MusicManager>().Play("button");
            Destroy(gameObject);
        }

        public void initSilerValue()
        {
            var bgmValue = Facade.Instance<MusicManager>().MusicVolume;
            var seValue = Facade.Instance<MusicManager>().EffectVolume;

            slider_BGM.GetComponentInChildren<Slider>().value = bgmValue;
            slider_SE.GetComponentInChildren<Slider>().value = seValue;
        }

        public void OnUpdateVolume()
        {
            var bgmValue = slider_BGM.GetComponentInChildren<Slider>().value;
            var seValue = slider_SE.GetComponentInChildren<Slider>().value;
            base.OnUpdateMusicVolume(bgmValue);
            base.OnUpdateSoundVolume(seValue);
        }

    }
}