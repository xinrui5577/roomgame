using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Effect
{
    public class FlashEffect : MonoBehaviour
    { 
        private enum State
        {
            Start,
            Extend,
            ExtendEnd,
            Waiting,
            Dissolving,
            Stop
        }

        /// <summary>
        /// 伸展总时间
        /// </summary>
        public float MaxExtendTime = 2;
        /// <summary>
        /// 渐隐总时间
        /// </summary>
        public float MaxColorTime = 1;
        /// <summary>
        /// 停留总时间
        /// </summary>
        public float MaxWaitTime = 0.5f;
        /// <summary>
        /// 开始颜色
        /// </summary>
        public Color StartColor = new Color(1, 1, 1, 1);
        /// <summary>
        /// 结束颜色
        /// </summary>
        public Color EndColor = new Color(0, 0, 0, 0);    
        /// <summary>
        /// 闪电样式
        /// </summary>
        public tk2dClippedSprite FlashSprite;
        /// <summary>
        /// 激光头
        /// </summary>
        public tk2dSpriteAnimator ThunderLight; 
        /// <summary>
        /// 激光播放效果大小
        /// </summary>
        public float ThunderLightSize = 0.1f;
        /// <summary>
        /// 激光播放效果大小比率
        /// </summary>
        public float ThunderLightSizeRate;
     
        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private State _state = State.Stop; 
        private float _passTime = 0;  
        private tk2dSprite _thunderLightSprite; 
        private float _spriteWidth;

        void Start()
        {
            var rect = FlashSprite.ClipRect;
            rect.width = 0;
            FlashSprite.ClipRect = rect;
            _thunderLightSprite = ThunderLight.GetComponent<tk2dSprite>();
        }

        /// <summary>
        /// 播放特效
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="targetPosition"></param>
        public void Play(Vector3 startPosition, Vector3 targetPosition)
        {
            _startPosition = startPosition;
            _targetPosition = targetPosition; 
            _state = State.Start;
        }

        // Update is called once per frame
        void Update ()
        {
            switch (_state)
            {
                case State.Start:
                    OnStart();
                    break;
                case State.Extend:  //伸展
                    OnExtend();
                    break;
                case State.ExtendEnd://结束伸展
                    OnExtendEnd();
                    break;
                case State.Waiting: //等待
                    OnWaiting();
                    break;
                case State.Dissolving: //渐隐
                    OnDissolving();
                    break; 
            } 
        }
     
        /// <summary>
        /// 开始准备
        /// </summary>
        private void OnStart()
        { 
            transform.position = _startPosition; 
            if (_startPosition == _targetPosition)
            {
                _state = State.ExtendEnd;
                MaxWaitTime += MaxExtendTime;
                return;
            } 
            var bound = FlashSprite.GetBounds();
            _spriteWidth = bound.size.x; 
            _state = State.Extend; 
            var lookDirect = _targetPosition - _startPosition;   
            transform.rotation = Quaternion.LookRotation(Vector3.forward, lookDirect); 
        }

        /// <summary>
        /// 伸展期
        /// </summary>
        private void OnExtend()
        {    
            _passTime += Time.deltaTime;
            if (_passTime > MaxExtendTime)
            {
                _passTime = MaxExtendTime;
                _state = State.ExtendEnd;
            }

            var lerp = Vector3.Lerp(_startPosition, _targetPosition, _passTime / MaxExtendTime);
            //if (lerp == _targetPosition) _state = State.ExtendEnd;
            FlashExtend(lerp);  
            lerp.z -= 1;
            ThunderLight.transform.position = lerp;
        }

        private void OnExtendEnd()
        {
            _passTime = 0;
            _state = State.Waiting; 
            var size = ThunderLightSize*ThunderLightSizeRate;
            _thunderLightSprite.scale = new Vector3(size, size);
            ThunderLight.Play();
        }

        private void OnWaiting()
        {
            _passTime += Time.deltaTime;
            if (_passTime > MaxWaitTime)
            {
                _passTime = 0;
                _state = State.Dissolving;
            } 
        }

        /// <summary>
        /// 渐隐期
        /// </summary>
        private void OnDissolving()
        {
            _passTime += Time.deltaTime;
            if (_passTime > MaxColorTime) _passTime = MaxColorTime;
            var color = Color.Lerp(StartColor, EndColor, _passTime / MaxColorTime);
            if (color == EndColor)
            {
                Destroy(gameObject);
                return;
            }
            FlashSprite.color = color;
            _thunderLightSprite.color = color;    
        }

        private void FlashExtend(Vector3 curPos)
        {
            var dist = Vector3.Distance(_startPosition, curPos); 
            var rate = dist / _spriteWidth;
            if (dist > _spriteWidth)
            {
                var scale = FlashSprite.scale;
                scale.x = rate;
                FlashSprite.scale = scale;
            }
            else
            { 
                var rect = FlashSprite.ClipRect;
                rect.width = rate;
                FlashSprite.ClipRect = rect;
            }
        } 
    }
} 