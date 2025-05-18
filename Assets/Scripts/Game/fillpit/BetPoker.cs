using com.yxixia.utile.YxDebug;
using UnityEngine;
#pragma warning disable 649

namespace Assets.Scripts.Game.fillpit
{
    /// <summary>
    /// 下注区域的扑克管理
    /// </summary>
    public class BetPoker : MonoBehaviour
    {
        
        /// <summary>
        /// 玩家盖牌
        /// </summary>
        private int[] _selfPokerValues;

        public Vector3[] DealCardMoveArray;


        /// <summary>
        /// 不能看牌
        /// </summary>
        public bool CantShowPoint
        {
            get { return _selfPokerValues == null || _selfPokerValues.Length <= 0; }
        }

        /// <summary>
        /// 存储手牌数据
        /// </summary>
        /// <param name="cards">手牌值得数组,只取前两个元素</param>
        public void SetHandPokersValue(int[] cards)
        {
            if (cards == null || cards.Length <= 0)
                return;

            _selfPokerValues = cards;
        }


       /// <summary>
       /// 设置牌值
       /// </summary>
       /// <param name="real">设置牌的真实值</param>
        public void SetPokerValue(bool real)
        {
            if (CantShowPoint)
                return;
            for (int i = 0; i < _selfPokerValues.Length; i++)
            {
                var go = transform.GetChild(i);
                if (go == null) continue;
                var card = go.GetComponentInChildren<PokerCard>();
                if (card == null) continue;
                card.SetCardId(real ? _selfPokerValues[i] : 0);
            }
        }

        void ShowSelfPokerFront()
        {
            if (CantShowPoint)
                return;
            for (int i = 0; i < _selfPokerValues.Length; i++)
            {
                var go = transform.GetChild(i);
                if (go == null) continue;
                var card = go.GetComponentInChildren<PokerCard>();
                if (card == null) continue;
                card.SetCardFront();
            }
        }
        
        /// <summary>
        /// 是否展示手牌
        /// </summary>
        public void ShowPokerValue()
        {
            SetPokerValue(true);
            ShowSelfPokerFront();
        }

        public void HidePokerValue()
        {
            SetPokerValue(false);
            ShowSelfPokerFront();
        }

        /// <summary>
        /// 翻牌
        /// </summary>
        public void TurnOverCard(int[] cards)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                var card = transform.GetChild(i).GetComponentInChildren<PokerCard>();
                if (card == null)
                    continue;

                int cardVal = cards[i];
                if (card.Id <= 0)
                {
                    card.SetCardId(cardVal);
                    if (cardVal == 0) continue;
                    card.TurnCard();
                }
                else
                {
                    card.SetCardId(cardVal);
                    card.SetCardFront();
                }
            }
        }
   
        /// <summary>
        /// 关闭对象
        /// </summary>
        public void CloseGameObject(GameObject gob)
        {
            gob.SetActive(false);
        }
        

        public void Reset()
        {
            _selfPokerValues = null;

            if (DealCardMoveArray != null && DealCardMoveArray.Length > 0)
            {
                transform.localPosition = DealCardMoveArray[0];
            }
        }


        /// <summary>
        /// 移动位置手牌,有过程
        /// </summary>
        /// <param name="index"></param>
        public void MoveCards(int index)
        {
            var tp = GetComponent<TweenPosition>();
            if (tp == null) return;

            tp.from = transform.localPosition;
            tp.to = DealCardMoveArray[index];
            tp.ResetToBeginning();
            tp.PlayForward();
        }

        
        /// <summary>
        /// 移动位置手牌,无过程
        /// </summary>
        /// <param name="index"></param>
        public void MoveCardsNoAnim(int index)
        {
            if (DealCardMoveArray == null) return;
            int len = DealCardMoveArray.Length;
            if (index >= len) return;
            transform.localPosition = DealCardMoveArray[index];
        }
    }
   
}