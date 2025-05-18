using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Manager;
using System;
using System.Collections;
using Assets.Scripts.Game.paijiu.ImgPress.Main;
using YxFramwork.Common;
using YxFramwork.Framework.Core;

#pragma warning disable 414

namespace Assets.Scripts.Game.paijiu.Mgr
{
    /// <summary>
    /// 发牌类
    /// </summary>
    public class DealerMgr : MonoBehaviour
    {
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
        public GameObject CardPrefab;

        /// <summary>
        /// 每次发牌的张数
        /// </summary>
        private int _dealCount = 1;

        /// <summary>
        /// 本回合第一个发牌的人
        /// </summary>
        public int FirstSeat { protected get; set; }

        private readonly Queue<PaiJiuCardInfo> _dealingCards = new Queue<PaiJiuCardInfo>();


        #region     发手牌



        private void BigDeal()
        {
            var main = App.GetGameManager<PaiJiuGameManager>();
            var gdata = App.GetGameData<PaiJiuGameData>();
            Debug.Log("===== dealcount == " + _dealCount + " , length = " + _dealingCards.Count + " ===== ");
            for (int i = 0; i < _dealCount; i++)
            {
                PaiJiuCardInfo pokerInfo = _dealingCards.Dequeue();         //弹出要发的牌
                int cardIndex = pokerInfo.CardIndex;                        //发到了第几张手牌
                int cardSeat = pokerInfo.Seat;                              //发牌人座位号
                PaiJiuPlayer paiJiuPlayer = gdata.GetPlayer<PaiJiuPlayer>(cardSeat, true);
                Transform toPos = paiJiuPlayer.PokersTrans[cardIndex % paiJiuPlayer.PokersTrans.Length];

                GameObject dealPoker = DealOnes(BigBirth, toPos, pokerInfo.PokerVal, cardSeat == App.GameData.SelfSeat, cardIndex);  //发牌
                PaiJiuCard pokerCard = dealPoker.GetComponent<PaiJiuCard>();
                pokerCard.SetCardFront();
                paiJiuPlayer.AddPoker(pokerCard);
                Facade.Instance<MusicManager>().Play("dealer");
                main.PublicPokers.Add(pokerCard);
            }
        }

