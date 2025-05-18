using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Common.MahJongCommon
{
    public class PlayerDirection :MonoBehaviour
    {

        private static int _selfIndex = 0;

        [SerializeField]
        private UISprite[] _directionSprites;

        void Awake()
        {
            _selectSprite = _directionSprites[0];
        }

        public void ResetDNXBState(bool selectFirst=true)
        {
            foreach (var spr in _directionSprites)
            {
                spr.gameObject.SetActive(false);
            }
            CancelInvoke("Breathe");
            if (selectFirst)
            {
                if (_selectSprite == null)
                {
                    _selectSprite = _directionSprites[0];
                }
                _selectSprite.alpha = 1;
            }
        }

        private UISprite _selectSprite;
        public void SetDNXBShow(int index)
        {
            if (_selectSprite!=null)
            {
                CancelInvoke("Breathe");
                _selectSprite.alpha = 1;
                _selectSprite.gameObject.SetActive(false);
            }
            if (index<0||index>=_directionSprites.Length)
            {
                
            }
            _selectSprite = _directionSprites[index];
            _selectSprite.gameObject.SetActive(true);
            InvokeRepeating("Breathe",0,0.1f);
        }

        private float changeNumber = 0.1f;

        void Breathe()
        {
            if (_selectSprite.alpha<=0.2f)
            {
                changeNumber = 0.1f;
            }
            if (_selectSprite.alpha >=0.9f)
            {
                changeNumber =-0.1f;
            }
            _selectSprite.alpha += changeNumber;
        }
    }
}
