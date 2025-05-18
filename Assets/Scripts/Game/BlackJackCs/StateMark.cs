using UnityEngine;

namespace Assets.Scripts.Game.BlackJackCs
{
    public class StateMark : MonoBehaviour
    {
      
        protected void OnDisable()
        {
            _playing = false;
        }

        public float BlackjackTime = 0.2f;

        private bool _playing;
        /// <summary>
        /// 显示黑杰克
        /// </summary>
        public void ShowBalckJackMark()
        {

            if (_playing)
                return;
            _playing = true;

            UISprite stateSprite = GetComponent<UISprite>();
            stateSprite.spriteName = "blackjack";
            stateSprite.alpha = 0;
            stateSprite.MakePixelPerfect();
            stateSprite.transform.localScale = Vector3.one * 16;
            gameObject.SetActive(true);


            //设置游戏动画
            TweenScale ts = GetComponent<TweenScale>() ?? gameObject.AddComponent<TweenScale>();
            ts.from = Vector3.one * 4;
            ts.to = Vector3.one;
            ts.duration = BlackjackTime;
            ts.ResetToBeginning();
            ts.PlayForward();

            TweenPosition tp = GetComponent<TweenPosition>() ?? gameObject.AddComponent<TweenPosition>();
            tp.from = new Vector3(0,480,0);
            tp.to = new Vector3(0, 140, 0);
            tp.duration = BlackjackTime;
            tp.ResetToBeginning();
            tp.PlayForward();

            TweenAlpha.Begin(gameObject, BlackjackTime, 1);
        }
        
        /// <summary>
        /// 显示输牌标记
        /// </summary>
        public void ShowLostMark()
        {

            if (_playing)
                return;
            _playing = true;

            UISprite stateSprite = GetComponent<UISprite>();
            stateSprite.alpha = 1;
            stateSprite.spriteName = "lost";
            stateSprite.MakePixelPerfect();
            transform.localScale = Vector3.zero;
            gameObject.SetActive(true);

            //设置显示动画
            TweenScale ts = GetComponent<TweenScale>() ?? gameObject.AddComponent<TweenScale>();
            ts.from = Vector3.zero;
            ts.to = Vector3.one;
            ts.duration = 0.3f;
            ts.ResetToBeginning();
            ts.PlayForward();
        }

        /// <summary>
        /// 隐藏输牌标记
        /// </summary>
        public void HideLostMark()
        {

            //设置隐藏动画
            TweenScale ts = GetComponent<TweenScale>() ?? gameObject.AddComponent<TweenScale>();
            ts.from = Vector3.one;
            ts.to = Vector3.zero;
            ts.duration = 0.3f;
            ts.ResetToBeginning();
            ts.PlayForward();

            _playing = false;
        }

    }
}