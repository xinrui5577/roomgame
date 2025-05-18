using UnityEngine;

namespace Assets.Scripts.Common.UI
{
    /// <inheritdoc />
    /// <summary>
    /// 按钮,支持按住
    /// </summary>
    public class NguiLongBtn : UIButton
    {
        //长按事件处理
        public float Interval = .03f;

        public float LongPressStartTime = 0.5f;
        private int _isPressed;

        protected override void OnPress(bool isPress)
        {
            base.OnPress(isPress);
            if (isPress)
            {
                _isPressed = 1;
                _lastClickTime = 0;
            }
            else
            {
                _lastClickTime = 0;
                _isPressed = 0;
            }
        }

        private float _lastClickTime = 0;
        private float _lastEventTime = 0;
        void Update()
        {
            if (_isPressed < 1) return;
            switch (_isPressed)
            {
                case 1:
                    _lastClickTime += Time.deltaTime;
                    if (_lastClickTime > LongPressStartTime)
                    {
                        //长按
                        _isPressed = 2;
                    }
                    return;
                case 2:
                    _lastEventTime += Time.deltaTime;
                    if (_lastEventTime > Interval)
                    {
                        _lastEventTime = 0;
                        _isPressed = 3;
                    }
                    return;
                case 3:
                    LongPressedFunc();
                    _lastEventTime = 0;
                    _isPressed = 2;
                    return;
                default:
                    return;

            }
        }

        void LongPressedFunc()
        {
            EventDelegate.Execute(onClick);
        }
    }
}
