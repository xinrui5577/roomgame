using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.jh.ui
{
    public class JhCardGroup : MonoBehaviour
    {
        public enum GroupStatus
        {
            Look,
            GiveUp,
            CompareOut
        }

        public List<JhCard> Cards;

        public GameObject CardsStatus;

        public UILabel StatusName;

        public float CardInterval = 80.0f;

        public void SetCardsValue(int[] cardsValue )
        {
            for (int i = 0; i < cardsValue.Length; i++)
            {
                Cards[i].SetValue(cardsValue[i]);
            }
        }

        public void SendCardAnimation()
        {
            TweenPosition pos = GetComponent<TweenPosition>();
            if (pos != null)
            {
                pos.PlayForward();
            }   
        }


        public void SetCardsStatus(GroupStatus status)
        {
            if (CardsStatus != null)
            {
                if (!CardsStatus.activeSelf)
                {
                    CardsStatus.SetActive(true);
                }
            }

            if (StatusName != null)
            {
                switch (status)
                {
                    case GroupStatus.Look:
                        StatusName.text = "已看牌";
                        break;
                    case GroupStatus.GiveUp:
                        StatusName.text = "已弃牌";
                        break;
                    case GroupStatus.CompareOut:
                        StatusName.text = "比牌失败";
                        break;

                }
            }
            
        }

        public void Reset()
        {
            foreach (JhCard card in Cards)
            {
                card.ShowBack();
                card.StopAllCoroutines();
                card.transform.localScale = Vector3.one;
            }
            if (CardsStatus != null)
            {
                CardsStatus.SetActive(false);
            }
            ResetCardsPos();
        }

        public void ResetCardsPos()
        {
            for (int i = 0; i < Cards.Count; i++)
            {
                Cards[i].transform.localPosition = new Vector3(i * CardInterval,0,0);
            }
        }
    }
}
