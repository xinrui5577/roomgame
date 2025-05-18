using UnityEngine;

namespace Assets.Scripts.Game.BlackJackCs
{
    public class BankerPanel : BjPlayerPanel
    {

        [SerializeField]
        protected Transform CardPos0;

        protected override void OnAwake()
        {
            base.OnAwake();
            CardsId = new int[7];
        }

        public void ShowBankerCard0(int cardId)
        {

            PokerCard poker = CardPos0.GetComponentInChildren<PokerCard>();
            CardsId[0] = cardId;

            if (poker != null)
            {
                poker.SetCardId(cardId);
                poker.TurnCard();
            }

        }

        public override void CheckCardPoint()
        {

            SetHandCardPoint();

            if (HandCardPoint == 0)
                return;

            if (HandCardPoint > 21)
            {
                StateMark.ShowLostMark();
                ShowPoint(HandCardPoint);
                return;
            }


            //当玩家有A且没有超过21点时,可能出现两个数字
            if (HaveAce)
            {
                if (HandCardPoint + 10 == 21)
                {
                    if (OnesPokerCount == 2)
                    {
                        StateMark.ShowBalckJackMark();
                    }

                    ShowPoint(21);
                }
                else if (HandCardPoint + 10 < 21)
                {
                    ShowPoint(HandCardPoint + 10);
                }
                else
                {
                    ShowPoint(HandCardPoint);
                }
            }
            else
            {
                ShowPoint(HandCardPoint);
            }
          
        }
    }
}