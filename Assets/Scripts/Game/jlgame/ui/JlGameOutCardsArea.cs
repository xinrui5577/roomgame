using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.jlgame.ui
{
    public class JlGameOutCardsArea : MonoBehaviour
    {
        public List<JlGameCardItem> OutCards;
        private float _flyTime = 0.6f;

        public void FromHandToOutArea(JlGameCardItem jlGameCardItem,bool rejoin=false)
        {
            var posTemp = transform.localPosition;
            jlGameCardItem.transform.parent = transform;
            Vector3 scaleTemp = Vector3.one * 0.5f;
            _flyTime = rejoin ? 0 : 0.4f;

            posTemp.x = (jlGameCardItem.Card.Value - 1) * 54.8f;
            posTemp.y = 0;
            if (OutCards.Count != 0)
            {
                if (jlGameCardItem.Value < OutCards[0].Value)
                {
                    OutCards[0].SelectShade.gameObject.SetActive(OutCards.Count > 1);
                    OutCards.Insert(0, jlGameCardItem);
                }
                if (jlGameCardItem.Value > OutCards[OutCards.Count - 1].Value)
                {
                    OutCards[OutCards.Count - 1].SelectShade.gameObject.SetActive(OutCards.Count > 1);
                   
                    OutCards.Add(jlGameCardItem);
                }
            }
            else
            {
                OutCards.Add(jlGameCardItem);
            }

            var depth = 0;
            switch (jlGameCardItem.Card.Colour)
            {
                case 4:
                    depth += 40;
                    break;
                case 2:
                    depth += 30;
                    break;
                case 3:
                    depth += 20;
                    break;
                case 1:
                    depth += 10;
                    break;
            }
            jlGameCardItem.SetCardDepth(jlGameCardItem.Card.Id + depth);
            jlGameCardItem.GetComponent<BoxCollider>().enabled = false;
            TweenPosition.Begin(jlGameCardItem.gameObject, _flyTime, posTemp);
            TweenScale.Begin(jlGameCardItem.gameObject, _flyTime, scaleTemp);
        }

        public void ChangeBlack()
        {
            OutCards[0].SelectShade.gameObject.SetActive(true);
            OutCards[OutCards.Count - 1].SelectShade.gameObject.SetActive(true);
        }

        public void Reset()
        {
            while (transform.childCount>0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }

            OutCards.Clear();
        }
    }
}
