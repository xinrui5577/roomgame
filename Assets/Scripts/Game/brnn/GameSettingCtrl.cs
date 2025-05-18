using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.brnn
{
    public class GameSettingCtrl : MonoBehaviour
    {

        public UISprite MusicSprite;
        public UISprite SoundSprite;
        public GameObject Pay;
        private int _isMusic;
        private int _isSound;

        protected void Start()
        {
            ChangeMusic();
            ChangeSound();
        }

        public void ChangeMusic()
        {
            if (_isMusic == 0)
            {
                _isMusic = 1;
                Facade.Instance<MusicManager>().MusicVolume = 1;
                MusicSprite.spriteName = "30";
            }
            else
            {
                _isMusic = 0;
                Facade.Instance<MusicManager>().MusicVolume = 0;
                MusicSprite.spriteName = "32";
            }
        }

        public void ChangeSound()
        {
            if (_isSound == 0)
            {
                _isSound = 1;
                Facade.Instance<MusicManager>().EffectVolume = 1;
                SoundSprite.spriteName = "36";
            }
            else
            {
                _isSound = 0;
                Facade.Instance<MusicManager>().EffectVolume = 0;
                SoundSprite.spriteName = "34";
            }
        }

        public void OpenUrl()
        {
            if (App.IsGameLoadFromHall)
            {
                Debug.Log("打开大厅支付界面");
                YxWindowManager.OpenWindow("StoreWindow");
            }
            else
            {
                Application.OpenURL("http://www.kawuxing.com/chess/index.php/Payment/index/token/" + App.GetGameData<BrnnGameData>().UserToken + "/uid/" + App.GetGameData<BrnnGameData>().UserId);
            }
        }

        public void QuitGame()
        {
            App.QuitGameWithMsgBox();
        }
    }
}
