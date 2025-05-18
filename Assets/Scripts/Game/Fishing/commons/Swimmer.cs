using System;
using Assets.Scripts.Game.Fishing.entitys;
using Assets.Scripts.Game.Fishing.enums;
using UnityEngine;
using YxFramwork.Framework.Core;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Game.Fishing.commons
{
    /// <summary>
    /// 游物
    /// </summary>
    public class Swimmer : MonoBehaviour
    {
        public float Speed = 1F;
        public Transform Body;
        public long Path;
        public float Angles;
        public Rect LiveArea;

        public float MinLiveTime = 3;
        private float _radius;
        private Transform _transform;

        public CapsuleCollider Collider;

        private Fish _fish;//todo 优化base类

        void Awake()
        {
            Random.InitState((int)System.DateTime.Now.Ticks);
            _fish = GetComponent<Fish>();
            _transform = transform;
            InitSize();
            Facade.EventCenter.AddEventListeners<EFishingEventType,ESwimmerState>(EFishingEventType.SwimmerState,ChangeState);
        }

        public Transform CurTransform
        {
            get { return _transform; }
        }

        private void InitSize()
        {
            _radius = Collider.height / 2 ;
        }

        public float Radius
        {
            get
            {
                return _radius;
            }
        }

        void Start()
        {
            InitDirection();
        }
         
        /// <summary>
        /// 初始化方向
        /// </summary>
        private void InitDirection()
        {
            var bodyAngles = Body.localEulerAngles;
            bodyAngles.y = -Angles;
            Body.localEulerAngles = bodyAngles;
        }

        private void ChangeState(ESwimmerState type)
        {
            switch (type)
            {
                case ESwimmerState.Fixed:
                    Fixed();
                    break;
                case ESwimmerState.Swim:
                    Swim();
                    break;
                case ESwimmerState.RunAway:
                    RunAway();
                    break;
            }
        }

        private float _runAwaySpeed=1 ;
        /// <summary>
        /// 逃跑
        /// </summary>
        public void RunAway()
        {
            Path = 0;
            _runAwaySpeed = 5;
            Random.InitState((int)DateTime.Now.Ticks);
            Angles = Random.Range(135, 225);
            ChangeDirection();
        }

        /// <summary>
        /// 停止游动
        /// </summary>
        public void Fixed()
        {
            IsFixed = true;
        }

        /// <summary>
        /// 开始游动
        /// </summary>
        public void Swim()
        {
            IsFixed = false;
        }

        private float _lastTime = 1;
        private int _index;
        private float _anglesOff;
        void Update()
        {
            //1秒取个方向值
            var deltaTime = Time.deltaTime;
            _lastTime += deltaTime;
            if (_lastTime >= 1f)
            {
                var lastTime = (int)_lastTime;
                _lastTime = _lastTime - lastTime;
                //朝着这个方向移动
                var newf = Path >> (_index * 2);
                var dir = (newf & 3) % 4;//方向
                if (_index < 32)
                {
                    _index++;
                    if (dir > 0)
                    {
                        switch (dir)
                        {
                            case 1: //左
                                _anglesOff = 30;
                                break;
                            case 2: //右
                                _anglesOff = -30;
                                break;
                            case 3: //动作
                                if (_fish != null)
                                {
                                    _fish.ChangeAction();
                                }
                                break;
                            default:
                                _anglesOff = 0;
                                break;
                        }
                    }
                    else
                    {
                        _anglesOff = 0;
                    }
                }
                else
                {
                    _anglesOff = 0;
                }
            }
            if (_anglesOff>0 || _anglesOff<0)
            {
                ChangeDirection();
            }
            Swimming();
            CheckOut();
        }

        public void ChangeDirection()
        {
            Angles += _anglesOff * Time.deltaTime;
            var bodyAngles = Body.localEulerAngles;
            bodyAngles.y = -Angles;
            Body.localEulerAngles = bodyAngles;
            var colliderTs = Collider.transform;
            var f = colliderTs.localEulerAngles;
            f.z = Angles;
            colliderTs.localEulerAngles = f;


        }

        public bool IsValid { get; private set; }

        private void CheckOut()
        { 
            if (IsValid)
            {
                if (!LiveArea.Contains(_transform.localPosition,true))
                {
                    if (RemoveEvent != null)
                    {
                        var fishData = _fish.Data;
                        RemoveEvent(fishData.FishId,fishData.Type);
                    }
                    Destroy(gameObject);
                }
            }
            else
            {
                if (LiveArea.Contains(_transform.localPosition, true))
                {
                    IsValid = true;
                }
            }
        }
         
        public bool IsFixed { get; private set; }

        public Action<int,EFishType> RemoveEvent;

        private void Swimming()
        {
            if (IsFixed) return;
            var a1 = Angles * Mathf.Deg2Rad;
            var dir = Vector3.one;
            dir.x = Mathf.Cos(a1);
            dir.y = Mathf.Sin(a1);
            dir.z = 0;
            _transform.localPosition += Time.deltaTime * Speed * _runAwaySpeed * dir;
        }

        public void AddFish(Fish fish)
        {
        }


        void OnDestroy()
        {
            Facade.EventCenter.RemoveEventListener<EFishingEventType, ESwimmerState>(EFishingEventType.SwimmerState, ChangeState);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            //            Gizmos.DrawWireSphere(transform.position, _boxCollider.radius * 0.003125f);
            InitSize();
            Gizmos.DrawWireSphere(transform.position, Radius);
        } 
    }
}
