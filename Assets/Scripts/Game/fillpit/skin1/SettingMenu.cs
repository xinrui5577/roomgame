using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.fillpit.skin1
{
    public class SettingMenu : MonoBehaviour
    {

        /// <summary>
        /// 关闭按钮
        /// </summary>
        [SerializeField]
        private UIButton _closeBtn = null;

        /// <summary>
        /// 音乐设置拉条
        /// </summary>
        [SerializeField]
        private UISlider _musicSlider = null;


        /// <summary>
        /// 音效设置拉条
        /// </summary>
        [SerializeField]
        private UISlider _soundSlider = null;

        // Use this for initialization
        protected void Start()
        {
            _closeBtn.onClick.Add(new EventDelegate(OnClickClose));

            //初始化声音设置
            _soundSlider.value = Facade.Instance<MusicManager>().EffectVolume;
            _musicSlider.value = Facade.Instance<MusicManager>().MusicVolume;
        }

        public void OnClickClose()
        {
            gameObject.SetActive(false);
        }

        public void SetMusicValue()
        {
            Facade.Instance<MusicManager>().MusicVolume = _musicSlider.value;
        }

        public void SetSoundValue()
        {
            Facade.Instance<MusicManager>().EffectVolume = _soundSlider.value;

        }
    }
}