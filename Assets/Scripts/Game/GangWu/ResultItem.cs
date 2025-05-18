using Assets.Scripts.Game.GangWu.Mgr;
using UnityEngine;
using com.yxixia.utile.YxDebug;
using YxFramwork.Tool;
using YxFramwork.Common;


namespace Assets.Scripts.Game.GangWu
{

    public class ResultItem : MonoBehaviour
    {


        /// <summary>
        /// 用于显示玩家的名字
        /// </summary>
        [SerializeField]
        private UILabel _nameLabel = null;

        /// <summary>
        /// 用于显示玩家的输赢情况
        /// </summary>
        [SerializeField]
        private UILabel _winGoldLabel = null;


        /// <summary>
        /// 用于显示玩家的手牌点数
        /// </summary>
        [SerializeField]
        private UISprite _playerTypeSprite = null;

        /// <summary>
        /// 头像图片
        /// </summary>
        [SerializeField]
        private UITexture _headImage = null;

        /// <summary>
        /// 用于显示玩家的手牌
        /// </summary>
        [SerializeField]
        private PokerCard[] _cards = null;

        /// <summary>
        /// 玩家是否是本局的赢家
        /// </summary>
        private int _winValue;

        /// <summary>
        /// 玩家赢取的点数
        /// </summary>
        public int WinValue { get { return _winValue; } }


        /// <summary>
        /// 设置用户的手牌
        /// </summary>
        /// <param name="cards">玩家的卡牌值</param>
        private void SetUserCards(int[] cards)
        {
            //设置扑克的数值
            for (int i = 0; i < cards.Length; i++)
            {
                _cards[i].SetCardId(cards[i]);
                _cards[i].SetCardFront();
            }

            //设置只显示已经设置好的内容
            int count = cards.Length;    //设置显示的个数,即只显示玩家手牌的个数
            for (int i = 0; i < _cards.Length; i++)
            {
                _cards[i].gameObject.SetActive(i < count);
            }
        }


        /// <summary>
        /// 初始化小结成员的信息
        /// </summary>
        /// <param name="userInfo"></param>
        public void InitSumItem(ResultUserInfo userInfo)
        {
            if (userInfo == null)
            {
                YxDebug.LogEvent("数据有错","ResultItem");
                return;
            }

            int serverSeat = userInfo.Seat;
            var panel = App.GameData.GetPlayer<PlayerPanel>(serverSeat, true);


            _nameLabel.text = panel.NameLabel.Value;

            //PortraitDb.SetPortrait(userInfo.AvatarX,_headImage,userInfo.SexI);
            _headImage.mainTexture = panel.HeadPortrait.GetTexture(); 
            _winValue = userInfo.WinVal;

            _winGoldLabel.text = string.Format("总分: {0}", YxUtiles.ReduceNumber(_winValue));  //设置分数

            SetUserCards(userInfo.Cards);        //将玩家手牌显示在玩家结算界面

            //显示玩家的状态,赢,棋牌,输
            _playerTypeSprite.spriteName = string.Format("icon_{0}", _winValue > 0 ? "win" : "lost");
            _playerTypeSprite.MakePixelPerfect();
        }
    }
}