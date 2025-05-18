using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.Ttzkf
{
    public class SettingCtrl : MonoBehaviour
    {
        public GameObject Bg;
        public GameObject BtnChangeRoom;
        public GameObject BtnQuitGame;
        public UISlider MusicSlider;
        public UISlider SoundSlider;

        protected void Start()
        {
            if (Bg.activeSelf)
            {
                Bg.SetActive(false);
            }

            if (MusicSlider != null) MusicSlider.value = Facade.Instance<MusicManager>().MusicVolume;
            if (SoundSlider != null) SoundSlider.value = Facade.Instance<MusicManager>().EffectVolume;
        }
        /// <summary>
        /// 点击关闭按钮
        /// </summary>
        public void OnClickCloseBtn()
        {
            Bg.SetActive(false);
        }
        /// <summary>
        /// 点击设置按钮
        /// </summary>
        public void OnClickSettingBtn()
        {
            Bg.SetActive(true);
        }
        /// <summary>
        /// 改变声音的大小
        /// </summary>
        /// <param name="volume"></param>
        public void OnUpdateMusicVolume(float volume)
        {
            Facade.Instance<MusicManager>().MusicVolume = volume;
        }
        /// <summary>
        /// 改变音效的大小
        /// </summary>
        /// <param name="volume"></param>
        public void OnUpdateSoundVolume(float volume)
        {
            Facade.Instance<MusicManager>().EffectVolume = volume;
        }

        public void HideBtn()
        {
            if (BtnChangeRoom == null) return;
            BtnChangeRoom.SetActive(false);
            BtnQuitGame.SetActive(false);
        }
        /// <summary>
        /// 换桌的按钮点击事件
        /// </summary>
        public void HuanZhuo()
        {
            App.GetGameManager<TtzGameManager>().UOut();
        }
        /// <summary>
        /// 返回大厅
        /// </summary>
        public void BackHall()
        {
            App.GetGameManager<TtzGameManager>().OnReturnHall();
        }
    }
}
