using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.BlackJackCs
{
    public class AddBetBtnItem : MonoBehaviour
    {

        /// <summary>
        /// 添加筹码的倍数
        /// </summary>
        [SerializeField]
        private int _addTimes;

        private int _ante;

        public int Ante
        {
            set { _ante = value; }
            get { return _ante; }
        }

        public string SpriteName
        {
            get
            {
                var spr = GetComponent<UISprite>() ?? GetComponentInChildren<UISprite>();
                return spr.name;
            }
        }

        /// <summary>
        /// 下注的数值(筹码倍数 * Ante)
        /// </summary>
        public int BetValue
        {
            get
            {
                return _addTimes * _ante;
            }
        }

        public int AddTimes
        {
            set
            {
                _addTimes = value;
            }

            get
            {
                return _addTimes;
            }
        }

        /// <summary>
        /// 下注按钮显示的数值
        /// </summary>
        int AddValue
        {
            get
            {
                return _addTimes * _ante;
            }
        }

        // Use this for initialization
        protected void Start()
        {
            var gdata = App.GetGameData<BlackJackGameData>();
            _ante = gdata.Ante;
            _ante = _ante > 0 ? _ante : 1;

            UIButton btn = GetComponent<UIButton>();

            btn.onClick.Add(new EventDelegate(() =>
            {
                //发送下注消息
                Dictionary<string, int> data = new Dictionary<string, int>()
                {
                    {"gold", AddValue},
                    {"seat", gdata.SelfSeat}
                };

              App.GetRServer<BlackJackGameServer>().SendRequest(GameRequestType.Ante, data);

            }));

            //显示当前局此按钮添加筹码的值
            GetComponentInChildren<UILabel>().text = YxUtiles.ReduceNumber(AddValue); //Tool.Tools.GetChipFaceValue(AddValue, false);

        }

        protected void OnEnable()
        {
            CheckCouldClick();
        }

        public void CheckCouldClick()
        {
            var self = App.GameData.GetPlayerInfo();
            if (self == null)
                return;
            var gold = self.CoinA;
            if (gold < BetValue)
            {
                GetComponent<BoxCollider>().enabled = false;
                GetComponent<UIButton>().state = UIButtonColor.State.Disabled;
            }
            else
            {
                GetComponent<BoxCollider>().enabled = true;
                GetComponent<UIButton>().state = UIButtonColor.State.Normal;
            }
        }
    }
}