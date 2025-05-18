using System.Collections.Generic;
using System.Linq;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.duifen
{
    /// <summary>
    /// 下注区域的扑克管理
    /// </summary>
    public class BetPoker : MonoBehaviour
    {

        /// <summary>
        /// 玩家手中的牌
        /// </summary>
        [HideInInspector]
        public List<PokerCard> PlayerPokers = new List<PokerCard>();

        public int PokerCount
        {
            get
            {
                if (PlayerPokers == null)
                    return 0;

                return PlayerPokers.Count;
            }
        }


        /// <summary>
        /// 直接显示牌的面值
        /// </summary>
        /// <param name="cardVal"></param>
        /// <param name="index"></param>
        public void SetPokerValue(int cardVal, int index)
        {
            if (index >= PlayerPokers.Count)
                return;

            PokerCard card = PlayerPokers[index];
            if (card == null)
                return;
            card.SetCardId(cardVal);
            card.SetCardFront();
        }



        /// <summary>
        /// 弃牌后,将所有手牌扣下
        /// </summary>
        public void FoldTurnCards()
        {
            foreach (var poker in PlayerPokers)
            {
                if (poker.Id != 0)
                {
                    poker.SetDiPaiMark(false);
                    poker.SetCardId(0);
                    poker.TurnCard();
                }
            }
        }



        private readonly PvList _pvList = new PvList();

        public void Reset()
        {
            PlayerPokers.Clear();
            _pvList.Clear();
        }


        internal void AddPoker(PokerCard pokerCard)
        {
            PlayerPokers.Add(pokerCard);
            AddPokerVal(pokerCard.Value);
        }


        internal void AddPokerVal(int val)
        {
            if (val == 0)
                return;

            _pvList.Add(val);
        }


        /// <summary>
        /// 手牌点数(只读)
        /// </summary>
        public int HandPokerPoint
        {
            get
            {
                //返回手牌分数
                return
                    (from t in _pvList
                        let count = t.Count
                        let val = t.Val
                        select val*count + 10*(count < 2 ? 0 : count > 3 ? 5 : count)).Sum();
            }
        }

        /// <summary>
        /// 是否有炸弹
        /// </summary>
        public bool HaveBomb
        {
            get
            {
                if (_pvList != null && _pvList.Count > 1)
                {
                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (int i = 0; i < _pvList.Count; i++)
                    {
                        if (_pvList[i].Count >= 4)
                        {
                            YxDebug.Log(" === player have a Bomb , bomb is " + _pvList[i].Val + " === ");
                            return true;
                        }
                    }
                }
                return false;
            }
        }

      

        public class PvItem
        {
            public int Val;
            public int Count;

            public PvItem(int pokerVal)
            {
                Val = pokerVal;
                Count = 1;
            }
        }

        public class PvList : List<PvItem>
        {

            public void Add(int cardVal)
            {
                if (cardVal == 1 || cardVal == 14)
                    cardVal = 15;

                for (int i = 0; i < Count; i++)
                {
                    if (this[i].Val == cardVal)
                    {
                        this[i].Count++;
                        return;
                    }
                }
                PvItem item = new PvItem(cardVal);
                Add(item);
            }

        }

        internal PokerCard GetPokerCard(int cardIndex)
        {
            if(PlayerPokers.Count <= cardIndex)
            {
                return null;
            }
            return PlayerPokers[cardIndex];
        }
    }
}