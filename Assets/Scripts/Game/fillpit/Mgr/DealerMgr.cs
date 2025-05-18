using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
// ReSharper disable UnusedMember.Local

namespace Assets.Scripts.Game.fillpit.Mgr
{
    /// <summary>
    /// 发牌类
    /// </summary>
    public class DealerMgr : MonoBehaviour
    {

        [Range(0,1)]
        public float SelfCardScale = 0.7f;

        [Range(0,1)]
        public float OtherCardScale = 0.4f;

        // Update is called once per frame
        void Update ()
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
        /// 大牌发牌索引
        /// </summary>
        [HideInInspector]
        public int BigIndex;

        /// <summary>
        /// 隐藏牌的个数
        /// </summary>
        public int HideN = 2;

        /// <summary>
        /// 本回合第一个发牌的人
        /// </summary>
        public int FirstSeat { private get; set; }

     

        #region     发手牌
        /// <summary>
        /// 发一张牌 有过程
        /// </summary>
        protected virtual GameObject DealOnes(Transform from, Transform to,int cardValue = 0,bool isBig = false,int index = 0)
        {
            GameObject gob = Instantiate(PokerPrefab);
            gob.transform.parent = from;
            gob.transform.localPosition = Vector3.zero;
            PokerCard pCard = gob.GetComponent<PokerCard>();
            pCard.SetCardDepth(100 + index * 2);
            gob.transform.parent = to;
            gob.transform.localScale = /*isBig ? Vector3.one * SelfCardScale :*/ Vector3.one;
            gob.GetComponent<PokerCard>().SetCardId(cardValue);

            var comp = gob.GetComponent<TweenPosition>();
            comp.duration = 0.15f;//0.25f;
            comp.from = gob.transform.localPosition;
            comp.to = Vector3.zero;
            comp.ResetToBeginning();
            comp.PlayForward();
            return gob;
        }

        /// <summary>
        /// 发一张牌,无过程
        /// </summary>
        /// <param name="to"></param>
        /// <param name="cardValue"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        protected virtual void DealOnes(Transform to, int cardValue = 0, int depth = 0)
        {
            GameObject gob = Instantiate(PokerPrefab);
            gob.transform.parent = to;
            gob.transform.localScale = Vector3.one;
            gob.transform.localScale *= 1/to.localScale.x;
            gob.transform.localPosition = Vector3.zero;
            PokerCard pCard = gob.GetComponent<PokerCard>();
            pCard.SetCardDepth(100 + depth * 2);
            pCard.SetCardId(cardValue);
            pCard.SetCardFront();
            if (PublicCardId > 0)
            {
                pCard.SetPublicMarkActive(PublicCardId == cardValue);
            }
            App.GetGameManager<FillpitGameManager>().PublicPokers.Add(pCard);
        }

        /// <summary>
        /// 发送一个人的手牌,无过程
        /// </summary>
        /// <param name="pokerValues"></param>
        /// <param name="panel"></param>
        /// <param name="seatIndex"></param>
        public virtual void DealOnesPokers(int[] pokerValues,PlayerPanel panel,int seatIndex)
        {
            if (pokerValues.Length > BigIndex)
            {
                BigIndex = pokerValues.Length;
            }
            var pokerTrans = panel.PokersTrans;
            for (int i = 0; i < pokerValues.Length; i++)
            {
                int cardValue = i < HideN ? 0 : pokerValues[i];
                DealOnes(pokerTrans[i], cardValue, i);
            }
        }
        #endregion


        #region 发公共牌

        /// <summary>
        /// 需要发的牌
        /// </summary>
        [HideInInspector]
        public Queue<DealPokerInfo> DealCards = new Queue<DealPokerInfo>();

        //private Queue<Transform> _userTransform = null;


        /// <summary>
        /// 公共牌,多人共用牌
        /// </summary>
        [HideInInspector]
        public int PublicCardId = -1;

        /// <summary>
        /// 发给的第一个人的作为索引
        /// </summary>
        private int _fristIndex;

        /// <summary>
        /// 公开牌,当存在selfCard时,重置发牌索引.发牌与座位一一对应
        /// </summary>
        /// <param name="pokers">要发的牌</param>
        /// <param name="seats">得牌的座位</param>
        public void BeginBigDeal(int[] pokers, int[] seats)
        {
            _fristIndex = GetFristCardIndex(seats);
            
            for (int i = 0; i < seats.Length; i++)
            {
                int index = (i + _fristIndex)%seats.Length;
                DealPokerInfo pokerInfo = new DealPokerInfo
                {
                    PokerVal = pokers[index],
                    Seat = seats[index],
                    Index = BigIndex
                };
                DealCards.Enqueue(pokerInfo);
            }
            ++BigIndex;
            IsBigDeal = true;
        }

        /// <summary>
        /// 获取第一个发牌的人的索引
        /// </summary>
        /// <returns></returns>
        int GetFristCardIndex(int[] seats)
        {
            if (FirstSeat == 0)
            {
                return 0;
            }

            for (int i = 0; i < seats.Length; i++)
            {
                if (seats[i] == FirstSeat)
                {
                    return i;
                }
            }
            return 0;
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
            //if (DispensePokers.Count == 0)
            if (DealCards.Count == 0)
            {
                YxDebug.Log("发牌结束!!");
                App.GameData.GetPlayer<PlayerPanel>().SetSeePokerBtnActive(true);
                IsBigDeal = false;
            }
        }

        /// <summary>
        /// 快速发掉所有堆栈中的牌
        /// </summary>
        public virtual void FastDeal()
        {
            if (DealCards == null || DealCards.Count < 1) return;
            IsBigDeal = false;
            while(DealCards.Count > 0)
            {
                var card = DealCards.Dequeue();
                var panel = App.GetGameData<FillpitGameData>().GetPlayer<PlayerPanel>(card.Seat, true);
                var to = panel.PokersTrans[card.Index];
                DealOnes(to, card.PokerVal, card.Index);
            }
        }


        protected virtual void BigDeal()
        {
            if (DealCards.Count <= 0) return;

            var card = DealCards.Dequeue();

            Transform toPos =
                App.GetGameData<FillpitGameData>().GetPlayer<PlayerPanel>(card.Seat, true).PokersTrans[card.Index];

            GameObject dealPoker = DealOnes(BigBirth, toPos, card.PokerVal, true, card.Index);

            dealPoker.GetComponent<PokerCard>().SetCardFront();

            if (PublicCardId > 0)
            {
                dealPoker.GetComponent<PokerCard>().SetPublicMarkActive(card.PokerVal == PublicCardId);
            }

            Facade.Instance<MusicManager>().Play("dealer");

            App.GetGameManager<FillpitGameManager>().PublicPokers.Add(dealPoker.GetComponent<PokerCard>());
        }

        public void Reset()
        {
            IsBigDeal = false;
            PublicCardId = -1;
            BigIndex = 0;
            FirstSeat = 0;
            DealCards.Clear();
        }

        #endregion

    }

    public class DealPokerInfo
    {
        public int PokerVal;
        public int Seat;
        public int Index ;
    }


}
