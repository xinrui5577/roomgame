using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Manager;
using System;
using Assets.Scripts.Game.duifen.ImgPress.Main;
using YxFramwork.Common;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.duifen.Mgr
{
    /// <summary>
    /// 发牌类
    /// </summary>
    public class DealerMgr : MonoBehaviour
    {
        
        // Use this for initialization
        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once UnusedMember.Local
        void Start ()
        {

        }
	
        // Update is called once per frame
        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once UnusedMember.Local
        void Update ()
        {
            //SmallDealUpdate();
            BigDealUpdate();
        }

        /// <summary>
        /// 是否正在发手牌
        /// </summary>
        [HideInInspector]
        public bool IsSmallDeal;
        /// <summary>
        /// 是否正在发公共牌
        /// </summary>
        [HideInInspector]
        public bool IsBigDeal;
        /// <summary>
        /// 公共牌出生点
        /// </summary>
        public Transform BigBirth;
        /// <summary>
        /// 手牌出生点
        /// </summary>
        public Transform SmallBirth;
        /// <summary>
        /// 扑克预设
        /// </summary>
        public GameObject PokerPrefab;
        /// <summary>
        /// 手牌发牌索引
        /// </summary>
        //[HideInInspector]
        //public int SmallIndex;
  

        /// <summary>
        /// 本回合第一个发牌的人
        /// </summary>
        public int FirstSeat { get; set; }

        private readonly Queue<PokerInfo> _dealingCards = new Queue<PokerInfo>();

        /// <summary>
        /// 小盲注的位置索引
        /// </summary>
        //private int _smallD = 0;

        #region     发手牌


        /// <summary>
        /// 发一张牌 有过程
        /// </summary>
        private GameObject DealOnes(Transform from, Transform to,int cardValue = 0,int index = 0)
        {
            GameObject gob = Instantiate(PokerPrefab);
            gob.transform.parent = from;
            gob.transform.localPosition = Vector3.zero;
            PokerCard pCard = gob.GetComponent<PokerCard>();
            pCard.SetCardDepth(100 + index * 2);
            gob.transform.parent = to;
            gob.GetComponent<PokerCard>().SetCardId(cardValue);

            TweenPosition tp = gob.GetComponent<TweenPosition>();
            tp.duration = 0.15f;//0.25f;
            tp.from = gob.transform.localPosition;
            tp.to = Vector3.zero;
            tp.ResetToBeginning();
            tp.PlayForward();

            TweenScale ts = gob.GetComponent<TweenScale>();
            ts.duration = 0.15f;
            ts.from = Vector3.one * 0.3f;
            ts.to = Vector3.one * 0.76f;
            ts.ResetToBeginning();
            ts.PlayForward();

            return gob;
        }

 

        /// <summary>
        /// 发一张牌,无过程
        /// </summary>
        /// <param name="to"></param>
        /// <param name="cardValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private PokerCard DealOnes(Transform to, int cardValue = 0, int index = 0)
        {
            GameObject gob = Instantiate(PokerPrefab);
            gob.transform.parent = to;
            gob.transform.localScale = Vector3.one * 0.76f;
            gob.transform.localScale *= 1/to.localScale.x;
            gob.transform.localPosition = Vector3.zero;
            PokerCard pCard = gob.GetComponent<PokerCard>();
            pCard.SetCardDepth(100 + index * 2);
            pCard.SetCardId(cardValue);
            pCard.SetCardFront();
            
            App.GetGameManager<DuifenGameManager>().PublicPokers.Add(pCard);
            return pCard;
        }

        /// <summary>
        /// 发送一个人的手牌,无过程
        /// </summary>
        /// <param name="pokerValues"></param>
        /// <param name="panel"></param>
        public void DealOnesPokers(int[] pokerValues,DuifenPlayerPanel panel)
        {
            Transform[] pokerTrans = panel.PokersTrans;
            for (int i = 0; i < pokerValues.Length; i++)
            {
                int cardValue = pokerValues[i];
                var pokerCard = DealOnes(pokerTrans[i], cardValue, i);
                panel.UserBetPoker.AddPoker(pokerCard);
                pokerCard.SetDiPaiMark(i <= 1);
            }

            panel.ShowPointLabel();
        }

        /// <summary>
        /// 发出接受到的所有牌
        /// </summary>
        public void FastDeal()
        {
            if (_dealingCards.Count <= 0)
                return;

            IsBigDeal = false;

            int cardCount = _dealingCards.Count;
            for (int i = 0; i < cardCount; i++)
            {
                PokerInfo pokerInfo = _dealingCards.Dequeue();
                var panel = App.GameData.GetPlayer<DuifenPlayerPanel>(pokerInfo.Seat, true);
                if(panel.HaveThisCard(pokerInfo.CardIndex))
                {
                    panel.TurnCard(pokerInfo.CardIndex, pokerInfo.PokerVal, false);
                }
                else
                {
                    PokerCard poker = DealOnes(panel.PokersTrans[pokerInfo.CardIndex], pokerInfo.PokerVal, pokerInfo.CardIndex);
                    panel.AddPoker(poker);
                }

                panel.ShowBetLabel();
            }

        }

        #endregion

        #region 发公共牌



        public void BeginBigDeal(int[] pokers,int[] seats,int count,Action finish = null)
        {
            if (_dealingCards.Count <= 0)
                _dealingCards.Clear();

            int pokersLength = pokers.Length;
            int seatsLength = seats.Length;

            //数据存储处理
            for (int i = 0; i < pokersLength; i++)
			{
                PokerInfo pokerInfo = new PokerInfo
                {
                    Seat = seats[i % seatsLength],
                    PokerVal = pokers[i],
                    CardIndex = count,
                };
                _dealingCards.Enqueue(pokerInfo);
			}

            IsBigDeal = true;   //开始发牌

            OnFinish = finish;
        }

        public void BeginBigDeal(int poker, int seat, int count, Action finish = null)
        {
            if (_dealingCards.Count <= 0)
                _dealingCards.Clear();

            //数据存储处理
           
            PokerInfo pokerInfo = new PokerInfo
            {
                Seat = seat,
                PokerVal = poker,
                CardIndex = count,
            };

            _dealingCards.Enqueue(pokerInfo);

            IsBigDeal = true;   //开始发牌

            OnFinish = finish;
        }


        /// <summary>
        /// 发手牌间隔时间
        /// </summary>
        public float BigSpace;

        /// <summary>
        /// 手牌计时器
        /// </summary>
        private float _bigCount;

      
        /// <summary>
        /// 发公共牌update
        /// </summary>
        private void BigDealUpdate()
        {
            _bigCount += Time.deltaTime;
            if (!IsBigDeal || _bigCount < BigSpace)
                return;

            _bigCount = 0f;
            BigDeal();
            if (_dealingCards.Count <= 0)
            {
                YxDebug.Log("显示牌牌结束!!");
                IsBigDeal = false;
                // ReSharper disable once UseNullPropagation
                if (OnFinish != null) OnFinish();
            }
        }

        public Action OnFinish;

        /// <summary>
        /// 发到了第几张手牌
        /// </summary>
        [HideInInspector]
        public int DealerCardIndex = 0;

        /// <summary>
        /// 发到了第几个人
        /// </summary>
        [HideInInspector]
        public int DealerSeatIndex = 0;

        [HideInInspector]
        public int[] SelfCards;

        public void SetFirstTwoPokersValue(int[] cards)
        {
            SelfCards = new int[cards.Length];
            for (int i = 0; i < SelfCards.Length; i++)
            {
                SelfCards[i] = cards[i];
            }
        }

        private void BigDeal()
        {
            if (_dealingCards.Count <= 0) return;

            var gdata = App.GameData;
            PokerInfo pokerInfo = _dealingCards.Dequeue();          //弹出要发的牌
            
            int cardIndex = pokerInfo.CardIndex;                        //发到了第几张手牌
            int cardSeat = pokerInfo.Seat;                              //发牌人座位号

            var duifenPlayerPanel = gdata.GetPlayer<DuifenPlayerPanel>(cardSeat, true);
            Transform toPos = duifenPlayerPanel.PokersTrans[cardIndex % duifenPlayerPanel.PokersTrans.Length];

            if(duifenPlayerPanel.HaveThisCard(cardIndex))
            {
                duifenPlayerPanel.TurnCard(cardIndex, pokerInfo.PokerVal);
            }
            else
            {
                GameObject dealPoker = DealOnes(BigBirth, toPos, pokerInfo.PokerVal, cardIndex);  //发牌
                var pokerCard = dealPoker.GetComponent<PokerCard>();
                pokerCard.SetCardFront();
                duifenPlayerPanel.AddPoker(pokerCard);
                Facade.Instance<MusicManager>().Play("dealer");
                App.GetGameManager<DuifenGameManager>().PublicPokers.Add(pokerCard);
                pokerCard.SetDiPaiMark(cardIndex <= 1);
            }
        }

        public void DealFirstTwoPoker(int[] seats,int selfSeat)
        {
            int seatsLength = seats.Length;
            for (int i = 0; i < SelfCards.Length; i++)
            {
                int[] cards = new int[seatsLength];
                for (int j = 0; j < seatsLength; j++)
                {
                    cards[j] = seats[j] == selfSeat ? SelfCards[i] : 0;
                }
                BeginBigDeal(cards, seats, i);
            }
        }

       
        public void Rest()
        {
            IsBigDeal = false;
            FirstSeat = 0;
            _dealingCards.Clear();
        }

        #endregion

    }

    public struct PokerInfo
    {
        public int PokerVal;
        public int Seat;
        public int CardIndex;
    }

}
