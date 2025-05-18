using System;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.bjlb
{
    public class CardsCtrl : MonoBehaviour
    {

        public GameObject[] Target;
        public GameObject ClonedCards;
        private int _index;
        private int _zIndex = -1;
        private ISFSArray _cards = new SFSArray();
        private int[][] _cardsArr = new int[2][];
        private float _interval = 0.3f;
        public UILabel[] Points;
        public void BeginGiveCards(ISFSObject responseData)
        {
            //todo 清理桌面
            _cards = responseData.GetSFSArray("cards");
            _cardsArr[0] = _cards.GetIntArray(0);
            _cardsArr[1] = _cards.GetIntArray(1);
            InvokeRepeating("GiveCards", _interval, _interval);
        }


        private void GiveCards()
        {
            if (_index >= 4)
            {
                _index = 0;
                _zIndex = -1;
                FanPai();
                CancelInvoke("GiveCards");
                return;
            }
            if (_index % 2 == 0)
            {
                _zIndex ++;
            }
            GameObject temp = InstantiateCards(ClonedCards, Target[_index]);
            temp.GetComponent<SpringPosition>().target = Target[_index].transform.position;
            temp.GetComponent<UISprite>().spriteName = _zIndex == 0 ? "0x" + _cardsArr[_index % 2][_zIndex].ToString("X") : "PokerBackTexture";
            _index ++;
        }

        private void FanPai()
        {
            Invoke("Step1", _interval);
            Invoke("Step2", _interval * 2);
            Invoke("Step3", _interval * 3);
            Invoke("Step4", _interval * 4 );
        }

        private void Step1()
        {
            CancelInvoke("Step1");
            Facade.Instance<MusicManager>().Play("Open");
            var cards = App.GetGameData<BjlGameData>().Cards;
            if (cards == null || cards.Count < 2) return;
            if (cards[2] == null) return;
            cards[2].GetComponent<UISprite>().spriteName = "0x" + _cardsArr[0][1].ToString("X");
        }

        private void Step2()
        {
            CancelInvoke("Step2");
            Points[0].gameObject.SetActive(true);
        
            valInts[0] = GetCardsValue(_cardsArr[0]);
            Points[0].text =  valInts[0]+"点";
            Points[0].ProcessText();
            
        }

        private void Step3()
        {
            CancelInvoke("Step3");
            Facade.Instance<MusicManager>().Play("Open");
            var cards = App.GetGameData<BjlGameData>().Cards;
            if (cards == null || cards.Count < 3) return;
            cards[3].GetComponent<UISprite>().spriteName = "0x" + _cardsArr[1][1].ToString("X");
        }
        private void Step4()
        {
            CancelInvoke("Step4");
            Points[1].gameObject.SetActive(true);
            valInts[1] = GetCardsValue(_cardsArr[1]);
            Points[1].text = valInts[1] +"点";
            Points[1].ProcessText();
            AddResult();
        }

        private void AddResult()
        {
            var cfg = App.GetGameData<BjlGameData>().TrendConfig;
            cfg.AddTrend(valInts[0] - valInts[1]);
            var view = cfg.TrendView;
            if (view == null) return;
            view.UpdateView();
        }


        private int[] valInts = new int[2];

        private GameObject InstantiateCards(GameObject go,GameObject p)
        {
            Facade.Instance<MusicManager>().Play("Card");
            var temp = Instantiate(go);
            temp.transform.parent = p.transform;
            temp.transform.position = go.transform.position;
            temp.transform.localScale = Vector3.one;
            temp.SetActive(true);
            App.GetGameData<BjlGameData>().Cards.Add(temp);
            return temp;
        }

        public void ReSet()
        {
            Points[0].gameObject.SetActive(false);
            Points[1].gameObject.SetActive(false);
        }
        private int ExplainCards(string s)
        {
            if (s.IndexOfAny(new[] { 'A', 'B', 'C', 'D', 'E', 'F' }) != -1)
            {

                return 0;
            }
            return Convert.ToInt32(s.Substring(1, 1));

        }

        private int GetCardsValue(int[] s)
        {
            var temp = 0;
            foreach (var i in s)
            {
                temp = temp + ExplainCards(i.ToString("X"));
            }
            return temp % 10;
        }

        public virtual void Reset()
        {
            CancelInvoke();
        }
    }
}
