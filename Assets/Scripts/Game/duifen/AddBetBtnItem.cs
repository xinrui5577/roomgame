using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Game.duifen.ImgPress.Main;
using YxFramwork.Common;
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable UnusedMember.Local



namespace Assets.Scripts.Game.duifen
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
                return _addTimes;// * App.GetGameData<DuifenGlobalData>().Ante;
            }
        }


        // Use this for initialization
        void Start()
        {
            UIButton btn = GetComponent<UIButton>();

            var selfSeat = App.GameData.SelfSeat;
            btn.onClick.Add(new EventDelegate(() =>
                {

                    //发送下注消息
                    Dictionary<string, int> data = new Dictionary<string, int>()
                    {
                        {"gold", BetValue},
                        {"seat", selfSeat}
                    };

                    App.GetGameManager<DuifenGameManager>().SendRequest(GameRequestType.Bet, data);

                    //隐藏下注菜单
                    //Mgr.main.SpeakMgr.ShowNothing();

                }));
        }

        void OnEnable()
        {
            //显示当前局此按钮添加筹码的值
            GetComponentInChildren<UILabel>().text = App.GetGameData<DuifenGlobalData>().GetShowGoldValue(BetValue);
        }

        public void SetBtnEnable(bool enable)
        {
            GetComponent<BoxCollider>().enabled = enable;
            GetComponent<UIButton>().state = enable ? UIButtonColor.State.Normal : UIButtonColor.State.Disabled;
        }


    }
}