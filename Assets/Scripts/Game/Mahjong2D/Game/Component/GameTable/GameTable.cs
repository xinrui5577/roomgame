using System;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.Mahjong2D.Common.MahJongCommon;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using Assets.Scripts.Game.Mahjong2D.Game.Data;
using Assets.Scripts.Game.Mahjong2D.Game.GameCtrl;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.Mahjong2D.Game.Component.GameTable
{
    /// <summary>
    /// 牌桌上的其他信息
    /// </summary>
    public class GameTable : MonoSingleton<GameTable>
    {
        public  PlayerDirection PlayerDirection;
        /// <summary>
        /// 用于显示时间的文本
        /// </summary>
        [SerializeField]
        private UILabel _showTimeLabel;
        public UILabel ShowRoomIdLabel;
        public UILabel GameInfoLabel;
        public UILabel RoundLabel;
        public UILabel AnteLabel;
        public UILabel RateLabel;
        [Tooltip("桌面背景")]
        public UITexture TabelBg;
        [Tooltip("背景图片（用于切换）")]
        public Texture2D[] bgTextures;
        [SerializeField]
        private UITexture _changeTex;
        [SerializeField]
        private UILabel _cardNumber;
        private static float _timestamp;
        private bool _isInGameRoom;
        private float tweenEuler;
        private float deltaEuler;
        [SerializeField]
        Color _defColor = new Color(0, 251, 209, 52);
        [SerializeField]
        Color _beginColor=new Color(251,0,0,52);
        [HideInInspector]
        public float ColorPercent;
        /// <summary>
        /// 剩余麻将数量格式显示
        /// </summary>
        public string LeftNumFormat = "{0}";
        /// <summary>
        ///  剩余麻将数量颜色格式
        /// </summary>
        public string LeftNumColorFormat = "{0}";
        /// <summary>
        /// 剩余麻将限制数量
        /// </summary>
        public int LeftLimitNum = 4;
        /// <summary>
        /// 剩余数量显示特殊颜色
        /// </summary>
        public bool LeftNoticeWithColor = false;
        

        public override void Awake()
        {
            base.Awake();
            ShowTime();
            Reset();
            ShowRoomIdLabel.TrySetComponentValue(string.Empty);
            GameInfoLabel.TrySetComponentValue(string.Empty);
            RoundLabel.TrySetComponentValue(string.Empty);
            DealColorFromCache();
            if (TabelBg)
            {
                Facade.EventCenter.AddEventListeners<string, int>(ConstantData.KeyNowBgIndex, ChangeBgs);
                ChangeBgs(0);
            }
        }
        

        private CurrentGameType CurrentGame
        {
            get { return App.GetGameData<Mahjong2DGameData>().CurrentGame; }
            set { App.GetGameData<Mahjong2DGameData>().CurrentGame = value; }
        }

        public void InitInfoPanel()
        {
            ShowRoomIdLabel.TrySetComponentValue(CurrentGame.ShowRoomId.ToString());
            HelpInfoControl.Instance.NeedWrap = CurrentGame.IsCreateRoom;
            HelpInfoControl.Instance.LayoutResetPosition();
            AnteLabel.TrySetComponentValue(string.Format("底注：{0}", CurrentGame.Ante));
            RateLabel.TrySetComponentValue(string.Format("x{0}", CurrentGame.Rate));
            RoundLabel.TrySetComponentValue(CurrentGame.ShowRoundInfo);
            CheckRule();
        }

        /// <summary>
        /// 简单版游戏信息
        /// </summary>
        private void CheckRule()
        {
            var info = CurrentGame.SimpleRule.Replace(";", "\n").Replace(" ","");
            GameInfoLabel.TrySetComponentValue(info);
        }

        public void Reset()
        {
            AnteLabel.TrySetComponentValue(string.Empty);
            RateLabel.TrySetComponentValue(string.Empty);
            PlayerDirection.ResetDNXBState();
            UpdateLeftNum(0);
        }


        public void SetPlayerDir(int distance,bool firstTime=false)
        {
            tweenEuler = distance * 90;
            deltaEuler = tweenEuler/30;
            if (tweenEuler.Equals(PlayerDirection.transform.localEulerAngles.z))
            {
                return;
            }
            if (firstTime)
            {
                InvokeRepeating("Rotate",3, 0.1f);

            }
            else
            {
                PlayerDirection.transform.localEulerAngles=new Vector3(0, 0, tweenEuler);
            }

        }

        void Rotate()
        {
            PlayerDirection.transform.localEulerAngles=new Vector3(0,0,deltaEuler+ PlayerDirection.transform.localEulerAngles.z);
            tweenEuler -= deltaEuler;
            if (deltaEuler>0)
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
        /// 设置东南西北
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

        void RefreshTime()
        {
            DateTime now = DateTime.Now;
            string showInfo = "";
            if(string.IsNullOrEmpty(App.GameKey))
            {
                return;
            }
            switch ((EnumGameKeys) Enum.Parse(typeof (EnumGameKeys), App.GameKey))
            {
                case EnumGameKeys.shmj:
                    showInfo= string.Format("{0:D2}:{1:D2}",now.Hour,now.Minute );
                    break;
                default:
                    showInfo= string.Format("{0} {1:D2}:{2:D2}",
                        now.Hour < 12 || now.Hour == 24 ? "AM" : "PM",
                        now.Hour % 12,
                        now.Minute
                        );
                    break;
            }
            _showTimeLabel.TrySetComponentValue(showInfo);

        }

        public void UpdateLeftNum(int num)
        {
            var showContent = string.Empty;
            showContent = LeftNoticeWithColor ? string.Format(num<= LeftLimitNum ? LeftNumColorFormat : LeftNumFormat, num) : string.Format(LeftNumFormat, num);
            _cardNumber.TrySetComponentValue(showContent);
        }
        private void DealColorFromCache()
        {        
            Color getColor= GameTools.GetSaveColor(_defColor,out ColorPercent);
            SetColor(getColor);
        }

        public void SetColor(Color color)
        {
            if (_changeTex != null)
            {
                _changeTex.color = color;
            }
        }

        public override void OnDestroy()
        {
            Facade.EventCenter.RemoveEventListener<string, int>(ConstantData.KeyNowBgIndex, ChangeBgs);
            base.OnDestroy();
        }

        public void ChangeBgs(int data)
        {
            if (TabelBg)
            {
                var index = (PlayerPrefs.GetInt(ConstantData.KeyNowBgIndex, 0)+data)% bgTextures.Length;
                if (bgTextures != null && bgTextures.Length>index)
                {
                    PlayerPrefs.SetInt(ConstantData.KeyNowBgIndex,index);
                    TabelBg.mainTexture = bgTextures[index];
                }
            }
        }

    }
}
