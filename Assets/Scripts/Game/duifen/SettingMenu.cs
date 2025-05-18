using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
// ReSharper disable FieldCanBeMadeReadOnly.Local



namespace Assets.Scripts.Game.duifen
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
        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once UnusedMember.Local
        void Start()
        {
            _closeBtn.onClick.Add(new EventDelegate(OnClickClose));
            var musicMgr = Facade.Instance<MusicManager>();
            //初始化声音设置
            _soundSlider.value = musicMgr.EffectVolume;
            _musicSlider.value = musicMgr.MusicVolume;

        }

        private void OnClickClose()
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