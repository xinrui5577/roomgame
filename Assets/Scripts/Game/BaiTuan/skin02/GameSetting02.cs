using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Common;

namespace Assets.Scripts.Game.BaiTuan.skin02
{
    public class GameSetting02 : MonoBehaviour
    {
        public UISprite ChangeSoundBtn;

        public GameObject SettingList;

        public GameObject CloseBtn;

        /// <summary>
        /// 有声音的图片名称
        /// </summary>
        [SerializeField]
        private string _soundMaxSprName = string.Empty;
        /// <summary>
        /// 没有声音的图片名称
        /// </summary>
        [SerializeField]
        private string _soundMinSprName = string.Empty;

        private bool _hasSound = true;

        protected virtual void Start()
        {
            SetSound();
            HideSettingList();
        }


        /// <summary>
        /// 显示物体
        /// </summary>
        /// <param name="obj">显示的物体</param>
        public void ShowGameObj(GameObject obj)
        {
            obj.SetActive(true);
        }

        /// <summary>
        /// 显示物体
        /// </summary>
        /// <param name="obj">隐藏的物体</param>
        public void HideGameObj(GameObject obj)
        {
            obj.SetActive(false);
        }

        /// <summary>
        /// 点击声音按钮
        /// </summary>
        public void OnClickChangeSound()
        {
            _hasSound = !_hasSound;
            SetSound();
        }

        public void SetSound(bool haveSound)
        {
            _hasSound = haveSound;
            SetSound();
        }

        /// <summary>
        /// 设置声音
        /// </summary>
        private void SetSound()
        {
            string spriteName = _hasSound ? _soundMaxSprName : _soundMinSprName;
            ChangeSoundBtn.spriteName = spriteName;
            ChangeSoundBtn.MakePixelPerfect();
            ChangeSoundBtn.GetComponent<UIButton>().normalSprite = spriteName;
            int soundVal = _hasSound ? 1 : 0;
            Facade.Instance<MusicManager>().MusicVolume = soundVal;
            Facade.Instance<MusicManager>().EffectVolume = soundVal;
        }

        /// <summary>
        /// 点击返回按钮
        /// </summary>
        public void OnClickBackBtn()
        {
            App.GetGameManager<BtwGameManager>().OnQuitGameClick();
            HideSettingList();
        }

        /// <summary>
        /// 隐藏设置菜单
        /// </summary>
        public void HideSettingList()
        {
            if (SettingList != null)
                HideGameObj(SettingList);
            if (CloseBtn != null)
                HideGameObj(CloseBtn);
        }

        /// <summary>
        /// 显示设置菜单
        /// </summary>
        public void ShowSettingList()
        {
            if (SettingList != null)
                ShowGameObj(SettingList);
            if (CloseBtn != null)
                ShowGameObj(CloseBtn);
        }
    }
}