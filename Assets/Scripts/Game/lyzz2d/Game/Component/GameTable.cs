using System;
using Assets.Scripts.Game.lyzz2d.Game.GameCtrl;
using Assets.Scripts.Game.lyzz2d.Utils;
using Assets.Scripts.Game.lyzz2d.Utils.Single;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lyzz2d.Game.Component
{
    /// <summary>
    ///     牌桌上的其他信息
    /// </summary>
    public class GameTable : MonoSingleton<GameTable>
    {
        private static float _timestamp;
        public static bool IsGameMatchTime = false;

        [SerializeField] private UILabel _cardNumber;

        private bool _isInGameRoom;

        /// <summary>
        ///     用于显示时间的文本
        /// </summary>
        [SerializeField] private UILabel _showTimeLabel;

        public UILabel AnteLabel;
        public CountDownGo CountDownGo;
        private float deltaEuler;
        public UILabel GameInfoLabel;
        public GameObject LeftCardParent;

        public PlayerDirection PlayerDirection;
        public UILabel RateLabel;
        public UILabel RoundLabel;
        public UILabel ShowRoomIdLabel;
        private float tweenEuler;

        private CurrentGameType CurrentGame
        {
            get { return App.GetGameData<Lyzz2DGlobalData>().CurrentGame; }
            set { App.GetGameData<Lyzz2DGlobalData>().CurrentGame = value; }
        }


        public override void Awake()
        {
            base.Awake();
            ShowTime();
            Reset();
            ShowRoomIdLabel.text = "";
            GameInfoLabel.text = "";
            RoundLabel.text = "";
        }

        public void InitInfoPanel()
        {
            switch (CurrentGame.GameRoomType)
            {
                case -1:
                    ShowRoomIdLabel.transform.parent.gameObject.SetActive(true);
                    ShowRoomIdLabel.text = CurrentGame.RoomId;
                    break;
                default:
                    ShowRoomIdLabel.transform.parent.gameObject.SetActive(false);
                    break;
            }
            RoundLabel.gameObject.SetActive(CurrentGame.GameRoomType == -1);
            RateLabel.gameObject.SetActive(CurrentGame.Rate != 1);
            AnteLabel.text = string.Format("底注：{0}", CurrentGame.Ante);
            RateLabel.text = string.Format("x{0}", CurrentGame.Rate);
            ShowRoomIdLabel.text = CurrentGame.ShowRoomId.ToString();
            RoundLabel.text = CurrentGame.ShowRoundInfo;
            GameInfoLabel.text = CurrentGame.RuleInfo;
        }

        public void Reset()
        {
            AnteLabel.text = "";
            RateLabel.text = "";
            CountDownGo.Stop(true);
            PlayerDirection.ResetDNXBState();
            LeftCardParent.SetActive(false);
        }


        public void SetPlayerDir(int distance, bool firstTime = false)
        {
            tweenEuler = distance*90;
            deltaEuler = tweenEuler/30;
            if (tweenEuler.Equals(PlayerDirection.transform.localEulerAngles.z))
            {
                return;
            }
            if (firstTime)
            {
                InvokeRepeating("Rotate", 3, 0.1f);
            }
            else
            {
                PlayerDirection.transform.localEulerAngles = new Vector3(0, 0, tweenEuler);
            }
        }

        private void Rotate()
        {
            PlayerDirection.transform.localEulerAngles = new Vector3(0, 0,
                deltaEuler + PlayerDirection.transform.localEulerAngles.z);
            tweenEuler -= deltaEuler;
            if (deltaEuler > 0)
            {
                if (tweenEuler <= 0)
                {
                    CancelInvoke("Rotate");
                }
            }
            else
            {
                if (tweenEuler >= 0)
                {
                    CancelInvoke("Rotate");
                }
            }
        }

        /// <summary>
        ///     设置东南西北
        /// </summary>
        /// <param name="seat">Real ID</param>
        public void ChangeDir(int seat)
        {
            PlayerDirection.SetDNXBShow(seat);
        }

        private void ShowTime()
        {
            if (_showTimeLabel != null)
            {
                InvokeRepeating("RefreshTime", 0, 60);
            }
        }

        private void RefreshTime()
        {
            var now = DateTime.Now;
            _showTimeLabel.text = string.Format("{0} {1:D2}:{2:D2}",
                now.Hour < 12 || now.Hour == 24 ? "AM" : "PM",
                now.Hour%12,
                now.Minute
                );
        }

        public void UpdateLeftNum(int num)
        {
            _cardNumber.text = num.ToString();
        }
    }
}