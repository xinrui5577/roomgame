using System;
using System.Collections;
using UnityEngine;

/*===================================================
 *文件名称:     PlayParti.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2019-01-15
 *描述:        	播放特效View
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo.View
{
    public class PlayParticleView : MonoBehaviour 
    {
        #region UI Param
        [Tooltip("特效")]
        public ParticleSystem Particle;


        #endregion

        #region Data Param
        [Tooltip("播放完毕隐藏")]
        public bool FinishHide = true;

        [Tooltip("播放放完毕等待时间")]
        public float PlayWaitTime =0;

        [Tooltip("播放放完毕等待时间")]
        public float HideWaitTime = 3;
        #endregion

        #region Local Data

        #endregion

        #region Life Cycle

        void Awake()
        {
            if (Particle==null)
            {
                Particle = GetComponent<ParticleSystem>();
            }
        }

        #endregion

        #region Function

        private Coroutine _waitCor;
        public void Play()
        {
            Wait(PlayWaitTime, PlayParticlc);
        }

        private void PlayParticlc(object data)
        {
            if (Particle)
            {
                Particle.Stop();
                Particle.Play();
                if (FinishHide)
                {
                    Wait(HideWaitTime, delegate
                    {
                        Particle.gameObject.SetActive(false);
                    });
                }
            }
        }

        private void Wait(float waitTime, AsyncCallback finishCall)
        {
            if (_waitCor != null)
            {
                StopCoroutine(_waitCor);
            }
            _waitCor = StartCoroutine(WaitForTime(waitTime, finishCall));
        }

        IEnumerator WaitForTime(float time, AsyncCallback finishCall)
        {
            while (time >ConstantData.IntValue)
            {
                var delaTime = Time.deltaTime;
                time -= delaTime;
                yield return new WaitForSeconds(delaTime);
            }
            if (finishCall!=null)
            {
                finishCall(null);
            }
        }
        #endregion
    }
}
