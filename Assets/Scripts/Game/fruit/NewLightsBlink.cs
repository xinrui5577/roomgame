/*
 * 時間：2018年7月10日09:51:57
 * 功能：事件操控两圈灯泡闪烁
 */

using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

namespace Assets.Scripts.Game.fruit
{
    public class NewLightsBlink : MonoBehaviour
    {
        //存放大灯和小灯
        public GameObject[] BigLights;
        public GameObject[] SmallLights;

        //灯的状态
        public enum newLightsStatus
        {
            idle,
            horseLamp,
            run,
            end,
            win
        }

        public newLightsStatus m_lightStatus = newLightsStatus.idle;
        private static newLightsStatus lastStatus; //监测状态切换调复位函数

        private static NewLightsBlink instance;
        public static NewLightsBlink getInstance()
        {
            return instance;
        }

        void Awake()
        {
            instance = this;
            currentTime = Time.time;

            lastStatus = m_lightStatus;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            currentTime = Time.time;

            if (currentTime - bigLightsUpdate > 0.3f)
            {
                bigLightsBlink();
                bigLightsUpdate = Time.time;
            }

            if (currentTime - smallLightsUpdate > .1f)
            {
                smallLightsBlink();
                smallLightsUpdate = Time.time;
            }
        }


        #region swapLightStatus

        public void onWinLights(object obj, EventArgs eventArgs)
        {
            m_lightStatus = newLightsStatus.win;

            if (m_lightStatus != lastStatus)
            {
                this.BigLightsReset();
                lastStatus = m_lightStatus;
            }
        }

        public void onLoseLights(object obj, EventArgs eventArgs)
        {
            m_lightStatus = newLightsStatus.idle;

            if (m_lightStatus != lastStatus)
            {
                this.BigLightsReset();
                lastStatus = m_lightStatus;
            }
        }

        public void onIdleLights(object obj, EventArgs eventArgs)
        {
            m_lightStatus = newLightsStatus.idle;

            if (m_lightStatus != lastStatus)
            {
                this.BigLightsReset();
                this.SmallLightsReset();
                lastStatus = m_lightStatus;
            }
        }

        public void onRunLights(object obj, EventArgs eventArgs)
        {
            //Debug.Log("onRunLights()");
            m_lightStatus = newLightsStatus.run;

            if (m_lightStatus != lastStatus)
            {
                this.SmallLightsReset();
                lastStatus = m_lightStatus;
            }
        }

        public void onEndLights(object obj, EventArgs eventArgs)
        {
            Invoke("delayOnEndLights", 1.5f);
            //Debug.Log("onRunLights()");    
        }

        private void delayOnEndLights()
        {
            m_lightStatus = newLightsStatus.end;

            if (m_lightStatus != lastStatus)
            {
                this.SmallLightsReset();
                lastStatus = m_lightStatus;
            }
        }

        //public void onHorseLights(object obj, EventArgs eventArgs)
        //{
        //    m_lightStatus = newLightsStatus.horseLamp;

        //    if (m_lightStatus != lastStatus)
        //    {
        //        this.BigLightsReset();

        //        lastStatus = m_lightStatus;
        //    }
        //}

        #endregion

        private float currentTime;
        private float bigLightsUpdate;
        private float smallLightsUpdate;

        int bigLightsIdx = 0;

        private void bigLightsBlink()
        {
            switch (m_lightStatus)
            {
                case newLightsStatus.win:
                    {
                        foreach (var item in BigLights)
                        {
                            if (item.activeSelf)
                            {
                                item.SetActive(false);
                            }
                            else
                            {
                                item.SetActive(true);
                            }

                        }
                        break;
                    }

                default:
                    {
                        while (bigLightsIdx < BigLights.Length)
                        {
                            foreach (var item in BigLights)
                                item.gameObject.SetActive(false);

                            BigLights[bigLightsIdx++].SetActive(true);
                            BigLights[bigLightsIdx++].SetActive(true);

                            if (bigLightsIdx == BigLights.Length)
                                bigLightsIdx = 0;

                            return;
                        }
                        break;
                    }
            }
        }

        private static float smallLightAlpah = 0;
        private static int smallLightIdx = 0;
        private void smallLightsBlink()
        {
            switch (m_lightStatus)
            {
                case newLightsStatus.win:
                    {
                        foreach (var item in SmallLights)
                        {
                            if (item.activeSelf)
                            {
                                item.SetActive(false);
                            }
                            else
                            {
                                item.SetActive(true);
                            }
                        }
                        break;
                    }
                case newLightsStatus.run:
                    {
                        //while (smallLightIdx < SmallLights.Length)
                        //{
                        //    SmallLights[smallLightIdx].GetComponent<Image>().color = new Color(255, 255, 255, smallLightAlpah += Time.deltaTime);
                        //    smallLightIdx++;

                        //    if (smallLightIdx == SmallLights.Length)
                        //        smallLightIdx = 0;
                        //    return;
                        //}

                        break;
                    }
                default:
                    {
                        SmallLightsReset();
                        break;
                    }
            }
        }

        //灯泡复位
        private void BigLightsReset()
        {
            foreach (var item in BigLights)
                item.SetActive(false);
        }

        private void SmallLightsReset()
        {
            foreach (var item in SmallLights)
            {
                item.SetActive(true);
                item.GetComponent<Image>().color = new Color(255, 255, 255, 1);
            }
            smallLightIdx = 0;
            smallLightAlpah = 0;
        }

        public GameObject[] fruitBtnFrameLight;
        //压中之后闪烁整体按钮边框闪烁
        public void CtrlBtnFrameShake(object obj, EventArgs eventArgs)
        {
            if (SlotInfo.GoodLuckPoints != null && SlotInfo.GoodLuckPoints.Count > 0)
            {
                int i = 0;
                while (i < SlotInfo.GoodLuckPoints.Count)
                {
                    var fruitType = SlotInfo.GetSlotInfo().pointToOddsList[SlotInfo.GoodLuckPoints[i]].FruitType;
                    Debug.Log(fruitType);

                    if (SlotInfo.FruitSlotList[fruitType] > 0)
                    {
                        fruitBtnFrameLight[(int)fruitType].SetActive(true);
                        Invoke("ShutDownShake", 3);
                    }

                    i++;

                    if (i == SlotInfo.GoodLuckPoints.Count)  //不要在lucky后影响单独的水果被选中
                        SlotInfo.GoodLuckPoints.Clear();                  
                }
            }
            else if (SlotInfo.FruitSlotList[SlotInfo.GetSlotInfo().pointToOddsList[SlotInfo.LotteryPoint].FruitType] > 0)
            {
                //blink
                var tempType = (int)SlotInfo.GetSlotInfo().pointToOddsList[SlotInfo.LotteryPoint].FruitType;
                fruitBtnFrameLight[tempType].SetActive(true);
                Invoke("ShutDownShake", 3);
            }
        }

        public void ShutDownShake()
        {
            foreach (var item in fruitBtnFrameLight)
            {
                item.SetActive(false);
            }         
        }

    }

}
