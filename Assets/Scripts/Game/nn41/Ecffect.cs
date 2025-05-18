using UnityEngine;

namespace Assets.Scripts.Game.nn41
{
    public class Ecffect : MonoBehaviour
    {
        public UISprite HuoSprite0;
        public UISprite HuoSprite1;
        public GameObject Effect1;
        public GameObject Effect2;
        public UISprite Effect2Sprite;

        private int _sprite1Index = 1;
        private int _sprite2Index = 1;
        protected void Start()
        {
        }

        public void SetWinEffect()
        {
            Effect1.SetActive(true);
            InvokeRepeating("SetWinEffectA", 0.1f, 0.1f);
            Invoke("Close", 3);
        }

        protected void SetWinEffectA()
        {
            if (_sprite1Index > 3)
            {
                _sprite1Index = 1;
            }
            HuoSprite0.spriteName = "h" + _sprite1Index;
            HuoSprite1.spriteName = "h" + _sprite1Index;
            _sprite1Index++;
        }

        public void SetLostEffect()
        {
            Effect2.SetActive(true);
            InvokeRepeating("SetLostEffectA", 0.1f, 0.1f);
            Invoke("Close", 3);
        }

        protected void SetLostEffectA()
        {
            if (_sprite2Index > 10)
            {
                _sprite2Index = 1;
                Close();
                return;
            }
            Effect2Sprite.spriteName = "n" + _sprite2Index;
            _sprite2Index++;
        }

        private void Close()
        {
            _sprite1Index = 1;
            _sprite2Index = 1;
            Effect1.SetActive(false);
            Effect2.SetActive(false);
            CancelInvoke("SetLostEffectA");
            CancelInvoke("SetWinEffectA");
        }
    }
}
