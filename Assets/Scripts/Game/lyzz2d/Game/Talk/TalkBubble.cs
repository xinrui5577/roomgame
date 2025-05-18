using Assets.Scripts.Game.lyzz2d.Utils;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.lyzz2d.Game.Talk
{
    public class TalkBubble : MonoBehaviour
    {
        [SerializeField] private UISprite _bgSprite;

        [SerializeField] private readonly int _defHeight = 130;

        [SerializeField] private readonly int _defWidth = 130;

        [SerializeField] private UISprite _phiz;

        private readonly int _phizBgHeight = 130;

        private readonly int _phizBgWidth = 200;

        [SerializeField] private UILabel _showLabel;

        private void Awake()
        {
            _showLabel.onPostFill += delegate { _bgSprite.width = _defWidth + _showLabel.width; };
        }

        public void ShowLabel(string text)
        {
            Show(true);
            _showLabel.gameObject.SetActive(true);
            _showLabel.text = text;
            Invoke("Hide", 5);
        }

        public void ShowPhiz(int index)
        {
            Show(true);
            var atla = AtlasManager.Instance.GetAtlaByName(index.ToString());
            if (atla != null)
            {
                _phiz.atlas = atla;
                var ani = _phiz.gameObject.GetComponent<UISpriteAnimation>();
                ani.namePrefix = index.ToString();
                ani.Play();
                var phizWidth = atla.spriteList[0].width;
                var phizHeight = atla.spriteList[0].height;
                _bgSprite.width = phizWidth > _phizBgWidth ? phizWidth : _phizBgWidth;
                _bgSprite.height = phizHeight > _phizBgHeight ? phizHeight : _phizBgHeight;
                _phiz.gameObject.SetActive(true);
                Invoke("Hide", 5);
            }
            else
            {
                YxDebug.LogError(string.Format("There is not exist such atla,name is {0}", index));
            }
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
            Reset();
            gameObject.SetActive(state);
        }

        private void Hide()
        {
            CancelInvoke("Hide");
            Show(false);
        }
    }
}