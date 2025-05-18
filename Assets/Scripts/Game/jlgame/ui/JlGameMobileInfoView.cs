using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.jlgame.ui
{
    public class JlGameMobileInfoView : MonoBehaviour
    {
        /// <summary>
        /// 电池图片
        /// </summary>
        public UISprite[] BatterySprites;
        /// <summary>
        /// 电池电量
        /// </summary>
        private float _batteryPower = 100;
        /// <summary>
        /// 网络图片
        /// </summary>
        public UISprite NetSprite;
        /// <summary>
        /// 时间
        /// </summary>
        public UILabel TimeLabel;

        public string NetG;
        public string NetWifi;

        protected float BatteryRate
        {
            get { return _batteryPower; }
            set { _batteryPower = value; }
        }
        private bool _isLoop;
        [DllImport("__Internal")]
        protected static extern float getiOSBatteryLevel();

        private void SetBatteryRate(float curr, float amount) { BatteryRate = curr / amount * 100; }

        protected void Start()
        {
            if (!_isLoop)
            {
                _isLoop = true;
                StartCoroutine(LoopUpdata());
            }
#if UNITY_ANDROID || UNITY_EDITOR
            Facade.Instance<WeChatApi>().BatteryAction = SetBatteryRate;
#endif
        }

        protected IEnumerator LoopUpdata()
        {
            while (_isLoop)
            {
                SetTime();
                SetNetType();
                SetBatteryPower();

                //半分钟刷新一次
                yield return new WaitForSeconds(30);
            }
            _isLoop = false;
        }

        protected void SetTime()
        {
            var now = DateTime.Now;
            if (TimeLabel != null) TimeLabel.text = string.Format("{0:D2}:{1:D2}", now.Hour, now.Minute);
        }

        protected void SetNetType()
        {
            if (NetSprite == null) return;
            //wifi
            NetSprite.spriteName = Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork ? NetWifi : NetG;
            NetSprite.MakePixelPerfect();
        }

        protected void SetBatteryPower()
        {
#if UNITY_IOS && !UNITY_EDITOR
           BatteryRate = (int)(100 * getiOSBatteryLevel());
#endif
            int power = 100 / BatterySprites.Length;
            for (int i = 0; i < BatterySprites.Length; i++)
            {
                BatterySprites[i].gameObject.SetActive(power * i + 10 <= BatteryRate);
            }
        }

        protected void OnDisable()
        {
            _isLoop = false;
            StopAllCoroutines();
        }

    }
}
