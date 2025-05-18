using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Game.bjlb
{
    public class GameSettingCtrl : MonoBehaviour
    {

        public void OpenUrl()
        {
            //TODO PAYMENT
            //Application.OpenURL("http://www.kawuxing.com/chess/index.php/Payment/index/token/" + GlobalData.UserToken + "/uid/" + GlobalData.UserID);
        }

        public UISprite ChangeSoundBtn;

        public GameObject SettingList;

        public GameObject CloseBtn;

        /// <summary>
        /// 箭头图片旋转
        /// </summary>
        public Transform Arrow;

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

        protected virtual void Start()
        {
            SetSound(Facade.Instance<MusicManager>().MusicVolume > 0);  
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
            SetSound();
        }

        public void SetSound(bool haveSound)
        {
            ChangeSoundBtn.GetComponent<UIToggle>().value = haveSound;
            SetSound();
        }

        /// <summary>
        /// 设置声音
        /// </summary>
        private void SetSound()
        {
            bool hasSound = ChangeSoundBtn.GetComponent<UIToggle>().value;
            string spriteName = hasSound ? _soundMaxSprName : _soundMinSprName;
            ChangeSoundBtn.spriteName = spriteName;
            ChangeSoundBtn.MakePixelPerfect();
            int soundVal = hasSound ? 1 : 0;
            Facade.Instance<MusicManager>().MusicVolume = soundVal;
            Facade.Instance<MusicManager>().EffectVolume = soundVal;
        }

        /// <summary>
        /// 点击返回按钮
        /// </summary>
        public void OnClickBackBtn()
        {
            bool isBanker = App.GetGameData<BjlGameData>().IsBanker;
            if(isBanker)
            {
                YxMessageBox.Show(new YxMessageBoxData
                {
                    Msg = "您现在是庄家,无法离开游戏",
                    Delayed = 5
                });
            }
            else
            {
                App.GetGameManager<BjlGameManager>().OnQuitGameClick();
            }

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
            if (Arrow != null)
                Arrow.localEulerAngles = Vector3.zero;
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
            if (Arrow != null)
                Arrow.localEulerAngles = new Vector3(0, 0, 180);
        }

    }
}
