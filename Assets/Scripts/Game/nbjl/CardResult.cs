using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common.Utils;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

/*===================================================
 *文件名称:     CardResult.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-06-26
 *描述:        	庄家或闲家显示牌局结果
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.nbjl
{
    public class CardResult : BaseMono 
    {
        #region UI Param
        [Tooltip("结果牌")]
        public CardItem[] Cards;
        [Tooltip("结果点数")]
        public YxBaseLabelAdapter ResultScore;
        [Tooltip("牌类型（庄还是闲）")]
        public CardResultType Type;

        #endregion

        #region Data Param

        [Tooltip("结果点数")]
        public string ResultFormat="{0}点";

        [Tooltip("延时播放时间")]
        public float DelayTime;

        [Tooltip("翻牌频率")]
        public float CardOpenFrame=1.5f;
        [Tooltip("牌显示结束")]
        public List<EventDelegate> OnCardShowFinish; 

        /// <summary>
        /// 播放的索引
        /// </summary>
        private int _showIndex;
        /// <summary>
        /// 牌
        /// </summary>
        private List<int> _cards=new List<int>();
        /// <summary>
        /// 最终值
        /// </summary>
        private int _totalValue;

        /// <summary>
        /// 牌数量
        /// </summary>
        private int _cardNum;

        /// <summary>
        /// 播放模式
        /// </summary>
        private int _playModel;
        #endregion

        #region Local Data

        #endregion

        #region Life Cycle

        protected override void OnAwake()
        {
            Facade.EventCenter.AddEventListeners<CardResultType, CardsData>(Type, GetCardsResult);
            Facade.EventCenter.AddEventListeners<LocalRequest, int>(LocalRequest.ReqGiveCards, OnCardShow);
            Facade.EventCenter.AddEventListeners<LocalRequest, int>(LocalRequest.Init, OnInit);
            if (Type==CardResultType.Zhuang)
            {
                Facade.EventCenter.AddEventListeners<LocalRequest, bool>(LocalRequest.LeisureDouble, OnLeisureDouble);
            }
            Reset();
        }

        #endregion

        #region Function
        /// <summary>
        /// 显示牌面信息
        /// </summary>
        /// <param name="cardsInfo"></param>
        private void GetCardsResult(CardsData cardsInfo)
        {
            _showIndex = 0;
            _totalValue = 0;
            _cards = cardsInfo.Cards;
            _cardNum = cardsInfo.CardNum;
            if (Type == CardResultType.Xian)
            {
                Facade.EventCenter.DispatchEvent(LocalRequest.LeisureDouble, _cardNum == 2);
            }
            ResultScore.gameObject.SetActive(true);
            if(_playModel==ConstantData.KeyQuickModel)
            {
                var count = _cards.Count;
                for (int i = 0; i < count; i++)
                {
                    var value = _cards[i];
                    if (value != 0)
                    {
                        var realValue = value % 16;
                        if (realValue >= 10)
                        {
                            realValue = 0;
                        }
                        _totalValue = (realValue + _totalValue) % 10;
                        Cards[i].QucikPlay(value);
                        ResultScore.TrySetComponentValue(string.Format(ResultFormat, _totalValue));
                    }
                }
                StartCoroutine(OnCardShowFinish.WaitExcuteCalls());
            }
            else
            {
                InvokeRepeating("ShowCardItem", DelayTime, CardOpenFrame);
            }
        }

        /// <summary>
        /// 显示牌
        /// </summary>
        private void ShowCardItem()
        {
            var value = _cards[_showIndex];
            if (value!=0)
            {
                var realValue = value % 16;
                if (realValue >= 10)
                {
                    realValue = 0;
                }
                _totalValue = (realValue + _totalValue) % 10;
                Cards[_showIndex].Play(value);
                ResultScore.TrySetComponentValue(string.Format(ResultFormat, _totalValue));
                var sound = "";
                if (Type == CardResultType.Xian)
                {
                    sound = _showIndex >= 2 ? ConstantData.KeySoundLeisureAdd : ConstantData.KeySoundLeisure;
                }
                else
                {
                    sound = _showIndex >= 2 ? ConstantData.KeySoundBankerAdd : ConstantData.KeySoundBanker;
                }
                Facade.Instance<MusicManager>().Play(sound);
            }
            _showIndex++;
            if (Type==CardResultType.Zhuang)
            {
                if(_showIndex==2&&_leisureDouble)
                {
                    OnShowBankerLast();
                    return;
                }
            }
            if(_showIndex>=_cards.Count||_cards[_showIndex] ==0||_showIndex>= _cardNum)
            {
                StartCoroutine(OnCardShowFinish.WaitExcuteCalls());
                CancelInvoke("ShowCardItem");
            }
        }

        /// <summary>
        /// 初始化消息
        /// </summary>
        /// <param name="num"></param>
        private void OnInit(int num)
        {
            Reset();
        }

        private bool _leisureDouble;
        private void OnLeisureDouble(bool state)
        {
            _leisureDouble = state;
        }

        private void OnShowBankerLast()
        {
            Invoke("ShowBankerLast", CardOpenFrame/2);
           
        }

        private void ShowBankerLast()
        {
            CancelInvoke("ShowBankerLast");
            CancelInvoke("ShowCardItem");
            ShowCardItem();
        }

        /// <summary>
        /// 显示牌阶段
        /// </summary>
        /// <param name="state"></param>
        private void OnCardShow(int state)
        {
            _playModel = state;
        }

        /// <summary>
        /// 重置
        /// </summary>
        private void Reset()
        {
            for (int i = 0; i < _cards.Count; i++)
            {
                Cards[i].Reset();
            }
            CancelInvoke("ShowCardItem");
            ResultScore.TrySetComponentValue("");
            ResultScore.gameObject.SetActive(false);
        }

        #endregion
    }

    /// <summary>
    /// 牌信息
    /// </summary>
    public class CardsData
    {
        /// <summary>
        /// 牌组
        /// </summary>
        public List<int> Cards { get; private set; }
        /// <summary>
        /// 牌合计结果
        /// </summary>
        public int Result { get; private set; }
        /// <summary>
        /// 对子
        /// </summary>
        public bool DoubleCard { get; private set; }
        /// <summary>
        /// 天王
        /// </summary>
        public bool King { get; private set; }
        /// <summary>
        /// 牌数量
        /// </summary>
        public int CardNum { get; private set; }

        /// <summary>
        /// 结果类型
        /// </summary>
        public CardResultType CardsType { get; private set; }

        public CardsData(CardResultType type,ISFSObject data)
        {
            CardsType = type;
            var cards = data.GetIntArray(ConstantData.KeyCards);
            if (cards!=null)
            {
                Cards = cards.ToList();
            }
            Result = data.GetInt(ConstantData.KeyValue);
            DoubleCard = data.GetBool(ConstantData.KeyDoubleCard);
            CardNum = data.GetInt(ConstantData.KeyCardNum);
            King = data.GetBool(ConstantData.KeyKing);
        }

        public CardsData(CardResultType type,int value,bool doubleCard,int cardNum,bool king,int[] getCards)
        {
            Cards = getCards.ToList();
            CardsType = type;
            Result = value;
            DoubleCard = doubleCard;
            CardNum = cardNum;
            King = king;
        }
    }

    /// <summary>
    /// 牌结果类型
    /// </summary>
    public enum CardResultType
    {
        /// <summary>
        /// 闲
        /// </summary>
        Xian,
        /// <summary>
        /// 庄
        /// </summary>
        Zhuang,
    }
}
