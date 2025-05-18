using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.brnn
{
    public class EffectCtrl : MonoBehaviour
    {

        public SpringPosition Spring;
        public UISprite Sprite;
        public GameObject SpriteGo;
        private bool _isshow = true;

        public void ShowEffect(int e)
        {
            SpriteGo.SetActive(true);
            Spring.enabled = false;
            if (e == 0)
            {
                Sprite.spriteName = "AllLose";
                Sprite.MakePixelPerfect();
            }
            else
            {
                Sprite.spriteName = "AllWin";
                Sprite.MakePixelPerfect();
            }
            Spring.target = new Vector3(0, 0, 0);
            Spring.enabled = true;
            _isshow = true;
            Invoke("CloseEffect", 2);
        }

        protected void CloseEffect()
        {
            App.GetGameManager<BrnnGameManager>().ParticleCtrl.SetParticleEffect(1);
            Spring.enabled = false;
            Spring.target = new Vector3(0, 1000, 0);
            Spring.enabled = true;
            _isshow = false;
        }

        public void OnStop()
        {
            if (!_isshow)
            {
                SpriteGo.SetActive(false);
            }
        }

    }
}
