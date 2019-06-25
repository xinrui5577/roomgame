using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using Assets.Scripts.Common.components;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Common.UI
{
    /// <summary>
    /// 设置窗口
    /// </summary>
    public class SettingWindow : YxNguiWindow
    {
        /// <summary>
        /// 音效滑块
        /// </summary>
        [Tooltip("音效滑块")]
        public UISlider EffectVolume;
        /// <summary>
        /// 音乐滑块
        /// </summary>
        [Tooltip("音乐滑块")]
        public UISlider BackMusicVolume;
        /// <summary>
        /// 版本文本
        /// </summary>
        [Tooltip("版本文本")]
        public UILabel VersionLabel;
        /// <summary>
        /// 音乐滑块组
        /// </summary>
        [Tooltip("音乐滑块")]
        public UIToggle[] MusiceToggles;
        /// <summary>
        /// 玩家昵称文本
        /// </summary>
        [Tooltip("玩家昵称文本")]
        public UILabel UserNickLabel;
        /// <summary>
        /// 头像
        /// </summary>
        [Tooltip("头像")]
        public UITexture Portrait;

        protected override void OnAwake()
        {
            var count = MusiceToggles.Length;
            var typeName = PlayerPrefs.GetString("MusiceType", count>0?MusiceToggles[0].name:"");
            for (var i = 0; i < count; i++)
            {
                var toggle = MusiceToggles[i];
                if (toggle.name != typeName)
                {
                    toggle.startsActive = false;
                    continue;
                }
                toggle.startsActive = true;
                toggle.value = true;
            }
        }

        protected override void OnStart()
        {  
            EffectVolume.value = MusicManager.Instance.EffectVolume;
            BackMusicVolume.value = MusicManager.Instance.MusicVolume;
            if (VersionLabel != null) VersionLabel.text = Application.version;
            var userInfo = UserInfoModel.Instance.UserInfo;
            if (UserNickLabel != null)
            {
                UserNickLabel.text = userInfo.NickM;
            }
            if (Portrait != null)
            {
                PortraitRes.SetPortrait(userInfo.AvatarX, Portrait, userInfo.SexI);
            } 
        }

        public void OnUpdateMusicVolume(float volume)
        {
            MusicManager.Instance.MusicVolume = volume;
        }

        public void OnUpdateSoundVolume(float volume)
        {
            MusicManager.Instance.EffectVolume = volume;
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
                        Application.Quit();
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
//            MusicManager.Instance.PlayBacksound(typeName);
            PlayerPrefs.SetString("MusiceType", typeName);
        }
    }
}
