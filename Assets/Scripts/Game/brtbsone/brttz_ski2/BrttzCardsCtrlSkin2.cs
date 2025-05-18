using Sfs2X.Entities.Data;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.brtbsone
{
    public class BrttzCardsCtrlSkin2 : BrttzCardsCtrl
    {
        public float ShowCardWaitTime = 0.5f;
        public float ShowPointWaitTime = 0.05f;
        public ISFSArray MingCardsValue = new SFSArray();

        protected int ShowMingIndex = 0;

        /// <summary>
        /// 接收服务器---发一张明牌
        /// </summary>
        /// <param name="responseData"></param>
        public override void BeginGiveMingCards(ISFSObject responseData)
        {
            MingCardsValue = responseData.GetSFSArray(Parameter.Cards);
           BeginGiveCards();
        }
        //接受发牌
        public override void BeginGiveCards(ISFSObject responseData)
        {
            if (IsRejoin)
            {
                IsRejoin = false;
                return;
            }
            CardsValue = responseData.GetSFSArray(Parameter.Cards);
            ShowIndex = 1;
            CardsArrindex = 3;
            ShowCards();
        }

        public override void ShowCards()
        {
            if (CardsValue == null) return;
            StartCoroutine("ShowCard");
        }

        protected IEnumerator ShowCard()
        {
            var wait = new WaitForSeconds(ShowCardWaitTime);
            for (int i = 0; i < 4; i++)
            {
                var value = CardsValue.GetSFSObject(i).GetIntArray(Parameter.Cards);
                var go = Cards[CardsArrindex];
                var mj = go.GetComponent<MjCard>();
                mj.CardValue = value[ShowIndex];
                mj.TurnCard();
                CardsArrindex += 2;
                CardsArrindex = CardsArrindex >= 8 ? CardsArrindex % 8 : CardsArrindex;
                yield return new WaitForSeconds(ShowPointWaitTime);
                if (HistoryCards.ContainsKey(value[ShowIndex]))
                {
                    HistoryCards[value[ShowIndex]]++;
                    ShowNum = value[ShowIndex] - 1;
                    ShowTarget = HistoryCards[value[ShowIndex]];
                    HistoryCardsCtrl.RefrshDataOnPlay(ShowNum, ShowTarget);
                }
                ShowPoints(i);
                yield return wait;
            }
        }

        public override void SendMingCardFirst(ISFSObject gameInfo)
        {
            if (!gameInfo.ContainsKey(Parameter.RollResult))
                return;
            var data = gameInfo.GetSFSObject(Parameter.RollResult);
            MingCardsValue = data.GetSFSArray(Parameter.Cards);
            int temp = 0;
            int cardIndex = 2;
            for (int i = 0; i < FirstCardsNum; i++)
            {
                cardIndex = cardIndex >= 8 ? cardIndex % 8 : cardIndex;
                var go = Cards[cardIndex];
                go.transform.parent = Target[temp].GetComponent<BrttzCardPostion>().TargetPositions[i % 2];
                go.transform.localScale = Vector3.one * CardsBgScaleNums[1];
                go.transform.localPosition = Vector3.zero;
                if (i % 2 != 0) temp++;
                cardIndex++;
            }
            CardsArrindex = 2;
            ShowMingCards(true);

        }
        protected void ShowMingCards(bool isFrist = false)
        {
            for (int i = 0; i < 4; i++)
            {
                var value = MingCardsValue.GetSFSObject(i).GetIntArray(Parameter.Cards);
                var go = Cards[CardsArrindex];
                var mj = go.GetComponent<MjCard>();
                mj.CardValue = value[ShowMingIndex];
                mj.TurnCard(isFrist);
                CardsArrindex += 2;
                CardsArrindex = CardsArrindex >= 8 ? CardsArrindex % 8 : CardsArrindex;
                if (HistoryCards.ContainsKey(value[ShowMingIndex]))
                {
                    HistoryCards[value[ShowMingIndex]]++;
                    ShowNum = value[ShowMingIndex] - 1;
                    ShowTarget = HistoryCards[value[ShowMingIndex]];
                    HistoryCardsCtrl.RefrshDataOnPlay(ShowNum, ShowTarget);
                }
            }
        }

        public override void GetDicesPoints(int[] dices)
        {

        }
        public override void OnGiveCardsOver()
        {
            CardsArrindex = 2;
            ShowMingCards();
        }
    }
}