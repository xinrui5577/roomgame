using Assets.Scripts.Game.lswc.Core;
using YxFramwork.Manager;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.lswc.Windows
{
    /// <summary>
    /// 声音设置窗口
    /// </summary>
    public class LSSettingWindow : InstanceControl
    {
        private Slider _musicVolume;

        private Slider _effectVolume;

        private Button _sureBtn;

        private  Button _cancelBtn;

        private void Awake()
        {
            Find();
            InitListenr();
            var musicMgr = Facade.Instance<MusicManager>();
            _musicVolume.value = musicMgr.MusicVolume;
            _effectVolume.value = musicMgr.EffectVolume;
        }

        private void Find()
        {
            _sureBtn = transform.FindChild("Buttons/sure").GetComponent<Button>();
            _cancelBtn = transform.FindChild("Buttons/cancel").GetComponent<Button>();
            _musicVolume = transform.FindChild("musicSlider").GetComponent<Slider>();
            _effectVolume = transform.FindChild("SoundSlider").GetComponent<Slider>();
        }
        private void InitListenr()
        {
            _sureBtn.onClick.AddListener(OnClickSureBtn);
            _cancelBtn.onClick.AddListener(OnClickCancelBtn);
        }

        private void OnClickSureBtn()
        {
            App.GetGameManager<LswcGamemanager>().SystemControl.PlaySuccess(true);
            var musicMgr = Facade.Instance<MusicManager>();
            musicMgr.MusicVolume = _musicVolume.value;
            musicMgr.EffectVolume = _effectVolume.value;
            Hide();
        }

        private void OnClickCancelBtn()
        {
            App.GetGameManager<LswcGamemanager>().SystemControl.PlaySuccess(true);
            Hide();
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public override void OnExit()
        {
        }
    }
}
