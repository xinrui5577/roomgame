using System.Collections.Generic;
using Assets.Scripts.Game.GangWu.Main;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Common;

namespace Assets.Scripts.Game.GangWu.Mgr
{
    /// <summary>
    /// 发牌类
    /// </summary>
    public class DealerMgr : MonoBehaviour
    {
      
        // Update is called once per frame
        protected void Update ()
        {
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
        /// 大牌发牌索引
        /// </summary>
        [HideInInspector]
        public int BigIndex;

        /// <summary>
        /// 小盲注的位置索引
        /// </summary>
        //private int _smallD = 0;

#region 发手牌
       
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
            gob.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            gob.GetComponent<PokerCard>().SetCardId(cardValue);

            gob.GetComponent<TweenPosition>().duration = /*0.15f;/*/0.25f;//*/
            gob.GetComponent<TweenPosition>().from = gob.transform.localPosition;
            gob.GetComponent<TweenPosition>().to = Vector3.zero;
            gob.GetComponent<TweenPosition>().ResetToBeginning();
            gob.GetComponent<TweenPosition>().PlayForward();

            return gob;
        }


        /// <summary>
        /// 发一张牌 无过程
        /// </summary>
        private GameObject DealOnes(Transform to, int cardValue = 0, int index = 0)
        {
            GameObject gob = Instantiate(PokerPrefab);
            gob.transform.parent = to;
            gob.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            gob.transform.localScale *= 1/to.localScale.x;
            gob.transform.localPosition = Vector3.zero;
            PokerCard pCard = gob.GetComponent<PokerCard>();
            pCard.SetCardDepth(100 + index * 2);
            pCard.SetCardId(cardValue);
            pCard.SetCardFront();
          
            App.GetGameManager<GangWuGameManager>().PublicPokers.Add(pCard);
            return gob;
        }

        /// <summary>
        /// 发送一个人的手牌,无过程
        /// </summary>
        /// <param name="pokerValues"></param>
        /// <param name="pokerTrans"></param>
        /// <param name="seatIndex"></param>
        public void DealOnesPokers(int[] pokerValues,Transform[] pokerTrans,int seatIndex)
        {

            if (pokerValues.Length > BigIndex)
            {
                BigIndex = pokerValues.Length;
            }
          
            for (int i = 0; i < pokerValues.Length; i++)
            {
                int cardValue = i < 1 ? 0 : pokerValues[i];
                DealOnes(pokerTrans[i], cardValue, i);
            }

        }


        #endregion

        #region 发公共牌

        private readonly Queue<PokerInfo> _showPokersInfo = new Queue<PokerInfo>();
       

        
        /// <summary>
        /// 发手牌间隔时间
        /// </summary>
        public float BigSpace;
        /// <summary>
        /// 手牌计时器
        /// </summary>
        private float _bigTimer;

        
        /// <summary>
        /// 发公共牌update
        /// </summary>
        private void BigDealUpdate()
        {
            _bigTimer += Time.deltaTime;
            if (!IsBigDeal || _bigTimer < BigSpace)
                return;

            _bigTimer = 0f;


            //新发牌方式
            DoBigDeal();
            if(_showPokersInfo.Count <= 0)
            {
                YxDebug.Log("发牌结束!!");
                IsBigDeal = false;
            }
        }

        

        private void DoBigDeal()
        {
            var gdata = App.GetGameData<GangwuGameData>();
            PokerInfo pokerInfo = _showPokersInfo.Dequeue();
            Transform toPos = gdata.GetPlayer<PlayerPanel>(pokerInfo.Seat,true).PokersTrans[pokerInfo.PokerCount];
            GameObject dealPoker = DealOnes(BigBirth, toPos, pokerInfo.PokerValue, pokerInfo.PokerCount);

            //显示牌正面
            PokerCard pokerCard = dealPoker.GetComponent<PokerCard>();
            pokerCard.SetCardFront();

            //将牌放入公共牌堆,方便管理
            App.GetGameManager<GangWuGameManager>().PublicPokers.Add(pokerCard);

            Facade.Instance<MusicManager>().Play("dealer");

        }

       

        public void BeginBigDeal(int[] pokers, int[] seats, int cardCount)
        {
            if (_showPokersInfo.Count <= 0)
                _showPokersInfo.Clear();

            for (int i = 0; i < pokers.Length; i++)
            {
                PokerInfo pokerInfo = new PokerInfo
                {
                    Seat = seats[i],
                    PokerCount = cardCount,
                    PokerValue = pokers[i]
                };
                _showPokersInfo.Enqueue(pokerInfo);
            }

            IsBigDeal = true;
        } 

        /// <summary>
        /// 当收到结算信息时,进行紧急处理
        /// </summary>
        public void OnResult()
        {
            var gdata = App.GameData;
            if (_showPokersInfo.Count <= 0)
                return;

            IsBigDeal = false;              //停止有动画的发牌

            PokerInfo pinfo = _showPokersInfo.Dequeue();

            int cardIndex = pinfo.PokerCount;// BigIndex / _playerSeats.Length;                                        //发到了第几张手牌
            int seatIndex = pinfo.Seat; // _playerSeats[(BigIndex + _fristSeat) % _playerSeats.Length];           //发到了第几个人

            Transform toPos = gdata.GetPlayer<PlayerPanel>(seatIndex,true).PokersTrans[cardIndex];

            int cardValue;

            //第一张是个人牌,从第二张开始才是公共牌
            if (seatIndex == gdata.SelfSeat)
            {
                cardValue = cardIndex < 1 ? gdata.GetPlayer<PlayerPanel>().UserBetPoker.LeftCardValue : pinfo.PokerValue;// _showPokers.Dequeue();
            }
            else
            {
                cardValue = cardIndex < 1 ? 0 : pinfo.PokerValue; // _showPokers.Dequeue();
            }

            GameObject dealPoker = DealOnes(toPos, cardValue, cardIndex);
            //_cardIndex

            dealPoker.GetComponent<PokerCard>().SetCardFront();

            Facade.Instance<MusicManager>().Play("dealer");

            App.GetGameManager<GangWuGameManager>().PublicPokers.Add(dealPoker.GetComponent<PokerCard>());
        }



        public void Rest()
        {
            IsBigDeal = false;
            BigIndex = 0;
           
            _showPokersInfo.Clear();
        }

        #endregion
    }

}

public struct PokerInfo
{
    public int PokerCount;
    public int Seat;
    public int PokerValue;
}
