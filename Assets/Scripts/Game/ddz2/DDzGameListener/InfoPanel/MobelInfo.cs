using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

#pragma warning disable 649


namespace Assets.Scripts.Game.ddz2.DDzGameListener.InfoPanel
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
        [SerializeField]
        private UISprite _netSprite;

        /// <summary>
        /// 时间
        /// </summary>
        [SerializeField]
        private UILabel _timeLabel;
        
        /// <summary>
        /// 界面
        /// </summary>
        [SerializeField]
        private GameObject _mobelInfoView;


        // ReSharper disable once InconsistentNaming
        protected float batteryRate
        {
            get { return _batteryPower; }
            set { _batteryPower = value; }
        }


        [DllImport("__Internal")]
        protected static extern float getiOSBatteryLevel();

        // ReSharper disable once UnusedMember.Local
        private void SetBatteryRate(float curr, float amount) { batteryRate = curr / amount * 100; }

        public string NetG;

        public string NetWifi;

        private bool _isLoop;

        // Use this for initialization
        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once UnusedMember.Local
        void Start()
        {
            _mobelInfoView.SetActive(true);         //显示界面
            if (!_isLoop)
            {
                _isLoop = true;
                StartCoroutine(LoopUpdata());
            }

#if UNITY_ANDROID || UNITY_EDITOR   
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

#if UNITY_IOS && !UNITY_EDITOR  
           batteryRate = (int)(100 * getiOSBatteryLevel());
#endif

            int power = 100 / _batterySprites.Length;
            for (int i = 0; i < _batterySprites.Length; i++)
            {
                _batterySprites[i].gameObject.SetActive(power * i + 10 <= batteryRate);
            }
            
        }

        protected virtual IEnumerator LoopUpdata()
        {
            while (_isLoop)
            {
                SetTime();
                SetNetType();
                SetDianLiang();

                //一分钟刷新一次
                yield return new WaitForSeconds(30);
            }
            _isLoop = false;
        }

        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once UnusedMember.Local
        void OnDisable()
        {
            _isLoop = false;
            StopAllCoroutines();
        }
    }
}