using UnityEngine;

namespace Assets.Scripts.Game.BlackJackCs
{
    public class DoubleMark : MonoBehaviour
    {

        [SerializeField]
        private UISpriteAnimation _sprAnim;

        [SerializeField]
        private GameObject _doubleMark;

        /// <summary>
        /// 动画帧图片个数
        /// </summary>
        [SerializeField]
        private int spriteCount = 7;

        [SerializeField]
        private Vector3 _targetPos;

        [SerializeField]
        float times = 1;
     

        /// <summary>
        /// 显示双倍
        /// </summary>
        public void ShowDoubleMark()
        {
            _sprAnim.gameObject.SetActive(true);
            _sprAnim.ResetToBeginning();
            _sprAnim.Play();

            float spaceTime = 1 / (float)_sprAnim.framesPerSecond * spriteCount;
            float waitTime = spaceTime * times;
            _doubleMark.SetActive(true);
            _doubleMark.transform.localPosition = new Vector3(130, 56, 0);
            _doubleMark.GetComponent<UISprite>().alpha = 1;
            TweenAlpha.Begin(_doubleMark, waitTime, 0);
            TweenAlpha ta = GetComponent<TweenAlpha>() ?? gameObject.AddComponent<TweenAlpha>();
            ta.from = 1;
            ta.to = 0;
            ta.delay = waitTime;
            ta.duration = waitTime / 1.5f;
            ta.ResetToBeginning();
            ta.PlayForward();

            TweenPosition tp = GetComponent<TweenPosition>() ?? gameObject.AddComponent<TweenPosition>();
            tp.delay = waitTime;
            tp.duration = waitTime / 1.5f;
            tp.from = new Vector3(130, 56, 0);
            tp.to = _targetPos;
            tp.ResetToBeginning();
            tp.PlayForward();
            
            //TweenPosition.Begin(_doubleMark, waitTime ,_targetPos);

            Invoke("HideEffect", spaceTime);
        }
   
        /// <summary>
        /// 隐藏特效
        /// </summary>
        public void HideEffect()
        {
            CancelInvoke("HideEffect");
            _sprAnim.gameObject.SetActive(false);
        }


        /// <summary>
        /// 隐藏双倍标记和动画
        /// </summary>
        public void HideDoubleMark()
        {
            HideEffect();
            gameObject.SetActive(false);
        }

    }
}