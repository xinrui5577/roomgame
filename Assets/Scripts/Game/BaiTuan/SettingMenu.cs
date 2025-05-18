using UnityEngine;
using System.Collections;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.BaiTuan
{

    public class SettingMenu : MonoBehaviour
    {

        [Tooltip("设置窗口对象")] [SerializeField] private GameObject _settingView = null;

        [Tooltip("音乐音量设置")] [SerializeField] private UISlider _volumeSlider = null;

        [Tooltip("音效音量设置")] [SerializeField] private UISlider _effectSlider = null;


        // Use this for initialization
        void Start()
        {

            _settingView.gameObject.SetActive(false);
            _volumeSlider.value = Facade.Instance<MusicManager>().MusicVolume;
            _effectSlider.value = Facade.Instance<MusicManager>().EffectVolume;

        }

        public void OnDragVolumeSlider(float volume)
        {

            Facade.Instance<MusicManager>().MusicVolume = volume;

        }

        public void OnDragEffectSlider(float volume)
        { 
            Facade.Instance<MusicManager>().EffectVolume = volume; 
        }



        /// <summary>
        /// 打开设置窗口
        /// </summary>
        public void OnClickSettingBtn()
        { 
            _settingView.gameObject.SetActive(true); 
        }

        /// <summary>
        /// 关闭设置窗口
        /// </summary>
        public void OnClickCloseBtn()
        { 
            _settingView.SetActive(false);

        }
    }
}