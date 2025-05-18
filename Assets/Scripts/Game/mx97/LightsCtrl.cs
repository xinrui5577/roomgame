using UnityEngine;
using System.Collections;
using System;

namespace Assets.Scripts.Game.mx97
{
    public class LightsCtrl : MonoBehaviour
    {
        public GameObject[] smallLights;
        public GameObject[] bigLights;

        float winNextShakeTime;
        public float winRate = 0.5f;

        float idleNextShakeTime;
        public float idleRate = 0.5f;

        int bigLightsIdx = 0;

        public enum StatusL
        {
            win,
            idle
        }
        public StatusL lightStatus = StatusL.idle;

        private static LightsCtrl instance;
        public static LightsCtrl GetInstance()
        {
            return instance;
        }

        void Awake()
        {
            instance = this;
        }

        // Use this for initialization
        void Start()
        {
            winNextShakeTime -= winRate;
            idleNextShakeTime -= idleRate;
        }

        // Update is called once per frame
        void Update()
        {
            if (lightStatus == StatusL.idle)
                IdleLightsBlink();
            else if (lightStatus == StatusL.win)
                WinLightsBlink();
        }

        void WinLightsBlink()
        {
            if (Time.time - winNextShakeTime > winRate)
            {
                winNextShakeTime = Time.time;

                foreach (var item in smallLights)
                {
                    item.SetActive(!item.activeSelf);
                }
                foreach (var item in bigLights)
                {
                    item.SetActive(!item.activeSelf);
                }
            }
        }

        void IdleLightsBlink()
        {
            if (Time.time - idleNextShakeTime > idleRate)
            {
                idleNextShakeTime = Time.time;

                while (bigLightsIdx < bigLights.Length)
                {
                    foreach (var item in bigLights)
                        item.gameObject.SetActive(false);

                    bigLights[bigLightsIdx++].SetActive(true);
                    bigLights[bigLightsIdx++].SetActive(true);

                    if (bigLightsIdx == bigLights.Length)
                        bigLightsIdx = 0;

                    return;
                }
            }
        }

        //外部更改灯状态
        public void ChangeLightStatus(StatusL status)
        {
            foreach (var item in bigLights)
            {
                item.SetActive(false);
            }
            foreach (var item in smallLights)
            {
                item.SetActive(true);
            }

            lightStatus = status;
        }
    }
}