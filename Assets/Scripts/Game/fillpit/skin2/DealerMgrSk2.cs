using Assets.Scripts.Game.fillpit.Mgr;
using Assets.Scripts.Game.fillpit.skin1;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.fillpit.skin2
{
    public class DealerMgrSk2 : DealerMgr
    {
        protected override void BigDeal()
        {
            var gdata = App.GetGameData<FillpitGameData>();
            var gmgr = App.GetGameManager<FillpitGameManagerSk1>();
            var card = DealCards.Dequeue();

            Transform toPos = gdata.GetPlayer<PlayerPanel>(card.Seat, true).PokersTrans[card.Index];

            GameObject dealPoker = DealOnes(BigBirth, toPos, card.PokerVal, true, card.Index);

            if (PublicCardId > 0)
            {
                dealPoker.GetComponent<PokerCard>().SetPublicMarkActive(card.PokerVal == PublicCardId);
            }

            Facade.Instance<MusicManager>().Play("dealer");
            gmgr.PublicPokers.Add(dealPoker.GetComponent<PokerCard>());
        }


        /// <summary>
        /// 发一张牌 有过程
        /// </summary>
        protected override GameObject DealOnes(Transform from, Transform to, int cardValue = 0, bool isBig = false, int index = 0)
        {
            GameObject gob = Instantiate(PokerPrefab);
            var gobTran = gob.transform;
            gobTran.parent = from;
            gobTran.localPosition = Vector3.zero;
            gobTran.parent = to;
            gobTran.localScale = isBig ? Vector3.one * SelfCardScale : Vector3.one;
            var betPoker = gobTran.GetComponentInParent<BetPoker>();
            betPoker.MoveCards(index);

            PokerCard pCard = gob.GetComponent<PokerCard>();
            pCard.SetCardDepth(100 + index * 2);
            pCard.SetCardId(cardValue);

            var comp = gob.GetComponent<TweenPosition>();
            comp.duration = 0.15f;//0.25f;
            comp.from = gob.transform.localPosition;
            comp.to = Vector3.zero;
            comp.ResetToBeginning();
            comp.PlayForward();

            comp.onFinished.Add(new EventDelegate
                (() =>
                {
                    pCard.TurnCard();
                }));

            return gob;
        }

        public override void FastDeal()
        {
            if (DealCards == null || DealCards.Count < 1) return;
            IsBigDeal = false;
            while (DealCards.Count > 0)
            {
                var card = DealCards.Dequeue();
                var panel = App.GetGameData<FillpitGameData>().GetPlayer<PlayerPanel>(card.Seat, true);
                int index = card.Index;
                var to = panel.PokersTrans[index];
                panel.UserBetPoker.MoveCardsNoAnim(index);
                DealOnes(to, card.PokerVal, index);
            }
        }

        /// <summary>
        /// 发某人的手牌,无过程
        /// </summary>
        /// <param name="pokerValues"></param>
        /// <param name="panel"></param>
        /// <param name="seatIndex"></param>
        public override void DealOnesPokers(int[] pokerValues, PlayerPanel panel, int seatIndex)
        {
            base.DealOnesPokers(pokerValues, panel, seatIndex);
            int len = pokerValues.Length;
            if (len <= 0)
            {
                return;
            }
            panel.UserBetPoker.MoveCardsNoAnim(len - 1);
        }
    }
}
