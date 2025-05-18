using Assets.Scripts.Common.UI;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.slyz.Windows
{
    public class SlyzSettingWindow : SettingWindow
    {
        /// <summary>
        /// 设置 皇家同花顺以上关闭自动开始  
        /// </summary>
        public UIToggle RoyalFlushCheckBox;
        /// <summary>
        /// 设置 同花顺及以上关闭自动开始
        /// </summary>
        public UIToggle StraightFlushCheckbox;
        /// <summary>
        /// 设置 炸弹及以上关闭自动开始
        /// </summary>
        public UIToggle FourOfKindCheckbox;
        /// <summary>
        /// 设置 葫芦及以上关闭自动开始
        /// </summary>
        public UIToggle FullHouseCheckbox;                                                 
        /// <summary>
        /// 
        /// </summary>
        public UIToggle CheckboxAudio = null;

        protected override void OnFreshView()
        {
            var gdata = App.GetGameData<SlyzGameData>();
            var autoState = gdata.AutoStartState;
            SetToogles(autoState);
            InitVolume();
        }

        private void SetToogles(int autoState)
        {
            var isSelecte = autoState > -1;
            FullHouseCheckbox.Set(isSelecte && autoState <= CardTeam.TYPE_HL, false);
            FourOfKindCheckbox.Set(isSelecte && autoState <= CardTeam.TYPE_ZD, false);
            StraightFlushCheckbox.Set(isSelecte && autoState <= CardTeam.TYEP_THS, false);
            RoyalFlushCheckBox.Set(isSelecte && autoState <= CardTeam.TYPE_HJTHS, false);
        }

        private void InitVolume()
        {
            if (CheckboxAudio == null) return;
            var volum = PlayerPrefs.GetInt("setting_slyz_checkbox_audio", 1);
            Facade.Instance<MusicManager>().EffectVolume = volum;
            Facade.Instance<MusicManager>().MusicVolume = volum;
            CheckboxAudio.value = volum == 1;
        }

        /// <summary>
        /// 设置自动停止类型
        /// </summary>
        /// <param name="toggle"></param>
        public void OnAutoCloseWithCardType(UIToggle toggle)
        {
            var gdata = App.GetGameData<SlyzGameData>();
            int type;
            int.TryParse(toggle.name, out type);
            if (!toggle.value && gdata.AutoStartState == type)
            {
                type = -1;
            }
            gdata.AutoStartState = type;
            SetToogles(type);
        }

        public void OnAudioClick()
        {
            var change = CheckboxAudio.value;
            var volum = change ? 1 : 0;
            Facade.Instance<MusicManager>().EffectVolume = volum;
            Facade.Instance<MusicManager>().MusicVolume = volum;
            PlayerPrefs.SetInt("setting_slyz_checkbox_audio", volum);
        }
    }
}
