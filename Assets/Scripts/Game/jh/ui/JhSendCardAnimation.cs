using System.Collections;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.jh.ui
{
    public class JhSendCardAnimation : MonoBehaviour
    {

        public float FlyTime;

        public float IntervalTime;

        public Vector3 StartPos;

        public EventDelegate OnOneFinish;

        public EventDelegate OnFinish;
        public void OnSendCard(List<List<JhCard>> cards,int start)
        {
            List<List<Vector3>> vec = new List<List<Vector3>>();
            foreach (List<JhCard> jhCards in cards)
            {
                List<Vector3> v = new List<Vector3>();
                foreach (JhCard card in jhCards)
                {
                    v.Add(card.transform.position);
                    YxDebug.LogError(" position " + card.transform.position);
                    card.transform.position = StartPos;
                    YxDebug.LogError(" position after" + card.transform.position);
                    card.transform.localScale = new Vector3(0.5f,0.5f,1);
                }
                vec.Add(v);
            }
            StartCoroutine(CardSend(cards, vec, start));

        }

        protected IEnumerator CardSend(List<List<JhCard>> cards,List<List<Vector3>> posList, int start)
        {
            int cnt = cards.Count;
            for (int index = 0; index < cards[0].Count; index++)
            {
                int loop = start;
                while (true)
                {
                    JhCard card = cards[loop][index];
                    card.StartCoroutine(CardMove(card, StartPos, posList[loop][index]));
                    yield return new WaitForSeconds(IntervalTime);
                    loop = (loop + 1) % cnt;
                    if (loop == start)
                    {
                        break;
                    }

                }
            }
            

            yield return new WaitForSeconds(FlyTime);

            if (OnFinish != null)
            {
                OnFinish.Execute();
            }
        } 

        protected IEnumerator CardMove(JhCard card,Vector3 start,Vector3 end)
        {
            float time = 0;
            while (true)
            {
                time += Time.deltaTime;

                if (time > FlyTime)
                {
                    
                    card.transform.position = end;
                    card.transform.localScale = Vector3.one;
//                    if (OnOneFinish != null)
//                    {
//                        OnOneFinish.parameters[0] = new EventDelegate.Parameter(card);
//                        OnOneFinish.Execute();
//                    }
                    break;
                }

                card.transform.position = Vector3.Lerp(start, end, time / FlyTime);
                card.transform.localScale = new Vector3(0.5f + 0.5f * time / FlyTime, 0.5f + 0.5f * time / FlyTime,1);
                yield return 1;
            }
        }

        public void Reset()
        {
            StopAllCoroutines();
        }
    }
}
