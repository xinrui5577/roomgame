using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.sanpian.CommonCode
{
    public  class TalkBubble : MonoBehaviour
    {
        [SerializeField]
        private UISprite _bgSprite;
        [SerializeField]
        private int _defWidth=130;

        [SerializeField]
        private int _defHeight=130;

        private int _phizBgWidth=200;

        private int _phizBgHeight = 130;

        [SerializeField]
        private UILabel _showLabel;

        [SerializeField]
        private UISprite _phiz;

        public float TalkTime;

        void Awake()
        {
            _showLabel.onPostFill += delegate
            {
                YxDebug.LogWarning("onPostFill：" + _showLabel.width);
                _bgSprite.width = _defWidth + _showLabel.width;
            };
        }

        public void ShowLabel(string text)
        {              
            Show(true);
            _showLabel.gameObject.SetActive(true);
            _showLabel.text = text;
            Invoke("Hide", TalkTime);
        }

       
        public void ShowPhiz(int index)
        {
            Show(true);
            //_phiz.spriteName = index.ToString();
            //_phiz.MakePixelPerfect();
            char temp = (char) index;
            _phiz.spriteName = temp.ToString() + 1;
            UISpriteAnimation ani = _phiz.gameObject.GetComponent<UISpriteAnimation>();
            ani.namePrefix = temp.ToString();
            _phiz.gameObject.SetActive(true);
            _bgSprite.width = _phizBgWidth;
            _bgSprite.height = _phizBgHeight;
            Invoke("Hide", TalkTime);
        }

        private void Reset()
        {
            CancelInvoke("Hide");
            _bgSprite.width = _defWidth;
            _bgSprite.height = _defHeight;
            _showLabel.text = "";
            _phiz.gameObject.SetActive(false);
            _showLabel.gameObject.SetActive(false);
        }

        private void Show(bool state)
        {
            //Reset();
            CancelInvoke("Hide");
            gameObject.SetActive(state);        
        }

        void Hide()
        {
            CancelInvoke("Hide");
            Show(false);
        }
    }
}
