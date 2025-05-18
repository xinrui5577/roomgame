using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Common.MahJongCommon
{
    /// <summary>
    /// 每位玩家计时使用
    /// </summary>
    public class CountDownGo : MonoSingleton<CountDownGo>
    {
        [Tooltip("显示文本（剩余时间）")]
        public UILabel ShowLabel;
        [Tooltip("默认倒计时时间")]
        public int CountDownTime = 15;
        private int _currentSecond;
        protected void LateUpdate ()
        {
            if (enabled)
            {
                if (_elapsedSeconds >= _seconds)
                {
                    Stop();
                    _elapsedSeconds = _seconds;
                }
                _elapsedSeconds += Time.deltaTime;
                int seconds = Mathf.FloorToInt(_seconds - _elapsedSeconds);
                FreshNumber(seconds);
            }
        }

        private int _seconds;
        private float _elapsedSeconds;
        public void Begin()
        {
            _elapsedSeconds = 0;
            _seconds = CountDownTime;
            FreshNumber(_seconds);
            enabled = true;
        }

        public void FreshNumber(int seconds) {
            if (_currentSecond != seconds) {
                _currentSecond = seconds;
                if (_currentSecond <= 0)
                {
                    _currentSecond = 0;
                }
                ShowLabel.TrySetComponentValue(_currentSecond.ToString().PadLeft(2, '0'));
            }
        }

        public void Stop(bool isResult=false)
        {
            if (isResult)
            {
                FreshNumber(0);
            }
            enabled = false;
        }
    }
}
