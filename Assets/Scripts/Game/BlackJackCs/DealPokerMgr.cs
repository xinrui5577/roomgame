using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.BlackJackCs
{
    public class DealPokerMgr : MonoBehaviour
    {

        public Transform BrithTran;

        public Transform HideTran;

        public Transform To;

        /// <summary>
        /// Tween方法运行的时间长度
        /// </summary>
        [Tooltip("Tween方法运行的时间长度")]
        [SerializeField]
        private float _runTime = 0.5f;


        /// <summary>
        /// 扑克的预制体
        /// </summary>
        public GameObject PokerPrefab;

        bool _isDealing;

        /// <summary>
        /// 扑克数值队列
        /// </summary>
        readonly Queue<int> _pokers = new Queue<int>();

        /// <summary>
        /// 发牌玩家座位号队列
        /// </summary>
        //readonly Queue<int> _seats = new Queue<int>();

        readonly List<GameObject> _pokerList = new List<GameObject>();

        private readonly Queue<BjPlayerPanel> _dealPanels = new Queue<BjPlayerPanel>();

        //测试代码
        //protected void Update()
        //{
            //if (Input.GetMouseButtonDown(1))
            //{
            //    int[] seats = new int[] { 0, 1, 2, 3, 4 };
            //    int[] pokers = new int[] { 33, 34, 35, 36, 37, 38, 39, 40, 41, 42 };
            //    //int[] pokers = new int[] { 33, 34, 35, 36, 37, };
            //    EnqueueCardInfo(seats, pokers);
            //    //DealPokerNoAnim(1, pokers);
            //}

            //if (Input.GetMouseButtonDown(2))
            //{
            //    int[] seats = new int[] { 0 };
            //    int[] pokers = new int[] { 55 };
            //    EnqueueCardInfo(seats, pokers);
            //}

            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    Reset();
            //}
        //}

        /// <summary>
        /// 发牌,有过程
        /// </summary>
        /// <param name="seats">座位数组</param>
        /// <param name="cards">牌值数组</param>
        public void EnqueueCardInfo(int[] seats, int[] cards)
        {
            int seatLength = seats.Length;
            var gdata = App.GameData;
            for (int i = 0; i < cards.Length;i++ )
            {
                int seat = seats[i%seatLength];
                //_seats.Enqueue(seat);
                _dealPanels.Enqueue(gdata.GetPlayer<BjPlayerPanel>(seat, true));
                _pokers.Enqueue(cards[i]);
            }
        }

        public void EnqueueCardInfo(BjPlayerPanel panel, int[] cards)
        {
            int len = cards.Length;
            for (int i = 0; i < len; i++)
            {
                EnqueueCardInfo(panel, cards[i]);
            }
        }

        public void EnqueueCardInfo(BjPlayerPanel panel, int card)
        {
            _dealPanels.Enqueue(panel);
            _pokers.Enqueue(card);
        }


        /// <summary>
        /// 开始发牌
        /// </summary>
        public void BeginDealCards()
        {
            if (_isDealing)
                return;
            StartCoroutine(BegintoDeal());
        }

        /// <summary>
        /// 发牌,有过程
        /// </summary>
        /// <param name="seat">座位数组</param>
        /// <param name="card">牌值数组</param>
        public void EnqueueCardInfo(int seat, int card)
        {
            //_seats.Enqueue(seat);
            _dealPanels.Enqueue(App.GameData.GetPlayer<BjPlayerPanel>(seat, true));
            _pokers.Enqueue(card);
            
            if (_isDealing)
                return;
            StartCoroutine(BegintoDeal());
        }

        /// <summary>
        /// 发牌协程
        /// </summary>
        /// <returns></returns>
        // ReSharper disable once FunctionRecursiveOnAllPaths
        IEnumerator BegintoDeal()
        {
            _isDealing = true;
            //var gdata = App.GameData;
            while (_pokers.Count > 0)
            {
                BjPlayerPanel panel = _dealPanels.Dequeue(); //gdata.GetPlayer<BjPlayerPanel>(_seats.Dequeue(), true);   // blackjackGameManager.GetInstance().BjSeatSort[_seats.Dequeue()];
                int pokerId = _pokers.Dequeue();
                int index = panel.PokerCount;
                panel.CardsId[index] = panel.CardsId[index] > 0 ? panel.CardsId[index] : pokerId;
                GameObject gob = DoDealOnePokerWithAnim(BrithTran,
                    panel.GetPokerTran,
                    index, index, panel.CardsId[index]);
                _pokerList.Add(gob);
                gob.GetComponent<PokerCard>().Owner = panel;
                yield return new WaitForSeconds(0.3f);
            }

            _isDealing = false;
        }


        /// <summary>
        /// 发一张牌,有过程
        /// </summary>
        /// <param name="from">起始位置</param>
        /// <param name="to">目标位置</param>
        /// <param name="index"></param>
        /// <param name="depth">牌层级</param>
        /// <param name="cardId"></param>
        /// <param name="onFinish">动画结束时的执行方法</param>
        /// <returns></returns>
        public GameObject DoDealOnePokerWithAnim(Transform from, Transform to, int index, int depth,int cardId, Action onFinish = null)
        {

            GameObject gob = Instantiate(PokerPrefab);
            PokerCard pCard = gob.GetComponent<PokerCard>();
            pCard.SetCardDepth(100 + depth * 2);
            pCard.Index = index;
            gob.transform.parent = to;
            gob.transform.position = from.position;
            gob.transform.localScale = Vector3.one;

            SpringPosition sp = gob.GetComponent<SpringPosition>();
            sp.target = to.position;
            sp.enabled = true;
            sp.onFinished = (() =>
            {
                UIGrid grid = to.GetComponentInParent<UIGrid>();

                if (grid != null)
                {
                    grid.Reposition();
                }

                if (onFinish != null)
                    onFinish();

                //当传入牌值为0时，是牌的背面，所以不翻牌
                if(cardId != 0)
                {
                    pCard.SetCardId(cardId);
                    pCard.TurnCard();
                }
            });


            TweenAlpha ta = gob.GetComponent<TweenAlpha>();
            ta.from = 0.5f;
            ta.to = 1;
            ta.duration = _runTime;
            ta.ResetToBeginning();
            ta.PlayForward();


            TweenRotation tr = gob.GetComponent<TweenRotation>();
            tr.from = new Vector3(0, 0, -66);
            tr.to = Vector3.zero;
            tr.duration = _runTime;
            tr.ResetToBeginning();
            tr.PlayForward();

            return gob;

        }

        

        /// <summary>
        /// 发牌,无动画
        /// </summary>
        /// <param name="seat">发牌玩家座位</param>
        /// <param name="cards">发牌玩家手牌</param>
        public void DealPokerNoAnim(int seat,int[] cards)
        {
            var panel = App.GameData.GetPlayer<BjPlayerPanel>(seat, true);     // blackjackGameManager.GetInstance().BjSeatSort[seat];
            DealPokerNoAnim(panel, cards);
        }

        public void DealPokerNoAnim(BjPlayerPanel panel, int[] cards)
        {
            foreach (int v in cards)
            {
                panel.CardsId[panel.PokerCount] = v;
                DealOnePokerNoAnim(panel.GetPokerTran, v, panel.PokerCount);
            }
            panel.CheckCardPoint();
        }


        /// <summary>
        /// 发一张牌，无过程
        /// </summary>
        /// <param name="to">目标位置</param>
        /// <param name="card">牌的数值</param>
        /// <param name="depth">牌的层级</param>
        void DealOnePokerNoAnim(Transform to, int card, int depth)
        {
            GameObject gob = Instantiate(PokerPrefab);
            _pokerList.Add(gob);

            gob.transform.parent = to.transform;
            gob.transform.localScale = Vector3.one;
            gob.transform.localPosition = Vector3.zero;

            PokerCard pCard = gob.GetComponent<PokerCard>();
            pCard.SetCardId(card);
            pCard.SetCardFront();
            pCard.SetCardDepth(100 + depth);

            to.gameObject.GetComponentInParent<UIGrid>().Reposition();
        }

        public void DealAllPoker()
        {
            _isDealing = false;
            //var gdata = App.GameData;
            while (_pokers.Count > 0)
            {
                var panel = _dealPanels.Dequeue(); //gdata.GetPlayer<BjPlayerPanel>(_seats.Dequeue()); //blackjackGameManager.GetInstance().BjSeatSort[_seats.Dequeue()];
                int pokerId = _pokers.Dequeue();
                int index = panel.PokerCount;
                panel.CardsId[index] = panel.CardsId[index] > 0 ? panel.CardsId[index] : pokerId;
                DealOnePokerNoAnim(panel.GetPokerTran, pokerId, panel.PokerCount);
            }

            StopCoroutine("BegintoDeal");
        }

        
        /// <summary>
        /// 隐藏牌,有过程
        /// </summary>
        /// <param name="poker">要隐藏的牌</param>
        /// <param name="to">目标位置</param>
        /// <param name="onFinish">结束时回掉方法</param>
        public void HidePokerWithAnim(GameObject poker,Transform to = null,Action onFinish = null)
        {

            if (poker == null)
                return;

            PokerCard pCard = poker.GetComponent<PokerCard>();
            pCard.SetCardId(0);
            pCard.SetCardFront();

            if(to == null)
            {
                to = HideTran;
            }

            poker.transform.parent = to.transform;

            SpringPosition sp = poker.GetComponent<SpringPosition>();
            sp.target = Vector3.zero;
            sp.enabled = true;
           
                

            TweenAlpha ta = poker.GetComponent<TweenAlpha>();
            ta.from = 1;
            ta.to = 0;
            ta.duration = _runTime;
            ta.ResetToBeginning();
            ta.PlayForward();
            ta.AddOnFinished(onFinish != null
                ? new EventDelegate(() => { onFinish(); })
                : new EventDelegate(() => { Destroy(poker); }));


            TweenRotation tr = poker.GetComponent<TweenRotation>();
            tr.from = new Vector3(0, 0, -4);
            tr.to = Vector3.zero;
            tr.duration = _runTime;
            tr.ResetToBeginning();
            tr.PlayForward();

            TweenScale ts = poker.GetComponent<TweenScale>();
            ts.from = poker.transform.localScale;
            ts.to = Vector3.one*0.3f;
            ts.ResetToBeginning();
            ts.PlayForward();

        }


        public void HidePokerNoAnim(GameObject poker)
        {
            Destroy(poker);
        }

        /// <summary>
        /// 清理掉所有的牌
        /// </summary>
        public void CleanPokers()
        {
            Reset();

            if (_pokerList == null || _pokerList.Count == 0)
                return;
            foreach(GameObject poker in _pokerList)
            {
                if (poker != null)
                    Destroy(poker);
            }
            _pokerList.Clear();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            //_seats.Clear();
            _dealPanels.Clear();
            _pokers.Clear();
            HideTran.DestroyChildren();
        }


    }

}
