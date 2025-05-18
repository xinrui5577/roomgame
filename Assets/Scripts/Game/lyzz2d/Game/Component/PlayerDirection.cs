using UnityEngine;

namespace Assets.Scripts.Game.lyzz2d.Game.Component
{
    public class PlayerDirection : MonoBehaviour
    {
        private static int _selfIndex = 0;

        private static readonly string[] DnxbStr = {"dong", "bei", "xi", "nan"};

        private static readonly string[] DnxbZhStr = {"东", "北", "西", "南"};

        [SerializeField] private UISprite[] _directionSprites;

        private UISprite _selectSprite;

        private float changeNumber = 0.1f;

        private void Awake()
        {
            _selectSprite = _directionSprites[0];
        }

        public void ResetDNXBState()
        {
            foreach (var spr in _directionSprites)
            {
                spr.gameObject.SetActive(false);
            }
            CancelInvoke("Breathe");
            _selectSprite.alpha = 1;
        }

        public void SetDNXBShow(int index)
        {
            if (_selectSprite != null)
            {
                CancelInvoke("Breathe");
                _selectSprite.alpha = 1;
                _selectSprite.gameObject.SetActive(false);
            }
            _selectSprite = _directionSprites[index];
            _selectSprite.gameObject.SetActive(true);
            InvokeRepeating("Breathe", 0, 0.1f);
        }

        private void Breathe()
        {
            if (_selectSprite.alpha <= 0.2f)
            {
                changeNumber = 0.1f;
            }
            if (_selectSprite.alpha >= 0.9f)
            {
                changeNumber = -0.1f;
            }
            _selectSprite.alpha += changeNumber;
        }
    }
}