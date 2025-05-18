using Assets.Scripts.Game.lyzz2d.Utils.Single;
using UnityEngine;

namespace Assets.Scripts.Game.lyzz2d.Game.Component
{
    /// <summary>
    ///     每位玩家计时使用
    /// </summary>
    public class CountDownGo : MonoSingleton<CountDownGo>
    {
        private float _elapsedSeconds;

        private int _seconds;

        [SerializeField] private UILabel _showLabel;

        // Use this for initialization
        private int currentSecond;

        // Update is called once per frame
        protected void LateUpdate()
        {
            if (enabled)
            {
                if (_elapsedSeconds >= _seconds)
                {
                    Stop();
                    _elapsedSeconds = _seconds;
                }
                _elapsedSeconds += Time.deltaTime;
                var seconds = Mathf.FloorToInt(_seconds - _elapsedSeconds);
                FreshNumber(seconds);
            }
        }

        public void Begin(int seconds)
        {
            _elapsedSeconds = 0;
            _seconds = seconds;
            FreshNumber(_seconds);
            enabled = true;
        }

        public void FreshNumber(int seconds)
        {
            if (currentSecond != seconds)
            {
                currentSecond = seconds;
                if (currentSecond <= 0)
                {
                    currentSecond = 0;
                }
                _showLabel.text = currentSecond.ToString().PadLeft(2, '0');
            }
        }

        public void Stop(bool isResult = false)
        {
            if (isResult)
            {
                FreshNumber(0);
            }
            enabled = false;
        }
    }
}