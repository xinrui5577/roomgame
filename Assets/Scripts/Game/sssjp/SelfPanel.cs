using Assets.Scripts.Game.sssjp.ImgPress.Main;
using UnityEngine;
using YxFramwork.Common;
#pragma warning disable 649

namespace Assets.Scripts.Game.sssjp
{
    public class SelfPanel : SssPlayer
    {
        /// <summary>
        /// 开始按钮
        /// </summary>
        [SerializeField]
        private UIButton _startBtn;

        /// <summary>
        /// 准备按钮
        /// </summary>
        [SerializeField]
        private UIButton _readyBtn;

        /// <summary>
        /// 快进按钮
        /// </summary>
        [SerializeField]
        private UIButton _SpeedupBtn;

        /// <summary>
        /// 准备时间
        /// </summary>
        [SerializeField]
        private UILabel _readyTimeLabel;

        public bool SpeedupBtnActive
        {
            get { return _SpeedupBtn.gameObject.activeSelf; }
        }

        public override void OnUserReady()
        {
            base.OnUserReady();
            if (_startBtn == null)
                return;
            var gdata = App.GetGameData<SssGameData>();
            _startBtn.gameObject.SetActive(gdata.IsRoomGame && Info.Seat == 0 && !gdata.IsPlayed);
        }

        /// <summary>
        /// 当可以开始游戏
        /// </summary>
        public override void OnCouldStart()
        {
            if (Info.Seat != 0 || _startBtn == null)
                return;

            _startBtn.gameObject.SetActive(true);
            _startBtn.GetComponent<BoxCollider>().enabled = true;
            _startBtn.state = UIButtonColor.State.Normal;
        }

        /// <summary>
        /// 点击开始(外挂方法)
        /// </summary>
        public void OnClickStartBtn()
        {
            App.GetRServer<SssjpGameServer>().SendRequest(GameRequestType.CouldStart, null);
        }

        /// <summary>
        /// 设置准备按钮是否显示
        /// </summary>
        /// <param name="active"></param>
        public override void SetReadyBtnActive(bool active)
        {
            if (_readyBtn == null)
                return;

            _readyBtn.gameObject.SetActive(active);
        }

        /// <summary>
        /// 快进按钮控制
        /// </summary>
        public void SetSpeedupBtnActive(bool active)
        {
            if (_SpeedupBtn == null) return;
            _SpeedupBtn.gameObject.SetActive(active);
        }

        public override void SetReadyStatue(bool isReady)
        {
            base.SetReadyStatue(isReady);
            _startBtn.gameObject.SetActive(!App.GetGameData<SssGameData>().IsPlayed && isReady);
        }

        public override void FinishChoiseCards()
        {
            base.FinishChoiseCards();
            App.GetGameManager<SssjpGameManager>().ChoiseMgr.HideChoiseView();

            //显示快进按钮
            var gdata = App.GetGameData<SssGameData>();
            if (gdata.IsRoomGame)
            {
                SetSpeedupBtnActive(true);
            }
        }

        public override void OnGameStart()
        {
            base.OnGameStart();
            _startBtn.gameObject.SetActive(false);  //隐藏开始按钮
            HideReadyTimeLabel();
        }

        public override void OnAllowReady()
        {
            base.OnAllowReady();
            var gdata = App.GetGameData<SssGameData>();
            if (gdata.ShowAutoReadyTime)
            {
                CountDownReadyTime(gdata.ReadyTime);
            }
        }

        private int _timer;

        public void CountDownReadyTime(int time)
        {
            if (_readyTimeLabel == null) return;
            if (time <= 0) return;
            _timer = time;
            _readyTimeLabel.gameObject.SetActive(_timer > 0);
            _readyTimeLabel.text = _timer.ToString();
            InvokeRepeating("CountDown", 1f, 1f);
        }

        protected void CountDown()
        {
            if (--_timer < 0)
            {
                HideReadyTimeLabel();
                return;
            }

            _readyTimeLabel.text = _timer.ToString();
        }

        void HideReadyTimeLabel()
        {
            if (_readyTimeLabel == null) return;
            _readyTimeLabel.gameObject.SetActive(false);
            CancelInvoke("CountDowm");
        }

    }
}
