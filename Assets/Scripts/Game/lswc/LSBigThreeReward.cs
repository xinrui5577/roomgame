using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.lswc.Core;
using Assets.Scripts.Game.lswc.Data;
using UnityEngine;

namespace Assets.Scripts.Game.lswc
{
    /// <summary>
    /// 转盘结束时，大三元的处理
    /// </summary>
    public class LSBigThreeReward :InstanceControl
    {
        private Animation _curAnimation;

        private Transform _lion;

        private Transform _monkey;

        private Transform _rabbit;

        private Transform _panda;

        private string _playeAnimationState = "3";

        private Transform _curTrans;

        private void Start()
        {
           Find();
            _curTrans = _lion;
        }

        private void Find()
        {
            _lion = transform.GetChild(0);
            _monkey = transform.GetChild(1);
            _panda = transform.GetChild(2);
            _rabbit = transform.GetChild(3);
        }

        public void HideAll()
        {
           _lion.gameObject.SetActive(false);
           _panda.gameObject.SetActive(false);
           _rabbit.gameObject.SetActive(false);
           _monkey.gameObject.SetActive(false);
        }


        public void PlayAnimation(int time)
        {
            StartCoroutine(PlayAnimations(time));          
        }

        private IEnumerator PlayAnimations(int times)
        {
            for (int i = 0; i <times; i++)
            {
                while (_curAnimation.isPlaying)
                {
                    yield return new WaitForEndOfFrame();
                }
                _curAnimation.Play(_playeAnimationState);
            }
        }

        public void SetCurrentAnimal(LSAnimalType type)
        {
            _curTrans.gameObject.SetActive(false);
            switch (type)
            {
                    case LSAnimalType.GOLD_HZ:
                    case LSAnimalType.HZ:
                    _curTrans = _monkey;
                    break;
                    case LSAnimalType.XM:
                    case LSAnimalType.GOLD_XM:
                    _curTrans = _panda;
                    break;
                    case LSAnimalType.SZ:
                    case LSAnimalType.GOLD_SZ:
                    _curTrans = _lion;
                    break;
                    case LSAnimalType.TZ:
                    case LSAnimalType.GOLD_TZ:
                    _curTrans = _rabbit;
                    break;
            }
            _curAnimation = _curTrans.GetComponent<Animation>();
            _curTrans.gameObject.SetActive(true);
        }

        public override void OnExit()
        {
            
        }
    }
}
