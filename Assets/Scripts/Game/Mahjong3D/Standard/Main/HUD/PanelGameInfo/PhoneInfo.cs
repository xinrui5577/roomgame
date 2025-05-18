using System.Runtime.InteropServices;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class PhoneInfo : MonoBehaviour
    {
        public Sprite NetG;
        public Sprite NetWifi;      
        public Image NetType;       
        public Image DianLiang1;
        public Image DianLiang2;
        public Image DianLiang3;
        public Text SysTime;
        public float UpdateTime = 20;
        private float _battery = 100;
        private float mTimer;

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
            UpdateData();
#if UNITY_ANDROID
            YxFramwork.Framework.Core.Facade.Instance<YxFramwork.Tool.WeChatApi>().BatteryAction = SetBatteryRate;
#endif
        }

        private void Update()
        {
            mTimer += Time.deltaTime;
            if (mTimer > UpdateTime)
            {
                UpdateData();
                mTimer = 0;
            }
        }

        private void UpdateData()
        {
            SetTime();
            SetNetType();
            SetDianLiang();
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
#if UNITY_IOS
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
    }
}
