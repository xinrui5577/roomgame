using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Texas
{
    /// <summary>
    /// 下注区域的扑克管理
    /// </summary>
    public class BetPoker : MonoBehaviour
    {

        /// <summary>
        /// 左侧的扑克位置
        /// </summary>
        public GameObject LeftPokerPos;
        /// <summary>
        /// 右侧的扑克位置
        /// </summary>
        public GameObject RightPokerPos;
        /// <summary>
        /// 第一张扑克
        /// </summary>
        [HideInInspector]
        public PokerCard LeftPoker = null;
        /// <summary>
        /// 第二张扑克
        /// </summary>
        [HideInInspector]
        public PokerCard RightPoker = null;
        /// <summary>
        /// 用户数据
        /// </summary>
        public PlayerPanel PlayerP;
        /// <summary>
        /// 牌型显示
        /// </summary>
        public UISprite PokerType;
        

        /// <summary>
        /// 翻牌
        /// </summary>
        public void TurnOverCard()
        {
            RightPoker.TurnCard();

            LeftPoker.TurnCard();
        }
        /// <summary>
        /// 接收手牌
        /// </summary>
        public void ReceiveSmall()
        {
            if (PlayerP.Info == null)
            {
                YxDebug.LogError("用户信息为空!");
                return;
            }

            PokerCard poker = RightPoker ?? LeftPoker;

            if (PlayerP.Info.Seat == App.GameData.SelfSeat)
            {
                //自己手牌处理
                poker.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
                poker.SetCardFront();
            }
        }
  
        /// <summary>
        /// 关闭对象
        /// </summary>
        public void CloseGameObject(GameObject gob)
        {
            gob.SetActive(false);
        }

        public void SetCardsValue(int[] cardsVal)
        {
            SetCard(LeftPoker, cardsVal[0]);
            SetCard(RightPoker, cardsVal[1]);
        }

        void SetCard(PokerCard card, int cardVal)
        {
            if (card == null) return;
            card.SetCardId(cardVal);
            card.TurnCard();
        }

        public void Reset()
        {
            if (LeftPoker != null)
            {
                Destroy(LeftPoker.gameObject);
                LeftPoker = null;
            }
            if (RightPoker != null)
            {
                Destroy(RightPoker.gameObject);
                RightPoker = null;
            }
        }

        public void SelectCards(int[] cards,Color unselectedColor)
        {
            foreach (int cardValue in cards)
            {
                SelectCard(LeftPoker, cardValue);
                SelectCard(RightPoker, cardValue);
            }
            SetUnselectedColor(LeftPoker,unselectedColor);
            SetUnselectedColor(RightPoker, unselectedColor);
        }

        private void SetUnselectedColor(PokerCard poker, Color unselectedColor)
        {
            if (poker == null || poker.IsSelect) return;
            poker.SetColor(unselectedColor);
        }

        void SelectCard(PokerCard poker, int selectedVal)
        {
            if (poker == null) return;
            if (selectedVal == poker.Id)
                poker.Selected();
        }
    }
 
}