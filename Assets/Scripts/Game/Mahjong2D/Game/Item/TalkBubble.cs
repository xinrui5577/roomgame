using System.Collections;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.Mahjong2D.Game.GameCtrl;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Game.Item
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
        [Tooltip("表情带动画")]
        public bool PhizWithAni = true;
        [Tooltip("隐藏时间")]
        public float HideTime = 5;
        /// <summary>
        /// 静态表情格式
        /// </summary>
        public string StaticPhizFormat = "{0}-0";
        /// <summary>
        ///  图集格式
        /// </summary>
        public string AtlasFormat = "{0}";

        void Awake()
        {
            Reset();
            _showLabel.onPostFill += delegate
            {
                _bgSprite.width = _defWidth + _showLabel.width;
            };
        }

        public void ShowLabel(string text)
        {              
            Show(true);
            _showLabel.gameObject.SetActive(true);
            _showLabel.TrySetComponentValue(text);
            _bgSprite.enabled=true;
            DoHide();
        }

        private Coroutine _hideCor;
        public void ShowPhiz(int index)
        {
            Show(true);
            if (PhizWithAni)
            {
                UIAtlas atla = AtlasManager.Instance.GetAtlaByName(string.Format(AtlasFormat,index));
                if (atla != null)
                {
                    _phiz.atlas = atla;
                    UISpriteAnimation ani = _phiz.gameObject.GetComponent<UISpriteAnimation>();
                    ani.namePrefix = index.ToString();
                    ani.Play();
                    int phizWidth = atla.spriteList[0].width;
                    int phizHeight = atla.spriteList[0].height;
                    _bgSprite.width = phizWidth > _phizBgWidth ? phizWidth : _phizBgWidth;
                    _bgSprite.height = phizHeight > _phizBgHeight ? phizHeight : _phizBgHeight;
                    _bgSprite.gameObject.TrySetComponentValue(true);
                }
                else
                {
                    YxDebug.LogError(string.Format("There is not exist such atla,name is {0}", index));
                }
            }
            else
            {
                _phiz.TrySetComponentValue(string.Format(StaticPhizFormat,index));
                _phiz.MakePixelPerfect();
            }
            _phiz.gameObject.SetActive(true);
            DoHide();
        }

        private void DoHide()
        {
            if (gameObject.activeInHierarchy)
            {
                if (_hideCor != null)
                {
                    StopCoroutine(_hideCor);
                }
                _hideCor = StartCoroutine(Hide(HideTime));
            }
        }

        private void Reset()
        {
            _bgSprite.width = _defWidth;
            _bgSprite.height = _defHeight;
            _bgSprite.enabled = false;
            _showLabel.TrySetComponentValue(string.Empty);
            _phiz.gameObject.SetActive(false);
            _showLabel.gameObject.SetActive(false);
        }

        private void Show(bool state)
        {
            Reset();
            gameObject.SetActive(state);        
        }

        IEnumerator Hide(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            Show(false);
        }
    }
}
