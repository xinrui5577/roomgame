

using System;
using Assets.Scripts.Game.Fishing.commons;
using Assets.Scripts.Game.Fishing.datas;
using Assets.Scripts.Game.Fishing.enums;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Game.Fishing.entitys
{
    public class Fish : MonoBehaviour
    {
        /// <summary>
        /// 锁定位置
        /// </summary>
        public Vector2 LockSpace;

        public Renderer SkinRenderer;
        public FishData Data;
        public Swimmer TheSwimmer; 
        /// <summary>
        /// 状态机
        /// </summary>
        public Animator TheAnimator;

        /// <summary>
        /// 被打颜色
        /// </summary>
        public Color HitColor = Color.red;


        public string DieActionName = "die";
        public string SpecialSwimActionName;
        public string BeHitActionName;
        /// <summary>
        /// 能力
        /// </summary>
        public EFishAbilityType AbilityType;

        public string[] EnterSpeaks;
        public string[] RandomSpeaks;
        public string[] DieSpeaks;

        void Awake()
        { 
            if (TheSwimmer == null)
            {
                TheSwimmer = GetComponent<Swimmer>();
            }
            _speakTime = Random.Range(10, 30);
        }

        void Start()
        {
            if (EnterSpeaks.Length > 0)
            {
                var index = DateTime.Now.Second % (EnterSpeaks.Length * 10);
                var speak = EnterSpeaks.GetElement(index);
                if (!string.IsNullOrEmpty(speak))
                {
                    Facade.Instance<MusicManager>().Play(speak);
                }
            }
        }

        public bool Availability {
            get
            {
                return TheSwimmer != null && TheSwimmer.enabled;
            }
        }

        public void Die()
        { 
            if (AbilityType == EFishAbilityType.Immortal)
            {
                ChangeHitAction();
            }
            else
            {
                ChangeDieAction();
            } 

            if (AbilityType != EFishAbilityType.Immortal)
            {
                Facade.EventCenter.DispatchEvent(EFishingEventType.ReleaseArt, this);
                ToDie();
            }
        }

        public void DieSpeak()
        {
            var dieLen = DieSpeaks.Length;
            if (dieLen > 0)
            {
                var dieIndex = DateTime.Now.Second % dieLen;
                var dieSpek = DieSpeaks.GetElement(dieIndex);
                if (!string.IsNullOrEmpty(dieSpek))
                {
                    Facade.Instance<MusicManager>().Play(dieSpek);
                }
            }
        }

        public void ChangeAction()
        {
            if (TheAnimator != null && !string.IsNullOrEmpty(SpecialSwimActionName))
            {
//                TheAnimator.Play(SpecialSwimActionName);
                TheAnimator.CrossFade(SpecialSwimActionName, 0.2f);
            }
        }

        public void ChangeHitAction()
        {
            if (TheAnimator != null && !string.IsNullOrEmpty(BeHitActionName))
            {
//                TheAnimator.Play(BeHitActionName);
                TheAnimator.CrossFade(BeHitActionName, 0.2f);
            }
        }
        public void ChangeDieAction()
        {
            if (TheAnimator != null && !string.IsNullOrEmpty(DieActionName))
            {
//                TheAnimator.Play(DieActionName);
                TheAnimator.CrossFade(DieActionName, 0.2f);
            }
        }

        private void ToDie()
        {
            if (TheSwimmer != null)
            {
                TheSwimmer.enabled = false;
            }
            _actionTime = 2;
            _actionType = EFishActionType.Die;
        }

        public void BeHit()
        {
            //染红
            _actionTime = 0.1f;
            SetRed(); 
            _actionType = EFishActionType.Hit;
        }

        public void SetRed()
        {
            SkinRenderer.material.color = Color.red;
        }

        private float _actionTime;

        private EFishActionType _actionType;
        void Update()
        {
            switch (_actionType)
            {
                case EFishActionType.Hit:
                    BeHitAction();
                    break;
                case EFishActionType.Die:
                    DieAction();
                    break;
            }
            RandomSpeak();
        }

        private float _speakTime;
        private void RandomSpeak()
        {
            _speakTime -= Time.deltaTime;
            if (_speakTime > 0) return;
            var dieLen = RandomSpeaks.Length;
            if (dieLen > 0)
            {
                var index = DateTime.Now.Second % (dieLen * 3);
                var speak = RandomSpeaks.GetElement(index);
                if (!string.IsNullOrEmpty(speak))
                {
                    Facade.Instance<MusicManager>().Play(speak);
                }
            }
            _speakTime = Random.Range(8, 20);
        }

        private void DieAction()
        {
            //染红
            _actionTime -= Time.deltaTime;
            var mat = SkinRenderer.material;
            var old = mat.color;
            old.a = _actionTime;
            mat.color = old;
            if (_actionTime < 0)
            {
                _actionType = EFishActionType.Normal;
                Destroy(gameObject);
            }
        }

        private void BeHitAction()
        {
            //染红
            _actionTime -= Time.deltaTime;
            if (_actionTime < 0)
            {
                SkinRenderer.material.color = Color.white;
                _actionType = EFishActionType.Normal;
            }

        }

        enum EFishActionType
        {
            Normal,
            Hit,
            Die
        }
    }
}
