using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.pdk.DDz2Common
{
    public class PhoneStateCtrl : MonoBehaviour {

        private float _battery = 100;

        private float batteryRate
        {
            get { return _battery; }
            set { _battery = value; }
        }

        private void SetBatteryRate(float curr, float amount) { batteryRate = curr / amount * 100; }

        [DllImport("__Internal")]
        private static extern float getiOSBatteryLevel();

        private void Start()
        {
            #if UNITY_ANDROID
                Facade.Instance<WeChatApi>().BatteryAction = SetBatteryRate;
            #endif

            StartCoroutine(CheckPhoneState());
        }


/*        void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(CheckPhoneState());
        }*/

        public GameObject[] PhoneBatteryStages;

        IEnumerator CheckPhoneState()
        {

            while (true)
            {

                #if UNITY_IOS
                    batteryRate = (int)(100 * getiOSBatteryLevel());
                #endif

                PhoneBatteryStages[0].SetActive(true);
                PhoneBatteryStages[1].SetActive(true);
                PhoneBatteryStages[2].SetActive(true);

                if (batteryRate < 85f && batteryRate > 50f)
                {
                    PhoneBatteryStages[0].SetActive(false);
                }
                else if (batteryRate <= 50f && batteryRate > 20f)
                {
                    PhoneBatteryStages[0].SetActive(false);
                    PhoneBatteryStages[1].SetActive(false);
                }
                else if (batteryRate < 20f)
                {
                    PhoneBatteryStages[0].SetActive(false);
                    PhoneBatteryStages[1].SetActive(false);
                    PhoneBatteryStages[2].SetActive(false);
                }

                yield return new WaitForSeconds(3f);
            }
// ReSharper disable FunctionNeverReturns
        }
// ReSharper restore FunctionNeverReturns


/*        /// <summary>
        /// 获得安卓手机电量
        /// </summary>
        /// <returns></returns>
        public static int GetBatteryLevForAndroid()
        {
            try
            {
                string capacityString = System.IO.File.ReadAllText("/sys/class/power_supply/battery/capacity");
                return int.Parse(capacityString);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to read battery power; " + e.Message);
            }
            return -1;
        }  */
    }
}
