using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Game.paijiu.ImgPress.Main;
using YxFramwork.Common;
using YxFramwork.Tool;


namespace Assets.Scripts.Game.paijiu
{
    public class AddBetBtnItem : MonoBehaviour
    {

        /// <summary>
        /// 添加筹码的倍数
        /// </summary>
        [SerializeField]
        private int _addTimes;

        private string _spriteName;

        public int AddTimes
        {
            set { _addTimes = value; }
        }

        public string SpriteName
        {
            get
            {
                if (string.IsNullOrEmpty(_spriteName))
                    _spriteName = GetComponent<UISprite>().spriteName;

                return _spriteName;
            }
        }

        /// <summary>
        /// 下注的数值(筹码倍数 * Ante)
        /// </summary>
        public int BetValue
        {
            get
            {
                return _addTimes * App.GetGameData<PaiJiuGameData>().Ante;
            }
        }


        // Use this for initialization
        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once UnusedMember.Local
        void Start()
        {
            UIButton btn = GetComponent<UIButton>();

            btn.onClick.Add(new EventDelegate(() =>
                {
                    //发送下注消息
                    Dictionary<string, int> data = new Dictionary<string, int>() { { "gold", BetValue }, { "seat", App.GameData.SelfSeat } };

                    App.GetRServer<PaiJiuGameServer>().SendRequest(GameRequestType.UserBet, data);

                }));
        }

        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once UnusedMember.Local
        void OnEnable()
        {
            //显示当前局此按钮添加筹码的值
            GetComponentInChildren<UILabel>().text =YxUtiles.ReduceNumber(BetValue);
        }

        public void SetBtnEnable(bool enable)
        {
            GetComponent<BoxCollider>().enabled = enable;
            GetComponent<UIButton>().state = enable ? UIButtonColor.State.Normal : UIButtonColor.State.Disabled;
        }


    }
}