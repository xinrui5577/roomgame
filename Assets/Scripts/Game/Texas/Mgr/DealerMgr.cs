using System.Collections.Generic;
using Assets.Scripts.Game.Texas.Main;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.Texas.Mgr
{
    /// <summary>
    /// 发牌类
    /// </summary>
    public class DealerMgr :MonoBehaviour
    { 
        // Update is called once per frame
        protected void Update ()
        {
            SmallDealUpdate();
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
        /// 手牌发牌服务器索引
        /// </summary>
        [HideInInspector]
        public int SmallIndex;
        /// <summary>
        /// 大牌发牌索引
        /// </summary>
        [HideInInspector]
        public int BigIndex;

#region 发手牌
        /// <summary>
        /// 自己的手牌
        /// </summary>
        private int[] _selfPoker;
        /// <summary>
        /// 发手牌结束点 有空座位
        /// </summary>
        private int _smallEnd;

        /// <summary>
        /// 发手牌 small:小盲注 selfPoker:自己手牌
        /// </summary>
        /// <param name="smallD"></param>
        /// <param name="selfPoker"></param>
        public void BeginSmallDeal(int smallD, int[] selfPoker)
        {
            IsSmallDeal = true;
            SmallIndex = smallD;
            _selfPoker = selfPoker;

            _smallEnd = smallD + App.GameData.PlayerList.Length * 2;
        }
        /// <summary>
        /// 发手牌 无过程
        /// </summary>
        /// <param name="serverSeat"></param>
        /// <param name="selfPoker"></param>
        public void SmallDeal(int serverSeat,int[] selfPoker = null)
        {
            var gdata = App.GameData;
            var player = gdata.GetPlayer<PlayerPanel>(serverSeat, true);
            var userBetPoker = player.UserBetPoker;
            var to = userBetPoker.LeftPokerPos.transform;
            var dealPoker = DealOnes(to, selfPoker != null ? selfPoker[0] : 0);
            var poker = dealPoker.GetComponent<PokerCard>();
            userBetPoker.LeftPoker = poker;

            to = userBetPoker.RightPokerPos.transform;
            dealPoker = DealOnes(to, selfPoker != null ? selfPoker[1] : 0, false, 1);
            poker = dealPoker.GetComponent<PokerCard>();
            userBetPoker.RightPoker = poker;

            if (selfPoker != null)
            {
                userBetPoker.LeftPoker.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
                userBetPoker.RightPoker.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
                userBetPoker.LeftPoker.SetCardFront();
                userBetPoker.RightPoker.SetCardFront();
            }
        }
        /// <summary>
        /// 发手牌间隔时间
        /// </summary>
        public float SmallSpace;
        /// <summary>
        /// 手牌计时器
        /// </summary>
        private float _smallCount;
        /// <summary>
        /// 发手牌update
        /// </summary>
        private void SmallDealUpdate()
        {
            _smallCount += Time.deltaTime;
            if (!IsSmallDeal || _smallCount < SmallSpace)
                return;

            _smallCount = 0f;
            SmallDeal();

            SmallIndex++;
            if (SmallIndex >= _smallEnd)
            {
                YxDebug.Log("发手牌结束!!");
                IsSmallDeal = false;
            }
        }

        private void SmallDeal()
        {
			if (SmallIndex < 0) { return; }
            var gdata = App.GameData;
            var allPlayer = gdata.PlayerList;   //准备玩家的列表
            var allPlayerCount = allPlayer.Length;
            var trueIndex = SmallIndex%allPlayerCount;
            var smallPlayer = gdata.GetPlayer<PlayerPanel>(trueIndex, true);
            var smallBetPoker = smallPlayer.UserBetPoker;
            if (smallPlayer.Info != null && smallPlayer.ReadyState)
            {
                var oneOrTwo = SmallIndex - (_smallEnd - allPlayerCount*2) >= allPlayerCount;       //发了第几张牌
                var to = oneOrTwo ? smallBetPoker.RightPokerPos.transform : smallBetPoker.LeftPokerPos.transform;

                var v = 0;
            
                if (trueIndex == App.GameData.SelfSeat)
                {
                    v = oneOrTwo ? _selfPoker[1] : _selfPoker[0];
                }

                GameObject dealPoker = DealOnes(SmallBirth, to, v,false, SmallIndex);//SeatSort[trueIndex].UserIcon.transform);
                Facade.Instance<MusicManager>().Play("dealer");
                YxDebug.Log("发手牌");
                //扑克赋值
                if (!oneOrTwo)
                {
                    smallBetPoker.LeftPoker = dealPoker.GetComponent<PokerCard>();
                }
                else
                {
                    smallBetPoker.RightPoker = dealPoker.GetComponent<PokerCard>();
                }

                dealPoker.GetComponent<TweenPosition>().AddOnFinished(smallBetPoker.ReceiveSmall);
            }
            else
            {
                SmallIndex++;
                if (SmallIndex >= _smallEnd)
                {
                    YxDebug.Log("发手牌结束!!");
                    IsSmallDeal = false;
                    return;
                }
                SmallDeal();
            }
        }

        /// <summary>
        /// 发一张牌 有过程
        /// </summary>
        private GameObject DealOnes(Transform from, Transform to,int cardValue = 0,bool isBig = false,int index = 0)
        {
            var gob = Instantiate(PokerPrefab);
            gob.transform.parent = from;
            gob.transform.localScale = isBig ? new Vector3(0.7f, 0.7f, 0.7f):new Vector3(0.4f, 0.4f, 0.4f);
            gob.transform.localPosition = Vector3.zero;
            var pCard = gob.GetComponent<PokerCard>();
            pCard.SetCardDepth(100 + index * 2);
            gob.transform.parent = to;
            gob.GetComponent<PokerCard>().SetCardId(cardValue);
            var tposition = gob.GetComponent<TweenPosition>();
            tposition.duration = 0.25f;
            tposition.from = gob.transform.localPosition;
            tposition.to = Vector3.zero;
            tposition.ResetToBeginning();
            tposition.PlayForward();

            return gob;
        }
        /// <summary>
        /// 发一张牌 无过程
        /// </summary>
        private GameObject DealOnes(Transform to, int cardValue = 0, bool isBig = false, int index = 0)
        {
            GameObject gob = Instantiate(PokerPrefab);
            gob.transform.parent = to;
            gob.transform.localScale = isBig ? new Vector3(0.7f, 0.7f, 0.7f) : new Vector3(0.4f, 0.4f, 0.4f);
            gob.transform.localScale *= 1/to.localScale.x;
            gob.transform.localPosition = Vector3.zero;
            PokerCard pCard = gob.GetComponent<PokerCard>();
            pCard.SetCardDepth(100 + index * 2);
            gob.GetComponent<PokerCard>().SetCardId(cardValue);

            return gob;
        }


        #endregion

        #region 发公共牌
        /// <summary>
        /// 公共牌 分发完需要清空
        /// </summary>
        private Queue<int> _publicPokers = new Queue<int>();

        /// <summary>
        /// 发公共牌
        /// </summary>
        public void BeginBigDeal(int[] poker)
        {
            IsBigDeal = true;
            BigIndex = 0;
            string testStr = string.Empty;
            foreach (var i in poker)
            {
                testStr += i + " , ";
            }

            Debug.Log(" === length , poker.Length + == " + testStr + " ==== " );

            foreach (int i in poker)
            {
                _publicPokers.Enqueue(i);
            }


        }

        public void BeginBigDeal(int poker)
        {
            _publicPokers.Clear();
            IsBigDeal = true;
            _publicPokers.Enqueue(poker);
            BigIndex = 0;
        }
        /// <summary>
        /// 发公共牌 无过程
        /// </summary>
        /// <param name="poker"></param>
        public void BigDeal(int[] poker)
        {
            for (int i = 0; i < poker.Length; i++)
            {
                GameObject dealPoker = DealOnes(App.GetGameManager<TexasGameManager>().PublicPokerPos[i].transform, poker[i], true);
                dealPoker.GetComponent<PokerCard>().SetCardFront();
                App.GetGameManager<TexasGameManager>().PublicPokers.Add(dealPoker.GetComponent<PokerCard>());
            }
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
            BigIndex ++;
            if (_publicPokers.Count == 0)
            {
                YxDebug.Log("发公共牌结束!!");
                IsBigDeal = false;
            }
        }

        private void BigDeal()
        {
            if (_publicPokers.Count <= 0)
            {
                return;
            }
            int v = _publicPokers.Dequeue();
            int index = App.GetGameManager<TexasGameManager>().PublicPokers.Count;
         
            GameObject dealPoker = DealOnes(BigBirth, App.GetGameManager<TexasGameManager>().PublicPokerPos[index].transform, v,true);
            Facade.Instance<MusicManager>().Play("dealer");
            YxDebug.Log("发手牌");
            dealPoker.GetComponent<PokerCard>().SetCardFront();
            App.GetGameManager<TexasGameManager>().PublicPokers.Add(dealPoker.GetComponent<PokerCard>());
        }

        #endregion

        public void Reset()
        {
            IsBigDeal = false;
            IsSmallDeal = false;
            _publicPokers.Clear();
            SmallIndex = 0;
            _smallEnd = 0;
        }

    }

}
