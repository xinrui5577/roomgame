using UnityEngine;
using Assets.Scripts.Game.fillpit.Tool;
using com.yxixia.utile.YxDebug;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.DataBundles;
using YxFramwork.Tool;

// ReSharper disable FieldCanBeMadeReadOnly.Local



namespace Assets.Scripts.Game.fillpit
{
    public class SumItem : MonoBehaviour
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
        /// 用于显示玩家是否是房主
        /// </summary>
        [SerializeField]
        private GameObject _owenMark = null;

        /// <summary>
        /// 用于显示玩家的手牌点数
        /// </summary>
        [SerializeField]
        private UILabel _cardsValueLabel = null;

        /// <summary>
        /// 头像图片
        /// </summary>
        [SerializeField]
        private YxBaseTextureAdapter _headImage = null;

        /// <summary>
        /// 用于显示玩家的手牌
        /// </summary>
        [SerializeField]
        private PokerCard[] _cards = null;

        [SerializeField]
        private GameObject _foldMark = null;

        /// <summary>
        /// 通杀标记
        /// </summary>
        [SerializeField]
        private GameObject _allKillMark = null;

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
        /// <param name="cardsVal">玩家的卡牌值</param>
        public void SetUserCards(int[] cardsVal)
        {
            //设置扑克的数值
            for (int i = 0; i < cardsVal.Length; i++)
            {
                _cards[i].SetCardId(cardsVal[i]);
                _cards[i].SetCardFront();
            }

            //设置只显示已经设置好的内容
            int count = cardsVal.Length;    //设置显示的个数,即只显示玩家手牌的个数
            for (int i = 0; i < _cards.Length; i++)
            {
                _cards[i].gameObject.SetActive(i < count);
            }
        }



        /// <summary>
        /// 设置用户信息的颜色
        /// </summary>
        /// <param name="hex">颜色值,可用16进制0xffffff</param>
        public void SetLabelColor(int hex)
        {
            _nameLabel.color = Tools.ChangeToColor(0xffff00);
            _cardsValueLabel.color = Tools.ChangeToColor(0xffff00);
            _winGoldLabel.color = Tools.ChangeToColor(0xffff00);
        }


        /// <summary>
        /// 初始化小结成员的信息
        /// </summary>
        /// <param name="data"></param>
        public void InitSumItem(Sfs2X.Entities.Data.ISFSObject data)
        {
            if (data.ContainsKey("seat"))
            {
                //获取信息
                int seat = data.GetInt("seat");
                var gdata = App.GetGameData<FillpitGameData>();
                var player = gdata.GetPlayerInfo(seat,true);
                if (player == null)
                {
                    gameObject.SetActive(false);
                    return;
                }

                _nameLabel.text = player.NickM;
                PortraitDb.SetPortrait(player.AvatarX, _headImage, player.SexI);

                _winValue = data.GetInt("win");
                _cardsValueLabel.text = _winValue.ToString();
                _cardsValueLabel.text = data.GetInt("cardsValue").ToString();
                _winGoldLabel.text = YxUtiles.ReduceNumber(_winValue);//App.GetGameData<GlobalData>().GetShowGold(_winValue);
                if (_allKillMark != null)
                {
                    bool isAllKill = (gdata.Dkak && (data.ContainsKey("doubleKing") && data.GetBool("doubleKing"))) ||
                                     (gdata.Sfak && (data.ContainsKey("sameFour") && data.GetBool("sameFour")));
                    _allKillMark.SetActive(_winValue > 0 && isAllKill);
                }

                SetUserCards(data.GetIntArray("cards"));
                YxDebug.LogArray(data.GetIntArray("cards"));

                if (data.ContainsKey("isgame") && _foldMark != null)
                {
                    _foldMark.SetActive(!data.GetBool("isgame"));
                }

                if (seat == gdata.SelfSeat)
                {
                    SetLabelColor(0xffff00);
                }
                _owenMark.SetActive(gdata.IsRoomGame && player.Id == gdata.OwnerId);
            }
        }
    }
}