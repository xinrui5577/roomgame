using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public class PhoneInfo : MonoBehaviour
    {       
        public Text SysTime;

        public Image NetType;
        public Sprite NetWifi;
        public Sprite NetG;

        public Image DianLiang1;
        public Image DianLiang2;
        public Image DianLiang3;

        private float _battery = 100;       

        protected float batteryRate 
        {
            get { return _battery; }
            set { _battery = value; }
        }

        private void SetBatteryRate(float curr, float amount) { batteryRate = curr / amount * 100; }

        [DllImport("__Internal")]
        protected static extern float getiOSBatteryLevel();

        void Start()
        {
            StartCoroutine(LoopUpdata());

#if UNITY_ANDROID
           Facade.Instance<WeChatApi>().BatteryAction = SetBatteryRate;
#endif
        }

        public virtual void SetNetType()
        {
            //wifi
            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                if (NetType != null) NetType.sprite = NetWifi;
            }
            else
            {
                if (NetType != null) NetType.sprite = NetG;
            }
        }

        public virtual void SetDianLiang()
        {
            if (DianLiang1 != null) DianLiang1.gameObject.SetActive(true);
            if (DianLiang2 != null) DianLiang2.gameObject.SetActive(true);
            if (DianLiang3 != null) DianLiang3.gameObject.SetActive(true);

#if UNITY_IOS&&!UNITY_EDITOR
           batteryRate = (int)(100 * getiOSBatteryLevel());
#endif

            if (batteryRate <= 85f)
            {
                if (DianLiang1 != null) DianLiang1.gameObject.SetActive(false);
            }

            if (batteryRate <= 50f)
            {
                if (DianLiang2 != null) DianLiang2.gameObject.SetActive(false);
            }

            if (batteryRate <= 20f)
            {
                if (DianLiang3 != null) DianLiang3.gameObject.SetActive(false);
            }
        }

        public virtual void SetTime()
        {
            //刷新时间
            System.DateTime now = System.DateTime.Now;
            if (SysTime != null) SysTime.text = string.Format("{0:D2}:{1:D2}", now.Hour, now.Minute);
        }

        protected virtual IEnumerator LoopUpdata()
        {
            while (true)
            {
                SetTime();
                SetNetType();
                SetDianLiang();

                //一分钟刷新一次
                yield return new WaitForSeconds(30);
            }
        }
    }
}
