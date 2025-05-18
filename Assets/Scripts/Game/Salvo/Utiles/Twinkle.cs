using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.Salvo.Utiles
{
    public class Twinkle : MonoBehaviour
    {
        private GameObject _object;
        public float ShowTime = 0.5f;
        public float HideTime = 0.5f;
        public bool IsRunAwake;
        public GameObject Target;
         
        public void Play(GameObject target)
        {
            Play(target,0); 
        }

        public void Play(GameObject target,int count ,Action Onfinish=null)
        {
            var temp = _object;
            _object = target;
            if (temp == null) StartCoroutine(Twinkling(count,Onfinish));
            else temp.SetActive(true);
        }

        public void Stop()
        {
            if (_object == null) return;
            _object.SetActive(true);
            _object = null;
        }

        private IEnumerator Twinkling(int count=0, Action Onfinish = null)
        {
            var tempCount = 0;
            var isShow = false;
            var maxCount = count*2;
            while (_object != null)
            { 
                isShow = !isShow;
                var waitTime = isShow ? ShowTime : HideTime;
                _object.SetActive(isShow);
                tempCount++;
                yield return new WaitForSeconds(waitTime);
                if (maxCount>0) if (tempCount > maxCount) yield break;
            }
            if (Onfinish != null) Onfinish();
        }

        private void OnDisable()
        {
            _object = null;
        }

        private void OnEnable()
        {
            if (!IsRunAwake) return;
            if (Target == null) return;
            Play(Target);
        }
    }
}
