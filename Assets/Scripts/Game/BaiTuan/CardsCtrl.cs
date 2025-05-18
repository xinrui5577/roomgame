using System;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.BaiTuan
{
    public class CardsCtrl : MonoBehaviour
    {

        public GameObject[] Target;
        public GameObject[] Points;
        public GameObject ClonedCards;
        protected int _index;
        private int _chipdepth = 2;
        protected ISFSArray _cards = new SFSArray();
        private int[] _cardv = new int[4];
        public EffectCtrl EffectCtrl;
        public int FirstCardsNum = 8;

        public int[] Pg;
        /// <summary>
        /// 庄家每门的输赢
        /// </summary>
        internal int[] Bpg;



        public void GiveCardsOnFrist(ISFSObject gameInfo)
        {
            if (!gameInfo.ContainsKey("rollResult"))
                return;
            _chipdepth = 2;
            var data = gameInfo.GetSFSObject("rollResult");
            var CardsValues = data.GetSFSArray("cards");
            int temp = 0;
            for (int i = 0; i < FirstCardsNum; i++)
            {
                var go = Instantiate(ClonedCards);
                go.transform.parent = Target[temp].transform;
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;
                var values = CardsValues.GetIntArray(temp);
                go.GetComponent<UISprite>().depth = _chipdepth;

                go.GetComponent<UISprite>().spriteName = "0x" + values[i % 2].ToString("X");
                if (i % 2 != 0)
                {
                    go.GetComponent<UISprite>().depth = _chipdepth - 1;
                    go.transform.localPosition = new Vector3(-32, 0, 0);
                    temp++;
                }
                App.GetGameData<BtwGameData>().CurrentCardList.Add(go);
            }
            for (int i = 0; i < 4; i++)
            {
                Points[i].GetComponent<UILabel>().text = "" + GetCardsValue(CardsValues.GetIntArray(i)) + "点";
                if (GetCardsValue(CardsValues.GetIntArray(i)) == 0)
                    Points[i].GetComponent<UILabel>().text = "弊十";
                Points[i].SetActive(true);
                _cardv[i] = GetCardsValue(CardsValues.GetIntArray(i));
                if (i == 3)
                {
                    CheckEffect();
                    DeskEffect();
                    App.GetGameManager<BtwGameManager>().ResultListCtrl.AddResult(_cardv);
                }
            }
        }

        public virtual void BeginGiveCards(ISFSObject responseData)
        {
            _index = 0;
            _chipdepth = 2;
            _cards = responseData.GetSFSArray("cards");
            InvokeRepeating("GiveCards", 0.5f, 0.2f);
            CancelInvoke("AnimationShip");
            CancelInvoke("AnimationTank");
            CancelInvoke("AnimationPlane");
            ShowGameObject(Plane, false);
            ShowGameObject(Tank, false);
            ShowGameObject(Ship, false);
            Bpg = new[] { 0, 0, 0 };
            Pg = new[] { 0, 0, 0 };
            if (responseData.ContainsKey("bpg"))
                Bpg = responseData.GetIntArray("bpg");
            if (responseData.ContainsKey("pg"))
                Pg = responseData.GetIntArray("pg");
        }

        public void ReceiveResult(ISFSObject responseData)
        {

            if (responseData.ContainsKey("total"))
            {
                App.GameData.GetPlayerInfo().CoinA = responseData.GetLong("total");
            }
            if (responseData.ContainsKey("pg"))
            {
                Pg = new[] { 0, 0, 0, 0 };
                Pg = responseData.GetIntArray("pg");
            }
            if (responseData.ContainsKey("bpg"))
            {
                Bpg = new[] { 0, 0, 0, 0 };
                Bpg = responseData.GetIntArray("bpg");
            }
        }

        public void ShowCards3()
        {
            Facade.Instance<MusicManager>().Play("Open");
            int[] cards = _cards.GetIntArray(3);
            var gdata = App.GetGameData<BtwGameData>();
            if (gdata.TopCardArray == null || gdata.TopCardArray.Length < 2)
                return;
            gdata.TopCardArray[0].GetComponent<UISprite>().spriteName = "0x" + cards[0].ToString("X");
            gdata.TopCardArray[1].GetComponent<UISprite>().spriteName = "0x" + cards[1].ToString("X");
            _ponitsIndex = 3;
            Invoke("ShowPoints", 0.3f);
        }

        protected int _ponitsIndex;


        protected virtual void ShowPoints()
        {
            var point = GetCardsValue(_cards.GetIntArray(_ponitsIndex));
            Points[_ponitsIndex].GetComponent<UILabel>().text = "" + point + "点";
            Facade.Instance<MusicManager>().Play("dian" + point);
            if (point == 0)
            {
                Points[_ponitsIndex].GetComponent<UILabel>().text = "弊十";
            }
            Points[_ponitsIndex].SetActive(true);
            _cardv[_ponitsIndex] = GetCardsValue(_cards.GetIntArray(_ponitsIndex));
            if (_ponitsIndex == 3)
            {
                CheckEffect();
                DeskEffect();
                App.GetGameManager<BtwGameManager>().ResultListCtrl.AddResult(_cardv);
            }

        }

        private bool _isAll;

        public GameObject Ship;
        public GameObject Tank;
        public GameObject Plane;
        private void CheckEffect()
        {
            if (_cardv[0] > _cardv[3])
            {
                InvokeRepeating("AnimationShip", 0, 0.1f);
            }
            if (_cardv[1] > _cardv[3])
            {
                InvokeRepeating("AnimationTank", 0.01f, 0.1f);
            }
            if (_cardv[2] > _cardv[3])
            {
                InvokeRepeating("AnimationPlane", 0.02f, 0.1f);
            }
            if (EffectCtrl == null) return;
            if (_cardv[3] >= _cardv[2] && _cardv[3] >= _cardv[1] && _cardv[3] >= _cardv[0])
            {
                _isAll = true;
                EffectCtrl.ShowWithData(1);
                return;
            }
            if (_cardv[3] < _cardv[2] && _cardv[3] < _cardv[1] && _cardv[3] < _cardv[0])
            {
                _isAll = true; EffectCtrl.ShowWithData(0);
            }
        }
        private int _index1;
        protected virtual void AnimationShip()
        {
            ShowGameObject(Ship, _index1 % 2 == 0);
            _index1++;
            if (_index1 >= 100)
            {
                _index1 = 0;
            }
        }
        private int _index2;
        protected virtual void AnimationTank()
        {
            ShowGameObject(Tank, _index2 % 2 == 0);
            _index2++;
            if (_index2 >= 100)
            {
                _index2 = 0;
            }
        }
        private int _index3;
        protected virtual void AnimationPlane()
        {
            ShowGameObject(Plane, _index3 % 2 == 0);
            _index3++;
            if (_index3 >= 100)
            {
                _index3 = 0;
            }
        }

        private void ShowGameObject(GameObject go, bool isShow)
        {
            if (go == null)
            {
                return;
            }
            go.SetActive(isShow);
        }

        public void DeskEffect()
        {
            _isAll = false;
        }

        public void ShowCards0()
        {
            Facade.Instance<MusicManager>().Play("Open");
            int[] cards = _cards.GetIntArray(0);
            GameObject[] leftCards = App.GetGameData<BtwGameData>().LeftCardArray;
            if (leftCards == null || leftCards.Length <= 0 || cards == null || cards.Length < 2)
            {
                return;
            }
            if (leftCards[0] != null)
            {
                leftCards[0].GetComponent<UISprite>().spriteName = "0x" + cards[0].ToString("X");
            }
            if (leftCards[1] != null)
            {
                leftCards[1].GetComponent<UISprite>().spriteName = "0x" + cards[1].ToString("X");
            }
            Invoke("ShowCards1", 1.0f);
            _ponitsIndex = 0;
            Invoke("ShowPoints", 0.3f);
        }

        public void ShowCards1()
        {
            Facade.Instance<MusicManager>().Play("Open");
            var cards = _cards.GetIntArray(1);
            var downCards = App.GetGameData<BtwGameData>().DownCardArray;
            if (downCards == null || downCards.Length <= 0 || cards == null || cards.Length < 2)
            {
                return;
            }
            if (downCards[0] != null)
            {
                downCards[0].GetComponent<UISprite>().spriteName = "0x" + cards[0].ToString("X");
            }
            if (downCards[1] != null)
            {
                downCards[1].GetComponent<UISprite>().spriteName = "0x" + cards[1].ToString("X");
            }

            Invoke("ShowCards2", 1.0f);
            _ponitsIndex = 1;
            Invoke("ShowPoints", 0.3f);
        }

        public void ShowCards2()
        {
            Facade.Instance<MusicManager>().Play("Open");
            var cards = _cards.GetIntArray(2);
            var rightCards = App.GetGameData<BtwGameData>().RightCardArray;

            if (rightCards == null || rightCards.Length <= 0 || cards == null || cards.Length < 2)
            {
                return;
            }
            if (rightCards[0] != null)
            {
                rightCards[0].GetComponent<UISprite>().spriteName = "0x" + cards[0].ToString("X");
            }
            if (rightCards[1] != null)
            {
                rightCards[1].GetComponent<UISprite>().spriteName = "0x" + cards[1].ToString("X");
            }

            Invoke("ShowCards3", 1.0f);
            _ponitsIndex = 2;
            Invoke("ShowPoints", 0.3f);
        }

        protected void GiveCards() //发牌
        {
            InstantiateChip(ClonedCards);
        }

        public void OnGiveCardsOver() //发牌结束
        {
            Invoke("ShowCards0", 1);
        }

        private void InstantiateChip(GameObject go)
        {

            if (_index >= 8)
            {
                CancelInvoke("GiveCards");
                OnGiveCardsOver();
                return;
            }
            Facade.Instance<MusicManager>().Play("Card");
            int arrindex;
            GameObject temp = Instantiate(go);
            temp.transform.position = go.transform.position;
            temp.transform.parent = Target[_index % 4].transform;
            temp.transform.localScale = Vector3.one;
            temp.GetComponent<UISprite>().depth = _chipdepth;
            _chipdepth++;
            var sp = temp.GetComponent<SpringPosition>();
            var v = Vector3.zero;
            if (_index < 4)
            {
                v = new Vector3(-32, 0, 0);
                arrindex = 0;
            }
            else
            {
                arrindex = 1;
            }
            switch (_index % 4)
            {
                case 0:
                    App.GetGameData<BtwGameData>().LeftCardArray[arrindex] = temp;
                    break;
                case 1:
                    App.GetGameData<BtwGameData>().DownCardArray[arrindex] = temp;
                    break;
                case 2:
                    App.GetGameData<BtwGameData>().RightCardArray[arrindex] = temp;
                    break;
                case 3:
                    App.GetGameData<BtwGameData>().TopCardArray[arrindex] = temp;
                    break;
            }
            sp.target = v;
            sp.enabled = true;
            App.GetGameData<BtwGameData>().CurrentCardList.Add(temp);
            _index++;
        }

        public void ReSetPonits()
        {
            foreach (GameObject i in Points)
            {
                i.SetActive(false);
            }
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
            int temp = 0;
            foreach (int i in s)
            {
                temp = temp + ExplainCards(i.ToString("X"));
            }
            return temp % 10;
        }
        
    }
}
