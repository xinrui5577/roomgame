using System;
using com.yxixia.utile.Utiles;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Common.UI
{
    /// <summary>
    /// 设置窗口
    /// </summary>
    public class SettingWindow : YxWindow
    {
        /// <summary>
        /// 音效滑块
        /// </summary>
        [Tooltip("音效滑块")]
        public YxBaseSliderAdapter EffectVolume;
        /// <summary>
        /// 音乐滑块
        /// </summary>
        [Tooltip("音乐滑块")]
        public YxBaseSliderAdapter BackMusicVolume;
        /// <summary>
        ///
        /// </summary>
        [Tooltip("")]
        public YxBaseToggleAdapter[] MusiceToggles;
        /// <summary>
        /// 玩家信息
        /// </summary>
        public YxBasePlayer PlayerInfo;

        protected override void OnAwake()
        {
            var musicMgr = Facade.Instance<MusicManager>();
            SetToggles(MusiceToggles.GetElement(0), musicMgr.MusicVolume>0);
            SetToggles(MusiceToggles.GetElement(1), musicMgr.EffectVolume > 0);
        }

        private void SetToggles(YxBaseToggleAdapter toggle,bool state)
        {
            if (toggle == null) return;
            toggle.StartsActive = state;
            toggle.Value = state;
        }

        protected override void OnStart()
        {
            var musicMgr = Facade.Instance<MusicManager>();
            if (EffectVolume != null) { EffectVolume.Value = musicMgr.EffectVolume; }
            if (BackMusicVolume != null) { BackMusicVolume.Value = musicMgr.MusicVolume;}
            if (PlayerInfo != null) { PlayerInfo.Info = UserInfoModel.Instance.UserInfo; }
        }

        public void OnUpdateMusicVolume(float volume)
        {
            Facade.Instance<MusicManager>().MusicVolume = volume;
        }

        public void OnUpdateSoundVolume(float volume)
        {
            Facade.Instance<MusicManager>().EffectVolume = volume;
        }

        /// <summary>
        /// 切换账号
        /// </summary>
        public void OnChangeAccount()
        {
            HallMainController.Instance.ChangeAccount();
            Close();
        }

        public void OnQuit()
        {
            Close();
            YxMessageBox.Show("确定要退出大厅吗?", "", (box, btnName) =>
                {
                    if (btnName == YxMessageBox.BtnLeft)
                    {
                        //Application.Quit();
                        App.QuitGame();
                    }
                }, true, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle); 
        }

        public void OnChangeMusiceType()
        {
            var toggle = UIToggle.current;
            if (toggle == null) return;
            if (!toggle.value) return;
            var typeName = toggle.name;
            var oldName = PlayerPrefs.GetString("MusiceType");
            if(typeName == oldName)return;
            PlayerPrefs.SetString("MusiceType", typeName);
        }


        public void OnChangeToggle(YxBaseToggleAdapter toggle)
        {
            if (toggle == null) return;
            var value = toggle.Value ? 1 : 0;
            var index = Array.IndexOf(MusiceToggles,toggle);
            if (index == 0)
            {
                Facade.Instance<MusicManager>().MusicVolume = value;
            }
            else
            {
                Facade.Instance<MusicManager>().EffectVolume = value;
            }
        }

        [SerializeField]
        private YxEUIType _uitype;
        public override YxEUIType UIType
        {
            get { return _uitype; }
        }
    }
}
