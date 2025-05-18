using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using YxFramwork.Framework.Core;  //不可删除
using YxFramwork.Tool;      //不可删除

#pragma warning disable 649

namespace Assets.Scripts.Game.duifen.MobelInfo
{
    public class MobelInfo : MonoBehaviour
    {

        /// <summary>
        /// 电池电量图片
        /// </summary>
        [SerializeField]
        private UISprite[] _batterySprites;

        /// <summary>
        /// 电池电量
        /// </summary>
        private float _batteryPower = 100;

        /// <summary>
        /// 网络图片
        /// </summary>
        private UISprite _netSprite;

        /// <summary>
        /// 时间
        /// </summary>
        [SerializeField]
        private UILabel _timeLabel;


        protected float BatteryRate
        {
            get { return _batteryPower; }
            set { _batteryPower = value; }
        }


        [DllImport("__Internal")]
        protected static extern float getiOSBatteryLevel();

        private void SetBatteryRate(float curr, float amount) { BatteryRate = curr / amount * 100; }

        public string NetG;

        public string NetWifi;



        // Use this for initialization
        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once UnusedMember.Local
        void Start()
        {

            StartCoroutine(LoopUpdata());

#if UNITY_ANDROID   
           Facade.Instance<WeChatApi>().BatteryAction = SetBatteryRate;
#endif

        }



        public virtual void SetTime()
        {
            //刷新时间
            System.DateTime now = System.DateTime.Now;
            if (_timeLabel != null) _timeLabel.text = string.Format("{0:D2}:{1:D2}", now.Hour, now.Minute);
        }

        public virtual void SetNetType()
        {
            if (_netSprite == null)
                return;

            //wifi
            _netSprite.spriteName = Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork ? NetWifi : NetG;

            _netSprite.MakePixelPerfect();
        }

        public virtual void SetDianLiang()
        {

#if UNITY_IOS
           BatteryRate = (int)(100 * getiOSBatteryLevel());
#endif

            int power = 100 / _batterySprites.Length;
            for (int i = 0; i < _batterySprites.Length; i++)
            {
                _batterySprites[i].gameObject.SetActive(power * i + 10 <= BatteryRate);
            }
            
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
            // ReSharper disable once FunctionNeverReturns
        }
    }
}