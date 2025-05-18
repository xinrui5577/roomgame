using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.BaiTuan
{
    public class GameSettingCtrl : MonoBehaviour
    {
        public GameObject Pay;
        private bool _isSoundOn;
        public UISprite SoundBtnSprite;
        public UIButton SoundBtn;
        //private int _isSound = 1;
        public void Start()
        {
            _isSoundOn = Facade.Instance<MusicManager>().MusicVolume > 0;
            _isSoundOn = !_isSoundOn;
            ChangeMusic();
        }
        public void ChangeMusic()
        {
            _isSoundOn = !_isSoundOn;
            if (_isSoundOn)
            {
                SoundBtn.normalSprite = "12";
                SoundBtn.hoverSprite = "13";
                SoundBtn.pressedSprite = "14";
                SoundBtnSprite.spriteName = "12";
                Facade.Instance<MusicManager>().OnOffVolume(true);
            }
            else
            {
                SoundBtn.normalSprite = "15";
                SoundBtn.hoverSprite = "16";
                SoundBtn.pressedSprite = "17";
                SoundBtnSprite.spriteName = "15";
                Facade.Instance<MusicManager>().OnOffVolume(false);
            }
        }
    }
}
