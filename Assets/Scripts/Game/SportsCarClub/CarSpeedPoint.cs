/*
 * 时间：2018年8月11日09:36:15
 * 功能：汽车仪表指针=>变化跟随音效
 */

using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Game.SportsCarClub
{
    public class CarSpeedPoint : MonoBehaviour
    {
        public float rotatedSpeed = 0;

        //指针加速度&减速度
        public float acceleration = .2f;
        public float deceleration = .2f;

        //仪表指针极限值
        public float maxLimit = 200;
        public float minLimit = 0;

        public Transform ownRotatedTrans;

        //加速时指针抖动那一下
        public bool allowShake = true;

        public enum Status
        {
            speedUp,
            speedDown,
            stop
        }

        #region instance单例
        private static CarSpeedPoint instance;
        public static CarSpeedPoint GetInstance()
        {
            return instance;
        }
        #endregion

        public Status CarPointStatus;

        // Use this for initialization
        void Start()
        {
            instance = this;
            previous = Status.stop;
        }

        // Update is called once per frame
        void Update()
        {
            CarPointChangedPerFrame();
            OnStatusChanged();
            OwnRotated();

            if (allowShake && CarPointStatus == Status.speedUp)
                StartCoroutine(OnSpeedUpShake());
        }

        public void CarPointChangedPerFrame()
        {
            switch (CarPointStatus)
            {
                case Status.stop:
                    {
                        rotatedSpeed = 0;
                        break;
                    }
                case Status.speedUp:
                    {
                        rotatedSpeed += acceleration;
                        break;
                    }
                case Status.speedDown:
                    {
                        rotatedSpeed -= deceleration;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        //之前的状态
        Status previous;
        private void OnStatusChanged()
        {
            if (previous != CarPointStatus)
            {
                rotatedSpeed = 0;
                previous = CarPointStatus;
            }
        }

        private float angleValue;
        public void OwnRotated()
        {
            angleValue += rotatedSpeed;

            transform.RotateAround(ownRotatedTrans.position, -Vector3.forward, rotatedSpeed);

            #region LockFinalPos
            if (angleValue > maxLimit && CarPointStatus == Status.speedUp)
            {
                CarPointStatus = Status.stop;

                transform.localPosition = new Vector3(19.88f, -24.66f, 0);
                transform.localRotation = Quaternion.Euler(0, 0, -56f);
            }
            else if (angleValue < minLimit && CarPointStatus == Status.speedDown)
            {
                CarPointStatus = Status.stop;
                angleValue = 0;

                transform.localPosition = new Vector3(-25f, -30.8f, 0);
                transform.localRotation = Quaternion.Euler(0, 0, 145f);
            }
            #endregion
        }

        private IEnumerator OnSpeedUpShake()
        {
            yield return new WaitForSeconds(0.2f);
            CarPointStatus = Status.speedDown;
            yield return new WaitForSeconds(0.2f);
            CarPointStatus = Status.speedUp;

            allowShake = false;
        }
    }

}