        /// <summary>
        /// 发一张牌 有过程
        /// </summary>
        private GameObject DealOnes(Transform from, Transform to, int cardValue = 0, bool isSelf = false, int index = 0)
        {
            GameObject gob = Instantiate(CardPrefab);
            gob.transform.parent = from;
            gob.transform.localPosition = Vector3.zero;
            PaiJiuCard pCard = gob.GetComponent<PaiJiuCard>();
            pCard.SetCardDepth(100 + index * 2);
            gob.transform.parent = to;
            pCard.SetCardId(cardValue);
            pCard.SetCardFront();
            pCard.CardIndex = index;
            pCard.CardCouldClick(isSelf);

            TweenPosition tp = gob.GetComponent<TweenPosition>();
            tp.duration = 0.15f;//0.25f;
            tp.from = gob.transform.localPosition;
            tp.to = Vector3.zero;
            tp.ResetToBeginning();
            tp.PlayForward();

            TweenScale ts = gob.GetComponent<TweenScale>();
            ts.duration = 0.15f;
            ts.from = Vector3.one * 0.3f;
            ts.to = Vector3.one * 0.8f;
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
        /// <param name="isSelf"></param>
        /// <returns></returns>
        private PaiJiuCard DealOnes(Transform to, int cardValue = 0, int index = 0, bool isSelf = false)
        {
            GameObject gob = Instantiate(CardPrefab);
            gob.transform.parent = to;
            gob.transform.localScale = Vector3.one * 0.8f;
            gob.transform.localPosition = Vector3.zero;
            PaiJiuCard pCard = gob.GetComponent<PaiJiuCard>();
            pCard.SetCardDepth(100 + index * 2);
            pCard.CardIndex = index;
            pCard.SetCardId(cardValue);
            pCard.SetCardFront();
            pCard.CardCouldClick(isSelf);

            App.GetGameManager<PaiJiuGameManager>().PublicPokers.Add(pCard);
            return pCard;
        }


        /// <summary>
        /// 发送一个人的手牌,无过程
        /// </summary>
        /// <param name="pokerValues"></param>
        /// <param name="panel"></param>
        public void DealOnesPokers(int[] pokerValues, PaiJiuPlayer panel)
        {
            Transform[] pokerTrans = panel.PokersTrans;

            for (int i = 0; i < pokerValues.Length; i++)
            {
                var cardValue = pokerValues[i];
                var pokerCard = DealOnes(pokerTrans[i], cardValue, i, panel.Equals(App.GetGameData<PaiJiuGameData>().GetPlayer()));
                panel.UserBetPoker.AddPoker(pokerCard);
            }
        }


        #endregion

        #region 发公共牌



        public void BeginBigDeal(int[] val, int dealCount = 4, Action finish = null)
        {
            _dealCount = dealCount;

            var gdata = App.GetGameData<PaiJiuGameData>();

            var panels = gdata.PlayerList;
            int bankerSeat = App.GetGameData<PaiJiuGameData>().BankerSeat;
            //一次发4张牌,所有人都发
            for (int i = 0; i < panels.Length; i++)
            {
                int dealSeat = (i + bankerSeat + 1) % panels.Length;    //从初门开始发牌
                for (int j = 0; j < dealCount; j++)
                {
                    PaiJiuCardInfo cardInfo = new PaiJiuCardInfo
                    {
                        Seat = dealSeat,
                        PokerVal = dealSeat == App.GameData.SelfSeat ? val[j] : 0,
                        CardIndex = j
                    };
                    _dealingCards.Enqueue(cardInfo);
                }
            }

            if (!IsBigDeal)
            {
                IsBigDeal = true;
                StartCoroutine(DealWinAnim());
            }
            OnFinish = finish;
        }

        /// <summary>
        /// 发了多少个玩家的牌
        /// </summary>
        private int _dealingIndex;
        IEnumerator DealWinAnim()
        {
            var main = App.GetGameManager<PaiJiuGameManager>();
            while (IsBigDeal && _dealingCards.Count > 0)
            {
                BigDeal();
                yield return new WaitForSeconds(0.5f);
            }

            StopAllCoroutines();
            if (IsBigDeal)
            {
                main.SpeakMgr.ShowSpeak(GameRequestType.SendCard);
            }
            IsBigDeal = false;
            DealFinish();
        }

        /// <summary>
        /// 发出接受到的所有牌
        /// </summary>
        public void FastDeal()
        {
            if (_dealingCards.Count <= 0)
                return;
            int selfSeat = App.GameData.SelfSeat;

            var gdata = App.GetGameData<PaiJiuGameData>();

            int cardCount = _dealingCards.Count;
            for (int i = 0; i < cardCount; i++)
            {
                PaiJiuCardInfo pokerInfo = _dealingCards.Dequeue();

                PaiJiuPlayer panel = gdata.GetPlayer<PaiJiuPlayer>(pokerInfo.Seat, true);
                PaiJiuCard card = DealOnes(panel.PokersTrans[pokerInfo.CardIndex], pokerInfo.PokerVal, pokerInfo.CardIndex, pokerInfo.Seat == selfSeat);
                panel.AddPoker(card);
            }
        }

        void DealFinish()
        {
            if (OnFinish != null)
                OnFinish();
        }

        public void OnCompare()
        {
            IsBigDeal = false;        //取消发完牌后显示组牌按钮
            StopAllCoroutines();      //停止发牌动画
            FastDeal();               //快速发牌
        }


        /// <summary>
        /// 发手牌间隔时间
        /// </summary>
        public float BigSpace;

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



        public void Rest()
        {
            IsBigDeal = false;
            FirstSeat = 0;
            _dealingIndex = 0;
            StopAllCoroutines();
        }

        #endregion

    }

    public struct PaiJiuCardInfo
    {
        public int PokerVal;
        public int Seat;
        public int CardIndex;
    }

}
